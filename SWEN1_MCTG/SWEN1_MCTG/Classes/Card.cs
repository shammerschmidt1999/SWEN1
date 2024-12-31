using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN1_MCTG.Interfaces;

namespace SWEN1_MCTG.Classes
{
    // Parent class for all cards
    public abstract class Card : ICard
    {
        // Constructor
        protected Card(string name, double damage, GlobalEnums.ElementType elementType)
        {
            _name = name;
            _damage = damage;
            _elementType = elementType;
        }

        public Card() {}

        // Fields
        protected int _id;
        protected string _name;
        protected double _damage;
        protected GlobalEnums.ElementType _elementType;

        // Properties

        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public string Name
        {
            get => _name;
            private set => _name = value;
        }
        public double Damage
        {
            get => _damage;
            private set => _damage = value;
        }
        public GlobalEnums.ElementType ElementType
        {
            get => _elementType;
            private set => _elementType = value;
        }

        // Methods
        public abstract void PrintInformation();
    }
}