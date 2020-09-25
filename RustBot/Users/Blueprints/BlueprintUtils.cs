using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using RustBot;

namespace RustBot.Users.Blueprints
{
    class BlueprintUtils
    {
        public static Dictionary<string, Blueprint> allBlueprints;
        public static List<PlayerBlueprints> playerBlueprints;

        public static bool SaveBlueprints(ulong discordID)
        {
            if (playerBlueprints.FirstOrDefault(x => x.DiscordID == discordID) == default(PlayerBlueprints)) { return false; }

            //If the file already exists, delete it
            File.Delete($"Users/Blueprints/{discordID}.json");
            //Obtain the players blueprints
            PlayerBlueprints bps = playerBlueprints.FirstOrDefault(x => x.DiscordID == discordID);
            //Write the blueprints to a file
            Utilities.WriteToJsonFile($"Users/Blueprints/{discordID}.json", bps);

            return true;
        }

        public static void UpdateBlueprints(ulong discordID, PlayerBlueprints newbps)
        {
            PlayerBlueprints oldbps = playerBlueprints.FirstOrDefault(x => x.DiscordID == discordID);

            if(oldbps == default(PlayerBlueprints))
            {
                playerBlueprints.Add(newbps);
                SaveBlueprints(discordID);
            }
            else
            {
                playerBlueprints.Remove(oldbps);
                playerBlueprints.Add(newbps);

                SaveBlueprints(discordID);
            }
        }


        public static Blueprint GetBlueprint(string input) 
        {
            Dictionary<string, int> matches = new Dictionary<string, int> { };
            Dictionary<string, Blueprint> containAllLetters = new Dictionary<string, Blueprint> { };

            foreach(var bp in allBlueprints)
            {
                bool success = true;
                char[] inputChars = input.ToLower().ToCharArray();

                foreach(char c in inputChars)
                {
                    if (!bp.Key.ToLower().Contains(c))
                    {
                        success = false;
                        break;
                    }
                }

                if (success) 
                {
                    if(bp.Key.Split(" ").Length == 0)
                    {
                        if (bp.Key.ToLower().Contains(input.ToLower()))
                        {
                            containAllLetters.Add(bp.Key, bp.Value);
                        }
                    }
                    else
                    {
                        containAllLetters.Add(bp.Key, bp.Value);
                    }
                }
            }

            foreach(var bp in containAllLetters)
            {
                matches.Add(bp.Key, LevenshteinDistance(input.ToLower(), bp.Key.ToLower()));
            }

            var match = matches.OrderBy(kvp => kvp.Value).FirstOrDefault();

            if (match.Key != null) { return allBlueprints[match.Key]; }
            else { return null; }
        }

        public static async Task<Dictionary<string, Blueprint>> GetAllBlueprints()
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri("https://rustlabs.com/blueprint-tracker"));
                Dictionary<string, Blueprint> bps = new Dictionary<string, Blueprint> { };

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                //All Level 1 bps
                foreach(HtmlNode node in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/div[2]/ul/li"))
                {
                    string workbench = node.GetAttributeValue("class", null);
                    int cost = Convert.ToInt32(node.GetAttributeValue("data-research-cost", null));
                    string iconUrl = node.SelectSingleNode("img").GetAttributeValue("src", null).Replace("//rustlabs.com/", "https://rustlabs.com/");
                    string bpName = node.SelectSingleNode("img").GetAttributeValue("alt", null);

                    Blueprint bp = new Blueprint() { Name = bpName, Cost = cost, Icon = iconUrl, Workbench = 1};
                    bps.Add(bpName, bp);
                }

                //All Level 1 bps
                foreach (HtmlNode node in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/div[3]/ul/li"))
                {
                    string workbench = node.GetAttributeValue("class", null);
                    int cost = Convert.ToInt32(node.GetAttributeValue("data-research-cost", null));
                    string iconUrl = node.SelectSingleNode("img").GetAttributeValue("src", null).Replace("//rustlabs.com/", "https://rustlabs.com/");
                    string bpName = node.SelectSingleNode("img").GetAttributeValue("alt", null);

                    Blueprint bp = new Blueprint() { Name = bpName, Cost = cost, Icon = iconUrl, Workbench = 2 };
                    bps.Add(bpName, bp);
                }

                //All Level 1 bps
                foreach (HtmlNode node in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/div[4]/ul/li"))
                {
                    string workbench = node.GetAttributeValue("class", null);
                    int cost = Convert.ToInt32(node.GetAttributeValue("data-research-cost", null));
                    string iconUrl = node.SelectSingleNode("img").GetAttributeValue("src", null).Replace("//rustlabs.com/", "https://rustlabs.com/");
                    string bpName = node.SelectSingleNode("img").GetAttributeValue("alt", null);

                    Blueprint bp = new Blueprint() { Name = bpName, Cost = cost, Icon = iconUrl, Workbench = 3 };
                    bps.Add(bpName, bp);
                }

                return bps;
            }
        }

        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }

    public class PlayerBlueprints
    {
        public ulong DiscordID { get; set; }
        public Dictionary<string, Blueprint> Blueprints { get; set; }
    }

    public class Blueprint
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Icon { get; set; }
        public int Workbench { get; set; }
    }
}
