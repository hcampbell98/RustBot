using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using Discord;
using System.Text;
using System.Linq;
using Discord.WebSocket;
using System.Collections.Generic;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class GuildList : ModuleBase<SocketCommandContext>
{
    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns guild information")]
    [Remarks("Admin")]
    public async Task SendGuildList()
    {
        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));;
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Guild List");

        List<SocketGuild> guilds = Program._client.Guilds.OrderByDescending(x => x.MemberCount).ToList();

        for (int i = 0; i < 5; i++)
        {
            eb.AddField($"{guilds[i].Name}", $"Members - {guilds[i].MemberCount}");
        }
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        await ReplyAsync("", false, eb.Build());
    }


    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns guild information")]
    [Remarks("Admin")]
    public async Task SendGuildList(string guildId)
    {
        StringBuilder sb = new StringBuilder();
        SocketGuild g = Program._client.Guilds.FirstOrDefault(x => x.Id.ToString() == guildId);

        if (g == null) { throw new Exception("Specified guild not found."); }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        int totalBots = await TotalBotsAsync(g);

        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));;
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Guild Information");
        eb.AddField($"{g.Name}", $"**Guild Owner:** {g.Owner.Nickname}\n" +
            $"**Total Members:** {g.MemberCount}\n" +
            $"**Bots:** {totalBots}\n" +
            $"**Users:** {g.MemberCount - totalBots}\n" +
            $"**Text Channels:** {g.TextChannels.Count}\n" +
            $"**Voice Channels:** {g.VoiceChannels.Count}\n" +
            $"**Server Region:** {g.VoiceRegionId}\n" +
            $"**Created At:** {g.CreatedAt}\n");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
    }

    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns members of a guild")]
    [Remarks("Admin")]
    public async Task SendGuildList(string guildId, bool genInvite)
    {
        StringBuilder sb = new StringBuilder();
        SocketGuild g = Program._client.Guilds.FirstOrDefault(x => x.Id.ToString() == guildId);

        if (g == null) { throw new Exception("Specified guild not found."); }

        try
        {
            //Gets first channel in the server and generates an invite link
            INestedChannel chnl = (INestedChannel)g.TextChannels.First();
            var invite = await chnl.CreateInviteAsync();

            //Appends invite link to message
            sb.Append("" + invite.Url);
        }
        catch (Exception)
        {
            await ReplyAsync("No links found");
            return;
        }


        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User));;
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Invite");
        eb.AddField($"{g.Name}", sb.ToString());
        eb.WithColor(Color.Blue);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
    }

    public static async Task<int> TotalBotsAsync(SocketGuild g)
    {
        int total = 0;
        await g.DownloadUsersAsync();

        foreach (SocketGuildUser u in  g.Users)
        {
            if (u.IsBot) { total++; }
        }

        return total;
    }
}