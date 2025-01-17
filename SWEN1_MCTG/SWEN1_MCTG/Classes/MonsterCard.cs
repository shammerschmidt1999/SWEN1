using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    // Derived class from Card, MonsterCard
    public class MonsterCard : Card
    {
        // Constructor
        public MonsterCard(string name, MonsterType monsterType, double damage, ElementType elementType)
            : base(name, damage, elementType)
        {
            _monsterType = monsterType;
        }

        // Fields
        private MonsterType _monsterType;

        // Properties
        public MonsterType MonsterType
        {
            get => _monsterType;
            private set => _monsterType = value;
        }
    }
}