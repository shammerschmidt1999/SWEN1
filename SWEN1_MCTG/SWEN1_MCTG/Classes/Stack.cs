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
        // Fields
        private List<Card> _cards = new List<Card>();

        // Properties
        public List<Card> Cards
        {
            get => _cards;
            set => _cards = value;
        }

        // Methods
        public void AddCardToStack(Card newCard)
        {
            _cards.Add(newCard);
        }

        public Card GetCardFromStack(string cardName)
        {
            var card = _cards.Find(x => x.Name == cardName);
            if (card == null)
            {
                throw new ArgumentException($"Card with name {cardName} not found in stack.");
            }
            return card;
        }

        public void RemoveCardFromStack(string cardName)
        {
            Card? cardToRemove = _cards.Find(x => x.Name == cardName);
            if (cardToRemove != null)
            {
                _cards.Remove(cardToRemove);
            }
        }

        public void PrintStack()
        {
            foreach (Card card in _cards)
            {
                card.PrintInformation();
            }
        }
    }
}