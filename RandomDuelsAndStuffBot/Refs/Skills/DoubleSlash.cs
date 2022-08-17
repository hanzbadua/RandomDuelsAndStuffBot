using DSharpPlus.Entities;
using RandomDuelsAndStuffBot.Combat;
using RandomDuelsAndStuffBot.Common;
using RandomDuelsAndStuffBot.Enemies;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Skills;

namespace RandomDuelsAndStuffBot.Refs.Skills
{
    public sealed class DoubleSlash : Skill
    {
        public override string Name { get; } = "Double Slash";
        public override string Description { get; } = "Two quick slashes; each slash does damage equal to 45% of your Attack Damage, +5% per 1 flat Armor Pen; each slash has a difference instance of armor mitigation";
        public override string ActionDescription { get; } = "You slashed twice precisely, dealing a total of {0} damage";
        public override int ManaCost { get; } = 5;
        public override int Cooldown { get; } = 0;

        public override void Effect(DiscordEmbedBuilder toModify, ResolvedPlayerStats p, ResolvedEnemy e, TempEnemyCombatData temp)
        {
            int dmg;
            // This is code for one strike - we multiply it by 2 later for actual damage in order to take account for mitigations per strike
            dmg = p.AttackDamage * (45 + 5 * p.ArmorPenFlat) / 100;
            CombatMath.PostEnemyPhysicalMitigations(ref dmg, p, e);
            temp.Health -= dmg * 2;
            toModify.WithDescription(string.Format(ActionDescription, dmg * 2));
            return;
        }
    }
}
