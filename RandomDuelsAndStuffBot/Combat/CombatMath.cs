using RandomDuelsAndStuffBot.CommonInterfaces;
using RandomDuelsAndStuffBot.Enemies;
using RandomDuelsAndStuffBot.Players;

namespace RandomDuelsAndStuffBot.Combat
{
    public static class CombatMath
    {
        public static void PostEnemyPhysicalMitigations(ref int modify, ResolvedPlayerStats p, ICommonResolveableStats e)
        {
            int enemyEffectiveAr = e.Armor - (e.Armor * p.ArmorPenPercent / 100) - p.ArmorPenFlat;
            if (enemyEffectiveAr < 0)
            {
                enemyEffectiveAr = 0;
            }

            if (modify - enemyEffectiveAr <= 0)
            {
                modify = 1;
            }
            else
            {
                modify -= enemyEffectiveAr;
            }
        }

        public static void PostEnemyMagicalMitigations(ref int modify, ResolvedPlayerStats p, ICommonResolveableStats e)
        {
            int enemyEffectiveMr = e.MagicResist - (e.MagicResist * p.MagicPenPercent / 100) - p.MagicPenFlat;
            if (enemyEffectiveMr < 0)
            {
                enemyEffectiveMr = 0;
            }

            if (modify - enemyEffectiveMr <= 0)
            {
                modify = 1;
            }
            else
            {
                modify -= enemyEffectiveMr;
            }
        }

        public static bool CriticalStrike(ref int modify, ResolvedPlayerStats p)
        {
            if (p.CritChance >= Globals.Rng.Next(0, 101))
            {
                modify += modify * (75 + p.BonusCritDamage) / 100;
                return true;
            }

            return false;
        }

        public static void GetBasicAttack(ResolvedPlayerStats p, ICommonResolveableStats e, out bool wasCrit, out int result)
        {
            int aa = Globals.Rng.Next(p.AttackDamage - (p.AttackDamage * 25 / 100), p.AttackDamage + (p.AttackDamage * 25 / 100));
            wasCrit = CriticalStrike(ref aa, p);
            PostEnemyPhysicalMitigations(ref aa, p, e);
            result = aa;
        }
    }
}
