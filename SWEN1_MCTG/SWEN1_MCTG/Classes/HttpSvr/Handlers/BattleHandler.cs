using static SWEN1_MCTG.GlobalEnums;
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

    // Queue to manage battle requests
    private static readonly Queue<TaskCompletionSource<(User, User, HttpSvrEventArgs)>> BattleQueue = new();

    // Lock object to ensure thread safety when accessing the queue
    private static readonly object BattleQueueLock = new();

    public BattleHandler()
    {
        _connectionString = AppSettings.GetConnectionString("DefaultConnection");
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
            // Extract token from request
            string? token = e.Headers.FirstOrDefault(h => h.Name == "Authorization")?.Value?.Split(' ').Last();

            if (token == null) 
                throw new UserException("Authorization token is missing");

            // Authenticate user and get user entity
            User user = await AuthenticateUser(token) ?? throw new UserException("Authorization failed");

            // Create a task completion source for the battle
            TaskCompletionSource<(User, User, HttpSvrEventArgs)> tcs = new();

            // Set the HttpSvrEventArgs for the opponent to null
            HttpSvrEventArgs? opponentEventArgs = null;

            // Lock the queue
            lock (BattleQueueLock)
            {
                if (BattleQueue.Count > 0) // If there is a player in the queue
                {
                    var existingTcs = BattleQueue.Dequeue(); // Get the tcs from the first connection
                    var opponent = (ValueTuple<User, HttpSvrEventArgs>)existingTcs.Task.AsyncState!; // Get the user entity and args from the tcs created by the first connection
                    opponentEventArgs = opponent.Item2; // Set the opponentEventArgs (first connection)
                    existingTcs.SetResult((opponent.Item1, user, e)); // Set the TCS for the first connection (First connected user, second connected user, args)
                    tcs.SetResult((user, opponent.Item1, e)); // Set the TCS for the second connection (Second connected user, first connected user, args)
                }
                else // If there is no other player in the queue
                {
                    tcs = new TaskCompletionSource<(User, User, HttpSvrEventArgs)>((user, e)); // Assign the user that connects first and their Args to the tcs
                    BattleQueue.Enqueue(tcs); // Enqueue the tcs with the information
                    return true; // Return true indicating the user is waiting for an opponent
                }
            }

            // Await the task completion source to get the battle participants
            // player1 is the second connected user, player2 is the first connected user, player1EventArgs is the second connected user's args
            var (player1, player2, player1EventArgs) = await tcs.Task;

            // Check if the players are the same user
            if (player1.Id == player2.Id)
            {
                throw new BattleException("You cannot battle yourself.");
            }

            // Load player decks
            List<Card> player1Deck = await _stackRepository.GetUserDeckAsync(player1);
            List<Card> player2Deck = await _stackRepository.GetUserDeckAsync(player2);

            player1.ChangeUserDeck(new Stack(player1.Id, player1Deck));
            player2.ChangeUserDeck(new Stack(player2.Id, player2Deck));

            // Cancel battle if userDeck is empty
            if (player1Deck.Count == 0 || player2Deck.Count == 0)
            {
                throw new BattleException("One of the players has an empty deck.");
            }

            // Start battle in a new thread
            _ = Task.Run(async () =>
            {
                try
                {
                    Battle battle = new Battle(player1, player2);
                    RoundResults result = battle.StartBattle();

                    // Update stats
                    UpdatePlayerStats(player1, result == RoundResults.Victory ? RoundResults.Victory : RoundResults.Defeat);
                    UpdatePlayerStats(player2, result == RoundResults.Defeat ? RoundResults.Victory : RoundResults.Defeat);

                    // Send battle result to both players
                    string jsonResponse = GenerateResponse(result, battle.BattleLog, player1, player2);

                    // Send response to both players
                    player1EventArgs.Reply(HttpStatusCode.OK, jsonResponse); // Reply to the first player (second connected)
                    opponentEventArgs?.Reply(HttpStatusCode.OK, jsonResponse); // Reply to the second player (first connected)

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

    /// <summary>
    /// Method to update the player stats based on the battle result
    /// </summary>
    /// <param name="user"> One of the two players </param>
    /// <param name="result"> Their result </param>
    private void UpdatePlayerStats(User user, RoundResults result)
    {
        user.ApplyBattleResult(result);
    }

    /// <summary>
    /// Authenticates the user with the given token
    /// </summary>
    /// <param name="token"> The users token string </param>
    /// <returns> user entity if successful; null else</returns>
    private async Task<User?> AuthenticateUser(string token)
    {
        var authResult = await Token.AuthenticateTokenAsync(token);
        return authResult.Success ? authResult.User : null;
    }

    /// <summary>
    /// Generates the response for the battle
    /// </summary>
    /// <param name="result"> Result of the battle </param>
    /// <param name="battleLog"> BattleLog JsonArray of the battle </param>
    /// <param name="player1"> Player 1 entity </param>
    /// <param name="player2"> Player 2 entity </param>
    /// <returns> Formatted string with battle information </returns>
    private string GenerateResponse(RoundResults result, JsonArray battleLog, User player1, User player2)
    {
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        string battleWinner = "Draw";

        switch (result)
        {
            case RoundResults.Defeat:
                battleWinner = player2.Username;
                break;
            case RoundResults.Victory:
                battleWinner = player1.Username;
                break;
        }

        string response = JsonSerializer.Serialize(new JsonObject
        {
            ["success"] = true,
            ["message"] = "Battle completed.",
            ["result"] = result.ToString(),
            ["battleLog"] = battleLog,
            ["battleWinner"] = battleWinner
        }, options);

        return response;
    }
}
