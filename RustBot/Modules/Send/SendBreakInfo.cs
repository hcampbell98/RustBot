using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using Discord;
using Discord.Addons.Interactive;
using System.Linq;
using System.Text;
using System.Diagnostics;
using RustBot.Aliases;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Break : InteractiveBase
{
    Stopwatch sw = new Stopwatch();

    [Command("break", RunMode = RunMode.Async)]
    [Summary("Sends structure breaking info. [placeable] is the object you're trying to break, such as \"Stone Wall\" or \"Sheet Metal Door\".")]
    [Remarks("Tools")]
    public async Task SendBreakInfo([Remainder] string placeable)
    {
        sw.Start();
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        SearchBreakable sb;

        if (AliasManager.GetItemName(placeable) != null) { placeable = AliasManager.GetItemName(placeable); }

        //-----------------------Structure Type------------------------
        sw.Stop();
        var sInfoMsg = await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Structure Type", "1. Building Block\n2. Placeable/Door/Window\n\n**Please type the number of the structure type.**", Context.Message.Author, Color.Red));
        var response = await NextMessageAsync();
        sw.Start();
        //If structure
        if (response.Content == "1") { sb = await Utilities.SearchForBreakable("block", placeable.ToLower()); }
        //If placeable
        else if (response.Content == "2") { sb = await Utilities.SearchForBreakable("placeable", placeable.ToLower()); }
        //If anything else
        else { await ReplyAsync("Please type the number of the structure type. Run the command again."); return; }

        sw.Stop();

        //-----------------------Attack Type------------------------
        var aInfoMsg = await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Attack Type", "1. Explosive\n2. Melee\n3. Guns\n4. Throw\n\n**Please type the number of the attack type.**", Context.Message.Author, Color.Red));
        var attackType = await NextMessageAsync();
        sw.Start();

        string attack;

        if (attackType.Content == "1") { attack = "explosive"; }
        else if (attackType.Content == "2") { attack = "melee"; }
        else if (attackType.Content == "3") { attack = "guns"; }
        else if (attackType.Content == "4") { attack = "throw"; }
        else { await ReplyAsync("Please type the number of the attack type. Run the command again."); return; }

        //-----------------------Side Type------------------------
        string side = null;
        if (response.Content == "1")
        {
            var sideMessage = await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Block Side", "1. Hard\n2. Soft\n\n**Please type the number of the attack type.**", Context.Message.Author, Color.Red));
            var sideType = await NextMessageAsync();
            sw.Start();

            if (attackType.Content == "1") { side = "hard"; }
            else if (attackType.Content == "2") { side = "soft"; }
            else { await ReplyAsync("Please type the number of the block side. Run the command again."); return; }
        }

        BreakableInfo bi;

        //If item is not found, reply with an error message and return
        if (sb == null) { await ReplyAsync("Structure/Placeable not found."); return; }

        //If the item is in the cache, don't search for it.
        if (Utilities.breakCache.ContainsKey(sb.ItemName)) { bi = Utilities.breakCache[sb.ItemName]; }
        else { bi = await Utilities.GetBreakableInfo(sb, attack, side); }

        //If item isn't placeable
        if (bi == null) { await ReplyAsync("Structure/Placeable not found."); return; }

        //Removes the attack info message
        await aInfoMsg.DeleteAsync();
        //Removes the structure info message
        await sInfoMsg.DeleteAsync();

        try
        {
            await ReplyAsync("", false, GetEmbed(bi));
        }
        catch (Exception)
        {
            await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Error", $"There are too many results to fit into a Discord Embed, please click [here]({bi.URL}) to see your results.", Context.Message.Author, Color.Red));
        }

    }

    public Embed GetEmbed(BreakableInfo breakable)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        sw.Stop();
        fb.WithText($"Called by {Context.Message.Author.Username} | Completed in {sw.ElapsedMilliseconds}ms");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        eb.WithColor(Color.Red);
        eb.WithThumbnailUrl(breakable.Icon);
        eb.WithTitle($"{breakable.ItemName}");
        eb.WithFooter(fb);

        eb.AddField("Information", $"HP: {breakable.HP}", true);

        StringBuilder sb = new StringBuilder();

        //List<AttackDurability> sortedList = breakable.DurabilityInfo.OrderBy(x => Convert.ToInt32(Utilities.GetNumbers(x.Sulfur))).ToList();

        foreach (AttackDurability ab in breakable.DurabilityInfo)
        {
            sb.Append($"```css\n//{ab.Tool}\\\\ \nQuantity: {ab.Quantity} Time: {ab.Time}s\n");
            if (ab.Fuel != "-") { sb.Append($"Fuel: {Utilities.GetNumbers(ab.Fuel)} "); }
            if (ab.Sulfur != "-") { sb.Append($"Sulfur: {Utilities.GetNumbers(ab.Sulfur)}"); }
            sb.Append("```");
        }

        eb.WithDescription(sb.ToString());

        return eb.Build();
    }
}