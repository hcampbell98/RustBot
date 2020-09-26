using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;
using System.Text;
using System.Reflection;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Thanks : ModuleBase<SocketCommandContext>
{
    [Command("thanks", RunMode = RunMode.Async)]
    [Summary("A list of all our fantastic premium users.")]
    [Remarks("Support")]
    public async Task SendThanks()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Thank You List");
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
        eb.WithFooter(fb);

        eb.AddField("High Quality", $"{GetMemberList(new HighQuality())}");
        eb.AddField("Wooden", $"{GetMemberList(new Wooden())}");
        eb.AddField("Cloth", $"{GetMemberList(new Cloth())}");

        await ReplyAsync("", false, eb.Build());
    }

    public string GetMemberList(PremiumRank rank)
    {
        if(rank is Cloth)
        {
            if (Cloth.Members.Count == 0) { return "No members yet :( Be the first by typing r!premium"; }
            else
            {
                StringBuilder members = new StringBuilder();

                foreach (PremiumUser user in Cloth.Members)
                {
                    members.Append($"{user.Name}\n");
                }

                return members.ToString();
            }
        }
        else if(rank is Wooden)
        {
            if (Wooden.Members.Count == 0) { return "No members yet :( Be the first by typing r!premium"; }
            else
            {
                StringBuilder members = new StringBuilder();

                foreach (PremiumUser user in Wooden.Members)
                {
                    members.Append($"{user.Name}\n");
                }

                return members.ToString();
            }
        }
        else if (rank is HighQuality)
        {
            if (HighQuality.Members.Count == 0) { return "No members yet :( Be the first by typing r!premium"; }
            else
            {
                StringBuilder members = new StringBuilder();

                foreach (PremiumUser user in HighQuality.Members)
                {
                    members.Append($"{user.Name}\n");
                }

                return members.ToString();
            }
        }

        return "Not found";
    }
}