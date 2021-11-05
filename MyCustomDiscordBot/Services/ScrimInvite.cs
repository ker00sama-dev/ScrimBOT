using Discord.WebSocket;
using MyCustomDiscordBot.Models;

namespace MyCustomDiscordBot.Services
{
    public class ScrimInvite
    {
        public ulong GuildId;

        public Team InvitingTeam;

        public SocketUser InvitingTeamCaptain;

        public string Description;

        public ScrimInvite(ulong guildId, string description, SocketUser invitingTeamCaptain, Team inviter)
        {
            GuildId = guildId;
            Description = description;
            InvitingTeam = inviter;
            InvitingTeamCaptain = invitingTeamCaptain;
        }
    }
}
