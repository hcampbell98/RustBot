using SSRPBalanceBot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace RustBot.Users.Teams
{
    class TeamUtils
    {
        public static List<Team> teamData;
        public static List<UserSettings> userSettings;
        public static Dictionary<ulong, Team> pendingInvites = new Dictionary<ulong, Team> { };

        public static bool CreateTeam(ulong teamLeader, ulong[] teamMembers, IRole role, SocketGuild guild, bool notifications = true, string baseCoords = "Not yet set.")
        {
            Team t = new Team() { TeamLeader = teamLeader, Members = teamMembers, Notifications = notifications, BaseCoords = baseCoords, RoleID = role.Id, GuildID = guild.Id};

            if (CheckIfInTeam(teamLeader)) { return false; }
            foreach (var member in teamMembers) { if (CheckIfInTeam(member)) { return false; } }

            teamData.Add(t);
            Utilities.WriteToJsonFile<Team>($"Users/Teams/{teamLeader}.json", t);
            return true;
        }

        public static bool LeaveTeam(ulong discordID)
        {
            if (File.Exists($"Users/Teams/{discordID}.json"))
            {
                File.Delete($"Users/Teams/{discordID}.json");
                Team team = teamData.First(x => x.TeamLeader == discordID);

                Program._client.GetGuild(team.GuildID).GetRole(team.RoleID).DeleteAsync();

                teamData.Remove(team);
            }
            else
            {
                Team team = teamData.FirstOrDefault(x => x.Members.Contains(discordID));

                if (team == null) { return false; }
                else
                {
                    Program._client.GetGuild(team.GuildID).GetUser(discordID).RemoveRoleAsync(Program._client.GetGuild(team.GuildID).GetRole(team.RoleID));

                    ulong[] teamMembers = team.Members.Where((source, index) => index != Array.FindIndex(team.Members, row => row == discordID)).ToArray();
                    teamData.Remove(team);
                    team.Members = teamMembers;
                    teamData.Add(team);

                    File.Delete($"Users/Teams/{team.TeamLeader}.json");
                    Utilities.WriteToJsonFile<Team>($"Users/Teams/{team.TeamLeader}.json", team);
                }
            }

            return true;
        }

        public static List<Team> LoadTeams()
        {
            List<Team> teams = new List<Team> { };
            if (!Directory.Exists("Users/Teams")) { Directory.CreateDirectory("Users/Teams"); }

            foreach (var team in Directory.GetFiles("Users/Teams"))
            {
                teams.Add(Utilities.ReadFromJsonFile<Team>(File.ReadAllText(team)));
            }

            return teams;
        }

        public static async void AddToTeam(Team team, ulong newMember)
        {
            List<ulong> memberList = new List<ulong> { };

            foreach(ulong member in team.Members)
            {
                memberList.Add(member);
            }

            memberList.Add(newMember);
            Team updatedTeam = team;
            updatedTeam.Members = memberList.ToArray();

            teamData.Remove(team);
            teamData.Add(updatedTeam);

            await Program._client.GetGuild(updatedTeam.GuildID).GetUser(newMember).AddRoleAsync(Program._client.GetGuild(updatedTeam.GuildID).GetRole(updatedTeam.RoleID));

            if (File.Exists($"Users/Teams/{updatedTeam.TeamLeader}.json")) { File.Delete($"Users/Teams/{updatedTeam.TeamLeader}.json"); }
            Utilities.WriteToJsonFile<Team>($"Users/Teams/{updatedTeam.TeamLeader}.json", updatedTeam);
        }

        public static void SetServer(Team team, ServerInfo server)
        {
            Team updatedTeam = team;
            updatedTeam.Server = server;

            teamData.Remove(team);
            teamData.Add(updatedTeam);

            if (File.Exists($"Users/Teams/{updatedTeam.TeamLeader}.json")) { File.Delete($"Users/Teams/{updatedTeam.TeamLeader}.json"); }
            Utilities.WriteToJsonFile<Team>($"Users/Teams/{updatedTeam.TeamLeader}.json", updatedTeam);
        }

        public static List<UserSettings> LoadSettings()
        {
            List<UserSettings> users = new List<UserSettings> { };
            if (!Directory.Exists("Users/Teams/UserSettings")) { Directory.CreateDirectory("Users/Teams/UserSettings"); }

            foreach (var user in Directory.GetFiles("Users/Teams/UserSettings"))
            {
                users.Add(Utilities.ReadFromJsonFile<UserSettings>(File.ReadAllText(user)));
            }

            return users;
        }

        public static void UpdateSettings(ulong discordID, bool? notifications = null, bool? invites = null)
        {
            foreach(UserSettings u in userSettings)
            {
                if (u.DiscordID == discordID) 
                {
                    if (notifications == null) { notifications = u.NotificationsEnabled; }
                    else if (invites == null) { invites = u.InvitesEnabled; }

                    userSettings.Remove(u); 
                    break; 
                }
            }

            if (notifications == null) { notifications = true; }
            else if (invites == null) { invites = true; }

            UserSettings s = new UserSettings() { DiscordID = discordID, InvitesEnabled = (bool)invites, NotificationsEnabled = (bool)notifications };

            userSettings.Add(s);
            if (File.Exists($"Users/Teams/UserSettings/{discordID}.json")) { File.Delete($"Users/Teams/UserSettings/{discordID}.json"); }
            Utilities.WriteToJsonFile<UserSettings>($"Users/Teams/UserSettings/{discordID}.json", s);
        }


        public static bool CheckIfInTeam(ulong discordID)
        {
            foreach(Team team in teamData)
            {
                if (team.TeamLeader == discordID) { return true; }

                foreach(var member in team.Members)
                {
                    if (member == discordID) { return true; }
                }
            }
            return false;
        }

        public static Team GetTeam(ulong discordID)
        {
            foreach (Team team in teamData)
            {
                if (team.TeamLeader == discordID) { return team; }

                foreach (var member in team.Members)
                {
                    if (member == discordID) { return team; }
                }
            }
            return null;
        }

        public static UserSettings GetSettings(ulong discordID)
        {
            UserSettings u = userSettings.FirstOrDefault(x => x.DiscordID == discordID);

            if(u == default(UserSettings))
            {
                return new UserSettings() { NotificationsEnabled = true, InvitesEnabled = true, DiscordID = discordID};
            }
            else
            {
                return u;
            }
        }
    }

    public class Team
    {
        public ulong TeamLeader { get; set; }
        public ulong[] Members { get; set; }
        public ulong RoleID { get; set; }
        public ulong GuildID { get; set; }
        public bool Notifications { get; set; }
        public string BaseCoords { get; set; }
        public ServerInfo Server { get; set; }

    }

    public class UserSettings
    {
        public ulong DiscordID { get; set; }
        public bool NotificationsEnabled { get; set; }
        public bool InvitesEnabled { get; set; }

    }
}
