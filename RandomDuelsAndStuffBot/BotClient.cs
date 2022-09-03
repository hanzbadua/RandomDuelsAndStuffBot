using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using RandomDuelsAndStuffBot.Commands;
using RandomDuelsAndStuffBot.Players;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RandomDuelsAndStuffBot
{
    public sealed class BotClient
    {
        private static async Task Main()
        {
            await new BotClient().RunBotAsync();
        }

        private readonly EventId _loggingEvent = new(0, "BotLogging");
        private DiscordClient _client;
        private CommandsNextExtension _cmds;

        public async Task RunBotAsync()
        {
            if (!File.Exists("token.txt"))
            {
                throw new FileNotFoundException($"File 'token.txt' which should contain the bot token to log in to appears to not exist... exiting...");
            }

            string token = await File.ReadAllTextAsync("token.txt");

            // create a bot client config incl. token
            DiscordConfiguration config = new()
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.AllUnprivileged
            };

            // instantiate our bot client
            _client = new DiscordClient(config);

            // allow our bot client to be 'interactive' (use respond-by-message, respond-by-reaction)
            _client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // register events related to our client
            _client.Ready += OnReady;
            _client.GuildAvailable += OnGuildAvailable;
            _client.ClientErrored += OnClientError;

            // dependency injection
            ServiceProvider services = new ServiceCollection()
                .AddSingleton<PlayerData>()
                .BuildServiceProvider();

            // create command configuration
            CommandsNextConfiguration commandConfig = new()
            {
                StringPrefixes = new[] { "$" },
                EnableDms = false,
                EnableMentionPrefix = false,
                Services = services
            };

            // register command config to our bot client
            _cmds = _client.UseCommandsNext(commandConfig);

            // register events related to our commands
            _cmds.CommandExecuted += OnCommandExecute;
            _cmds.CommandErrored += OnCommandError;

            // register commands
            _cmds.RegisterCommands<MainCommands>();
            _cmds.RegisterCommands<InventoryCommands>();
            _cmds.RegisterCommands<SkillCommands>();
            _cmds.RegisterCommands<DebugCommands>();

            // use custom help formatter
            _cmds.SetHelpFormatter<CustomHelpFormatter>();

            // connect the client + log in
            await _client.ConnectAsync();

            // run the bot client permanently in background, until close
            await Task.Delay(-1);
        }

        // "Async method lacks 'await' operators and will run synchronously" warning suppression
        // we don't care if these run synchronously - we're just matching the required event method signature
#pragma warning disable CS1998
        private async Task OnReady(DiscordClient dc, ReadyEventArgs args)
        {
            dc.Logger.LogInformation(_loggingEvent, "_client is ready to process events");
            return;
        }

        private async Task OnGuildAvailable(DiscordClient dc, GuildCreateEventArgs args)
        {
            dc.Logger.LogInformation(_loggingEvent, $"Available guild: {args.Guild.Name}");
            return;
        }

        private async Task OnClientError(DiscordClient dc, ClientErrorEventArgs args)
        {
            dc.Logger.LogError(_loggingEvent, args.Exception, "Exception occured");
            return;
        }

        private async Task OnCommandExecute(CommandsNextExtension cmds, CommandExecutionEventArgs args)
        {
            args.Context.Client.Logger.LogInformation(_loggingEvent, $"{args.Context.User.Username} successfully executed '{args.Command.QualifiedName}'");
            return;
        }

        private async Task OnCommandError(CommandsNextExtension cmds, CommandErrorEventArgs args)
        {
            args.Context.Client.Logger.LogError(_loggingEvent, $"{args.Context.User.Username} tried executing '{args.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {args.Exception.GetType()}: {args.Exception.Message ?? "<no message>"}", DateTime.Now);
            // command doesn't exist / invalid args etc
            if (args.Exception is CommandNotFoundException || args.Exception is ArgumentException || args.Exception is InvalidOperationException)
            {
                await args.Context.Message.CreateReactionAsync(DiscordEmoji.FromName(cmds.Client, ":grey_question:"));
            }
            // permissions not valid
            else if (args.Exception is ChecksFailedException)
            {
                await args.Context.RespondAsync(new DiscordEmbedBuilder()
                {
                    Title = "Access denied",
                    Description = "You do not have the permissions required to use this command",
                    Color = Globals.ColorRed
                });
            }
        }
#pragma warning restore CS1998 
    }
}
