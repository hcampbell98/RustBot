using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord.Addons.Interactive;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Module : InteractiveBase
{
    [Command("teamserver", RunMode = RunMode.Async)]
    [Summary("Sets the server of the team. You can specify the IP address of the server or the server name. E.g r!server 127.0.0.1, r!server Generic-Server-Name")]
    [Remarks("Team Leader")]
    public async Task SendRoll([Remainder]string search)
    {
        

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //Checks whether the player is in a team, and whether or not they own it
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_No_Team, Context.User)); return; }
        if (team.TeamLeader != Context.User.Id) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_Not_Leader, Context.User)); return; }

        //Grabs the server
        ServerInfo s = await Utilities.GetServer(search);
        bool correct;

        //Asks whether or not the server is correct
        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Server", $"Add **{s.ServerName}** as this team's server?", "1. Yes\n2. No\n\n**Please type the number of your answer.**", Context.User));
        var response = await NextMessageAsync();

        if (response.Content == "1") { correct = true; }
        else if (response.Content == "2") { correct = false; }
        else { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Server", "Error", Language.Team_Creation_Error_Invalid, Context.User)); return; }

        //If correct, add team and display success message, else display unsuccessful message
        if (correct) { TeamUtils.SetServer(team, s); await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Server", "Success", $"Successfully added **{s.ServerName}** as this team's server.", Context.User)); }
        else { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Server", "Unsuccessful", Language.Team_Server_Error_Broad, Context.User)); }
    }
}