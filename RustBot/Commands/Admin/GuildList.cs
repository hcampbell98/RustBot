using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Data;
using Discord;
using System.Text;
using System.Linq;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class GuildList : ModuleBase<SocketCommandContext>
{
    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns list of guilds the bot is currently in")]
    public async Task SendGuildList()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        StringBuilder sb = new StringBuilder();
        int totalMembers = 0;

        foreach(var guild in Program._client.Guilds)
        {
            sb.Append($"{guild.Name} | {guild.MemberCount} | {guild.Id}\n");
            totalMembers += guild.MemberCount;
        }


        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Guild List");
        eb.AddField($"{Program._client.Guilds.Count} | {totalMembers}", sb.ToString());
        eb.WithColor(Color.Blue);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
        await Utilities.StatusMessage("roll", Context);
    }

    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns members of a guild")]
    public async Task SendGuildList(string guildID)
    {
        StringBuilder sb = new StringBuilder();
        string gName = "";
        int gMembers = 0;

        foreach (var guild in Program._client.Guilds)
        {
            if (guild.Id.ToString() == guildID)
            {
                foreach (var member in guild.Users)
                {
                    sb.Append(member.Username + "\n");
                }
                gName = guild.Name;
                gMembers += guild.MemberCount;
            }
        }


        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Guild List");
        eb.AddField($"{gName} | {gMembers}", sb.ToString());
        eb.WithColor(Color.Blue);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
        await Utilities.StatusMessage("roll", Context);
    }

    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns members of a guild")]
    public async Task SendGuildList(string guildID, bool genInvite)
    {
        StringBuilder sb = new StringBuilder();
        string gName = "";


        foreach (var guild in Program._client.Guilds)
        {
            try
            {
                if (guild.Id.ToString() == guildID)
                {
                    //Gets first channel in the server and generates an invite link
                    INestedChannel chnl = (INestedChannel)guild.TextChannels.First();
                    var invite = await chnl.CreateInviteAsync();

                    gName = guild.Name;

                    //Appends invite link to message
                    sb.Append("" + invite.Url);
                }
            }
            catch (Exception)
            {
                await ReplyAsync("No links found");
                return;
            }
        }


        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Invite");
        eb.AddField($"{gName}", sb.ToString());
        eb.WithColor(Color.Blue);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
        await Utilities.StatusMessage("roll", Context);
    }
}