using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class SetGame : ModuleBase<SocketCommandContext>
{
    [Command("setgame", RunMode = RunMode.Async)]
    [Summary("Sets the bots game")]
    [Remarks("Admin")]
    public async Task SetGameAsync(string status)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.Admin) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        await Program._client.SetGameAsync(status);
    }
}