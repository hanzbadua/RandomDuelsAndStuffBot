using System;

namespace RandomDuelsAndStuffBot.Items
{
    // Items directly derived from this type are 'Normal' items and MUST have an ItemId in the 'Normal' range
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

        public static Clz Get<Clz>() where Clz : Item, new()
        {
            return new Clz();
        }
    }
}
