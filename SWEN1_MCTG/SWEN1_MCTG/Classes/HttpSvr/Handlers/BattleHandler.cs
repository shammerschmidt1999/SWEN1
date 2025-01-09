using static SWEN1_MCTG.GlobalEnums;
using SWEN1_MCTG.Classes.Battle;
using SWEN1_MCTG.Classes.Exceptions;
using SWEN1_MCTG.Classes.HttpSvr.Handlers;
using SWEN1_MCTG.Classes.HttpSvr;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG;
using System.Text.Json.Nodes;
using System.Text.Json;

public class BattleHandler : Handler, IHandler
{
    private readonly string _connectionString;
    private readonly IStackRepository _stackRepository;
    private readonly IUserRepository _userRepository;
    private static readonly Queue<TaskCompletionSource<(User, User, HttpSvrEventArgs)>> BattleQueue = new();
    private static readonly object BattleQueueLock = new();

    public BattleHandler()
    {
        _connectionString = AppSettings.GetConnectionString("TestConnection");
        _stackRepository = new StackRepository(_connectionString);
        _userRepository = new UserRepository(_connectionString);
    }

    public override async Task<bool> HandleAsync(HttpSvrEventArgs e)
    {
        if (e.Method != "POST" || e.Path != "/battles")
        {
            return false;
        }

        return await _HandleBattleAsync(e);
    }

    public async Task<bool> _HandleBattleAsync(HttpSvrEventArgs e)
    {
        JsonObject reply = new JsonObject() { ["success"] = true, ["message"] = "Waiting for opponent." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            // Authorization
            string? token = e.Headers.FirstOrDefault(h => h.Name == "Authorization")?.Value?.Split(' ').Last();
            if (token == null) throw new UserException("Authorization token is missing");

            User user = await AuthenticateUser(token) ?? throw new UserException("Authorization failed");

            // Enqueue user for battle
            TaskCompletionSource<(User, User, HttpSvrEventArgs)> tcs = new();
            HttpSvrEventArgs? opponentEventArgs = null;

            lock (BattleQueueLock)
            {
                if (BattleQueue.Count > 0)
                {
                    var existingTcs = BattleQueue.Dequeue();
                    var opponent = (ValueTuple<User, HttpSvrEventArgs>)existingTcs.Task.AsyncState!;
                    opponentEventArgs = opponent.Item2;
                    existingTcs.SetResult((opponent.Item1, user, e));
                    tcs.SetResult((user, opponent.Item1, e));
                }
                else
                {
                    tcs = new TaskCompletionSource<(User, User, HttpSvrEventArgs)>((user, e));
                    BattleQueue.Enqueue(tcs);
                    return true;
                }
            }

            var task = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));
            if (task != tcs.Task) throw new TimeoutException("No opponent found within the allowed time.");

            var (player1, player2, player1EventArgs) = await tcs.Task;

            // Load player decks
            List<Card> player1Deck = await _stackRepository.GetUserDeckAsync(player1);
            List<Card> player2Deck = await _stackRepository.GetUserDeckAsync(player2);

            player1.UserDeck = new Stack { Cards = player1Deck };
            player2.UserDeck = new Stack { Cards = player2Deck };

            // Start battle asynchronously
            _ = Task.Run(async () =>
            {
                try
                {
                    var battle = new Battle(player1, player2);
                    RoundResults result = await battle.StartBattleAsync();

                    // Update stats
                    UpdatePlayerStats(player1, result == RoundResults.Victory ? RoundResults.Victory : RoundResults.Defeat);
                    UpdatePlayerStats(player2, result == RoundResults.Defeat ? RoundResults.Victory : RoundResults.Defeat);

                    // Send battle result to both players
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var battleLog = battle.BattleLog;
                    var jsonResponse = JsonSerializer.Serialize(new JsonObject
                    {
                        ["success"] = true,
                        ["message"] = "Battle completed.",
                        ["result"] = result.ToString(),
                        ["battleLog"] = battleLog,
                        ["battleWinner"] = result == RoundResults.Victory ? player1.Username : player2.Username
                    }, options);

                    player1EventArgs.Reply(HttpStatusCode.OK, jsonResponse);
                    opponentEventArgs?.Reply(HttpStatusCode.OK, jsonResponse);

                    // Update player stats in database
                    await _userRepository.UpdateAsync(player1);
                    await _userRepository.UpdateAsync(player2);

                }
                catch (Exception ex)
                {
                    tcs.SetException(ex); // Handle exceptions
                    opponentEventArgs?.Reply(HttpStatusCode.BAD_REQUEST, ex.Message);
                }
            });

            return true;
        }
        catch (Exception ex)
        {
            reply = new JsonObject { ["success"] = false, ["message"] = "Unexpected error: " + ex.Message };
            e.Reply(HttpStatusCode.BAD_REQUEST, JsonSerializer.Serialize(reply));
            return false;
        }
    }

    private void UpdatePlayerStats(User user, GlobalEnums.RoundResults result)
    {
        switch (result)
        {
            case RoundResults.Defeat:
                user.Defeats++;
                user.Elo -= 3;
                break;

            case RoundResults.Draw:
                user.Draws++;
                break;

            case RoundResults.Victory:
                user.Wins++;
                user.Elo += 5;
                break;
        }
    }

    private async Task<User?> AuthenticateUser(string token)
    {
        var authResult = await Token.AuthenticateTokenAsync(token);
        return authResult.Success ? authResult.User : null;
    }
}
