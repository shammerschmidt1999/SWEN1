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
        private List<Card> Cards
        {
            get;
        } = new List<Card>();

        // Methods
        public void AddCardToStack(Card newCard)
        {
            Cards.Add(newCard);
        }

        public Card GetCardFromStack(string CardName)
        {
            return Cards.Find(x => x.Name == CardName);
        }

        public void RemoveCardFromStack(string CardName)
        {
            Cards.Remove(Cards.Find(x => x.Name == CardName));
        }
    }
}
