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
            _userStack = new Stack();
            _userCoinPurse = new CoinPurse();
            _elo = 0;
        }

        // Fields
        private string _username;
        private string _password;
        private int _elo;
        private Stack _userStack;
        private Stack _userDeck;
        private Stack _userHand;
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

        public Stack UserStack
        {
            get => _userStack;
            private set => _userStack = value;
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
        public void PrintUser()
        {
            Console.WriteLine("Username: " + _username);
            Console.WriteLine("Password: " + _password);
            Console.WriteLine("Coins: " + _userCoinPurse.GetCoinsValue());
            PrintStack();
        }

        public void PrintStack()
        {
            _userStack.PrintStack();
        }
    }
}