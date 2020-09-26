using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot.Users.Blueprints;
using System.Text;
using Discord;
using System.Diagnostics;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class FindBlueprint : ModuleBase<SocketCommandContext>
{
    [Command("blueprint", RunMode = RunMode.Async)]
    [Summary("Displays blueprint information such as scrap cost and workbench level.")]
    [Remarks("Tools")]
    public async Task SendBlueprint([Remainder]string blueprint)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        StringBuilder sb = new StringBuilder();
        Blueprint bp = BlueprintUtils.GetBlueprint(blueprint);

        if (bp == null) { await ReplyAsync($"Blueprint `{blueprint}` not found."); return; }

        sb.Append($"Research Cost: {bp.Cost}\n");
        sb.Append($"Workbench Level: {bp.Workbench}\n");

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        eb.WithTitle("Blueprint");
        eb.AddField($"{bp.Name}", $"{sb}");
        eb.WithThumbnailUrl(bp.Icon);
        eb.WithColor(PremiumUtils.SelectEmbedColour(Context.User));

        fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User, sw));;
        fb.WithIconUrl(Context.User.GetAvatarUrl());
        eb.WithFooter(fb);

        await ReplyAsync("", false, eb.Build());
    }
}