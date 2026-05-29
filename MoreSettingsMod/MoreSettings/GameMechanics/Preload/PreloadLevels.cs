using dc;
using dc.en;
using dc.pr;
using dc.ui;
using dc.libs;
using dc.cine;
using dc.level;
using dc.tool;
using dc.hxd.res;
using dc.tool.atk;
using dc.tool.log;
using dc.tool.utils;
using dc.hl.types;
using ModCore.Mods;
using dc.haxe.ds;
using dc.en.hero;
using ModCore.Utilities;
using Hashlink.Virtuals;
using ModCore.Modules;
using dc.achievements;
using HaxeProxy.Runtime;
using dc.tool.mod.script;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using CoreLibrary.Core.Extensions;
using MoreSettings.Utilities;

namespace MoreSettings.GameMechanics.Preload
{
    public static partial class PreloadLevels
    {
        private static readonly List<string> allLevels = new()
        {
            "PrisonCourtyard", "SewerShort", "PrisonDepths", "PrisonCorrupt", "PrisonRoof",
            "Ossuary", "SewerDepths", "Bridge", "BeholderPit", "StiltVillage",
            "AncientTemple", "Cemetery", "ClockTower", "Crypt", "TopClockTower",
            "Cavern", "Giant", "Castle", "Distillery", "Throne",
            "Astrolab", "Observatory", "Greenhouse",
            "Swamp", "SwampHeart", "Tumulus", "Cliff", "GardenerStage",
            "Shipwreck", "Lighthouse", "QueenArena", "Bank", "PurpleGarden",
            "DookuCastle", "DookuCastleHard", "DeathArena", "DookuArena"
        };

        public sealed class CachedLevelData
        {
            public ArrayObj LevelMaps = null!;
            public ArrayObj ExtraMobs = null!;
            public LevelIfor_Virtual CustomLevelInfo = null!;
            public bool LootGenDone;
            public ArrayObj? DecoZones;
        }

        private static readonly Dictionary<string, CachedLevelData> levelCache = new();
        private static int cachedSeed;
        private static bool preloadDone;

        /// <summary>
        /// 计算下一关卡的种子。
        /// </summary>
        private static int GetNextLevelSeed(dc.pr.Game self, int? forcedSeed)
        {
            return forcedSeed ?? (self.data.gameSeed + self.getUniqId());
        }
        private static bool CacheValid() => preloadDone;

        public static void ClearCache()
        {
            levelCache.Clear();
            cachedSeed = 0;
            preloadDone = false;
        }

        public static CachedLevelData? GetCached(string levelId)
        {
            levelCache.TryGetValue(levelId, out var data);
            return data;
        }

        public static ArrayObj? GetCachedDecoZones(string levelId)
        {
            levelCache.TryGetValue(levelId, out var data);
            return data?.DecoZones;
        }

        #region 主方法
        public static void loadMainLevel(
            Hook_Game.orig_loadMainLevel orig,
            dc.pr.Game self,
            LevelTransition cine,
            dc.String id,
            Ref<bool> activate,
            int? forcedSeed)
        {
            LogUtils.Class.logInformation($"(Chiu Yi)Loading level {id}".ToHaxeString(), new()
            {
                className = "pr.Game".ToHaxeString(),
                methodName = "loadMainLevel".ToHaxeString(),
                lineNumber = 805
            });

            var levelData = Data.Class.level.byId.get(id);
            var levelInfo = ((HaxeProxyBase)levelData).ToVirtual<LevelIfor_Virtual>();

            string cid = id.ToString();
            bool isSubMode = self.get_isInSubMode();
            bool isRichterCastle = self.hero is Richter || cid == "RichterCastle";

            // 1. 初始化（每次必走，不可缓存）
            RunInitPhase(self, id, cid, levelInfo, isSubMode, isRichterCastle);

            // 2. 获取关卡数据（优先缓存）
            int seed = GetNextLevelSeed(self, forcedSeed);
            var cached = CacheValid() ? GetCached(cid) : null;

            LevelIfor_Virtual customLevelInfo = null!;
            ArrayObj levelMaps = null!;
            ArrayObj extraMobs = null!;

            if (cached != null)
            {
                Logger.Information($"使用缓存关卡数据: {cid}");
                levelMaps = cached.LevelMaps;
                extraMobs = cached.ExtraMobs;
                customLevelInfo = cached.CustomLevelInfo;
            }
            else
            {
                ScriptManager.Class.get_instance().loadLevel(id);
                Boot.Class.tryRender();

                customLevelInfo = ScriptManager.Class.get_instance().getCustomLevelInfo(levelInfo);

                var levelGen = new LevelGen(Boot.Class.tryRender);
                levelMaps = levelGen.generate(self.user, seed, customLevelInfo, Ref<bool>.Null);
                extraMobs = BuildExtraMobs(self);

                if (!isSubMode)
                    levelGen.genMobs(self.user, levelMaps, extraMobs, Ref<int>.In(0));
                
                else
                    GenerateCursedMobs(self, levelGen, levelMaps, customLevelInfo, extraMobs);
                

                Boot.Class.tryRender();
                Boot.Class.tryRender();

                if (cid == "PrisonStart" && !CacheValid())
                {
                    PreloadAll(self, seed, cid);
                    levelCache[cid] = new CachedLevelData
                    {
                        LevelMaps = levelMaps,
                        ExtraMobs = extraMobs,
                        CustomLevelInfo = customLevelInfo,
                    };
                }
            }


            bool isFirstLevel = true;
            for (int i = 0; i < levelMaps.length; i++)
            {
                var map = (LevelMap)levelMaps.getDyn(i);
                if (map == null) { isFirstLevel = false; continue; }

                new Level(self, map, null, false, Ref<bool>.In(!isFirstLevel), cine);
                Boot.Class.tryRender();
                isFirstLevel = false;
            }

            //战利品、激活
            RunPostGenPhase(self, id, cid, levelMaps, seed, customLevelInfo,
                isSubMode, cine, incentivized: false,
                skipLootGen: cached?.LootGenDone ?? false);
        }
        
        #endregion
    }
}
