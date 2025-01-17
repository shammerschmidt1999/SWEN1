using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Stack class for user to store cards
    public class Stack : IStack
    {
        public Stack()
        {
        }

        public Stack(int userId, List<Card> cards)
        {
            _userId = userId;
            _cards = cards;
        }

        // Fields
        private List<Card> _cards = new List<Card>();
        private int _userId;

        // Properties
        public List<Card> Cards
        {
            get => _cards;
            private set => _cards = value;
        }

        public int UserId
        {
            get => _userId;
            private set => _userId = value;
        }

        // Methods
        /// <summary>
        /// Adds a card to the List of cards
        /// </summary>
        /// <param name="newCard"> The card to be added to the List </param>
        public void AddCardToStack(Card newCard)
        {
            _cards.Add(newCard);
        }

        /// <summary>
        /// Gets one random card from the Stack
        /// </summary>
        /// <returns> One card at a random index between 0 and cards.Count </returns>
        public Card? GetRandomCardFromStack()
        {
            Random rng = new Random();
            int randomIndex = rng.Next(0, _cards.Count);

            return _cards[randomIndex];
        }
    }
}