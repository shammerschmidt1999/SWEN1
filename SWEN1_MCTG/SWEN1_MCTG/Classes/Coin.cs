using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    // Used by the User to buy Cards
    public class Coin
    {
        // Constructor
        public Coin(CoinType coinType)
        {
            _coinType = coinType;
            _value = (int)coinType;
        }

        // Fields
        private int _value;
        private CoinType _coinType;

        // Properties
        public int Value => (int)_coinType;

        public CoinType CoinType
        {
            get => _coinType;
            private set => _coinType = value;
        }
    }
}