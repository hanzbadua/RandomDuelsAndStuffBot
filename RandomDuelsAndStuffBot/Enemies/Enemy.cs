using System.Collections.Generic;

namespace RandomDuelsAndStuffBot.Enemies
{
    // please rewrite all enemy+gen code...
    public abstract class Enemy
    {
        public abstract string Name { get; }
        public abstract int DerivedHealthValue { get; }
        public abstract int DerivedDamageValue { get; }
        public abstract int DerivedDefensiveValue { get; }
        public abstract EnemyType Type { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
