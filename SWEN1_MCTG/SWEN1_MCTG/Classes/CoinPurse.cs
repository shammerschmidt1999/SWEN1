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
            // Add 5 coins to the purse
            Coin newCoin = new Coin(GlobalEnums.CoinType.Diamond);
            AddCoin(newCoin);
        }

        // Fields
        private List<Coin> _coins = new List<Coin>();

        // Properties
        public List<Coin> Coins
        {
            get => _coins;
            private set => _coins = value;
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
        /// <param name="coinType">The type of coin to remove.</param>
        /// <param name="count">Number of coins to remove.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public bool RemoveCoins(GlobalEnums.CoinType coinType, int count)
        {
            var coinsToRemove = _coins.Where(c => c.CoinType == coinType).Take(count).ToList();

            if (coinsToRemove.Count < count)
            {
                return false; // Not enough coins of the specified type
            }

            foreach (var coin in coinsToRemove)
            {
                _coins.Remove(coin);
            }

            return true;
        }

        /// <summary>
        /// Converts coins of a lower value to a higher value if possible.
        /// </summary>
        /// <param name="fromType">The type of coin to convert from.</param>
        /// <param name="toType">The type of coin to convert to.</param>
        /// <returns>True if conversion was successful; otherwise, false.</returns>
        public bool ConvertCoins(GlobalEnums.CoinType fromType, GlobalEnums.CoinType toType)
        {
            int fromValue = (int)fromType;
            int toValue = (int)toType;

            if (toValue <= fromValue || GetCoinsValueByType(fromType) < toValue)
            {
                return false; // Cannot convert
            }

            int requiredCount = toValue / fromValue;
            if (RemoveCoins(fromType, requiredCount))
            {
                AddCoin(new Coin(toType));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the total value of coins of a specific type.
        /// </summary>
        /// <param name="coinType">The type of coin.</param>
        /// <returns>Total value of coins of the specified type.</returns>
        private int GetCoinsValueByType(GlobalEnums.CoinType coinType)
        {
            return _coins.Where(c => c.CoinType == coinType).Sum(c => c.Value);
        }

        /// <summary>
        /// Removes a coin object from the Coin List
        /// </summary>
        /// <param name="coinToRemove"> Coin to be removed from the Coin List </param>
        public void RemoveCoin(Coin coinToRemove)
        {
            _coins.Remove(coinToRemove);
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
        /// Displays amount of coins and sum of coin value for each coin type
        /// </summary>
        public void PrintCoins()
        {
            int bronzeValue = 0;
            int silverValue = 0;
            int goldValue = 0;
            int platinumValue = 0;
            int diamondValue = 0;

            foreach (Coin coin in _coins)
            {
                switch(coin.CoinType)
                {
                    case GlobalEnums.CoinType.Bronze:
                        bronzeValue += coin.Value;
                        break;
                    case GlobalEnums.CoinType.Silver:
                        silverValue += coin.Value;
                        break;
                    case GlobalEnums.CoinType.Gold:
                        goldValue += coin.Value;
                        break;
                    case GlobalEnums.CoinType.Platinum:
                        platinumValue += coin.Value;
                        break;
                    case GlobalEnums.CoinType.Diamond:
                        diamondValue += coin.Value;
                        break;
                }
            }
            Console.WriteLine($"{ bronzeValue / ((int)GlobalEnums.CoinType.Bronze) }x Bronze: { bronzeValue }");
            Console.WriteLine($"{ silverValue / ((int)GlobalEnums.CoinType.Silver) }x Silver: { silverValue }");
            Console.WriteLine($"{ goldValue / ((int)GlobalEnums.CoinType.Gold) }x Gold: { goldValue }");
            Console.WriteLine($"{ platinumValue / ((int)GlobalEnums.CoinType.Platinum) }x Platinum: { platinumValue }");
            Console.WriteLine($"{ diamondValue / ((int)GlobalEnums.CoinType.Diamond) }x Diamond: { diamondValue }");
            Console.WriteLine($"Total: { GetCoinsValue() }");
        }
    }
}