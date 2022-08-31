using RandomDuelsAndStuffBot.CommonInterfaces;
using System.Collections.Generic;

namespace RandomDuelsAndStuffBot.Enemies
{
    // please rewrite all enemy gen code...
    public sealed class ResolvedEnemy : ICommonResolveableStats
    {
        public string Name { get; init; } 
        public int MaxHealth { get; set; }
        public int AttackDamage { get; set; }
        public int AbilityPower { get; set; }
        public int Armor { get; set; }
        public int MagicResist { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
