using RandomDuelsAndStuffBot.CommonInterfaces;

namespace RandomDuelsAndStuffBot.Players
{
    // These should ideally be get-init properties but they are get-set for post-item player stats calculations
    public sealed class ResolvedPlayerStats : ICommonResolveableStats
    {
        public int MaxHealth { get; set; }
        public int MaxMana { get; set; } 
        public int AttackDamage { get; set; } 
        public int AbilityPower { get; set; } 
        public int CritChance { get; set; } 
        public int BonusCritDamage { get; set; } 
        public int ArmorPenPercent { get; set; }
        public int ArmorPenFlat { get; set; }
        public int MagicPenPercent { get; set; }
        public int MagicPenFlat { get; set; }
        public int Omnivamp { get; set; }
        public int Armor { get; set; }
        public int MagicResist { get; set; }
    }
}
