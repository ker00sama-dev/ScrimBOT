using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyCustomDiscordBot.Settings;
using System.Threading;
using System.Threading.Tasks;
using static MyCustomDiscordBot.MyCustomDiscordBot.DiscordBOTGaming;
using VMProtect;

namespace MyCustomDiscordBot
{
    public class Worker : BackgroundService
    {
        private readonly BaseSocketClient _client;

        private readonly BotSettings _botSettings;

        private readonly CommandHandler _commandHandler;

        public Worker(BaseSocketClient client, IOptions<BotSettings> botSettings, CommandHandler commandHandler)
        {
            _botSettings = botSettings.Value;
            _client = client;
            _commandHandler = commandHandler;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.LoginAsync(TokenType.Bot, Config.token);
            await _client.SetGameAsync("ScrimBOT.Me", "", ActivityType.Watching);
            await _client.StartAsync();
            await _commandHandler.Init();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
