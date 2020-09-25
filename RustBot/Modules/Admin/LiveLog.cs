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
public class LiveLog : ModuleBase<SocketCommandContext>
{
    [Command("livelog", RunMode = RunMode.Async)]
    [Summary("Live logs guild messages.")]
    [Remarks("Admin")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    public async Task ConfigLiveLog(string guildID, string outputChannel)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.Admin) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        LoggingUtils.livelogGuild = Program._client.GetGuild(Convert.ToUInt64(guildID));
        LoggingUtils.livelogOutput = Context.Guild.GetTextChannel(Convert.ToUInt64(Utilities.GetNumbers(outputChannel)));

        await ReplyAsync($"Logging Guild **{LoggingUtils.livelogGuild.Name}** in Channel **{LoggingUtils.livelogOutput.Name}**");
    }
}