using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Classes.Exceptions
{
    public class UserException : Exception
    {
        public UserException(string message) : base(message)
        { }
    }
}
