using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Classes.Exceptions
{
    public class BattleException : Exception
    {
        public BattleException(string message) : base(message)
        { }
    }
}