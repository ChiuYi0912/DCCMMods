using System.Dynamic;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
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
using IngameDebugConsole;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Menu;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Storage;
using Newtonsoft.Json;
using Serilog;

namespace EnemiesVsEnemies
{
    public class EnemiesVsEnemiesMod(ModInfo info) : ModBase(info),
    IOnAfterLoadingCDB,
    IOnGameEndInit,
    IOnHeroUpdate,
    IModMenuProvider
    {
        public static Config<ModConfig> config = new("EnemiesVsEnemiesConfig");

        public static ILogger GetLogger = null!;
        private static TeamManager teamManager = null!;
        private static EnemySpawner enemySpawner = null!;
        private static HookManager hookManager = null!;
        private static MobGroupHelper mobGroupHelper = null!;
        private static ShowEnemiesOptsions showEnemiesOptsions = null!;

        private static string Version = string.Empty;
        private static string ModInfoName = string.Empty;

        public override void Initialize()
        {
            base.Initialize();
            Version = Info.Version = "0.6.8";
            Info.Name = ModInfoName = "EnemiesVsEnemies (Enhanced)";
            GetLogger = Logger;

            InitializeManagers();
            hookManager.InitializeHooks();

            config.Value.Teams.Clear();
            config.Save();
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
        void IOnHeroUpdate.OnHeroUpdate(double dt) { }
        IModMenu IModMenuProvider.GetModMenu() => showEnemiesOptsions;
        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb) => mobGroupHelper.Refresh();


        private void InitializeManagers()
        {
            teamManager = new TeamManager();
            teamManager.Initialize(config.Value);

            enemySpawner = new EnemySpawner(teamManager, config.Value);

            hookManager = new HookManager(teamManager, config.Value);

            mobGroupHelper = new MobGroupHelper();

            showEnemiesOptsions = new ShowEnemiesOptsions();

            EventSystem.BroadcastEvent<IOnHookInitialize>();

            LogInfo("所有管理器已初始化");
        }


        public static TeamManager GetTeamManager() => teamManager;
        public static EnemySpawner GetEnemySpawner() => enemySpawner;
        public static HookManager GetHookManager() => hookManager;
        public static ModConfig GetConfig() => config.Value;
        public static MobGroupHelper GetMobGroupHelper() => mobGroupHelper;
        public static string GetVersion() => Version;
        public static string GetName() => ModInfoName;
        public static void LogInfo(string message) => GetLogger.LogInformation($"[EnemiesVsEnemies] {message}");


    }
}