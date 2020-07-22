using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Coinflip : ModuleBase<SocketCommandContext>
{
    [Command("coinflip", RunMode = RunMode.Async)]
    [Summary("Coinflips a mentioned user.")]
    public async Task SendRoll(string opponent)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Random rnd = new Random();

        string oppID = Utilities.GetNumbers(opponent);

        //Checks if the user was mentioned correctly. If not, displays an error message and returns.
        if (Context.Guild.Users.FirstOrDefault(user => user.Id.ToString() == oppID) == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Coinflip", "Error", "You need to @mention your opponent.", Context.Message.Author, Color.Blue)); return; }

        string[] players = { Context.Message.Author.Id.ToString(), oppID };

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Coinflip", "Outcome", $"<@!{players[rnd.Next(0,2)]}> wins!", Context.Message.Author, Color.Blue));
    }
}