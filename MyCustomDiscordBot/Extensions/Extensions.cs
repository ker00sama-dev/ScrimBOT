using Discord;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Extensions
{
    public static class Extensions
    {
        public static string ToDiscordProgressBar(this int number, int size) 
        {
            if (number == 0) number = 1;
            number = (int)(number / (size * 6.7));

            string[] emoji = {
            "<:e_:909092978725879859>",//Start full   //0
            "<:e_:909092978482626560>",//Center full  //1
            "<:e_:909092978906255461>",//End full     //2

            "<:e_:909092979103391785>",//Start null   //3
            "<:e_:909092979191463956>",//Center null  //4
            "<:e_:909092979208228885>",//End null     //5
            };

            string fillbar = (number > 0) ? emoji[0] : emoji[3];

            for (int i = 2; i < size; i++) fillbar += number >= i ? emoji[1] : emoji[4];

            fillbar += (number > size - 1) ? emoji[2] : emoji[5];

            return fillbar;
        }
        public static async Task DeleteMessageAfterSeconds(this IMessage message, int seconds)
        {
            await Task.Delay(seconds * 1000);
            await message.DeleteAsync();
        }
    }
}
