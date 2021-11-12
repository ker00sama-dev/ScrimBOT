using Discord;
using System.Threading.Tasks;

namespace MyCustomDiscordBot.Extensions
{
    public static class Extensions
    {
        public static string ToDiscordProgressBar(this int number, int size)
        {
            if (number == 0) number = 1; number = number / (size / 2);

            string[] emoji = {
            "<:e_:908029764768497684>",//Start full   //0
            "<:e_:908029764546203718>",//Center full  //1
            "<:e_:908029764873383966>",//End full     //2

            "<:e_:908029764969844746>",//Start null   //3
            "<:e_:908029765078888518>",//Center null  //4
            "<:e_:908029765305393152>",//End null     //5
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
