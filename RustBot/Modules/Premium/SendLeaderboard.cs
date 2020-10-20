using Discord.Commands;
using System;
using System.Threading.Tasks;
using RustBot;
using RustBot.Permissions;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Discord;
using System.Text;
using System.Net.Sockets;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
public class Leaderboard : ModuleBase<SocketCommandContext>
{
    [Command("leaderboard", RunMode = RunMode.Async)]
    [Summary("Leaderboard of player stats which grows each time a new player runs the r!stats command. Add yourself to the leaderboard by typing r!stats.")]
    [Remarks("Misc")]
    public async Task SendLeaderboard([Remainder]string board = null)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        
        
        if(board == null)
        {
            EmbedBuilder eb = new EmbedBuilder();
            EmbedFooterBuilder fb = new EmbedFooterBuilder();

            eb.WithTitle($"Leaderboard");
            fb.WithIconUrl(Context.User.GetAvatarUrl());

            eb.AddField("Information", "To view the leaderboards, you will need to specify a specific statistic to list. Add yourself to the leaderboard by typing r!stats.");
            eb.AddField("Valid Statistics", "```css\ndeaths, kill_player, headshot, bullet_fired, bullet_hit_player, bullet_hit_building, bullet_hit_entity, bullet_hit_bear, bullet_hit_wolf, bullet_hit_boar, harvest_stones, harvest_cloth, harvest_wood, rocket_fired, item_drop, death_suicide, blueprint_studied, calories_consumed, placed_blocks```");
            eb.AddField("Correct Usage", "The correct usage of the command is `r!leaderboard [statistic]`. For example, you could do `r!leaderboard kill_player` to view the kills leaderboard, or `r!leaderboard bullet_fired` to view the bullets fired leaderboard.");
            fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User, sw));

            await ReplyAsync("", false, eb.Build());
            
        }
        else
        {
            if (Context.User.GetPremiumRank() != null && !(Context.User.GetPremiumRank() is Cloth))
            {
                Dictionary<string, string> selectedPlayers = GetPlayers(board, 10);

                if (selectedPlayers == null) { await ReplyAsync("", false, Utilities.GetEmbedMessage("Leaderboard", "Error", Language.Leaderboard_Error_Not_Found, Context.User, Utilities.GetFooter(Context.User, sw))); return; }

                EmbedBuilder eb = new EmbedBuilder();
                EmbedFooterBuilder fb = new EmbedFooterBuilder();

                eb.WithTitle($"Leaderboard");
                eb.WithThumbnailUrl("http://i.bunnyslippers.dev/3r0fx3g4.png");
                fb.WithIconUrl(Context.User.GetAvatarUrl());

                StringBuilder sb = new StringBuilder();
                sb.Append("```css\n");

                int count = 1;
                foreach (KeyValuePair<string, string> player in selectedPlayers)
                {
                    sb.Append($"\n{count}. {player.Key}: {player.Value}");
                    count++;
                }
                sb.Append("        ```");

                eb.AddField(board, sb.ToString());
                fb.WithText(PremiumUtils.SelectFooterEmbedText(Context.User, sw));

                eb.WithFooter(fb);

                await ReplyAsync("", false, eb.Build());
            }
            else
            {
                await ReplyAsync("", false, Utilities.GetEmbedMessage("Premium Error", "Leaderboard", Language.Premium_Verification_Error_Failed, Context.User, Utilities.GetFooter(Context.User, sw)));
            }
        }
    }

    public string[] validBoards = { "deaths", "kill_player", "headshot", "bullet_fired", "bullet_hit_player", "bullet_hit_building", "bullet_hit_entity", "bullet_hit_bear", "bullet_hit_wolf", "bullet_hit_boar", "harvest_stones", "harvest_cloth", "harvest_wood", "rocket_fired", "item_drop", "death_suicide", "blueprint_studied", "calories_consumed", "placed_blocks" };

    public Dictionary<string, string> GetPlayers(string board, int numOfPlayers)
    {
        //If the specified board doesn't exist, then return null
        if (!validBoards.Contains(board)) { return null; }

        //Declares the Dictionary that will store a players stats for the specified board
        //Key = Player's Name | Value = Statistic
        Dictionary<string, string> basePlayers = new Dictionary<string, string> { }; 

        //Loops through each cached player
        foreach (KeyValuePair<string, Dictionary<string, string>> player in Utilities.statCache)
        {
            //Adds the player and statistic to the Dictionary
            basePlayers.Add(player.Value["player_name"], player.Value[board]);
        }

        //Converts the Dictionary to a list of KeyValuePair's and sorts descending
        List<KeyValuePair<string, string>> sortedList = SortPlayerDictionary(basePlayers);

        //
        Dictionary<string, string> selectedPlayers = new Dictionary<string, string> { };

        if (basePlayers.Count < numOfPlayers) { numOfPlayers = basePlayers.Count; }
        Console.WriteLine(selectedPlayers.Count);

        for (int i = 0; i < numOfPlayers; i++)
        {
            KeyValuePair<string, string> player = sortedList.ElementAt(i);
            selectedPlayers.Add(player.Key, player.Value);
        }

        return selectedPlayers;

    }

    public List<KeyValuePair<string, string>> SortPlayerDictionary(Dictionary<string, string> basePlayers)
    {
        List<KeyValuePair<string, string>> sortedList = basePlayers.ToList();
        sortedList.Sort(
            delegate (KeyValuePair<string, string> pair1,
            KeyValuePair<string, string> pair2)
            {
                return Convert.ToDouble(pair2.Value).CompareTo(Convert.ToDouble(pair1.Value));
            });

        return sortedList;
    }
}