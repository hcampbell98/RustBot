using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using Discord;
using System.Text;
using System.Linq;
using Discord.WebSocket;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class GuildList : ModuleBase<SocketCommandContext>
{
    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns list of guilds the bot is currently in")]
    [Remarks("Admin")]
    public async Task SendGuildList()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        StringBuilder sb = new StringBuilder();
        int totalMembers = 0;

        foreach(var guild in Program._client.Guilds)
        {
            sb.Append($"{guild.Name} | {guild.MemberCount}\n");
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
    }

    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns members of a guild")]
    [Remarks("Admin")]
    public async Task SendGuildList(string guildName)
    {
        StringBuilder sb = new StringBuilder();
        SocketGuild g = Program._client.Guilds.FirstOrDefault(x => x.Name.ToLower() == guildName.ToLower());

        if (g == null) { throw new Exception("Specified guild not found."); }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        int totalBots = await TotalBotsAsync(g);

        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Guild Information");
        eb.AddField($"{g.Name}", $"Guild Owner: {g.Owner.Nickname}" +
            $"Total Members: {g.MemberCount}" +
            $"Bots: {totalBots}" +
            $"Users: {g.MemberCount - totalBots}" +
            $"Text Channels: {g.TextChannels.Count}" +
            $"Voice Channels: {g.VoiceChannels.Count}" +
            $"Server Region: {g.VoiceRegionId}" +
            $"Created At: {g.CreatedAt}");
        eb.WithColor(Color.Red);
        eb.WithFooter(fb);



        await ReplyAsync("", false, eb.Build());
    }

    [Command("guildlist", RunMode = RunMode.Async)]
    [Summary("Returns members of a guild")]
    [Remarks("Admin")]
    public async Task SendGuildList(string guildName, bool genInvite)
    {
        StringBuilder sb = new StringBuilder();
        SocketGuild g = Program._client.Guilds.FirstOrDefault(x => x.Name.ToLower() == guildName.ToLower());

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


        fb.WithText($"Called by {Context.Message.Author.Username}");
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