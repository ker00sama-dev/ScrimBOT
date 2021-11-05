namespace MyCustomDiscordBot.Models
{
    public class MapRecord
    {
        public string MapName { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public MapRecord(string mapName)
        {
            MapName = mapName;
            Wins = 0;
            Losses = 0;
        }
    }
}
