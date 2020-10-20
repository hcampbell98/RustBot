using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;
using RustBot.Users.Guilds;
using System.Collections.Generic;
using RustBot.Logging;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class ServersInfo : ModuleBase<SocketCommandContext>
{
    [Command("servers", RunMode = RunMode.Async)]
    [Summary("Gets the top 5 Rust servers.")]
    [Remarks("Misc")]
    public async Task GetServers()
    {
        
        if (!GuildUtils.GetSettings(Context.Guild.Id).ServerSearch) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Rust Servers", "Error", Language.ServerInfo_Disabled, Context.User)); return; }

        var s = await Utilities.GetServers();

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Top 5 Servers");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        foreach (var ad in Utilities.adServers)
        {
            if (ad.Value < DateTime.Now)
            {
                Utilities.adServers.Remove(ad.Key);
            }
        }

        foreach (var server in s)
        {
            eb.AddField($"{server.ServerName}", $"```css\nIP/Port: {server.IP}:{server.Port}\nPlayers: {server.CurrentPlayers}/{server.MaxPlayers}\nMap: {server.Map}\nMap Size: {server.MapSize}\nLast Wipe: {server.LastWipe}\nRank: {server.Rank}```");
        }

        if (Utilities.adServers.Count == 0)
        {
            //If there are no ads
            eb.AddField("Ad Spot", $"This is an Ad spot for server owners to promote their servers to over {Statistics.GetTotalUsers()} users. For information on how to display your server here, contact Bunny#9220.");
        }
        else
        {
            //If there are ads available
            Random rnd = new Random();
            rnd.Next(0, Utilities.adServers.Count);

            var serverIp = RandomValues(Utilities.adServers).First();
            var server = await Utilities.GetServer(serverIp);

            eb.AddField($"Ad - {server.ServerName}", $"```css\nIP/Port: {server.IP}:{server.Port}\nPlayers: {server.CurrentPlayers}/{server.MaxPlayers}\nMap: {server.Map}\nMap Size: {server.MapSize}\nLast Wipe: {server.LastWipe}\nRank: {server.Rank}```");
        }

        await ReplyAsync("", false, eb.Build());
    }

    [Command("server", RunMode = RunMode.Async)]
    [Summary("Gets information about a specific server. You can specify the IP address of the server or the server name. E.g r!server 127.0.0.1, r!server Generic-Server-Name")]
    [Remarks("Misc")]
    public async Task GetServer([Remainder]string search)
    {
        
        if (!GuildUtils.GetSettings(Context.Guild.Id).ServerSearch) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Rust Servers", "Error", Language.ServerInfo_Disabled, Context.User)); return; }

        var server = await Utilities.GetServer(search);

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Search");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        eb.AddField($"{server.ServerName}", $"```css\nIP/Port: {server.IP}:{server.Port}\nPlayers: {server.CurrentPlayers}/{server.MaxPlayers}\nMap: {server.Map}\nMap Size: {server.MapSize}\nLast Wipe: {DateTime.Parse(server.LastWipe)}\nRank: {server.Rank}```");

        await ReplyAsync("", false, eb.Build());
    }

    [Command("createad", RunMode = RunMode.Async)]
    [Summary("Adds a server to the ad list via the IP specified. timeSpan is in hours")]
    [Remarks("Admin")]
    [RequireOwner]
    public async Task AddServerAd(string search, int timeSpan = 1)
    {
        //timeSpan is in hours

        if (!GuildUtils.GetSettings(Context.Guild.Id).ServerSearch) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Rust Servers", "Error", Language.ServerInfo_Disabled, Context.User)); return; }

        var server = await Utilities.GetServer(search);

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Search");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        eb.AddField("Rust Servers", Language.ServerInfo_AddedAd);

        Utilities.adServers.Add(search, DateTime.Now.AddHours(timeSpan));
        await ReplyAsync("", false, eb.Build());
    }

    [Command("removead", RunMode = RunMode.Async)]
    [Summary("Adds a server to the ad list via the IP specified. timeSpan is in hours")]
    [Remarks("Admin")]
    [RequireOwner]
    public async Task RemoveServerAd(string ip)
    {
        if (!GuildUtils.GetSettings(Context.Guild.Id).ServerSearch) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Rust Servers", "Error", Language.ServerInfo_Disabled, Context.User)); return; }

        Utilities.adServers.Remove(ip);

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Search");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        eb.AddField("Rust Servers", Language.ServerInfo_RemovedAd);

        await ReplyAsync("", false, eb.Build());
    }

    public IEnumerable<TKey> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        Random rand = new Random();
        List<TKey> values = Enumerable.ToList(dict.Keys);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }
}