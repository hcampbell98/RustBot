using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;
using System.IO;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class UpdateNotifications : ModuleBase<SocketCommandContext>
{
    [Command("notifications", RunMode = RunMode.Async)]
    [Summary("Opts in/out of team notifications")]
    [Remarks("Team")]
    public async Task ToggleNotifications()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        bool status;

        if (TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == Context.User.Id) != null && TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == Context.User.Id).NotificationsEnabled) { TeamUtils.UpdateSettings(Context.User.Id, false); status = false; }
        else { TeamUtils.UpdateSettings(Context.User.Id, true); status = true; }

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Notifications", "Updated", $" Notifications Enabled: {status.ToString()}", Context.User, Color.Red)); return;
    }
}