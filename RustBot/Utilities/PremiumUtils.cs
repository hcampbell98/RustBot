using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RustBot
{
    class PremiumUtils
    {
        public static async Task<PremiumRank> VerifyPremium(string transId, ulong discordId)
        {
            using (WebClient wc = new WebClient())
            {
                string page = await wc.DownloadStringTaskAsync(new Uri($"https://nickgor.com/rustbot/donations/CheckDonated.php?id={transId}"));

                if (page == "null") { return null; }
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
                default:
                    return null;
            }
        }



    }

    public  class PremiumRank
    {

    }

    public class Cloth : PremiumRank
    {
        public static string Name = "Cloth";
        public const string ProductId = "et9AV5dKPN";
        public static int PermLevel = 1;
        public static List<SocketUser> Members;
    }

    public class Wooden : PremiumRank
    {
        public static string Name = "Wooden";
        public const string ProductId = "temp";
        public static int PermLevel = 2;
        public static List<SocketUser> Members;
    }
}
