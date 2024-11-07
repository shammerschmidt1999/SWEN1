using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // CoinPurse class for user to store coins
    public class CoinPurse
    {
        // Fields
        public List<Coin> Coins
        {
            get;
            private set;
        } = new List<Coin>();

        // Methods
        public void AddCoins(Coin newCoin)
        {
            Coins.Add(newCoin);
        }

        public void RemoveCoins(Coin newCoin)
        {
            Coins.Remove(newCoin);
        }

        public int GetCoinsValue()
        {
            int sum = 0;

            foreach (Coin coin in Coins)
            {
                sum += coin.Value;
            }

            return sum;
        }
    }
}
