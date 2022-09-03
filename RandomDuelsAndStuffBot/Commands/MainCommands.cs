using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Combat;
using RandomDuelsAndStuffBot.Enemies;
using RandomDuelsAndStuffBot.Items;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Refs;
using RandomDuelsAndStuffBot.Refs.Skills;
using RandomDuelsAndStuffBot.Refs.Weapons;
using RandomDuelsAndStuffBot.Skills;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Commands
{
    // main game commands
    public sealed class MainCommands : GameCommandModuleBase
    {
        [Command("initialize"), Aliases("init"), Description("Initialize your character")]
        public async Task Initialize(CommandContext ctx)
        {
            DiscordEmbedBuilder msg = new() { Title = "Character initialization" };

            if (Players.Data.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync(msg
                    .WithColor(Globals.ColorRed)
                    .WithDescription("You have already initialized your character")
                    .Build());
                return;
            }

             await ctx.RespondAsync(msg
                .WithColor(Globals.ColorBlue)
                .WithDescription("Initializing character... successful!")
                .Build());

            Players.Data[ctx.User.Id] = new();
            Player p = Players.Data[ctx.User.Id];
            p.Slot1 = Item.Get<KitchenKnife>();
            p.Skill1 = Skill.Get<DoubleSlash>();
        }

        [Command("balance"), Aliases("bal"), Description("Check your current gold balance")]
        public async Task Balance(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id]; 
             await ctx.RespondAsync($"Current gold amount: {p.Gold}");
        }

        [Command("stats"), Description("Check your current stats")]
        public async Task Stats(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];
            ResolvedPlayerStats s = p.GetCombinedStats();
            DiscordEmbedBuilder msg = new DiscordEmbedBuilder
            {
                Title = "Stats",
                Color = Globals.ColorBlue
            }
            .AddField("Level, XP", $"{p.Level}, {p.XP}/{p.CalculateXPForNextLevel()}")
            .AddField("Max Health", $"{s.MaxHealth}")
            .AddField("Max Mana", $"{s.MaxMana}")
            .AddField("Attack Damage", $"{s.AttackDamage}")
            .AddField("Ability Power", $"{s.AbilityPower}")
            .AddField("Crit Chance", $"{s.CritChance}%")
            .AddField("Bonus Crit Damage", $"{s.BonusCritDamage}%")
            .AddField("Armor Pen", $"{s.ArmorPenFlat} flat | {s.ArmorPenPercent}%")
            .AddField("Magic Pen", $"{s.MagicPenFlat} flat | {s.MagicPenPercent}%")
            .AddField("Omnivamp", $"{s.Omnivamp}%")
            .AddField("Resistances", $"{s.Armor} armor | {s.MagicResist} magic resist");
            //.WithFooter($"to check your inventory, use '$inventory'{NL}to check your gold, use '$balance'{NL}to check your equipped items, use '$equipped'");

             await ctx.RespondAsync(msg.Build());
        }

        [Command("equipped"), Description("Check your currently equipped items and currently learned skills")]
        public async Task Equipped(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];
            const string na = "N/A";

            DiscordEmbedBuilder msg = new()
            {
                Title = "Equipped items + currently learned skills",
                Color = Globals.ColorBlue
            };

             msg.AddField("Slot 1", p.Slot1 is not null ? p.Slot1.Name : na)
                .AddField("Slot 2", p.Slot2 is not null ? p.Slot2.Name : na)
                .AddField("Slot 3", p.Slot3 is not null ? p.Slot3.Name : na)
                .AddField("Slot 4", p.Slot4 is not null ? p.Slot4.Name : na)
                .AddField("Slot 5", p.Slot5 is not null ? p.Slot5.Name : na)
                .AddField("Slot 6", p.Slot6 is not null ? p.Slot6.Name : na)
                .AddField("Skill (1)", p.Skill1 is not null ? p.Skill1.Name : na)
                .AddField("Skill (2)", p.Skill2 is not null ? p.Skill2.Name : na)
                .AddField("Skill (3)", p.Skill3 is not null ? p.Skill3.Name : na);
             await ctx.RespondAsync(msg.Build());
        }

        /*
        [Command("encounter"), Description("Maybe you'll find something worthwhile to fight")]
        public async Task Encounter(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];

            p.BusyBlockStart();
            await CombatRoutines.ExecuteCombatRoutine(ctx, p, null);
            p.BusyBlockEnd();
        }
        */
    }
}