using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Derived class from Card, MonsterCard
    public class MonsterCard : Card
    {
        // Constructor
        public MonsterCard(string name, GlobalEnums.MonsterType monsterType, double damage, GlobalEnums.ElementType elementType)
            : base(name, damage, elementType)
        {
            _monsterType = monsterType;
        }

        // Fields
        private GlobalEnums.MonsterType _monsterType;

        // Properties
        public GlobalEnums.MonsterType MonsterType
        {
            get => _monsterType;
            private set => _monsterType = value;
        }

        // Methods
        /// <summary>
        /// Displays Information of the card
        /// </summary>
        public override void PrintInformation()
        {
            Console.WriteLine("MonsterCard Name: " + _name);
            Console.WriteLine("MonsterCard Type: " + _monsterType);
            Console.WriteLine("MonsterCard Element: " + _elementType);
            Console.WriteLine("MonsterCard Damage: " + _damage);
        }
    }
}