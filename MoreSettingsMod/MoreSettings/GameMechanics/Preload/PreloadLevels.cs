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
using MoreSettings.Configuration;
using CoreLibrary.Core.Extensions;
using MoreSettings.Base.Modules;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;


namespace MoreSettings.GameMechanics.Preload
{
    public class PreloadLevels
    {
        public PreloadLevels()
        {

        }
        private void loadMainLevel(
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


            #region PrisonStart 统计初始化

            if (!isSubMode && cid == "PrisonStart")
            {
                bool hasMods = self.user.activeMods != null && self.user.activeMods.length > 0;
                self.serverStats = CreateServerStats(self, hasMods);
            }

            #endregion 


            #region 随机皮肤 / 头部

            if (self.data.cgData != null && !isSubMode)
            {
                if (self.data.cgData.randomSkin)
                {
                    if (cid == "PrisonStart" || (self.data.cgData.randomSkinEveryLevel && levelInfo.group == 0))
                        RandomizeSkin(self);
                }
                if (self.data.cgData.randomHead)
                {
                    if (cid == "PrisonStart" || (self.data.cgData.randomHeadEveryLevel && levelInfo.group == 0))
                        RandomizeHead(self);
                }
            }
            if (levelInfo.group == 0 && self.user.getHeroSkinInfos().consoleCmdId.ToString() == "hotlineMiami")
                ApplyHotlineMiamiSkin(self);

            #endregion


            #region 更新服务器统计

            if (self.serverStats != null)
            {
                self.updateServerStatsHistory();
                self.serverStats.history.push(new virtual_brut_cellsEarned_level_surv_tact_time_
                {
                    level = id,
                    brut = -1,
                    surv = -1,
                    tact = -1,
                    cellsEarned = 0,
                    time = -1
                });
            }

            #endregion

            #region 清理当前关卡和英雄和其他

            if (self.hero != null)
                self.hero.clean();

            Assets.Class.lib.resetUsed();

            if (!isRichterCastle)
            {
                self.data.sync(self, id);
                if (!isSubMode)
                    Save.Class.syncGameData(self.user, self.data, self);
            }

            Boot.Class.tryRender();


            if (cid == "RichterCastle" && self.hero is not Richter)
                self.hero = Hero.Class.create(self, "Richter".ToHaxeString());
            else if (cid != "RichterCastle" && self.hero is Richter)
                self.hero = Hero.Class.create(self, "Beheaded".ToHaxeString());


            if (self.curLevel != null)
            {
                var powers = self.curLevel.powers;
                for (int i = 0; i < powers.length; i++)
                {
                    var power = powers.getDyn(i);
                    if (power != null && !power?.destroyed)
                        power!.onDispose();
                }
            }

            // 清理音乐缓存
            var cache = Loader.Class.currentInstance.cache;
            {
                var iter = cache.keys();
                while (iter.hasNext.Invoke())
                {
                    var key = iter.next.Invoke();
                    var res = cache.get(key) as Resource;
                    if (res != null && res.entry.get_path().substr(0, 5).ToString() == "music"
                        && Assets.Class.PRELOAD_SUB_MUSICS.indexOf(res.entry.get_path(), null) < 0)
                    {
                        cache.remove(key);
                    }
                }
            }

            #endregion

            #region 初始化

            self.user.story.onLoadMainLevel(self, id);
            AttackData.Class.initPool(200);

            if (self.hero != null)
            {
                self.hero.activeSkillsManager.clearSavedItemCooldowns();
                self.hero.cleanDOT();

                bool resetKillCount = levelInfo.group != 1
                    && levelInfo.id.ToString() != "BossRushHUB"
                    && !self.isBossRush();
                if (resetKillCount)
                {
                    self.data.killCount = 0;
                    self.data.maxKillCount = 0;
                    if (cid == "PrisonStart" || isSubMode)
                    {
                        self.data.gameFlags.remove("FlawlessBiome".ToHaxeString());
                    }
                    else
                    {
                        self.data.gameFlags.set("FlawlessBiome".ToHaxeString(), 1);
                    }
                    HUD.Class.ME.killCount.setIcon(Assets.Class.fx.getTile("iconPerfect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null));
                    HUD.Class.ME.killCount.setValue(0, Ref<bool>.Null);
                    HUD.Class.ME.killCount.onResize();
                }
                else
                {
                    self.user.deathCells = 0;
                    self.hero.restoreDepletedItems();

                    if (self.curLevel?.map.id.ToString() == "Distillery"
                        && self.hero.cd.fastCheck.exists(775946240))
                    {
                        Achievements.Class.setAchievement(
                            new EAchievement.FEAT_FINISHLEVEL_NOLAUNCHER(), Ref<bool>.Null);
                    }
                }

                // 无伤奖励
                if (!isSubMode && self.curLevel != null
                    && self.curLevel.map.id.ToString() != "PrisonStart")
                {
                    int? flawless = self.data.gameFlags.get("FlawlessBiome".ToHaxeString());
                    if (flawless > 0)
                        self.unlockVortexBadSeedHead();
                }

                // Greenhouse 蘑菇仔
                if (self.curLevel?.map.id.ToString() == "Greenhouse"
                    && (self.hero.inventory.hasItem("SpawnFriendlyHardy".ToHaxeString())
                        || self.hero.inventory.hasItem("ExplodeFriendlyHardy".ToHaxeString()))
                    && !self.hero.cd.fastCheck.exists(778043392))
                {
                    if (HeadCheckHelper.Class.unlockHead("MushroomBoi".ToHaxeString()))
                        self.delayer.addS(null, new HlAction(() => { }), 2.0);
                }
            }

            // 保存游戏时间
            ArrayObj savedPowers = null!;
            if (self.curLevel != null)
            {
                self.data.saveLevelGameTime(self.curLevel.map.id);
                savedPowers = self.curLevel.powers;
            }

            // Twitch 初始化
            for (int i = 0; i < self.twitchVotes.length; i++)
            {
                var vote = self.twitchVotes.getDyn(i);
                if (vote != null)
                    vote.initGfx();
            }

            // 清理旧子关卡
            for (int i = self.subLevels.length - 1; i >= 0; i--)
            {
                var sub = self.subLevels.getDyn(i);
                if (sub != null)
                {
                    if (sub.root?.parent != null)
                        sub.root.parent.removeChild(sub.root);
                    sub.disposeImmediately();
                }
            }
            self.subLevels = (ArrayObj)ArrayUtils.CreateDyn().array;
            self.curLevel = null;

            #endregion

            #region 加载关卡脚本和资源

            ScriptManager.Class.get_instance().loadLevel(id);
            Boot.Class.tryRender();

            var customLevelInfo = ScriptManager.Class.get_instance().getCustomLevelInfo(levelInfo);

            #endregion

            #region  游戏修改
            self.gameplayMods = (self.data.cgData == null || isRichterCastle)
                ? (ArrayObj)ArrayUtils.CreateDyn().array
                : self.data.cgData.getGameplayMods();

            if (self.data._twitchMode && self.nextTwitchGameplayMod != null!)
            {
                var arr = (ArrayObj)ArrayUtils.CreateDyn().array;
                arr.push(self.nextTwitchGameplayMod);
                self.gameplayMods = self.gameplayMods.concat(arr);
            }

            #endregion

            #region 额外卷轴关卡

            if (cid == "PrisonStart" && !self.isScoring() && self.user.br_getDifficulty() >= 3)
            {
                RollBonusQuarterScrollLevels(self);
            }
            #endregion

            #region 激励and诅咒关卡

            bool incentivized = false;
            if (!isSubMode)
            {
                incentivized = cid == self.data.currentIncentivizedLevel?.ToString();
                self.chooseNextIncentiveLevel(customLevelInfo);

                if (self.nextCursedLevels.indexOf(id, null) != -1
                    || dc.ui.Console.Class.ME.flags.exists("allCursedLevels".ToHaxeString()))
                {
                    self.data.currentCursedLevel = id;
                    self.cursedLevelsCount++;
                    self.cursedChestsBonusChance = 0.1;
                }
                if (self.data.currentCursedLevel != id)
                    self.chooseNextCursedLevels(customLevelInfo);
            }

            if (cid == "PrisonStart" && !self.isScoring())
                self.shopTypeChance = 0;

            #endregion

            //拟态魔 
            HandleShopMimic(self, customLevelInfo);

            #region 生成关卡数据

            int seed = forcedSeed ?? (self.data.gameSeed + self.getUniqId());
            var levelGen = new LevelGen(Boot.Class.tryRender);
            var levelMaps = levelGen.generate(self.user, seed, customLevelInfo, Ref<bool>.Null);

            #endregion

            #region 游戏模式决定的怪物生成

            var extraMobs = (ArrayObj)ArrayUtils.CreateDyn().array;
            if (self.hasGameplayMod(new GameplayMod.ExtraFlyingBois()))
            {
                extraMobs.push(new MobGenInfos("BatKamikaze".ToHaxeString(), Ref<double>.In(3)));
                var flyInfo = new MobGenInfos("Fly".ToHaxeString(), Ref<double>.In(5));
                flyInfo.setPack(2, 3);
                extraMobs.push(flyInfo);
            }
            if (self.hasGameplayMod(new GameplayMod.InvisibleMobs()))
                extraMobs.push(new MobGenInfos("Fogger".ToHaxeString(), Ref<double>.In(6)));
            if (self.hasGameplayMod(new GameplayMod.SpikedMushrooms()))
                extraMobs.push(new MobGenInfos("Spiker".ToHaxeString(), Ref<double>.In(3)));
            if (self.hasGameplayMod(new GameplayMod.BulletHell()))
            {
                var mageInfo = new MobGenInfos("Mage360".ToHaxeString(), Ref<double>.In(6));
                mageInfo.setPack(2, null);
                extraMobs.push(mageInfo);
            }

            #endregion

            #region  Mob生成

            if (!isSubMode)
            {
                int zero = 0;
                levelGen.genMobs(self.user, levelMaps, extraMobs, Ref<int>.In(zero));
            }
            else
            {
                GenerateCursedMobs(self, levelGen, levelMaps, customLevelInfo, extraMobs);
            }

            #endregion

            Boot.Class.tryRender();
            Boot.Class.tryRender();

            #region 创建 Level

            bool isFirstLevel = true;
            for (int i = 0; i < levelMaps.length; i++)
            {
                var map = (LevelMap)levelMaps.getDyn(i);
                if (map == null)
                    isFirstLevel = false;

                var level = new Level(self, map, null, false,
                    Ref<bool>.In(!isFirstLevel),
                    cine);

                Boot.Class.tryRender();
                isFirstLevel = false;
            }

            #endregion

            #region 子关卡战利品升级 (诅咒模式)

            if (isSubMode)
            {
                for (int i = 0; i < self.subLevels.length; i++)
                {
                    var sub = self.subLevels.getDyn(i);
                    if (sub != null)
                    {
                        sub.map.lootLevel++;
                        sub.isCursed = true;
                    }
                }
            }

            #endregion

            #region 战利品生成

            var nonSecretLevels = new List<Level>();
            for (int i = 0; i < self.subLevels.length; i++)
            {
                var level = self.subLevels.getDyn(i);
                if (level != null && !level?.isSecret)
                    nonSecretLevels.Add(level);
            }

            var mapsForLoot = nonSecretLevels.Select(l => l.map).ToArrayObj();
            var lootGen = new LootGen(self.user, mapsForLoot, seed,
                self.data.tierDistribution, self.hero,
                Ref<bool>.In(incentivized), Ref<bool>.Null);
            Boot.Class.tryRender();

            #endregion

            #region 放置关卡物品

            for (int i = 0; i < self.subLevels.length; i++)
            {
                Level level = self.subLevels.getDyn(i);
                if (level != null)
                {
                    level.finalizeCreation();
                    Boot.Class.tryRender();
                }
            }

            #endregion 

            #region 设置玩家唤醒状态


            //唤醒状态
            if (cid == "PrisonStart" && self.isScoring())
            {
                if (self.hero != null)
                    self.hero.awake = false;
            }

            #endregion
            #region 激活主关卡

            var mainMap = levelMaps.length > 0 ? (LevelMap)levelMaps.getDyn(0) : null;
            self.activateSubLevel(mainMap, null, Ref<bool>.In(incentivized), Ref<bool>.Null);

            Boot.Class.tryRender();

            #endregion
            #region BossRush

            if (self.isBossRush())
                self.bossRush.onLevelActivated();

            #endregion
            #region Twitch 延迟效果

            for (int i = 0; i < self.gameplayMods.length; i++)
            {
                var mod = self.gameplayMods.getDyn(i);
                if (mod == self.nextTwitchGameplayMod || cid == "PrisonStart")
                {
                    double delay = 1.5 + 2 * i;
                    self.delayer.addS(null, new HlAction(() => { }), delay);
                }
            }
            #endregion

            #region 判断恶魔城dlc

            if (((HaxeProxyBase)customLevelInfo).GetDynamicMemberNames().Contains("dlc"))
            {
                double initialDelay = (customLevelInfo.dlc.ToString() == "Purple" && customLevelInfo.group != 1)
               ? 3.5 : 1.0;
                if (!self.isBossRush())
                {
#pragma warning disable CS0618
                    dc.pr.Game.loadMainLevelContext_20414 loadMainLevelContext_2 = new dc.pr.Game.loadMainLevelContext_20414(id, self, customLevelInfo, incentivized, false);
#pragma warning restore CS0618

                    self.delayer.addS(null,
                        new HlAction(() => loadMainLevelContext_2.ArrowFunctionEntry_34173()),
                        initialDelay);
                }
            }

            #endregion
            #region 新游戏弹窗

            if (cid == "PrisonStart" && !self.isScoring())
            {
                var story = self.user.story;
                if (story.counters.get("BankUnlockPopUp".ToHaxeString()) != 1
                    && (self.getBiomeVisitCount("Throne".ToHaxeString()) > 0
                        || self.getBiomeVisitCount("QueenArena".ToHaxeString()) > 0))
                {
                    self.endGamePopUps.push(new HlAction(() => { }));
                }
                if (story.counters.get("BRUnlockPopUp".ToHaxeString()) != 1
                    && (self.getBiomeVisitCount("Throne".ToHaxeString()) > 0
                        || self.getBiomeVisitCount("QueenArena".ToHaxeString()) > 0))
                {
                    self.endGamePopUps.push(new HlAction(() => { }));
                }
                if (story.counters.get("1BCPopUp".ToHaxeString()) == 1)
                    self.endGamePopUps.push(new HlAction(() => { }));

                for (int i = 0; i < self.endGamePopUps.length; i++)
                    self.endGamePopUps.getDyn(i)?.Invoke();
            }

            #endregion

            //竞速模式
            if (self.isScoring())
                self.scoring.initScore();


            //疫病
            if (self.infection != null && self.user.br_hasInfection())
                self.infection.loadMobAtlas();

            //清理
            Assets.Class.lib.flushCache();
            Boot.Class.tryRender();

            //恢复powers
            if (savedPowers != null && self.curLevel != null)
            {
                for (int i = 0; i < savedPowers.length; i++)
                {
                    var power = savedPowers.getDyn(i);
                    if (power != null && !power?.destroyed)
                        self.curLevel.powers.push(power);
                }
            }

            #region 自定义持续流血
            if (self.hasGameplayMod(new GameplayMod.BloodThirst()) && self.curLevel != null)
            {
                var heroes = self.curLevel.entitiesByClass.get(60929) as ArrayObj;
                if (heroes != null)
                {
                    for (int i = 0; i < heroes.length; i++)
                    {
                        var hero = heroes.getDyn(i) as Hero;
                        if (hero != null)
                            hero.cd.fastCheck.set(673185792, 10.0 * hero.cd.baseFps);
                    }
                }
            }
            #endregion

            //保存
            if (Main.Class.ME.options.assistMode.continueEnabled)
                Main.Class.ME.writeSave();
        }


        #region 辅助方法
        private static virtual_bossRune_endKind_forge_hasMods_history_isCustom_meta_runNum_
            CreateServerStats(dc.pr.Game self, bool hasMods)
        {
            var stats = new virtual_bossRune_endKind_forge_hasMods_history_isCustom_meta_runNum_
            {
                runNum = self.user.userStats.runs,
                isCustom = self.data.cgData != null,
                hasMods = hasMods,
                bossRune = self.user.br_numActivated(),
                meta = self.user.itemMeta.getAllMetaUnlocked()
            };

            var forgeArr = ArrayUtils.CreateFloat();
            int maxUpgrade = self.user.itemMeta.f_getMaxUpgradeLevel();
            for (int i = 0; i <= maxUpgrade; i++)
                forgeArr.push(self.user.itemMeta.f_getRawInvestedRatio(i));
            stats.forge = forgeArr;
            stats.history = (ArrayObj)ArrayUtils.CreateDyn().array;
            return stats;
        }

        private static void RandomizeSkin(dc.pr.Game self)
        {
            var available = self.user.itemMeta.listSkinAvailable();
            var unlocked = new List<dc.String>();
            for (int i = 0; i < available.length; i++)
            {
                var skin = available.getDyn(i);
                if (skin != null && self.data.cgData.skinsLocked.indexOf(skin, null) == -1)
                    unlocked.Add(skin);
            }
            if (unlocked.Count > 0)
            {
                var chosen = unlocked[Std.Class.random(unlocked.Count)];
                if (self.hero.initDone)
                    self.hero.applySkin(chosen);
                else
                    self.user.heroSkin = chosen;
            }
        }

        private static void RandomizeHead(dc.pr.Game self)
        {
            var available = self.user.itemMeta.listHeadsAvailable();
            var unlocked = new List<dc.String>();
            for (int i = 0; i < available.length; i++)
            {
                var head = available.getDyn(i);
                if (head != null
                    && self.data.cgData.headsLocked.indexOf(head, null) == -1
                    && self.isCompatibleHead(head))
                    unlocked.Add(head);
            }
            if (unlocked.Count > 0)
                self.user.heroHeadSkin = unlocked[Std.Class.random(unlocked.Count)];
        }

        private static void ApplyHotlineMiamiSkin(dc.pr.Game self)
        {
            var candidates = new List<virtual_colorMap_consoleCmdId_glowData_group_head_incompatibleHeads_item_model_onlyDefaultHead_scarfBlendMode_scarfs_>();
            for (int i = 0; i < Data.Class.skin.all.get_length(); i++)
            {
                var raw = Data.Class.skin.all.getDyn(i);
                var hSkin = ((HaxeProxyBase)raw).ToVirtual<virtual_colorMap_consoleCmdId_glowData_group_head_incompatibleHeads_item_model_onlyDefaultHead_scarfBlendMode_scarfs_>();
                if (hSkin.consoleCmdId.ToString() == "hotlineMiami"
                    && hSkin.item != self.user.getHeroSkinInfos().item)
                    candidates.Add(hSkin);
            }
            if (candidates.Count > 0)
            {
                var chosen = candidates[Std.Class.random(candidates.Count)];
                var skinId = (chosen.item?.ToString()) ?? "PrisonerDefault";
                self.user.heroSkin = skinId.ToHaxeString();
            }
        }

        private static void RollBonusQuarterScrollLevels(dc.pr.Game self)
        {
            var candidates = new List<LevelIfor_Virtual>();
            var allLevels = Data.Class.level.all;
            for (int i = 0; i < allLevels.get_length(); i++)
            {
                dynamic raw = allLevels.getDyn(i);
                var info = ((HaxeProxyBase)raw).ToVirtual<LevelIfor_Virtual>();
                if (info.group == 0 && (info.flagsProps.genFlags & (1 << 16)) == 0)
                    candidates.Add(info);
            }

            self.bonusQuarterScrollLevels = new StringMap().ToVirtual<virtual_exists_get_iterator_keys_remove_set_toString_>();
            int bonusCount = 4;
            for (int i = 0; i < bonusCount && candidates.Count > 0; i++)
            {
                int idx = Std.Class.random(candidates.Count);
                var chosen = candidates[idx];
                candidates.RemoveAt(idx);
                self.bonusQuarterScrollLevels.set.Invoke(chosen.id, 1);
            }
        }

        private static void HandleShopMimic(dc.pr.Game self,
            LevelIfor_Virtual customInfo)
        {
            bool alreadySpawned = GameUtils.Class.haveVisitedBiome("Bank".ToHaxeString())
                || Main.Class.ME.options.assistMode.lockMimicSpawn;
            if (alreadySpawned)
            {
                self.shopMimicBiomeDepth = null;
                self.spawnMimicInNextLevel = false;
                self.data.gameFlags.set("shopMimicSpawned".ToHaxeString(), 1);
            }
            else if (self.shopMimicBiomeDepth != null
                && customInfo.worldDepth >= self.shopMimicBiomeDepth
                && !self.get_isInSubMode()
                && self.user.userStats.hasSeenMob("ShopMimic".ToHaxeString())
                && self.user.br_getDifficulty() > 0)
            {
                self.shopMimicBiomeDepth = null;
                self.spawnMimicInNextLevel = true;
                self.data.gameFlags.set("shopMimicSpawned".ToHaxeString(), 1);
            }
        }

        private static void GenerateCursedMobs(dc.pr.Game self, LevelGen levelGen, ArrayObj levelMaps,
            LevelIfor_Virtual customInfo,
            ArrayObj extraMobs)
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

            int seed = self.data.gameSeed + customInfo.worldDepth;
            var rand = new Rand(seed);
            rand.seed = (rand.seed * 16807.0) % 2147483647.0;
            int add = ((int)rand.seed & 1073741823) % 2;
            int additionalCount = 9 + add;

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
        #endregion
    }
}

