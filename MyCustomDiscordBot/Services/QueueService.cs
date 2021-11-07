using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class QueueService
    {
        private readonly MatchService _matchService;

        private readonly DiscordSocketClient _client;

        private readonly DatabaseService _databaseService;

        private readonly EmbedService _embedService;

        private readonly UtilityService _utilityService;

        private List<PugQueue> Queues { get; }

        public QueueService(MatchService matchService, DiscordSocketClient client, DatabaseService databaseService, EmbedService embedService, UtilityService utilityService)
        {
            _matchService = matchService;
            _client = client;
            _databaseService = databaseService;
            _embedService = embedService;
            _utilityService = utilityService;
            Queues = new List<PugQueue>();
        }

        public void CreatePugQueue(QueueConfig config)
        {
            foreach (PugQueue queue in Queues)
            {
                if (queue.MessageId == config.MessageId)
                {
                    return;
                }
            }
            Queues.Add(new PugQueue(config));
        }

        public void UpdatePugQueue(QueueConfig config)
        {
            int changeIndex = -1;
            for (int i = 0; i < Queues.Count; i++)
            {
                if (Queues[i].MessageId == config.MessageId)
                {
                    changeIndex = i;
                    break;
                }
            }
            if (changeIndex >= 0)
            {
                Queues[changeIndex] = new PugQueue(config);
            }
        }

        public async Task<bool> HandleUserWentAFK(SocketUser user)
        {
            bool returnVal = false;
            foreach (PugQueue queue in Queues)
            {
                int removeIndex = -1;
                for (int i = 0; i < queue.Users.Count; i++)
                {
                    if (queue.Users[i].DiscordId == user.Id)
                    {
                        removeIndex = i;
                        break;
                    }
                }
                if (removeIndex < 0)
                {
                    continue;
                }
                queue.Users.RemoveAt(removeIndex);
                IMessage queueMessage = await _utilityService.GetMessageFromTextChannel(queue.GuildId, queue.ChannelId, queue.MessageId);
                if (queueMessage != null)
                {
                    IUserMessage message = queueMessage as IUserMessage;
                    if (message != null)
                    {
                        await message.ModifyAsync(delegate (MessageProperties x)
                        {
                            x.Embed = _embedService.GetQueueEmbed(queue);
                        });
                    }
                }
                returnVal = true;
            }
            return returnVal;
        }

        public PugQueue GetPugQueue(ulong messageId)
        {
            foreach (PugQueue queue in Queues)
            {
                if (queue.MessageId == messageId)
                {
                    return queue;
                }
            }
            return null;
        }

        public bool UserExistsInQueue(ulong messageId, ulong userDiscordId)
        {
            PugQueue pugQueue = GetPugQueue(messageId);
            if (pugQueue == null)
            {
                return false;
            }
            foreach (DbUser userCurr in pugQueue.Users)
            {
                if (userCurr.DiscordId == userDiscordId)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RemoveFromPugQueue(ulong messageId, ulong discordId)
        {
            PugQueue queue = GetPugQueue(messageId);
            if (queue == null)
            {
                return false;
            }
            int removeIndex = -1;
            for (int i = 0; i < queue.Users.Count; i++)
            {
                if (queue.Users[i].DiscordId == discordId)
                {
                    removeIndex = i;
                    break;
                }
            }
            if (removeIndex >= 0)
            {
                queue.Users.RemoveAt(removeIndex);
                IMessage message = await _utilityService.GetMessageFromTextChannel(queue.GuildId, queue.ChannelId, messageId);
                Embed queueEmbed = _embedService.GetQueueEmbed(queue);
                await (message as IUserMessage).ModifyAsync(delegate (MessageProperties x)
                {
                    x.Embed = queueEmbed;
                });
            }
            return true;
        }

        public async Task<bool> AddToPugQueue(ulong messageId, DbUser user, ulong guildId)
        {
            foreach (PugQueue queueCurr in Queues)
            {
                if (queueCurr.GuildId == guildId)
                {
                    DbUser userFound = queueCurr.Users.Find((DbUser x) => x.DiscordId == user.DiscordId);
                    if (userFound != null)
                    {
                        await (await _client.GetGuild(guildId).GetTextChannel(queueCurr.ChannelId).SendMessageAsync(_client.GetUser(user.DiscordId).Mention + " you are already in a queue for this server.")).DeleteMessageAfterSeconds(5);
                        return true;
                    }
                }
            }
            PugQueue queue = GetPugQueue(messageId);
            if (queue == null)
            {
                return false;
            }
            _ = user.SuspensionReturnDate;
            if (user.SuspensionReturnDate > DateTime.UtcNow)
            {
                await (await _client.GetGuild(queue.GuildId).GetTextChannel(queue.ChannelId).SendMessageAsync($"{_client.GetUser(user.DiscordId).Mention}, you are suspended from pugs for {Math.Round((user.SuspensionReturnDate - DateTime.UtcNow).TotalMinutes, 2)} minutes.")).DeleteMessageAfterSeconds(5);
                return true;
            }
            if (queue.Maps.Count <= 0)
            {
                SocketGuild guild = _client.GetGuild(queue.GuildId);
                SocketTextChannel queueChannel = guild.GetTextChannel(queue.ChannelId);
                await (await queueChannel.SendMessageAsync("There are no maps in the " + queue.Name + " map pool. Please add at least one before continuing.")).DeleteMessageAfterSeconds(5);
                return true;
            }
            queue.Users.Add(user);
            if (queue.Users.Count >= queue.Capacity)
            {
                List<DbUser> copyList = new List<DbUser>();
                foreach (DbUser u in queue.Users)
                {
                    copyList.Add(u);
                }
                queue.Users.Clear();
                Match match = await _matchService.GenerateMatchAsync(copyList, queue.SortType, queue.GuildId, queue.MessageId);
                match.PugQueueMessageId = queue.MessageId;
                SocketGuild guild2 = _client.GetGuild(queue.GuildId);
                RestVoiceChannel team1Voice = await guild2.CreateVoiceChannelAsync($"Match #{match.Number} Team 1");
                RestVoiceChannel team2Voice = await guild2.CreateVoiceChannelAsync($"Match #{match.Number} Team 2");
                ServerConfig serverConfig = await _databaseService.GetServerConfigAsync(guild2.Id);
                RestTextChannel matchInfoChannel = await guild2.CreateTextChannelAsync($"\ud83c\udfc6-match-#{match.Number}");
                await matchInfoChannel.ModifyAsync(delegate (TextChannelProperties x)
                {
                    x.CategoryId = serverConfig.MatchesCategoryId;
                });
                await team1Voice.ModifyAsync(delegate (VoiceChannelProperties x)
                {
                    x.CategoryId = serverConfig.MatchesCategoryId;
                    x.UserLimit = queue.Capacity / 2 + 1;
                });
                await team2Voice.ModifyAsync(delegate (VoiceChannelProperties x)
                {
                    x.CategoryId = serverConfig.MatchesCategoryId;
                    x.UserLimit = queue.Capacity / 2 + 1;
                });
                OverwritePermissions connectFalse = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);
                if (guildId != EmbedService.ServerIDs())
                {
                    await team1Voice.AddPermissionOverwriteAsync(_client.GetGuild(guild2.Id).EveryoneRole, connectFalse);
                    await team2Voice.AddPermissionOverwriteAsync(_client.GetGuild(guild2.Id).EveryoneRole, connectFalse);
                }
                List<string> mentionedUsers = new List<string>();

                foreach (ulong discordId4 in match.AllPlayerDiscordIds)
                {
                    mentionedUsers.Add(_client.GetUser(discordId4).Mention);
                }
                await matchInfoChannel.SyncPermissionsAsync();
                SocketTextChannel queueChannel2 = guild2.GetTextChannel(queue.ChannelId);
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithColor(Color.Green);
                builder.WithDescription($"Match #{match.Number} has been generated. Please visit {matchInfoChannel.Mention} to begin your match.");
                await (await queueChannel2.SendMessageAsync(null, isTTS: false, builder.Build())).DeleteMessageAfterSeconds(3);

                await matchInfoChannel.SendMessageAsync("Attention, your match is ready! " + string.Join(" ", mentionedUsers));

                RestTextChannel restTextChannel = matchInfoChannel;
                await restTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, guild2.Id));
                OverwritePermissions sendMessagesFalse = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);
                OverwritePermissions sendMessagesTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                foreach (ulong discordId3 in match.AllPlayerDiscordIds)
                {
                    await matchInfoChannel.AddPermissionOverwriteAsync(guild2.GetUser(discordId3), sendMessagesTrue);
                }
                await matchInfoChannel.AddPermissionOverwriteAsync(guild2.EveryoneRole, sendMessagesFalse);


                OverwritePermissions connectTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                if (queue.SortType == SortType.Elo)
                {
                    foreach (ulong discordId2 in match.Team1DiscordIds)
                    {
                        if (guildId != EmbedService.ServerIDs())
                        {
                            await team1Voice.AddPermissionOverwriteAsync(guild2.GetUser(discordId2), connectTrue);
                        }
                        SocketGuildUser moveMe2 = guild2.GetUser(discordId2);
                        if (moveMe2 != null && moveMe2.VoiceChannel != null)
                        {
                            await moveMe2.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                //x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)team1Voice;
                            });
                        }
                    }
                    foreach (ulong discordId2 in match.Team2DiscordIds)
                    {
                        if (guildId != EmbedService.ServerIDs())
                        {
                            await team2Voice.AddPermissionOverwriteAsync(guild2.GetUser(discordId2), connectTrue);
                        }
                        SocketGuildUser moveMe = guild2.GetUser(discordId2);
                        if (moveMe != null && moveMe.VoiceChannel != null)
                        {
                            await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                //	x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)team2Voice;
                            });
                        }
                    }
                }
                else if (queue.SortType == SortType.Captains)
                {
                    await team1Voice.AddPermissionOverwriteAsync(guild2.GetUser(match.Captain1DiscordId), connectTrue);
                    await team2Voice.AddPermissionOverwriteAsync(guild2.GetUser(match.Captain2DiscordId), connectTrue);
                }
                match.Team1VoiceChannelId = team1Voice.Id;
                match.Team2VoiceChannelId = team2Voice.Id;
                match.MatchInfoChannelId = matchInfoChannel.Id;
                await _databaseService.UpsertMatchAsync(guildId, match);
            }
            Embed queueEmbed = _embedService.GetQueueEmbed(queue);
            await ((await _utilityService.GetMessageFromTextChannel(queue.GuildId, queue.ChannelId, queue.MessageId)) as IUserMessage).ModifyAsync(delegate (MessageProperties x)
            {
                x.Embed = queueEmbed;
            });
            return true;
        }
    }
}
