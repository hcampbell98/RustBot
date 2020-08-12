using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class ServersInfo : ModuleBase<SocketCommandContext>
{
    [Command("servers", RunMode = RunMode.Async)]
    [Summary("Gets the top 5 Rust servers.")]
    [Remarks("Misc")]
    public async Task GetServers()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        var s = await Utilities.GetServers();

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Top 5 Servers");
        eb.WithColor(Color.Red);
        eb.WithFooter(fb);

        foreach (var server in s)
        {
            eb.AddField($"{server.ServerName}", $"```css\nIP/Port: {server.IP}:{server.Port}\nPlayers: {server.CurrentPlayers}/{server.MaxPlayers}\nMap: {server.Map}\nMap Size: {server.MapSize}\nLast Wipe: {DateTime.Parse(server.LastWipe)}\nRank: {server.Rank}```");
        }

        await ReplyAsync("", false, eb.Build());
    }

    [Command("server", RunMode = RunMode.Async)]
    [Summary("Gets information about a specific server. You can specify the IP address of the server or the server name. E.g r!server 127.0.0.1, r!server Generic-Server-Name")]
    [Remarks("Misc")]
    public async Task GetServer([Remainder]string search)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        var server = await Utilities.GetServer(search);

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Search");
        eb.WithColor(Color.Red);
        eb.WithFooter(fb);

        eb.AddField($"{server.ServerName}", $"```css\nIP/Port: {server.IP}:{server.Port}\nPlayers: {server.CurrentPlayers}/{server.MaxPlayers}\nMap: {server.Map}\nMap Size: {server.MapSize}\nLast Wipe: {DateTime.Parse(server.LastWipe)}\nRank: {server.Rank}```");

        await ReplyAsync("", false, eb.Build());
    }
}