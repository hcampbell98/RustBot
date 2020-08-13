using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Addons.Interactive;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RustBot.Logging;
using System.Diagnostics;
using System.Linq;
using RustBot.Users.Teams;
using RustBot.Users.Guilds;

namespace SSRPBalanceBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public static DiscordSocketClient _client;
        public static CommandService _commands;
        public static IServiceProvider _services;

        public static List<DateTimeOffset> stackCooldownTimer = new List<DateTimeOffset>();
        public static List<SocketGuildUser> stackCooldownTarget = new List<SocketGuildUser>();

        public static int messageCooldown = 2;
        public static string prefix = "r!";

        public static List<NextRoll> nextRolls = new List<NextRoll> { };

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = ConfigureServices();

            _client.Log += Log;
            _client.JoinedGuild += GuildJoinedHandler;
            _client.LeftGuild += GuildLeftHandler;
            _client.Ready += _client_Ready;
            _commands.CommandExecuted += _commands_CommandExecuted;

            await InstallCommandsAsync();
            var token = File.ReadAllText("Config/token.cfg");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            TeamUtils.teamData = TeamUtils.LoadTeams();
            TeamUtils.userSettings = TeamUtils.LoadSettings();
            GuildUtils.guildData = GuildUtils.LoadSettings();
            LoggingUtils.apiKey = File.ReadAllText("Config/topggToken.cfg");
            await LoggingUtils.UpdateStats();

            await Task.Delay(-1);
        }

        private Task _commands_CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult arg3)
        {
            Statistics.runCommands += 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!arg3.IsSuccess && arg3.ErrorReason != "Unknown command.") 
            { 
                Console.WriteLine($"{arg3.ErrorReason}");

                Embed eb = Utilities.GetEmbedMessage("Error", "Reason", $"{arg3.ErrorReason} Try r!help {command.Value.Name}.", (SocketUser)context.User, Color.Red, Utilities.GetFooter((SocketUser)context.User, sw));

                context.Channel.SendMessageAsync("", false, eb); 
                return Task.CompletedTask; 
            }

            SocketCommandContext cntxt = context as SocketCommandContext;
            string serverName;

            //Ff the source channel is a DM
            if (cntxt.IsPrivate) { serverName = "Private Channel"; }
            else { serverName = context.Guild.Name; }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Time: {DateTime.Now} | Ran command: [{command.Value.Name}] | Called by: {context.Message.Author} | Server: {serverName}");
            Console.ForegroundColor = ConsoleColor.Gray;

            return Task.CompletedTask;
        }

        private async Task<Task> _client_Ready()
        {
            await _client.SetGameAsync($"{prefix}help | {_client.Guilds.Count} Servers");
            return Task.CompletedTask;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: _services);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            Statistics.messagesRead += 1;
            var message = messageParam as SocketUserMessage;

            var context = new SocketCommandContext(_client, message);
            if (message == null) return;
            if (message.Author.IsBot) { return; }
            int argPos = 0;

            await LiveLog(message);

            await LoggingUtils.Log(message, DateTime.Now, context.IsPrivate);

            if (!(message.HasStringPrefix(prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

            ulong channelID = GuildUtils.GetSettings(context.Guild.Id).ChannelID;
            if (channelID != default(ulong)) { if (context.Message.Channel.Id != channelID) { await context.Channel.SendMessageAsync($"The bot only responds to commands in channel: <#{channelID}>"); return; } }

            if (!await CheckCooldown(message))
            {
                var result = await _commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _services);

                //If the command run doesn't exist, the error message won't be thrown.
                //if (!result.IsSuccess && result.ErrorReason != "Unknown command.") { await context.Channel.SendMessageAsync($"Check the syntax of your command and try again. Try the {prefix}help docs."); Console.WriteLine(result.ErrorReason); }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task<Task> LiveLog(SocketMessage message)
        {
            var context = new SocketCommandContext(_client, (SocketUserMessage)message);

            if (LoggingUtils.livelogGuild != null && LoggingUtils.livelogOutput != null)
            {
                if (context.Guild == LoggingUtils.livelogGuild) 
                {
                    if (message.Content == "") { await LoggingUtils.livelogOutput.SendMessageAsync($"*[{message.Channel.Name}]* **{message.Author.Username}** - {message.Attachments.First().Url}"); }
                    else{ await LoggingUtils.livelogOutput.SendMessageAsync($"*[{message.Channel.Name}]* **{message.Author.Username}** - {message.Content}"); }
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Task> GuildJoinedHandler(SocketGuild g)
        {
            await LoggingUtils.UpdateStats();
            Statistics.guildChanges += 1;
            Statistics.lastGuildJoined = g.Name;

            await LoggingUtils.GuildJoinedAlert(g);

            await _client.SetGameAsync($"{prefix}help | {_client.Guilds.Count} Servers");
            return Task.CompletedTask;
        }

        private async Task<Task> GuildLeftHandler(SocketGuild g)
        {
            await LoggingUtils.UpdateStats();
            Statistics.guildChanges -= 1;

            await _client.SetGameAsync($"{prefix}help | {_client.Guilds.Count} Servers");
            return Task.CompletedTask;
        }

        private async Task<bool> CheckCooldown(SocketUserMessage message)
        {
            bool cooldown;
            //Check if your user list contains who just used that command.
            if (Program.stackCooldownTarget.Contains(message.Author as SocketGuildUser))
            {
                //If they have used this command before, take the time the user last did something, add 5 seconds, and see if it's greater than this very moment.
                if (Program.stackCooldownTimer[Program.stackCooldownTarget.IndexOf(message.Author as SocketGuildUser)].AddSeconds(messageCooldown) >= DateTimeOffset.Now)
                {
                    //If enough time hasn't passed, reply letting them know how much longer they need to wait, and end the code.
                    int secondsLeft = (int)(Program.stackCooldownTimer[Program.stackCooldownTarget.IndexOf(message.Author as SocketGuildUser)].AddSeconds(messageCooldown) - DateTimeOffset.Now).TotalSeconds;

                    if (secondsLeft > 0) { await message.Author.SendMessageAsync($"You have to wait {secondsLeft} seconds before you can use that command again!"); }
                    cooldown = true;
                }
                else
                {
                    //If enough time has passed, set the time for the user to right now.
                    Program.stackCooldownTimer[Program.stackCooldownTarget.IndexOf(message.Author as SocketGuildUser)] = DateTimeOffset.Now;
                    cooldown = false;
                }
                return cooldown;
            }
            else
            {
                //If they've never used this command before, add their username and when they just used this command.
                Program.stackCooldownTarget.Add(message.Author as SocketGuildUser);
                Program.stackCooldownTimer.Add(DateTimeOffset.Now);
                return false;
            }
        }
    }
}