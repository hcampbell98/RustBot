using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using Discord;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Wipe : ModuleBase<SocketCommandContext>
{
    [Command("wipe", RunMode = RunMode.Async)]
    [Summary("Returns wipe information")]
    [Remarks("Tools")]
    public async Task SendWipeInfo()
    {
        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        //Gets the first day of next month
        DateTime dt = DateTime.Now;
        DateTime firstDayNextMonth = dt.AddMonths(1).AddDays(-dt.Day + 1);

        DateTime nextWipe = GetNextWeekday(DayOfWeek.Thursday);
        DateTime nextForceWipe = GetForceWipe(firstDayNextMonth);

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Wipe Information", "Wipes", $"Next Force Wipe is on {nextForceWipe.ToString("MMMM dd")}, {Math.Ceiling(nextForceWipe.Subtract(DateTime.Now).TotalDays)} days away.\nNext weekly wipe is on {nextWipe.ToString("MMMM dd")}, {Math.Ceiling(nextWipe.Subtract(DateTime.Now).TotalDays)} days away.", Context.Message.Author, Color.Red));
    }

    static DateTime GetNextWeekday(DayOfWeek day)
    {
        DateTime result = DateTime.Now.AddDays(1);
        while (result.DayOfWeek != day)
            result = result.AddDays(1);
        return result;
    }

    public static DateTime GetForceWipe(DateTime currentMonth)
    {
        var day = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        day = FindNext(DayOfWeek.Thursday, day);
        //day = FindNext(DayOfWeek.Thursday, day.AddDays(1));
        return day;
    }

    private static DateTime FindNext(DayOfWeek dayOfWeek, DateTime after)
    {
        DateTime day = after;
        while (day.DayOfWeek != dayOfWeek) day = day.AddDays(1);
        return day;
    }
}