using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RandomDuelsAndStuffBot
{
    public sealed class CustomHelpFormatter : BaseHelpFormatter
    {
        // color used is blurple
        private readonly DiscordEmbedBuilder Message = new() { Color = new DiscordColor(0x5865F2) };

        public CustomHelpFormatter(CommandContext ctx) : base(ctx)
        {
        }

        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            if (cmd is CommandGroup)
            {
                _ = Message.WithTitle($"Group `{cmd.Name}`")
                    .WithDescription(cmd.Description);
                return this;
            }

            _ = Message.WithTitle($"Command `{cmd.Name}`")
                .WithDescription(cmd.Description);


            string args = string.Empty;
            foreach (CommandOverload i in cmd.Overloads)
            {
                if (i.Arguments.Count == 0)
                {
                    continue;
                }

                foreach (string x in i.Arguments.Select(a => $"`{a.Name}` ({a.Description}){Globals.NL}"))
                {
                    args += $"{x} ";
                }
            }

            if (!string.IsNullOrEmpty(args))
            {
                _ = Message.AddField("Arguments", args);
            }

            string aliasesList = string.Empty;

            foreach (string i in cmd.Aliases)
            {
                aliasesList += $"`{i}` ";
            }

            if (!string.IsNullOrEmpty(aliasesList))
            {
                _ = Message.AddField("Aliases", aliasesList);
            }

            return this;
        }
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            _ = Message.WithTitle("Available commands");

            string cmds = string.Empty;
            string groups = string.Empty;

            foreach (Command i in subcommands)
            {
                if (i is CommandGroup)
                {
                    groups += $"`{i.Name}` ";
                    continue;
                }

                cmds += $"`{i.Name}`: {i.Description}{Globals.NL}";
            }

            _ = Message.WithDescription(cmds);

            if (!string.IsNullOrEmpty(groups))
            {
                _ = Message.AddField("Available groups", groups);
            }

            return this;
        }

        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(null, Message.Build());
        }
    }
}
