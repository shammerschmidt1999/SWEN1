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
        protected Card(string name, GlobalEnums.ElementType element, double damage)
        {
            _name = name;
            _element = element;
            _damage = damage;
        }

        // Fields
        protected string _name;
        protected double _damage;
        protected GlobalEnums.ElementType _element;

        // Properties
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public double Damage
        {
            get => _damage;
            set => _damage = value;
        }
        public GlobalEnums.ElementType Element
        {
            get => _element;
            set => _element = value;
        }

        // Methods
        public abstract void Action();
        public abstract void PrintInformation();
    }
}