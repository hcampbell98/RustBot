using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RustBot
{
    class SteamLink
    {
        public static dynamic GetSteam(string id)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    dynamic json = JsonConvert.DeserializeObject(wc.DownloadString($"https://nickgor.com/scripts/get_link.php?discordID={id}"));
                    return json.steamID;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
