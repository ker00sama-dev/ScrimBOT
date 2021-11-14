using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyCustomDiscordBot.MyCustomDiscordBot;
using MyCustomDiscordBot.Services;
using MyCustomDiscordBot.Settings;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
#pragma warning disable CS0105 // The using directive for 'MyCustomDiscordBot.MyCustomDiscordBot' appeared previously in this namespace
using MyCustomDiscordBot.MyCustomDiscordBot;
#pragma warning restore CS0105 // The using directive for 'MyCustomDiscordBot.MyCustomDiscordBot' appeared previously in this namespace

namespace MyCustomDiscordBot
{
    public class Program
    {
        public enum MemoryProtectionConsts : uint
        {
            EXECUTE = 0x10,
            EXECUTE_READ = 0x20,
            EXECUTE_READWRITE = 0x40,
            NOACCESS = 0x01,
            READONLY = 0x02,
            READWRITE = 0x04
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, MemoryProtectionConsts flNewProtect,
            int lpflOldProtect);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "RtlSecureZeroMemory")]

        private static extern void SecureZeroMemory(IntPtr ptr, IntPtr cnt);
        private static int ErasePEHeader() // hModule = Handle to the module, procName = Process name (eg. "notepad")
        {
            int OldProtect = 0;
            IntPtr pBaseAddr = GetModuleHandle(null);
            VirtualProtect(pBaseAddr, 4096, // Assume x86 page size
             MemoryProtectionConsts.READWRITE, OldProtect);
            SecureZeroMemory(pBaseAddr, (IntPtr)4096);
            return 0;
        }

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {



            //AntiDebuggers antiDebuggers = new AntiDebuggers();

            //Task.Run(() => antiDebuggers.RegisteredWaitHandleAssemblyProductAttribute());
            Mutex mutex = new System.Threading.Mutex(false, "MyUniqueMutexName");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    // Run the application


                   // CreateHostBuilder(args).Build().Run();

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new DiscordBOTGaming());
                }
                else
                {
                    MessageBox.Show("An instance of the application is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
        //public  static void kero()
        //  {
        //     
        //  }//where are your handling 

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices(delegate (HostBuilderContext hostContext, IServiceCollection services)
            {
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<BotSettings>(configuration.GetSection("BotSettings"));
                DiscordSocketClient discord = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    GatewayIntents = Discord.GatewayIntents.All,
                });
                services.AddSingleton<DatabaseService>()
                .AddSingleton(discord)//lets try now
                .AddSingleton<UtilityService>()
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
