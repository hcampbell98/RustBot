using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Roll : ModuleBase<SocketCommandContext>
{
    [Command("roll", RunMode = RunMode.Async)]
    [Summary("Randomly picks a winner")]
    [Remarks("Fun")]
    public async Task SendRoll(int max)
    {
        

        Random rnd = new Random();
        int roll = rnd.Next(1, max);

        NextRoll rollData = Program.nextRolls.FirstOrDefault(id => id.userID == Context.Message.Author.Id);

        //If no nextRolls exist, run normal roll
        if (rollData == null) 
        {
            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} rolled `{roll}`");
        }
        else
        {
            Program.nextRolls.Remove(rollData);

            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} rolled `{rollData.nextRoll}`");
        }

    }
}