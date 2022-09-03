using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Enemies;
using RandomDuelsAndStuffBot.Players;
using RandomDuelsAndStuffBot.Skills;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot.Combat
{
    public static class CombatRoutines
    {
        // we can improve this...
        public static async Task ExecutePVERoutine(CommandContext ctx, Player p, ResolvedEnemy e)
        {
            DiscordEmoji swordEmoji = DiscordEmoji.FromName(ctx.Client, ":crossed_swords:");
            DiscordEmoji oneEmoji = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji twoEmoji = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji threeEmoji = DiscordEmoji.FromName(ctx.Client, ":three:");

            ResolvedPlayerStats s = p.GetCombinedStats();

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Encounter: {e.Name}",
                Description = $"{e.Name} approaches you! What do you do?",
                Color = Globals.ColorBlue
            };

            DiscordMessage resp = await ctx.RespondAsync(embed.Build());

            await resp.CreateReactionAsync(swordEmoji);

            if (p.Skill1 is not null)
            {
                await resp.CreateReactionAsync(oneEmoji);
            }

            if (p.Skill2 is not null)
            {
                await resp.CreateReactionAsync(twoEmoji);
            }

            if (p.Skill3 is not null)
            {
                await resp.CreateReactionAsync(threeEmoji);
            }

            TempPlayerCombatData tmp = new(s.MaxHealth, s.MaxMana);
            TempEnemyCombatData tme = new(e.MaxHealth);

            while (true)
            {
                // Player's turn
                UpdateDisplayValues(ctx, embed, s, e, tmp, tme);

                await resp.ModifyAsync(embed.Build());
                InteractivityResult<MessageReactionAddEventArgs> result = await resp.WaitForReactionAsync(ctx.Member);
                if (!result.TimedOut)
                {
                    if (result.Result.Emoji == swordEmoji)
                    {
                        CombatMath.GetBasicAttack(s, e, out bool crit, out int aa);
                        tme.Health -= aa;
                        embed.WithDescription(crit ? $"You critically striked {e.Name} for {aa} damage!" : $"You deal {aa} damage to {e.Name}");
                        await resp.DeleteReactionAsync(swordEmoji, ctx.User);
                    }
                    else if (result.Result.Emoji == oneEmoji && p.Skill1 is not null)
                    {
                        if (tmp.Mana < p.Skill1.ManaCost)
                        {
                            embed.WithDescription($"You don't have enough mana to cast {p.Skill1.Name} ({tmp.Mana}/{p.Skill1.ManaCost}), please choose another action");
                            await resp.DeleteReactionAsync(oneEmoji, ctx.User);
                            continue;
                        }

                        tmp.Mana -= p.Skill1.ManaCost;

                        p.Skill1.Effect(embed, s, e, tme);
                        await resp.DeleteReactionAsync(oneEmoji, ctx.User);
                    }
                    else if (result.Result.Emoji == twoEmoji && p.Skill2 is not null)
                    {
                        if (tmp.Mana < p.Skill2.ManaCost)
                        {
                            embed.WithDescription($"You don't have enough mana to cast {p.Skill2.Name} ({tmp.Mana}/{p.Skill2.ManaCost}), please choose another action");
                            await resp.DeleteReactionAsync(twoEmoji, ctx.User);
                            continue;
                        }

                        tmp.Mana -= p.Skill2.ManaCost;

                        p.Skill2.Effect(embed, s, e, tme);
                        await resp.DeleteReactionAsync(twoEmoji, ctx.User);
                    }
                    else if (result.Result.Emoji == threeEmoji && p.Skill3 is not null)
                    {
                        if (tmp.Mana < p.Skill3.ManaCost)
                        {
                            embed.WithDescription($"You don't have enough mana to cast {p.Skill3.Name} ({tmp.Mana}/{p.Skill3.ManaCost}), please choose another action");
                            await resp.DeleteReactionAsync(threeEmoji, ctx.User);
                            continue;
                        }

                        tmp.Mana -= p.Skill3.ManaCost;

                        p.Skill3.Effect(embed, s, e, tme);
                        await resp.DeleteReactionAsync(threeEmoji, ctx.User);
                    }
                }
                else
                {
                    await resp.ModifyAsync(new DiscordEmbedBuilder
                    {
                        Title = "Encounter timed out. You took too long to make a decision - no rewards given",
                        Color = Globals.ColorRed
                    }.Build());
                    await resp.DeleteAllReactionsAsync();
                    return;
                }

                if (await CheckForWinOrLossPVE(ctx, embed, resp, s, e, tmp, tme))
                {
                    return;
                }

                // Enemy's turn
                UpdateDisplayValues(ctx, embed, s, e, tmp, tme);
                await resp.ModifyAsync(embed.Build());

                int dmg = Globals.Rng.Next(e.AttackDamage - (e.AttackDamage * 25 / 100), e.AttackDamage + (e.AttackDamage * 25 / 100)) - s.Armor;
                tmp.Health -= dmg;

                if (await CheckForWinOrLossPVE(ctx, embed, resp, s, e, tmp, tme))
                {
                    return;
                }

                embed.AppendToDescription($"{e.Name} did {dmg} to you");
            }
        }

        // TODO TODO TODO TODO TODO XD
        // We'll implement this some other day
        // Other player handling stuff such as validation checks will be done outside of this function
        public static async Task ExecutePVPRoutine(CommandContext ctx, Player p1, Player p2, TempPVPCombatData pvpTemp)
        {
            DiscordEmoji swordEmoji = DiscordEmoji.FromName(ctx.Client, ":crossed_swords:");
            DiscordEmoji xEmoji = DiscordEmoji.FromName(ctx.Client, ":x:");
            DiscordEmoji oneEmoji = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji twoEmoji = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji threeEmoji = DiscordEmoji.FromName(ctx.Client, ":three:");

            ResolvedPlayerStats s = p1.GetCombinedStats();
            ResolvedPlayerStats u = p2.GetCombinedStats();

            DiscordEmbedBuilder embed = new()
            {
                Title = $"Duel: {pvpTemp.User1.Username} vs {pvpTemp.User2.Username}",
                Description = $"{pvpTemp.User2.Username}: if you want to accept this duel challenge, please react with {swordEmoji}{Globals.NL}To decline, please react with {xEmoji}",
                Color = Globals.ColorBlue
            };

            DiscordMessage resp = await ctx.RespondAsync(embed.Build());
            await resp.CreateReactionAsync(swordEmoji);
            await resp.CreateReactionAsync(xEmoji);
            InteractivityResult<MessageReactionAddEventArgs> d = await resp.WaitForReactionAsync(pvpTemp.User2);
            if (!d.TimedOut)
            {
                if (d.Result.Emoji == swordEmoji)
                {
                    await resp.ModifyAsync(embed.WithDescription($"{pvpTemp.User2.Username} accepted the duel!").Build());
                    await resp.DeleteAllReactionsAsync();
                } else if (d.Result.Emoji == xEmoji)
                {
                    await resp.ModifyAsync(embed.WithDescription($"{pvpTemp.User2.Username} declined the duel...").WithColor(Globals.ColorRed).Build());
                    await resp.DeleteAllReactionsAsync();
                    return;
                }
            }
            else
            {
                await resp.ModifyAsync(new DiscordEmbedBuilder
                {
                    Title = "Encounter timed out. You took too long to make a decision - no rewards given",
                    Color = Globals.ColorRed
                }.Build());
                await resp.DeleteAllReactionsAsync();
                return;
            }
        }

        private static void UpdateDisplayValues(CommandContext ctx, DiscordEmbedBuilder msg, ResolvedPlayerStats p, ResolvedEnemy e, TempPlayerCombatData tempPlayer, TempEnemyCombatData tempEnemy)
        {
            int strippedEnemyAr = e.Armor - (e.Armor * p.ArmorPenPercent / 100) - p.ArmorPenFlat;
            int strippedEnemyMr = e.MagicResist - (e.MagicResist * p.MagicPenPercent / 100) - p.MagicPenFlat;

            if (strippedEnemyAr < 0)
            {
                strippedEnemyAr = 0;
            }

            if (strippedEnemyMr < 0)
            {
                strippedEnemyMr = 0;
            }

            if (tempPlayer.Health < 0)
            {
                tempPlayer.Health = 0;
            }

            if (tempEnemy.Health < 0)
            {
                tempEnemy.Health = 0;
            }

            // See footer note below
            // var swordEmoji = DiscordEmoji.FromName(ctx._client, ":crossed_swords:");
            // var oneEmoji = DiscordEmoji.FromName(ctx._client, ":one:");
            // var twoEmoji = DiscordEmoji.FromName(ctx._client, ":two:");
            // var threeEmoji = DiscordEmoji.FromName(ctx._client, ":three:");
            
            DiscordEmoji hpEmoji = DiscordEmoji.FromName(ctx.Client, ":heart:");
            DiscordEmoji manaEmoji = DiscordEmoji.FromName(ctx.Client, ":droplet:");
            DiscordEmoji adEmoji = DiscordEmoji.FromName(ctx.Client, ":knife:");
            DiscordEmoji apEmoji = DiscordEmoji.FromName(ctx.Client, ":star:");
            DiscordEmoji arEmoji = DiscordEmoji.FromName(ctx.Client, ":shield:");
            DiscordEmoji mrEmoji = DiscordEmoji.FromName(ctx.Client, ":zap:");

             msg.ClearFields()
                // Embed footers in Discord don't support Twemoji, so it looks kinda ugly...
                //.WithFooter($"Health {hpEmoji}, Mana {manaEmoji}, Attack Damage {adEmoji}, Ability Power {apEmoji}, Armor {arEmoji}, Magic Resist {mrEmoji}{NL}Basic Attack {swordEmoji}, Skills {oneEmoji}{twoEmoji}{threeEmoji}")
                .AddField("Your stats", $"{hpEmoji} {tempPlayer.Health}/{p.MaxHealth}{Globals.NL}{manaEmoji} {tempPlayer.Mana}/{p.MaxMana}{Globals.NL}{adEmoji} {p.AttackDamage}, {apEmoji} {p.AbilityPower}{Globals.NL}{arEmoji} {p.Armor}, {mrEmoji} {p.MagicResist}")
                .AddField($"{e.Name}'s stats", $"{hpEmoji} {tempEnemy.Health}/{e.MaxHealth}{Globals.NL}{adEmoji} {e.AttackDamage}, {apEmoji} {e.AbilityPower}{Globals.NL}{arEmoji} {e.Armor} ({strippedEnemyAr}), {mrEmoji} {e.MagicResist} ({strippedEnemyMr})");
        }


        // Returns true upon win or loss, false if combat will continue
        private static async Task<bool> CheckForWinOrLossPVE(CommandContext ctx, DiscordEmbedBuilder embed, DiscordMessage modify, ResolvedPlayerStats p, ResolvedEnemy e, TempPlayerCombatData tempPlayer, TempEnemyCombatData tempEnemy)
        {
            // Loss
            if (tempPlayer.Health <= 0)
            {
                embed.AppendToDescription($"You lost against {e.Name}... you got no rewards").WithColor(Globals.ColorRed);
                UpdateDisplayValues(ctx, embed, p, e, tempPlayer, tempEnemy);
                await modify.ModifyAsync(embed.Build());
                await modify.DeleteAllReactionsAsync();
                return true;
            }
            else if (tempEnemy.Health <= 0) // Win
            {
                embed.AppendToDescription($"You won fighting against {e.Name}! Implement rewards here...").WithColor(Globals.ColorGreen);
                UpdateDisplayValues(ctx, embed, p, e, tempPlayer, tempEnemy);
                await modify.ModifyAsync(embed.Build());
                await modify.DeleteAllReactionsAsync();
                return true;
            }

            return false;
        }
    }
}
