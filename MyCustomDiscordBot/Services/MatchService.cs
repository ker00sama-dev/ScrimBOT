using MongoDB.Bson;
using MongoDB.Driver;
using MyCustomDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class MatchService
    {
        private readonly DatabaseService _databaseService;

        public MatchService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task CalculateWinForMatch(Match match, int winningTeamNumber, ulong guildId)
        {
            if (match.SortType == SortType.Scrim)
            {
                switch (winningTeamNumber)
                {
                    case 1:
                        {
                            Team winners2 = await _databaseService.GetTeamInGuild(match.Captain1DiscordId, guildId);
                            winners2.Points += 10;
                            await _databaseService.UpsertTeamAsync(guildId, winners2);
                            break;
                        }
                    case 2:
                        {
                            Team winners = await _databaseService.GetTeamInGuild(match.Captain2DiscordId, guildId);
                            winners.Points += 10;
                            await _databaseService.UpsertTeamAsync(guildId, winners);
                            break;
                        }
                }
            }
            match.Winners = winningTeamNumber;
            ServerConfig config = await _databaseService.GetServerConfigAsync(guildId);
            switch (winningTeamNumber)
            {
                case 1:
                    foreach (ulong discordId4 in match.Team1DiscordIds)
                    {
                        await calculateWinForUser(await _databaseService.GetUserInGuild(discordId4, guildId));
                    }
                    foreach (ulong discordId3 in match.Team2DiscordIds)
                    {
                        await calculateLossForUser(await _databaseService.GetUserInGuild(discordId3, guildId));
                    }
                    break;
                case 2:
                    foreach (ulong discordId2 in match.Team2DiscordIds)
                    {
                        await calculateWinForUser(await _databaseService.GetUserInGuild(discordId2, guildId));
                    }
                    foreach (ulong discordId in match.Team1DiscordIds)
                    {
                        await calculateLossForUser(await _databaseService.GetUserInGuild(discordId, guildId));
                    }
                    break;
            }
            await _databaseService.UpsertMatchAsync(guildId, match);
            async Task calculateLossForUser(DbUser user)
            {
                user.ELO -= config.LossAmount;
                if (user.ELO < 0)
                {
                    user.ELO = 0;
                }
                bool mapFound = false;
                foreach (MapRecord mapRecord in user.MapRecords)
                {
                    if (mapRecord.MapName.ToLower() == match.Map.ToLower())
                    {
                        mapFound = true;
                        mapRecord.Losses++;
                        break;
                    }
                }
                if (!mapFound)
                {
                    MapRecord record = new MapRecord(match.Map);
                    record.Losses++;
                    user.MapRecords.Add(record);
                }
                await _databaseService.UpsertUser(guildId, user);
            }
            async Task calculateWinForUser(DbUser user)
            {
                user.ELO += config.WinAmount;
                bool mapFound2 = false;
                foreach (MapRecord mapRecord2 in user.MapRecords)
                {
                    if (mapRecord2.MapName.ToLower() == match.Map.ToLower())
                    {
                        mapFound2 = true;
                        mapRecord2.Wins++;
                        break;
                    }
                }
                if (!mapFound2)
                {
                    MapRecord record2 = new MapRecord(match.Map);
                    record2.Wins++;
                    user.MapRecords.Add(record2);
                }
                await _databaseService.UpsertUser(guildId, user);
            }
        }

        public async Task<Match> GenerateMatchAsync(List<DbUser> usersInQueue, SortType sortType, ulong guildId, ulong queueMessageId)
        {
            Match match = new Match();
            IMongoCollection<Match> collection = _databaseService.GetMatchesForGuild(guildId);
            Match match2 = match;
            match2.Number = await collection.CountDocumentsAsync(new BsonDocument()) + 1;
            match.AllPlayerDiscordIds = usersInQueue.Select((DbUser x) => x.DiscordId).ToList();
            Random randomer = new Random();
            QueueConfig queueConfig = (await _databaseService.GetServerConfigAsync(guildId)).QueueConfigs.Find((QueueConfig x) => x.MessageId == queueMessageId);
            int randomMapIndex = randomer.Next(queueConfig.Maps.Count);
            match.Map = queueConfig.Maps[randomMapIndex];
            switch (sortType)
            {
                case SortType.Elo:
                    {
                        usersInQueue.Sort((DbUser a, DbUser b) => (a.ELO <= b.ELO) ? 1 : (-1));
                        for (int i = 0; i < usersInQueue.Count; i++)
                        {
                            if (i % 2 == 0)
                            {
                                match.Team1DiscordIds.Add(usersInQueue[i].DiscordId);
                            }
                            else
                            {
                                match.Team2DiscordIds.Add(usersInQueue[i].DiscordId);
                            }
                        }
                        match.Captain1DiscordId = match.Team1DiscordIds[0];
                        match.Captain2DiscordId = match.Team2DiscordIds[0];
                        match.ActiveCaptainDiscordId = match.Captain1DiscordId;
                        break;
                    }
                case SortType.Captains:
                    {
                        match.PickingPoolDiscordIds = match.AllPlayerDiscordIds.ToList();
                        int captain1Index = randomer.Next(match.PickingPoolDiscordIds.Count);
                        match.Captain1DiscordId = match.PickingPoolDiscordIds[captain1Index];
                        match.PickingPoolDiscordIds.RemoveAt(captain1Index);
                        int captain2Index = randomer.Next(match.PickingPoolDiscordIds.Count);
                        match.Captain2DiscordId = match.PickingPoolDiscordIds[captain2Index];
                        match.PickingPoolDiscordIds.RemoveAt(captain2Index);
                        match.Team1DiscordIds.Add(match.Captain1DiscordId);
                        match.Team2DiscordIds.Add(match.Captain2DiscordId);
                        match.SortType = sortType;
                        match.ActiveCaptainDiscordId = match.Captain1DiscordId;
                        break;
                    }
            }
            await _databaseService.UpsertMatchAsync(guildId, match);
            return match;
        }
    }
}
