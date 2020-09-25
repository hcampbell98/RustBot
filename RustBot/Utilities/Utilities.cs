using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace RustBot
{
    class Utilities
    {
        public static string apiKey = File.ReadAllText("Config/steamApiKey.cfg");
        public static Dictionary<string, BreakableInfo> breakCache = new Dictionary<string, BreakableInfo> { };
        public static Dictionary<string, Item> itemCache = new Dictionary<string, Item> { };
        public static Dictionary<string, Dictionary<string, string>> statCache = new Dictionary<string, Dictionary<string, string>> { };
        public static Dictionary<string, ProfileInfo> profileInfoCache = new Dictionary<string, ProfileInfo> { };

        public static string searchPlaceCache;
        public static string searchBlockCache;

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
                writer.Write(Environment.NewLine);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string line) where T : new()
        {
            TextReader reader = null;
            try
            {
                return JsonConvert.DeserializeObject<T>(line);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public static List<T> FillList<T>(string path) where T : new()
        {
            List<T> list = new List<T> { };
            string[] lines = File.ReadAllLines(path);
            foreach (var item in lines)
            {
                list.Add(ReadFromJsonFile<T>(item));
            }

            return list;
        }

        //--------------Item information-----------------------------------------------------------------------------------------------

        public static async Task<List<SearchItem>> SearchForItem(string itemName)
        {
            using (WebClient wc = new WebClient()) 
            { 
                string page = await wc.DownloadStringTaskAsync(new Uri($"https://rustlabs.com/search={itemName}"));
                List<SearchItem> items = new List<SearchItem> { };


                //Console.WriteLine(page);

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                //If there's only one result, RustLabs automatically redirects to the items info page. This accounts for that.
                if (parsePage.DocumentNode.SelectSingleNode("/html/head/title").InnerText != "Search")
                {
                    string itemIcon = parsePage.DocumentNode.SelectSingleNode("/html/head/meta[10]").GetAttributeValue("content", null).Replace("//rustlabs.com/", "https://rustlabs.com/");
                    string ItemName = parsePage.DocumentNode.SelectSingleNode("/html/head/meta[7]").GetAttributeValue("content", null);
                    string URL = parsePage.DocumentNode.SelectSingleNode("/html/head/meta[9]").GetAttributeValue("content", null);

                    items.Add(new SearchItem() { ItemName = ItemName, Icon = itemIcon, URL = URL });
                    return items;
                }

                //Actual search page
                var searchResults = parsePage.GetElementbyId("wrap");

                if (searchResults.InnerHtml.Contains("Nothing found.")) { return null; }
                else
                {
                    foreach(HtmlNode row in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div/div/table/tbody/tr"))
                    {
                        //If the item is a skin, skip
                        if (row.GetAttributeValue("data-group2", "") != "items") { continue; }

                        HtmlNodeCollection cells = row.SelectNodes("td");

                        string itemIcon = cells[0].SelectSingleNode("img").Attributes[0].Value.Replace("//rustlabs.com/", "https://rustlabs.com/");
                        string ItemName = cells[1].SelectSingleNode("a").InnerText;
                        string URL = cells[1].SelectSingleNode("a").GetAttributeValue("href", null).Replace("//rustlabs.com/", "https://rustlabs.com/");



                        items.Add(new SearchItem() { ItemName = ItemName, Icon = itemIcon, URL = URL });
                    }

                    return items;
                }
            }
        }

        public static async Task<Item> GetItemInfo(SearchItem item)
        {
            using (WebClient wc = new WebClient())
            {
                if (itemCache.ContainsKey(item.ItemName)) { return itemCache[item.ItemName]; }

                string page = await wc.DownloadStringTaskAsync(new Uri(item.URL + "#sort=3,1,0"));

                List<ItemInfoRow> infoTableStats = new List<ItemInfoRow> { };
                List<DropChance> dc = new List<DropChance> { };
                List<Ingredient> ingredients = new List<Ingredient> { };
                string itemDesc;

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                var infoTable = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[1]/table/tbody");
                itemDesc = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[1]/p/text()").InnerText;
                var infoTabs = parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[2]/div[1]/table/tbody/tr");

                //If the item has an info table, grab the data from it and store in infoTableStats
                if (infoTable != null)
                {
                    foreach (HtmlNode row in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/div[1]/table/tbody/tr"))
                    {
                        HtmlNodeCollection cells = row.SelectNodes("td");

                        infoTableStats.Add(new ItemInfoRow() { Stat = cells[0].InnerText, Value = cells[1].InnerText});
                    }
                }
                if(infoTable != null)
                {
                    //Gets the drop chance statistics for the item.
                    foreach (var rows in infoTabs)
                    {
                        HtmlNodeCollection cells = rows.SelectNodes("td");

                        dc.Add(new DropChance() { Container = cells[0].GetAttributeValue("data-value", ""), Chance = cells[3].GetAttributeValue("data-value", ""), Amount = cells[2].InnerText });

                    }
                }

                //Gets crafting info for the item.
                foreach (var rows in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[@class=\"tab-block\"]/*"))
                {
                    //If the item has crafting info
                    if(rows.GetAttributeValue("data-name", "") == "craft")
                    {
                        var craftTable = new HtmlDocument();
                        craftTable.LoadHtml(rows.InnerHtml);

                        foreach (var r in craftTable.DocumentNode.SelectNodes("/table/tbody/tr"))
                        {
                            HtmlNodeCollection cells = r.SelectNodes("td");

                            if (cells[1].InnerText.Contains(item.ItemName))
                            {
                                foreach(var ing in cells[2].SelectNodes("a"))
                                {
                                    string ingName = ing.SelectSingleNode("img").GetAttributeValue("alt", "");
                                    string ingAmount = ing.SelectSingleNode("span").InnerText;

                                    ingredients.Add(new Ingredient() { IngredientName = ingName, IngredientAmount = ingAmount });
                                }
                            }

                        }
                    }
                }


                Item item1 = new Item() { ItemName = item.ItemName, Icon = item.Icon, URL = item.URL, DropChances = dc, ItemInfoTable = infoTableStats, Description = itemDesc, Ingredients = ingredients };
                itemCache.Add(item.ItemName, item1);
                ScheduleAction(delegate () { itemCache.Remove(item1.ItemName); }, DateTime.Now.AddHours(12));

                return item1;
            }
        }

        //--------------Breaking information-----------------------------------------------------------------------------------------------

        public static async Task<SearchBreakable> SearchForBreakable(string itemType, string itemName)
        {
            using (WebClient wc = new WebClient())
            {
                string page;

                //Building Blocks
                if (itemType == "block") 
                {
                    if (searchBlockCache == null) { page = await wc.DownloadStringTaskAsync(new Uri("https://rustlabs.com/group=building-blocks")); searchBlockCache = page; ScheduleAction(delegate () { searchBlockCache = null; }, DateTime.Now.AddHours(12)); }
                    else { page = searchBlockCache; }

                    var parsePage = new HtmlDocument();
                    parsePage.LoadHtml(page);

                    foreach(var blockType in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div"))
                    {
                        var typePage = new HtmlDocument();
                        typePage.LoadHtml(blockType.InnerHtml);

                        foreach (var r in typePage.DocumentNode.SelectNodes("/table/tbody/tr"))
                        {
                            HtmlNodeCollection cells = r.SelectNodes("td");

                            if (cells[0].SelectSingleNode("a").InnerText.ToLower() == itemName) 
                            {
                                string blockName = cells[0].SelectSingleNode("a").InnerText;
                                string blockURL = "https://rustlabs.com" + cells[0].SelectSingleNode("a").GetAttributeValue("href", "");

                                return new SearchBreakable() { ItemName = blockName, URL = blockURL};
                            }
                        }
                    }

                }
                //Placeable items
                else 
                {
                    if (searchPlaceCache == null) { page = await wc.DownloadStringTaskAsync(new Uri("https://rustlabs.com/group=itemList")); searchPlaceCache = page; ScheduleAction(delegate () { searchBlockCache = null; }, DateTime.Now.AddHours(12)); }
                    else { page = searchPlaceCache; }

                    var parsePage = new HtmlDocument();
                    parsePage.LoadHtml(page);

                    foreach (var placeable in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div/a"))
                    {
                        var p = new HtmlDocument();
                        p.LoadHtml(placeable.InnerHtml);

                        HtmlNodeCollection cells = p.DocumentNode.SelectNodes("span");

                        if (cells[1].InnerText.ToLower() != itemName) { continue; }

                        string placeableName = cells[1].InnerText;
                        string placeableIcon = cells[0].SelectSingleNode("img").GetAttributeValue("src", "").Replace("//rustlabs.com/", "https://rustlabs.com/");
                        string placeableUrl = "https://rustlabs.com" + placeable.GetAttributeValue("href", "");

                        return new SearchBreakable() { ItemName = placeableName, Icon = placeableIcon, URL = placeableUrl };
                    }
                }

                return null;
            }
        }

        public static async Task<BreakableInfo> GetBreakableInfo(SearchBreakable item, string attackType, string side)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri(item.URL));

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                string itemHP;
                List<AttackDurability> attacksList = new List<AttackDurability> { }; 

                //System to obtain the HP of the item
                HtmlNode hpTable = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[2]/table");

                if (hpTable == null) { return null; }

                var hpParsed = new HtmlDocument();
                hpParsed.LoadHtml(hpTable.InnerText);

                itemHP = hpParsed.DocumentNode.SelectNodes("tbody/tr/td")[1].InnerText;

                //System to obtain durability info
                foreach (var tabs in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[@class=\"tab-block\"]/*"))
                {
                    //If the item has crafting info
                    if (tabs.GetAttributeValue("data-name", "") == "destroyed-by")
                    {
                        var durabilityTab = new HtmlDocument();
                        durabilityTab.LoadHtml(tabs.InnerHtml);

                        foreach(var attack in durabilityTab.DocumentNode.SelectNodes("/table/tbody/tr"))
                        {
                            if(side == null)
                            {
                                if (attack.GetAttributeValue("data-group", "") == attackType)
                                {
                                    HtmlNodeCollection cells = attack.SelectNodes("td");

                                    string wepName = cells[1].GetAttributeValue("data-value", "");
                                    string quantity = cells[2].GetAttributeValue("data-value", "");
                                    string time = cells[3].GetAttributeValue("data-value", "");
                                    string fuel = cells[4].InnerText;
                                    string sulfur = cells[5].InnerText;

                                    attacksList.Add(new AttackDurability() { Tool = wepName, Quantity = quantity, Time = time, Fuel = fuel, Sulfur = sulfur });
                                }
                            }
                            else
                            {
                                if (attack.GetAttributeValue("data-group", "") == attackType && attack.GetAttributeValue("data-group2", "") == side)
                                {
                                    HtmlNodeCollection cells = attack.SelectNodes("td");

                                    string wepName = cells[1].GetAttributeValue("data-value", "");
                                    string quantity = cells[2].GetAttributeValue("data-value", "");
                                    string time = cells[3].GetAttributeValue("data-value", "");
                                    string fuel = cells[4].InnerText;
                                    string sulfur = cells[5].InnerText;

                                    attacksList.Add(new AttackDurability() { Tool = wepName, Quantity = quantity, Time = time, Fuel = fuel, Sulfur = sulfur });
                                }
                            }
                        }
                    }
                }

                //Obtains the item icon
                string iconURL = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[2]/img").GetAttributeValue("src", "").Replace("//rustlabs.com/", "https://rustlabs.com/");
                BreakableInfo bi = new BreakableInfo() { ItemName = item.ItemName, DurabilityInfo = attacksList, HP = itemHP, Icon = iconURL, URL = item.URL };

                breakCache.Add(bi.ItemName, bi);
                ScheduleAction(delegate() { breakCache.Remove(bi.ItemName); }, DateTime.Now.AddHours(12));

                return bi;
            }
        }

        //Item Store Info
        public static async Task<List<ItemStoreItem>> GetItemStore()
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri("https://store.steampowered.com/itemstore/252490/"));

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                List<ItemStoreItem> storeList = new List<ItemStoreItem> { };

                try
                {
                    foreach (var itemRow in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[7]/div[4]/div/div[4]/div/div[1]/a"))
                    {
                        string itemLink = itemRow.GetAttributeValue("href", "");
                        string itemName = itemRow.SelectSingleNode("div/div[2]/div[1]").InnerText;
                        string itemPrice = itemRow.SelectSingleNode("div/div[2]/div[2]").InnerText;
                        string itemIcon = itemRow.SelectSingleNode("div/div[1]/img").GetAttributeValue("src", "");

                        storeList.Add(new ItemStoreItem() { ItemName = itemName, ItemPrice = itemPrice, ItemURL = itemLink, ItemIcon = itemIcon });
                    }
                    return storeList;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        //Skin Price Info
        public static async Task<List<SkinInfo>> GetSkinInfo(string skinName, string orderBy, string orderDirection)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri($"https://rustlabs.com/skins#search={skinName};order={orderBy},{orderDirection}"));

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                List<SkinInfo> skinList = new List<SkinInfo> { };

                foreach (var searchResult in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/a"))
                {
                    if (searchResult.Attributes["style"].Value != "")
                    {
                        string sName = searchResult.GetAttributeValue("data-name", "");
                        string sURL = searchResult.GetAttributeValue("href", "").Replace("//rustlabs.com/", "https://rustlabs.com/");
                        string sPrice = searchResult.GetAttributeValue("data-price", "");
                        string sUsualPrice = searchResult.GetAttributeValue("data-price-2", "");

                        skinList.Add(new SkinInfo() { SkinName = sName, SkinPrice = sPrice, SkinURL = sURL, SkinUsualPrice = sUsualPrice });
                    }
                }
                

                return skinList;
            }
        }

        //Player Info
        public static async Task<Dictionary<string, string>> GetPlayerInfo(string steamID64)
        {
            if (statCache.ContainsKey(steamID64)) { return statCache[steamID64]; }

            using (WebClient wc = new WebClient())
            {
                string page;

                try
                {
                    page = await wc.DownloadStringTaskAsync(new Uri($"http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=252490&key={apiKey}&steamid={steamID64}&include_appinfo=1&format=xml"));
                }
                catch (WebException)
                {
                    return null;
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(page);
                XmlNodeList stats = xmlDoc.SelectNodes("/playerstats/stats/stat");

                Dictionary<string, string> playerStats = new Dictionary<string, string> { };
                statCache.Add(steamID64, playerStats);
                ScheduleAction(delegate () { statCache.Remove(steamID64); }, DateTime.Now.AddHours(2));

                foreach(XmlNode stat in stats)
                {
                    string name = stat.SelectSingleNode("name").InnerText;
                    string value = stat.SelectSingleNode("value").InnerText;

                    playerStats.Add(name, value);
                }

                return playerStats;
            }
        }


        public static Embed GetEmbedMessage(string messageTitle, string fieldTitle, string fieldContents, SocketUser user, Color messageColor, EmbedFooterBuilder fb = null)
        {
            EmbedBuilder eb = new EmbedBuilder();

            if(fb == null && user != null)
            {
                fb = new EmbedFooterBuilder();
                fb.WithText($"Called by {user.Username}");
                fb.WithIconUrl(user.GetAvatarUrl());
            }

            eb.WithTitle($"{messageTitle}");
            eb.AddField($"{fieldTitle}", $"{fieldContents}");
            eb.WithColor(messageColor);
            eb.WithFooter(fb);

            return eb.Build();
        }

        public static EmbedFooterBuilder GetFooter(SocketUser user, Stopwatch sw)
        {
            EmbedFooterBuilder fb = new EmbedFooterBuilder();

            fb.WithText($"Called by {user.Username} | Completed in {sw.ElapsedMilliseconds}ms");
            fb.WithIconUrl(user.GetAvatarUrl());

            return fb;
        }

        private static Random randomStr = new Random(DateTime.Now.Millisecond);
        //Generates a random string from the characters in the variable chars
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[randomStr.Next(s.Length)]).ToArray());
        }

        public static string GetNumbers(string text)
        {
            text = text ?? string.Empty;
            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        //Schedule a task for a certain time
        public static async void ScheduleAction(Action action, DateTime ExecutionTime)
        {
            await Task.Delay(ExecutionTime.Subtract(DateTime.Now));
            action();
        }

        public static async Task<List<ServerInfo>> GetServers()
        {
            using (WebClient wc = new WebClient())
            {
                List<ServerInfo> serverInfo = new List<ServerInfo>() { };
                string page = await wc.DownloadStringTaskAsync(new Uri("https://api.battlemetrics.com/servers?filter[game]=rust&page[size]=5&sort=rank"));

                dynamic json = JsonConvert.DeserializeObject(page);
                
                /*
                foreach(var server in json.data)
                {
                    Console.WriteLine(server.attributes["name"]);
                }
                */
                
                foreach(var server in json.data)
                {
                    ServerInfo si = new ServerInfo()
                    {
                        ServerName = server.attributes["name"],
                        IP = server.attributes["ip"],
                        Port = server.attributes["port"],
                        MaxPlayers = server.attributes["maxPlayers"],
                        CurrentPlayers = server.attributes["players"],
                        ServerType = server.attributes["details"]["rust_type"],
                        Map = server.attributes["details"]["map"],
                        MapSize = server.attributes["details"]["rust_world_size"],
                        LastWipe = server.attributes["details"]["rust_last_wipe"],
                        Rank = server.attributes["rank"]
                    };

                    serverInfo.Add(si);
                }
                

                return serverInfo;
            }
        }

        public static async Task<ServerInfo> GetServer(string searchQuery)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri($"https://api.battlemetrics.com/servers?filter[search]=%22{HttpUtility.UrlEncode(searchQuery)}%22&filter[game]=rust&sort=rank"));

                dynamic json = JsonConvert.DeserializeObject(page);

                var server = json.data[0];

                ServerInfo si = new ServerInfo()
                {
                    ServerName = server.attributes["name"],
                    IP = server.attributes["ip"],
                    Port = server.attributes["port"],
                    MaxPlayers = server.attributes["maxPlayers"],
                    CurrentPlayers = server.attributes["players"],
                    ServerType = server.attributes["details"]["rust_type"],
                    Map = server.attributes["details"]["map"],
                    MapSize = server.attributes["details"]["rust_world_size"],
                    LastWipe = server.attributes["details"]["rust_last_wipe"],
                    Rank = server.attributes["rank"]
                };

                return si;
            }
        }
    }

    public class ServerInfo
    {
        public string ServerName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public string ServerType { get; set;}
        public string Map { get; set; }
        public int MapSize { get; set; }
        public string LastWipe { get; set; }
        public int Rank { get; set; }
    }

    public class PlayerStat
    {
        public string Name { get; set; }

        public string Value { get; set; } = "0";
    }

    public class SkinInfo
    {
        public string SkinName { get; set; }
        public string SkinURL { get; set; }
        public string SkinPrice { get; set; }
        public string SkinUsualPrice { get; set; }
    }

    public class ItemStoreItem
    {
        public string ItemName { get; set; }
        public string ItemURL { get; set; }
        public string ItemPrice { get; set; }
        public string ItemIcon { get; set; }
    }

    public class Item
    {
        public string ItemName { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public List<ItemInfoRow> ItemInfoTable { get; set; }
        public List<DropChance> DropChances { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public string IngredientName { get; set; }
        public string IngredientAmount { get; set; }
    }

    public class SearchItem
    {
        public string ItemName { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
    }

    public class ItemInfoRow
    {
        public string Stat { get; set; }
        public string Value { get; set; }
    }

    public class DropChance
    {
        public string Container { get; set; }
        public string Amount { get; set; }
        public string Chance { get; set; }
    }

    public class SearchBreakable
    {
        public string ItemName { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
    }

    public class BreakableInfo
    {
        public string ItemName { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public string HP { get; set; }
        public List<AttackDurability> DurabilityInfo { get; set; }
    }

    public class AttackDurability
    {
        public string Tool { get; set; }
        public string Quantity { get; set; }
        public string Time { get; set; }
        public string Fuel { get; set; }
        public string Sulfur { get; set; }
    }
}

public static class DictExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>
(this IDictionary<TKey, TValue> dictionary,
 TKey key,
 TValue defaultValue)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }
}
