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
public class Smelting : InteractiveBase
{
    int sulfurTime = 5;
    int metalTime = 10;
    int highQualTime = 20;
    double woodPerSecond = 0.5;

    string[] sulfurAliases = { "sulfur", "sulfur ore", "sulf", "sulphur", "s" };
    string[] metalAliases = { "metal", "metal ore", "m", "frags", "metal frags" };
    string[] highQualAliases = { "hqm", "hqm ore", "high qual", "hq", "high qual metal", "high quality", "high quality metal" };

    [Command("furn", RunMode = RunMode.Async)]
    [Summary("Sends small furnace info")]
    [Remarks("Tools")]
    public async Task SendFurnace(string oreType, int amount)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        if (amount > 3000) { await ReplyAsync("The max value allowed is 3000."); return; }
        if (amount < 0) { await ReplyAsync("You can't smelt nothing retard."); return; }


        //Sulfur
        foreach(string alias in sulfurAliases)
        {
            if(oreType.ToLower() == alias)
            {
                double totalTime = (sulfurTime * amount) / 3;
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

                await ReplyAsync("", false, GetSmallFurnEmbed("Sulfur Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }

        //Metal
        foreach (string alias in metalAliases)
        {
            if (oreType.ToLower() == alias)
            {
                double totalTime = (metalTime * amount) / 3;
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

                await ReplyAsync("", false, GetSmallFurnEmbed("Metal Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }

        //High Quality Metal
        foreach (string alias in highQualAliases)
        {
            if (oreType.ToLower() == alias)
            {
                double totalTime = (highQualTime * amount) / 3;
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

                await ReplyAsync("", false, GetSmallFurnEmbed("High Quality Metal Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }
    }

    [Command("furnl", RunMode = RunMode.Async)]
    [Summary("Sends large furnace info")]
    [Remarks("Tools")]
    public async Task SendLFurnace(string oreType, int amount)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        if (amount > 15000) { await ReplyAsync("The max value allowed is 15,000."); return; }
        if (amount < 0) { await ReplyAsync("You can't smelt nothing retard."); return; }


        //Sulfur
        foreach (string alias in sulfurAliases)
        {
            if (oreType.ToLower() == alias)
            {
                double totalTime = (sulfurTime * amount) / 15;
                double totalWood = totalTime * woodPerSecond;
                double oddSlot;
                double otherSlots;

                TimeSpan t = TimeSpan.FromSeconds(totalTime);

                string formattedTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);

                if ((amount / 15.0) - Math.Floor(amount / 15.0) < 0.5)
                {
                    oddSlot = Math.Ceiling(amount / 15.0);
                    otherSlots = Math.Floor(amount / 15.0);
                }
                else
                {
                    oddSlot = Math.Floor(amount / 15.0);
                    otherSlots = Math.Ceiling(amount / 15.0);
                }

                await ReplyAsync("", false, GetLargeFurnEmbed("Sulfur Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }

        //Metal
        foreach (string alias in metalAliases)
        {
            if (oreType.ToLower() == alias)
            {
                double totalTime = (metalTime * amount) / 15;
                double totalWood = totalTime * woodPerSecond;
                double oddSlot;
                double otherSlots;

                TimeSpan t = TimeSpan.FromSeconds(totalTime);

                string formattedTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);

                if ((amount / 15.0) - Math.Floor(amount / 15.0) < 0.5)
                {
                    oddSlot = Math.Ceiling(amount / 15.0);
                    otherSlots = Math.Floor(amount / 15.0);
                }
                else
                {
                    oddSlot = Math.Floor(amount / 15.0);
                    otherSlots = Math.Ceiling(amount / 15.0);
                }

                await ReplyAsync("", false, GetLargeFurnEmbed("Metal Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }

        //High Quality Metal
        foreach (string alias in highQualAliases)
        {
            if (oreType.ToLower() == alias)
            {
                double totalTime = (highQualTime * amount) / 15;
                double totalWood = totalTime * woodPerSecond;
                double oddSlot;
                double otherSlots;

                TimeSpan t = TimeSpan.FromSeconds(totalTime);

                string formattedTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);

                if ((amount / 15.0) - Math.Floor(amount / 15.0) < 0.5)
                {
                    oddSlot = Math.Ceiling(amount / 15.0);
                    otherSlots = Math.Floor(amount / 15.0);
                }
                else
                {
                    oddSlot = Math.Floor(amount / 15.0);
                    otherSlots = Math.Ceiling(amount / 15.0);
                }

                await ReplyAsync("", false, GetLargeFurnEmbed("High Quality Metal Ore", totalTime, totalWood, oddSlot, otherSlots, formattedTime));
                break;
            }
        }
    }

    public Embed GetSmallFurnEmbed(string oreType, double totalTime, double totalWood, double oddSlot, double otherSlots, string formattedTime)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        eb.WithColor(Color.Red);

        eb.WithThumbnailUrl("https://rustlabs.com/img/items180/furnace.png");
        eb.WithTitle($"Small Furnace");
        eb.AddField(oreType, $"Time: {formattedTime}\nWood Required: {totalWood}\nCharcoal Produced: {Math.Floor(totalWood * 0.75)}");
        eb.AddField("Efficient Slot Numbers", $"{oddSlot} - {otherSlots} - {otherSlots}");

        return eb.Build();
    }

    public Embed GetLargeFurnEmbed(string oreType, double totalTime, double totalWood, double oddSlot, double otherSlots, string formattedTime)
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        eb.WithColor(Color.Red);

        eb.WithThumbnailUrl("https://rustlabs.com/img/items180/furnace.large.png");
        eb.WithTitle($"Large Furnace");
        eb.AddField(oreType, $"Time: {formattedTime}\nWood Required: {totalWood}\nCharcoal Produced: {Math.Floor(totalWood * 0.75)}");
        eb.AddField("Efficient Slot Numbers", $"{oddSlot} - {oddSlot} - {oddSlot} - {oddSlot} - {oddSlot} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots} - {otherSlots}");

        return eb.Build();
    }
}