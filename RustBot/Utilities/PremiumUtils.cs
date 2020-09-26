using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using RustBot;

namespace RustBot
{
    class PremiumUtils
    {
        //Verifies a Premium purchase and returns the rank purchased
        public static async Task<PremiumRank> VerifyPremium(string transId, ulong discordId)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri($"https://nickgor.com/rustbot/donations/CheckDonated.php?id={transId}"));

                if (page == "null\n") { return null; }
                else
                {
                    dynamic json = JsonConvert.DeserializeObject(page);

                    if (json.buyer_id == discordId.ToString())
                    {
                        return GetPremiumRank(json);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static PremiumRank GetPremiumRank(dynamic json)
        {
            string prodId = json.product_id;

            switch (prodId)
            {
                case Cloth.ProductId:
                    return new Cloth();
                case Wooden.ProductId:
                    return new Wooden();
                case HighQuality.ProductId:
                    return new HighQuality();
                default:
                    return null;
            }
        }

        public static bool AssignPremiumRank(SocketUser u, PremiumRank rank)
        {
            if((object)rank is Cloth)
            {
                if (Cloth.Members.FirstOrDefault(x => x.DiscordID == u.Id) == default(PremiumUser)) { Cloth.Members.Add(new PremiumUser() { Name = u.Username, DiscordID = u.Id}); SavePremiumRank(rank); }
                
                return true;
            }
            else if((object)rank is Wooden)
            {
                if (Wooden.Members.FirstOrDefault(x => x.DiscordID == u.Id) == default(PremiumUser)) { Wooden.Members.Add(new PremiumUser() { Name = u.Username, DiscordID = u.Id }); SavePremiumRank(rank); }

                return true;
            }
            else if ((object)rank is HighQuality)
            {
                if (HighQuality.Members.FirstOrDefault(x => x.DiscordID == u.Id) == default(PremiumUser)) { HighQuality.Members.Add(new PremiumUser() { Name = u.Username, DiscordID = u.Id }); SavePremiumRank(rank); }

                return true;
            }
            return false;
        }

        public static void SavePremiumRank(PremiumRank rank)
        {

            if ((object)rank is Cloth)
            {
                File.Delete("Users/Premium/cloth.json");
                Utilities.WriteToJsonFile("Users/Premium/cloth.json", Cloth.Members);
            }
            else if ((object)rank is Wooden)
            {
                File.Delete("Users/Premium/wooden.json");
                Utilities.WriteToJsonFile("Users/Premium/wooden.json", Wooden.Members);
            }
            else if ((object)rank is HighQuality)
            {
                File.Delete("Users/Premium/highqual.json");
                Utilities.WriteToJsonFile("Users/Premium/highqual.json", HighQuality.Members);
            }
        }

        public static void LoadRanks()
        {
            if (!File.Exists("Users/Premium/cloth.json")) { File.Create("Users/Premium/cloth.json"); }
            if (!File.Exists("Users/Premium/wooden.json")) { File.Create("Users/Premium/wooden.json"); }
            if (!File.Exists("Users/Premium/highqual.json")) { File.Create("Users/Premium/highqual.json"); }

            string clothFile = File.ReadAllText("Users/Premium/cloth.json");
            string woodenFile = File.ReadAllText("Users/Premium/wooden.json");
            string highqualFile = File.ReadAllText("Users/Premium/highqual.json");

            //Cloth rank
            if (clothFile == "") { Cloth.Members = new List<PremiumUser> { }; }
            else { Cloth.Members = Utilities.ReadFromJsonFile<List<PremiumUser>>(clothFile); }

            //Wooden rank
            if (woodenFile == "") { Wooden.Members = new List<PremiumUser> { }; }
            else { Wooden.Members = Utilities.ReadFromJsonFile<List<PremiumUser>>(woodenFile); }

            //High Quality rank
            if (highqualFile == "") { HighQuality.Members = new List<PremiumUser> { }; }
            else { HighQuality.Members = Utilities.ReadFromJsonFile<List<PremiumUser>>(highqualFile); }
        }

        public static Discord.Color GetRandomColour()
        {
            var random = new Random();
            var hex = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
            System.Drawing.Color hexColour = ColorTranslator.FromHtml(hex);

            Discord.Color c = new Discord.Color(hexColour.R, hexColour.G, hexColour.B);

            return c;
        }

        public static Discord.Color SelectEmbedColour(SocketUser u)
        {
            if(u.GetPremiumRank() == null)
            {
                return Discord.Color.Red;
            }
            else
            {
                return GetRandomColour();
            }
        }

        public static string SelectFooterEmbedText(SocketUser u, Stopwatch sw = null)
        {
            if(u.GetPremiumRank() == null)
            {
                if(sw == null)
                {
                    return $"Called by {u.Username}";
                }
                else
                {
                    return $"Called by {u.Username} | Completed in {sw.ElapsedMilliseconds}ms";
                }
            }
            else
            {
                if(sw == null)
                {
                    return $"Called by Premium User {u.Username}";
                }
                else
                {
                    return $"Called by Premium User {u.Username} | Completed in {sw.ElapsedMilliseconds}ms";
                }
            }
        }

    }

    public static class PremiumExtensions
    {
        public static PremiumRank GetPremiumRank(this SocketUser u)
        {
            if (Cloth.Members.FirstOrDefault(x => x.DiscordID == u.Id) != default(PremiumUser)) { return new Cloth(); }
            else if (Wooden.Members.FirstOrDefault(x => x.DiscordID == u.Id) != default(PremiumUser)) { return new Wooden(); }
            else if (HighQuality.Members.FirstOrDefault(x => x.DiscordID == u.Id) != default(PremiumUser)) { return new HighQuality(); }
            else
            {
                return null;
            }
        }
    }

    public class PremiumUser
    {
        public string Name { get; set; }
        public ulong DiscordID { get; set; }
    }

    public class PremiumRank
    {
        public string Name;
        public string ProductId;
        public int PermLevel;
        public List<PremiumUser> Members;
    }

    public class Cloth : PremiumRank
    {
        public new string Name = "Cloth";
        public new const string ProductId = "et9AV5dKPN";
        public new static int PermLevel = 1;
        public new static List<PremiumUser> Members { get; set; } = new List<PremiumUser> { };
    }

    public class Wooden : PremiumRank
    {
        public new string Name = "Wooden";
        public new const string ProductId = "temp";
        public new int PermLevel = 2;
        public new static List<PremiumUser> Members { get; set; } = new List<PremiumUser> { };
    }

    public class HighQuality : PremiumRank
    {
        public new string Name = "High Quality";
        public new const string ProductId = "lTODj5wpwd";
        public new int PermLevel = 5;
        public new static List<PremiumUser> Members { get; set; } = new List<PremiumUser> { };
    }
}
