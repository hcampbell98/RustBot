using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
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
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //If the user isn't in a team or isn't the team leader, display an error message
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not a member of a team. Please create one using r!createteam", Context.User, Color.Red)); return; }
        if (team.TeamLeader != Context.User.Id) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not the team leader.", Context.User, Color.Red)); return; }
        if (!team.Notifications) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "Notifications are disabled.", Context.User, Color.Red)); return; }

        foreach (ulong u in team.Members)
        {
            try
            {
                //If the user has notifications disabled, the message won't be sent
                if(TeamUtils.GetSettings(u).NotificationsEnabled)
                    await Program._client.GetUser(u).SendMessageAsync($"<@!{u}>", false, Utilities.GetEmbedMessage("Team Notifications", "Team Alert", $"{message}", Context.User, Color.Red));
            }
            catch (HttpException)
            {

            }
        }
    }
}