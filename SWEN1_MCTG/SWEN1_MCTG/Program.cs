﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            User testUser = new User("TestUser", "TestPassword");
            testUser.PrintUser();
            testUser.UserCoinPurse.PrintCoins();

        }
    }
}
