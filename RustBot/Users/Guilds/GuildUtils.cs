using RustBot.Users.Teams;
using SSRPBalanceBot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RustBot.Users.Guilds
{
    class GuildUtils
    {
        public static List<GuildSettings> guildData = new List<GuildSettings> { };

        public static void UpdateSettings(ulong guildID, bool search = true)
        {
            GuildSettings s = new GuildSettings() { GuildID = guildID, ServerSearch = search};

            guildData.Add(s);
            if (File.Exists($"Users/Teams/UserSettings/{guildID}.json")) { File.Delete($"Users/Guilds/{guildID}.json"); }
            Utilities.WriteToJsonFile<GuildSettings>($"Users/Guilds/{guildID}.json", s);
        }

        public static GuildSettings GetSettings(ulong guildID)
        {
            GuildSettings u = guildData.FirstOrDefault(x => x.GuildID == guildID);

            if (u == default(GuildSettings))
            {
                return new GuildSettings() { ServerSearch = true, GuildID = guildID};
            }
            else
            {
                return u;
            }
        }

        public static List<GuildSettings> LoadSettings()
        {
            List<GuildSettings> users = new List<GuildSettings> { };
            if (!Directory.Exists("Users/Guilds")) { Directory.CreateDirectory("Users/Guilds"); }

            foreach (var user in Directory.GetFiles("Users/Guilds"))
            {
                users.Add(Utilities.ReadFromJsonFile<GuildSettings>(File.ReadAllText(user)));
            }

            return users;
        }
    }

    public class GuildSettings
    {
        public ulong GuildID { get; set; }
        public bool ServerSearch { get; set; }
    }
}
