using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class ToggleInvite : ModuleBase<SocketCommandContext>
{
    [Command("toggleinvites", RunMode = RunMode.Async)]
    [Summary("Toggles on/off a users ability to be invited to a team.")]
    [Remarks("Team")]
    public async Task Toggle()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        bool status;

        if (TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == Context.User.Id) != null && TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == Context.User.Id).InvitesEnabled) { TeamUtils.UpdateSettings(Context.User.Id, invites: false); status = false; }
        else { TeamUtils.UpdateSettings(Context.User.Id, invites: true); status = true; }

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invites", "Updated", $"Team Invites Enabled: {status.ToString()}", Context.User, Color.Red)); return;
    }
}