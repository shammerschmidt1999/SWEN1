using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Interfaces
{
    public interface IPackage
    {
        int Price { get; set; }
        List<Card> Cards { get; set; }
        void AddCard(Card card);
        void RemoveCard(Card card);
    }

}
