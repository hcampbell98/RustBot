using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Text.RegularExpressions;
using System.Linq;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;

namespace SSRPBalanceBot
{
    class Utilities
    {
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
                        if (row.GetAttributeValue("data-group2", "") == "skins") { continue; }

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
                string page = await wc.DownloadStringTaskAsync(new Uri(item.URL + "#sort=3,1,0"));

                List<ItemInfoRow> infoTableStats = new List<ItemInfoRow> { };
                List<DropChance> dc = new List<DropChance> { };
                List<Ingredient> ingredients = new List<Ingredient> { };
                string itemDesc;

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                var infoTable = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[1]/table/tbody");
                itemDesc = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[1]/p/text()").InnerText;

                //If the item has an info table, grab the data from it and store in infoTableStats
                if(infoTable != null)
                {
                    foreach (HtmlNode row in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[1]/div[1]/table/tbody/tr"))
                    {
                        HtmlNodeCollection cells = row.SelectNodes("td");

                        infoTableStats.Add(new ItemInfoRow() { Stat = cells[0].InnerText, Value = cells[1].InnerText});
                    }
                }

                //Gets the drop chance statistics for the item.
                foreach(var rows in parsePage.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[2]/div[1]/table/tbody/tr"))
                {
                    HtmlNodeCollection cells = rows.SelectNodes("td");

                    dc.Add(new DropChance() { Container = cells[0].GetAttributeValue("data-value", ""), Chance = cells[3].GetAttributeValue("data-value", ""), Amount = cells[2].InnerText });
                    
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
                    page = await wc.DownloadStringTaskAsync(new Uri("https://rustlabs.com/group=building-blocks"));

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
                    page = await wc.DownloadStringTaskAsync(new Uri("https://rustlabs.com/group=build"));

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

        public static async Task<BreakableInfo> GetBreakableInfo(SearchBreakable item, string attackType)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri(item.URL));

                var parsePage = new HtmlDocument();
                parsePage.LoadHtml(page);

                string itemHP;
                List<AttackDurability> attacksList = new List<AttackDurability> { }; 

                //System to obtain the HP of the item
                string hpTable = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[2]/table").InnerHtml;

                var hpParsed = new HtmlDocument();
                hpParsed.LoadHtml(hpTable);

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

                            if(attack.GetAttributeValue("data-group", "") == attackType)
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

                //Obtains the item icon
                string iconURL = parsePage.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[1]/div[2]/img").GetAttributeValue("src", "").Replace("//rustlabs.com/", "https://rustlabs.com/");

                return new BreakableInfo() { ItemName = item.ItemName, DurabilityInfo = attacksList, HP = itemHP, Icon = iconURL, URL = item.URL};
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
                    if (IsNodeVisible(searchResult))
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


        public static Embed GetEmbedMessage(string messageTitle, string fieldTitle, string fieldContents, SocketUser user, Color messageColor)
        {
            EmbedBuilder eb = new EmbedBuilder();
            EmbedFooterBuilder fb = new EmbedFooterBuilder();

            fb.WithText($"Called by {user.Username}");
            fb.WithIconUrl(user.GetAvatarUrl());

            eb.WithTitle($"{messageTitle}");
            eb.AddField($"{fieldTitle}", $"{fieldContents}");
            eb.WithColor(messageColor);
            eb.WithFooter(fb);

            return eb.Build();
        }

        public static Task StatusMessage(string cmd, SocketCommandContext Context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Time: {DateTime.Now} | Ran command: [{cmd}] | Called by: {Context.Message.Author} | Server: {Context.Guild.Name}");
            Console.ForegroundColor = ConsoleColor.Gray;
            return Task.CompletedTask;
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

        //Check node visibility
        private static bool IsNodeVisible(HtmlAgilityPack.HtmlNode node)
        {
            var attribute = node.Attributes["style"];

            bool thisVisible = false;

            if (attribute == null || CheckStyleVisibility(attribute.Value))
                thisVisible = true;

            if (thisVisible && node.ParentNode != null)
                return IsNodeVisible(node.ParentNode);

            return thisVisible;
        }

        private static bool CheckStyleVisibility(string style)
        {
            if (string.IsNullOrWhiteSpace(style))
                return true;

            var keys = ParseHtmlStyleString(style);

            if (keys.Keys.Contains("display"))
            {
                string display = keys["display"];
                if (display != null && display == "none;")
                    return false;
            }

            if (keys.Keys.Contains("visibility"))
            {
                string visibility = keys["visibility"];
                if (visibility != null && visibility == "hidden")
                    return false;
            }

            return true;
        }

        public static Dictionary<string, string> ParseHtmlStyleString(string style)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            style = style.Replace(" ", "").ToLowerInvariant();

            string[] settings = style.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in settings)
            {
                if (!s.Contains(':'))
                    continue;
                string[] data = s.Split(':');
                result.Add(data[0], data[1]);
            }

            return result;
        }

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
