using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using Discord;
using RustBot.Users.Guilds;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class DisableServerSearch : ModuleBase<SocketCommandContext>
{
    [Command("togglesearch", RunMode = RunMode.Async)]
    [Summary("Used by server owners/admins to toggle on/off the ability to search for servers.")]
    [Remarks("Guild")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task ToggleSearch()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        GuildUtils.UpdateSearch(Context.Guild.Id, GuildUtils.GetSettings(Context.Guild.Id).ServerSearch ^= true);
        await ReplyAsync("", false, Utilities.GetEmbedMessage("Toggle Search", "Response", $"Server Search Enabled: {GuildUtils.GetSettings(Context.Guild.Id).ServerSearch}", Context.User, Color.Red));
    }
}