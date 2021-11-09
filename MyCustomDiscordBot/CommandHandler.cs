using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace MyCustomDiscordBot
{
    public class CommandHandler
    {

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly GlobalServersService _globalServersService;
        private readonly DatabaseService _databaseService;
        private readonly QueueService _queueService;
        private readonly UtilityService _utilityService;
        private readonly EmbedService _embedService;
        private readonly ILogger<Worker> _logger;
        private readonly MatchService _matchService;

        private readonly IServiceProvider _sp;

        public CommandHandler(IServiceProvider services)
        {

            this._sp = services;
            this._client = services.GetRequiredService<DiscordSocketClient>();
            this._commands = services.GetRequiredService<CommandService>();
            this._globalServersService = services.GetRequiredService<GlobalServersService>();
            this._databaseService = services.GetRequiredService<DatabaseService>();
            this._queueService = services.GetRequiredService<QueueService>();
            this._utilityService = services.GetRequiredService<UtilityService>();
            this._embedService = services.GetRequiredService<EmbedService>();
            this._matchService = services.GetRequiredService<MatchService>();
            this._logger = services.GetRequiredService<ILogger<Worker>>();
            //okay try now <3

        }
        //public CommandHandler(IServiceProvider sp, EmbedService test ,Discord.WebSocket.DiscordSocketClient client, CommandService commandService, GlobalServersService globalServersService, DatabaseService databaseService, QueueService queueService, UtilityService utilityService, ILogger<Worker> logger)
        //{

        //    _embedService = test;
        //    _client = client;
        //    //_client = new DiscordSocketClient();
        //    _commands = commandService;
        //    _sp = sp;
        //    _logger = logger;
        //    _globalServersService = globalServersService;
        //    _databaseService = databaseService;
        //    _queueService = queueService;
        //    _utilityService = utilityService; 


        //}

        public async Task Init()
        {
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += HandleJoinedGuild;
            _client.Connected += HandleConnected;
            _client.ButtonExecuted += _client_ButtonExecuted;
            //for buttons your don't need this
            //_client.SlashCommandExecuted += _discord_SlashCommandExecuted;
            _client.GuildMemberUpdated += C_GuildMemberUpdated;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _sp);
        }

        private async Task _client_ButtonExecuted(SocketMessageComponent interaction)
        {
            // id on't know why your Diagonstioc tool not working

            //await interaction.UpdateAsync(x =>
            //{
            //    x.Components = null;
            //    x.Embed = new EmbedBuilder() { Title = "wait.." }.Build();
            //    x.Content = $"<@{interaction.User.Id}>";
            //});
            //await interaction.DeferLoadingAsync(true); i am sure that its working right now test it

            if (interaction.User.Id == _client.CurrentUser.Id)
            {
                await interaction.UpdateAsync(x =>
                {
                    //  x.Content = "text"; //text
                    // x.Components = null;
                    // x.Embed = null;
                });
                return;

            }

            var _user = interaction.User;

            SocketTextChannel textChannel = interaction.Channel as SocketTextChannel;

            if (textChannel == null)
            {
                return;
                await interaction.UpdateAsync(x =>
                {
                    //  x.Content = "text"; //text
                    // x.Components = null;
                    // x.Embed = null;
                });
            }

            if (interaction.Data.CustomId == "join") //or anything that you want
            {
                await interaction.UpdateAsync(x =>
                {
                    //  x.Content = "text"; //text
                    // x.Components = null;
                    // x.Embed = null;
                });
                DbUser user = await _databaseService.GetUserInGuild(_user.Id, textChannel.Guild.Id);
                if (user == null)
                {
                    user = await _databaseService.RegisterUserAsync(textChannel.Guild.Id, _client.GetUser(_user.Id).Username, _user.Id);
                    await interaction.UpdateAsync(x =>
                    {
                        // x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = null;
                    });
                }
                if (await _queueService.AddToPugQueue(interaction.Message.Id, user, textChannel.Guild.Id))
                {
                    IMessage message = await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, interaction.Channel.Id, interaction.Message.Id);
                    await Task.Delay(250);
                    //await message.RemoveReactionAsync(new Emoji("✅"), _client.GetUser(reaction.UserId));
                    //  await button.User.SendMessageAsync(_client.GetUser(_user.Id).Mention + ", You Are Joined Queue");
                    await interaction.UpdateAsync(x =>
                    {
                        //x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = null;
                    });
                }

            }
            else if (interaction.Data.CustomId == "exit")
            {
                await interaction.UpdateAsync(x =>
                {
                    //x.Content = "text"; //text
                    // x.Components = null;
                    // x.Embed = null;
                });
                bool queueFound = true;
                if (_queueService.UserExistsInQueue(interaction.Message.Id, _user.Id))
                {
                    queueFound = await _queueService.RemoveFromPugQueue(interaction.Message.Id, _user.Id);
                    await interaction.UpdateAsync(x =>
                    {
                        //  x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = null;
                    });
                }
                if (queueFound)
                {
                    //your should modify the message with new componnets instead of removing reactions
                    await (await _utilityService.GetMessageFromTextChannel(textChannel.Guild.Id, interaction.Channel.Id, interaction.Message.Id)).RemoveReactionAsync(new Emoji("❌"), _client.GetUser(_user.Id));
                    //example
                    //await (button.Channel.SendMessageAsync(_client.GetUser(_user.Id).Mention + ", You Are Removed From Queue")).DeleteMessageAfterSeconds(2);

                    await (await interaction.Channel.SendMessageAsync(_client.GetUser(_user.Id).Mention + ", You Are Removed From Queue")).DeleteMessageAfterSeconds(2);
                    // await (await button.Channel.SendMessageAsync("")).DeleteMessageAfterSeconds(3);

                    await interaction.UpdateAsync(x =>
                    {
                        //x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = null;
                    });
                }
            }

            //    /////////////// Channge Map 


            if (interaction.Data.CustomId == "map")
            {
                //     var _user = button.User;

                SocketTextChannel VOTEChannel = interaction.Channel as SocketTextChannel;

                if (VOTEChannel == null)
                {
                    return;
                }
                Match match = await _databaseService.GetMatchForChannelAsync(VOTEChannel.Guild.Id, interaction.Channel.Id);
                if (match == null)
                {
                    await interaction.Channel.SendMessageAsync("This is not a match channel or something weird happened.");
                    return;
                }
                if (match.MapChangeVoteDiscordIds.Contains(_user.Id))
                {
                    await interaction.Channel.SendMessageAsync(_client.GetUser(_user.Id).Mention + " you have already voted to change the map.");
                    return;
                }
                match.MapChangeVoteDiscordIds.Add(_user.Id);
                int mapChangeVotesNeeded = _queueService.GetPugQueue(match.PugQueueMessageId).Capacity / 2 + 1;
                if (match.MapChangeVoteDiscordIds.Count < mapChangeVotesNeeded)
                {
                    await _databaseService.UpsertMatchAsync(VOTEChannel.Guild.Id, match);
                    _ = await interaction.Channel.SendMessageAsync(_client.GetUser(_user.Id).Mention + $" has voted to change the map. `{match.MapChangeVoteDiscordIds.Count} / {mapChangeVotesNeeded}`");
                }
                else
                {
                    foreach (QueueConfig qConfig in (await _databaseService.GetServerConfigAsync(VOTEChannel.Guild.Id)).QueueConfigs)
                    {
                        if (qConfig.MessageId == match.PugQueueMessageId)
                        {
                            Random randomer = new Random();
                            match.Map = qConfig.Maps[randomer.Next(qConfig.Maps.Count)];
                            match.MapChangeVoteDiscordIds.Clear();
                            break;
                        }
                    }
                    int amount = 1000;
                    await _databaseService.UpsertMatchAsync(VOTEChannel.Guild.Id, match);
                    IEnumerable<IMessage> messages = await VOTEChannel.GetMessagesAsync(amount + 1).FlattenAsync();
                    await ((ITextChannel)VOTEChannel).DeleteMessagesAsync(messages);
                    // const int delay = 3000;
                    // IUserMessage m = await ReplyAsync($"I have deleted {amount} messages for ya. :)");
                    //await Task.Delay(delay);
                    // await m.DeleteAsync();
                    ISocketMessageChannel channel2 = interaction.Channel; //i found the problem, your embed service is returned null
                  //  await channel2.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, VOTEChannel.Guild.Id)); // notworking i dont know why 
                    RestUserMessage blankEmbedMessage = await channel2.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, VOTEChannel.Guild.Id));



                    ButtonBuilder bl = new ButtonBuilder() { Label = "🏆-Team #1 WIN", IsDisabled = false, Style = ButtonStyle.Success, CustomId = "bl" };
                    ButtonBuilder gr = new ButtonBuilder() { Label = "🏆-Team #2 WIN", IsDisabled = false, Style = ButtonStyle.Success, CustomId = "gr" };
                    //ButtonBuilder map = new ButtonBuilder() { Label = "Change Map", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "map" };
                    ComponentBuilder componentBuilder = new ComponentBuilder()
                          .WithButton(bl)
                          .WithButton(gr);
                        //  .WithButton(map);
                    await blankEmbedMessage.ModifyAsync(x => x.Components = componentBuilder.Build());
                    await interaction.UpdateAsync(x =>
                    {
                        //    x.Content = "text"; //text
                        // x.Components = null;
                        // x.Embed = await _embedService.GetMatchEmbedAsync(match, VOTEChannel.Guild.Id);
                    });
                    await interaction.Channel.SendMessageAsync("The map vote has passed and the map has been changed to: `" + match.Map + "`!");

                    //await ReplyAsync(); //do them again ok


                }
                await interaction.Channel.DeleteMessageAsync(5);




            }




            //////////////// Change Map 


            //    /////////////// bl


            if (interaction.Data.CustomId == "bl")
            {
                //     var _user = button.User;
                SocketTextChannel BL = interaction.Channel as SocketTextChannel;
                int teamNumber = 1;
                ServerConfig config = await _databaseService.GetServerConfigAsync(BL.Guild.Id);
                SocketGuildUser author = interaction.User as SocketGuildUser;
                if (author != null)
                {
                    bool roleFound = false;
                    foreach (SocketRole role in author.Roles)
                    {
                        if (role.Id == config.ScoreReporterRoleId)
                        {
                            roleFound = true;
                            break;
                        }
                    }
                    if (!roleFound)
                    {
                        await interaction.Channel.SendMessageAsync(author.Mention + ", you do not have the score reporting role.");
                        await interaction.UpdateAsync(x =>
                        {
                            //    x.Content = "text"; //text
                            // x.Components = null;
                            // x.Embed = await _embedService.GetMatchEmbedAsync(match, VOTEChannel.Guild.Id);
                        });
                        return;
                    }
                }
                if (teamNumber != 1 && teamNumber != 2)
                {
                    await interaction.Channel.SendMessageAsync("Please specify either `1` or `2` only, with respect to the winning team.");

                    return;
                }
                Match match = await _databaseService.GetMatchForChannelAsync(BL.Guild.Id, interaction.Channel.Id);
                if (match.Winners == 1 || match.Winners == 2)
                {
                    await interaction.Channel.SendMessageAsync($"This match has already been reported as a team ${match.Winners} victory. You may use `.giveelo @User AMOUNT` and `.takeelo @User AMOUNT` to correct any mistakes.");
                    return;
                }
                if (match == null)
                {
                    await interaction.Channel.SendMessageAsync("Either this is not a match channel or something went wrong.");
                    return;
                }
                await _matchService.CalculateWinForMatch(match, teamNumber, BL.Guild.Id);
                SocketVoiceChannel waiting = BL.Guild.GetVoiceChannel(config.WaitingForMatchChannelId);
                foreach (ulong discordId in match.AllPlayerDiscordIds)
                {
                    SocketGuildUser moveMe = BL.Guild.GetUser(discordId);
                    if (moveMe != null && moveMe.VoiceChannel != null)
                    {
                        await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            //      x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)waiting;//can you fix ? 
                        });
                        await Task.Delay(250);
                    }
                }
                SocketVoiceChannel team1Voice = BL.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
                if (team1Voice != null)
                {
                    await team1Voice.DeleteAsync();
                }
                await Task.Delay(250);
                SocketVoiceChannel team2Voice = BL.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
                if (team2Voice != null)
                {
                    await team2Voice.DeleteAsync();
                }
                SocketTextChannel matchInfoChannel = interaction.Channel as SocketTextChannel;
                if (matchInfoChannel != null)
                {
                    await matchInfoChannel.DeleteAsync();
                }
                SocketTextChannel matchLogsChannel = BL.Guild.GetTextChannel(config.MatchLogsChannelId);
                SocketTextChannel socketTextChannel = matchLogsChannel;
                await socketTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.MatchLogEmbed(match, BL.Guild.Id));


            }




            //////////////// bl

            //    /////////////// bl


            if (interaction.Data.CustomId == "gr")
            {
                //     var _user = button.User;
                SocketTextChannel BL = interaction.Channel as SocketTextChannel;
                int teamNumber = 2;
                ServerConfig config = await _databaseService.GetServerConfigAsync(BL.Guild.Id);
                SocketGuildUser author = interaction.User as SocketGuildUser;
                if (author != null)
                {
                    bool roleFound = false;
                    foreach (SocketRole role in author.Roles)
                    {
                        if (role.Id == config.ScoreReporterRoleId)
                        {
                            roleFound = true;
                            break;
                        }
                    }
                    if (!roleFound)
                    {
                        await interaction.Channel.SendMessageAsync(author.Mention + ", you do not have the score reporting role.");
                        await interaction.UpdateAsync(x =>
                        {
                            //    x.Content = "text"; //text
                            // x.Components = null;
                            // x.Embed = await _embedService.GetMatchEmbedAsync(match, VOTEChannel.Guild.Id);
                        });
                        return;
                    }
                }
                if (teamNumber != 1 && teamNumber != 2)
                {
                    await interaction.Channel.SendMessageAsync("Please specify either `1` or `2` only, with respect to the winning team.");
                    return;
                }
                Match match = await _databaseService.GetMatchForChannelAsync(BL.Guild.Id, interaction.Channel.Id);
                if (match.Winners == 1 || match.Winners == 2)
                {
                    await interaction.Channel.SendMessageAsync($"This match has already been reported as a team ${match.Winners} victory. You may use `.giveelo @User AMOUNT` and `.takeelo @User AMOUNT` to correct any mistakes.");
                    return;
                }
                if (match == null)
                {
                    await interaction.Channel.SendMessageAsync("Either this is not a match channel or something went wrong.");
                    return;
                }
                await _matchService.CalculateWinForMatch(match, teamNumber, BL.Guild.Id);
                SocketVoiceChannel waiting = BL.Guild.GetVoiceChannel(config.WaitingForMatchChannelId);
                foreach (ulong discordId in match.AllPlayerDiscordIds)
                {
                    SocketGuildUser moveMe = BL.Guild.GetUser(discordId);
                    if (moveMe != null && moveMe.VoiceChannel != null)
                    {
                        await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            //      x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)waiting;//can you fix ? 
                        });
                        await Task.Delay(250);
                    }
                }
                SocketVoiceChannel team1Voice = BL.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
                if (team1Voice != null)
                {
                    await team1Voice.DeleteAsync();
                }
                await Task.Delay(250);
                SocketVoiceChannel team2Voice = BL.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
                if (team2Voice != null)
                {
                    await team2Voice.DeleteAsync();
                }
                SocketTextChannel matchInfoChannel = interaction.Channel as SocketTextChannel;
                if (matchInfoChannel != null)
                {
                    await matchInfoChannel.DeleteAsync();
                }
                SocketTextChannel matchLogsChannel = BL.Guild.GetTextChannel(config.MatchLogsChannelId);
                SocketTextChannel socketTextChannel = matchLogsChannel;
                await socketTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.MatchLogEmbed(match, BL.Guild.Id));


            }




            //////////////// bl

        }//you shoud set it first, no any ref



        private async Task HandleGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketUser arg2)
        {
            if ((arg2.Status == UserStatus.Offline || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.Idle) && await _queueService.HandleUserWentAFK(arg2))
            {
                await arg2.SendMessageAsync("Hi, it looks like you went AFK or offline in Discord. You have been removed from any PUG queues you were in. If this was not intentional, please make sure your status in discord is set to `online`. Thank you!");
            }
        }

        private async Task C_GuildMemberUpdated(Cacheable<Discord.WebSocket.SocketGuildUser, ulong> arg1, Discord.WebSocket.SocketGuildUser arg2)
        {

            if ((arg2.Status == UserStatus.Offline || arg2.Status == UserStatus.AFK || arg2.Status == UserStatus.Idle) && await _queueService.HandleUserWentAFK(arg2)) //working as well, the result was false
            {
                await arg2.SendMessageAsync("Hi, it looks like you went AFK or offline in Discord. You have been removed from any PUG queues you were in. If this was not intentional, please make sure your status in discord is set to `online`. Thank you!");
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
                if ((message.HasCharPrefix(Config.Prfix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) && !message.Author.IsBot)
                {
                    SocketCommandContext context = new SocketCommandContext(_client, message);
                    await _commands.ExecuteAsync(context, argPos, _sp);
                }
            }
        }
    }
}
