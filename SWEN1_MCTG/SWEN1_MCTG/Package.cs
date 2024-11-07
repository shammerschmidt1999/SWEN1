using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // Package class for user to buy
    public class Package
    {
        // Fields
        public int Price
        {
            get;
            private set;
        }
        public List<Card> Cards
        {
            get;
            private set;
        } = new List<Card>();
    }
}
