using System.Text.Json.Nodes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers;

public class DeckHandler : Handler, IHandler
{
    private readonly string _connectionString;
    private readonly IUserRepository _userRepository;
    private readonly IStackRepository _stackRepository;

    public DeckHandler()
    {
        _connectionString = AppSettings.GetConnectionString("TestConnection");
        _userRepository = new UserRepository(_connectionString);
        _stackRepository = new StackRepository(_connectionString);
    }


    /// <summary>
    /// Handles package purchase
    /// </summary>
    /// <param name="e"> Console arguments </param>
    /// <returns> bool if request was successful or not </returns>
    public override async Task<bool> HandleAsync(HttpSvrEventArgs e)
    {
        if ((e.Path.TrimEnd('/', ' ', '\t') == "/deck") && (e.Method == "GET"))
        {
            return await _DisplayDeckAsync(e);
        }
        else if ((e.Path.TrimEnd('/', ' ', '\t') == "/deck") && (e.Method == "PUT"))
        {
            return await _EditDeckAsync(e);
        }
        return false;
    }

    /// <summary>
    /// Method to display the cards that the user currently has in deck
    /// </summary>
    /// <param name="e"> HttpSvrEventArgs </param>
    /// <returns>TRUE if operation was successful; FALSE if not </returns>
    private async Task<bool> _DisplayDeckAsync(HttpSvrEventArgs e)
    {
        JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

            if (ses.Success)
            {
                User user = await _userRepository.GetByUsernameAsync(ses.User!.Username);
                Stack userCards = await _stackRepository.GetByUserIdAsync(ses.User!.Id);

                // Format the message as a JSON array
                JsonArray cardsArray = await _generateDeckArrayAsync(userCards);

                if (cardsArray.Count == 0)
                {
                    status = HttpStatusCode.NO_CONTENT;
                    reply = new JsonObject()
                    {
                        ["success"] = false,
                        ["message"] = "No cards found."
                    };
                }
                else
                {
                    status = HttpStatusCode.OK;
                    reply = new JsonObject()
                    {
                        ["success"] = true,
                        ["message"] = cardsArray
                    };
                }
            }
            else
            {
                status = HttpStatusCode.UNAUTHORIZED;
                reply = new JsonObject() { ["success"] = false, ["message"] = "Unauthorized." };
            }
        }
        catch (Exception)
        {
            reply = new JsonObject() { ["success"] = false, ["message"] = "Unexpected error." };
        }

        e.Reply(status, reply?.ToJsonString());
        return true;
    }

    /// <summary>
    /// Generates a JSON Array of cards to print
    /// </summary>
    /// <param name="userCards"> The cards of the user </param>
    /// <returns> JSON Array with information of each card </returns>
    private async Task<JsonArray> _generateDeckArrayAsync(Stack userCards)
    {
        JsonArray cardsArray = new JsonArray();

        foreach (Card card in userCards.Cards)
        {
            if (!card.InDeck)
            {
                continue;
            }

            JsonObject cardObject = new JsonObject()
            {
                ["Name"] = card.Name,
                ["ElementType"] = card.ElementType.ToString(),
                ["Damage"] = card.Damage
            };
            if (card is MonsterCard monsterCard)
            {
                cardObject["MonsterType"] = monsterCard.MonsterType.ToString();
            }

            cardObject["inDeck"] = card.InDeck;

            cardsArray.Add(cardObject);
        }

        return cardsArray;
    }

    /// <summary>
    /// Sets cards to be inDeck or not
    /// </summary>
    /// <param name="e"> HttpSvrEventArgs </param>
    /// <returns> TRUE if operation was successful; FALSE if not </returns>
    private async Task<bool> _EditDeckAsync(HttpSvrEventArgs e)
    {
        JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            (bool Success, User? User) ses = await Token.AuthenticateBearerAsync(e);

            if (ses.Success)
            {
                User user = await _userRepository.GetByUsernameAsync(ses.User!.Username);
                Stack userCards = await _stackRepository.GetByUserIdAsync(ses.User!.Id);

                // Parse the JSON payload
                JsonArray cardNames = JsonNode.Parse(e.Payload)?.AsArray() ?? new JsonArray();

                foreach (JsonNode cardNameNode in cardNames)
                {
                    string cardName = cardNameNode.ToString();
                    List<Card> cards = userCards.Cards.Where(c => c.Name == cardName).ToList();

                    foreach (Card card in cards)
                    {
                        if (card.InDeck)
                        {
                            card.SetInDeck(false);
                        }
                        else
                        {
                            // Check if the user has 4 cards in the deck
                            if (UserHasFourCardsInDeck(userCards))
                            {
                                status = HttpStatusCode.BAD_REQUEST;
                                reply = new JsonObject() { ["success"] = false, ["message"] = "Deck is full." };
                                e.Reply(status, reply?.ToJsonString());
                                return true;
                            }
                            card.SetInDeck(true);
                        }
                        await _stackRepository.SetCardInDeckAsync(card.InDeck, card.Id, user.Id);
                    }
                }

                status = HttpStatusCode.OK;
                reply = new JsonObject() { ["success"] = true, ["message"] = "Deck updated successfully." };
            }
            else
            {
                status = HttpStatusCode.UNAUTHORIZED;
                reply = new JsonObject() { ["success"] = false, ["message"] = "Unauthorized." };
            }
        }
        catch (Exception)
        {
            reply = new JsonObject() { ["success"] = false, ["message"] = "Unexpected error." };
        }

        e.Reply(status, reply?.ToJsonString());
        return true;
    }

    private bool UserHasFourCardsInDeck(Stack userCards)
    {
        return userCards.Cards.Count(c => c.InDeck) >= 4;
    }
}