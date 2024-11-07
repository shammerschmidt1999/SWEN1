using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // CoinPurse class for user to store coins
    public class CoinPurse : ICoinPurse
    {
        // Fields
        private List<Coin> _coins = new List<Coin>();

        // Properties
        public List<Coin> Coins
        {
            get => _coins;
            set => _coins = value;
        }

        // Methods
        public void AddCoin(Coin newCoin)
        {
            _coins.Add(newCoin);
        }

        public void RemoveCoin(Coin newCoin)
        {
            _coins.Remove(newCoin);
        }

        public int GetCoinsValue()
        {
            return _coins.Sum(coin => coin.Value);
        }
    }
}