using RandomDuelsAndStuffBot.Items;
using RandomDuelsAndStuffBot.Refs;
using RandomDuelsAndStuffBot.Skills;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomDuelsAndStuffBot.Players
{
    // For types which you json serialize/deserialize constantly like Player
    // Use the [JsonSerializable] and specify typeof(any reference types) for serialization performance reasons 
    [JsonSerializable(typeof(EquippableItem))]
    [JsonSerializable(typeof(List<Item>))]
    [JsonSerializable(typeof(List<Skill>))]
    // Default Player values should be new player values
    public sealed class Player
    {
        public int Level { get; private set; } = 1;
        public int XP { get; private set; } = 0;
        public int MaxHealth { get; private set; } = 50;
        public int MaxMana { get; private set; } = 10;
        public int AttackDamage { get; private set; } = 10;
        public int AbilityPower { get; private set; } = 0;
        public int CritChance { get; private set; } = 0;
        public int BonusCritDamage { get; private set; } = 0;
        public int ArmorPenPercent { get; private set; } = 0;
        public int ArmorPenFlat { get; private set; } = 0;
        public int MagicPenPercent { get; private set; } = 0;
        public int MagicPenFlat { get; private set; } = 0;
        public int Omnivamp { get; private set; } = 0;
        public int Armor { get; private set; } = 5;
        public int MagicResist { get; private set; } = 5;
        public int Gold { get; private set; } = 0;
        public bool Busy { get; private set; } = false;
        public List<Item> Inventory { get; } = new();
        public EquippableItem Slot1 { get; set; } = null;
        public EquippableItem Slot2 { get; set; } = null;
        public EquippableItem Slot3 { get; set; } = null;
        public EquippableItem Slot4 { get; set; } = null;
        public EquippableItem Slot5 { get; set; } = null;
        public EquippableItem Slot6 { get; set; } = null;
        public List<Skill> KnownSkills { get; } = new();
        public Skill Skill1 { get; set; } = null;
        public Skill Skill2 { get; set; } = null;
        public Skill Skill3 { get; set; } = null;

        // get player stats incl item stats
        public ResolvedPlayerStats GetCombinedStats()
        {
            ResolvedPlayerStats stats = new()
            {
                MaxHealth = this.MaxHealth,
                MaxMana = this.MaxMana,
                AttackDamage = this.AttackDamage,
                AbilityPower = this.AbilityPower,
                CritChance = this.CritChance,
                BonusCritDamage = this.BonusCritDamage,
                ArmorPenPercent = this.ArmorPenPercent,
                ArmorPenFlat = this.ArmorPenFlat,
                MagicPenPercent = this.MagicPenPercent,
                MagicPenFlat = this.MagicPenFlat,
                Omnivamp = this.Omnivamp,
                Armor = this.Armor,
                MagicResist = this.MagicResist
            };

            if (Slot1 is not null) AddStatsFromItem(stats, Slot1);
            if (Slot2 is not null) AddStatsFromItem(stats, Slot2);
            if (Slot3 is not null) AddStatsFromItem(stats, Slot3);
            if (Slot4 is not null) AddStatsFromItem(stats, Slot4);
            if (Slot5 is not null) AddStatsFromItem(stats, Slot5);
            if (Slot6 is not null) AddStatsFromItem(stats, Slot6);

            return stats;
        }

        private static void AddStatsFromItem(ResolvedPlayerStats p, EquippableItem i)
        {
            p.MaxHealth += i.MaxHealth;
            p.MaxMana += i.MaxMana;
            p.AttackDamage += i.AttackDamage;
            p.AbilityPower += i.AbilityPower;
            p.CritChance += i.CritChance;
            p.BonusCritDamage += i.CritDamage;
            p.ArmorPenPercent += i.ArmorPenPercent;
            p.ArmorPenFlat += i.ArmorPenFlat;
            p.MagicPenPercent += i.MagicPenPercent;
            p.MagicPenFlat += i.MagicPenFlat;
            p.Omnivamp += i.Omnivamp;
            p.Armor += i.Armor;
            p.MagicResist += i.MagicResist;
        }

        public int CalculateXPForNextLevel()
        {
            return 100 + (Level * Level * 14);
        }

        public void BusyBlockStart()
        {
            Busy = true;
        }

        public void BusyBlockEnd()
        {
            Busy = false;
        }
    }
}
