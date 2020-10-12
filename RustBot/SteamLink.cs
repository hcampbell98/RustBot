using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RustBot
{
    class SteamLink
    {
        public static Dictionary<string, string> idCache = new Dictionary<string, string>{};

        public static dynamic GetSteam(string id)
        {
            if (idCache.ContainsKey(id)) { return idCache[id]; }

            try
            {
                using (WebClient wc = new WebClient())
                {
                    dynamic json = JsonConvert.DeserializeObject(wc.DownloadString($"https://bunnyslippers.dev/scripts/get_link.php?discordID={id}"));

                    idCache.Add(id, Convert.ToString(json.steamID));
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
