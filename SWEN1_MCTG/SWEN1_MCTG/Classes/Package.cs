using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Package class for user to buy
    public class Package : IPackage
    {
        // Fields
        private int _price = 5;
        private List<Card> _cards = new List<Card>();

        // Properties
        public int Price
        {
            get => _price;
            set => _price = value;
        }

        public List<Card> Cards
        {
            get => _cards;
            set => _cards = value;
        }

        // Methods
        public void AddCard(Card card)
        {
            _cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }
    }
}