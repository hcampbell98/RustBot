using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using Discord.WebSocket;
using System.Collections.Generic;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class Leave : ModuleBase<SocketCommandContext>
{
    [Command("leaveserver", RunMode = RunMode.Async)]
    [Summary("Leaves the specified server")]
    public async Task LeaveServer(string serverID)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.Admin) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        IReadOnlyCollection<SocketGuild> guildList = Program._client.Guilds;
        SocketGuild g = guildList.FirstOrDefault(i => i.Id.ToString() == serverID);
        await g.LeaveAsync();

        await Utilities.StatusMessage("leaveserver", Context);
    }
}