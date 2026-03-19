using dc;
using dc.hxd;
using EnemiesVsEnemies.Configuration;

namespace EnemiesVsEnemies.Utilities
{
    public class KeyBindingHelper
    {
        private readonly KeyBindingsConfig GetKeyconfig;

        public KeyBindingHelper(KeyBindingsConfig config)
        {
            GetKeyconfig = config;
        }

        public bool IsSpawnTeamAPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.SpawnTeamAKey);
        }

        public bool IsSpawnTeamBPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.SpawnTeamBKey);
        }

        public bool IsSpawnGoldPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.SpawnGoldKey);
        }

        public bool IsSpawnForceFieldPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.SpawnForceFieldKey);
        }

        public bool IsCloseForceFieldPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.CloseForceFieldKey);
        }

        public bool IsEndBattlePressed()
        {
            return Key.Class.isPressed(GetKeyconfig.EndBattleKey);
        }

        public bool IsBeginBattlePressed()
        {
            return Key.Class.isPressed(GetKeyconfig.BeginBattleKey);
        }

        public bool IsIncreaseEnemyCountPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.IncreaseEnemyCountKey);
        }

        public bool IsDecreaseEnemyCountPressed()
        {
            return Key.Class.isPressed(GetKeyconfig.DecreaseEnemyCountKey);
        }

        public bool IsKeyPressed(int keyCode)
        {
            return Key.Class.isPressed(keyCode);
        }
    }
}