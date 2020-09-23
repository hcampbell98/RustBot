using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Verify : ModuleBase<SocketCommandContext>
{
    [Command("verify", RunMode = RunMode.Async)]
    [Summary("cmdSummary")]
    [Remarks("Admin")]
    public async Task VerifyPremium(string transId)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }


    }
}