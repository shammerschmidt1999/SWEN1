using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Interfaces
{
    public interface IBattle
    {
        RoundResults StartBattle();
        public double CalculateDamageTest(Card card1, Card card2, double card1Damage);
        public RoundResults CompareDamageTest(double player1Damage, double player2Damage);
    }
}
