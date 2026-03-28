using System.Collections.Generic;
using CoreLibrary.Core.Utilities;

namespace EnemiesVsEnemies.Configuration
{
    public class ModConfig
    {
        public Dictionary<string, TeamConfig> Teams { get; set; } = new Dictionary<string, TeamConfig>();
        public Dictionary<string, EnemySpawnConfig> EnemyPresets { get; set; } = new Dictionary<string, EnemySpawnConfig>();
        public KeyBindingsConfig KeyBindings { get; set; } = new KeyBindingsConfig();
        public GeneralSettings General { get; set; } = new GeneralSettings();

        public ModConfig()
        {

        }

        public static ModConfig CreateDefaultConfig()
        {
            //TEST
            var config = new ModConfig();

            config.Teams["TeamA"] = new TeamConfig("TeamA", "红色队伍", 0xFF0000)
            {
                OpposingTeamIds = new List<string> { "TeamB" },
                DefaultEnemies = new List<string> { "U28_VacuumCleaner", "U28_VacuumCleaner" }
            };

            config.Teams["TeamB"] = new TeamConfig("TeamB", "蓝色队伍", 0x0000FF)
            {
                OpposingTeamIds = new List<string> { "TeamA" },
                DefaultEnemies = new List<string> { "U28_VacuumCleaner" }
            };

            config.Teams["TeamC"] = new TeamConfig("TeamC", "黄色队伍", CreateColor.ColorFromHex("#ffff00"))
            {
                OpposingTeamIds = new List<string> { "TeamC" },
                DefaultEnemies = new List<string> { "U28_VacuumCleaner", "Mimic", "Mimic" }
            };

            config.EnemyPresets["U28_VacuumCleaner"] = new EnemySpawnConfig("U28_VacuumCleaner", 30, 20, 1);

            config.KeyBindings = new KeyBindingsConfig();
            config.General = new GeneralSettings();

            return config;
        }
    }

    public class GeneralSettings
    {
        public int DefaultEnemyCount { get; set; } = 1;
        public bool ForceFieldEnabled { get; set; } = true;
        public bool HeroInvincible { get; set; } = true;
        public bool AutoSetEnemyTeams { get; set; } = true;
        public bool BossCameraTrackingDisabled { get; set; } = true;
        public bool ShowDebugInfo { get; set; } = true;

        public GeneralSettings() { }
    }
}