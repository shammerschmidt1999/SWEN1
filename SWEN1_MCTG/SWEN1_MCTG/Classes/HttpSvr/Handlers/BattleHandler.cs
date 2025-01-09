using SWEN1_MCTG;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Classes.Battle;
using SWEN1_MCTG.Classes.Exceptions;
using SWEN1_MCTG.Classes.HttpSvr;
using System.Text.Json.Nodes;
using SWEN1_MCTG.Classes.HttpSvr.Handlers;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;
using SWEN1_MCTG.Data.Repositories.Classes;
using System.Text;
using Newtonsoft.Json;

public class BattleHandler : Handler, IHandler
{
    private readonly string _connectionString;
    private readonly IStackRepository _stackRepository;
    private readonly IUserRepository _userRepository;
    private static readonly Queue<TaskCompletionSource<(User, User)>> BattleQueue = new();
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

    private async Task<bool> _HandleBattleAsync(HttpSvrEventArgs e)
    {
        JsonObject reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            // Authorization
            string? token = e.Headers.FirstOrDefault(h => h.Name == "Authorization")?.Value?.Split(' ').Last();
            if (token == null) throw new UserException("Authorization token is missing");

            User user = await AuthenticateUser(token) ?? throw new UserException("Authorization failed");

            // Enqueue user for battle
            var tcs = new TaskCompletionSource<(User, User)>();
            lock (BattleQueueLock)
            {
                if (BattleQueue.Count > 0)
                {
                    var existingTcs = BattleQueue.Dequeue();
                    var opponent = existingTcs.Task.AsyncState as User ?? throw new InvalidOperationException("Invalid opponent state");
                    existingTcs.SetResult((opponent, user));
                    tcs.SetResult((user, opponent));
                }
                else
                {
                    tcs = new TaskCompletionSource<(User, User)>(user);
                    BattleQueue.Enqueue(tcs);
                }
            }

            var task = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));
            if (task != tcs.Task) throw new TimeoutException("No opponent found within the allowed time.");

            var (player1, player2) = await tcs.Task;

            // Load player decks
            List<Card> player1Deck = await _stackRepository.GetUserDeckAsync(player1);
            List<Card> player2Deck = await _stackRepository.GetUserDeckAsync(player2);

            player1.UserDeck = new Stack { Cards = player1Deck };
            player2.UserDeck = new Stack { Cards = player2Deck };

            // Start battle
            Battle battle = new Battle(player1, player2);
            RoundResults result = await battle.StartBattleAsync();

            // Update stats and persist to the database based on the battle result
            if (result == RoundResults.Victory)
            {
                // Player 1 is the winner, Player 2 is the loser
                UpdatePlayerStats(player1, RoundResults.Victory);
                UpdatePlayerStats(player2, RoundResults.Defeat);
            }
            else if (result == RoundResults.Defeat)
            {
                // Player 2 is the winner, Player 1 is the loser
                UpdatePlayerStats(player2, RoundResults.Victory);
                UpdatePlayerStats(player1, RoundResults.Defeat);
            }
            else
            {
                // It's a draw, update both players
                UpdatePlayerStats(player1, RoundResults.Draw);
                UpdatePlayerStats(player2, RoundResults.Draw);
            }

            await _userRepository.UpdateAsync(player1);
            await _userRepository.UpdateAsync(player2);

            // Create response
            reply = new JsonObject
            {
                ["success"] = true,
                ["message"] = "Battle completed.",
                ["result"] = result.ToString(),
                ["battleLog"] = JsonConvert.SerializeObject(battle.BattleLog)
            };
            status = HttpStatusCode.OK;
        }
        catch (TimeoutException ex)
        {
            reply = new JsonObject { ["success"] = false, ["message"] = ex.Message };
        }
        catch (UserException ex)
        {
            reply = new JsonObject { ["success"] = false, ["message"] = ex.Message };
        }
        catch (Exception ex)
        {
            reply = new JsonObject { ["success"] = false, ["message"] = "Unexpected error: " + ex.Message };
        }

        e.Reply(status, reply.ToJsonString());
        return true;
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
