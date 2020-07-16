using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Break : ModuleBase<SocketCommandContext>
{
    string[] attackTypes = { "explosive", "melee", "guns", "throw" };

    [Command("break", RunMode = RunMode.Async)]
    [Summary("Sends structure breaking info")]
    public async Task SendBreakInfo(string type, string attackType, [Remainder]string item)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        SearchBreakable sb;
        bool attackCorrect = false;

        //If structure
        if (type == "structure" || type == "block" || type == "building" || type == "build") { sb = await Utilities.SearchForBreakable("block", item); }
        //If placeable
        else if (type == "placeable" || type == "item" || type == "door") { sb = await Utilities.SearchForBreakable("placeable", item); }
        //If anything else
        else { await ReplyAsync("Type can either be *structure* or *placeable*."); return; }

        //If the attack type is wrong, return.

        foreach(string attack in attackTypes)
        {
            if (attack == attackType) { attackCorrect = true; }
        }

        if (!attackCorrect) { await ReplyAsync("Attack type should either be *explosive*, *melee*, *throw* or *guns*."); return; }

        //If item is not found, reply with an error message and return
        if (sb == null) { await ReplyAsync("Structure/Placeable not found."); return; }

        BreakableInfo bi = await Utilities.GetBreakableInfo(sb, attackType.ToLower());


        await ReplyAsync("", false, GetEmbed(bi));
        await Utilities.StatusMessage("break", Context);
    }

    public Embed GetEmbed(BreakableInfo breakable)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithThumbnailUrl(breakable.Icon);
        eb.WithTitle($"{breakable.ItemName}");

        eb.AddField("Information", $"HP: {breakable.HP}");

        StringBuilder sb = new StringBuilder();

        foreach (AttackDurability ab in breakable.DurabilityInfo)
        {
            sb.Append($"**{ab.Tool}**\n*Quantity:* {ab.Quantity}    *Time:* {ab.Time}\n*Fuel:* {ab.Fuel.Replace("-", "0").Replace("\n", "")}    *Sulfur:* {ab.Sulfur.Replace("-", "0").Replace("\n", "")}\n");
        }

        eb.WithDescription(sb.ToString());

        return eb.Build();
    }
}