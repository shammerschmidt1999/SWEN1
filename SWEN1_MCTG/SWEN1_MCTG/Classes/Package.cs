using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Package class for user to buy
    public class Package : IPackage
    {
        // Fields
        private int _price;
        private GlobalEnums.PackageType _packageType;
        private int _amountOfCards;
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

        public GlobalEnums.PackageType PackageType
        {
            get => _packageType;
            set => _packageType = value;
        }

        public int AmountOfCards
        {
            get => _amountOfCards;
            set => _amountOfCards = value;
        }

        // Methods
        /// <summary>
        /// Adds a card to the package
        /// </summary>
        /// <param name="card"> The card to be added </param>
        public void AddCard(Card card)
        {
            _cards.Add(card);
        }

        /// <summary>
        /// Removes from the package
        /// </summary>
        /// <param name="card"> The card to be removed </param>
        public void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }
    }
}