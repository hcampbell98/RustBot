using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;
using System.IO;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class UpdateTeamNotifications : ModuleBase<SocketCommandContext>
{
    [Command("teamnotifications", RunMode = RunMode.Async)]
    [Summary("Toggles a teams notifications on/off.")]
    [Remarks("Team Leader")]
    public async Task ToggleNotifications()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //If the user isn't in a team or isn't the team leader, display an error message
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not a member of a team. Please create one using r!createteam", Context.User, Color.Red)); return; }
        if (team.TeamLeader != Context.User.Id) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not the team leader.", Context.User, Color.Red)); return; }

        //Create a new team based on the original and update the notification status
        Team updatedTeam = team;
        if (team.Notifications) { updatedTeam.Notifications = false; }
        else { updatedTeam.Notifications = true; }

        //Delete the team file, write the new team file, and update the teams list with the new team
        File.Delete($"Users/Teams/{team.TeamLeader}.json");
        Utilities.WriteToJsonFile<Team>($"Users/Teams/{team.TeamLeader}.json", updatedTeam);
        TeamUtils.teamData.Remove(team);
        TeamUtils.teamData.Add(updatedTeam);

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Updated", $"Team Notifications Enabled: {updatedTeam.Notifications.ToString()}", Context.User, Color.Red)); return;
    }
}