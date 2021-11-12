using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace MyCustomDiscordBot.Models
{
    public class DbUser
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public ulong DiscordId { get; set; }

        public int ELO { get; set; }

        public List<MapRecord> MapRecords { get; set; }

        public DateTime SuspensionReturnDate { get; set; }

        public ObjectId TeamId { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }

        public void IncreaseExperience(int experience)
        {
            static int MaxExperience(int Level) => (Level + 20) * 50;


            Experience += experience;
            var maxExperience = MaxExperience(Level);
            if (Experience >= maxExperience)
            {
                Experience = Experience - maxExperience;
                Level++;
            }
        }


        public DbUser(string username, ulong discordId)
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
