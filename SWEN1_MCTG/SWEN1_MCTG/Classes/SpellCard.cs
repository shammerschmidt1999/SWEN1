using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Derived class from Card, SpellCard
    public class SpellCard : Card
    {
        // Constructor
        public SpellCard(string name, double damage, GlobalEnums.ElementType elementType)
            : base(name, damage, elementType)
        {
        }
    }
}