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
        public void AddCoin(Coin newCoin)
        {
            _coins.Add(newCoin);
        }

        public void RemoveCoin(Coin coinToRemove)
        {
            _coins.Remove(coinToRemove);
        }

        public int GetCoinsValue()
        {
            return _coins.Sum(coin => coin.Value);
        }

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
            Console.WriteLine($"Bronze: { bronzeValue }");
            Console.WriteLine($"Silver: { silverValue }");
            Console.WriteLine($"Gold: { goldValue }");
            Console.WriteLine($"Platinum: { platinumValue }");
            Console.WriteLine($"Diamond: { diamondValue }");
            Console.WriteLine($"Total: { GetCoinsValue() }");
        }
    }
}