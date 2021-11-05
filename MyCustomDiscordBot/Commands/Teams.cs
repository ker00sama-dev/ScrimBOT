using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Commands
{
    public class Teams : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _databaseService;

        private readonly ILogger<Worker> _logger;

        private readonly EmbedService _embedService;

        private readonly ScrimService _scrimService;

        public Teams(DatabaseService databaseService, ILogger<Worker> logger, EmbedService embedService, ScrimService scrimService)
        {
            _databaseService = databaseService;
            _logger = logger;
            _embedService = embedService;
            _scrimService = scrimService;
        }

        [Command("post")]
        [Alias(new string[] { "scrim" })]
        [Summary("Post a scrim to the scrim board.")]
        public async Task PostScrim([Remainder] string description)
        {
            if (description.Length < 3 || description.Length > 40)
            {
                await (await ReplyAsync("Please provide a scrim descrpition that is between 3 and 40 characters of length.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null)
                {
                    await (await ReplyAsync("You are not on a team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (team.MemberDiscordIds.Count < config.MaximumTeamSize)
            {
                await (await ReplyAsync($"{base.Context.User.Mention}, your team does not have the required amount of members yet. ({config.MaximumTeamSize})")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
            }
            else if (_scrimService.PostExistsForTeam(base.Context.Guild.Id, team))
            {
                await (await ReplyAsync(base.Context.User.Mention + ", your team already has an existing post on this board. You may use the `removepost` command if needed.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
            }
            else if (_scrimService.GetNumberOfScrimInvitesForGuild(base.Context.Guild.Id) >= 15)
            {
                await (await ReplyAsync(base.Context.User.Mention + ", there may only be 15 scrim invites at one time on this board. Please use one of the available challenges instead of posting another one.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
            }
            else
            {
                await _scrimService.PostForScrim(base.Context.Guild.Id, description, base.Context.User, team);
                await base.Context.Message.DeleteAsync();
            }
        }

        [Command("accept")]
        [Summary("Accept a challenge on the scrim board.")]
        public async Task AcceptScrimPost(SocketGuildUser enemy = null)
        {
            if (enemy == null)
            {
                await (await ReplyAsync(base.Context.User.Mention + ", please specify a user on the board to accept their scrim challenge.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", you are not on a team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            Team enemyTeam = _scrimService.GetTeamForScrimInvite(base.Context.Guild.Id, enemy.Id);
            if (enemyTeam == null)
            {
                await (await ReplyAsync(base.Context.User.Mention + ", that user does not have an active post on the scrim board.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            RestTextChannel matchChannel = await _scrimService.CreateMatchFromScrim(base.Context.Guild.Id, enemyTeam, team);
            string description3 = "";
            foreach (ulong discordId2 in enemyTeam.MemberDiscordIds)
            {
                description3 = description3 + base.Context.Guild.GetUser(discordId2).Mention + " ";
            }
            description3 += "`versus` ";
            foreach (ulong discordId in team.MemberDiscordIds)
            {
                description3 = description3 + base.Context.Guild.GetUser(discordId).Mention + " ";
            }
            description3 = description3 + "`is starting... please go to your match channel now.` " + matchChannel.Mention;
            await base.Context.Message.DeleteAsync();
            await (await ReplyAsync(description3)).DeleteMessageAfterSeconds(20);
        }

        [Command("removepost")]
        [Summary("Remove your post from the scrim board.")]
        public async Task RemoveScrimPost()
        {
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", you are not on a team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            await _scrimService.RemovePost(base.Context.Guild.Id, team);
            await base.Context.Message.DeleteAsync();
        }

        [Command("createteam")]
        [Summary("Create a team to scrim with.")]
        public async Task CreateTeam([Remainder] string name)
        {
            if (name.Length < 3 || name.Length > 20)
            {
                await (await ReplyAsync(base.Context.User.Mention + ", please specify a name for your team that is between 3 and 20 characters long.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            Team team2 = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
            if (team2 != null)
            {
                await (await ReplyAsync("You are already in the team: " + team2.Name)).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            team2 = new Team(name)
            {
                CaptainDiscordId = base.Context.User.Id,
                MemberDiscordIds = { base.Context.User.Id }
            };
            await _databaseService.UpsertTeamAsync(base.Context.Guild.Id, team2);
            DbUser user = await _databaseService.GetUserInGuild(base.Context.User.Id, base.Context.Guild.Id);
            if (user == null)
            {
                user = new DbUser(base.Context.User.Username, base.Context.User.Id);
            }
            user.TeamId = team2.Id;
            try
            {
                await _databaseService.UpsertUser(base.Context.Guild.Id, user);
            }
            catch (Exception e)
            {
                await ReplyAsync("Error creating user: \n\n" + e.Message);
                _logger.LogError("Error creating user in " + base.Context.Guild.Name + ": " + e.Message);
            }
            await ReplyAsync(base.Context.User.Mention + " has created the team: " + team2.Name);
        }

        [Command("addplayers")]
        [Summary("Add the mentioned players to your team.")]
        public async Task AddPlayer(SocketGuildUser u1 = null, SocketGuildUser u2 = null, SocketGuildUser u3 = null, SocketGuildUser u4 = null, SocketGuildUser u5 = null, SocketGuildUser u6 = null)
        {
            if (base.Context.Message.MentionedUsers.Count <= 0)
            {
                await (await ReplyAsync(base.Context.User.Mention + ", please @Mention at least one user to join your team.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null || team.CaptainDiscordId != base.Context.User.Id)
                {
                    await (await ReplyAsync("Either you are not on a team, or you are not the captain of your team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (base.Context.Message.MentionedUsers.Count + team.MemberDiscordIds.Count > config.MaximumTeamSize)
            {
                await (await ReplyAsync($"{base.Context.User.Mention}, adding this many players to your team will exceed the maximum team limit (maximum is {config.MaximumTeamSize}). You may only add {config.MaximumTeamSize - team.MemberDiscordIds.Count} more players.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            foreach (SocketUser user in base.Context.Message.MentionedUsers)
            {
                DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
                if (dbUser == null)
                {
                    dbUser = new DbUser(user.Username, user.Id);
                }
                if (team.CaptainDiscordId == user.Id)
                {
                    await ReplyAsync(base.Context.User.Mention + " you cannot remove yourself from a team like this (you are the captain). Use the `deleteteam` command.");
                    continue;
                }
                Team teamFound = await _databaseService.GetTeamInGuild(user.Id, base.Context.Guild.Id);
                if (teamFound != null)
                {
                    await ReplyAsync(user.Mention + " is already on team: " + teamFound.Name + " and was not added to your team, " + base.Context.User.Mention + ".");
                }
                else if (team.MemberDiscordIds.Contains(dbUser.DiscordId))
                {
                    await ReplyAsync(user.Mention + " is already on your team.");
                }
                else
                {
                    dbUser.TeamId = team.Id;
                    team.MemberDiscordIds.Add(dbUser.DiscordId);
                    await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
                }
            }
            await _databaseService.UpsertTeamAsync(base.Context.Guild.Id, team);
            string mentions = "";
            foreach (ulong discordId in team.MemberDiscordIds)
            {
                mentions = mentions + base.Context.Guild.GetUser(discordId).Mention + " ";
            }
            await ReplyAsync("Team: " + team.Name + " now includes: " + mentions);
        }

        [Command("team")]
        [Summary("See your team's profile, or a specfic @Mentioned user's team info.")]
        public async Task SeeTeamProfile(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = base.Context.Guild.GetUser(base.Context.User.Id);
            }
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(user.Id, base.Context.Guild.Id);
                if (team == null)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", a team does not exist for the user " + user.Mention)).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            await ReplyAsync(null, isTTS: false, await _embedService.TeamEmbed(team));
        }

        [Command("removeplayers")]
        [Summary("Remove the mentioned users from your team.")]
        public async Task RemovePlayers(SocketGuildUser u1 = null, SocketGuildUser u2 = null, SocketGuildUser u3 = null, SocketGuildUser u4 = null, SocketGuildUser u5 = null, SocketGuildUser u6 = null)
        {
            if (base.Context.Message.MentionedUsers.Count <= 0)
            {
                await (await ReplyAsync("Please @Mention at least one user to remove from your team.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
                return;
            }
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null || team.CaptainDiscordId != base.Context.User.Id)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", either you are not on a team, or you are not the captain of your team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            foreach (SocketUser user in base.Context.Message.MentionedUsers)
            {
                DbUser dbUser = await _databaseService.GetUserInGuild(user.Id, base.Context.Guild.Id);
                if (dbUser == null)
                {
                    dbUser = new DbUser(user.Username, user.Id);
                }
                if (team.CaptainDiscordId == user.Id)
                {
                    await ReplyAsync(base.Context.User.Mention + " you cannot remove yourself from a team like this (you are the captain). Use the `deleteteam` command.");
                }
                else if (team.MemberDiscordIds.Contains(dbUser.DiscordId))
                {
                    dbUser.TeamId = default(ObjectId);
                    team.MemberDiscordIds.Remove(dbUser.DiscordId);
                    await _databaseService.UpsertUser(base.Context.Guild.Id, dbUser);
                }
                else
                {
                    await ReplyAsync(user.Mention + " is not on your team.");
                }
            }
            await _databaseService.UpsertTeamAsync(base.Context.Guild.Id, team);
            string mentions = "";
            foreach (ulong discordId in team.MemberDiscordIds)
            {
                mentions = mentions + base.Context.Guild.GetUser(discordId).Mention + " ";
            }
            await ReplyAsync("Your team now includes: " + mentions);
        }

        [Command("setcaptain")]
        [Summary("Transfer captain status to the @Mentioned player.")]
        public async Task TransferCaptain(SocketGuildUser newCaptain)
        {
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null || team.CaptainDiscordId != base.Context.User.Id)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", either you are not on a team, or you are not the captain of your team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e.Message);
            }
            if (!team.MemberDiscordIds.Contains(newCaptain.Id))
            {
                await (await ReplyAsync(newCaptain.Mention + " is not on your team.")).DeleteMessageAfterSeconds(10);
                await base.Context.Message.DeleteAsync();
            }
            else
            {
                team.CaptainDiscordId = newCaptain.Id;
                await _databaseService.UpsertTeamAsync(base.Context.Guild.Id, team);
                await ReplyAsync(newCaptain.Mention + " is now the captain of the team " + team.Name);
            }
        }

        [Command("deleteteam")]
        [Summary("As a captain, delete your own team.")]
        public async Task DeleteTeam()
        {
            Team team = null;
            try
            {
                team = await _databaseService.GetTeamInGuild(base.Context.User.Id, base.Context.Guild.Id);
                if (team == null || team.CaptainDiscordId != base.Context.User.Id)
                {
                    await (await ReplyAsync(base.Context.User.Mention + ", either you are not on a team, or you are not the captain of your team.")).DeleteMessageAfterSeconds(10);
                    await base.Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception e2)
            {
                await ReplyAsync("Error fetching team for guild: \n\n" + e2.Message);
                _logger.LogError("Error fetching team in guild: " + base.Context.Guild.Name + ": " + e2.Message);
            }
            foreach (ulong discordId in team.MemberDiscordIds)
            {
                DbUser user = await _databaseService.GetUserInGuild(discordId, base.Context.Guild.Id);
                user.TeamId = default(ObjectId);
                await _databaseService.UpsertUser(base.Context.Guild.Id, user);
            }
            try
            {
                await _databaseService.DeleteTeam(base.Context.Guild.Id, team);
                await ReplyAsync(base.Context.User.Mention + ", you have deleted your team: " + team.Name + "!");
            }
            catch (Exception e2)
            {
                await ReplyAsync("Error deleting team for guild: \n\n" + e2.Message);
                _logger.LogError("Error deleting team in guild: " + base.Context.Guild.Name + ": " + e2.Message);
            }
        }
    }
}
