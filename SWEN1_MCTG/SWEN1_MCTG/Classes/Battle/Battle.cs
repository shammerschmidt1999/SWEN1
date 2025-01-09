using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.Battle
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
        public async Task<GlobalEnums.RoundResults> StartBattleAsync()
        {
            while (_roundCount < _maxRounds && _player1.UserDeck.Cards.Count > 0 && _player2.UserDeck.Cards.Count > 0)
            {
                _roundCount++;
                BattleRound();
            }

            return DetermineWinner();
        }

        private void BattleRound()
        {

            StartingMessage();

            Card card1 = _player1.UserDeck.GetRandomCardFromStack();
            Card card2 = _player2.UserDeck.GetRandomCardFromStack();
            string winnerName = "Draw";

            double damage1 = CalculateDamage(card1, card2, card1.Damage);
            double damage2 = CalculateDamage(card2, card1, card2.Damage);

            GlobalEnums.RoundResults result = CompareDamage(damage1, damage2);

            if (result == GlobalEnums.RoundResults.Victory)
            {
                _player1.UserDeck.AddCardToStack(card2);
                _player2.UserDeck.Cards.Remove(card2);
            }
            else if (result == GlobalEnums.RoundResults.Defeat)
            {
                _player2.UserDeck.AddCardToStack(card1);
                _player1.UserDeck.Cards.Remove(card1);
            }

            // Add formatted log
            _battleLog.Add($"--- Round {_roundCount} ---");
            _battleLog.Add($"{_player1.Username} ({card1.Name}) [{damage1}] vs {_player2.Username} ({card2.Name}) [{damage2}]");
            _battleLog.Add($"Round Winner: {winnerName}");
            _battleLog.Add("");
        }

        /// <summary>
        /// Compares the calculated damage of two cards
        /// </summary>
        /// <param name="player1Damage"> Damage of player 1 </param>
        /// <param name="player2Damage"> Damage of player 2 </param>
        /// <returns> RoundResult Enum Value </returns>
        public GlobalEnums.RoundResults CompareDamage(double player1Damage, double player2Damage)
        {
            if (player1Damage > player2Damage)
                return GlobalEnums.RoundResults.Victory;

            if (player1Damage < player2Damage)
                return GlobalEnums.RoundResults.Defeat;

            return GlobalEnums.RoundResults.Draw;
        }

        /// <summary>
        /// Calculates the damage of a card against another card
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <param name="card1Damage"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculates the damage of a spell card against another spell card
        /// </summary>
        /// <param name="card1"> Card that needs damage calculation </param>
        /// <param name="card2ElementType"> Type of element the first card plays against </param>
        /// <param name="card1Damage"> Damage of card1 </param>
        /// <returns> The damage with consideration of element synergies </returns>
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
        /// <summary>
        /// Calculates the damage of a spell card against a monster card
        /// </summary>
        /// <param name="card1"> Card that needs damage calculation </param>
        /// <param name="card2"> Card that the first card plays against </param>
        /// <param name="card1Damage"> Damage of card1 </param>
        /// <returns> The damage with consideration of element synergies and monster synergies </returns>
        public double CalculateSpellVsMonsterDamage(SpellCard card1, MonsterCard card2, double card1Damage)
        {
            if (card2.MonsterType == GlobalEnums.MonsterType.Kraken)
                return 0;

            if (card2.MonsterType == GlobalEnums.MonsterType.Knight && card1.ElementType == GlobalEnums.ElementType.Water)
                return 99999;

            return CalculateSpellVsSpellDamage(card1, card2.ElementType, card1Damage);
        }

        /// <summary>
        /// Calculates the damage of a monster card against another monster card
        /// </summary>
        /// <param name="card1"> Card that needs damage calculation </param>
        /// <param name="card2"> Card that the first card plays against </param>
        /// <param name="card1Damage"> Damage of card1 </param>
        /// <returns> The damage with consideration of monster synergies </returns>
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

        private GlobalEnums.RoundResults DetermineWinner()
        {
            GlobalEnums.RoundResults result;

            if (_player1.UserDeck.Cards.Count > _player2.UserDeck.Cards.Count)
            {
                result = GlobalEnums.RoundResults.Victory;
            }
            else if (_player1.UserDeck.Cards.Count < _player2.UserDeck.Cards.Count)
            {
                result = GlobalEnums.RoundResults.Defeat;
            }
            else
            {
                result = GlobalEnums.RoundResults.Draw;
            }

            return result;
        }

        private void StartingMessage()
        {
            _battleLog.Add($"Battle between {_player1.Username} and {_player2.Username} starts now...");
            _battleLog.Add($"{_player1.Username}'s deck: ");

            foreach (Card card in _player1.UserDeck.Cards)
            {
                _battleLog.Add($"{card.Name}");
            }

            _battleLog.Add($"{_player2.Username}'s deck: ");

            foreach (Card card in _player2.UserDeck.Cards)
            {
                _battleLog.Add($"{card.Name}");
            }
        }
    }
}
