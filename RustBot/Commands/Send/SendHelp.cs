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
public class Help : ModuleBase<SocketCommandContext>
{
    [Command("help", RunMode = RunMode.Async)]
    [Summary("Sends help info")]
    public async Task SendHelpMessage()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Program p = new Program();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        List<CommandInfo> commands = Program._commands.Commands.ToList();
        commands = commands.OrderBy(x => x.Name).ToList();

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Help");
        eb.WithColor(Color.Red);
        eb.WithFooter(fb);

        foreach (CommandInfo command in commands)
        {
            if (command.Module.Group != "admin" || command.Module.Group != "wip")
            {
                StringBuilder args = new StringBuilder();
                foreach (ParameterInfo param in command.Parameters)
                {
                    args.Append($" [{param.Name}]");
                }

                //Name of command - Example, !help
                eb.AddField($"{Program.prefix}{command.Name}{args.ToString()}", $"{command.Summary}");
            }
        }

        sw.Stop();
        fb.WithText($"Called by {Context.Message.Author.Username} | Completed in {sw.ElapsedMilliseconds}ms");

        await ReplyAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());

        await Utilities.StatusMessage("help", Context);
    }
}