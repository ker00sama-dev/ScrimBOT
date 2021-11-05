using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot.Services;
using MyCustomDiscordBot.Settings;
using System;

namespace MyCustomDiscordBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
            {
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<BotSettings>(configuration.GetSection("BotSettings"));
                services.AddSingleton<DatabaseService>().AddSingleton<DiscordSocketClient>().AddSingleton<UtilityService>()
                    .AddSingleton<GlobalServersService>()
                    .AddSingleton<BaseSocketClient, DiscordSocketClient>((IServiceProvider sp) => sp.GetRequiredService<DiscordSocketClient>())
                    .AddSingleton<CommandHandler>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<EmbedService>()
                    .AddSingleton<MatchService>()
                    .AddSingleton<QueueService>()
                    .AddSingleton<ScrimService>();
                services.AddHostedService<Worker>();
            }).ConfigureLogging(SetLogging);
        }

        public static void SetLogging(HostBuilderContext context, ILoggingBuilder builder)
        {
            builder.AddConfiguration(context.Configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        }
    }
}
