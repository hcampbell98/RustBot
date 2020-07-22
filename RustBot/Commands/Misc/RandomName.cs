using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Text.RegularExpressions;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class RandomName : ModuleBase<SocketCommandContext>
{
    [Command("randomname", RunMode = RunMode.Async)]
    [Summary("Randomly picks a name from a comma separated list")]
    public async Task SendRandomName([Remainder]string input)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        //Removes spaces before and after commas
        string whitespaceRemovedInput = Regex.Replace(input, " *, *", ",");

        //Separates names into individual strings and stores in array
        string[] names = whitespaceRemovedInput.Split(',');
        double percentageChance = 100 / names.Length;

        Random rnd = new Random();

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Random Name");
        eb.AddField("Result", $"{names[rnd.Next(0, names.Length - 1)]} ({Math.Round(percentageChance, 2)}% chance)");
        eb.WithColor(Color.Blue);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
    }
}