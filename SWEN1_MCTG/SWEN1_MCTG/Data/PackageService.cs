using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Data.Repositories.Classes;

namespace SWEN1_MCTG.Data;

public class PackageService : IPackageService
{
    private readonly StackRepository _stackRepository;
    private readonly CardRepository _cardRepository;
    private readonly CoinPurseRepository _coinPurseRepository;
    private readonly string _connectionString;

    public PackageService()
    {
        _connectionString = AppSettings.GetConnectionString("TestConnection");
        _stackRepository = new StackRepository(_connectionString);
        _cardRepository = new CardRepository(_connectionString);
        _coinPurseRepository = new CoinPurseRepository(_connectionString);
    }

    /// <summary>
    /// Allows a user to purchase a package
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="packageType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task PurchasePackageAsync(int userId, GlobalEnums.PackageType packageType)
    {
        // Get corresponding CoinPurse
        CoinPurse coinPurse = await _coinPurseRepository.GetByUserIdAsync(userId);

        // Check if user has enough coins to purchase package
        if (coinPurse.GetCoinsValue() < (int)packageType)
        {
            throw new InvalidOperationException("Not enough coins to purchase package");
        }

        // Create the package
        Package package = new Package(packageType);

        // Determine the amount of cards to be drawn from the database
        int amountOfCards = package.AmountOfCards;

        // Determine the amount of decisions the user can make
        int possibleDecisions = package.PossibleDecisions;

        // Get random cards from the database
        List<Card> randomCards = await _cardRepository.GetRandomCardsAsync(amountOfCards);

        // Add cards to package
        package.AddCards(randomCards);

        // Let the user choose the cards
        List<Card> userCardSelection = GetUserCardSelection(package.Cards, possibleDecisions);

        // Extract the exact coins needed for the package
        Dictionary<GlobalEnums.CoinType, int> coinsUsed = coinPurse.ExtractCoins(package.Price);

        if (coinsUsed == null)
        {
            throw new InvalidOperationException("Failed to extract the required coins for the package");
        }

        // Add the cards to the users stack
        await _stackRepository.AddCardsToUserAsync(userId, userCardSelection);

        // Update the user's coin purse in the repository
        await _coinPurseRepository.UpdateCoinPurseAsync(coinPurse);

        Console.WriteLine("Package purchased successfully!");
        Console.WriteLine("Cards added to your personal cards!");
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
            bool validInput;
            do
            {
                Console.WriteLine($"Choose card {i + 1}:");
                string input = Console.ReadLine();
                validInput = ValidateUserInput(input, randomCards.Count, chosenIndices, out choice);

                if (validInput)
                {
                    chosenIndices.Add(choice);
                    selectedCards.Add(randomCards[choice]);
                }
            } while (!validInput);
        }

        return selectedCards;
    }

    /// <summary>
    /// Validates the user's input for selecting cards
    /// </summary>
    /// <param name="input"> The user's input </param>
    /// <param name="maxIndex"> The maximum valid index for card selection </param>
    /// <param name="chosenIndices"> The set of already chosen indices </param>
    /// <param name="choice"> The parsed choice if valid </param>
    /// <returns> True if the input is valid; otherwise, false </returns>
    private bool ValidateUserInput(string input, int maxIndex, HashSet<int> chosenIndices, out int choice)
    {
        bool validInput = int.TryParse(input, out choice);
        choice -= 1; // Adjust for zero-based index

        if (!validInput)
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
        else if (choice < 0 || choice >= maxIndex)
        {
            Console.WriteLine("Invalid choice. Please select a valid card.");
            validInput = false;
        }
        else if (chosenIndices.Contains(choice))
        {
            Console.WriteLine("You have already chosen this card. Please select a different card.");
            validInput = false;
        }

        return validInput;
    }
}
