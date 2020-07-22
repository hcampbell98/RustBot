using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class ItemStore : ModuleBase<SocketCommandContext>
{
    [Command("itemstore", RunMode = RunMode.Async)]
    [Summary("Sends the current item store")]
    public async Task SendITemStore()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        List<ItemStoreItem> itemStore = await Utilities.GetItemStore();

        if (itemStore == null) { await ReplyAsync("The item store is yet to refresh. Please wait a while and try again."); }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        eb.WithColor(Color.Red);
        eb.WithTitle($"Item Store");
        eb.WithFooter(fb);

        foreach(var item in itemStore)
        {
            eb.AddField($"{item.ItemName}", $"Price: {Regex.Replace(item.ItemPrice, @"\t|\n|\r", "")}\nLink: [{item.ItemName}]({item.ItemURL})");
        }

        sw.Stop();
        fb.WithText($"Called by {Context.Message.Author.Username} | Completed in {sw.ElapsedMilliseconds}ms");

        await ReplyAsync("", false, eb.Build());
    }
}