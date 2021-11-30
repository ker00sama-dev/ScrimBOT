using Discord;
using VMProtect;

namespace DiscordBot.Modules
{
    public class EmbedHelper
    {
        public static Microsoft.Win32.RegistryKey XXXXX2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("DiscordBOT");

        public static Embed SuccessEmbed(string description, string footer = "")
        {
            var successEmbed = new EmbedBuilder();

            successEmbed.WithTitle("Success!");
            successEmbed.WithColor(Color.Green);
            successEmbed.WithDescription(description);
            successEmbed.WithFooter(footer);
            successEmbed.WithCurrentTimestamp();

            return successEmbed.Build();
        }
        public static Embed Unregistered()
        {
            var successEmbed = new EmbedBuilder();

            successEmbed.WithTitle("Unregistered Command.");
            successEmbed.WithColor(Color.DarkRed);

            //successEmbed.WithDescription(description);
            successEmbed.WithFooter($"Sorry, I could not find that command.");
          //  successEmbed.WithCurrentTimestamp();

            return successEmbed.Build();
        }
        public static Embed ErrorEmbed(string description, string footer = "")
        {
            var successEmbed = new EmbedBuilder();

            successEmbed.WithTitle("Error!");
            successEmbed.WithColor(Color.Red);
            successEmbed.WithDescription(description);
            successEmbed.WithFooter(footer);
            successEmbed.WithCurrentTimestamp();

            return successEmbed.Build();
        }
    }
}