using MyCustomDiscordBot.MyCustomDiscordBot;
using System;
using VMProtect;
using MyCustomDiscordBot.Models;
using MyCustomDiscordBot.Services;

namespace MyCustomDiscordBot
{
   // ServerConfig kero = await DatabaseService.GetServerConfigAsync(base.Context.Guild.Id);

    //public static class Config
    //{
    //    public static string token = "OTA3NTkwMTUxOTIyOTEzMjgw.YYpZMg.1uSiJjFfnY1ijIw5boYUqB2X4ZI";
    //    public static string DBConnectionString = @"mongodb+srv://admin:01032021020aA#@botpug.g0mkk.mongodb.net/botPug?retryWrites=true&w=majority";
    //    public static ulong id = 909023733547671632;
    //    public static char Prfix = '!';
    //}
    public static class Config
    {

        public static Microsoft.Win32.RegistryKey XXXXX2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("ScrimBOT");

        //   public static string token = "OTA3NTkwMTUxOTIyOTEzMjgw.YYpZMg.1uSiJjFfnY1ijIw5boYUqB2X4ZI";
        //    public static ulong id = 909023733547671632;
        //public static char Prfix = '!';
        public static string token = (string)XXXXX2.GetValue(@"Token2");
  public static string DBConnectionString = @"mongodb+srv://admin:01032021020aA#@botpug.g0mkk.mongodb.net/botPug?retryWrites=true&w=majority";
 // public static string DBConnectionString = @"mongodb+srv://W5eytF3bVXUOkbom:W5eytF3bVXUOkbom@scrimbotvalo.s0fg5.mongodb.net/scrimbotvalo?retryWrites=true&w=majority";//Valo

     //   public static char Prfix = char.Parse(XXXXX2.GetValue(@"perfix").ToString());

    //  public static string dbname = char.Parse(XXXXX2.GetValue(@"perfix").ToString());
    }
}
