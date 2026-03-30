using System.Collections.Generic;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Utilities;

namespace EnemiesVsEnemies.Configuration
{
    public class ModConfig
    {
        public Dictionary<string, TeamConfig> Teams { get; set; } = new();
        public Dictionary<string, EnemySpawnConfig> EnemyPresets { get; set; } = new();
        public Dictionary<int, ContorlLbleKeyConfig> ControlKeys { get; set; } = new();
        public GeneralSettings General { get; set; } = new();

        public void defaultValues()
        {
            ControlKeys = new Dictionary<int, ContorlLbleKeyConfig>
            {
                {
                    42, new ContorlLbleKeyConfig
                    {
                        Name = "SpawnEnemyTrigger",
                        act = 42,
                        Third = KeyHelper.T,
                        Primary =KeyHelper.T,
                        Secondary=KeyHelper.T
                    }
                }
            };
        }


        public class GeneralSettings
        {
            public int DefaultEnemyCount { get; set; } = 1;
            public bool ForceFieldEnabled { get; set; } = true;
            public bool HeroInvincible { get; set; } = true;
            public bool AutoSetEnemyTeams { get; set; } = false;
            public bool BossCameraTrackingDisabled { get; set; } = false;
            public bool ShowDebugInfo { get; set; } = true;

            public bool IslockedController = false;
        }
    }
}