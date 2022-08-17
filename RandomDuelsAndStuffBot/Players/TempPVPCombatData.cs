using DSharpPlus.Entities;

namespace RandomDuelsAndStuffBot.Players
{
    public sealed class TempPVPCombatData
    {
        public DiscordUser User1 { get; }
        public DiscordUser User2 { get; }
        public TempPVPCombatData(DiscordUser u1, DiscordUser u2)
        {
            User1 = u1;
            User2 = u2;
        }
    }
}
