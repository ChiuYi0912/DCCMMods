using CoreLibrary.Core.Utilities;
using dc;
using dc.tool.weap;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.Utilities;
using IngameDebugConsole;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Storage;
using Serilog;

namespace EnemiesVsEnemies
{
    public class EnemiesVsEnemiesMod : ModBase, IOnHeroUpdate, IOnAfterLoadingCDB
    {
        public static Config<ModConfig> config = new("EnemiesVsEnemiesConfig");

        public static ILogger GetLogger = null!;
        private static TeamManager teamManager = null!;
        private static EnemySpawner enemySpawner = null!;
        private static HookManager hookManager = null!;
        private static MobGroupHelper mobGroupHelper = null!;

        private KeyBindingHelper keyBindingHelper = null!;



        private int currentEnemyCount = 1;

        public EnemiesVsEnemiesMod(ModInfo info) : base(info)
        {
        }



        public override void Initialize()
        {
            base.Initialize();
            GetLogger = Logger;
            LoadDefaultConfigIfNeeded();
            InitializeManagers();


            hookManager.InitializeHooks();

            Info.Version = "0.5.0";
            Info.Name = "EnemiesVsEnemies (Enhanced)";

            LogInfo("EnemiesVsEnemies Mod 已初始化");

        }

        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (CricketSelectorGui.isLockedController)
                return;

            HandleKeyBindings();
            Destroymobs();
        }


        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb) => mobGroupHelper.Refresh();


        private void LoadDefaultConfigIfNeeded()
        {
            if (config.Value.Teams.Count == 0 || config.Value.EnemyPresets.Count == 0)
            {
                LogInfo("创建默认配置...");
                config.Value = ModConfig.CreateDefaultConfig();
                config.Save();
                LogInfo("默认配置已创建并保存");
            }
            else
            {
                LogInfo("配置已加载");
            }
        }

        private void InitializeManagers()
        {
            teamManager = new TeamManager();
            teamManager.Initialize(config.Value);

            enemySpawner = new EnemySpawner(teamManager, config.Value);

            hookManager = new HookManager(teamManager, config.Value);

            keyBindingHelper = new KeyBindingHelper(config.Value.KeyBindings);

            mobGroupHelper = new MobGroupHelper();

            currentEnemyCount = config.Value.General.DefaultEnemyCount;

            LogInfo("所有管理器已初始化");
        }

        private void HandleKeyBindings()
        {
            // 生成队伍敌人 (键B)
            if (keyBindingHelper.IsSpawnTeamAPressed())
            {
                foreach (var mobs in config.Value.Teams)
                {
                    enemySpawner.SpawnDefaultEnemiesForTeam(mobs.Value.Id);
                }
            }
        }

        private void Destroymobs()
        {
            if (keyBindingHelper.IsSpawnTeamDestroyMobs())
            {
                enemySpawner.DestroyAllMobsFromCreateList();
            }
        }


        private void UpdateEnemySpawnCount()
        {
            foreach (var preset in config.Value.EnemyPresets.Values)
            {
                preset.SpawnCount = currentEnemyCount;
            }
            config.Save();
        }


        public static TeamManager GetTeamManager() => teamManager;
        public static EnemySpawner GetEnemySpawner() => enemySpawner;
        public static HookManager GetHookManager() => hookManager;
        public static ModConfig GetConfig() => config.Value;
        public static MobGroupHelper GetMobGroupHelper() => mobGroupHelper;


        public void AddNewTeam(string teamId, string name, int teamColor, List<string> opposingTeamIds, List<string> defaultEnemies)
        {
            var teamConfig = new TeamConfig(teamId, name, teamColor)
            {
                OpposingTeamIds = opposingTeamIds,
                DefaultEnemies = defaultEnemies
            };

            teamManager.AddTeam(teamConfig);
            config.Save();

            LogInfo($"已添加新队伍: {name} (ID: {teamId})");
        }


        public void AddEnemyPreset(string presetId, string enemyType, int lifeTier = 30, int damageTier = 20, int spawnCount = 1)
        {
            var spawnConfig = new EnemySpawnConfig(enemyType, lifeTier, damageTier, spawnCount);
            enemySpawner.AddEnemyPreset(presetId, spawnConfig);
            config.Save();

            LogInfo($"已添加敌人预设: {presetId} ({enemyType})");
        }



        private void LogInfo(string message)
        {
            if (config.Value.General.ShowDebugInfo)
            {
                Logger.LogInformation($"[EnemiesVsEnemies] {message}");
            }
        }

        private void LogError(string message)
        {
            System.Console.WriteLine($"[EnemiesVsEnemies ERROR] {message}");
        }



    }
}