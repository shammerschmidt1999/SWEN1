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
        }

        // Fields
        private string _username;
        private string _password;
        private Stack _userStack;
        private CoinPurse _userCoinPurse;

        // Properties
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

        public Stack UserStack
        {
            get => _userStack;
            set => _userStack = value;
        }

        public CoinPurse UserCoinPurse
        {
            get => _userCoinPurse;
            set => _userCoinPurse = value;
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