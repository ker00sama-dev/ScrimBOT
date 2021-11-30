using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;

using static MyCustomDiscordBot.MyCustomDiscordBot.DiscordBOTGaming;
using Microsoft.VisualBasic;
using System.Data;
using System;
using Discord.WebSocket;
using MyCustomDiscordBot.Extensions;
using System.Threading;

namespace MyCustomDiscordBot.Services
{
    public class DatabaseService
    {
        public readonly BotSettings _botSettings;
        private readonly DiscordSocketClient _client;

        public static MongoClient Client { get; set; }

        public List<IMongoDatabase> Databases { get; set; }

        public DatabaseService(DiscordSocketClient client, IOptions<BotSettings> botSettings)
        {
            _client = client;
            _botSettings = botSettings.Value;
            Client = new MongoClient(Config.DBConnectionString);
            Databases = new List<IMongoDatabase>();
        }


        public IMongoDatabase GetDatabaseFromGuild(ulong guildId)
        {
            return Client.GetDatabase(guildId.ToString());
        }


        public IMongoCollection<Team> GetTeamsForGuild(ulong guildId)
        {
            return GetDatabaseFromGuild(guildId).GetCollection<Team>("Teams");
        }

        public IMongoCollection<DbUser> GetUsersForGuild(ulong guildId)
        {
            return GetDatabaseFromGuild(guildId).GetCollection<DbUser>("Users");
        }

        public async Task<List<DbUser>> GetTop25Users(ulong guildId)
        {
            IMongoCollection<DbUser> collection = GetUsersForGuild(guildId);
            List<DbUser> listCursor = (await collection.FindAsync(new BsonDocument())).ToList();
            listCursor.Sort((DbUser a, DbUser b) => (a.ELO <= b.ELO) ? 1 : (-1));
            return listCursor.Take(5).ToList();
        }


        public async Task<DbUser> GetUserInGuild(ulong userDiscordId, ulong guildId)
        {
            IMongoCollection<DbUser> collection = GetUsersForGuild(guildId);
            return await (await collection.FindAsync((DbUser x) => x.DiscordId == userDiscordId)).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeamInGuild(ulong discordId, ulong guildId)
        {
            IMongoCollection<Team> collection = GetTeamsForGuild(guildId);
            return await (await collection.FindAsync((Team x) => x.MemberDiscordIds.Contains(discordId))).FirstOrDefaultAsync();
        }

        public async Task<Team> GetTeamInGuild(ObjectId teamId, ulong guildId)
        {
            IMongoCollection<Team> collection = GetTeamsForGuild(guildId);
            return await (await collection.FindAsync((Team x) => x.Id == teamId)).FirstOrDefaultAsync();
        }

        public IMongoCollection<Match> GetMatchesForGuild(ulong guildId)
        {
            return GetDatabaseFromGuild(guildId).GetCollection<Match>("Matches");
        }

        public async Task UpsertTeamAsync(ulong guildId, Team team)
        {
            IMongoCollection<Team> collection = GetTeamsForGuild(guildId);
            if (await collection.CountDocumentsAsync((Team x) => x.Id == team.Id) > 0)
            {
                await collection.ReplaceOneAsync((Team x) => x.Id == team.Id, team);
            }
            else
            {
                await collection.InsertOneAsync(team);
            }
        }

        public async Task UpsertMatchAsync(ulong guildId, Match match)
        {
            IMongoCollection<Match> collection = GetMatchesForGuild(guildId);
            if (await collection.CountDocumentsAsync((Match x) => x.Number == match.Number) > 0)
            {
                await collection.ReplaceOneAsync((Match x) => x.Number == match.Number, match);
            }
            else
            {
                await collection.InsertOneAsync(match);
            }
        }

        public async Task<Match> GetMatchForChannelAsync(ulong guildId, ulong matchInfoChannelId)
        {
            IMongoCollection<Match> collection = GetMatchesForGuild(guildId);
            return await (await collection.FindAsync((Match x) => x.MatchInfoChannelId == matchInfoChannelId)).FirstOrDefaultAsync();
        }

        public async Task<bool> ServerConfigExistsAsync(ulong guildId)
        {
            IMongoDatabase database = GetDatabaseFromGuild(guildId);
            IMongoCollection<ServerConfig> collection = database.GetCollection<ServerConfig>("ServerConfigs");
            if (await collection.CountDocumentsAsync((ServerConfig x) => x.GuildId == guildId) <= 0)
            {
                return false;
            }
            return true;
        }

        public async Task DeleteTeam(ulong guildId, Team team)
        {
            IMongoCollection<Team> collection = GetTeamsForGuild(guildId);
            await collection.DeleteOneAsync((Team x) => x.Id == team.Id);
        }

        public async Task UpsertUser(ulong guildId, DbUser user)
        {
            IMongoCollection<DbUser> collection = GetUsersForGuild(guildId);
            if (await (await collection.FindAsync((DbUser x) => x.DiscordId == user.DiscordId)).FirstOrDefaultAsync() == null)
            {
                await collection.InsertOneAsync(user);
                return;
            }
            await collection.ReplaceOneAsync((DbUser x) => x.DiscordId == user.DiscordId, user);
        }

        public async Task ResetUser(ulong guildId, ISocketMessageChannel channel, string Mention, SocketGuild Guild)
        {
            IMongoCollection<DbUser> collection = GetUsersForGuild(guildId);

            List<DbUser> listCursor = (await collection.FindAsync(new BsonDocument())).ToList();
            List<DbUser> topPlayers = listCursor.ToList();
            ServerConfig config = await GetServerConfigAsync(Guild.Id);
            var Message = await channel.SendMessageAsync("Starting....");


            for (int i = 0; i < topPlayers.Count; i++)
            {
            
                var filter = Builders<DbUser>.Filter.Eq("ELO", topPlayers[i].ELO);
                var update = Builders<DbUser>.Update.Set("ELO", 0);
                await collection.UpdateManyAsync(filter, update);
                SocketUser user = _client.GetUser(topPlayers[i].DiscordId);
                //string kero = $"{user.Mention} => ELO : ({topPlayers[i].ELO}) => 0 ";
                //await Message.ModifyAsync(msg => msg.Content = kero);


            }
         
            await Message.ModifyAsync(msg => msg.Content = ":white_check_mark: Done");
            await (Message).DeleteMessageAfterSeconds(2);

            SocketTextChannel matchLogsChannel = Guild.GetTextChannel(config.MatchLogsChannelId);
            SocketTextChannel socketTextChannel = matchLogsChannel;
            await socketTextChannel.SendMessageAsync($"`ELO` Has been Reset By {Mention}");


            // await Collection.InsertManyAsync(batch.AsEnumerable());


        }
        public async Task UpsertServerConfigAsync(ServerConfig config)
        {
            IMongoDatabase database = GetDatabaseFromGuild(config.GuildId);
            IMongoCollection<ServerConfig> collection = database.GetCollection<ServerConfig>("ServerConfigs");
            if (await collection.CountDocumentsAsync((ServerConfig x) => x.GuildId == config.GuildId) > 0)
            {
                await collection.ReplaceOneAsync((ServerConfig x) => x.GuildId == config.GuildId, config);
            }
            else
            {
                await collection.InsertOneAsync(config);
            }
        }

        public async Task<ServerConfig> GetServerConfigAsync(ulong guildId)
        {
            IMongoDatabase database = Client.GetDatabase(guildId.ToString());
            IMongoCollection<ServerConfig> collection = database.GetCollection<ServerConfig>("ServerConfigs");
            return await (await collection.FindAsync(new BsonDocument())).FirstOrDefaultAsync();
        }
      

        public async Task<DbUser> RegisterUserAsync(ulong guildId, string username, ulong userDiscordId)
        {
            IMongoCollection<DbUser> collection = GetUsersForGuild(guildId);
            DbUser user = new DbUser(username, userDiscordId);
            await collection.InsertOneAsync(user);
            return user;
        }
    }
}
