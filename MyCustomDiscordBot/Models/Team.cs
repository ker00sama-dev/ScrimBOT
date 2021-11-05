using MongoDB.Bson;
using System.Collections.Generic;

namespace MyCustomDiscordBot.Models
{
    public class Team
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public List<ulong> MemberDiscordIds { get; set; }

        public int Points { get; set; }

        public List<MapRecord> MapRecords { get; set; }

        public ulong CaptainDiscordId { get; set; }

        public Team(string name)
        {
            Id = ObjectId.GenerateNewId();
            Name = name;
            MemberDiscordIds = new List<ulong>();
            Points = 0;
            MapRecords = new List<MapRecord>();
        }
    }
}
