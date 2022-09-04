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

        public ulong Startfull { get; set; }
        public ulong Centerfull { get; set; }
        public ulong Endfull { get; set; }
        public ulong Startnull { get; set; }
        public ulong Centernull { get; set; }
        public ulong Endnull { get; set; }
        public ulong norank { get; set; }
        public ulong Bronze { get; set; }
        public ulong Silver { get; set; }
        public ulong Gold { get; set; }
        public ulong Platinum { get; set; }
        public ulong Diamond { get; set; }
        public ulong Master { get; set; }
        public ulong legend { get; set; }
        public ulong mythical { get; set; }
        public ulong GrandMaster { get; set; }
        public int checkranking { get; set; }
        public string prefix { get; set; }

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
            checkranking = 0;
            QueueConfigs = new List<QueueConfig>();
            MaximumTeamSize = 0;
            WaitingForMatchChannelId = 0uL;
            Startfull = 0uL;
            Centerfull = 0uL;
            Endfull = 0uL;
            Startnull = 0uL;
            Centernull = 0uL;
            Endnull = 0uL;
            norank = 0uL;
            Bronze = 0uL;
            Silver = 0uL;
            Gold = 0uL;
            Platinum = 0uL;
            Diamond = 0uL;
            Master = 0uL;
            legend = 0uL;
            mythical = 0uL;
            GrandMaster = 0uL;
            prefix = "!";
        }
    }
}
