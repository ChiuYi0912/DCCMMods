using dc;
using dc.level;
using dc.tool;
using dc.hl.types;
using dc.tool.mod.script;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using dc.pr;
using CoreLibrary.Core.Extensions;

namespace MoreSettings.GameMechanics.Preload
{
    public static partial class PreloadLevels
    {
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

        private static void PreloadSingleDeep(dc.pr.Game self, string levelId, int seed)
        {
            var id = levelId.ToHaxeString();
            var levelData = Data.Class.level.byId.get(id);
            if (levelData == null) return;

            var levelInfo = ((HaxeProxyBase)levelData).ToVirtual<LevelIfor_Virtual>();
            var levelGen  = new LevelGen(Boot.Class.tryRender);
            var extraMobs = BuildExtraMobs(self);

            ScriptManager.Class.get_instance().loadLevel(id);
            Boot.Class.tryRender();

            var customLevelInfo = ScriptManager.Class.get_instance().getCustomLevelInfo(levelInfo);
            var levelMaps       = levelGen.generate(self.user, seed, customLevelInfo, Ref<bool>.Null);

            int zero = 0;
            levelGen.genMobs(self.user, levelMaps, extraMobs, Ref<int>.In(zero));
            Boot.Class.tryRender();

            var savedSubLevels = self.subLevels;
            var savedCurLevel  = self.curLevel;
            self.subLevels = (ArrayObj)ArrayUtils.CreateDyn().array;
            self.curLevel  = null;

            var platformSnap = SavePlatformOccupations(levelMaps);
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
                RestorePlatformOccupations(levelMaps, platformSnap);
                self.subLevels = savedSubLevels;
                self.curLevel  = savedCurLevel;
            }

            levelCache[levelId] = new CachedLevelData
            {
                LevelMaps       = levelMaps,
                ExtraMobs       = extraMobs,
                CustomLevelInfo = customLevelInfo,
                LootGenDone     = true,
                DecoZones       = cachedDecoZones,
            };
        }

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
            var rooms = mainDisp.lmap.rooms;
            if (rooms != null)
            {
                for (int i = 0; i < rooms.length; i++)
                {
                    Room r = rooms.getDyn(i);
                    if (r != null)
                    {
                        try { mainDisp.decorateRoom(r); } catch { }
                        if ((i + 1) % 20 == 0) Boot.Class.tryRender();
                    }
                }
                Boot.Class.tryRender();
            }
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

        private static Dictionary<int, (int count, Dictionary<int, dynamic> occ)> SavePlatformOccupations(ArrayObj levelMaps)
        {
            var snap = new Dictionary<int, (int, Dictionary<int, dynamic>)>();
            for (int mi = 0; mi < levelMaps.length; mi++)
            {
                var map = (LevelMap)levelMaps.getDyn(mi);
                if (map?.platforms == null) continue;
                for (int pi = 0; pi < map.platforms.length; pi++)
                {
                    var pf = map.platforms.getDyn(pi) as Platform;
                    if (pf == null || pf.occupations == null) continue;
                    snap[pi] = (pf.occupiedCount, pf.occupations.ToDictionary(v => System.Convert.ToInt32(v)));
                }
            }
            return snap;
        }

        private static void RestorePlatformOccupations(ArrayObj levelMaps, Dictionary<int, (int count, Dictionary<int, dynamic> occ)> snap)
        {
            if (snap == null || snap.Count == 0) return;
            var map = (LevelMap)levelMaps.getDyn(0);
            if (map?.platforms == null) return;

            foreach (var kv in snap)
            {
                if (kv.Key >= map.platforms.length) continue;
                var pf = map.platforms.getDyn(kv.Key) as Platform;
                if (pf == null || pf.occupations == null) continue;

                var keys = pf.occupations.keys();
                while (keys.hasNext())
                    pf.occupations.set(keys.next(), 0);

                foreach (var okv in kv.Value.occ)
                    pf.occupations.set(okv.Key, okv.Value);

                pf.occupiedCount = kv.Value.count;
            }
        }
    }
}
