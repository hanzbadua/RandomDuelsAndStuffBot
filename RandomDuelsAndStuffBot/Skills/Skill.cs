using DSharpPlus.Entities;
using RandomDuelsAndStuffBot.Enemies;
using RandomDuelsAndStuffBot.Items;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Refs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomDuelsAndStuffBot.Skills
{
    public abstract class Skill
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ActionDescription { get; }
        public abstract int ManaCost { get; }
        public abstract int Cooldown { get; }
        public abstract void Effect(DiscordEmbedBuilder toModify, ResolvedPlayerStats p, ResolvedEnemy e, TempEnemyCombatData temp);
        public override string ToString()
        {
            return Name;
        }

        public static T Get<T>() where T : Skill, new()
        {
            return new T();
        }
    }
}
