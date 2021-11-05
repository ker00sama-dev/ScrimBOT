using System.Collections.Generic;

namespace MyCustomDiscordBot.Models
{
    public class QueueConfig
    {
        public int Capacity { get; set; }

        public ulong ChannelId { get; set; }

        public ulong MessageId { get; set; }

        public ulong GuildId { get; set; }

        public SortType SortType { get; set; }

        public string Name { get; set; }

        public List<string> Maps { get; set; }

        public QueueConfig(string name, int capacity, ulong guildId, ulong channelId, ulong messageId)
        {
            Capacity = capacity;
            ChannelId = channelId;
            GuildId = guildId;
            Name = name;
            MessageId = messageId;
            SortType = SortType.Elo;
            Maps = new List<string>();
        }
    }
}
