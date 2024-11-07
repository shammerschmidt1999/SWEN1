using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN1_MCTG.Classes
{
    // Derived class from Card, MonsterCard
    public class MonsterCard : Card
    {
        // Constructor
        public MonsterCard(string name, GlobalEnums.ElementType element, double damage, double health)
            : base(name, element, damage)
        {
            _health = health;
        }

        // Fields
        private double _health;

        // Properties
        public double Health => _health;

        // Methods
        public override void Action()
        {
            Console.WriteLine("MonsterCard action");
        }

        public override void PrintInformation()
        {
            Console.WriteLine("MonsterCard Name: " + _name);
            Console.WriteLine("MonsterCard Element: " + _element);
            Console.WriteLine("MonsterCard Damage: " + _damage);
            Console.WriteLine("MonsterCard Health: " + _health);
        }
    }
}