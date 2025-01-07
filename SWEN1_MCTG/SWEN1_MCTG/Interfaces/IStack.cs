using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IStack
    {
        void AddCardToStack(Card newCard);
        Card? GetRandomCardFromStack();
    }
}
