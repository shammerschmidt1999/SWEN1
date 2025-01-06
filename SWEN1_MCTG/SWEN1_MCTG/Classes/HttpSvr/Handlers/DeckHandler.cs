using System.Text.Json.Nodes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers;

public class DeckHandler : Handler, IHandler
{
    private readonly string _connectionString;
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStackRepository _stackRepository;

    public DeckHandler()
    {
        _connectionString = AppSettings.GetConnectionString("TestConnection");
        _cardRepository = new CardRepository(_connectionString);
        _userRepository = new UserRepository(_connectionString);
        _stackRepository = new StackRepository(_connectionString);
    }


    /// <summary>
    /// Handles package purchase
    /// </summary>
    /// <param name="e"> Console arguments </param>
    /// <returns> bool if request was successful or not </returns>
    public override bool Handle(HttpSvrEventArgs e)
    {
        if ((e.Path.TrimEnd('/', ' ', '\t') == "/deck") && (e.Method == "GET"))
        {
            return _DisplayDeck(e);
        }
        else if ((e.Path.TrimEnd('/', ' ', '\t') == "/deck") && (e.Method == "PUT"))
        {
            return _EditDeck(e);
        }
        return false;
    }

    public bool _DisplayDeck(HttpSvrEventArgs e)
    {
        JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            (bool Success, User? User) ses = Token.Authenticate(e);

            if (ses.Success)
            {
                User user = _userRepository.GetByUsername(ses.User!.Username);
                Stack userCards = _stackRepository.GetByUserId(ses.User!.Id);

                // Format the message as a JSON array
                JsonArray cardsArray = _generateDeckArray(userCards);

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

    private JsonArray _generateDeckArray(Stack userCards)
    {

        JsonArray cardsArray = new JsonArray();

        foreach (var card in userCards.Cards)
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

    public bool _EditDeck(HttpSvrEventArgs e)
    {
        JsonObject? reply = new JsonObject() { ["success"] = false, ["message"] = "Invalid request." };
        int status = HttpStatusCode.BAD_REQUEST;

        try
        {
            (bool Success, User? User) ses = Token.Authenticate(e);

            if (ses.Success)
            {
                User user = _userRepository.GetByUsername(ses.User!.Username);
                Stack userCards = _stackRepository.GetByUserId(ses.User!.Id);

                // Parse the JSON payload
                JsonArray cardNames = JsonNode.Parse(e.Payload)?.AsArray() ?? new JsonArray();

                foreach (var cardNameNode in cardNames)
                {
                    string cardName = cardNameNode.ToString();
                    var cards = userCards.Cards.Where(c => c.Name == cardName).ToList();

                    foreach (var card in cards)
                    {
                        if (card.InDeck)
                        {
                            card.SetInDeck(false);
                        }
                        else
                        {
                            card.SetInDeck(true);
                        }
                        _stackRepository.SetCardInDeck(card.InDeck, card.Id, user.Id);
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


}