using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // Stack class for user to store cards
    public class Stack
    {
        // Fields
        private List<Card> Cards { get; } = [];

        // Methods
        public void AddCardToStack(Card newCard)
        {
            Cards.Add(newCard);
        }

        public Card? GetCardFromStack(string cardName)
        {
            return Cards.Find(x => x.Name == cardName);
        }

        public void RemoveCardFromStack(string cardName)
        {
            Card? cardToRemove = Cards.Find(x => x.Name == cardName);
            if (cardToRemove != null)
            {
                Cards.Remove(cardToRemove);
            }
        }
    }
}
