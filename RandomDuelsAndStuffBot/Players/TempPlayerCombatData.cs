namespace RandomDuelsAndStuffBot.Players
{
    public sealed class TempPlayerCombatData
    {
        public int Health { get; set; }
        public int Mana { get; set; }

        public TempPlayerCombatData(int h, int m)
        {
            Health = h;
            Mana = m;
        }
    }
}
