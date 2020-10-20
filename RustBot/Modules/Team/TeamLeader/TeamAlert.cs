using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;
using Discord.WebSocket;
using Discord.Net;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class TeamAlerter : ModuleBase<SocketCommandContext>
{
    [Command("teamalert", RunMode = RunMode.Async)]
    [Summary("Sends a message to each team member.")]
    [Remarks("Team Leader")]
    public async Task TeamAlert([Remainder]string message)
    {
        

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //If the user isn't in a team or isn't the team leader, display an error message
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_No_Team, Context.User)); return; }
        if (team.TeamLeader != Context.User.Id) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_Not_Leader, Context.User)); return; }
        if (!team.Notifications) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_Notifications_Disabled, Context.User)); return; }

        foreach (ulong u in team.Members)
        {
            try
            {
                //If the user has notifications disabled, the message won't be sent
                if(TeamUtils.GetSettings(u).NotificationsEnabled)
                    await Program._client.GetUser(u).SendMessageAsync($"<@!{u}>", false, Utilities.GetEmbedMessage("Team Notifications", "Team Alert", $"{message}", Context.User));
            }
            catch (HttpException)
            {

            }
        }
    }
}