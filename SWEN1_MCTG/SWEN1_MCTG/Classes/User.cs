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
        private User()
        {
        }

        // Fields
        private string _username = string.Empty;
        private string _password = string.Empty;
        private int _elo = 1000;
        private Stack _userCards = new Stack(); // All current cards
        private Stack _userDeck = new Stack(); // Cards in played deck
        private Stack _userHand = new Stack(); // Cards in hand
        private Stack _userDiscard = new Stack(); // Discarded cards
        private CoinPurse _userCoinPurse = new CoinPurse();

        // Properties
        public string Username
        {
            get => _username;
            private set => _username = value;
        }

        public string Password
        {
            get => _password;
            private set => _password = value;
        }

        public Stack UserCards
        {
            get => _userCards;
            private set => _userCards = value;
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
            private set => _elo = value;
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
            if (_Users.ContainsKey(username))
            {
                throw new UserException("Username already exists");
            }

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

        public static (bool Success, string Token) Logon(string username, string password)
        {
            if (_Users.ContainsKey(username) && _Users[username].Password == password)
            {
                return (true, Token._CreateTokenFor(_Users[username]));
            }

            return (false, string.Empty);
        }

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

        public static User? Get(string userName)
        {
            _Users.TryGetValue(userName, out User? user);
            return user;
        }

        public static void ClearList()
        {
            _Users.Clear();
        }
    }
}