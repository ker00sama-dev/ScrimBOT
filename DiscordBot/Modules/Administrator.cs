using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using VMProtect;
using MyCustomDiscordBot.MyCustomDiscordBot;
using static MyCustomDiscordBot.MyCustomDiscordBot.DiscordBOTGaming;
using System.Collections.Generic;

namespace DiscordBot.Modules
{

    public class Administrator : ModuleBase<SocketCommandContext>
    {

        private readonly DatabaseService _databaseService;

        private readonly EmbedService _embedService;

        private readonly QueueService _queueService;

        private readonly UtilityService _utilityService;

        private readonly ILogger<Worker> _logger;

        private readonly ScrimService _scrimService;


        public Administrator(DatabaseService databaseService, EmbedService embedService, QueueService queueService, UtilityService utilityService, ILogger<Worker> logger, ScrimService scrimService)
        {

            _databaseService = databaseService;
            _embedService = embedService;
            _queueService = queueService;
            _utilityService = utilityService;
            _scrimService = scrimService;
            _logger = logger;
        }
        [Command("addreactions")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Add the reactions to a queue.")]
        public async Task AddReactionsToQueue(ulong messageId)
        {
     
#pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'ulong' is never equal to 'null' of type 'ulong?'
            if (messageId == null)
#pragma warning restore CS0472 // The result of the expression is always 'false' since a value of type 'ulong' is never equal to 'null' of type 'ulong?'
            {
                await ReplyAsync("I could not find a message by that Id.");

            }
            IMessage message = base.Context.Channel.GetCachedMessage(messageId);
            if (message == null)
            {
                message = await base.Context.Channel.GetMessageAsync(messageId);
            }
            if (message == null)
            {
                await ReplyAsync(base.Context.User.Mention + ", I could not find a message by that Id.");
                return;
            }
            await message.AddReactionAsync(new Emoji("✅"));
            await message.AddReactionAsync(new Emoji("❌"));
        }
        [Command("resetelo")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Add the reactions to a queue.")]
        public async Task resetelo()
        {
            await _databaseService.ResetUser(base.Context.Guild.Id, channel: base.Context.Channel, base.Context.User.Mention, base.Context.Guild);
         //   await ReplyAsync(@"All Users Has been Reset To ELO : ( 0 ) ");

        }

        [Command("setteamsort")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set the team sort mode; captains or elo.")]
        public async Task SetTeamSortMode(ulong queueMessageId, string preference)
        {
            if (preference.ToLower() != "captains" && preference.ToLower() != "elo")
            {
                await ReplyAsync("Please make sure your preference is only `captains` or `elo`.");
                return;
            }
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            int changeIndex = -1;
            for (int i = 0; i < config.QueueConfigs.Count; i++)
            {
                if (config.QueueConfigs[i].MessageId == queueMessageId)
                {
                    changeIndex = i;
                    break;
                }
            }
            SortType sortType = SortType.Elo;
            if (preference.ToLower() == "captains")
            {
                sortType = SortType.Captains;
            }
            else if (preference.ToLower() == "elo")
            {
                sortType = SortType.Elo;
            }
            if (changeIndex >= 0)
            {
                config.QueueConfigs[changeIndex].SortType = sortType;
                await _databaseService.UpsertServerConfigAsync(config);
                _queueService.UpdatePugQueue(config.QueueConfigs[changeIndex]);
                await ReplyAsync($"The {config.QueueConfigs[changeIndex].Name} queue's sort type is now: `{config.QueueConfigs[changeIndex].SortType}`");
                IMessage queueMessage = await _utilityService.GetMessageFromTextChannel(base.Context.Guild.Id, config.QueueConfigs[changeIndex].ChannelId, config.QueueConfigs[changeIndex].MessageId);
                if (queueMessage != null)
                {
                    IUserMessage editMe = queueMessage as IUserMessage;
                    if (editMe != null)
                    {
                        await editMe.ModifyAsync(delegate (MessageProperties x)
                        {
                            x.Embed = _embedService.GetQueueEmbed(_queueService.GetPugQueue(editMe.Id));
                        });
                    }
                }
                await base.Context.Message.DeleteAsync();
            }
            else
            {
                await ReplyAsync("I could not find a queue in this guild by that Id.");
            }
        }

        [Command("addmap")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Add a map to a queue's map pool.")]
        public async Task AddMapToPool(ulong messageId, [Remainder] string name)
        {
            if (name.Length <= 0 || name.Length > 16)
            {
                await ReplyAsync("Please make sure the map name has between 1 and 16 characters in length");
                return;
            }
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            QueueConfig qConfig = config.QueueConfigs.Find((QueueConfig x) => x.MessageId == messageId);
            foreach (string map in qConfig.Maps)
            {
                if (map.ToLower() == name.ToLower())
                {
                    await ReplyAsync("That map: `" + name + "` is already in the map pool for the " + qConfig.Name + " queue.");
                    return;
                }
            }
            qConfig.Maps.Add(name);
            await _databaseService.UpsertServerConfigAsync(config);
            _queueService.UpdatePugQueue(qConfig);
            await ReplyAsync(name + " has been added to the " + qConfig.Name + " queue's map pool.");
            await base.Context.Message.DeleteAsync();
        }

        [Command("maps")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("See a map pool for a queue.")]
        public async Task SeeMapPool(ulong messageId)
        {
            QueueConfig qConfig = (await _databaseService.GetServerConfigAsync(base.Context.Guild.Id)).QueueConfigs.Find((QueueConfig x) => x.MessageId == messageId);
            await ReplyAsync("Maps for `" + qConfig.Name + "` queue: " + string.Join(" ", qConfig.Maps));
     
        }

        [Command("clearqueue")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Clear a queue by its name")]
        public async Task ClearQueue(string queueName)
        {
            QueueConfig qConfig = (await _databaseService.GetServerConfigAsync(base.Context.Guild.Id)).QueueConfigs.Find((QueueConfig x) => x.Name.ToLower() == queueName.ToLower());
            if (qConfig == null)
            {
                await (await ReplyAsync(base.Context.User.Mention + " I could not find a queue by the name: " + queueName)).DeleteMessageAfterSeconds(8);
                return;
            }
            PugQueue queue = _queueService.GetPugQueue(qConfig.MessageId);
            queue.Users.Clear();
            Embed queueEmbed = _embedService.GetQueueEmbed(queue);
            await ((await _utilityService.GetMessageFromTextChannel(queue.GuildId, queue.ChannelId, queue.MessageId)) as IUserMessage).ModifyAsync(delegate (MessageProperties x)
            {
                x.Embed = queueEmbed;
            });
            await (await ReplyAsync(base.Context.User.Mention + " has cleared the queue: " + queueName)).DeleteMessageAfterSeconds(8);
        }

        [Command("removemap")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Remove a map from a queue's map pool.")]
        public async Task RemoveMapFromPool(ulong messageId, [Remainder] string name)
        {
            if (name.Length <= 0 || name.Length > 16)
            {
                await ReplyAsync("Please make sure the map name has between 1 and 16 characters in length");
                return;
            }
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            QueueConfig qConfig = config.QueueConfigs.Find((QueueConfig x) => x.MessageId == messageId);
            int removeIndex = -1;
            for (int i = 0; i < qConfig.Maps.Count; i++)
            {
                if (qConfig.Maps[i].ToLower() == name.ToLower())
                {
                    removeIndex = i;
                    break;
                }
            }
            if (removeIndex < 0)
            {
                await ReplyAsync(name + " was not in the map pool for the " + qConfig.Name + " queue.");
            }
            else
            {
                qConfig.Maps.RemoveAt(removeIndex);
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync(name + " has been removed from the " + qConfig.Name + " queue's map pool.");
            }
            await base.Context.Message.DeleteAsync();
        }

        [Command("deletequeue")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Delete a queue by its name.")]
        public async Task DeleteQueueByName(string name = null)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            int removeIndex = -1;
            for (int i = 0; i < config.QueueConfigs.Count; i++)
            {
                if (config.QueueConfigs[i].Name.ToLower() == name.ToLower())
                {
                    removeIndex = i;
                    break;
                }
            }
            if (removeIndex >= 0)
            {
                IMessage message = await _utilityService.GetMessageFromTextChannel(base.Context.Guild.Id, config.QueueConfigs[removeIndex].ChannelId, config.QueueConfigs[removeIndex].MessageId);
                if (message != null)
                {
                    await message.DeleteAsync();
                }
                config.QueueConfigs.RemoveAt(removeIndex);
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The " + name + " queue has been deleted.");
                await base.Context.Message.DeleteAsync();
            }
            else
            {
                await ReplyAsync("I could not find a queue in this server by the name of: `" + name + "`");
                await base.Context.Message.DeleteAsync();
            }
        }

        [Command("setscorereporter")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set the score reporter role.")]
        public async Task SetScoreReporterRole(SocketRole role)
        {
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.ScoreReporterRoleId = role.Id;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The Score Reporter role has been set to " + role.Mention);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting the score reporter role: \n\n" + e.Message);
                _logger.LogError("Error setting score reporter role in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }
        [Command("createqueue")]
        // [Remarks("prefix [new prefix]")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set up a reaction based queue in a given channel.")]
        public async Task CreateReactionQueueb(string name = null, int capacity = 0, string sortType = null)
        {

#pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
            if (name == null || capacity == null || sortType == null)
#pragma warning restore CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
            {
                ServerConfig config2 = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);

                await (await ReplyAsync($"{config2.prefix.ToString()}createqueue [name] [maximum users] [captains or elo ]")).DeleteMessageAfterSeconds(2);
                return;

            }

            if (string.IsNullOrEmpty(sortType))
            {
                throw new ArgumentException($"'{nameof(sortType)}' cannot be null or empty.", nameof(sortType));
            }

            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (config.MatchesCategoryId == 0 || config.StatsChannelId == 0 || config.MatchLogsChannelId == 0 || config.ScoreReporterRoleId == 0 || config.MaximumTeamSize <= 0 || config.WaitingForMatchChannelId == 0)
            {
                await ReplyAsync("Please make sure to set the matches category, stats channel, match logs channel and the score reporter role before setting up a queue. You may use `.config` to see your progress.");
                return;
            }
            if (name.Length <= 1 || name.Length > 14)
            {
                await ReplyAsync("Please specify the name of the queue to be between 2 and 14 characters long.");
                return;
            }
            if (capacity > 22 || capacity % 2 != 0)
            {
                await ReplyAsync("Please make sure the queue only holds up a maximum of 22 users, and that is an even number.");
                return;
            }
            if (sortType.ToLower() != "captains" && sortType.ToLower() != "elo")
            {
                await ReplyAsync("Please make sure the queue's sort type is only `captains` or `elo`.");
                return;
            }
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithDescription("Setting up queue...");
            RestUserMessage blankEmbedMessage = await base.Context.Channel.SendMessageAsync(null, isTTS: false, builder.Build());
            QueueConfig qConfig = new QueueConfig(name, capacity, base.Context.Guild.Id, base.Context.Channel.Id, blankEmbedMessage.Id);
            if (sortType.ToLower() == "captains")
            {
                qConfig.SortType = SortType.Captains;
            }
            else if (sortType.ToLower() == "elo")
            {
                qConfig.SortType = SortType.Elo;
            }
            ButtonBuilder lastButton = new ButtonBuilder() { Label = "Join  👋", IsDisabled = false, Style = ButtonStyle.Success, CustomId = "join" };
            ButtonBuilder nextButton = new ButtonBuilder() { Label = "leave ✋", IsDisabled = false, Style = ButtonStyle.Danger, CustomId = "exit" };
            ComponentBuilder componentBuilder = new ComponentBuilder()
                  .WithButton(lastButton)
                  .WithButton(nextButton);
            await blankEmbedMessage.ModifyAsync(delegate (MessageProperties x)
            {
                x.Embed = _embedService.GetPugQueueCreatedEmbed(qConfig);
            });
            await blankEmbedMessage.ModifyAsync(x => x.Components = componentBuilder.Build());

            config.QueueConfigs.Add(qConfig);
            await _databaseService.UpsertServerConfigAsync(config);
            _queueService.CreatePugQueue(qConfig);

            //await blankEmbedMessage.AddReactionAsync(new Emoji("✅"));
            //await blankEmbedMessage.AddReactionAsync(new Emoji("❌"));




            if (name.ToLower().Contains("crossfire"))
            {
                foreach (string map in qConfig.Maps)
                {
                    if (map.ToLower() == name.ToLower())
                    {
                        await ReplyAsync("That map: " + "Subbase" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "ankara" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "BlackWidow" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "Compound" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "Port" + " is already in the map pool for the " + qConfig.Name + " queue.");



                        return;
                    }
                }

                qConfig.Maps.Add("SubBase");
                qConfig.Maps.Add("ankara");
                qConfig.Maps.Add("BlackWidow");
                qConfig.Maps.Add("Compound");
                qConfig.Maps.Add("Port");

                await _databaseService.UpsertServerConfigAsync(config);
                _queueService.UpdatePugQueue(qConfig);
                await base.Context.Message.DeleteAsync();

            }
            else if (name.ToLower().Contains("valorant"))
            {
                foreach (string map in qConfig.Maps)
                {
                    if (map.ToLower() == name.ToLower())
                    {
                        await ReplyAsync("That map: " + "FRACTURE" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "ASCENT" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "SPLIT" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "HAVEN" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "BIND" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "ICEBOX" + " is already in the map pool for the " + qConfig.Name + " queue.");
                        await ReplyAsync("That map: " + "BREEZE" + " is already in the map pool for the " + qConfig.Name + " queue.");

                        return;
                    }
                }

                qConfig.Maps.Add("FRACTURE");
                qConfig.Maps.Add("ASCENT");
                qConfig.Maps.Add("SPLIT");
                qConfig.Maps.Add("HAVEN");
                qConfig.Maps.Add("BIND");
                qConfig.Maps.Add("ICEBOX");
                qConfig.Maps.Add("BREEZE");
                await _databaseService.UpsertServerConfigAsync(config);
                _queueService.UpdatePugQueue(qConfig);
                await base.Context.Message.DeleteAsync();
            }

            await base.Context.Message.DeleteAsync();


        }
        //[Command("createqueue")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        //[Summary("Set up a reaction based queue in a given channel.")]
        //public async Task CreateReactionQueue(string name, int capacity, string sortType)
        //{
        //    ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
        //    if (config.MatchesCategoryId == 0 || config.StatsChannelId == 0 || config.MatchLogsChannelId == 0 || config.ScoreReporterRoleId == 0 || config.MaximumTeamSize <= 0 || config.WaitingForMatchChannelId == 0)
        //    {
        //        await ReplyAsync("Please make sure to set the matches category, stats channel, match logs channel and the score reporter role before setting up a queue. You may use `.config` to see your progress.");
        //        return;
        //    }
        //    if (name.Length <= 1 || name.Length > 14)
        //    {
        //        await ReplyAsync("Please specify the name of the queue to be between 2 and 14 characters long.");
        //        return;
        //    }
        //    if (capacity > 22 || capacity % 2 != 0)
        //    {
        //        await ReplyAsync("Please make sure the queue only holds up a maximum of 22 users, and that is an even number.");
        //        return;
        //    }
        //    if (sortType.ToLower() != "captains" && sortType.ToLower() != "elo")
        //    {
        //        await ReplyAsync("Please make sure the queue's sort type is only `captains` or `elo`.");
        //        return;
        //    }
        //    EmbedBuilder builder = new EmbedBuilder();
        //    builder.WithDescription("Setting up queue...");
        //    RestUserMessage blankEmbedMessage = await base.Context.Channel.SendMessageAsync(null, isTTS: false, builder.Build());
        //    QueueConfig qConfig = new QueueConfig(name, capacity, base.Context.Guild.Id, base.Context.Channel.Id, blankEmbedMessage.Id);
        //    if (sortType.ToLower() == "captains")
        //    {
        //        qConfig.SortType = SortType.Captains;
        //    }
        //    else if (sortType.ToLower() == "elo")
        //    {
        //        qConfig.SortType = SortType.Elo;
        //    }
        //    await blankEmbedMessage.ModifyAsync(delegate (MessageProperties x)
        //    {
        //        x.Embed = _embedService.GetPugQueueCreatedEmbed(qConfig);
        //    });
        //    config.QueueConfigs.Add(qConfig);
        //    await _databaseService.UpsertServerConfigAsync(config);
        //    _queueService.CreatePugQueue(qConfig);
        //    await blankEmbedMessage.AddReactionAsync(new Emoji("✅"));
        //    await blankEmbedMessage.AddReactionAsync(new Emoji("❌"));

        //    if (name.ToLower().Contains("crossfire"))
        //    {
        //        foreach (string map in qConfig.Maps)
        //        {
        //            if (map.ToLower() == name.ToLower())
        //            {
        //                await ReplyAsync("That map: " + "Subbase" + " is already in the map pool for the " + qConfig.Name + " queue.");
        //                await ReplyAsync("That map: " + "ankara" + " is already in the map pool for the " + qConfig.Name + " queue.");
        //                await ReplyAsync("That map: " + "BlackWidow" + " is already in the map pool for the " + qConfig.Name + " queue.");
        //                await ReplyAsync("That map: " + "Compound" + " is already in the map pool for the " + qConfig.Name + " queue.");
        //                await ReplyAsync("That map: " + "Port" + " is already in the map pool for the " + qConfig.Name + " queue.");



        //                return;
        //            }
        //        }

        //        qConfig.Maps.Add("SubBase");
        //        qConfig.Maps.Add("ankara");
        //        qConfig.Maps.Add("BlackWidow");
        //        qConfig.Maps.Add("Compound");
        //        qConfig.Maps.Add("Port");

        //        await _databaseService.UpsertServerConfigAsync(config);
        //        _queueService.UpdatePugQueue(qConfig);
        //        await base.Context.Message.DeleteAsync();

        //    }

        //    await base.Context.Message.DeleteAsync();


        //}

        [Command("setmatchlogs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set match logs channel.")]
        public async Task SetMatchLogsChannel([Remainder] SocketGuildChannel channelMentioned)
        {
            if (base.Context.Message.MentionedChannels.Count <= 0)
            {
                await ReplyAsync("Please use the hashtag format (#channelName) after this command to set the match logs channel.");
                return;
            }
            if (base.Context.Message.MentionedChannels.Count > 1)
            {
                await ReplyAsync("Please only mention one channel.");
                return;
            }
            SocketGuildChannel matchLogsChannel = base.Context.Message.MentionedChannels.FirstOrDefault();
            if (matchLogsChannel == null)
            {
                await ReplyAsync("Something weird happened. Contact Kirlos Osama. Match Logs channel mention is null.");
                return;
            }
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.MatchLogsChannelId = matchLogsChannel.Id;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The match logs channel has been set to #" + matchLogsChannel.Name);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting match logs channel: \n\n" + e.Message);
                _logger.LogError("Error setting match logs channel in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }
        [Command("setwaitingchannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set match logs channel.")]
        public async Task SetWaitingChannel([Remainder] SocketGuildChannel channelMentioned)
        {
            if (base.Context.Message.MentionedChannels.Count <= 0)
            {
                await ReplyAsync("Please use the hashtag format (#channelName) after this command to set the match logs channel.");
                return;
            }
            if (base.Context.Message.MentionedChannels.Count > 1)
            {
                await ReplyAsync("Please only mention one channel.");
                return;
            }
            SocketGuildChannel WaitingForMatchChannelId = base.Context.Message.MentionedChannels.FirstOrDefault();
            if (WaitingForMatchChannelId == null)
            {
                await ReplyAsync("Something weird happened. Contact Kirlos Osama. Match Logs channel mention is null.");
                return;
            }
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.WaitingForMatchChannelId = WaitingForMatchChannelId.Id;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The Wating Match channel has been set to #" + WaitingForMatchChannelId.Name);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting match logs channel: \n\n" + e.Message);
                _logger.LogError("Error setting match logs channel in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }



        [Command("setmaxteamsize")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set the maximum team size for your server's scrims.")]
        public async Task SetMaxTeamSize(int maxTeamSize)
        {
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.MaximumTeamSize = maxTeamSize;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync($"The maximum team size has been set to `{config.MaximumTeamSize}`");
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting maximum team size: \n\n" + e.Message);
                _logger.LogError("Error setting maximum team size in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }

        [Command("createscrimboard")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set the scrim board for your guild.")]
        public async Task SetScrimBoard()
        {
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.ScrimBoardChannelId = base.Context.Channel.Id;
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithDescription("Setting up scrim board...");
                RestUserMessage blankEmbedMessage = await base.Context.Channel.SendMessageAsync(null, isTTS: false, builder.Build());
                Embed embed = await _embedService.ScrimBoardEmbed(_scrimService.GetScrimInvitesForGuild(base.Context.Guild.Id));
                await blankEmbedMessage.ModifyAsync(delegate (MessageProperties x)
                {
                    x.Embed = embed;
                });
                config.ScrimBoardMessageId = blankEmbedMessage.Id;
                await _databaseService.UpsertServerConfigAsync(config);
                await base.Context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting the scrim board: \n\n" + e.Message);
                _logger.LogError("Error setting the scrim board in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }

        [Command("setmatchescategory")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set match logs channel.")]
        public async Task SetMatchLogsChannel(ulong matchesCategoryId)
        {
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.MatchesCategoryId = matchesCategoryId;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The matches category channel has been set to " + base.Context.Guild.GetCategoryChannel(matchesCategoryId).Name);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting matches category Id: \n\n" + e.Message);
                _logger.LogError("Error setting matches category Id in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }

        [Command("setstatschannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set match logs channel.")]
        public async Task SetStatsChannel([Remainder] SocketGuildChannel channelMentioned)
        {
            if (base.Context.Message.MentionedChannels.Count <= 0)
            {
                await ReplyAsync("Please use the hashtag format (#channelName) after this command to set the statistics channel.");
                return;
            }
            if (base.Context.Message.MentionedChannels.Count > 1)
            {
                await ReplyAsync("Please only mention one channel.");
                return;
            }
            SocketGuildChannel statsChannel = base.Context.Message.MentionedChannels.FirstOrDefault();
            if (statsChannel == null)
            {
                await ReplyAsync("Something weird happened. Contact Kirlos O. Fawzi 👑#0001. Stats channel mention is null.");
                return;
            }
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
                config.StatsChannelId = statsChannel.Id;
                await _databaseService.UpsertServerConfigAsync(config);
                await ReplyAsync("The stats channel has been set to #" + statsChannel.Name);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting stats channel: \n\n" + e.Message);
                _logger.LogError("Error setting stats channel in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }

        [Command("setwinloss")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Set the pug win and loss amount; positive numbers only.")]
        public async Task SetWinLossAmounts(int winAmount, int lossAmount)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (winAmount <= 0 || lossAmount <= 0)
            {
                await ReplyAsync("Make sure to include only positive numbers for the win amount and then also loss amount.");
                return;
            }
            config.WinAmount = winAmount;
            config.LossAmount = lossAmount;
            await _databaseService.UpsertServerConfigAsync(config);
            await ReplyAsync($"Win amount has been set to: `{config.WinAmount}`. Loss amount has been set to: `{config.LossAmount}`.");
        }

        [Command("trunemoji")]
        [RequireUserPermission(GuildPermission.Administrator)]

        public async Task trunemoji(int winAmount)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            config.checkranking = winAmount;
            string kero = null;
            await _databaseService.UpsertServerConfigAsync(config);
            if (config.checkranking == 1)
            {
                kero = "ON";

            }
            else
            {

                kero = "OFF";


            }

            await ReplyAsync($"Progressbar has been set to Turn:{kero}.");
        }
        [Command("setprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]

        public async Task setprefix(string prefix = null)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (prefix == null)
            {
                await ReplyAsync($"{config.prefix}setprefix ex:[!,?,#,1,+,9,m] just one letter.");



            }
            if (prefix.Length == 1)
            {


                config.prefix = prefix;
                await _databaseService.UpsertServerConfigAsync(config);
                var successEmbed = new EmbedBuilder();

                successEmbed.WithTitle("Prefix set!");
                successEmbed.WithColor(Color.Red);

               successEmbed.WithDescription($"The bot prefix has been set to  `{config.prefix}` :white_check_mark:");
           //     successEmbed.WithFooter($"Sorry, I could not find that command.");
                successEmbed.WithCurrentTimestamp();

            //    await context.Channel.SendMessageAsync(null, isTTS: false, EmbedHelper.Unregistered());

                await ReplyAsync(null, isTTS: false, successEmbed.Build());
                return;



            }
            else
            {

                await ReplyAsync($"{config.prefix}setprefix [!,?,#,1,+,9,m] just one letter.");

                return;


            }

        }

        [Command("config")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("See your server's configuration.")]
        public async Task SeeServerConfig()
        {
            string kero = null ;
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (config == null)
            {
                await ReplyAsync("Config could not be pulled up. This may be due to bot down time while inviting it to your server. Please kick this bot and then re-invite it.");
                return;
            }
            if (config.checkranking == 1)
            {
                kero = "ON";

            }
            else
            {

                kero = "OFF";


            }
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Green);
            builder.WithTitle(base.Context.Guild.Name + " Configuration");
            builder.AddField("Match Logs Channel Id", config.MatchLogsChannelId.ToString(), inline: true);
            builder.AddField("Stats Channel Id", config.StatsChannelId.ToString(), inline: true);
            builder.AddField("Waiting Channel Id", config.WaitingForMatchChannelId.ToString(), inline: true);
            builder.AddField("Start Full Emoji ID", config.Startfull.ToString(), inline: true);
            builder.AddField("Center Full Emoji ID", config.Centerfull.ToString(), inline: true);
            builder.AddField("End Full Emoji ID", config.Endfull.ToString(), inline: true);
            builder.AddField("Start Null Emoji ID", config.Startnull.ToString(), inline: true);
            builder.AddField("Center Null Emoji ID", config.Centernull.ToString(), inline: true);
            builder.AddField("End Null Emoji ID", config.Endnull.ToString(), inline: true);
            builder.AddField("NoRank Emoji ID", config.norank.ToString(), inline: true);
            builder.AddField("Bronze  Emoji ID", config.Bronze.ToString(), inline: true);
            builder.AddField("Gold Emoji ID", config.Gold.ToString(), inline: true);
            builder.AddField("Platinum Emoji ID", config.Platinum.ToString(), inline: true);
            builder.AddField("Diamond Emoji ID", config.Diamond.ToString(), inline: true);
            builder.AddField("Master Emoji ID", config.Master.ToString(), inline: true);
            builder.AddField("legend Emoji ID", config.legend.ToString(), inline: true);
            builder.AddField("mythical Emoji ID", config.mythical.ToString(), inline: true);
            builder.AddField("GrandMaster Emoji ID", config.GrandMaster.ToString(), inline: true);
            builder.AddField("Matches Category Id", config.MatchesCategoryId.ToString());
            builder.AddField("Score Reporter Role Id", config.ScoreReporterRoleId.ToString());
           builder.AddField("Progressbar Status", $"{kero}");
           builder.AddField("Prefix", $"{config.prefix.ToString()}");
          //  builder.AddField("Prefix Static", $"{Config.Prfix.ToString()}");
            builder.AddField("Win Amount", config.WinAmount.ToString(), inline: true);
            builder.AddField("Loss Amount", config.LossAmount.ToString(), inline: true);
            builder.AddField("Maximum Team Size", config.MaximumTeamSize.ToString());
            if (config.QueueConfigs.Count <= 0)
            {
                builder.WithDescription("**No PUG queues have been configured for this server.**");
            }
            else
            {
                string description = "**Queue Configurations:**\n\n";
                foreach (QueueConfig qConfig in config.QueueConfigs)
                {
                    description += $"+ **Name**: {qConfig.Name} | **Team Size**: {qConfig.Capacity / 2}v{qConfig.Capacity / 2} | **Team Sort Type**: {qConfig.SortType} | **Channel Id**: {qConfig.ChannelId} | **MessageId**: {qConfig.MessageId}...\n\n";
                }
                builder.WithDescription(description);
            }
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = "Developed by Kirlos O. Fawzi 👑#0001"
            });
            await ReplyAsync(null, isTTS: false, builder.Build());
        }
        ///For Test 
        [Command("kick")]
        [Summary("Kick a user from the server.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser targetUser, [Remainder] string reason = "No reason provided.")
        {
            await targetUser.KickAsync(reason);
            await ReplyAsync($"**{targetUser}** has been kicked. Bye bye :wave:");
        }

        [Command("ban")]
        [Summary("Ban a user from the server")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser targetUser, [Remainder] string reason = "No reason provided.")
        {
            await Context.Guild.AddBanAsync(targetUser.Id, 0, reason);
            await ReplyAsync($"**{targetUser}** has been banned. Bye bye :wave:");
        }

        [Command("unban")]
        [Summary("Unban a user from the server")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Unban(ulong targetUser)
        {
            await Context.Guild.RemoveBanAsync(targetUser);
            await Context.Channel.SendMessageAsync($"The user has been unbanned :clap:");
        }
        [Command("role")]
        [Alias("roleinfo")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Show information about a role.")]
        public async Task RoleInfo([Remainder] SocketRole role)
        {
            // Just in case someone tries to be funny.
            if (role.Id == Context.Guild.EveryoneRole.Id)
                return;
            await ReplyAsync(
                $":flower_playing_cards: **{role.Name}** information```ini" +
                $"\n[Members]             {role.Members.Count()}" +
                $"\n[Role ID]             {role.Id}" +
                $"\n[Hoisted status]      {role.IsHoisted}" +
                $"\n[Created at]          {role.CreatedAt:dd/M/yyyy}" +
                $"\n[Hierarchy position]  {role.Position}" +
                $"\n[Color Hex]           {role.Color}```");
        }


        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task PurgeChat(int amount)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            const int delay = 3000;
            IUserMessage m = await ReplyAsync(base.Context.User.Mention + $"I have deleted messages :)");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        [Command("setemoji")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("progressbar")]
        public async Task setEmoji(ulong startfull = 0, ulong Centerfull = 0, ulong Endfull = 0, ulong Startnull = 0, ulong Centernull = 0, ulong Endnull = 0, ulong norank = 0, ulong Bronze = 0, ulong Silver = 0, ulong Gold = 0, ulong Platinum = 0, ulong Diamond = 0, ulong Master = 0, ulong legend = 0, ulong mythical = 0, ulong GrandMaster = 0)
        {
            if (startfull == 0 || Centerfull == 0 || Endfull == 0 || Startnull == 0 || Centernull == 0 || Endnull == 0 || norank == 0 || Bronze == 0 || Silver == 0 || Gold == 0 || Platinum == 0 || Diamond == 0 || Master == 0 || legend == 0 || mythical == 0 || GrandMaster == 0)
#pragma warning restore CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
            {
                ServerConfig config2 = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
   
                await ReplyAsync($"{  config2.prefix.ToString()}setemoji [startfull_id] [Centerfull_id] [Endfull_id]  [Startnull_id] [Centernull_id] [Endnull_id] [norank_id] [Bronze_id] [Silver_id] [Endnull_id] [Gold_id] [Platinum_id] [Diamond_id] [Master_id] [legend_id] [mythical_id] [GrandMaster_id]");
                return;

            }
            try
            {
                ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);

                config.Startfull = startfull;
                config.Centerfull = Centerfull;
                config.Endfull = Endfull;
                config.Startnull = Startnull;
                config.Centernull = Centernull;
                config.Endnull = Endnull;
                config.norank = norank;
                config.Bronze = Bronze;
                config.Silver = Silver;
                config.Gold = Gold;
                config.Platinum = Platinum;
                config.Diamond = Diamond;
                config.Master = Master;
                config.legend = legend;
                config.mythical = mythical;
                config.GrandMaster = GrandMaster;
                await _databaseService.UpsertServerConfigAsync(config);
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithColor(Color.Green);
                builder.WithTitle("Emoji Configuration");
                builder.AddField("Start Full Emoji ID", config.Startfull.ToString(), inline: true);
                builder.AddField("Center Full Emoji ID", config.Centerfull.ToString(), inline: true);
                builder.AddField("End Full Emoji ID", config.Endfull.ToString(), inline: true);
                builder.AddField("Start Null Emoji ID", config.Startnull.ToString(), inline: true);
                builder.AddField("Center Null Emoji ID", config.Centernull.ToString(), inline: true);
                builder.AddField("End Null Emoji ID", config.Endnull.ToString(), inline: true);
                builder.AddField("NoRank Emoji ID", config.norank.ToString(), inline: true);
                builder.AddField("Bronze  Emoji ID", config.Bronze.ToString(), inline: true);
                builder.AddField("Gold Emoji ID", config.Gold.ToString(), inline: true);
                builder.AddField("Platinum Emoji ID", config.Platinum.ToString(), inline: true);
                builder.AddField("Diamond Emoji ID", config.Diamond.ToString(), inline: true);
                builder.AddField("Master Emoji ID", config.Master.ToString(), inline: true);
                builder.AddField("legend Emoji ID", config.legend.ToString(), inline: true);
                builder.AddField("mythical Emoji ID", config.mythical.ToString(), inline: true);
                builder.AddField("GrandMaster Emoji ID", config.GrandMaster.ToString(), inline: true);

                builder.WithFooter(new EmbedFooterBuilder
                {
                    Text = "Developed by Kirlos O. Fawzi 👑#0001"
                });
                await ReplyAsync(null, isTTS: false, builder.Build());
            }
            catch (Exception e)
            {
                await ReplyAsync("Error setting match logs channel: \n\n" + e.Message);
                _logger.LogError("Error setting match logs channel in " + base.Context.Guild.Name + ": " + e.Message);
            }
        }
        

    }
}
