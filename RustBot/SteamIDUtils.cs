using System;
using System.Dynamic;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;

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

        public static ProfileInfo GetProfileInfo(string steamID64)
        {
            try
            {
                WebClient wc = new WebClient();
                string xml = wc.DownloadString($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={Utilities.apiKey}&format=xml&steamids={steamID64}");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nodeList = doc.SelectNodes("/response/players/player");

                string profileName = nodeList[0].SelectSingleNode("personaname").InnerText;
                string profileurl = nodeList[0].SelectSingleNode("profileurl").InnerText;
                string avatarsmall = nodeList[0].SelectSingleNode("avatar").InnerText;
                string avatarmedium = nodeList[0].SelectSingleNode("avatarmedium").InnerText;
                string avatarlarge = nodeList[0].SelectSingleNode("avatarfull").InnerText;
                string profilecreated = nodeList[0].SelectSingleNode("timecreated").InnerText;

                return new ProfileInfo() { SteamID64 = steamID64, ProfileName = profileName, ProfileURL = profileurl, AvatarSmall = avatarsmall, AvatarMedium = avatarmedium, AvatarLarge = avatarlarge, ProfileCreated = profilecreated };
            }
            catch (Exception e)
            {
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
