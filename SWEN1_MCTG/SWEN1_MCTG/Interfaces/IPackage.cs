﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IPackage
    {
        void AddCards(List<Card> cards);
        int GetAmountOfCards();
        int GetPossibleDecisions();
    }

}
