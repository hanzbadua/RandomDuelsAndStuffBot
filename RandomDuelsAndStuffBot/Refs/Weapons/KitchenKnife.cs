using RandomDuelsAndStuffBot.Common;
using RandomDuelsAndStuffBot.Items;

namespace RandomDuelsAndStuffBot.Refs.Weapons
{
    public sealed class KitchenKnife : EquippableItem
    {
        public override string Name { get; } = "Kitchen Knife";
        public override string Description { get; } = "Could be worse...";
        public override ItemRarity Rarity { get; } = ItemRarity.Common;
        public override int Value { get; } = 10;
        public override int AttackDamage { get; } = 5;
        public override int ArmorPenFlat { get; } = 1;
    }
}
