using MyCustomDiscordBot.Models;
using System.Collections.Generic;

namespace MyCustomDiscordBot.Services
{
    public class PugQueue
    {
        public ulong MessageId { get; }

        public ulong ChannelId { get; }

        public ulong GuildId { get; }

        public List<DbUser> Users { get; }

        public int Capacity { get; }

        public string Name { get; }

        public List<string> Maps { get; }

        public SortType SortType { get; }

        public PugQueue(QueueConfig config)
        {
            MessageId = config.MessageId;
            ChannelId = config.ChannelId;
            GuildId = config.GuildId;
            Users = new List<DbUser>();
            Capacity = config.Capacity;
            Name = config.Name;
            SortType = config.SortType;
            Maps = config.Maps;
        }
    }
}
