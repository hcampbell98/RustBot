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
public class TeamMembers : ModuleBase<SocketCommandContext>
{
    [Command("members", RunMode = RunMode.Async)]
    [Summary("Lists all the members in your current team.")]
    [Remarks("Team")]
    public async Task SendMembers()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Team team = TeamUtils.GetTeam(Context.User.Id);

        //If the user isn't in a team, display an error message
        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", "You are not a member of a team. Please create one using r!createteam", Context.User, Color.Red)); return; }

        StringBuilder sb = new StringBuilder();

        foreach(ulong member in team.Members)
        {
            sb.Append($"{Program._client.GetUser(member).Username}\n");
        }

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team List", $"{Program._client.GetUser(team.TeamLeader).Username}'s Team", $"{sb.ToString()}", Context.User, Color.Red));
    }
}