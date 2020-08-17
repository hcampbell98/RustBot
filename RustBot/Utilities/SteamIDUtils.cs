using System;
using System.Dynamic;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;

namespace SSRPBalanceBot
{
    public class SteamIDUtils
    {

        public static string RetrieveID(string input)
        {
            if (input.StartsWith("STEAM"))
            {
                return SteamIDConvert.Steam2ToSteam64(input).ToString();
            }
            else if (input.StartsWith("7656119"))
            {
                return input;
            }
            return null;

        }

        public static async Task<ProfileInfo> GetProfileInfo(string steamID64)
        {
            if (Utilities.profileInfoCache.ContainsKey(steamID64)) { return Utilities.profileInfoCache[steamID64]; }

            try
            {
                WebClient wc = new WebClient();
                string xml = await wc.DownloadStringTaskAsync(new Uri($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={Utilities.apiKey}&format=xml&steamids={steamID64}"));
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nodeList = doc.SelectNodes("/response/players/player");

                string profileName = nodeList[0].SelectSingleNode("personaname").InnerText;
                string profileurl = nodeList[0].SelectSingleNode("profileurl").InnerText;
                string avatarsmall = nodeList[0].SelectSingleNode("avatar").InnerText;
                string avatarmedium = nodeList[0].SelectSingleNode("avatarmedium").InnerText;
                string avatarlarge = nodeList[0].SelectSingleNode("avatarfull").InnerText;
                string profilecreated = nodeList[0].SelectSingleNode("timecreated").InnerText;

                ProfileInfo pi = new ProfileInfo() { SteamID64 = steamID64, ProfileName = profileName, ProfileURL = profileurl, AvatarSmall = avatarsmall, AvatarMedium = avatarmedium, AvatarLarge = avatarlarge, ProfileCreated = profilecreated };
                Utilities.profileInfoCache.Add(steamID64, pi);
                Utilities.ScheduleAction(delegate () { Utilities.profileInfoCache.Remove(steamID64); }, DateTime.Now.AddHours(2));

                return pi;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> GetHoursPlayed(string steamID64)
        {
            using (WebClient wc = new WebClient())
            {
                dynamic j = JsonConvert.DeserializeObject(await wc.DownloadStringTaskAsync($"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={Utilities.apiKey}&steamid={steamID64}&format=json"));

                foreach(var game in j.response.games)
                {
                    if(game.appid == "252490")
                    {
                        return Convert.ToString(game.playtime_forever);
                    }
                }

                return null;
            }
        }
    }

    public class ProfileInfo
    {
        public string SteamID64 { get; set; }
        public string ProfileName { get; set; }
        public string ProfileURL { get; set; }
        public string AvatarSmall { get; set; }
        public string AvatarMedium { get; set; }
        public string AvatarLarge { get; set; }
        public string ProfileCreated { get; set; }
    }
}
