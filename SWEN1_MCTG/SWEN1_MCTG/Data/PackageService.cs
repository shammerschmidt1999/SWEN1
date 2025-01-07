using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;

namespace SWEN1_MCTG.Data;

public class PackageService : IPackageService
{
    private readonly StackRepository _stackRepository;
    private readonly CardRepository _cardRepository;
    private readonly CoinPurseRepository _coinPurseRepository;
    private readonly string _connectionString;
    private readonly Func<List<Card>, int, List<Card>> _cardSelectionStrategy; // Used for testing

    public PackageService(StackRepository stackRepository, CardRepository cardRepository, CoinPurseRepository coinPurseRepository, Func<List<Card>, int, List<Card>> cardSelectionStrategy)
    {

        _stackRepository = stackRepository;
        _cardRepository = cardRepository;
        _coinPurseRepository = coinPurseRepository;
        _cardSelectionStrategy = cardSelectionStrategy ?? DefaultCardSelection; // Used for testing
    }

    public PackageService()
    {
        _connectionString = AppSettings.GetConnectionString("TestConnection");
        _stackRepository = new StackRepository(_connectionString);
        _cardRepository = new CardRepository(_connectionString);
        _coinPurseRepository = new CoinPurseRepository(_connectionString);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="packageType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void PurchasePackage(int userId, GlobalEnums.PackageType packageType)
    {
        // Get corresponding CoinPurse
        CoinPurse coinPurse = _coinPurseRepository.GetByUserId(userId);

        // Get the price of the package
        int packagePrice = (int)packageType;

        // Check if user has enough coins to purchase package
        if (coinPurse.GetCoinsValue() < packagePrice)
        {
            throw new InvalidOperationException("Not enough coins to purchase package");
        }

        // Extract the exact coins needed for the package
        Dictionary<GlobalEnums.CoinType, int> coinsUsed = coinPurse.ExtractCoins(packagePrice);
        if (coinsUsed == null)
        {
            throw new InvalidOperationException("Failed to extract the required coins for the package");
        }

        // Determine the amount of cards to be drawn from the database
        int amountOfCards = GetAmountOfCards(packageType);

        // Determine the amount of decisions the user can make
        int possibleDecisions = packageType == GlobalEnums.PackageType.Legendary ? 6 : 4;

        // Get random cards from the database
        List<Card> randomCards = _cardRepository.GetRandomCards(amountOfCards);

        // Let the user choose the cards
        List<Card> userCardSelection = GetUserCardSelection(randomCards, possibleDecisions);

        /* For testing purposes, the user's card selection is determined by the DefaultCardSelection method.
         * This method reads the user's input to select cards.
        List<Card> userCardSelection = _cardSelectionStrategy(randomCards, possibleDecisions);
        */

        // Add the cards to the users stack
        _stackRepository.AddCardsToUser(userId, userCardSelection);

        // Update the user's coin purse in the repository
        _coinPurseRepository.UpdateCoinPurse(coinPurse);

        Console.WriteLine("Package purchased successfully!");
        Console.WriteLine("Cards added to your personal cards!");
    }

    /// <summary>
    /// Get the amount of cards that should be drawn from the database
    /// </summary>
    /// <param name="packageType"> The type of Package the user wants to purchase </param>
    /// <returns> An integer that represents the size of the package according to the packageType </returns>
    /// <exception cref="ArgumentOutOfRangeException"> In case a non-existing packageType is entered </exception>
    private int GetAmountOfCards(GlobalEnums.PackageType packageType)
    {
        return packageType switch
        {
            GlobalEnums.PackageType.Basic => 5,
            GlobalEnums.PackageType.Premium => 10,
            GlobalEnums.PackageType.Legendary => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(packageType), "Package type not found")
        };
    }

    /// <summary>
    /// Reads the user's input to select cards
    /// </summary>
    /// <param name="randomCards"> A List of cards that are randomly drawn from the database </param>
    /// <param name="cardsToChoose"> The amount of cards a user can select </param>
    /// <returns> A list of cards with the selected cards </returns>
    private List<Card> GetUserCardSelection(List<Card> randomCards, int cardsToChoose)
    {
        Console.WriteLine("Select cards:");
        for (int i = 0; i < randomCards.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {randomCards[i].Name}");
        }

        List<Card> selectedCards = new List<Card>();
        HashSet<int> chosenIndices = new HashSet<int>();

        for (int i = 0; i < cardsToChoose; i++)
        {
            int choice;
            do
            {
                Console.WriteLine($"Choose card {i + 1}:");
                choice = int.Parse(Console.ReadLine()) - 1;

                if (chosenIndices.Contains(choice))
                {
                    Console.WriteLine("You have already chosen this card. Please select a different card.");
                }
            } while (chosenIndices.Contains(choice) || choice < 0 || choice >= randomCards.Count);

            chosenIndices.Add(choice);
            selectedCards.Add(randomCards[choice]);
        }

        return selectedCards;
    }


    // Used for testing purposes
    // Chooses random cards from the drawn cards to simulate user decision
    private List<Card> DefaultCardSelection(List<Card> randomCards, int cardsToChoose)
    {
        Console.WriteLine("Select cards:");
        for (int i = 0; i < randomCards.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {randomCards[i].Name}");
        }

        List<Card> selectedCards = new List<Card>();
        for (int i = 0; i < cardsToChoose; i++)
        {
            Console.WriteLine($"Choose card {i + 1}:");
            int choice = int.Parse(Console.ReadLine());
            selectedCards.Add(randomCards[choice - 1]);
        }

        return selectedCards;
    }
}