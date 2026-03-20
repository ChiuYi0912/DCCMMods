using System.Collections.Generic;
using dc.en;
using dc.tool;
using EnemiesVsEnemies.Configuration;
using Hashlink.Marshaling;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace EnemiesVsEnemies.Core
{
    public class TeamManager
    {
        private readonly Dictionary<string, Team> teams = new Dictionary<string, Team>();
        private readonly Dictionary<string, nint> originalTeamPointers = new Dictionary<string, nint>();
        private ModConfig GetModConfig = null!;

        public void Initialize(ModConfig config)
        {
            GetModConfig = config;

            teams.Clear();
            originalTeamPointers.Clear();

            foreach (var teamConfig in config.Teams.Values)
            {
                CreateTeam(teamConfig);
            }

            SetupTeamRelationships();
        }

        public Team GetTeam(string teamId)
        {
            if (teams.TryGetValue(teamId, out var team))
            {
                return team;
            }
            throw new KeyNotFoundException($"队伍 '{teamId}' 不存在");
        }

        public bool TryGetTeam(string teamId, out Team team)
        {
            return teams.TryGetValue(teamId, out team!);
        }

        public IEnumerable<Team> GetAllTeams()
        {
            return teams.Values;
        }

        public IEnumerable<string> GetTeamIds()
        {
            return teams.Keys;
        }

        public void AddTeam(TeamConfig teamConfig)
        {
            if (teams.ContainsKey(teamConfig.Id))
            {
                throw new System.ArgumentException($"队伍 '{teamConfig.Id}' 已存在");
            }

            CreateTeam(teamConfig);
            GetModConfig.Teams[teamConfig.Id] = teamConfig;
        }

        public void RemoveTeam(string teamId)
        {
            if (!teams.ContainsKey(teamId))
            {
                throw new KeyNotFoundException($"队伍 '{teamId}' 不存在");
            }

            var teamToRemove = teams[teamId];
            foreach (var otherTeam in teams.Values)
            {
                if (otherTeam != teamToRemove)
                {
                    otherTeam.opposingTeams.remove(teamToRemove);
                }
            }

            teams.Remove(teamId);
            originalTeamPointers.Remove(teamId);
            GetModConfig.Teams.Remove(teamId);
        }

        //来自(HkLab)
        private void CreateTeam(TeamConfig teamConfig)
        {
            var team = new Team();

            team.opposingTeams = (dc.hl.types.ArrayObj)ArrayUtils.CreateDyn().array;

            originalTeamPointers[teamConfig.Id] = team.opposingTeams.HashlinkPointer;

            team.opposingTeams.HashlinkObj.MarkStateful();

            teams[teamConfig.Id] = team;
        }

        //(仇恨配置)
        private void SetupTeamRelationships()
        {
            foreach (var teamConfig in GetModConfig.Teams.Values)
            {
                var team = teams[teamConfig.Id];

                for (int i = 0; i < team.opposingTeams.length; i++)
                    team.opposingTeams.remove(i);

                foreach (var opposingTeamId in teamConfig.OpposingTeamIds)
                {
                    if (teams.TryGetValue(opposingTeamId, out var opposingTeam))
                    {
                        team.opposingTeams.push(opposingTeam);
                    }
                }
            }
        }

        public bool ValidateTeamPointers()
        {
            foreach (var kvp in originalTeamPointers)
            {
                var teamId = kvp.Key;
                var originalPointer = kvp.Value;
                var team = teams[teamId];

                if (team.opposingTeams.HashlinkPointer != originalPointer)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Team> GetOpposingTeams(string teamId)
        {
            var team = GetTeam(teamId);
            var opposingTeams = new List<Team>();

            for (int i = 0; i < team.opposingTeams.length; i++)
            {
                var opposingTeam = team.opposingTeams.getDyn(i) as Team;
                if (opposingTeam != null)
                {
                    opposingTeams.Add(opposingTeam);
                }
            }
            
            return opposingTeams;
        }

        public bool AreTeamsOpposing(string teamId1, string teamId2)
        {
            if (!teams.ContainsKey(teamId1) || !teams.ContainsKey(teamId2))
            {
                return false;
            }

            var team1 = teams[teamId1];
            var team2 = teams[teamId2];

            for (int i = 0; i < team1.opposingTeams.length; i++)
            {
                var opposingTeam = team1.opposingTeams.getDyn(i) as Team;
                if (opposingTeam == team2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}