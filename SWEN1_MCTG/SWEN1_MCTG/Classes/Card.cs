﻿using System;
using SWEN1_MCTG.Interfaces;
using static SWEN1_MCTG.GlobalEnums;

namespace SWEN1_MCTG.Classes
{
    // Parent class for all cards
    public abstract class Card : ICard
    {
        // Constructor
        protected Card(string name, double damage, ElementType elementType)
        {
            _name = name;
            _damage = damage;
            _elementType = elementType;
            _inDeck = false;
        }

        // Fields
        protected int _id;
        protected string _name;
        protected double _damage;
        protected ElementType _elementType;
        protected bool _inDeck;
        protected Guid _instanceId;

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

        public ElementType ElementType
        {
            get => _elementType;
            private set => _elementType = value;
        }

        public bool InDeck
        {
            get => _inDeck;
            private set => _inDeck = value;
        }

        public Guid InstanceId
        {
            get => _instanceId;
            set => _instanceId = value;
        }

        // Methods
        public void SetInDeck(bool inDeck)
        {
            _inDeck = inDeck;
        }
    }
}