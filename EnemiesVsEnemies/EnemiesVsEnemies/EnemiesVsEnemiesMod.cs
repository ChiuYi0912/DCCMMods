using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.level;
using dc.pr;
using dc.tool.mod;
using dc.tool.weap;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.Utilities;
using IngameDebugConsole;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Storage;
using Serilog;

namespace EnemiesVsEnemies
{
    public class EnemiesVsEnemiesMod : ModBase, IOnAfterLoadingCDB, IOnGameEndInit
    {
        public static Config<ModConfig> config = new("EnemiesVsEnemiesConfig");

        public static ILogger GetLogger = null!;
        private static TeamManager teamManager = null!;
        private static EnemySpawner enemySpawner = null!;
        private static HookManager hookManager = null!;
        private static MobGroupHelper mobGroupHelper = null!;

        private static KeyBindingHelper keyBindingHelper = null!;



        private int currentEnemyCount = 1;
        private static string Version = string.Empty;

        public EnemiesVsEnemiesMod(ModInfo info) : base(info)
        {
        }



        public override void Initialize()
        {
            base.Initialize();
            Version = Info.Version = "0.6.3";
            Info.Name = "EnemiesVsEnemies (Enhanced)";
            GetLogger = Logger;

            InitializeManagers();
            hookManager.InitializeHooks();

            LogInfo("EnemiesVsEnemies Mod 已初始化");

        }


        void IOnGameEndInit.OnGameEndInit()
        {
            var res = Info.ModRoot.GetFilePath("res.pak");
            FsPak.Instance.FileSystem.loadPak(res.ToHaxeString());
            var json = CDBManager.Class.instance.getAlteredCDB();
            dc.Data.Class.loadJson(
               json,
               default);
        }


        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb) => mobGroupHelper.Refresh();




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

        public static void HandleKeyBindings()
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

        public static void Destroymobs()
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
        public static string Virsion() => Version;



        public void LogInfo(string message)
        {
            if (config.Value.General.ShowDebugInfo)
            {
                Logger.LogInformation($"[EnemiesVsEnemies] {message}");
            }
        }

        public void LogError(string message)
        {
            Logger.LogError($"[EnemiesVsEnemies ERROR] {message}");
        }
    }
}