using System.Text.Json.Nodes;
using SWEN1_MCTG.Data.Repositories.Classes;
using SWEN1_MCTG.Data.Repositories.Interfaces;
using SWEN1_MCTG.Data;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers;

    public class CardsHandler : Handler, IHandler
    {
        private readonly string _connectionString;
        private readonly IUserRepository _userRepository;
        private readonly IStackRepository _stackRepository;

        public CardsHandler()
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
        public override bool Handle(HttpSvrEventArgs e)
        {
            if ((e.Path.TrimEnd('/', ' ', '\t') == "/cards") && (e.Method == "GET"))
            {
                return _DisplayCards(e);
            }
            return false;
        }

        /// <summary>
        /// Displays information on all the users cards
        /// </summary>
        /// <param name="e"> HTTPEventArgs </param>
        /// <returns> TRUE if operation was successful; FALSE if it was unsuccessful </returns>
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

        /// <summary>
        /// Generates an Array with the information of the users cards
        /// </summary>
        /// <param name="userCards"> The users cards </param>
        /// <returns> A JsonArray with the user cards data </returns>
    private JsonArray _generateCardArray(Stack userCards)
    {

        JsonArray cardsArray = new JsonArray();

        foreach (Card card in userCards.Cards)
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