using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
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
            _client.ButtonExecuted += _client_ButtonExecuted;
            //for buttons your don't need this
            //_client.SlashCommandExecuted += _discord_SlashCommandExecuted;
            _client.GuildMemberUpdated += HandleGuildMemberUpdated;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _sp);
        }

        private async Task _client_ButtonExecuted(SocketMessageComponent button)
        {
            if (button.User.Id == _client.CurrentUser.Id) return;
            var _user = button.User;

            SocketTextChannel textChannel = button.Channel as SocketTextChannel;

            if (textChannel == null)
            {
                return;
            }

            if (button.Data.CustomId == "join") //or anything that you want
            {
                DbUser user = await _databaseService.GetUserInGuild(_user.Id, textChannel.Guild.Id);
                if (user == null)
                {
                    user = await _databaseService.RegisterUserAsync(textChannel.Guild.Id, _client.GetUser(_user.Id).Username, _user.Id);
                }
                if (await _queueService.AddToPugQueue(button.Message.Id, user, textChannel.Guild.Id))
                {
                    IMessage message = await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, button.Channel.Id, button.Message.Id);
                    await Task.Delay(250);
                    //await message.RemoveReactionAsync(new Emoji("✅"), _client.GetUser(reaction.UserId));
                    //  await button.User.SendMessageAsync(_client.GetUser(_user.Id).Mention + ", You Are Joined Queue");
                    await button.UpdateAsync(x =>
                    {
                        //x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = null;
                    });
                }
            }
            else if (button.Data.CustomId == "exit")
            {
                bool queueFound = true;
                if (_queueService.UserExistsInQueue(button.Message.Id, _user.Id))
                {
                    queueFound = await _queueService.RemoveFromPugQueue(button.Message.Id, _user.Id);
                }
                if (queueFound)
                {
                    //your should modify the message with new componnets instead of removing reactions
                    await (await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, button.Channel.Id, button.Message.Id)).RemoveReactionAsync(new Emoji("❌"), _client.GetUser(_user.Id));
                    //example
                    //await (button.Channel.SendMessageAsync(_client.GetUser(_user.Id).Mention + ", You Are Removed From Queue")).DeleteMessageAfterSeconds(2);

                   // await (await button.Channel.SendMessageAsync(base.Context.User.Mention + " I could not find a queue by the name: " )).DeleteMessageAfterSeconds(8);
                    await (await ReplyAsync(base.Context.User.Mention + " has cleared the queue: " + queueName)).DeleteMessageAfterSeconds(8);

                    await button.UpdateAsync(x =>
                    {
                        //x.Content = "text"; //text
                       // x.Components = null;
                       // x.Embed = null;
                    });
                }
            }


        }
        private async Task HandleGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketUser arg2)
        {
            if ((arg2.Status == UserStatus.Offline || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.Idle) && await _queueService.HandleUserWentAFK(arg2))
            {
                await arg2.SendMessageAsync("Hi, it looks like you went AFK or offline in Discord. You have been removed from any PUG queues you were in. If this was not intentional, please make sure your status in discord is set to `online`. Thank you!");
            }
        }

        private async Task C_GuildMemberUpdated(Cacheable<Discord.WebSocket.SocketGuildUser, ulong> arg1, Discord.WebSocket.SocketGuildUser arg2)
        {

            if ((arg2.Status == UserStatus.Offline || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.Idle) && await _queueService.HandleUserWentAFK(arg2))
            {
                await arg2.SendMessageAsync("Hi, it looks like you went AFK or offline in Discord. You have been removed from any PUG queues you were in. If this was not intentional, please make sure your status in discord is set to `online`. Thank you!");
            }
        }
        private async Task _discord_SlashCommandExecuted(SocketSlashCommand interaction)
        {
            if (interaction.User.Id != _client.CurrentUser.Id)
            {
                return;
            }
            //ISocketMessageChannel channel;
            // SocketTextChannel textChannel = channel as SocketTextChannel;
            // if (textChannel == null)
            // {
            //     return;
            //}
        }
        //private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        //{

        //}

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
