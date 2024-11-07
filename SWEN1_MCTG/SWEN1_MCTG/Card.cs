using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG
{
    // Parent class for all cards
    public class Card
    {
        // Constructor
        public Card(string name, GlobalEnums.ElementType element, double damage)
        {
            Name = name;
            Element = element;
            Damage = damage;
        }

        // Fields
        public string Name { get; protected set; }

        public double Damage { get; protected set; }

        public GlobalEnums.ElementType Element { get; protected set; }

        // Methods
        public virtual void Action(){ }
    }
}
