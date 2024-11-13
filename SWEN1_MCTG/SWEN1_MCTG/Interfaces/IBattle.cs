using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IBattle
    {
        void BattleRound(ICard card1, ICard card2, List<ICard> deck1, List<ICard> deck2);
        int CalculateDamage(ICard card1, ICard card2);
    }
}
