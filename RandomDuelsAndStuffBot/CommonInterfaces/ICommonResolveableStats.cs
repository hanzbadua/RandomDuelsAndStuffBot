namespace RandomDuelsAndStuffBot.CommonInterfaces
{
    // This is an interface representing stats which are expected to be implemented by ResolvedPlayerStats and ResolvedEnemy objects
    // Using an interface this way ensures we don't have to create unnecessary overloads for combat math functions
    public interface ICommonResolveableStats
    {
        public int MaxHealth { get; set; }
        public int AttackDamage { get; set; }
        public int AbilityPower { get; set; }
        public int Armor { get; set; }
        public int MagicResist { get; set; }

    }
}
