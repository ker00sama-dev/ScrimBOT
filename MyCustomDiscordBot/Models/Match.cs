using MongoDB.Bson;
using System.Collections.Generic;

namespace MyCustomDiscordBot.Models
{
    public class Match
    {
        public ObjectId Id { get; set; }

        public long Number { get; set; }

        public List<ulong> AllPlayerDiscordIds { get; set; }

        public List<ulong> PickingPoolDiscordIds { get; set; }

        public SortType SortType { get; set; }

        public ulong Captain1DiscordId { get; set; }

        public ulong Captain2DiscordId { get; set; }

        public List<ulong> Team1DiscordIds { get; set; }

        public List<ulong> Team2DiscordIds { get; set; }

        public int Winners { get; set; }

        public ulong ActiveCaptainDiscordId { get; set; }

        public string Map { get; set; }

        public ulong Team1VoiceChannelId { get; set; }

        public ulong Team2VoiceChannelId { get; set; }

        public ulong MatchInfoChannelId { get; set; }

        public ulong PugQueueMessageId { get; set; }

        public List<ulong> MapChangeVoteDiscordIds { get; set; }

        public Match()
        {
            Id = ObjectId.GenerateNewId();
            AllPlayerDiscordIds = new List<ulong>();
            PickingPoolDiscordIds = new List<ulong>();
            Team1DiscordIds = new List<ulong>();
            Team2DiscordIds = new List<ulong>();
            SortType = SortType.Elo;
            MapChangeVoteDiscordIds = new List<ulong>();
            Winners = -1;
        }
    }
}
