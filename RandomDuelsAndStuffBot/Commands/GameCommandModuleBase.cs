using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Items;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Refs;
using RandomDuelsAndStuffBot.Skills;
using System.Linq;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Commands
{
    public abstract class GameCommandModuleBase : BaseCommandModule
    {
        // handled by dependency injection, DON'T instantiate manually
        // there is one singleton instance of PlayerData (Players) shared across all command modules which extend GameCommandModuleBase
        // isn't that cool?
        public PlayerData Players { protected get; set; }

        // save data after every command
        public override async Task AfterExecutionAsync(CommandContext ctx)
        {
            await Players.SaveAsync();
        }

        // Check if the player hasn't initialized yet
        // append 'if (await PlayerIsInited(ctx)) return;' to the prologue of all game-related methods (other than help and init)
        protected async Task<bool> PlayerNotInitedOrBusy(CommandContext ctx)
        {
            if (!Players.Data.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync("You don't seem to exist in the player database - have you initialized? `$init`");
                return true;
            } 
            
            if (Players.Data[ctx.User.Id].Busy)
            {
                await ctx.RespondAsync("You appear to be doing something else right now - please resolve that first");
                return true;
            }

            return false;
        }

        // Remember to call PlayerIsInited() before calling this!
        protected async Task<bool> InventoryIsEmpty(CommandContext ctx, Player p)
        {

            // Inventory is empty check
            if (p.Inventory.Count == 0)
            {
                await ctx.RespondAsync("Your inventory is empty");
                return true;
            }

            return false;
        }

        protected async Task<bool> SkillsIsEmpty(CommandContext ctx, Player p)
        {
            if (p.KnownSkills.Count == 0)
            {
                await ctx.RespondAsync("You have no unused skills to view");
                return true;
            }

            return false;
        }

        // recommended to also do PlayerIsInited() and InventoryIsEmpty() checks before calling this
        // uses index, not count!
        protected async Task<bool> ItemIndexIsValid(CommandContext ctx, Player p, int index)
        {
            if (p.Inventory.ElementAtOrDefault(index) is null)
            {
                await ctx.RespondAsync($"There is no valid item in inventory index {index + 1}"); // count is index+1
                return false;
            }

            return true;
        }

        protected async Task<bool> KnownSkillIndexIsValid(CommandContext ctx, Player p, int index)
        {
            if (p.KnownSkills.ElementAtOrDefault(index) is null)
            {
                await ctx.RespondAsync($"There is no valid item in unused skill collection index {index + 1}"); // count is index+1
                return false;
            }

            return true;
        }
    }
}
