using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using SWEN1_MCTG.Classes.Exceptions;
using Npgsql;
using SWEN1_MCTG.Data.Repositories.Classes;

namespace SWEN1_MCTG.Classes
{
    // Represents a user in game
    public sealed class User : IUser
    {
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
            _userCoinPurse = new CoinPurse();
            _elo = 100;
            _wins = 0;
            _defeats = 0;
            _draws = 0;
        }

        // Fields
        private int _id;
        private string _username;
        private string _password;
        private int _elo;
        private Stack _userCards = new Stack(); // All current cards
        private Stack _userDeck = new Stack(); // Cards in played deck
        private CoinPurse _userCoinPurse = new CoinPurse();
        private int _wins;
        private int _defeats;
        private int _draws;

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

        public int Wins
        {
            get => _wins;
            set => _wins = value;
        }

        public int Defeats
        {
            get => _defeats;
            set => _defeats = value;
        }

        public int Draws
        {
            get => _draws;
            set => _draws = value;
        }


        /// <summary>
        /// Compares the given username and password with the stored ones, creates Token on success
        /// </summary>
        /// <param name="username"> Provided username </param>
        /// <param name="password"> Provided password </param>
        /// <returns> Success status and created token </returns>
        public static async Task<(bool Success, string Token)> LogonAsync(string username, string password)
        {
            UserRepository userRepository = new UserRepository(AppSettings.GetConnectionString("TestConnection"));
            User? user = await userRepository.ValidateCredentialsAsync(username, password);
            if (user != null)
            {
                string token = await Token._CreateTokenForAsync(user);
                return (true, token);
            }
            return (false, string.Empty);
        }
    }
}