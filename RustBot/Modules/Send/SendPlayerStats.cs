using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
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
    [Remarks("Tools")]
    public async Task SendPlayerStats(string steamID = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (PermissionManager.GetPerms(Context.Message.Author.Id) < PermissionConfig.User) { await Context.Channel.SendMessageAsync("Not authorised to run this command."); return; }

        Dictionary<string, string> playerStats;
        string steamID64;

        if (steamID == null)
        {
            steamID64 = SteamLink.GetSteam(Context.User.Id.ToString());

            if (steamID64 == null) { await ReplyAsync("It appears your steam account isn't linked to your Discord account. Please run r!link"); }

            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }
        else if (steamID.StartsWith("<"))
        {
            steamID64 = SteamLink.GetSteam(Utilities.GetNumbers(steamID));
            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }
        else
        {
            if (steamID.Contains("https://steamcommunity.com")) { steamID64 = Utilities.GetNumbers(steamID); }
            else if (steamID.Contains("STEAM") || steamID.StartsWith("7656119")) { steamID64 = SteamIDUtils.RetrieveID(steamID); }
            else { await ReplyAsync("Make sure the input is a valid SteamID/SteamID64 (e.g. 76561198254673414). You can also r!link your account which allows you to see your stats simply by typing r!stats."); return; }

            playerStats = await Utilities.GetPlayerInfo(steamID64);
        }

        //If the profile is private, handle the exception.
        if (playerStats == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Player Stats", "Error", "The profile specified may be private. If this profile is yours, please change to public and try again.", Context.User, Color.Red, Utilities.GetFooter(Context.User, sw))); return; }

        ProfileInfo profileInfo = await SteamIDUtils.GetProfileInfo(steamID64);
        PlayerStat defStat = new PlayerStat { Name = "Default", Value = "0" };

        //PvP Statistics
        double deaths = int.Parse(playerStats.GetValueOrDefault("deaths", "0"));
        double kill_player = int.Parse(playerStats.GetValueOrDefault("kill_player", "0"));
        double headshot = int.Parse(playerStats.GetValueOrDefault("headshot", "0"));

        //Rifle statistics
        int bullet_fired = int.Parse(playerStats.GetValueOrDefault("bullet_fired", "0"));
        int bullet_hit_player = int.Parse(playerStats.GetValueOrDefault("bullet_hit_player", "0"));
        int bullet_hit_building = int.Parse(playerStats.GetValueOrDefault("bullet_hit_building", "0"));
        int bullet_hit_entity = int.Parse(playerStats.GetValueOrDefault("bullet_hit_entity", "0"));
        int bullet_hit_sign = int.Parse(playerStats.GetValueOrDefault("bullet_hit_sign", "0"));
        //Animal Hits
        double bullet_hit_bear = int.Parse(playerStats.GetValueOrDefault("bullet_hit_bear", "0"));
        double bullet_hit_horse = int.Parse(playerStats.GetValueOrDefault("bullet_hit_horse", "0"));
        double bullet_hit_stag = int.Parse(playerStats.GetValueOrDefault("bullet_hit_stag", "0"));
        double bullet_hit_wolf = int.Parse(playerStats.GetValueOrDefault("bullet_hit_wolf", "0"));
        double bullet_hit_boar = int.Parse(playerStats.GetValueOrDefault("bullet_hit_boar", "0"));
        double bullet_hit_playercorpse = int.Parse(playerStats.GetValueOrDefault("bullet_hit_playercorpse", "0"));
        double bullet_hit_corpse = int.Parse(playerStats.GetValueOrDefault("bullet_hit_corpse", "0"));
        double animalTotal = bullet_hit_bear + bullet_hit_boar + bullet_hit_horse + bullet_hit_stag + bullet_hit_wolf + bullet_hit_corpse + bullet_hit_playercorpse;

        //Bow statistics
        int arrow_fired = int.Parse(playerStats.GetValueOrDefault("arrow_fired", "0"));
        int arrow_hit_player = int.Parse(playerStats.GetValueOrDefault("arrow_hit_player", "0"));
        int arrow_hit_building = int.Parse(playerStats.GetValueOrDefault("arrow_hit_building", "0"));

        //Harvest info
        double harvest_stones = int.Parse(playerStats.GetValueOrDefault("harvest.stones", "0"));
        double harvest_cloth = int.Parse(playerStats.GetValueOrDefault("harvest.cloth", "0"));
        double harvest_wood = int.Parse(playerStats.GetValueOrDefault("harvest.wood", "0"));

        //Misc info
        double rocket_fired = int.Parse(playerStats.GetValueOrDefault("rocket_fired", "0"));
        double item_drop = int.Parse(playerStats.GetValueOrDefault("item_drop", "0"));
        double blueprint_studied = int.Parse(playerStats.GetValueOrDefault("blueprint_studied", "0"));
        double death_suicide = int.Parse(playerStats.GetValueOrDefault("death_suicide", "0"));
        double inventory_opened = double.Parse(playerStats.GetValueOrDefault("INVENTORY_OPENED", "0"));
        double seconds_speaking = double.Parse(playerStats.GetValueOrDefault("seconds_speaking", "0"));
        double calories_consumed = double.Parse(playerStats.GetValueOrDefault("calories_consumed", "0"));
        double placed_blocks = double.Parse(playerStats.GetValueOrDefault("placed_blocks", "0"));

        //Calculated statistics
        double rifleAccuracy = (bullet_hit_player + bullet_hit_sign + animalTotal + bullet_hit_entity) / (bullet_fired);
        double headshotPercentage = (headshot / bullet_hit_player);
        double kdRatio = (kill_player / deaths);

        EmbedBuilder eb = new EmbedBuilder();

        eb.WithTitle($"{profileInfo.ProfileName}");
        eb.WithUrl($"{profileInfo.ProfileURL}");
        eb.WithThumbnailUrl(profileInfo.AvatarMedium);
        eb.WithColor(Color.Red);
        eb.AddField("PvP Info", $"```css\nKills: {kill_player}\nDeaths: {deaths}\nK/D Ratio: {Math.Round(kdRatio, 2)}\nHeadshots: {Math.Round(headshotPercentage * 100, 2)}%\nAccuracy: {Math.Round(rifleAccuracy * 100, 2)}%```", true);
        eb.AddField("Weapon Hits", $"```css\nBuilding Hits: {bullet_hit_building}\nBear Hits: {bullet_hit_bear}\nHorse Hits: {bullet_hit_horse}\nStag Hits: {bullet_hit_stag}\nWolf Hits: {bullet_hit_wolf}\nBoar Hits: {bullet_hit_boar}```", true);
        eb.AddField("Harvested", $"```css\nStone: {harvest_stones}\nWood: {harvest_wood}\nCloth: {harvest_cloth}```", true);
        eb.AddField("Misc", $"```css\nItems Dropped: {item_drop}\nBlueprints Studied: {blueprint_studied}\nSuicides: {death_suicide}\nInventory Opened: {inventory_opened}\nTime Speaking: {Math.Round(seconds_speaking, 2)}s\nCalories Consumed: {calories_consumed}\nBlocks Placed: {placed_blocks}\nRockets Fired: {rocket_fired}```");
        eb.AddField("Steam Link", "You can link your Steam account by running r!link. This allows you to see your stats simply by typing r!stats.", false);

        sw.Stop();
        eb.WithFooter(Utilities.GetFooter(Context.User, sw));
        await ReplyAsync("", false, eb.Build());
    }
}

