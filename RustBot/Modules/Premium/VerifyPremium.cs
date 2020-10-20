using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;
using System.Diagnostics;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Verify : ModuleBase<SocketCommandContext>
{
    [Command("verify", RunMode = RunMode.Async)]
    [Summary("Verifies your premium access. Just type r!verify [your PayPal Transaction ID].")]
    [Remarks("Support")]
    public async Task VerifyPremium(string transactionId)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        

        PremiumRank p = await PremiumUtils.VerifyPremium(transactionId, Context.User.Id);
        
        if(p == null || !PremiumUtils.AssignPremiumRank(Context.User, p))
        {
            await ReplyAsync("", false, Utilities.GetEmbedMessage("Premium Verification", "Verification Failed", Language.Premium_Verification_Error_Failed, Context.User));
        }
        else
        {
            EmbedBuilder eb = new EmbedBuilder();
            EmbedFooterBuilder fb = new EmbedFooterBuilder();

            fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

            eb.WithTitle($"Premium Verification");
            eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));
            eb.WithFooter(fb);
            eb.AddField("Verification Successful", "Congratulations, your rank has been assigned.");

            if ((object)p is Cloth)
            {
                eb.AddField("Rank Assigned", "Cloth");
            }
            else if ((object)p is Wooden)
            {
                eb.AddField("Rank Assigned", "Wooden");
            }
            else if ((object)p is HighQuality)
            {
                eb.AddField("Rank Assigned", "High Quality");
            }

            fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User, sw));;
            eb.WithFooter(fb);

            await ReplyAsync("", false, eb.Build());
        }
    }
}