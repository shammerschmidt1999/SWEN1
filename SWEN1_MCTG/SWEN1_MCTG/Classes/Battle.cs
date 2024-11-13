using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    public class Battle : IBattle
    {
        // Constructor
        public Battle(User player1, User player2) 
        { 
            _player1 = player1;
            _player2 = player2;
            _battleLog = new List<string>();
            _roundCount = 0;
            _maxRounds = 100;
        }

        // Fields
        private User _player1;
        private User _player2;
        private int _roundCount;
        private int _maxRounds;
        private List<string> _battleLog;

        // Properties
        public User Player1
        {
            get => _player1;
            set => _player1 = value;
        }

        public User Player2
        { 
            get => _player2;
            set => _player2 = value;
        }
        public int RoundCount
        {
            get => _roundCount;
            set => _roundCount = value;
        }

        public List<string> BattleLog
        {
            get => _battleLog;
            set => _battleLog = value;
        }

        public int MaxRounds
        {
            get => _maxRounds;
        }

        // Methods
        private void BattleRound(ICard card1, ICard card2, List<ICard> deck1, List<ICard> deck2)
        {
            int card1Damage = CalculateDamage(card1, card2);
            int card2Damage = CalculateDamage(card2, card1);
        }

        private int CalculateDamage(ICard card1, ICard card2)
        {
            return 0;
        }

    }
}
