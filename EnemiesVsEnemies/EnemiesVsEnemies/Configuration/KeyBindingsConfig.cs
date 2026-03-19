namespace EnemiesVsEnemies.Configuration
{
    public class KeyBindingsConfig
    {

        public int SpawnTeamAKey { get; set; } = 37;
        public int SpawnTeamBKey { get; set; } = 39;
        public int SpawnGoldKey { get; set; } = 67;
        public int SpawnForceFieldKey { get; set; } = 38;
        public int CloseForceFieldKey { get; set; } = 40;
        public int EndBattleKey { get; set; } = 112;
        public int BeginBattleKey { get; set; } = 113;
        public int IncreaseEnemyCountKey { get; set; } = 33; 
        public int DecreaseEnemyCountKey { get; set; } = 34; 

        public KeyBindingsConfig() { }
    }
}