using RandomDuelsAndStuffBot.Refs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomDuelsAndStuffBot.Items
{
    public abstract class Item
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemRarity Rarity { get; }
        public abstract int Value { get; }
        public override string ToString()
        {
            return Name;
        }

        public string ResolveTypeToString()
        {
            if (this is EquippableItem)
            {
                return "Equippable";
            }

            return "Normal";
        }

        public static T Get<T>() where T : Item, new()
        {
            return new T();
        }
    }
}
