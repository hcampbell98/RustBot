using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord.WebSocket;
using System.Collections.Generic;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class Purge : ModuleBase<SocketCommandContext>
{
    [Command("purge", RunMode = RunMode.Async)]
    [Summary("Deletes messages.")]
    [Remarks("Admin")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task PurgeChannel(int toDelete = -1)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        if (toDelete == -1) { await ReplyAsync("Specify the number of messages to purge. !purge [toDelete]"); }

        var messages = await Context.Channel.GetMessagesAsync(toDelete).FlattenAsync();
        await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

    }
}