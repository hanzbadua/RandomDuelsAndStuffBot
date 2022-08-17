namespace RandomDuelsAndStuffBot.Items
{
    // Items which are equippable, -> ArmorItem, WeaponItem, BootsItem
    public abstract class EquippableItem : Item 
    {
        public virtual int MaxHealth { get; } = 0;
        public virtual int MaxMana { get; } = 0;
        public virtual int AttackDamage { get; } = 0;
        public virtual int AbilityPower { get; } = 0;
        public virtual int CritChance { get; } = 0;
        public virtual int CritDamage { get; } = 0;
        public virtual int ArmorPenPercent { get; } = 0;
        public virtual int ArmorPenFlat { get; } = 0;
        public virtual int MagicPenPercent { get; } = 0;
        public virtual int MagicPenFlat { get; } = 0;
        public virtual int Omnivamp { get; } = 0;
        public virtual int Armor { get; } = 0;
        public virtual int MagicResist { get; } = 0;
    }
}
