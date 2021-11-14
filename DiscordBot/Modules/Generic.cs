using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MyCustomDiscordBot;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCustomDiscordBot.MyCustomDiscordBot;
using static MyCustomDiscordBot.MyCustomDiscordBot.DiscordBOTGaming;

namespace DiscordBot.Modules
{
    public class Generic : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _databaseService;

        private readonly QueueService _queueService;

        private readonly EmbedService _embedService;

        private readonly CommandService _commandService;

        public Generic(DatabaseService databaseService, QueueService queueService, EmbedService embedService, CommandService commandService)
        {
            _databaseService = databaseService;
            _queueService = queueService;
            _embedService = embedService;
            _commandService = commandService;
        }
        [Command("info")]
        public async Task BotInfo()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"Bot Info")
    .WithColor(Color.Green)
    .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
    .WithDescription($"The bot was made by {MentionUtils.MentionUser(488567658686447638)}\n\n" +
                     $"Current Prefix **{Config.Prfix}**")
    .WithFooter($"{DateTime.Now}");

            await ReplyAsync("", false, builder.Build());

        }


        [Command("help")]
        [Alias(new string[] { "commands" })]
        [Summary("Get more information on how to use the bot.")]
        public async Task SeeHelp(string command = null)
        {
            string prifx = Config.Prfix.ToString();
            await ReplyAsync(@"```
Commands
" + prifx + @"createqueue        - {NAMEQUEUE} {USERS MAXIMUM(22)}  {ELO" + prifx + @"CAPTAINS}
" + prifx + @"giveelo            - {@USER} {AMOUNT}
" + prifx + @"config
" + prifx + @"clearqueue         - {NAMEQUEUE} / Clear the queue
" + prifx + @"addmap             - {ID MASSGE} {NAMEMAP}
" + prifx + @"setmatchlogs       - {#channelName}
" + prifx + @"setwaitingchannel  - {#channelName}
" + prifx + @"setupreaction      - Recreates the reaction message to join the queue
" + prifx + @"eloreset           - Resets all elo" + prifx + @" must be superuser" + prifx + @" So I can do it or I added AshK as a superuser" + prifx + @" If you want someone else added let me know who
" + prifx + @"leaderboard        - Leaderboard in the stats channel
aliases: lb
" + prifx + @"pick               - Pick a player in captain pick matches" + prifx + @" Ex: " + prifx + @"pick @Player
aliases: " + prifx + @"p
" + prifx + @"needsub            - Need sub command used to find the next player in the queue to fill a spot for a missing player" + prifx + @" Ex: " + prifx + @"needsub @Player 
aliases: " + prifx + @"needsubfor
" + prifx + @"report             - Report the score of the match" + prifx + @" 0 - Cancel" + prifx + @" 1 - Team 1" + prifx + @" 2 - Team 2" + prifx + @" Ex: " + prifx + @"report 1
" + prifx + @"profile            - View a player profile" + prifx + @" Ex: " + prifx + @"profile || " + prifx + @"profile @Player
" + prifx + @"warn               - Mod warn a player" + prifx + @" Ex: " + prifx + @"warn @Player reason
" + prifx + @"warnings           - View the warnings of a player" + prifx + @" Ex: " + prifx + @"warnings @Player
    aliases: " + prifx + @"warns
" + prifx + @"disablequeue       - Disable queue for a specific time" + prifx + @" Ex: " + prifx + @"disablequeue 5d 12h

```");
            // await ReplyAsync("test");
            //  await ReplyAsync(description);
        }

        [Command("serverinfo")]
        public async Task serverinfo()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Server: {Context.Guild.Name}")
                .WithColor(Color.Magenta)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Owner", $"{Context.Guild.Owner}", inline: true)
                .AddField("Date Created", $"{Context.Guild.CreatedAt}", inline: true)
                .AddField("Members", $"{Context.Guild.MemberCount}", inline: true)
                .AddField("Roles", $"{Context.Guild.Roles.Count}", inline: true)
                .AddField("Text Channels", $"{Context.Guild.TextChannels.Count}", inline: true)
                .AddField("Voice Channels", $"{Context.Guild.VoiceChannels.Count}", inline: true)
                .WithFooter($"{DateTime.Now}");

            await ReplyAsync("", false, builder.Build());
        }
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

        //[Command("purge")]
        //[Summary("Bulk deletes messages in chat")]
        //[RequireBotPermission(GuildPermission.ManageMessages)]
        //[RequireUserPermission(GuildPermission.ManageMessages)]
        //public async Task Purge(int delNumber)
        //{
        //    var channel = Context.Channel as SocketTextChannel;
        //    var items = await channel.GetMessagesAsync(delNumber + 1).FlattenAsync();
        //    await channel.DeleteMessagesAsync(items);
        //}

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

        [Command("ping")]
        [Summary("Check whether the bot is working or not.")]

        public async Task PingInfo()
        {
            int latency = Context.Client.Latency;
            long milliseconds = 0;

            EmbedBuilder embed = new()
            {
                Title = $"Current bot ping",
                Description = $"**Gateway Ping:** ``{latency}ms``\n" +
                              "**Message Response Time:** *Evaluating*\n" +
                              "**Delta:** *Evaluating*",
            };

            Stopwatch sw = Stopwatch.StartNew();

            var message = await ReplyAsync(embed: embed.Build());

            sw.Stop();
            milliseconds = sw.ElapsedMilliseconds;

            Color color = Color.LightGrey;
            String indication = "";
            switch (milliseconds - latency)
            {
                case > 1000:
                    color = Color.DarkRed;
                    indication = "Bad";
                    break;
                case > 750:
                    color = Color.Red;
                    indication = "Mediocre";
                    break;
                case > 250:
                    color = Color.Orange;
                    indication = "Good";
                    break;
                case < 250:
                    color = Color.Green;
                    indication = "Excellent";
                    break;
            }

            EmbedBuilder embed2 = new()
            {
                Title = $"Current bot ping: *{indication}*",
                Description = $"**Gateway Ping:** ``{Context.Client.Latency}ms``\n" +
                              $"**Message Response Time:** {(milliseconds == 0 ? "*Evaluating*" : $"`{milliseconds}ms`")}\n" +
                              $"**Delta:** {(milliseconds == 0 ? "*Evaluating*" : $"`{milliseconds - latency}ms`")}",
                Color = color
            };

            await message.ModifyAsync(m => m.Embed = embed2.Build());
        }

        //[Command("avatar")]
        //[Summary("See your profile, or pull up a user's profile!")]
        //public async Task KeroPHOTO(SocketGuildUser user = null)
        //{

        //  //  SocketGuildUser member = user as SocketGuildUser;

        //    if (user != null)
        //    {

        //        await ReplyAsync("", false, await  _embedService.Pic(user.Id));

        //        return;
        //    }

        //        EmbedBuilder builder = new EmbedBuilder();
        //        builder.WithTitle("Avatar")
        ////.WithThumbnailUrl(Context.User.GetAvatarUrl())
        //.WithImageUrl(Context.User.GetAvatarUrl(ImageFormat.Auto, 128))
        //.WithFooter($"{Context.User.Username}");

        //        await ReplyAsync("", false, builder.Build());


        //}

        //public async Task Ping()
        //{
        //    ISocketMessageChannel channel = base.Context.Channel;
        //    //await ReplyAsync("Ping: " + new Ping().Send("google.com").RoundtripTime.ToString() + " ms");
        //    //await channel.
        //    ("Here is a button!", component: new ComponentBuilder()
        //    //                .WithButton(new
        //    ("label", "customidhere"))
        //    //                .Build());
        //}  
        [Command("SendMessage")]
        [Summary("Check whether the bot is working or not.")]
        public async Task SendMessage(Discord.WebSocket.SocketGuildUser user, string text)
        {
            // await Context.Channel.SendMessageAsync(text);

            await user.SendMessageAsync(text);


        }
        [Command("pick")]
        [Alias(new string[] { "p" })]

        [Summary("Pick a player to join your team.")]
        public async Task PickPlayer(SocketGuildUser player)
        {
            SocketTextChannel channel = base.Context.Channel as SocketTextChannel;
            if (channel == null)
            {
                await ReplyAsync("");

                return;
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, channel.Id);
            if (match == null)
            {

                await ReplyAsync("This is not a match channel or something weird happened.");
                return;
            }
            if (match.PickingPoolDiscordIds.Count <= 0)
            {
                await ReplyAsync("The match is ready and teams have already been picked!");
                return;
            }
            if (match.ActiveCaptainDiscordId != base.Context.User.Id)
            {
                await ReplyAsync(base.Context.User.Mention + " you are the active captain.");
                return;
            }
            if (!match.PickingPoolDiscordIds.Contains(player.Id))
            {
                await ReplyAsync(player.Mention + " is not in the player pool.");
                return;
            }
            SocketVoiceChannel team1Voice = base.Context.Guild.GetVoiceChannel(match.Team1VoiceChannelId);
            SocketVoiceChannel team2Voice = base.Context.Guild.GetVoiceChannel(match.Team2VoiceChannelId);
            OverwritePermissions connectFalse = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny);
            await team1Voice.AddPermissionOverwriteAsync(base.Context.Guild.EveryoneRole, connectFalse);
            await team2Voice.AddPermissionOverwriteAsync(base.Context.Guild.EveryoneRole, connectFalse);
            OverwritePermissions connectTrue = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
            if (match.ActiveCaptainDiscordId == match.Captain1DiscordId)
            {
                await team1Voice.AddPermissionOverwriteAsync(base.Context.Guild.GetUser(player.Id), connectTrue);
                match.Team1DiscordIds.Add(player.Id);
                match.PickingPoolDiscordIds.RemoveAt(match.PickingPoolDiscordIds.IndexOf(player.Id));
                match.ActiveCaptainDiscordId = match.Captain2DiscordId;
                if (match.PickingPoolDiscordIds.Count >= 2)
                {
                    await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                    ISocketMessageChannel channel2 = base.Context.Channel;
                    await channel2.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
                }
                else
                {
                    if (match.PickingPoolDiscordIds.Count != 1 || match.PickingPoolDiscordIds[0] == 0L)
                    {
                        return;
                    }
                    match.Team2DiscordIds.Add(match.PickingPoolDiscordIds[0]);
                    await team2Voice.AddPermissionOverwriteAsync(base.Context.Guild.GetUser(match.PickingPoolDiscordIds[0]), connectTrue);
                    match.PickingPoolDiscordIds.RemoveAt(0);
                    await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                    foreach (ulong discordId2 in match.Team1DiscordIds)
                    {
                        SocketGuildUser moveMe3 = base.Context.Guild.GetUser(discordId2);
                        if (moveMe3 != null && moveMe3.VoiceChannel != null)
                        {
                            await moveMe3.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                x.Channel = team1Voice;
                            });
                        }
                    }
                    foreach (ulong discordId4 in match.Team2DiscordIds)
                    {
                        SocketGuildUser moveMe4 = base.Context.Guild.GetUser(discordId4);
                        if (moveMe4 != null && moveMe4.VoiceChannel != null)
                        {
                            await moveMe4.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                x.Channel = team2Voice;
                            });
                        }
                    }
                }
            }
            else
            {
                if (match.ActiveCaptainDiscordId != match.Captain2DiscordId)
                {
                    return;
                }
                await team2Voice.AddPermissionOverwriteAsync(base.Context.Guild.GetUser(player.Id), connectTrue);
                match.Team2DiscordIds.Add(player.Id);
                match.PickingPoolDiscordIds.RemoveAt(match.PickingPoolDiscordIds.IndexOf(player.Id));
                match.ActiveCaptainDiscordId = match.Captain1DiscordId;
                if (match.PickingPoolDiscordIds.Count > 2)
                {
                    await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                }
                else if (match.AllPlayerDiscordIds.Count == 8 && match.PickingPoolDiscordIds.Count == 2)
                {
                    match.ActiveCaptainDiscordId = match.Captain2DiscordId;
                    await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                }
                else if (match.PickingPoolDiscordIds.Count == 1 && match.PickingPoolDiscordIds[0] != 0L)
                {
                    match.Team1DiscordIds.Add(match.PickingPoolDiscordIds[0]);
                    await team1Voice.AddPermissionOverwriteAsync(base.Context.Guild.GetUser(match.PickingPoolDiscordIds[0]), connectTrue);
                    match.PickingPoolDiscordIds.RemoveAt(0);
                    await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                    foreach (ulong discordId3 in match.Team1DiscordIds)
                    {
                        SocketGuildUser moveMe2 = base.Context.Guild.GetUser(discordId3);
                        if (moveMe2 != null && moveMe2.VoiceChannel != null)
                        {
                            await moveMe2.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                // x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)team1Voice;
                            });
                        }
                    }
                    foreach (ulong discordId in match.Team2DiscordIds)
                    {
                        SocketGuildUser moveMe = base.Context.Guild.GetUser(discordId);
                        if (moveMe != null && moveMe.VoiceChannel != null)
                        {
                            await moveMe.ModifyAsync(delegate (GuildUserProperties x)
                            {
                                //x.Channel = (Optional<IVoiceChannel>)(IVoiceChannel)team2Voice;
                            });
                        }
                    }
                }
                ISocketMessageChannel channel2 = base.Context.Channel;
                await channel2.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));
            }
        }


        [Command("changemap")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Vote to change to another random map.")]
        public async Task VoteMap()
        {
            SocketTextChannel channel = base.Context.Channel as SocketTextChannel;
            if (channel == null)
            {
                return;
            }
            Match match = await _databaseService.GetMatchForChannelAsync(base.Context.Guild.Id, channel.Id);
            if (match == null)
            {
                await ReplyAsync("This is not a match channel or something weird happened.");
                return;
            }
            if (match.MapChangeVoteDiscordIds.Contains(base.Context.User.Id))
            {
                await ReplyAsync(base.Context.User.Mention + " you have already voted to change the map.");
                return;
            }
            match.MapChangeVoteDiscordIds.Add(base.Context.User.Id);
            int mapChangeVotesNeeded = _queueService.GetPugQueue(match.PugQueueMessageId).Capacity / 2 + 1;
            if (match.MapChangeVoteDiscordIds.Count < mapChangeVotesNeeded)
            {
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                await ReplyAsync($"{base.Context.User.Mention} has voted to change the map. `{match.MapChangeVoteDiscordIds.Count} / {mapChangeVotesNeeded}`");
            }
            else
            {
                foreach (QueueConfig qConfig in (await _databaseService.GetServerConfigAsync(base.Context.Guild.Id)).QueueConfigs)
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
                await _databaseService.UpsertMatchAsync(base.Context.Guild.Id, match);
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                const int delay = 3000;


                // IUserMessage m = await ReplyAsync($"I have deleted {amount} messages for ya. :)");
                // await Task.Delay(delay);
                // await m.DeleteAsync();
                ISocketMessageChannel channelkero = base.Context.Channel; //i found the problem, your embed service is returned null

                RestUserMessage blankEmbedMessage = await channelkero.SendMessageAsync(null, isTTS: false, await _embedService.GetMatchEmbedAsync(match, base.Context.Guild.Id));


                ButtonBuilder bl = new ButtonBuilder() { Label = "🏆-Team #1 WIN", IsDisabled = false, Style = ButtonStyle.Success, CustomId = "bl" };
                ButtonBuilder gr = new ButtonBuilder() { Label = "🏆-Team #2 WIN", IsDisabled = false, Style = ButtonStyle.Success, CustomId = "gr" };
                ButtonBuilder map = new ButtonBuilder() { Label = "Change Map 🗺️", IsDisabled = false, Style = ButtonStyle.Primary, CustomId = "map" };
                ButtonBuilder Cancel = new ButtonBuilder() { Label = "Cancel ❌", IsDisabled = false, Style = ButtonStyle.Danger, CustomId = "Cancel" };
                ComponentBuilder componentBuilder = new ComponentBuilder()
                      .WithButton(bl)
                      .WithButton(gr)
                      .WithButton(map)
                     .WithButton(Cancel);
                await blankEmbedMessage.ModifyAsync(x => x.Components = componentBuilder.Build());
                await ReplyAsync("The map vote has passed and the map has been changed to: `" + match.Map + "`!");

            }
            await base.Context.Message.DeleteMessageAfterSeconds(2);
        }
        //[Command("setnickname")]
        //[Summary("")]
        //public async Task SetNickCF(string url)
        //{
        //    if (url == null)
        //    {

        //        await (await ReplyAsync($"{Config.Prfix}setnickname [URL = https://crossfire.z8games.com/profile/19323248] ")).DeleteMessageAfterSeconds(2);
        //        return;

        //    }
        //    SocketUser user = base.Context.User;
        //    var html = new WebClient().DownloadString(url);
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.Load(html);
        //    var result = doc.DocumentNode.SelectNodes("//h1[@class='PlayerHeader_ign_heading__AT-l_']");
        //    ISocketMessageChannel channel = base.Context.Channel;
        //    await channel.SendMessageAsync(result.ToString());
        //}
        [Command("stats")]
        [Alias(new string[] { "profile", "statistics" })]
        [Summary("See your profile, or pull up a user's profile!")]
        public async Task SeeProfile(SocketGuildUser user = null)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (base.Context.Channel.Id != config.StatsChannelId)
            {
                await ReplyAsync(base.Context.User.Mention + ", please use this command in the stats channel only." + config.StatsChannelId + "Channel ID" + base.Context.Channel.Id);

                await base.Context.Message.DeleteAsync();
                return;
            }
            SocketUser user2 = base.Context.User;
            SocketGuildUser member = user2 as SocketGuildUser;
            if (member != null)
            {
                if (user != null)
                {
                    member = user;
                }
                DbUser dbUser = await _databaseService.GetUserInGuild(member.Id, base.Context.Guild.Id);
                if (dbUser == null)
                {
                    await ReplyAsync(member.Mention + " does not have a profile in our system yet. To get one, simply play a match.");
                    return;
                }
                ISocketMessageChannel channel = base.Context.Channel;
                await channel.SendMessageAsync(null, isTTS: false, _embedService.ProfileEmbed(dbUser));


            }
        }

        [Command("leaderboard")]
        [Alias(new string[] { "lb" })]
        [Summary("See the top 25 users in your server.")]
        public async Task SeeLeaderboard(SocketGuildUser user = null)
        {
            ServerConfig config = await _databaseService.GetServerConfigAsync(base.Context.Guild.Id);
            if (base.Context.Channel.Id != config.StatsChannelId)
            {
                await ReplyAsync(base.Context.User.Mention + ", please use this command in the stats channel only.");
                await base.Context.Message.DeleteAsync();
            }
            else
            {
                ISocketMessageChannel channel = base.Context.Channel;
                EmbedService embedService = _embedService;
                await channel.SendMessageAsync(null, isTTS: false, await embedService.LeaderboardEmbed(await _databaseService.GetTop25Users(base.Context.Guild.Id)));
            }
        }
    }
}
