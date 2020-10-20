using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using System.Text;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Raid : ModuleBase<SocketCommandContext>
{
    [Command("raid", RunMode = RunMode.Async)]
    [Summary("Notifies all members of the current team that a raid is in progress.")]
    [Remarks("Team")]
    public async Task RaidNotifier()
    {
        

        Team team = TeamUtils.GetTeam(Context.User.Id);

        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_No_Team, Context.User)); return; }
        if (!team.Notifications) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_Notifications_Disabled, Context.User)); return; }
        if (Context.Guild != Program._client.GetGuild(team.GuildID)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_Wrong_Guild, Context.User)); return; }

        StringBuilder mentions = new StringBuilder();
        foreach(ulong member in team.Members)
        {
            mentions.Append($"<@!{member}>, ");
        }

        await ReplyAsync($"<@!{team.TeamLeader}>, {mentions.ToString()}", false, Utilities.GetEmbedMessage("Team Notifications", "Raid", $"Attention, we are currently being raided. Base Coords: {team.BaseCoords}, Server IP: {team.Server.IP}:{team.Server.Port}", Context.User));
    }
}