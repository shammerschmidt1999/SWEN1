using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Classes.Exceptions;

namespace SWEN1_MCTG.Classes
{
    // Represents a user in game
    public sealed class User : IUser
    {
        // Used temporarily to store users in memory
        private static Dictionary<string, User> _Users = new Dictionary<string, User>();

        // Constructor
        public User()
        {
        }

        public User(string username, string password)
        {
            _username = username;
            _password = password;
            _userCards = new Stack();
            _userDeck = new Stack();
            _userHand = new Stack();
            _userDiscard = new Stack();
            _userCoinPurse = new CoinPurse();
            _elo = 1000;
        }

        // Fields
        private int _id;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private int _elo = 1000;
        private Stack _userCards = new Stack(); // All current cards
        private Stack _userDeck = new Stack(); // Cards in played deck
        private Stack _userHand = new Stack(); // Cards in hand
        private Stack _userDiscard = new Stack(); // Discarded cards
        private CoinPurse _userCoinPurse = new CoinPurse();

        // Properties
        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public Stack UserCards
        {
            get => _userCards;
            set => _userCards = value;
        }

        public Stack UserDiscard
        {
            get => _userDiscard;
            private set => _userDiscard = value;
        }

        public CoinPurse UserCoinPurse
        {
            get => _userCoinPurse;
            private set => _userCoinPurse = value;
        }

        public int Elo
        {
            get => _elo;
            set => _elo = value;
        }

        public Stack UserDeck
        {
            get => _userDeck;
            private set => _userDeck = value;
        }

        public Stack UserHand
        {
            get => _userHand;
            private set => _userHand = value;
        }

        // Methods
        /// <summary>
        /// Prints User Information
        /// </summary>
        public void PrintUser()
        {
            Console.WriteLine("Username: " + _username);
            Console.WriteLine("Password: " + _password);
            Console.WriteLine("Coins: " + _userCoinPurse.GetCoinsValue());
            PrintStack(_userCards);
        }

        /// <summary>
        /// Prints information of chosen User stack (e.g. Cards, Hand,...)
        /// </summary>
        /// <param name="stack"> The Stack of Cards that should be printed </param>
        public void PrintStack(Stack stack)
        {
            stack.PrintStack();
        }

        public static void Create(string username, string password)
        {
            User user = new();
            {
                user._username = username;
                user._password = password;
                user._userCards = new Stack();
                user._userDeck = new Stack();
                user._userHand = new Stack();
                user._userDiscard = new Stack();
                user._userCoinPurse = new CoinPurse();
                user._elo = 1000;
            }
            _Users.Add(user.Username, user);
        }

        /// <summary>
        /// Compares the given username and password with the stored ones, creates Token on success
        /// </summary>
        /// <param name="username"> Provided username </param>
        /// <param name="password"> Provided password </param>
        /// <returns> Success status and created token </returns>
        public static (bool Success, string Token) Logon(string username, string password)
        {
            if (_Users.ContainsKey(username) && _Users[username].Password == password)
            {
                return (true, Token._CreateTokenFor(_Users[username]));
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// Changes user data if the token is valid
        /// </summary>
        /// <param name="token"> Token string </param>
        /// <exception cref="UserException"> Exception on wrong token </exception>
        public void Save(string token)
        {
            (bool Success, User? User) auth = Token.Authenticate(token);
            if (auth.Success)
            {
                if (auth.User!.Username != _username)
                {
                    throw new UserException("Trying to change other user's data.");
                }

            }
            else
            {
                throw new UserException("Authentication failed");
            }
        }

        /// <summary>
        /// Gets a user object by username
        /// </summary>
        /// <param name="userName"> Username to search for </param>
        /// <returns> User object with provided username </returns>
        public static User? Get(string userName)
        {
            _Users.TryGetValue(userName, out User? user);
            return user;
        }

        /// <summary>
        /// Checks if user exists in User list
        /// </summary>
        /// <param name="userName"> Username to search for </param>
        /// <returns> True if user exists, else false </returns>
        public static bool Exists(string userName)
        {
            return _Users.ContainsKey(userName);
        }

        // Wrapper method to add a coin to the user's CoinPurse
        public void AddCoin(Coin newCoin)
        {
            _userCoinPurse.AddCoin(newCoin);
        }

        // Wrapper method to remove a coin from the user's CoinPurse
        public void RemoveCoin(Coin coinToRemove)
        {
            _userCoinPurse.RemoveCoin(coinToRemove);
        }

        // Wrapper method to get the total value of coins
        public int GetTotalCoinValue()
        {
            return _userCoinPurse.GetCoinsValue();
        }

        // Wrapper method to print the coins
        public void PrintCoins()
        {
            _userCoinPurse.PrintCoins();
        }

        // Wrapper method to add a card to the user's stack
        public void AddCard(Card newCard, Stack stack)
        {
            stack.AddCardToStack(newCard);
        }

        // Wrapper method to remove a card from the user's stack
        public void RemoveCard(string cardName, Stack stack)
        {
            stack.RemoveCardFromStack(cardName);
        }

        // Wrapper method to get a card from the user's stack
        public Card GetCard(string cardName, Stack stack)
        {
            return stack.GetCardFromStack(cardName);
        }

    }
}