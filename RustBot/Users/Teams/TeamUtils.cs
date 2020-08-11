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
        public static List<Team> teamData = LoadTeams();

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

        static List<Team> LoadTeams()
        {
            List<Team> teams = new List<Team> { };

            foreach(var team in Directory.GetFiles("Users/Teams"))
            {
                teams.Add(Utilities.ReadFromJsonFile<Team>(File.ReadAllText(team)));
            }

            return teams;
        }

        static bool CheckIfInTeam(ulong discordID)
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
    }

    public class Team
    {
        public ulong TeamLeader { get; set; }
        public ulong[] Members { get; set; }
        public ulong RoleID { get; set; }
        public ulong GuildID { get; set; }
        public bool Notifications { get; set; }
        public string BaseCoords { get; set; }

    }
}
