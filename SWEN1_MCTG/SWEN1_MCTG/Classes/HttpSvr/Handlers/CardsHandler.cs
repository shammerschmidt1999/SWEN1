using System.Text.Json.Nodes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers;

    public class CardsHandler : Handler, IHandler
    {
        private readonly string _connectionString;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStackRepository _stackRepository;

        public CardsHandler()
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
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/cards") && (e.Method == "GET"))
            {
                return _DisplayCards(e);
            }
            return false;
        }

    public bool _DisplayCards(HttpSvrEventArgs e)
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
                JsonArray cardsArray = _generateCardArray(userCards);

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

    private JsonArray _generateCardArray(Stack userCards)
    {

        JsonArray cardsArray = new JsonArray();

        foreach (var card in userCards.Cards)
        {
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

}