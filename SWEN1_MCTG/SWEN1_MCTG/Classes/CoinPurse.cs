using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    // CoinPurse class for user to store coins
    public class CoinPurse : ICoinPurse
    {
        public CoinPurse()
        {
        }

        public CoinPurse(int userId)
        {
            _userId = userId;
        }

        public CoinPurse(CoinType coinType, int userId)
        {
            Coin firstCoin = new Coin(coinType);
            _coins.Add(firstCoin);
            _userId = userId;
        }

        // Fields
        private List<Coin> _coins = new List<Coin>();
        private int _userId;

        // Properties
        public List<Coin> Coins
        {
            get => _coins;
            private set => _coins = value;
        }

        public int UserId
        {
            get => _userId;
            private set => _userId = value;
        }

        // Methods
        /// <summary>
        /// Adds a coin object to the Coin List
        /// </summary>
        /// <param name="newCoin"> The Coin object to be added to the Coin list </param>
        public void AddCoin(Coin newCoin)
        {
            _coins.Add(newCoin);
        }

        /// <summary>
        /// Adds multiple coins at once to the CoinPurse.
        /// </summary>
        /// <param name="coins">List of coins to add.</param>
        public void AddCoins(IEnumerable<Coin> coins)
        {
            _coins.AddRange(coins);
        }

        /// <summary>
        /// Removes coins of a specific type.
        /// </summary>
        /// <param name="coinType"> The type of coin to remove </param>
        /// <param name="count"> Number of coins to remove </param>
        /// <returns> True if successful; otherwise, false </returns>
        public bool RemoveCoins(CoinType coinType, int count)
        {
            List<Coin> coinsToRemove = _coins.Where(c => c.CoinType == coinType).Take(count).ToList();

            if (coinsToRemove.Count < count)
            {
                return false; // Not enough coins of the specified type
            }

            foreach (Coin coin in coinsToRemove)
            {
                _coins.Remove(coin);
            }

            return true;
        }

        /// <summary>
        /// Gets the sum of the coin values in the Coin List
        /// </summary>
        /// <returns> Integer sum of the Coin Values in the Coin List</returns>
        public int GetCoinsValue()
        {
            return _coins.Sum(coin => coin.Value);
        }

        /// <summary>
        /// Calculates and removes the exact coins needed to reach a specified value.
        /// </summary>
        /// <param name="targetValue"> The total value to extract </param>
        /// <returns> A dictionary of CoinType and the count of coins used, or null if the operation fails </returns>
        public Dictionary<CoinType, int> ExtractCoins(int targetValue)
        {
            if (targetValue > GetCoinsValue())
            {
                // Not enough total value in the CoinPurse
                return null;
            }

            // Result dictionary to track the coins used
            Dictionary<CoinType, int> coinsUsed = new Dictionary<CoinType, int>();

            // Sort the coin types by their value in descending order
            IOrderedEnumerable<CoinType> sortedCoinTypes = Enum.GetValues(typeof(CoinType))
                .Cast<CoinType>()
                .OrderByDescending(ct => (int)ct);

            foreach (CoinType coinType in sortedCoinTypes)
            {
                int coinValue = (int)coinType;
                int coinCount = _coins.Count(c => c.CoinType == coinType);

                if (coinCount > 0 && targetValue > 0)
                {
                    // Determine the maximum number of coins to use
                    int coinsToUse = Math.Min(targetValue / coinValue, coinCount);
                    if (coinsToUse > 0)
                    {
                        // Update target value and track coins used
                        targetValue -= coinsToUse * coinValue;
                        coinsUsed[coinType] = coinsToUse;

                        // Remove the coins from the purse
                        RemoveCoins(coinType, coinsToUse);
                    }

                    // Handle partial coin usage if target value is still greater than 0
                    if (targetValue > 0 && coinValue > targetValue)
                    {
                        coinsUsed[coinType] = coinsUsed.ContainsKey(coinType) ? coinsUsed[coinType] + 1 : 1;
                        int remainingValue = coinValue - targetValue;
                        targetValue = 0;
                        RemoveCoins(coinType, 1);

                        // Add the remaining value back as smaller denomination coins
                        foreach (CoinType smallerCoinType in sortedCoinTypes.Reverse())
                        {
                            int smallerCoinValue = (int)smallerCoinType;
                            while (remainingValue >= smallerCoinValue)
                            {
                                AddCoin(new Coin(smallerCoinType));
                                remainingValue -= smallerCoinValue;
                            }
                        }
                    }
                }

                // Exit early if the target value is met
                if (targetValue == 0)
                {
                    break;
                }
            }

            // If the target value is not met, revert changes and return null
            if (targetValue > 0)
            {
                // Add the removed coins back to the purse
                foreach (KeyValuePair<CoinType, int> entry in coinsUsed)
                {
                    AddCoins(Enumerable.Repeat(new Coin(entry.Key), entry.Value));
                }
                return null;
            }

            return coinsUsed;
        }
    }
}