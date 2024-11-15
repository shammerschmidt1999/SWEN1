using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes.HttpSvr.Handlers
{
    public class UserHandler : Handler, IHandler
    {
        public override bool Handle(HttpSvrEventArgs e)
        {
            if (e.Path.StartsWith("users"))
            {
                // bla...

                //e.Reply()
                return true;
            }
            else if (e.Path.StartsWith("user"))
            {
                // bla
                if (e.Method == "GET")
                {
                    // bla...
                }

                //e.Reply()
                return true;
            }

            return false;
        }
    }
}
