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
        protected Card(string name, double damage)
        {
            _name = name;
            _damage = damage;
        }

        // Fields
        protected string _name;
        protected double _damage;

        // Properties
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

        // Methods
        public abstract void Action();
        public abstract void PrintInformation();
    }
}