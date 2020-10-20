using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using RustBot;
using Discord;
using System.Diagnostics;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class HoursPlayed : ModuleBase<SocketCommandContext>
{
    [Command("hours", RunMode = RunMode.Async)]
    [Summary("Returns the total number of hours a player has played Rust.")]
    [Remarks("Tools")]
    public async Task SendHours([Remainder]string steamID = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        

        string hoursPlayed;
        ProfileInfo pi;

        string steamID64;

        if (steamID == null)
        {
            steamID64 = SteamLink.GetSteam(Context.User.Id.ToString());

            if (steamID64 == null) { await ReplyAsync("It appears your steam account isn't linked to your Discord account. Please run r!link"); }

            hoursPlayed = await SteamIDUtils.GetHoursPlayed(steamID64);
            pi = await SteamIDUtils.GetProfileInfo(steamID64);
        }
        else if (steamID.StartsWith("<"))
        {
            steamID64 = SteamLink.GetSteam(Utilities.GetNumbers(steamID));
            hoursPlayed = await SteamIDUtils.GetHoursPlayed(steamID64);
            pi = await SteamIDUtils.GetProfileInfo(steamID64);
        }
        else
        {
            if (steamID.Contains("https://steamcommunity.com")) { steamID64 = Utilities.GetNumbers(steamID); }
            else if (steamID.Contains("STEAM") || steamID.StartsWith("7656119")) { steamID64 = SteamIDUtils.RetrieveID(steamID); }
            else { await ReplyAsync("Make sure the input is a valid SteamID/SteamID64 (e.g. 76561198254673414)."); return; }

            hoursPlayed = await SteamIDUtils.GetHoursPlayed(steamID64);
            pi = await SteamIDUtils.GetProfileInfo(steamID64);
        }

        await ReplyAsync("", false, Utilities.GetEmbedMessage("Hours Played", $"{pi.ProfileName}", $"Hours Played: {Convert.ToInt32(hoursPlayed) / 60}", Context.User, Utilities.GetFooter(Context.User, sw)));
    }
}