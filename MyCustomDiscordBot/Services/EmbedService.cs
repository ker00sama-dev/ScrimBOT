using Discord;
using Discord.WebSocket;
using MyCustomDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class EmbedService
    {
        private readonly DiscordSocketClient _client;

        private readonly DatabaseService _databaseService;

        private Random random = new Random();

        public EmbedService(DiscordSocketClient client, DatabaseService databaseService)
        {
            _client = client;
            _databaseService = databaseService;
        }

        public async Task<Embed> ScrimBoardEmbed(List<ScrimInvite> invites)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Green);
            string description = "**USER POSTING | TEAM POINTS | SCRIM DESCRIPTION**\n";
            foreach (ScrimInvite invite in invites)
            {
                description += $"\n{invite.InvitingTeamCaptain.Mention} | {invite.InvitingTeam.Points} | `{invite.Description}`";
            }
            builder.WithDescription(description);
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = "\nUse .post scrimDescription --> to make a post.\nUse .removepost --> to remove your team's post.\nUse .accept @PostingUser to accept their challenge."
            });
            return builder.Build();
        }

        public async Task<Embed> LeaderboardEmbed(List<DbUser> topPlayers)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Gold);
            string description = "**RANK | ELO | PLAYER**\n";
            for (int i = 0; i < topPlayers.Count; i++)
            {
                SocketUser user = _client.GetUser(topPlayers[i].DiscordId);
                if (user != null)
                {
                    description += $"`#{i + 1} | {topPlayers[i].ELO}` {user.Mention}\n";
                }
            }
            builder.WithDescription(description);
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = "Leaderboard"
            });
            return builder.Build();
        }

        public async Task<Embed> TeamEmbed(Team team)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Blue);
            builder.WithTitle(team.Name + "'s Stats");
            int totalWins = 0;
            int totalLosses = 0;
            foreach (MapRecord record in team.MapRecords)
            {
                builder.AddField(record.MapName, $"{record.Wins} | {record.Losses}", inline: true);
                totalWins += record.Wins;
                totalLosses += record.Losses;
            }
            int gamesPlayed = totalLosses + totalWins;
            string memberMentions = "\n";
            foreach (ulong discordId in team.MemberDiscordIds)
            {
                if (discordId != team.CaptainDiscordId)
                {
                    SocketUser user = _client.GetUser(discordId);
                    if (user != null)
                    {
                        memberMentions = memberMentions + user.Mention + "\n";
                    }
                }
            }
            memberMentions += "\n";
            builder.WithDescription($"**Captain:** {_client.GetUser(team.CaptainDiscordId).Mention}\n**Members**: {memberMentions} **Points**: `{team.Points}`\n**Games Played**: `{gamesPlayed}` | Wins: `{totalWins}` Losses `{totalLosses}`\n\n");
            return builder.Build();
        }

        public async Task<Embed> ProfileEmbed(DbUser user)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Blue);
            builder.WithTitle(user.Username + "'s Stats");
            int totalWins = 0;
            int totalLosses = 0;
            foreach (MapRecord record in user.MapRecords)
            {
                builder.AddField(record.MapName, $"{record.Wins} | {record.Losses}", inline: true);
                totalWins += record.Wins;
                totalLosses += record.Losses;
            }
            int gamesPlayed = totalLosses + totalWins;
            builder.WithDescription($"**ELO**: `{user.ELO}`\n**Games Played**: `{gamesPlayed}` | Wins: `{totalWins}` Losses `{totalLosses}`\n\n");
            builder.WithThumbnailUrl(_client.GetUser(user.DiscordId).GetAvatarUrl(ImageFormat.Auto, 128));
            return builder.Build();
        }

        public async Task<Embed> MatchLogEmbed(Match match, ulong guildId)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Red);
            builder.WithTitle($"Match Log: #{match.Number}");
            if (match.Winners > 0)
            {
                builder.WithFooter(new EmbedFooterBuilder
                {
                    Text = $"Team {match.Winners} victory!"
                });
            }
            else if (match.Winners == -2)
            {
                builder.WithFooter(new EmbedFooterBuilder
                {
                    Text = "Decision reversed! No elo granted."
                });
            }
            else if (match.Winners == 0)
            {
                builder.WithFooter(new EmbedFooterBuilder
                {
                    Text = "Match cancelled!"
                });
            }
            builder.WithDescription("Map: *" + match.Map + "*");
            string team1Value = "";
            foreach (ulong discordId2 in match.Team1DiscordIds)
            {
                team1Value += $"`{(await _databaseService.GetUserInGuild(discordId2, guildId)).ELO}`{_client.GetUser(discordId2).Mention}\n";
            }
            string team2Value = "";
            foreach (ulong discordId2 in match.Team2DiscordIds)
            {
                team2Value += $"`{(await _databaseService.GetUserInGuild(discordId2, guildId)).ELO}`{_client.GetUser(discordId2).Mention}\n";
            }
            builder.AddField("Team 1", team1Value, inline: true);
            builder.AddField("Team 2", team2Value, inline: true);
            return builder.Build();
        }

        public Embed GetPugQueueCreatedEmbed(QueueConfig config)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{config.Name} Queue: 0 / {config.Capacity}");
            builder.WithColor(Color.Gold);
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = $"Teams arranged by {config.SortType}"
            });
            return builder.Build();
        }

        public Embed GetQueueEmbed(PugQueue queue)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Gold);
            builder.WithTitle($"{queue.Name} Queue: {queue.Users.Count} / {queue.Capacity}");
            string description = "";
            foreach (DbUser user in queue.Users)
            {
                description += $"`{user.ELO}`{_client.GetUser(user.DiscordId).Mention}\n";
            }
            builder.WithDescription(description);
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = $"Teams arranged by {queue.SortType}"
            });
            return builder.Build();
        }

        public string RandomString(int length)
        {
            return new string((from s in Enumerable.Repeat("123364654654564974548548484", length)
                               select s[random.Next(s.Length)]).ToArray());
        }

        public async Task<Embed> GetMatchEmbedAsync(Match match, ulong guildId, Team team1 = null, Team team2 = null)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithColor(Color.Green);
            string password = string.Empty;

            if (guildId == ServerIDs())
            {

                password = RandomString(4).ToLower();


            }
            if (match.SortType == SortType.Elo)
            {
                builder.WithTitle($"PUG Match #{match.Number}");
                if (password != string.Empty)
                {
                    builder.WithDescription("Map: *" + match.Map + "*\n\nPassword: *" + password + "*");
                }
                else
                {
                    builder.WithDescription("Map: *" + match.Map + "*");
                }
                string team1Value3 = "";
                foreach (ulong discordId5 in match.Team1DiscordIds)
                {
                    team1Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                }
                string team2Value3 = "";
                foreach (ulong discordId5 in match.Team2DiscordIds)
                {
                    team2Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                }
              
                builder.AddField("Team 1", team1Value3, inline: true);
                builder.AddField("Team 2", team2Value3, inline: true);

           

                string Mapup = match.Map.ToUpper();
                //qConfig.Maps.Add("SubBase");
                //qConfig.Maps.Add("ankara");
                //qConfig.Maps.Add("BlackWidow");
                //qConfig.Maps.Add("Compound");
                //qConfig.Maps.Add("Port");
                if (Mapup.Contains("SUBBASE"))

                {

                    builder.WithImageUrl("https://cdn.discordapp.com/attachments/691520066575138866/906938436341104651/1000.png");



                }
                if (Mapup.Contains("COMPOUND"))
                {




                    builder.WithImageUrl("https://cdn.discordapp.com/attachments/691520066575138866/906938077711331338/latest.jpg");



                }
                if (Mapup.Contains("PORT"))
                {




                    builder.WithImageUrl("https://media.discordapp.net/attachments/691520066575138866/906939312652832828/480.png");



                }
                if (Mapup.Contains("BLACKWIDOW"))
                {




                    builder.WithImageUrl("https://media.discordapp.net/attachments/691520066575138866/906939006724481054/1000.png?width=901&height=676");



                }                if (Mapup.Contains("ANKARA"))
                {




                    builder.WithImageUrl("https://media.discordapp.net/attachments/691520066575138866/906938745842970644/1000.png");



                }

            }
            else if (match.SortType == SortType.Captains)
            {
                builder.WithTitle($"PUG Match #{match.Number}");
                if (match.PickingPoolDiscordIds.Count > 0)
                {
                    List<string> poolMentions = new List<string>();
                    foreach (ulong discordId8 in match.PickingPoolDiscordIds)
                    {
                        poolMentions.Add(_client.GetUser(discordId8).Mention);
                    }
                    string description = "Map: *" + match.Map + "*\n\n" + _client.GetUser(match.ActiveCaptainDiscordId).Mention + " is picking... please pick from a user in this pool: " + string.Join(" ", poolMentions);
                    builder.WithDescription(description + "\n\n*Pick using `.pick @user`*");
                    string team2Value3 = "";
                    foreach (ulong discordId5 in match.Team1DiscordIds)
                    {
                        team2Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                    }
                    string team1Value3 = "";
                    foreach (ulong discordId5 in match.Team2DiscordIds)
                    {
                        team1Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                    }
                    builder.AddField("Team 1", team2Value3, inline: true);
                    builder.AddField("Team 2", team1Value3, inline: true);
                }
                else
                {
                    if (password != string.Empty)
                    {
                        builder.WithDescription("Map: *" + match.Map + "*\n\nPassword: *" + password + "*");
                    }
                    else
                    {
                        builder.WithDescription("Map: *" + match.Map + "*");
                    }
                    string team1Value3 = "";
                    foreach (ulong discordId5 in match.Team1DiscordIds)
                    {
                        team1Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                    }
                    string team2Value3 = "";
                    foreach (ulong discordId5 in match.Team2DiscordIds)
                    {
                        team2Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                    }
                    builder.AddField("Team 1", team1Value3, inline: true);
                    builder.AddField("Team 2", team2Value3, inline: true);
                }
            }
            else if (match.SortType == SortType.Scrim)
            {
                builder.WithTitle($"Scrim Match #{match.Number}");
                builder.WithDescription("*Please choose and agree upon any map and game mode.*");
                string team2Value3 = "";
                foreach (ulong discordId5 in match.Team1DiscordIds)
                {
                    team2Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                }
                string team1Value3 = "";
                foreach (ulong discordId5 in match.Team2DiscordIds)
                {
                    team1Value3 += $"`{(await _databaseService.GetUserInGuild(discordId5, guildId)).ELO}`{_client.GetUser(discordId5).Mention}\n";
                }
                builder.AddField($"1: {team1.Name} [{team1.Points}]", team2Value3, inline: true);
                builder.AddField($"2: {team2.Name} [{team2.Points}]", team1Value3, inline: true);
            }
            builder.WithFooter(new EmbedFooterBuilder
            {
                Text = "Powered by " + _client.GetGuild(guildId).Name
            });
            return builder.Build();
        }

        public static ulong ServerIDs() => 903657723495866408L;
    }
}
