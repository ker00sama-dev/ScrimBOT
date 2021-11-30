using Discord;
using System.Threading.Tasks;
using VMProtect;
using MyCustomDiscordBot.Models;

namespace MyCustomDiscordBot.Extensions
{
    public static class Extensions
    {
        public static Microsoft.Win32.RegistryKey XXXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Crossfire");

        public static string ToDiscordProgressBar(this int number, int size,ServerConfig config)
        {
            if (number == 0) number = 1;
            number = (int)(number / (size * 6.7));
         
            //config.Startfull = startnull;
            //config.Centerfull = Centerfull;
            //config.Endfull = Endfull;
            //config.Startnull = Startnull;
            //config.Centernull = Centernull;
            //config.Endnull = Endnull;

            //string[] emoji = {
            //"<:e_:909092978725879859>",//Start full   //0
            //"<:e_:909092978482626560>",//Center full  //1
            //"<:e_:909092978906255461>",//End full     //2

            //"<:e_:909092979103391785>",//Start null   //3
            //"<:e_:909092979191463956>",//Center null  //4
            //"<:e_:909092979208228885>",//End null     //5
            //};

            string[] emoji = {
            "<:e_:"+config.Startfull.ToString()+">",//Start full   //0
            "<:e_:"+config.Centerfull.ToString()+">",//Start full   //0
            "<:e_:"+config.Endfull.ToString()+">",//Start full   //0

            "<:e_:"+config.Startnull.ToString()+">",//Start full   //0
            "<:e_:"+config.Centernull.ToString()+">",//Start full   //0
            "<:e_:"+config.Endnull.ToString()+">",//Start full   //0
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
