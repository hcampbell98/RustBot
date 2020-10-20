using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Donate : ModuleBase<SocketCommandContext>
{
    [Command("donate", RunMode = RunMode.Async)]
    [Summary("Help out the bot by donating")]
    [Remarks("Support")]
    public async Task SendDonate()
    {
        

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        eb.WithTitle("Consider donting to support the bot");
        eb.WithUrl("https://www.paypal.me/HJ718");
        eb.WithDescription("Developing, hosting and providing support for the bot is sadly not free. I work on my own and thus cannot afford to pay for all of this in the long term. This is why I am asking for your kind donations.\n\nDonating would allow the bot to be online 24/7/365 and receive continual updates with new features which you'll love.\n\nIf you are considering donating, [please click here](https://www.paypal.me/HJ718).");
        eb.WithThumbnailUrl("https://i.imgur.com/VA6V7Dn.png");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));

        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));;
        fb.WithIconUrl(Context.User.GetAvatarUrl());
        eb.WithFooter(fb);

        await ReplyAsync("", false, eb.Build());
    }
}