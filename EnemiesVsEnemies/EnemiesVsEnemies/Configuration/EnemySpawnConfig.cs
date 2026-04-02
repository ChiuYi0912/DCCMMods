namespace EnemiesVsEnemies.Configuration
{
    public class EnemySpawnConfig
    {
        public string EnemyType { get; set; } = string.Empty;
        public int LifeTier { get; set; } = 30;
        public int DamageTier { get; set; } = 30;
        public int SpawnCount { get; set; } = 1;
        public bool IsElite { get; set; } = false;

        public EnemySpawnConfig() { }

        public EnemySpawnConfig(string enemyType, int lifeTier = 30, int damageTier = 30, int spawnCount = 1, bool iselie = false)
        {
            EnemyType = enemyType;
            LifeTier = lifeTier;
            DamageTier = damageTier;
            SpawnCount = spawnCount;
            IsElite = iselie;
        }
    }
}