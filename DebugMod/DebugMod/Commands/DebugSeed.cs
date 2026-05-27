using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.hl.types;
using dc.level;
using dc.pr;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Utilities;
using Serilog;

namespace DebugMod.Commands
{
    public static class DebugSeed
    {
        [ConsoleMethod("run-seed", "")]
        public static void ChangeHeroShaderColor(TextWriter writer, int start, int end)
        {
            BatchTestSeeds(start, end);
        }

        [ConsoleMethod("output-seed", "")]
        public static void OutputAllseed(TextWriter writer)
        {
            PrintSortedSeedsWithGaps(writer);
        }

        public static void GetAllSecretChallenges(Hero hero, int seed)
        {
            Log.Error($"============= Testing seed {seed} =============");

            string levelId = "PrisonStart";
            var ldat = Data.Class.level.byId.get(levelId.ToHaxeString());
            var game = Game.Class.ME;
            var gamedata = game.data.tierDistribution;
            var user = Game.Class.ME.user;

            bool hasChallenge = false;

            var levelGen = new LevelGen(() => { });
            ArrayObj maps = levelGen.generate(user, seed, ((HaxeProxyBase)ldat).ToVirtual<virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_>(), Ref<bool>.Null);

            foreach (LevelMap map in maps)
            {
                var lootGen = new LootGen(user, maps, map.seed, gamedata, hero, Ref<bool>.In(false), Ref<bool>.Null);
                lootGen.generate();

                ArrayObj rooms = map.rooms;
                for (int i = 0; i < rooms.length; i++)
                {
                    if (!hasChallenge)
                    {
                        Room room = rooms.getDyn(i);
                        if (room?.secretLevels == null) continue;
                        for (int j = 0; j < room.secretLevels.length; j++)
                        {
                            InventItem item = room.secretLevels.getDyn(j);
                            if (item == null) continue;
                            Log.Debug($"{room.toString()}");
                            hasChallenge = true;
                            break;
                        }
                    }
                    if (hasChallenge) break;
                }
                if (hasChallenge) break;
            }
            if (!hasChallenge)
                Log.Debug($"种子 {seed} 没有生成挑战");

            Log.Error($"=============");
        }

        public static SeedHelper CheckSeedForChallenge(Hero hero, int seed)
        {
            string levelId = "PrisonStart";
            var ldat = Data.Class.level.byId.get(levelId.ToHaxeString());
            var game = Game.Class.ME;
            var gamedata = game.data.tierDistribution;
            var user = Game.Class.ME.user;

            var levelGen = new LevelGen(() => { });
            ArrayObj maps = levelGen.generate(user, seed, ((HaxeProxyBase)ldat).ToVirtual<virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_>(), Ref<bool>.Null);

            var lootGen = new LootGen(user, maps, seed, gamedata, hero, Ref<bool>.In(false), Ref<bool>.Null);
            lootGen.generate();
            foreach (LevelMap map in maps)
            {


                foreach (Room room in map.rooms)
                {
                    if (room.secretLevels != null && room.secretLevels.length > 0)
                    {
                        for (int j = 0; j < room.secretLevels.length; j++)
                        {
                            InventItem item = room.secretLevels.getDyn(j);
                            if (item == null) continue;
                            Log.Debug($"{room.toString()}");
                            var helper = new SeedHelper();
                            helper.hasChallenge = true;
                            helper.Location = room.id;
                            return helper;
                        }
                    }
                }
            }
            return null!;
        }


        private static readonly object _seedLock = new object();

        public static void BatchTestSeeds(int startSeed, int endSeed)
        {
            Hero hero = Game.Class.ME.hero;
            int processed = 0;
            for (int seed = startSeed; seed <= endSeed; seed++)
            {
                SeedHelper hasChallenge = CheckSeedForChallenge(hero, seed);
                if (hasChallenge != null)
                {
                    lock (_seedLock)
                    {
                        if (!DebugModMod.GetConfig.Value.SuperSeed.Contains(seed))
                        {
                            processed++;
                            DebugModMod.GetConfig.Value.SuperSeed.Add(seed);
                        }

                    }
                }
                string result = hasChallenge != null ? "true" : "false";
                string line = $"seed {seed}: {result}";
                Log.Debug(line);

                if (processed % 100 == 0)
                {
                    lock (_seedLock)
                    {
                        DebugModMod.GetConfig.Save();
                    }
                }
            }
            lock (_seedLock)
            {
                DebugModMod.GetConfig.Save();
            }
        }


        public static void PrintSortedSeedsWithGaps(TextWriter writer)
        {
            var seeds = DebugModMod.GetConfig.Value.SuperSeed.ToList();
            if (seeds.Count == 0)
            {
                Log.Debug("没有保存的种子。");
                return;
            }

            seeds.Sort();
            var output = new System.Text.StringBuilder();

            output.AppendLine(seeds[0].ToString());

            for (int i = 1; i < seeds.Count; i++)
            {
                int diff = seeds[i] - seeds[i - 1];
                if (diff > 10)
                {
                    output.AppendLine($"+++++++{diff}");
                }
                output.AppendLine(seeds[i].ToString());
            }

            writer.Write(output.ToString());
        }


        public class SeedHelper
        {
            public bool hasChallenge;
            public int Location;
        }
    }
}