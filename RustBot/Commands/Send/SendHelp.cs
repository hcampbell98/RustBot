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

        StringBuilder helpMessage = new StringBuilder();
        StringBuilder individualCMDs = new StringBuilder();

        helpMessage.Append("```");

        foreach (CommandInfo command in commands)
        {
            if (command.Module.Group != "admin")
            {
                //Name of command - Example, !help
                individualCMDs.Append(Program.prefix + command.Name);

                //Appends all parameters - Example, [item]
                foreach (ParameterInfo param in command.Parameters)
                {
                    individualCMDs.Append($" [{param.Name}]");
                }
                //Appends the command summary, Example - Sends help information
                individualCMDs.Append($" - {command.Summary}\n");

                helpMessage.Append(individualCMDs.ToString() + "\n");
                individualCMDs.Clear();
            }
        }
        helpMessage.Append("```");

        EmbedBuilder eb = new EmbedBuilder();
        eb.WithTitle("Help");
        eb.WithDescription(helpMessage.ToString());
        eb.WithUrl("https://nickgor.com");
        eb.WithCurrentTimestamp();
        eb.WithColor(Color.Green);

        await Context.Message.Channel.SendMessageAsync($"{Context.Message.Author.Mention}\n", false, eb.Build());

        await Utilities.StatusMessage("help", Context);
    }
}