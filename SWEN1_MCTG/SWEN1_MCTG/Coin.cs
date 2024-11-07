using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // Used by the User to buy Cards
    public class Coin(GlobalEnums.CoinType coinType)
    {
        // Fields
        public int Value { get; private set; } = CoinValueMap[coinType];

        public GlobalEnums.CoinType CoinType { get; private set; } = coinType;

        // Dictionary to map CoinType to Value
        private static readonly Dictionary<GlobalEnums.CoinType, int> CoinValueMap = new()
        {
            { GlobalEnums.CoinType.Bronze, 1 },
            { GlobalEnums.CoinType.Silver, 3 },
            { GlobalEnums.CoinType.Gold, 5 },
            { GlobalEnums.CoinType.Platinum, 10 }
        };
    }
}
