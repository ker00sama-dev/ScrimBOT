using Discord;
using System.Threading.Tasks;
using VMProtect;

namespace MyCustomDiscordBot.Extensions
{
    public static class Extensions
    {
        public static Microsoft.Win32.RegistryKey XXXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("DiscordBOT");
        static string startfull;
        static string Centerfull;
        static string Endfull;
        static string startnull;
        static string Centernull;
        static string Endnull;

        public static string ToDiscordProgressBar(this int number, int size)
        {
            if (number == 0) number = 1;
            number = (int)(number / (size * 6.7));

            if (XXXXX.GetValue(@"startfull") != null || XXXXX.GetValue(@"centerfull") != null || XXXXX.GetValue(@"Endfull") != null || XXXXX.GetValue(@"startnull") != null || XXXXX.GetValue(@"centernull") != null || XXXXX.GetValue(@"endnull") != null)
            {
                //progress bar
                startfull = XXXXX.GetValue(@"startfull").ToString();
                Centerfull = XXXXX.GetValue(@"centerfull").ToString();
                Endfull = XXXXX.GetValue(@"Endfull").ToString();
                startnull = XXXXX.GetValue(@"startnull").ToString();
                Centernull = XXXXX.GetValue(@"centernull").ToString();
                Endnull = XXXXX.GetValue(@"endnull").ToString();


            }
            else
            {
                startfull = ":e_:909092978725879859";
                Centerfull = ":e_:909092978482626560";
                Endfull = ":e_:909092978906255461";
                startnull = ":e_:909092979103391785";
                Centernull = ":e_:909092979191463956";
                Endnull = ":e_:909092979208228885";
            }

            //string[] emoji = {
            //"<:e_:909092978725879859>",//Start full   //0
            //"<:e_:909092978482626560>",//Center full  //1
            //"<:e_:909092978906255461>",//End full     //2

            //"<:e_:909092979103391785>",//Start null   //3
            //"<:e_:909092979191463956>",//Center null  //4
            //"<:e_:909092979208228885>",//End null     //5
            //};
           
              string[] emoji = {
            "<"+startfull+">",//Start full   //0
            "<"+Centerfull+">",//Start full   //0
            "<"+Endfull+">",//Start full   //0

            "<"+startnull+">",//Start full   //0
            "<"+Centernull+">",//Start full   //0
            "<"+Endnull+">",//Start full   //0
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
