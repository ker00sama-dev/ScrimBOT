using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class ScoreReporter : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _databaseService;

        private readonly MatchService _matchService;

        private readonly EmbedService _embedService;

        private readonly QueueService _queueService;

        public ScoreReporter(DatabaseService databaseService, MatchService matchService, EmbedService embedService, QueueService queueService)
        {
            _databaseService = databaseService;
            _matchService = matchService;
            _embedService = embedService;
            _queueService = queueService;
        }
        [Command("replace")]
        [Summary("Request a sub from the queue.")]
        public async Task replace(SocketGuildUser userToSub, SocketGuildUser userToSubto)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, base.Context.Channel.Id);
            if (match == null)
            {
                await ReplyAsync("Either this is not a match channel or something went wrong.");
                return;
            }
            PugQueue queue = _queueService.GetPugQueue(match.PugQueueMessageId);
            if (queue == null)
            {
                await ReplyAsync(base.Context.User.Mention + ", something weird happened and the queue was not found.");
            }
            else if (match.Team1DiscordIds.Contains(userToSub.Id))
            {
                ulong playerInQueueDiscordId2 = userToSubto.Id;
                match.Team1DiscordIds.Add(playerInQueueDiscordId2);
                match.Team1DiscordIds.Remove(userToSub.Id);
                await _queueService.RemoveFromPugQueue(queue.MessageId, playerInQueueDiscordId2);
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                SocketGuildUser userSubbedIn2 = base.Context.Guild.GetUser(playerInQueueDiscordId2);
                await ReplyAsync(userSubbedIn2.Mention + " has been subbed in for " + userToSub.Mention + ".");
                await Task.Delay(250);
                await ReplyAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
                SocketVoiceChannel team1 = base.Context.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
                if (team1 != null)
                {
                    OverwritePermissions connectTrue2 = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                    await team1.AddPermissionOverwriteAsync(userSubbedIn2, connectTrue2);
                }
                try
                {
                    if (userSubbedIn2.VoiceChannel != null)
                    {
                        await userSubbedIn2.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            x.Channel = team1;
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            else if (match.Team2DiscordIds.Contains(userToSub.Id))
            {
                ulong playerInQueueDiscordId2 = userToSubto.Id;
                match.Team2DiscordIds.Add(playerInQueueDiscordId2);
                match.Team2DiscordIds.Remove(userToSub.Id);
                await _queueService.RemoveFromPugQueue(queue.MessageId, playerInQueueDiscordId2);
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                SocketGuildUser userSubbedIn2 = base.Context.Guild.GetUser(playerInQueueDiscordId2);
                await ReplyAsync(userSubbedIn2.Mention + " has been subbed in for " + userToSub.Mention + ".");
                await Task.Delay(250);
                await ReplyAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
                SocketVoiceChannel team2 = base.Context.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
                if (team2 != null)
                {
                    OverwritePermissions connectTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                    await team2.AddPermissionOverwriteAsync(userSubbedIn2, connectTrue);
                }
                try
                {
                    if (userSubbedIn2.VoiceChannel != null)
                    {
                        await userSubbedIn2.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            x.Channel = team2;
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                await ReplyAsync(base.Context.User.Mention + ", " + userToSub.Mention + " is not a player in this match.");
            }
        }
        [Command("needsub")]
        [Summary("Request a sub from the queue.")]
        public async Task NeedSub(SocketGuildUser userToSub)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, base.Context.Channel.Id);
            if (match == null)
            {
                await ReplyAsync("Either this is not a match channel or something went wrong.");
                return;
            }
            PugQueue queue = _queueService.GetPugQueue(match.PugQueueMessageId);
            if (queue == null)
            {
                await ReplyAsync(base.Context.User.Mention + ", something weird happened and the queue was not found.");
            }
            else if (queue.Users.Count <= 0)
            {
                await ReplyAsync(base.Context.User.Mention + " there are no users in the " + queue.Name + " queue to sub from.");
            }
            else if (match.Team1DiscordIds.Contains(userToSub.Id))
            {
                ulong playerInQueueDiscordId2 = queue.Users[0].DiscordId;
                match.Team1DiscordIds.Add(playerInQueueDiscordId2);
                match.Team1DiscordIds.Remove(userToSub.Id);
                await _queueService.RemoveFromPugQueue(queue.MessageId, playerInQueueDiscordId2);
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                SocketGuildUser userSubbedIn2 = base.Context.Guild.GetUser(playerInQueueDiscordId2);
                await ReplyAsync(userSubbedIn2.Mention + " has been subbed in for " + userToSub.Mention + ".");
                await Task.Delay(250);
                await ReplyAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
                SocketVoiceChannel team1 = base.Context.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
                if (team1 != null)
                {
                    OverwritePermissions connectTrue2 = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                    await team1.AddPermissionOverwriteAsync(userSubbedIn2, connectTrue2);
                }
                try
                {
                    if (userSubbedIn2.VoiceChannel != null)
                    {
                        await userSubbedIn2.ModifyAsync(delegate (GuildUserProperties x)
                        {
                          x.Channel = team1;
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            else if (match.Team2DiscordIds.Contains(userToSub.Id))
            {
                ulong playerInQueueDiscordId2 = queue.Users[0].DiscordId;
                match.Team2DiscordIds.Add(playerInQueueDiscordId2);
                match.Team2DiscordIds.Remove(userToSub.Id);
                await _queueService.RemoveFromPugQueue(queue.MessageId, playerInQueueDiscordId2);
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                SocketGuildUser userSubbedIn2 = base.Context.Guild.GetUser(playerInQueueDiscordId2);
                await ReplyAsync(userSubbedIn2.Mention + " has been subbed in for " + userToSub.Mention + ".");
                await Task.Delay(250);
                await ReplyAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
                SocketVoiceChannel team2 = base.Context.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
                if (team2 != null)
                {
                    OverwritePermissions connectTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
                    await team2.AddPermissionOverwriteAsync(userSubbedIn2, connectTrue);
                }
                try
                {
                    if (userSubbedIn2.VoiceChannel != null)
                    {
                        await userSubbedIn2.ModifyAsync(delegate (GuildUserProperties x)
                        {
                            	x.Channel = team2;
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                await ReplyAsync(base.Context.User.Mention + ", " + userToSub.Mention + " is not a player in this match.");
            }
        }

        [Command("cooldown")]
        [Summary("Suspend a user for X minutes from PUGs.")]
        public async Task SuspendUser(SocketGuildUser user, double minutes)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
            if (dbUser == null)
            {
                await ReplyAsync(user.Mention + " has not played any matches yet and does not have an account in our database.");
                return;
            }
            dbUser.SuspensionReturnDate = DateTime.UtcNow.AddMinutes(minutes);
            await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
            await ReplyAsync($"{user.Mention} has been suspended from pick up games for {minutes} minutes.");
        }

        [Command("removecd")]
        [Summary("Suspend a user for X minutes from PUGs.")]
        public async Task UnSuspeend(SocketGuildUser user, double minutes = -99999)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
            if (dbUser == null)
            {
                await ReplyAsync(user.Mention + " has not played any matches yet and does not have an account in our database.");
                return;
            }
            dbUser.SuspensionReturnDate = DateTime.UtcNow.AddMinutes(minutes);
            await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
            await ReplyAsync($"{user.Mention} has been Unsuspended");
        }

        [Command("takeelo")]
        [Summary("Take X elo from a user's account.")]
        public async Task TakeElo(SocketGuildUser user, int amount)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
            if (dbUser == null)
            {
                await ReplyAsync("That user has no account in the database. Please make sure they play at least one match before taking elo.");
                return;
            }
            if (amount <= 0)
            {
                await ReplyAsync("Please make sure the amount specified is greater than 0.");
                return;
            }
            dbUser.ELO -= amount;
            await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
            await ReplyAsync($"{user.Mention} has had {amount} ELO taken from their account.");
        }

        [Command("giveelo")]
        [Summary("Give X elo to a user's account.")]
        public async Task GiveElo(SocketGuildUser user, int amount)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you need the score reporting role to do this.");
                    return;
                }
            }
            DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
            if (dbUser == null)
            {
                await ReplyAsync("That user has no account in the database. Please make sure they play at least one match before giving elo.");
                return;
            }
            if (amount <= 0)
            {
                await ReplyAsync("Please make sure the amount specified is greater than 0.");
                return;
            }
            dbUser.ELO += amount;
            await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
            await ReplyAsync($"{user.Mention} has been granted {amount} additional ELO to their account.");
        }

        [Command("cancel")]
        [Summary("Cancel a match")]
        public async Task CancelMatch()
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you do not have the score reporting role.");
                    return;
                }
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, base.Context.Channel.Id);
            if (match.Winners == 0 || match.Winners == 1 || match.Winners == 2)
            {
                await ReplyAsync("This match has already been reported. You may use the `.giveelo` and `.takeelo` commands to make any corrections.");
                return;
            }
            match.Winners = 0;
            await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
            SocketVoiceChannel waiting = base.Context.Guild.GetVoiceChannel(config.WaitingForMatchChannelId);
            foreach (ulong discordId in match.AllPlayerDiscordIds)
            {
                SocketGuildUser moveMe = base.Context.Guild.GetUser(discordId);
                if (moveMe != null && moveMe.VoiceChannel != null)
                {
                    await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                    {
                        x.Channel = waiting;
                    });
                    await Task.Delay(250);
                }
            }
            SocketVoiceChannel team1Voice = base.Context.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
            if (team1Voice != null)
            {
                await team1Voice.DeleteAsync();
            }
            await Task.Delay(250);
            SocketVoiceChannel team2Voice = base.Context.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
            if (team2Voice != null)
            {
                await team2Voice.DeleteAsync();
            }
            await Task.Delay(250);
            SocketTextChannel matchInfoChannel = base.Context.Channel as SocketTextChannel;
            if (matchInfoChannel != null)
            {
                await matchInfoChannel.DeleteAsync();
            }
            SocketTextChannel matchLogsChannel = base.Context.Guild.GetTextChannel(config.MatchLogsChannelId);
            SocketTextChannel socketTextChannel = matchLogsChannel;
            await socketTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.MatchLogEmbed(match, base.Context.Guild.Id, base.Context.User));
        }

        [Command("report")]
        [Summary("Report a victory by the team number.")]
        public async Task ReportVictory(int teamNumber)
        { /// same it ?tsa??
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            SocketGuildUser author = base.Context.User as SocketGuildUser;
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
                    await ReplyAsync(author.Mention + ", you do not have the score reporting role.");
                    return;
                }
            }
            if (teamNumber != 1 && teamNumber != 2)
            {
                await ReplyAsync("Please specify either `1` or `2` only, with respect to the winning team.");
                return;
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, base.Context.Channel.Id);
            if (match.Winners == 1 || match.Winners == 2)
            {
                      

        await ReplyAsync($"This match has already been reported as a team ${match.Winners} victory. You may use `{config.prefix.ToString()}giveelo @User AMOUNT` and ` {config.prefix.ToString()}takeelo @User AMOUNT` to correct any mistakes.");
                return;
            }
            if (match == null)
            {
                await ReplyAsync("Either this is not a match channel or something went wrong.");
                return;
            }
            await _matchService.CalculateWinForMatch(match, teamNumber, base.Context.Guild.Id);
            SocketVoiceChannel waiting = base.Context.Guild.GetVoiceChannel(config.WaitingForMatchChannelId);
            foreach (ulong discordId in match.AllPlayerDiscordIds)
            {
                SocketGuildUser moveMe = base.Context.Guild.GetUser(discordId);
                if (moveMe != null && moveMe.VoiceChannel != null)
                {
                    await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                    {
                        x.Channel = waiting;
                    });
                    await Task.Delay(250);
                }
            }
            SocketVoiceChannel team1Voice = base.Context.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
            if (team1Voice != null)
            {
                await team1Voice.DeleteAsync();
            }
            await Task.Delay(250);
            SocketVoiceChannel team2Voice = base.Context.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
            if (team2Voice != null)
            {
                await team2Voice.DeleteAsync();
            }
            SocketTextChannel matchInfoChannel = base.Context.Channel as SocketTextChannel;
            if (matchInfoChannel != null)
            {
                await matchInfoChannel.DeleteAsync();
            }
            SocketTextChannel matchLogsChannel = base.Context.Guild.GetTextChannel(config.MatchLogsChannelId);
            SocketTextChannel socketTextChannel = matchLogsChannel;
            await socketTextChannel.SendMessageAsync(null, isTTS: false, await _embedService.MatchLogEmbed(match, base.Context.Guild.Id, base.Context.User));
        }
    }
}
