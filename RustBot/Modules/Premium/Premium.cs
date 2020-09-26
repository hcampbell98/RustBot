using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Premium : ModuleBase<SocketCommandContext>
{
    [Command("premium", RunMode = RunMode.Async)]
    [Summary("Take a look at what premium features we offer!")]
    [Remarks("Support")]
    public async Task SendPremium()
    {
        string bulletPoint = "<:small_blue_diamond:759438353057316866>";

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Premium");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithUrl("https://donatebot.io/checkout/701178110485463152");
        eb.WithThumbnailUrl("https://i.imgur.com/VA6V7Dn.png");

        eb.WithDescription("Consider [buying Premium](https://donatebot.io/checkout/701178110485463152) to support the development of the bot and gain access to some cool Premium benefits. Every penny spent goes straight back into keeping the bot up and running, and helps fund the development costs of new features. Thank you for your kindness. [Click here](https://donatebot.io/checkout/701178110485463152) to purchase a tier!");
        eb.AddField("High Quality Tier", $"This tier grants you access to the following benefits:\n{bulletPoint} Random Message Embed Colour\n{bulletPoint} A Spot on the r!thanks List\n{bulletPoint} Premium Embed Footer");

        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));
        eb.WithFooter(fb);

        await ReplyAsync("", false, eb.Build());
    }
}