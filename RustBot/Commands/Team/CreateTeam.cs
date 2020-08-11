using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using System.Collections.Generic;
using Discord.Addons.Interactive;
using Discord;
using RustBot.Users.Teams;
using System.Net.Sockets;
using Discord.WebSocket;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class CreateTeam : InteractiveBase
{
    [Command("createteam", RunMode = RunMode.Async)]
    [Summary("Creates a team with the mentioned users")]
    [Remarks("Team Leader")]
    public async Task SendRoll([Remainder]string mentions)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        //Gets the list of mentioned users, obtains their id's and adds them to an array.
        var mentionedUsers = Context.Message.MentionedUsers;
        List<ulong> members = new List<ulong> { };
        bool notifications;

        foreach(var mention in mentionedUsers)
        {
            if (TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == mention.Id) == default(UserSettings) || TeamUtils.userSettings.FirstOrDefault(x => x.DiscordID == mention.Id).InvitesEnabled) { members.Add(mention.Id); }
        }

        ulong[] teamMembers = members.ToArray();

        //Asks the user whether or not to enable notifications
        await ReplyAndDeleteAsync("", false, Utilities.GetEmbedMessage("Team Creation", "Notifications", "Would you like to enable notifications for this team?\n\n1. Yes\n2. No\n\n**Please type the number of your answer.**", Context.User, Color.Red));
        var notifResponse = await NextMessageAsync();

        if (notifResponse.Content == "1") { notifications = true; }
        else if (notifResponse.Content == "2") { notifications = false; }
        else { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Creation", "Error", "Please type the number of your answer. E.g: 1 for yes, 2 for no.", Context.User, Color.Red)); return; }

        //Creates the team role
        IRole teamRole = await Context.Guild.CreateRoleAsync($"{Context.User.Username}'s Team", null, null, false, null);

        //If any members are in a team already, display an error message and delete the team role.
        if (!TeamUtils.CreateTeam(Context.User.Id, teamMembers, teamRole, Context.Guild, notifications)) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Creation", "Unsuccessful", "Team creation failed. You or one of your team members may already be apart of a team. Please leave your current team to continue.", Context.User, Color.Red)); await Context.Guild.GetRole(teamRole.Id).DeleteAsync(); return; }

        //Assign the team role to every member
        foreach (SocketUser u in mentionedUsers)
        {
            await (u as IGuildUser).AddRoleAsync(teamRole);
        }
        await (Context.User as IGuildUser).AddRoleAsync(teamRole);

        //Displays the success message
        await ReplyAsync("", false, Utilities.GetEmbedMessage("Team Creation", "Success", "Team successfully created.", Context.User, Color.Red));
    }
}