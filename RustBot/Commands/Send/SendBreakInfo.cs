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
    [Summary("Sends structure breaking info.")]
    public async Task SendBreakInfo([Remainder]string item)
    {
        sw.Start();
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        SearchBreakable sb;

        if (AliasManager.GetItemName(item) != null) { item = AliasManager.GetItemName(item); }

        sw.Stop();
        var sInfoMsg = await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Structure Type", "1. Building Block\n2. Placeable/Door/Window\n\n**Please type the number of the structure type.**", Context.Message.Author, Color.Red));
        var response = await NextMessageAsync();
        sw.Start();
        //If structure
        if (response.Content == "1") { sb = await Utilities.SearchForBreakable("block", item.ToLower()); }
        //If placeable
        else if (response.Content == "2") { sb = await Utilities.SearchForBreakable("placeable", item.ToLower()); }
        //If anything else
        else { await ReplyAsync("Please type the number of the structure type. Run the command again."); return; }

        sw.Stop();
        //Asks for the attack type
        var aInfoMsg = await ReplyAsync("", false, Utilities.GetEmbedMessage("Break Info", "Attack Type", "1. Explosive\n2. Melee\n3. Guns\n4. Throw\n\n**Please type the number of the attack type.**", Context.Message.Author, Color.Red));
        var attackType = await NextMessageAsync();
        sw.Start();

        string attack;

        if (attackType.Content == "1") { attack = "explosive"; }
        else if (attackType.Content == "2") { attack = "melee"; }
        else if (attackType.Content == "3") { attack = "guns"; }
        else if (attackType.Content == "4") { attack = "throw"; }
        else { await ReplyAsync("Please type the number of the attack type. Run the command again."); return; }

        //If item is not found, reply with an error message and return
        if (sb == null) { await ReplyAsync("Structure/Placeable not found."); return; }

        BreakableInfo bi = await Utilities.GetBreakableInfo(sb, attack);

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

        eb.AddField("Information", $"HP: {breakable.HP}");

        StringBuilder sb = new StringBuilder();

        //List<AttackDurability> sortedList = breakable.DurabilityInfo.OrderBy(x => Convert.ToInt32(Utilities.GetNumbers(x.Sulfur))).ToList();

        foreach (AttackDurability ab in breakable.DurabilityInfo)
        {
            sb.Append($"//{ab.Tool}\\\\ \nQuantity: {ab.Quantity} Time: {ab.Time}s\nFuel: {Utilities.GetNumbers(ab.Fuel)} Sulfur: {Utilities.GetNumbers(ab.Sulfur)}\n\n");
        }

        eb.WithDescription("```css\n" + sb.ToString() + "```");

        return eb.Build();
    }
}