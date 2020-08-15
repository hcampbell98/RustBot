using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using RustBot.Aliases;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class ReloadLists : ModuleBase<SocketCommandContext>
{
    [Command("reloadlists", RunMode = RunMode.Async)]
    [Summary("Reloads all lists")]
    [Remarks("Admin")]
    public async Task Reload()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.Admin) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        AliasManager.itemAliases = Utilities.FillList<ItemAlias>("Aliases/itemAliases.json");

    }
}