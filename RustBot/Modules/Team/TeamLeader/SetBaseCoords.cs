using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;
using System.IO;
using System.Text.RegularExpressions;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class BaseCoords : ModuleBase<SocketCommandContext>
{
    [Command("setcoords", RunMode = RunMode.Async)]
    [Summary("Sets the coordinates of your team's base.")]
    [Remarks("Team Leader")]
    public async Task SetBaseCoords(string coords)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //If the input is not a valid coordinate, display an error and return.
        if (!Regex.Match(coords, @"^[a-zA-Z]{1,2}\d{1,2}$").Success) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Coordinates", "Error", "The input was an invalid coordinate. Examples of valid coordinated: D12, a14, c1, B28", Context.User, Color.Red)); return; }

        //If the user isn't in a team or isn't the team leader, display an error message
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Coordinates", "Error", "You are not a member of a team. Please create one using r!createteam", Context.User, Color.Red)); return; }
        if (team.TeamLeader != Context.User.Id) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not the team leader.", Context.User, Color.Red)); return; }

        //Create a new team based on the original and update the base coordinates
        Team updatedTeam = team;
        updatedTeam.BaseCoords = coords.ToUpper();

        //Delete the team file, write the new team file, and update the teams list with the new team
        File.Delete($"Users/Teams/{team.TeamLeader}.json");
        Utilities.WriteToJsonFile<Team>($"Users/Teams/{team.TeamLeader}.json", updatedTeam);
        TeamUtils.teamData.Remove(team);
        TeamUtils.teamData.Add(updatedTeam);

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Coordinates", "Updated", $"Coordinates Updated: {updatedTeam.BaseCoords}", Context.User, Color.Red)); return;
    }
}