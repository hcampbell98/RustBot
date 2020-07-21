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

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class BotInvide : ModuleBase<SocketCommandContext>
{
    [Command("invite", RunMode = RunMode.Async)]
    [Summary("Sends a link to invite the bot.")]
    public async Task SendHSendBotInvite()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Invite");
        eb.WithDescription("https://discord.com/oauth2/authorize?client_id=732215647135727716&scope=bot&permissions=207873");
        eb.WithThumbnailUrl("https://imgur.com/vwT3DuL.png");
        eb.WithColor(Color.Red);
        eb.WithFooter(fb);

        await ReplyAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());

        await Utilities.StatusMessage("invite", Context);
    }
}