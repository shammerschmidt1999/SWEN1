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

        // Fields
        private List<Card> _cards = new List<Card>();
        private int _userId;

        // Properties
        public List<Card> Cards
        {
            get => _cards;
            set => _cards = value;
        }

        public int UserId
        {
            get => _userId;
            set => _userId = value;
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
        /// Get a card object by name
        /// </summary>
        /// <param name="cardName"> The name of the card object you want to find </param>
        /// <returns> The card with the same name as the parameter in the List </returns>
        /// <exception cref="ArgumentException"></exception>
        public Card GetCardFromStack(string cardName)
        {
            Card card = _cards.Find(x => x.Name == cardName);
            if (card == null)
            {
                throw new ArgumentException($"Card with name {cardName} not found in stack.");
            }
            return card;
        }

        /// <summary>
        /// Removes a card from the List of Cards
        /// </summary>
        /// <param name="cardName"> The name of the Card you want to remove </param>
        public void RemoveCardFromStack(string cardName)
        {
            Card? cardToRemove = _cards.Find(x => x.Name == cardName);
            if (cardToRemove != null)
            {
                _cards.Remove(cardToRemove);
            }
        }
        /// <summary>
        /// Prints Information of each Card in the Stack
        /// </summary>
        public void PrintStack()
        {
            foreach (Card card in _cards)
            {
                card.PrintInformation();
            }
        }
        /// <summary>
        /// Shuffles the Stack randomly
        /// </summary>
        public void ShuffleStack()
        {
            Random rng = new Random();
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
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