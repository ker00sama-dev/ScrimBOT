using Discord;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Extensions
{
    public static class Extensions
    {
        public static async Task DeleteMessageAfterSeconds(this IMessage message, int seconds)
        {
            await Task.Delay(seconds * 1000);
            await message.DeleteAsync();
        }
    }
}
