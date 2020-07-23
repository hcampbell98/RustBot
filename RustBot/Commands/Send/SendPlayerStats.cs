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
using RustBot;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Stats : ModuleBase<SocketCommandContext>
{
    [Command("stats", RunMode = RunMode.Async)]
    [Summary("Sends player info. Use their SteamID/SteamID64")]
    public async Task SendPlayerStats(string steamID = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        List<PlayerStat> playerStats;



        if (steamID == null)
        {
            string steamID64 = SteamLink.GetSteam(Context.User.Id.ToString());
            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }
        else if (steamID.StartsWith("<"))
        {
            string steamID64 = SteamLink.GetSteam(Utilities.GetNumbers(steamID));
            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }
        else
        {
            string steamID64;

            if (steamID.Contains("https://steamcommunity.com")) { steamID64 = Utilities.GetNumbers(steamID); }
            else if (steamID.Contains("STEAM") || steamID.StartsWith("7656119")) { steamID64 = SteamIDUtils.RetrieveID(steamID); }
            else { await ReplyAsync("Make sure the input is either a Steam Community URL (numbers at the end) or a valid SteamID/SteamID64."); return; }

            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }

        //PvP Statistics
        double deaths = int.Parse(playerStats.First(x => x.Name == "deaths").Value);
        double kill_player = int.Parse(playerStats.First(x => x.Name == "kill_player").Value);
        double headshot = int.Parse(playerStats.First(x => x.Name == "headshot").Value);


        //Rifle statistics
        int bullet_fired = int.Parse(playerStats.First(x => x.Name == "bullet_fired").Value);
        int bullet_hit_player = int.Parse(playerStats.First(x => x.Name == "bullet_hit_player").Value);
        int bullet_hit_building = int.Parse(playerStats.First(x => x.Name == "bullet_hit_building").Value);
        int bullet_hit_entity = int.Parse(playerStats.First(x => x.Name == "bullet_hit_entity").Value);
        int bullet_hit_sign = int.Parse(playerStats.First(x => x.Name == "bullet_hit_sign").Value);
        //Animal Hits
        double bullet_hit_bear = int.Parse(playerStats.First(x => x.Name == "bullet_hit_bear").Value);
        double bullet_hit_horse = int.Parse(playerStats.First(x => x.Name == "bullet_hit_horse").Value);
        double bullet_hit_stag = int.Parse(playerStats.First(x => x.Name == "bullet_hit_stag").Value);
        double bullet_hit_wolf = int.Parse(playerStats.First(x => x.Name == "bullet_hit_wolf").Value);
        double bullet_hit_boar = int.Parse(playerStats.First(x => x.Name == "bullet_hit_boar").Value);
        double bullet_hit_playercorpse = int.Parse(playerStats.First(x => x.Name == "bullet_hit_playercorpse").Value);
        double bullet_hit_corpse = int.Parse(playerStats.First(x => x.Name == "bullet_hit_corpse").Value);
        double animalTotal = bullet_hit_bear + bullet_hit_boar + bullet_hit_horse + bullet_hit_stag + bullet_hit_wolf + bullet_hit_corpse + bullet_hit_playercorpse;

        //Bow statistics
        int arrow_fired = int.Parse(playerStats.First(x => x.Name == "arrow_fired").Value);
        int arrow_hit_player = int.Parse(playerStats.First(x => x.Name == "arrow_hit_player").Value);
        int arrow_hit_building = int.Parse(playerStats.First(x => x.Name == "arrow_hit_building").Value);

        //Harvest info
        double harvest_stones = int.Parse(playerStats.First(x => x.Name == "harvest.stones").Value);
        double harvest_cloth = int.Parse(playerStats.First(x => x.Name == "harvest.cloth").Value);
        double harvest_wood = int.Parse(playerStats.First(x => x.Name == "harvest.wood").Value);

        //Misc info
        //double rocket_fired = int.Parse(playerStats.First(x => x.Name == "rocket_fired").Value);
        double item_drop = int.Parse(playerStats.First(x => x.Name == "item_drop").Value);
        double blueprint_studied = int.Parse(playerStats.First(x => x.Name == "blueprint_studied").Value);
        double death_suicide = int.Parse(playerStats.First(x => x.Name == "death_suicide").Value);
        double inventory_opened = double.Parse(playerStats.First(x => x.Name == "INVENTORY_OPENED").Value);
        double seconds_speaking = double.Parse(playerStats.First(x => x.Name == "seconds_speaking").Value);
        double calories_consumed = double.Parse(playerStats.First(x => x.Name == "calories_consumed").Value);
        double placed_blocks = double.Parse(playerStats.First(x => x.Name == "placed_blocks").Value);

        //Calculated statistics
        double rifleAccuracy = (bullet_hit_player + bullet_hit_sign + animalTotal + bullet_hit_entity) / (bullet_fired);
        double headshotPercentage = (headshot / bullet_hit_player);
        double kdRatio = (kill_player / deaths);

        EmbedBuilder eb = new EmbedBuilder();

        eb.WithTitle($"Player Statistics");
        eb.WithColor(Color.Red);
        eb.AddField("PvP Info", $"Kills: {kill_player}\nDeaths: {deaths}\nK/D Ratio: {Math.Round(kdRatio, 2)}\nHeadshots: {Math.Round(headshotPercentage * 100, 2)}%\nAccuracy: {Math.Round(rifleAccuracy * 100, 2)}%", true);
        eb.AddField("Weapon Hits", $"Building Hits: {bullet_hit_building}\nBear Hits: {bullet_hit_bear}\nHorse Hits: {bullet_hit_horse}\nStag Hits: {bullet_hit_stag}\nWolf Hits: {bullet_hit_wolf}\nBoar Hits: {bullet_hit_boar}", true);
        eb.AddField("Harvested", $"Stone: {harvest_stones}\nWood: {harvest_wood}\nCloth: {harvest_cloth}", true);
        eb.AddField("Misc", $"Items Dropped: {item_drop}\nBlueprints Studied: {blueprint_studied}\nSuicides: {death_suicide}\nInventory Opened: {inventory_opened}\nTime Speaking: {Math.Round(seconds_speaking, 2)}s\nCalories Consumed: {calories_consumed}\nBlocks Placed: {placed_blocks}");

        sw.Stop();
        eb.WithFooter(Utilities.GetFooter(Context.User, sw));
        await ReplyAsync("", false, eb.Build());
    }
}