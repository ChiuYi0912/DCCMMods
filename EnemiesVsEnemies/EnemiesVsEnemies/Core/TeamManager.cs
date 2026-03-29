using System.Collections.Generic;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.en;
using dc.hl.types;
using dc.tool;
using EnemiesVsEnemies.Configuration;
using Hashlink.Marshaling;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;

namespace EnemiesVsEnemies.Core
{
    public class TeamManager
    {
        private readonly Dictionary<string, Team> teams = new();
        private readonly Dictionary<string, nint> originalTeamPointers = new();
        private ModConfig GetModConfig = null!;
        private ILogger logger = EnemiesVsEnemiesMod.GetLogger;

        public void Initialize(ModConfig config)
        {
            GetModConfig = config;
            
            teams.Clear();
            originalTeamPointers.Clear();

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
                #if DEBUG
                EnemiesVsEnemiesMod.GetLogger.Error($"队伍 '{teamConfig.Id}' 已存在");
                #endif
                return;
            }

            CreateTeam(teamConfig);
            GetModConfig.Teams[teamConfig.Id] = teamConfig;

            #if DEBUG
            EnemiesVsEnemiesMod.GetLogger.Information($"ADD Team:{teamConfig.Id}");
            #endif
        }

        public void RemoveTeam(string teamId)
        {
            if (!teams.ContainsKey(teamId))
            {
                return;
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

            #if DEBUG
            EnemiesVsEnemiesMod.GetLogger.Information($"Remove Team:{teamId}");
            #endif
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
        public void SetupTeamRelationships()
        {
            foreach (var teamConfig in GetModConfig.Teams.Values)
            {
                if (!teams.TryGetValue(teamConfig.Id, out var team))
                    continue;

                var array = team.opposingTeams;
                while (array.length > 0)
                {
                    array.pop();
                }

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

        public void AddOpposingTeam(string sourceTeamId, string targetTeamId, bool bidirectional = false)
        {
            if (!teams.TryGetValue(sourceTeamId, out var source))
            {
                logger.LogError($"队伍 '{sourceTeamId}' 不存在");
                return;
            }
            if (!teams.TryGetValue(targetTeamId, out var target))
            {
                logger.LogError($"队伍 '{targetTeamId}' 不存在");
                return;
            }


            if (!AreTeamsOpposing(sourceTeamId, targetTeamId))
            {
                source.opposingTeams.push(target);

                if (GetModConfig.Teams.TryGetValue(sourceTeamId, out var sourceConfig))
                {
                    if (!sourceConfig.OpposingTeamIds.Contains(targetTeamId))
                    {
                        sourceConfig.OpposingTeamIds.Add(targetTeamId);
                    }
                }
            }

            if (bidirectional && !AreTeamsOpposing(targetTeamId, sourceTeamId))
            {
                target.opposingTeams.push(source);
                if (GetModConfig.Teams.TryGetValue(targetTeamId, out var targetConfig))
                {
                    if (!targetConfig.OpposingTeamIds.Contains(sourceTeamId))
                    {
                        targetConfig.OpposingTeamIds.Add(sourceTeamId);
                    }
                }
            }
        }
    }
}