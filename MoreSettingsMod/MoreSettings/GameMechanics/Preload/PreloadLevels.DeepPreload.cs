using dc;
using dc.cine;
using dc.level;
using dc.tool;
using dc.hl.types;
using dc.tool.mod.script;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using CoreLibrary.Core.Extensions;
using dc.pr;

namespace MoreSettings.GameMechanics.Preload
{
    public static partial class PreloadLevels
    {
        /// <summary>首次进入 PrisonStart 后，预加载allLevels中的关卡</summary>
        public static void PreloadAll(dc.pr.Game self, int seed, string exceptLevelId)
        {
            levelCache.Clear();
            cachedSeed = seed;
            preloadDone = true;

            foreach (var levelId in allLevels)
            {
                if (levelId == exceptLevelId) continue;
                PreloadSingleDeep(self, levelId, seed);
            }
        }

        #region 深度预加载
        private static void PreloadSingleDeep(dc.pr.Game self, string levelId, int seed)
        {
            var id = levelId.ToHaxeString();
            var levelData = Data.Class.level.byId.get(id);
            if (levelData == null) return;

            var levelInfo = ((HaxeProxyBase)levelData).ToVirtual<LevelIfor_Virtual>();
            var levelGen = new LevelGen(Boot.Class.tryRender);
            var extraMobs = BuildExtraMobs(self);

            ScriptManager.Class.get_instance().loadLevel(id);
            Boot.Class.tryRender();

            var customLevelInfo = ScriptManager.Class.get_instance().getCustomLevelInfo(levelInfo);
            var levelMaps = levelGen.generate(self.user, seed, customLevelInfo, Ref<bool>.Null);

            int zero = 0;
            levelGen.genMobs(self.user, levelMaps, extraMobs, Ref<int>.In(zero));
            Boot.Class.tryRender();

            var savedSubLevels = self.subLevels;
            var savedCurLevel = self.curLevel;
            self.subLevels = (ArrayObj)ArrayUtils.CreateDyn().array;
            self.curLevel = null;

            ArrayObj? cachedDecoZones = null;

            try
            {
                CreateTempLevels(self, levelMaps);
                cachedDecoZones = ComputeDecoZones(self);
                RunLootGen(self, seed);

                for (int i = self.subLevels.length - 1; i >= 0; i--)
                {
                    var lvl = self.subLevels.getDyn(i);
                    if (lvl != null) lvl.disposeImmediately();
                }
            }
            finally
            {
                self.subLevels = savedSubLevels;
                self.curLevel = savedCurLevel;
            }

            levelCache[levelId] = new CachedLevelData
            {
                LevelMaps = levelMaps,
                ExtraMobs = extraMobs,
                CustomLevelInfo = customLevelInfo,
                LootGenDone = true,
                DecoZones = cachedDecoZones,
            };
        }
        #endregion

        private static void CreateTempLevels(dc.pr.Game self, ArrayObj levelMaps)
        {
            bool isFirst = true;
            for (int i = 0; i < levelMaps.length; i++)
            {
                var map = (LevelMap)levelMaps.getDyn(i);
                if (map == null) { isFirst = false; continue; }

                new Level(self, map, null, false, Ref<bool>.In(!isFirst), null);
                Boot.Class.tryRender();
                isFirst = false;
            }
        }

        private static ArrayObj? ComputeDecoZones(dc.pr.Game self)
        {
            var firstLvl = self.subLevels.getDyn(0) as Level;
            var mainDisp = firstLvl?.lDisp;
            if (mainDisp == null) return null;

            mainDisp.lmap.initDecoFlags();
            return mainDisp.parseDecoZones();
        }

        private static void RunLootGen(dc.pr.Game self, int seed)
        {
            var nonSecretLevels = new List<Level>();
            for (int i = 0; i < self.subLevels.length; i++)
            {
                var lvl = self.subLevels.getDyn(i);
                if (lvl != null && !lvl!.isSecret)
                    nonSecretLevels.Add(lvl);
            }
            if (nonSecretLevels.Count == 0) return;

            var mapsForLoot = nonSecretLevels.Select(l => l.map).ToArrayObj();
            new LootGen(self.user, mapsForLoot, seed,
                self.data.tierDistribution, self.hero,
                Ref<bool>.In(false), Ref<bool>.Null);
            Boot.Class.tryRender();
        }
    }
}
