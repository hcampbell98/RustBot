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
        

        Team team = TeamUtils.GetTeam(Context.User.Id);

        if (team == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", Language.Team_Error_No_Team, Context.User)); return; }
        if (TeamUtils.CheckIfInTeam(Context.Message.MentionedUsers.First().Id)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", Language.Team_Error_Has_Team, Context.User)); return; }
        if (!TeamUtils.GetSettings(Context.Message.MentionedUsers.First().Id).InvitesEnabled) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", Language.Team_Invite_Disabled, Context.User)); return; }
        if (TeamUtils.pendingInvites.ContainsKey(Context.Message.MentionedUsers.First().Id)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Invitation", "Error", Language.Team_Invite_Pending, Context.User)); return; }

        try
        {
            await Context.Message.MentionedUsers.First().SendMessageAsync("", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You have been invited to {Context.User.Username}'s Team. Reply with r!accept to join this team.", Context.User));
        }
        catch (HttpException)
        {
            await ReplyAsync($"<@!{Context.Message.MentionedUsers.First().Id}>", false, Utilities.GetEmbedMessage("Team Invite", $"{Context.User.Username}'s Team", $"You have been invited to {Context.User.Username}'s Team. Reply with r!accept to join this team.", Context.User));
        }

        TeamUtils.pendingInvites.Add(Context.Message.MentionedUsers.First().Id, team);
    }
}