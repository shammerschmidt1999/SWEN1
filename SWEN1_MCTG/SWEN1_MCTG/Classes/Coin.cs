using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Used by the User to buy Cards
    public class Coin
    {
        // Constructor
        public Coin(GlobalEnums.CoinType coinType)
        {
            _coinType = coinType;
            _value = (int)coinType;
        }

        // Fields
        private int _value;
        private GlobalEnums.CoinType _coinType;

        // Properties
        public int Value => (int)_coinType;

        public GlobalEnums.CoinType CoinType
        {
            get => _coinType;
            private set => _coinType = value;
        }
    }
}