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
public class LeaveTeam : ModuleBase<SocketCommandContext>
{
    [Command("leaveteam", RunMode = RunMode.Async)]
    [Summary("Leaves the team you are currently in.")]
    [Remarks("Team")]
    public async Task LeaveCurrentTeam()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        if (!TeamUtils.LeaveTeam(Context.User.Id)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Error", Language.Team_Error_No_Team, Context.User)); return; }
        
        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Notifications", "Left Team", Language.Team_Leave, Context.User));
    }
}