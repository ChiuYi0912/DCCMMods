using dc;
using dc.en;
using dc.pr;
using dc.en.hero;
using dc.hl.types;
using dc.level;
using dc.haxe.ds;
using dc.tool;
using dc.tool.utils;
using ModCore.Mods;
using ModCore.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using CoreLibrary.Core.Extensions;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using dc.libs;

namespace MoreSettings.GameMechanics.Preload
{

    public static partial class PreloadLevels
    {
        private static ArrayObj BuildExtraMobs(dc.pr.Game self)
        {
            var extraMobs = (ArrayObj)ArrayUtils.CreateDyn().array;

            if (self.hasGameplayMod(new GameplayMod.ExtraFlyingBois()))
            {
                extraMobs.push(new MobGenInfos("BatKamikaze".ToHaxeString(), Ref<double>.In(3)));
                var fly = new MobGenInfos("Fly".ToHaxeString(), Ref<double>.In(5));
                fly.setPack(2, 3);
                extraMobs.push(fly);
            }
            if (self.hasGameplayMod(new GameplayMod.InvisibleMobs()))
                extraMobs.push(new MobGenInfos("Fogger".ToHaxeString(), Ref<double>.In(6)));
            if (self.hasGameplayMod(new GameplayMod.SpikedMushrooms()))
                extraMobs.push(new MobGenInfos("Spiker".ToHaxeString(), Ref<double>.In(3)));
            if (self.hasGameplayMod(new GameplayMod.BulletHell()))
            {
                var mage = new MobGenInfos("Mage360".ToHaxeString(), Ref<double>.In(6));
                mage.setPack(2, null);
                extraMobs.push(mage);
            }

            return extraMobs;
        }

        private static virtual_bossRune_endKind_forge_hasMods_history_isCustom_meta_runNum_
            CreateServerStats(dc.pr.Game self, bool hasMods)
        {
            var stats = new virtual_bossRune_endKind_forge_hasMods_history_isCustom_meta_runNum_
            {
                runNum   = self.user.userStats.runs,
                isCustom = self.data.cgData != null,
                hasMods  = hasMods,
                bossRune = self.user.br_numActivated(),
                meta     = self.user.itemMeta.getAllMetaUnlocked()
            };
            var forgeArr = ArrayUtils.CreateFloat();
            int maxUpgrade = self.user.itemMeta.f_getMaxUpgradeLevel();
            for (int i = 0; i <= maxUpgrade; i++)
                forgeArr.push(self.user.itemMeta.f_getRawInvestedRatio(i));
            stats.forge   = forgeArr;
            stats.history = (ArrayObj)ArrayUtils.CreateDyn().array;
            return stats;
        }

        private static void RandomizeSkin(dc.pr.Game self)
        {
            var available = self.user.itemMeta.listSkinAvailable();
            var unlocked  = new List<dc.String>();
            for (int i = 0; i < available.length; i++)
            {
                var skin = available.getDyn(i);
                if (skin != null && self.data.cgData.skinsLocked.indexOf(skin, null) == -1)
                    unlocked.Add(skin);
            }
            if (unlocked.Count == 0) return;
            var chosen = unlocked[Std.Class.random(unlocked.Count)];
            if (self.hero.initDone) self.hero.applySkin(chosen);
            else self.user.heroSkin = chosen;
        }

        private static void RandomizeHead(dc.pr.Game self)
        {
            var available = self.user.itemMeta.listHeadsAvailable();
            var unlocked  = new List<dc.String>();
            for (int i = 0; i < available.length; i++)
            {
                var head = available.getDyn(i);
                if (head != null
                    && self.data.cgData.headsLocked.indexOf(head, null) == -1
                    && self.isCompatibleHead(head))
                    unlocked.Add(head);
            }
            if (unlocked.Count == 0) return;
            self.user.heroHeadSkin = unlocked[Std.Class.random(unlocked.Count)];
        }

        private static void ApplyHotlineMiamiSkin(dc.pr.Game self)
        {
            var candidates = new List<virtual_colorMap_consoleCmdId_glowData_group_head_incompatibleHeads_item_model_onlyDefaultHead_scarfBlendMode_scarfs_>();
            for (int i = 0; i < Data.Class.skin.all.get_length(); i++)
            {
                var raw   = Data.Class.skin.all.getDyn(i);
                var hSkin = ((HaxeProxyBase)raw).ToVirtual<virtual_colorMap_consoleCmdId_glowData_group_head_incompatibleHeads_item_model_onlyDefaultHead_scarfBlendMode_scarfs_>();
                if (hSkin.consoleCmdId.ToString() == "hotlineMiami" && hSkin.item != self.user.getHeroSkinInfos().item)
                    candidates.Add(hSkin);
            }
            if (candidates.Count == 0) return;
            var chosen = candidates[Std.Class.random(candidates.Count)];
            self.user.heroSkin = (chosen.item?.ToString() ?? "PrisonerDefault").ToHaxeString();
        }

        private static void RollBonusQuarterScrollLevels(dc.pr.Game self)
        {
            var candidates = new List<LevelIfor_Virtual>();
            var allLvls = Data.Class.level.all;
            for (int i = 0; i < allLvls.get_length(); i++)
            {
                dynamic raw = allLvls.getDyn(i);
                var info = ((HaxeProxyBase)raw).ToVirtual<LevelIfor_Virtual>();
                if (info.group == 0 && (info.flagsProps.genFlags & (1 << 16)) == 0)
                    candidates.Add(info);
            }
            self.bonusQuarterScrollLevels = new StringMap()
                .ToVirtual<virtual_exists_get_iterator_keys_remove_set_toString_>();

            int bonusCount = 4;
            for (int i = 0; i < bonusCount && candidates.Count > 0; i++)
            {
                int idx = Std.Class.random(candidates.Count);
                var chosen = candidates[idx];
                candidates.RemoveAt(idx);
                self.bonusQuarterScrollLevels.set.Invoke(chosen.id, 1);
            }
        }

        private static void HandleShopMimic(dc.pr.Game self, LevelIfor_Virtual customInfo)
        {
            bool alreadySpawned = GameUtils.Class.haveVisitedBiome("Bank".ToHaxeString())
                || Main.Class.ME.options.assistMode.lockMimicSpawn;
            if (alreadySpawned)
            {
                self.shopMimicBiomeDepth    = null;
                self.spawnMimicInNextLevel  = false;
                self.data.gameFlags.set("shopMimicSpawned".ToHaxeString(), 1);
            }
            else if (self.shopMimicBiomeDepth != null
                && customInfo.worldDepth >= self.shopMimicBiomeDepth
                && !self.get_isInSubMode()
                && self.user.userStats.hasSeenMob("ShopMimic".ToHaxeString())
                && self.user.br_getDifficulty() > 0)
            {
                self.shopMimicBiomeDepth    = null;
                self.spawnMimicInNextLevel  = true;
                self.data.gameFlags.set("shopMimicSpawned".ToHaxeString(), 1);
            }
        }

        private static void GenerateCursedMobs(dc.pr.Game self, LevelGen levelGen, ArrayObj levelMaps,
            LevelIfor_Virtual customInfo, ArrayObj extraMobs)
        {
            var allMobs = Data.Class.mob.all;
            var cursedMobs = ArrayUtils.CreateDyn();
            for (int i = 0; i < allMobs.get_length(); i++)
            {
                var raw = allMobs.getDyn(i);
                var mob = ((HaxeProxyBase)raw).ToVirtual<virtual_active_blueprints_canBeElite_colorSwaps_dlc_flesh1_flesh2_genTags_glowInnerColor_glowOuterColor_group_icon_id_index_life_maxPerPlatform_maxPerRoom_metaItems_minPfHeight_minPfSize_mobTags_name_newSkill_particles_pfCost_props_score_skill_volteDelay_weight_>();
                if (MobTools.Class.hasTag(mob, "Cursed".ToHaxeString()))
                    cursedMobs.push(raw);
            }

            int mobSeed = self.data.gameSeed + customInfo.worldDepth;
            var rand = new Rand(mobSeed);
            rand.seed = (rand.seed * 16807.0) % 2147483647.0;
            int additionalCount = 9 + (((int)rand.seed & 1073741823) % 2);

            for (int k = 0; k < additionalCount; k++)
            {
                var chosen = (HaxeProxyBase)rand.arrayPick(cursedMobs);
                var safe = chosen.ToVirtual<virtual_active_blueprints_canBeElite_colorSwaps_dlc_flesh1_flesh2_genTags_glowInnerColor_glowOuterColor_group_icon_id_index_life_maxPerPlatform_maxPerRoom_metaItems_minPfHeight_minPfSize_mobTags_name_newSkill_particles_pfCost_props_score_skill_volteDelay_weight_>();
                bool found = false;
                for (int m = 0; m < extraMobs.length; m++)
                {
                    var mobInfo = (MobGenInfos)extraMobs.getDyn(m);
                    if (mobInfo.mobId.ToString() == safe.id.ToString())
                    {
                        mobInfo.setMaxSpawn((mobInfo.maxSpawn ?? 0) + 1);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var newMob = new MobGenInfos(safe.id, Ref<double>.Null);
                    newMob.setMaxSpawn(1);
                    extraMobs.push(newMob);
                }
            }

            int zero = 0;
            levelGen.genMobs(self.user, levelMaps, extraMobs, Ref<int>.In(zero));
        }

        public static void FillLevels()
        {
            allLevels.Clear();
            var levelData = Data.Class.level.all;
            for (int i = 0; i < 85; i++)
            {
                var data = levelData.getDyn(i);
                allLevels.Add(data.id.ToString());
            }
            //var levelInfo = ((HaxeProxyBase)levelData).ToVirtual<LevelIfor_Virtual>();
        }
    }
}
