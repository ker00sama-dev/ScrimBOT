using Discord.WebSocket;
using MyCustomDiscordBot.Models;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Services
{
    public class GlobalServersService
    {
        private readonly DiscordSocketClient _client;

        private readonly DatabaseService _databaseService;

        public GlobalServersService(DiscordSocketClient client, DatabaseService databaseService)
        {
            _client = client;
            _databaseService = databaseService;
        }

        public async Task<bool> ConfigureNewServer(ulong guildId)
        {
            ServerConfig config = new ServerConfig(guildId);
            if (await _databaseService.ServerConfigExistsAsync(guildId))
            {
                return true;
            }
            try
            {
                await _databaseService.UpsertServerConfigAsync(config);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
