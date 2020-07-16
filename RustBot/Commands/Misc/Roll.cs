using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Linq;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Roll : ModuleBase<SocketCommandContext>
{
    [Command("roll", RunMode = RunMode.Async)]
    [Summary("Randomly picks a winner")]
    public async Task SendRoll(int max)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

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

        await Utilities.StatusMessage("roll", Context);
    }
}