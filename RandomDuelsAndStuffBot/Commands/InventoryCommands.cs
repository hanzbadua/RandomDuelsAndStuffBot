using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Items;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Refs;
using System;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Commands
{
    // inventory related cmds
    // literal piece of shit garbage code that I don't understand but it still works so..?
    public sealed class InventoryCommands : GameCommandModuleBase
    {
        [Command("inventory"), Aliases("inv"), Description("View the contents of your inventory")]
        public async Task Inventory(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            string contents = "";
            int index = 1;
            Player p = Players.Data[ctx.User.Id];

            if (await InventoryIsEmpty(ctx, p))
            {
                return;
            }

            foreach (Item i in p.Inventory)
            { 
                contents += $"{index}. {i.Name} ({Enum.GetName(i.Rarity)}, {i.ResolveTypeToString()}){Globals.NL}";
                index++;
            }

            DiscordEmbed msg = new DiscordEmbedBuilder()
            {
                Title = "Inventory",
                Color = Globals.ColorBlue,
                Description = contents
            }.Build();

            await ctx.RespondAsync(msg);
        }

        [Command("inventory"), Description("View an item in your inventory via index")]
        public async Task Inventory(CommandContext ctx, [Description("Inventory index of the item to view")] int count)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];
            if (await InventoryIsEmpty(ctx, p))
            {
                return;
            }

            int index = count - 1; // internal indexes start at 0, for humans it starts at 1, so sub by 1
            if (!await ItemIndexIsValid(ctx, p, index))
            {
                return;
            }

            Item item = p.Inventory[index];

            DiscordEmbedBuilder msg = new DiscordEmbedBuilder { Title = $"Viewing item: {item.Name}", Color = Globals.ColorBlue, Description = item.Description }
                .AddField("Rarity", Enum.GetName(item.Rarity))
                .AddField("Type", item.ResolveTypeToString())
                .AddField("Value", item.Value != 0 ? item.Value.ToString() : "Worthless");

            if (item is EquippableItem convert)
            {
                if (convert.MaxHealth != 0)
                {
                    msg.AddField("Max Health", convert.MaxHealth.ToString());
                }

                if (convert.MaxMana != 0)
                {
                    msg.AddField("Max Mana", convert.MaxMana.ToString());
                }

                if (convert.AttackDamage != 0)
                {
                    msg.AddField("Attack Damage", convert.AttackDamage.ToString());
                }

                if (convert.AbilityPower != 0)
                {
                    msg.AddField("Ability Power", convert.AbilityPower.ToString());
                }

                if (convert.CritChance != 0)
                {
                    msg.AddField("Crit Chance", convert.CritChance.ToString());
                }

                if (convert.CritDamage != 0)
                {
                    msg.AddField("Bonus Crit Damage", convert.CritDamage.ToString());
                }

                if (convert.ArmorPenPercent != 0 || convert.ArmorPenFlat != 0)
                {
                    msg.AddField("Armor Pen (flat|%)", $"{convert.ArmorPenFlat} | {convert.ArmorPenPercent}%");
                }

                if (convert.MagicPenPercent != 0 || convert.MagicPenFlat != 0)
                {
                    msg.AddField("Magic Pen (flat|%)", $"{convert.MagicPenFlat} | {convert.MagicPenPercent}%");
                }

                if (convert.Omnivamp != 0)
                {
                    msg.AddField("Omnivamp", convert.Omnivamp.ToString());
                }

                if (convert.Armor != 0)
                {
                    msg.AddField("Armor", convert.Armor.ToString());
                }

                if (convert.MagicResist != 0)
                {
                    msg.AddField("Magic Resist", convert.MagicResist.ToString());
                }
            }

            await ctx.RespondAsync(msg.Build());
        }

        [Command("equip"), Description("Equip an item via inventory index and equip slot; Valid equip slots: `slot1`, `slot2`, `slot3`, `slot4`, `slot5`, `slot6`")]
        public async Task Equip(CommandContext ctx, [Description("Inventory index of the item to equip")] int count, [RemainingText][Description("Slot name to equip")] string slot)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];

            if (await InventoryIsEmpty(ctx, p))
            {
                return;
            }

            int index = count - 1; // internal indexes start at 0, for humans it starts at 1, so sub by 1
            if (!await ItemIndexIsValid(ctx, p, index))
            {
                return;
            }

            DiscordEmbedBuilder msg = new();
            Item item = p.Inventory[index];

            if (item is not EquippableItem ret)
            {
                await ctx.RespondAsync(msg
                    .WithTitle("Invalid item to equip")
                    .WithDescription("This isn't a valid equippable item!")
                    .WithColor(Globals.ColorRed));
                return;
            }

            p.Inventory.RemoveAt(index);

            await ctx.RespondAsync(msg
                .WithTitle($"Equipping item `{ret.Name}`... in slot `{slot}`")
                .WithDescription("...successful! (note: if you had a previously equipped item in the slot, it has now been returned to your inventory)")
                .WithColor(Globals.ColorGreen)
                .Build());

            if (slot == "slot1")
            {
                if (p.Slot1 is not null)
                {
                    p.Inventory.Add(p.Slot1);
                }

                p.Slot1 = ret;
            }
            else if (slot == "slot2")
            {
                if (p.Slot2 is not null)
                {
                    p.Inventory.Add(p.Slot2);
                }

                p.Slot2 = ret;
            }
            else if (slot == "slot3")
            {
                if (p.Slot3 is not null)
                {
                    p.Inventory.Add(p.Slot3);
                }

                p.Slot3 = ret;
            }
            else if (slot == "slot4")
            {
                if (p.Slot4 is not null)
                {
                    p.Inventory.Add(p.Slot4);
                }

                p.Slot4 = ret;
            }
            else if (slot == "slot5")
            {
                if (p.Slot5 is not null)
                {
                    p.Inventory.Add(p.Slot5);
                }

                p.Slot5 = ret;
            }
            else if (slot == "slot6")
            {
                if (p.Slot6 is not null)
                {
                    p.Inventory.Add(p.Slot6);
                }

                p.Slot6 = ret;
            }
            else
            {
                DiscordEmbedBuilder err = new()
                {
                    Title = "Invalid equip slot specified",
                    Description = $"`{slot}` is not a valid equip slot{Globals.NL}Valid equip slots: `slot1`, `slot2`, `slot3`, `slot4`, `slot5`, `slot6`",
                    Color = Globals.ColorRed
                };

                await ctx.RespondAsync(err.Build());
                return;
            }
        }

        [Command("Unequip"), Description("Unequip an item in a slot; Valid unequip slots: `slot1`, `slot2`, `slot3`, `slot4`, `slot5`, `slot6`")]
        public async Task Unequip(CommandContext ctx, [RemainingText][Description("Slot name to unequip")] string slot)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            slot = slot.RemoveWhitespace().ToLowerInvariant();
            Player p = Players.Data[ctx.User.Id];
            DiscordEmbed noequip = new DiscordEmbedBuilder
            {
                Title = $"There's nothing to unequip in slot `{slot}`",
                Description = "You can use the command `$equipped` to check what you have currently equipped",
                Color = Globals.ColorRed
            }.Build();

            EquippableItem i;
            if (slot == "slot1")
            {
                if (p.Slot1 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot1;
                    p.Slot1 = null;
                }
            }
            else if (slot == "slot2")
            {
                if (p.Slot2 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot2;
                    p.Slot2 = null;
                }
            }
            else if (slot == "slot3")
            {
                if (p.Slot3 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot3;
                    p.Slot3 = null;
                }
            }
            else if (slot == "slot4")
            {
                if (p.Slot4 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot4;
                    p.Slot4 = null;
                }
            }
            else if (slot == "slot5")
            {
                if (p.Slot5 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot5;
                    p.Slot5 = null;
                }
            }
            else if (slot == "slot6")
            {
                if (p.Slot6 is null)
                {
                    await ctx.RespondAsync(noequip);
                    return;
                }
                else
                {
                    i = p.Slot6;
                    p.Slot6 = null;
                }
            }
            else
            {
                DiscordEmbedBuilder err = new()
                {
                    Title = "Invalid unequip slot specified",
                    Description = $"`{slot}` is not a valid unequip slot{Globals.NL}Valid unequip slots: `slot1`, `slot2`, `slot3`, `slot4`, `slot5`, `slot6`",
                    Color = Globals.ColorRed
                };

                await ctx.RespondAsync(err.Build());
                return;
            }

            await ctx.RespondAsync(new DiscordEmbedBuilder()
                .WithColor(Globals.ColorGreen)
                .WithTitle("Unequipping item...")
                .WithDescription($"Item `{i.Name}` successfully unequipped from slot `{slot}`")
                .Build());

            p.Inventory.Add(i);
        }
    }
}
