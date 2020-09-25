using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;
using System.Diagnostics;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class BotInvide : ModuleBase<SocketCommandContext>
{
    [Command("invite", RunMode = RunMode.Async)]
    [Summary("Sends a link to invite the bot.")]
    [Remarks("Info")]
    public async Task SendHSendBotInvite()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();



        eb.WithTitle($"Invite");
        eb.WithDescription("[Click here to add the bot to your own server!](https://discord.com/oauth2/authorize?client_id=732215647135727716&scope=bot&permissions=268643345)");
        eb.WithThumbnailUrl("https://imgur.com/vwT3DuL.png");
        eb.WithColor(Color.Red);;
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());
        fb.WithText($"Called by {Context.User.Username} | Completed in {sw.ElapsedMilliseconds}ms");
        eb.WithFooter(fb);

        await ReplyAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());

    }
}