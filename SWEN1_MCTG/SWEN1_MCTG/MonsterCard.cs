using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // Derived class from Card, MonsterCard
    public class MonsterCard(string name, GlobalEnums.ElementType element, double damage)
        : Card(name, element, damage)
    {
        // Methods
        public override void Action()
        {
            Console.WriteLine("MonsterCard Action");
        }
    }
}
