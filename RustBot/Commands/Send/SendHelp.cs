using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Help : ModuleBase<SocketCommandContext>
{
    [Command("help", RunMode = RunMode.Async)]
    [Summary("Sends help info")]
    public async Task SendHelpMessage()
    {
        Program p = new Program();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        List<CommandInfo> commands = Program._commands.Commands.ToList();
        commands = commands.OrderBy(x => x.Name).ToList();

        EmbedBuilder eb = new EmbedBuilder();
        EmbedFooterBuilder fb = new EmbedFooterBuilder();


        fb.WithText($"Called by {Context.Message.Author.Username}");
        fb.WithIconUrl(Context.Message.Author.GetAvatarUrl());

        eb.WithTitle($"Help");
        eb.WithColor(Color.Red);

        foreach (CommandInfo command in commands)
        {
            if (command.Module.Group != "admin")
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

        await Context.Message.Channel.SendMessageAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());

        await Utilities.StatusMessage("help", Context);
    }
}