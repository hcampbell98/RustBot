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
public class Decline : ModuleBase<SocketCommandContext>
{
    [Command("decline", RunMode = RunMode.Async)]
    [Summary("Declines a pending team invite.")]
    [Remarks("Team")]
    public async Task DeclineInvite()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        if (TeamUtils.pendingInvites.ContainsKey(Context.User.Id))
        {
            await ReplyAsync($"<@!{Context.Message.MentionedUsers.First().Id}>", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You declined the team invite.", Context.User, Color.Red));
            TeamUtils.pendingInvites.Remove(Context.User.Id);
        }
        else
        {
            await ReplyAsync($"<@!{Context.Message.MentionedUsers.First().Id}>", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You have no pending invites.", Context.User, Color.Red));
        }
    }
}