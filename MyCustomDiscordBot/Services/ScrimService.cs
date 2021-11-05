using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MyCustomDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class ScrimService
    {
        private readonly DiscordSocketClient _client;

        private readonly UtilityService _utilityService;

        private readonly DatabaseService _databaseService;

        private readonly EmbedService _embedService;

        private readonly ILogger<Worker> _logger;

        public List<ScrimInvite> Invites;

        public ScrimService(DiscordSocketClient client, UtilityService utilityService, DatabaseService databaseService, EmbedService embedService, ILogger<Worker> logger)
        {
            _client = client;
            _utilityService = utilityService;
            _databaseService = databaseService;
            _embedService = embedService;
            _logger = logger;
            Invites = new List<ScrimInvite>();
        }

        public Team GetTeamForScrimInvite(ulong guildId, ulong postingUserId)
        {
            foreach (ScrimInvite invite in Invites)
            {
                if (invite.InvitingTeamCaptain.Id == postingUserId)
                {
                    return invite.InvitingTeam;
                }
            }
            return null;
        }

        public async Task<RestTextChannel> CreateMatchFromScrim(ulong guildId, Team invitingTeam, Team acceptedTeam)
        {
            await RemovePost(guildId, invitingTeam);
            Match match = new Match
            {
                SortType = SortType.Scrim
            };
            IMongoCollection<Match> collection = _databaseService.GetMatchesForGuild(guildId);
            Match match2 = match;
            match2.Number = await collection.CountDocumentsAsync(new BsonDocument()) + 1;
            List<ulong> allPlayerDiscordIds = new List<ulong>();
            foreach (ulong discordId2 in invitingTeam.MemberDiscordIds)
            {
                allPlayerDiscordIds.Add(discordId2);
            }
            foreach (ulong discordId5 in acceptedTeam.MemberDiscordIds)
            {
                allPlayerDiscordIds.Add(discordId5);
            }
            match.AllPlayerDiscordIds = allPlayerDiscordIds;
            match.Captain1DiscordId = invitingTeam.CaptainDiscordId;
            match.Captain2DiscordId = acceptedTeam.CaptainDiscordId;
            match.Team1DiscordIds = invitingTeam.MemberDiscordIds;
            match.Team2DiscordIds = acceptedTeam.MemberDiscordIds;
            match.Map = "Any";
            SocketGuild guild = _client.GetGuild(guildId);
            ServerConfig serverConfig = await _databaseService.GetServerConfigAsync(guildId);
            RestTextChannel matchInfoChannel = await guild.CreateTextChannelAsync($"▶\ufe0f-match-#{match.Number}");
            await matchInfoChannel.ModifyAsync(delegate (TextChannelProperties x)
            {
                x.CategoryId = serverConfig.MatchesCategoryId;
            });
            List<string> mentionedUsers = new List<string>();
            foreach (ulong discordId6 in match.AllPlayerDiscordIds)
            {
                mentionedUsers.Add(_client.GetUser(discordId6).Mention);
            }
            await matchInfoChannel.SendMessageAsync("Attention, your match is ready! " + string.Join(" ", mentionedUsers));
            RestVoiceChannel team1Voice = await guild.CreateVoiceChannelAsync(invitingTeam.Name ?? "");
            await Task.Delay(250);
            RestVoiceChannel team2Voice = await guild.CreateVoiceChannelAsync(acceptedTeam.Name ?? "");
            OverwritePermissions connectTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
            OverwritePermissions sendMessagesFalse = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);
            OverwritePermissions sendMessagesTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
            foreach (ulong discordId4 in match.AllPlayerDiscordIds)
            {
                await matchInfoChannel.AddPermissionOverwriteAsync(guild.GetUser(discordId4), sendMessagesTrue);
                await Task.Delay(250);
            }
            await matchInfoChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, sendMessagesFalse);
            await team1Voice.ModifyAsync(delegate (VoiceChannelProperties x)
            {
                x.CategoryId = serverConfig.MatchesCategoryId;
                x.UserLimit = serverConfig.MaximumTeamSize + 1;
            });
            await Task.Delay(250);
            await team2Voice.ModifyAsync(delegate (VoiceChannelProperties x)
            {
                x.CategoryId = serverConfig.MatchesCategoryId;
                x.UserLimit = serverConfig.MaximumTeamSize + 1;
            });
            OverwritePermissions connectFalse = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);
            await team1Voice.AddPermissionOverwriteAsync(_client.GetGuild(guild.Id).EveryoneRole, connectFalse);
            await Task.Delay(250);
            await team2Voice.AddPermissionOverwriteAsync(_client.GetGuild(guild.Id).EveryoneRole, connectFalse);
            await Task.Delay(250);
            foreach (ulong discordId3 in match.Team1DiscordIds)
            {
                try
                {
                    await team1Voice.AddPermissionOverwriteAsync(guild.GetUser(discordId3), connectTrue);
                    SocketGuildUser moveMe2 = guild.GetUser(discordId3);
                    if (moveMe2 != null && moveMe2.VoiceChannel != null)
                    {
                        await moveMe2.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            //       Discord.Optional<IVoiceChannel> team1Voice1 = (Discord.Optional<IVoiceChannel>)(IVoiceChannel)team1Voice;
                            //      x.Channel = team1Voice1;
                        });
                        await Task.Delay(250);
                    }
                }
                catch (Exception e2)
                {
                    _logger.LogError(e2.Message);
                }
            }
            foreach (ulong discordId3 in match.Team2DiscordIds)
            {
                try
                {
                    await team2Voice.AddPermissionOverwriteAsync(guild.GetUser(discordId3), connectTrue);
                    SocketGuildUser moveMe = guild.GetUser(discordId3);
                    if (moveMe != null && moveMe.VoiceChannel != null)
                    {
                        await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            //	x.Channel = (Discord.Optional<IVoiceChannel>)(IVoiceChannel)team2Voice;
                        });
                        await Task.Delay(250);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
            match.Team1VoiceChannelId = team1Voice.Id;
            match.Team2VoiceChannelId = team2Voice.Id;
            match.MatchInfoChannelId = matchInfoChannel.Id;
            await _databaseService.UpsertMatchAsync(guildId, match);
            IMessage message = await _utilityService.GetMessageFromTextChannel(guildId, serverConfig.ScrimBoardChannelId, serverConfig.ScrimBoardMessageId);
            if (message != null)
            {
                IUserMessage embedMessage = message as IUserMessage;
                if (embedMessage != null)
                {
                    Embed embed = await _embedService.ScrimBoardEmbed(GetScrimInvitesForGuild(guildId));
                    await embedMessage.ModifyAsync(delegate (MessageProperties x)
                    {
                        x.Embed = embed;
                    });
                }
            }
            RestTextChannel restTextChannel = matchInfoChannel;
            await restTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, guild.Id, invitingTeam, acceptedTeam));
            return matchInfoChannel;
        }

        public bool PostExistsForTeam(ulong guildId, Team invitingTeam)
        {
            foreach (ScrimInvite invite in Invites)
            {
                if (invite.GuildId == guildId && invite.InvitingTeam.Id == invitingTeam.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public List<ScrimInvite> GetScrimInvitesForGuild(ulong guildId)
        {
            List<ScrimInvite> returnVal = new List<ScrimInvite>();
            foreach (ScrimInvite invite in Invites)
            {
                if (invite.GuildId == guildId)
                {
                    returnVal.Add(invite);
                }
            }
            return returnVal;
        }

        public int GetNumberOfScrimInvitesForGuild(ulong guildId)
        {
            return Invites.Select((ScrimInvite x) => x.GuildId == guildId).ToList().Count;
        }

        public async Task RemovePost(ulong guildId, Team team)
        {
            int removeIndex = -1;
            for (int i = 0; i < Invites.Count; i++)
            {
                if (Invites[i].InvitingTeam.Id == team.Id)
                {
                    removeIndex = i;
                    break;
                }
            }
            if (removeIndex < 0)
            {
                return;
            }
            Invites.RemoveAt(removeIndex);
            ServerConfig config = await _databaseService.GetServerConfigAsync(guildId);
            IMessage message = await _utilityService.GetMessageFromTextChannel(guildId, config.ScrimBoardChannelId, config.ScrimBoardMessageId);
            if (message == null)
            {
                return;
            }
            IUserMessage embedMessage = message as IUserMessage;
            if (embedMessage != null)
            {
                Embed embed = await _embedService.ScrimBoardEmbed(GetScrimInvitesForGuild(guildId));
                await embedMessage.ModifyAsync(delegate (MessageProperties x)
                {
                    x.Embed = embed;
                });
            }
        }

        public async Task PostForScrim(ulong guildId, string description, SocketUser invitingTeamCaptain, Team invitingTeam)
        {
            ScrimInvite invite = new ScrimInvite(guildId, description, invitingTeamCaptain, invitingTeam);
            Invites.Add(invite);
            ServerConfig config = await _databaseService.GetServerConfigAsync(guildId);
            IMessage message = await _utilityService.GetMessageFromTextChannel(guildId, config.ScrimBoardChannelId, config.ScrimBoardMessageId);
            if (message == null)
            {
                return;
            }
            IUserMessage embedMessage = message as IUserMessage;
            if (embedMessage != null)
            {
                Embed embed = await _embedService.ScrimBoardEmbed(GetScrimInvitesForGuild(guildId));
                await embedMessage.ModifyAsync(delegate (MessageProperties x)
                {
                    x.Embed = embed;
                });
            }
        }
    }
}
