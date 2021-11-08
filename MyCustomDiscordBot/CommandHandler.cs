using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyCustomDiscordBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;

        private readonly CommandService _commands;

        private readonly IServiceProvider _sp;

        private readonly GlobalServersService _globalServersService;

        private readonly DatabaseService _databaseService;

        private readonly QueueService _queueService;

        private readonly UtilityService _utilityService;

        private readonly ILogger<Worker> _logger;

        public CommandHandler(IServiceProvider sp, Discord.WebSocket.DiscordSocketClient client, CommandService commandService, GlobalServersService globalServersService, DatabaseService databaseService, QueueService queueService, UtilityService utilityService, ILogger<Worker> logger)
        {
            _client = client;
            //_client = new DiscordSocketClient();
            _commands = commandService;
            _sp = sp;
            _logger = logger;
            _globalServersService = globalServersService;
            _databaseService = databaseService;
            _queueService = queueService;
            _utilityService = utilityService;
        }

        public async Task Init()
        {
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += HandleJoinedGuild;
            _client.Connected += HandleConnected;
            _client.ReactionAdded += HandleReactionAdded;
            _client.GuildMemberUpdated += HandleGuildMemberUpdated;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _sp);
        }

        private async Task HandleGuildMemberUpdated(SocketUser arg1, SocketUser arg2)
        {
            if ((arg2.Status == UserStatus.Offline || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.Idle) && await _queueService.HandleUserWentAFK(arg2))
            {
                await arg2.SendMessageAsync("Hi, it looks like you went AFK or offline in Discord. You have been removed from any PUG queues you were in. If this was not intentional, please make sure your status in discord is set to `online`. Thank you!");
            } 
        }

        private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == _client.CurrentUser.Id)
            {
                return;
            }
            SocketTextChannel textChannel = channel as SocketTextChannel;
            if (textChannel == null)
            {
                return;
            }
            if (reaction.Emote.Name == "✅")
            {
                DbUser user = await _databaseService.GetUserInGuild(reaction.UserId, textChannel.Guild.Id);
                if (user == null)
                {
                    user = await _databaseService.RegisterUserAsync(textChannel.Guild.Id, _client.GetUser(reaction.UserId).Username, reaction.UserId);
                }
                if (await _queueService.AddToPugQueue(reaction.MessageId, user, textChannel.Guild.Id))
                {
                    IMessage message = await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, channel.Id, reaction.MessageId);
                    await Task.Delay(250);
                    await message.RemoveReactionAsync(new Emoji("✅"), _client.GetUser(reaction.UserId));
                }
            }
            else if (reaction.Emote.Name == "❌")
            {
                bool queueFound = true;
                if (_queueService.UserExistsInQueue(reaction.MessageId, reaction.UserId))
                {
                    queueFound = await _queueService.RemoveFromPugQueue(reaction.MessageId, reaction.UserId);
                }
                if (queueFound)
                {
                    await (await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, channel.Id, reaction.MessageId)).RemoveReactionAsync(new Emoji("❌"), _client.GetUser(reaction.UserId));
                }
            }
        }

        private async Task HandleConnected()
        {
            foreach (SocketGuild guild in _client.Guilds)
            {
                _logger.LogInformation($"Connected to {guild.Name}: ID: {guild.Id}");
                await _globalServersService.ConfigureNewServer(guild.Id);
                ServerConfig config = await _databaseService.GetServerConfigAsync(guild.Id);
                if (config.QueueConfigs.Count > 0)
                {
                    foreach (QueueConfig qConfig in config.QueueConfigs)
                    {
                        _queueService.CreatePugQueue(qConfig);
                    }
                }
                guild.TextChannels.FirstOrDefault();
            }
            _logger.LogInformation($"Connected to {_client.Guilds.Count} total guilds.");
        }

        private async Task HandleJoinedGuild(SocketGuild arg)
        {
            await _globalServersService.ConfigureNewServer(arg.Id);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message != null)
            {
                int argPos = 0;
                if ((message.HasCharPrefix(',', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) && !message.Author.IsBot)
                {
                    SocketCommandContext context = new SocketCommandContext(_client, message);
                    await _commands.ExecuteAsync(context, argPos, _sp);
                }
            }
        }
    }
}
