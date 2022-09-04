namespace MyCustomDiscordBot
{

    public static class Config
    {
        public static Microsoft.Win32.RegistryKey XXXXX2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("ScrimBOT");
        public static string token = (string)XXXXX2.GetValue(@"Token2");
        //  public static string DBConnectionString = @"mongodb+srv://admin:ScrimBot@scrimbot.e9kjvmt.mongodb.net/?retryWrites=true&w=majority";
        public static string DBConnectionString = (string)XXXXX2.GetValue(@"DBConnectionString");

    }
}
