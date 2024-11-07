using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // This class represents a user in the game
    public class User
    {
        // Fields
        public string Username
        {
            get;
            private set;
        } = string.Empty;

        public string Password
        {
            get;
            private set;
        } = string.Empty;

        public Stack Stack
        {
            get;
            private set;
        } = new Stack();

        public CoinPurse CoinPurse
        {
            get;
            private set;
        } = new CoinPurse();
    }
}
