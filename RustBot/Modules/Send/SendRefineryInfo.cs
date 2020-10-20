using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;
using Discord.Addons.Interactive;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Refinery : InteractiveBase
{
    int crudeTime = 5;
    double woodPerSecond = 0.67;

    [Command("refine", RunMode = RunMode.Async)]
    [Summary("Sends small refinery info")]
    [Remarks("Tools")]
    public async Task SendRefinery(int amount)
    {
        

        if (amount > 3000) { await ReplyAsync("The max value allowed is 3000."); return; }
        if (amount < 0) { await ReplyAsync("You can't smelt nothing retard."); return; }

        double totalTime = (crudeTime * amount) / 3;
        double totalWood = totalTime * woodPerSecond;
        double oddSlot;
        double otherSlots;

        TimeSpan t = TimeSpan.FromSeconds(totalTime);

        string formattedTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);

        if ((amount / 3.0) - Math.Floor(amount / 3.0) < 0.5)
        {
            oddSlot = Math.Ceiling(amount / 3.0);
            otherSlots = Math.Floor(amount / 3.0);
        }
        else
        {
            oddSlot = Math.Floor(amount / 3.0);
            otherSlots = Math.Ceiling(amount / 3.0);
        }

        await ReplyAsync("", false, GetSmallFurnEmbed("Crude Oil", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
    }

    public Embed GetSmallFurnEmbed(string oreType, double totalTime, double totalWood, double oddSlot, double otherSlots, string formattedTime)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));;
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));

        eb.WithThumbnailUrl("https://rustlabs.com/img/items180/small.oil.refinery.png");
        eb.WithTitle($"Small Oil Refinery");
        eb.AddField(oreType, $"Time: {formattedTime}\nWood Required: {Math.Floor(totalWood)}\nCharcoal Produced: {Math.Floor(totalWood * 0.75)}");
        eb.AddField("Efficient Slot Numbers", $"{oddSlot} - {otherSlots} - {otherSlots}");

        return eb.Build();
    }
}