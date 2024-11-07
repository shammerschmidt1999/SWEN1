using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IUser
    {
        string Username { get; set; }
        string Password { get; set; }
        Stack UserStack { get; set; }
        CoinPurse UserCoinPurse { get; set; }

        void PrintUser();
        void PrintStack();
    }
}
