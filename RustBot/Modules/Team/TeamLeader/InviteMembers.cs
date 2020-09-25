using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Teams;
using Discord;
using Discord.Net;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class InviteMembers : ModuleBase<SocketCommandContext>
{
    [Command("teaminvite", RunMode = RunMode.Async)]
    [Summary("Invites a mentioned user to your team.")]
    [Remarks("Team Leader")]
    public async Task TeamInvite([Remainder]string mention)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Team team = TeamUtils.GetTeam(Context.User.Id);

        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", "You are not a member of a team. Please create one using r!createteam", Context.User, Color.Red)); return; }
        if (TeamUtils.CheckIfInTeam(Context.Message.MentionedUsers.First().Id)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", "This person is already a member of a team. If they wish to leave, they can run r!leaveteam", Context.User, Color.Red)); return; }
        if (!TeamUtils.GetSettings(Context.Message.MentionedUsers.First().Id).InvitesEnabled) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", "This user has team invites disabled. They can re-enable them by running r!toggleinvites", Context.User, Color.Red)); return; }
        if (TeamUtils.pendingInvites.ContainsKey(Context.Message.MentionedUsers.First().Id)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", "This user already has a pending team invite. They can decline this by running r!decline", Context.User, Color.Red)); return; }

        try
        {
            await Context.Message.MentionedUsers.First().SendMessageAsync("", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You have been invited to {Context.User.Username}'s Team. Reply with r!accept to join this team.", Context.User, Color.Red));
        }
        catch (HttpException)
        {
            await ReplyAsync($"<@!{Context.Message.MentionedUsers.First().Id}>", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You have been invited to {Context.User.Username}'s Team. Reply with r!accept to join this team.", Context.User, Color.Red));
        }

        TeamUtils.pendingInvites.Add(Context.Message.MentionedUsers.First().Id, team);
    }
}