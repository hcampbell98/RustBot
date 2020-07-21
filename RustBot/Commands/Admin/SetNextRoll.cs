using Discord.Commands;
using System;
using System.Threading.Tasks;
using SSRPBalanceBot;
using SSRPBalanceBot.Permissions;
using System.Collections.Generic;
using System.Linq;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
[Group("admin")]
public class SetNextRoll : ModuleBase<SocketCommandContext>
{
    [Command("nextroll", RunMode = RunMode.Async)]
    [Summary("Chooses a random number between 0 and the specified value")]
    public async Task NextRoll(int nextRoll, string mention)
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.Admin) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        //If there's a mention, add the nextRoll for the mentioned user
        if(mention != null)
        {
            ulong dID;
            ulong.TryParse(Utilities.GetNumbers(mention), out dID);

            Program.nextRolls.Add(new NextRoll { userID = dID, nextRoll = nextRoll });
        }
        //Else add for the user that called the command
        else
        {
            Program.nextRolls.Add(new NextRoll { userID = Context.Message.Author.Id, nextRoll = nextRoll });
        }

        await Utilities.StatusMessage("nextroll", Context);
    }
}

public class NextRoll
{
    public ulong userID { get; set; } 
    public int nextRoll { get; set; }
}