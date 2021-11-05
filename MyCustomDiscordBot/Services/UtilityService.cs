using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class UtilityService
    {
        private readonly DiscordSocketClient _client;

        public UtilityService(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task<IMessage> GetMessageFromTextChannel(ulong guildId, ulong channelId, ulong messageId)
        {
            SocketTextChannel channel = _client.GetGuild(guildId).GetTextChannel(channelId);
            IMessage returnVal = channel.GetCachedMessage(messageId);
            if (returnVal == null)
            {
                returnVal = await channel.GetMessageAsync(messageId);
            }
            return returnVal;
        }
    }
}
