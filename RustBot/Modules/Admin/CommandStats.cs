using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;
using System.Text;
using RustBot.Logging;
using System.Collections.Generic;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("Admin")]
public class CommandStatistics : ModuleBase<SocketCommandContext>
{
    [Command("commandstats", RunMode = RunMode.Async)]
    [Summary("Displays the number of times each command has been run")]
    [Remarks("Admin")]
    [RequireOwner]
    public async Task SendStats()
    {
        StringBuilder sb = new StringBuilder();

        var ordered = Statistics.commandStats.OrderBy(x => x.Value);

        foreach(KeyValuePair<string, int> p in ordered)
        {
            sb.Append($"{p.Key} - {p.Value}\n");
        }

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Command Information", "Statistics", $"{sb}", Context.User, Color.Red));
    }
}