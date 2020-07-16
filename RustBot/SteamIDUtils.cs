using System;
using System.Net;
using System.Xml;
using Newtonsoft.Json;

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

        public static string GetName(string steamID64)
        {
            try
            {
                WebClient wc = new WebClient();
                string xml = wc.DownloadString($"http://steamcommunity.com/profiles/{steamID64}/?xml=1");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nodeList = doc.SelectNodes("/profile");

                return nodeList[0].SelectSingleNode("steamID").InnerText;
                

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
