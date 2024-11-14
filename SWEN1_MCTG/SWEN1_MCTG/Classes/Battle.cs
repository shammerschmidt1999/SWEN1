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
        public void BattleRound(Card card1, Card card2, List<ICard> deck1, List<ICard> deck2)
        {

            double card1Damage;
            double card2Damage;

            card1Damage = CalculateDamage(card1, card2, card1.Damage);
            card2Damage = CalculateDamage(card2, card1, card2.Damage);

        }

        public double CalculateDamage(Card card1, Card card2, double card1Damage)
        {
            double calculatedDamage = card1Damage;

            if (card1 is SpellCard && card2 is SpellCard)
                calculatedDamage = CalculateSpellVsSpellDamage((SpellCard)card1, card2.ElementType, card1Damage);

            if (card1 is SpellCard && card2 is MonsterCard)
                calculatedDamage = CalculateSpellVsMonsterDamage((SpellCard)card1, (MonsterCard)card2, card1Damage);

            if (card1 is MonsterCard && card2 is MonsterCard)
                calculatedDamage = CalculateMonsterVsMonsterDamage((MonsterCard)card1, (MonsterCard)card2, card1Damage);

            return calculatedDamage;
        }

        public double CalculateSpellVsSpellDamage(SpellCard card1, GlobalEnums.ElementType card2ElementType, double card1Damage)
        {
            if (card1.ElementType == GlobalEnums.ElementType.Water && card2ElementType == GlobalEnums.ElementType.Fire)
                return card1Damage * 2;

            if (card1.ElementType == GlobalEnums.ElementType.Fire && card2ElementType == GlobalEnums.ElementType.Normal)
                return card1Damage * 2;

            if (card1.ElementType == GlobalEnums.ElementType.Normal && card2ElementType == GlobalEnums.ElementType.Water)
                return card1Damage * 2;

            if (card1.ElementType == GlobalEnums.ElementType.Fire && card2ElementType == GlobalEnums.ElementType.Water)
                return card1Damage / 2;

            if (card1.ElementType == GlobalEnums.ElementType.Normal && card2ElementType == GlobalEnums.ElementType.Fire)
                return card1Damage / 2;

            if (card1.ElementType == GlobalEnums.ElementType.Water && card2ElementType == GlobalEnums.ElementType.Normal)
                return card1Damage / 2;

            return card1Damage;
        }

        public double CalculateSpellVsMonsterDamage(SpellCard card1, MonsterCard card2, double card1Damage)
        {
            if (card2.MonsterType == GlobalEnums.MonsterType.Kraken)
                return 0;

            if (card2.MonsterType == GlobalEnums.MonsterType.Knight && card1.ElementType == GlobalEnums.ElementType.Water)
                return 99999;

            return CalculateSpellVsSpellDamage(card1, card2.ElementType, card1Damage);
        }

        public double CalculateMonsterVsMonsterDamage(MonsterCard card1, MonsterCard card2, double card1Damage)
        {
            if (card1.MonsterType == GlobalEnums.MonsterType.Goblin && card2.MonsterType == GlobalEnums.MonsterType.Dragon)
                return 0;

            if (card1.MonsterType == GlobalEnums.MonsterType.Ork && card2.MonsterType == GlobalEnums.MonsterType.Wizard)
                return 0;

            if (card1.MonsterType == GlobalEnums.MonsterType.Dragon && card2.MonsterType == GlobalEnums.MonsterType.FireElve)
                return 0;

            return card1Damage;

        }

    }
}
