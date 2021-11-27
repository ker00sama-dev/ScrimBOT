using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using VMProtect;

using System.Collections.Generic;
namespace MyCustomDiscordBot.Models
{
    [BsonIgnoreExtraElements]
    public class DbUser
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public ulong DiscordId { get; set; }

        public int ELO { get; set; }

        public List<MapRecord> MapRecords { get; set; }

        public DateTime SuspensionReturnDate { get; set; }

        public ObjectId TeamId { get; set; }

        public  DbUser(string username, ulong discordId)
        {

            Id = ObjectId.GenerateNewId();
            DiscordId = discordId;
            Username = username;
            DiscordId = discordId;
            ELO = 0;
            MapRecords = new List<MapRecord>();
        }
    }
}
