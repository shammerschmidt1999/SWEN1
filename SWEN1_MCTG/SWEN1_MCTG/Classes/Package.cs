using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    // Package class for user to buy
    public class Package : IPackage
    {
        // Fields
        private int _price;
        private PackageType _packageType;
        private int _amountOfCards;
        private int _possibleDecisions;
        private List<Card> _cards = new List<Card>();

        public Package(PackageType packageType)
        {
            _packageType = packageType;
            _price = (int)packageType;
            _amountOfCards = GetAmountOfCards();
            _possibleDecisions = GetPossibleDecisions();
        }

        // Properties
        public int Price
        {
            get => _price;
            private set => _price = value;
        }

        public List<Card> Cards
        {
            get => _cards;
            private set => _cards = value;
        }

        public PackageType PackageType
        {
            get => _packageType;
            private set => _packageType = value;
        }

        public int AmountOfCards
        {
            get => _amountOfCards;
            private set => _amountOfCards = value;
        }

        public int PossibleDecisions
        {
            get => _possibleDecisions;
            set => _possibleDecisions = value;
        }

        public void AddCards(List<Card> cards)
        {
            _cards.AddRange(cards);
        }

        public int GetAmountOfCards()
        {
            return PackageType switch
            {
                PackageType.Basic => 5,
                PackageType.Premium => 10,
                PackageType.Legendary => 12,
                _ => throw new ArgumentOutOfRangeException(nameof(PackageType), "Package type not found")
            };
        }

        public int GetPossibleDecisions()
        {
            return PackageType == PackageType.Legendary ? 6 : 4;
        }
    }
}