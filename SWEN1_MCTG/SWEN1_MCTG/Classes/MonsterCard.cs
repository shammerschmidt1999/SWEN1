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
        public MonsterCard(string name, GlobalEnums.MonsterType monsterType, double damage)
            : base(name, damage)
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
        public override void Action()
        {
            Console.WriteLine("MonsterCard action");
        }

        public override void PrintInformation()
        {
            Console.WriteLine("MonsterCard Name: " + _name);
            Console.WriteLine("MonsterCard Type: " + _monsterType);
            Console.WriteLine("MonsterCard Damage: " + _damage);
        }
    }
}