using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Classes
{
    // Derived class from Card, SpellCard
    public class SpellCard : Card
    {
        // Constructor
        public SpellCard(string name, GlobalEnums.ElementType element, double damage)
            : base(name, element, damage)
        {
        }

        // Methods
        public override void Action()
        {
            Console.WriteLine("SpellCard action");
        }

        public override void PrintInformation()
        {
            Console.WriteLine("SpellCard Name: " + _name);
            Console.WriteLine("SpellCard Element: " + _element);
            Console.WriteLine("SpellCard Damage: " + _damage);
        }
    }
}