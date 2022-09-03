using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Refs;
using RandomDuelsAndStuffBot.Skills;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Commands
{
    // skill related cmds
    // orgasmic compared to inventory/combat code..
    public sealed class SkillCommands : GameCommandModuleBase
    {
        [Command("skills"), Description("View your known skill collection")]
        public async Task Skills(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];

            if (await SkillsIsEmpty(ctx, p))
            {
                return;
            }

            string contents = "";
            int index = 1;

            foreach (Skill i in p.KnownSkills)
            {
                contents += $"{index}. {i.Name}{Globals.NL}";
                index++;
            }

            DiscordEmbedBuilder msg = new()
            {
                Title = "Known Skills",
                Color = Globals.ColorBlue,
                Description = contents
            };

             await ctx.RespondAsync(msg.Build());
        }

        [Command("skills"), Description("View a skill in your known skills collection")]
        public async Task Skills(CommandContext ctx, [Description("Index of the skill to view")] int count)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];

            if (await SkillsIsEmpty(ctx, p))
            {
                return;
            }

            int index = count - 1; // internal indexes start at 0, for humans it starts at 1, so sub by 1
            if (!await KnownSkillIndexIsValid(ctx, p, index))
            {
                return;
            }

            Skill skill = p.KnownSkills[index];
            DiscordEmbedBuilder msg = new() { Title = $"Viewing skill: {skill.Name}", Color = Globals.ColorBlue, Description = skill.Description };

             await ctx.RespondAsync(msg.Build());
        }

        [Command("learn")]
        public async Task Learn(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

             await ctx.RespondAsync("You need to specify a skill to learn");
        }

        [Command("learn"), Description("Learn a skill in your known skills collection")]
        public async Task Learn(CommandContext ctx, [Description("Index of the skill to learn")] int count)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];

            if (await SkillsIsEmpty(ctx, p))
            {
                return;
            }

            int index = count - 1; // internal indexes start at 0, for humans it starts at 1, so sub by 1
            if (!await KnownSkillIndexIsValid(ctx, p, index))
            {
                return;
            }

            Skill s = p.KnownSkills[index];

            p.BusyBlockStart();

            DiscordEmbedBuilder msg = new() { Title = $"Learning skill {s.Name}", Color = Globals.ColorBlue };
            DiscordMessage req = await ctx.RespondAsync(msg);
            DiscordEmoji oneEmoji = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji twoEmoji = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji threeEmoji = DiscordEmoji.FromName(ctx.Client, ":three:");

            await req.CreateReactionAsync(oneEmoji);
            await req.CreateReactionAsync(twoEmoji);
            await req.CreateReactionAsync(threeEmoji);

            InteractivityResult<MessageReactionAddEventArgs> res = await req.WaitForReactionAsync(ctx.Member);

            if (!res.TimedOut)
            {
                if (res.Result.Emoji == oneEmoji)
                {
                    if (p.Skill1 is null)
                    {
                        await req.ModifyAsync(msg.WithTitle($"Skill {s.Name} learned in slot one").WithColor(Globals.ColorGreen).Build());
                        p.Skill1 = s;
                    }
                    else
                    {
                        await req.ModifyAsync(msg
                            .WithTitle($"Skill {s.Name} learned in slot one")
                            .WithDescription($"NOTE: you already had a skill in this slot, replacing current skill {p.Skill1.Name} with {s.Name}")
                            .WithColor(Globals.ColorGreen)
                            .Build());

                        p.KnownSkills.Add(p.Skill1);
                        p.Skill1 = s;
                    }
                }
                else if (res.Result.Emoji == twoEmoji)
                {
                    if (p.Skill2 is null)
                    {
                        await req.ModifyAsync(msg.WithTitle($"Skill {s.Name} learned in slot two").WithColor(Globals.ColorGreen).Build());
                        p.Skill2 = s;
                    }
                    else
                    {
                        await req.ModifyAsync(msg
                            .WithTitle($"Skill {s.Name} learned in slot two")
                            .WithDescription($"NOTE: you already had a skill in this slot, replacing current skill {p.Skill2.Name} with {s.Name}")
                            .WithColor(Globals.ColorGreen)
                            .Build());

                        p.KnownSkills.Add(p.Skill2);
                        p.Skill2 = s;
                    }
                }
                else if (res.Result.Emoji == threeEmoji)
                {
                    if (p.Skill3 is null)
                    {
                        await req.ModifyAsync(msg.WithTitle($"Skill {s.Name} learned in slot three").WithColor(Globals.ColorGreen).Build());
                        p.Skill3 = s;
                    }
                    else
                    {
                        await req.ModifyAsync(msg
                            .WithTitle($"Skill {s.Name} learned in slot three")
                            .WithDescription($"NOTE: you already had a skill in this slot, replacing current skill {p.Skill3.Name} with {s.Name}")
                            .WithColor(Globals.ColorGreen)
                            .Build());

                        p.KnownSkills.Add(p.Skill3);
                        p.Skill3 = s;
                    }
                }
            }
            else
            {
                await ctx.RespondAsync(msg.WithDescription("Timed out - no changes were made").WithColor(Globals.ColorRed).Build());
            }

            p.KnownSkills.RemoveAt(index);
            await req.DeleteAllReactionsAsync();
            p.BusyBlockEnd();
        }

        [Command("unlearn"), Description("Unlearn a currently learned skill and store the skill back in your known skills collection")]
        public async Task Unlearn(CommandContext ctx)
        {
            if (await PlayerNotInitedOrBusy(ctx))
            {
                return;
            }

            Player p = Players.Data[ctx.User.Id];
            p.BusyBlockStart();

            DiscordEmbedBuilder msg = new() { Title = "Unlearning skill...", Description = "Choose a skill slot to unlearn its skill from", Color = Globals.ColorBlue };
            DiscordMessage req = await ctx.RespondAsync(msg);
            DiscordEmoji oneEmoji = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji twoEmoji = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji threeEmoji = DiscordEmoji.FromName(ctx.Client, ":three:");


            await req.CreateReactionAsync(oneEmoji);
            await req.CreateReactionAsync(twoEmoji);
            await req.CreateReactionAsync(threeEmoji);

            InteractivityResult<MessageReactionAddEventArgs> res = await req.WaitForReactionAsync(ctx.Member);

            if (!res.TimedOut)
            {
                if (res.Result.Emoji == oneEmoji)
                {
                    if (p.Skill1 is not null)
                    {
                         await req.ModifyAsync(msg.WithDescription($"Skill {p.Skill1.Name} unlearned in slot one").WithColor(Globals.ColorGreen).Build());
                        p.KnownSkills.Add(p.Skill1);
                        p.Skill1 = null;
                    }
                    else
                    {
                         await req.ModifyAsync(msg.WithDescription($"There is no skill to unlearn in slot one - no changes made").WithColor(Globals.ColorRed).Build());
                    }
                }
                else if (res.Result.Emoji == twoEmoji)
                {
                    if (p.Skill2 is not null)
                    {
                         await req.ModifyAsync(msg.WithDescription($"Skill {p.Skill2.Name} unlearned in slot two").WithColor(Globals.ColorGreen).Build());
                        p.KnownSkills.Add(p.Skill2);
                        p.Skill2 = null;
                    }
                    else
                    {
                         await req.ModifyAsync(msg.WithDescription($"There is no skill to unlearn in slot two - no changes made").WithColor(Globals.ColorRed).Build());
                    }
                }
                else if (res.Result.Emoji == threeEmoji)
                {
                    if (p.Skill3 is not null)
                    {
                         await req.ModifyAsync(msg.WithDescription($"Skill {p.Skill3.Name} unlearned in slot three").WithColor(Globals.ColorGreen).Build());
                        p.KnownSkills.Add(p.Skill3);
                        p.Skill3 = null;
                    }
                    else
                    {
                         await req.ModifyAsync(msg.WithDescription($"There is no skill to unlearn in slot three - no changes made").WithColor(Globals.ColorRed).Build());
                    }
                }
            }
            else
            {
                await req.ModifyAsync(msg.WithDescription("Timed out - no changes were made").WithColor(Globals.ColorRed).Build());
            }

            await req.DeleteAllReactionsAsync();
            p.BusyBlockEnd();
        }
    }
}
