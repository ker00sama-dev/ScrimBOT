using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using MyCustomDiscordBot.Extensions;
using MyCustomDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MyCustomDiscordBot.Models
{
    public class Match
    {
        private Random random = new Random();

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
        public string pwd { get; set; }

        public List<ulong> MapChangeVoteDiscordIds { get; set; }
        public  string RandomString(int length)
        {
          
            return new string((from s in Enumerable.Repeat("123364654654564974548548484", length)
                               select s[random.Next(s.Length)]).ToArray());
        }
        public Match()
        {
            Id = ObjectId.GenerateNewId();
            AllPlayerDiscordIds = new List<ulong>();
            PickingPoolDiscordIds = new List<ulong>();
            Team1DiscordIds = new List<ulong>();
            Team2DiscordIds = new List<ulong>();
            SortType = SortType.Elo;
            pwd = RandomString(3).ToLower();
            MapChangeVoteDiscordIds = new List<ulong>();
            Winners = -1;
        }
    }
}
