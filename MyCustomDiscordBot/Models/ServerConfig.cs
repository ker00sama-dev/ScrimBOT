using MongoDB.Bson;
using System.Collections.Generic;
namespace MyCustomDiscordBot.Models
{
    public class ServerConfig
    {
        public ObjectId Id { get; set; }

        public ulong GuildId { get; set; }

        public ulong MatchLogsChannelId { get; set; }

        public ulong StatsChannelId { get; set; }

        public ulong MatchesCategoryId { get; set; }

        public ulong ScrimBoardMessageId { get; set; }

        public ulong ScrimBoardChannelId { get; set; }

        public List<QueueConfig> QueueConfigs { get; set; }

        public ulong ScoreReporterRoleId { get; set; }

        public int WinAmount { get; set; }

        public int LossAmount { get; set; }

        public int MaximumTeamSize { get; set; }

        public ulong WaitingForMatchChannelId { get; set; }

        public ServerConfig(ulong guildId)
        {
            Id = ObjectId.GenerateNewId();
            GuildId = guildId;
            MatchLogsChannelId = 0uL;
            StatsChannelId = 0uL;
            MatchesCategoryId = 0uL;
            ScoreReporterRoleId = 0uL;
            WinAmount = 12;
            LossAmount = 7;
            QueueConfigs = new List<QueueConfig>();
            MaximumTeamSize = 0;
            WaitingForMatchChannelId = 0uL;
        }
    }
}
