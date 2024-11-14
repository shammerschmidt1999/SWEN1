using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Represents a user in game
    public class User : IUser
    {
        // Constructor
        public User(string username, string password)
        {
            _username = username;
            _password = password;
            _userCards = new Stack();
            _userDeck = new Stack();
            _userHand = new Stack();
            _userDiscard = new Stack();
            _userCoinPurse = new CoinPurse();
            _elo = 0;
        }

        // Fields
        private string _username;
        private string _password;
        private int _elo;
        private Stack _userCards; // All current cards
        private Stack _userDeck; // Cards in played deck
        private Stack _userHand; // Cards in hand
        private Stack _userDiscard; // Discarded cards
        private CoinPurse _userCoinPurse;

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
    }
}