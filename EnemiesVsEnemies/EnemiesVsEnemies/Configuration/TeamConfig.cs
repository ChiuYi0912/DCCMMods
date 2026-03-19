using System.Collections.Generic;

namespace EnemiesVsEnemies.Configuration
{
    public class TeamConfig
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> OpposingTeamIds { get; set; } = new List<string>();
        public List<string> DefaultEnemies { get; set; } = new List<string>();
        public int TeamColor { get; set; } = 0xFFFFFF;

        public TeamConfig() { }

        public TeamConfig(string id, string name, int teamColor = 0xFFFFFF)
        {
            Id = id;
            Name = name;
            TeamColor = teamColor;
        }
    }
}