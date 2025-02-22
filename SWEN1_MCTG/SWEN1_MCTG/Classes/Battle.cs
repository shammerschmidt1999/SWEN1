﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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
            _roundCount = 0;
            _maxRounds = 100;
            _battleLog = new JsonArray();
        }

        // Fields
        private readonly User _player1;
        private readonly User _player2;
        private int _roundCount;
        private readonly int _maxRounds;
        private JsonArray _battleLog;

        public JsonArray BattleLog
        {
            get => _battleLog;
            set => _battleLog = value;
        }

        // Methods
        public RoundResults StartBattle()
        {
            // First log entry for battle start
            LogBattleStart(_player1, _player2);

            // Battle until max rounds or one player has no cards left
            while (
                    _roundCount < _maxRounds
                   && _player1.UserDeck.Cards.Count > 0
                   && _player2.UserDeck.Cards.Count > 0
                   )
            {
                _roundCount++; // Increment round count
                BattleRound(); // Start a battle round
            }

            // Return result of the battle
            return DetermineWinner();
        }

        /// <summary>
        /// Represents one battle round
        /// </summary>
        private void BattleRound()
        {
            // Get random cards from both players
            Card card1 = _player1.UserDeck.GetRandomCardFromStack();
            Card card2 = _player2.UserDeck.GetRandomCardFromStack();

            // Declare winner name, default is draw
            string winnerName = "Draw";

            // Calculate the damage of both cards against each other
            double damage1 = CalculateDamage(card1, card2, card1.Damage);
            double damage2 = CalculateDamage(card2, card1, card2.Damage);

            // Compare the damage of both cards
            RoundResults result = CompareDamage(damage1, damage2);

            if (result == RoundResults.Victory)
            {
                _player1.UserDeck.AddCardToStack(card2); // Add losers card to winners deck
                _player2.UserDeck.Cards.Remove(card2); // Remove losers card from losers deck
                winnerName = _player1.Username; // Set winner name
            }
            else if (result == RoundResults.Defeat)
            {
                _player2.UserDeck.AddCardToStack(card1); // Add losers card to winners deck
                _player1.UserDeck.Cards.Remove(card1); // Remove losers card from losers deck
                winnerName = _player2.Username; // Set winner name
            }

            // Log the battle round
            LogBattleRound(_roundCount, _player1, _player2, card1, card2, damage1, damage2, winnerName);
        }

        /// <summary>
        /// Logs the battle round
        /// </summary>
        /// <param name="roundNumber"> Current round number </param>
        /// <param name="player1"> Player 1 user entity </param>
        /// <param name="player2"> Player 2 user entity </param>
        /// <param name="card1"> Player 1 card entity </param>
        /// <param name="card2"> Player 2 card entity </param>
        /// <param name="damage1"> Damage after rules of card 1 </param>
        /// <param name="damage2"> Damage after rules of card 2</param>
        /// <param name="winnerName"> Name of the player with higher damage, or "Draw" </param>
        private void LogBattleRound(int roundNumber, User player1, User player2, Card card1, Card card2, double damage1, double damage2, string winnerName)
        {
            JsonObject battleLogEntry = new JsonObject()
            {
                ["RoundNumber"] = roundNumber,
                ["Player1"] = player1.Username,
                ["Player1 CardAmount"] = player1.UserDeck.Cards.Count,
                ["Player2"] = player2.Username,
                ["Player2 CardAmount"] = player2.UserDeck.Cards.Count,
                ["Player1Card"] = new JsonObject()
                {
                    ["Name"] = card1.Name,
                    ["ElementType"] = card1.ElementType.ToString(),
                    ["Damage"] = card1.Damage,
                    ["Damage after Rules"] = damage1
                },
                ["Player2Card"] = new JsonObject()
                {
                    ["Name"] = card2.Name,
                    ["ElementType"] = card2.ElementType.ToString(),
                    ["Damage"] = card2.Damage,
                    ["Damage after Rules"] = damage2
                },
                ["RoundWinner"] = winnerName
            };

            _battleLog.Add(battleLogEntry);
        }

        /// <summary>
        /// Logs the start of the battle
        /// </summary>
        /// <param name="player1"> Player 1 entity </param>
        /// <param name="player2"> Player 2 entity </param>
        private void LogBattleStart(User player1, User player2)
        {
            JsonObject battleLogEntry = new JsonObject()
            {
                ["RoundNumber"] = "Battle Start",
                ["Player1"] = player1.Username,
                ["Player1 CardAmount"] = player1.UserDeck.Cards.Count,
                ["Player2"] = player2.Username,
                ["Player2 CardAmount"] = player2.UserDeck.Cards.Count,
            };

            _battleLog.Add(battleLogEntry);
        }

        /// <summary>
        /// Compares the calculated damage of two cards
        /// </summary>
        /// <param name="player1Damage"> Damage of player 1 </param>
        /// <param name="player2Damage"> Damage of player 2 </param>
        /// <returns> RoundResult Enum Value </returns>
        private RoundResults CompareDamage(double player1Damage, double player2Damage)
        {
            if (player1Damage > player2Damage)
                return RoundResults.Victory;

            if (player1Damage < player2Damage)
                return RoundResults.Defeat;

            return RoundResults.Draw;
        }

        /// <summary>
        /// Calculates the damage of a card against another card
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <param name="card1Damage"></param>
        /// <returns> The damage after rules are applied </returns>
        private double CalculateDamage(Card card1, Card card2, double card1Damage)
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
        private double CalculateSpellVsSpellDamage(SpellCard card1, ElementType card2ElementType, double card1Damage)
        {
            if (card1.ElementType == ElementType.Water && card2ElementType == ElementType.Fire)
                return card1Damage * 2;

            if (card1.ElementType == ElementType.Fire && card2ElementType == ElementType.Normal)
                return card1Damage * 2;

            if (card1.ElementType == ElementType.Normal && card2ElementType == ElementType.Water)
                return card1Damage * 2;

            if (card1.ElementType == ElementType.Fire && card2ElementType == ElementType.Water)
                return card1Damage / 2;

            if (card1.ElementType == ElementType.Normal && card2ElementType == ElementType.Fire)
                return card1Damage / 2;

            if (card1.ElementType == ElementType.Water && card2ElementType == ElementType.Normal)
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
        private double CalculateSpellVsMonsterDamage(SpellCard card1, MonsterCard card2, double card1Damage)
        {
            if (card2.MonsterType == MonsterType.Kraken)
                return 0;

            if (card2.MonsterType == MonsterType.Knight && card1.ElementType == ElementType.Water)
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
        private double CalculateMonsterVsMonsterDamage(MonsterCard card1, MonsterCard card2, double card1Damage)
        {
            if (card1.MonsterType == MonsterType.Goblin && card2.MonsterType == MonsterType.Dragon)
                return 0;

            if (card1.MonsterType == MonsterType.Ork && card2.MonsterType == MonsterType.Wizard)
                return 0;

            if (card1.MonsterType == MonsterType.Dragon && card2.MonsterType == MonsterType.FireElve)
                return 0;

            return card1Damage;
        }

        /// <summary>
        /// Determines the winner of the battle
        /// </summary>
        /// <returns> RoundResult enum variable representing result </returns>
        private RoundResults DetermineWinner()
        {
            RoundResults result;

            if (_player1.UserDeck.Cards.Count > _player2.UserDeck.Cards.Count)
            {
                result = RoundResults.Victory;
            }
            else if (_player1.UserDeck.Cards.Count < _player2.UserDeck.Cards.Count)
            {
                result = RoundResults.Defeat;
            }
            else
            {
                result = RoundResults.Draw;
            }

            return result;
        }

        // Used for testing purposes
        public double CalculateDamageTest(Card card1, Card card2, double card1Damage)
        {
            return CalculateDamage(card1, card2, card1Damage);
        }

        public RoundResults CompareDamageTest(double player1Damage, double player2Damage)
        {
            return CompareDamage(player1Damage, player2Damage);
        }
    }
}
