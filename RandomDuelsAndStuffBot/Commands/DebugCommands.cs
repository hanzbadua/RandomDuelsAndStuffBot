using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Commands
{
    // Hidden debug commands
    [Group("debug"), Description("for debugging purposes, useable by owner only"), RequireOwner]
    public sealed class DebugCommands : GameCommandModuleBase
    {
        [Command("exit"), Description("Safely exit the bot client and save data"), RequireOwner]
        public async Task Exit(CommandContext ctx)
        {
            await ctx.RespondAsync($"Exiting safely and saving data...");
            Environment.Exit(0);
        }
    }
}
