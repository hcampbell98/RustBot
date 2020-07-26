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
    [Remarks("Misc")]
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
        eb.AddField("Info", "Type r!help and then the name of the command to see information about each individual command.");


        foreach(var c in commands.GroupBy(x => x.Remarks).Select(x => x))
        {
            //If the command is admin related, continue. We don't want admin commands mixed in with non-admin commands
            if (c.ElementAt(0).Remarks == "Admin") { continue; }

            //Used for preventing the same command being added twice in situations where overloads are specified
            string prevCommandName = "";

            StringBuilder args = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            foreach (CommandInfo command in c)
            {
                //Used for preventing the same command being added twice in situations where overloads are specified
                if (prevCommandName == command.Name) { continue; }
                prevCommandName = command.Name;

                foreach (ParameterInfo param in command.Parameters)
                {
                    args.Append($" [{param.Name}]");
                }
                sb.Append($"{Program.prefix}{command.Name}\n");
            }
            //Checks for missing remarks. If one is found, it prints an error message in the console.
            if (c.ElementAt(0).Remarks == "" || c.ElementAt(0).Remarks == null) { Console.WriteLine($"Missing Remark: {c.ElementAt(0).Name}"); }

            eb.AddField($"{c.ElementAt(0).Remarks} - {c.Count()}", $"```css\n{sb.ToString()}```", true);
        }

        sw.Stop();
        fb.WithText($"Called by {Context.Message.Author.Username} | Completed in {sw.ElapsedMilliseconds}ms");

        await ReplyAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());
    }

    [Command("help", RunMode = RunMode.Async)]
    [Summary("Sends help info")]
    [Remarks("Misc")]
    public async Task SendCommandHelpMessage(string cmdName)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        cmdName = cmdName.Replace("r!", "").ToLower();

        List<CommandInfo> commands = Program._commands.Commands.ToList();
        CommandInfo command = commands.FirstOrDefault(x => x.Name == cmdName.ToLower());

        StringBuilder args = new StringBuilder();
        foreach(ParameterInfo arg in command.Parameters)
        {
            args.Append($" [{arg.Name}]");
        }

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();

        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"{Program.prefix}{command.Name}");
        eb.WithColor(Color.Red);
        eb.WithDescription($"{command.Summary}");
        eb.AddField($"Usage", $"{Program.prefix}{command.Name} {args.ToString()}");

        eb.WithFooter(Utilities.GetFooter(Context.User, sw));
        await ReplyAsync("", false, eb.Build());
    }
}