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
using dc.hl.types;
using ModCore.Mods;
using dc.haxe.ds;
using dc.en.hero;
using ModCore.Utilities;
using Hashlink.Virtuals;
using dc.achievements;
using HaxeProxy.Runtime;
using CoreLibrary.Core.Extensions;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using MoreSettings.Utilities;

namespace MoreSettings.GameMechanics.Preload
{

    public static partial class PreloadLevels
    {
        #region 初始化阶段
        /// <summary>初始化阶段</summary>
        private static void RunInitPhase(dc.pr.Game self, dc.String id, string cid,
            LevelIfor_Virtual levelInfo, bool isSubMode, bool isRichterCastle)
        {
            if (!isSubMode && cid == "PrisonStart")
            {
                bool hasMods = self.user.activeMods != null && self.user.activeMods.length > 0;
                self.serverStats = CreateServerStats(self, hasMods);
            }

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

            if (self.hero != null) self.hero.clean();
            Assets.Class.lib.resetUsed();

            if (!isRichterCastle)
            {
                self.data.sync(self, id);
                if (!isSubMode) Save.Class.syncGameData(self.user, self.data, self);
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
                    if (power != null && !power!.destroyed) power!.onDispose();
                }
            }

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
                        self.data.gameFlags.remove("FlawlessBiome".ToHaxeString());
                    else
                        self.data.gameFlags.set("FlawlessBiome".ToHaxeString(), 1);

                    HUD.Class.ME.killCount.setIcon(Assets.Class.fx.getTile("iconPerfect".ToHaxeString(),
                        Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null));
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

                if (!isSubMode && self.curLevel != null && self.curLevel.map.id.ToString() != "PrisonStart")
                {
                    if (self.data.gameFlags.get("FlawlessBiome".ToHaxeString()) > 0)
                        self.unlockVortexBadSeedHead();
                }

                if (self.curLevel?.map.id.ToString() == "Greenhouse"
                    && (self.hero.inventory.hasItem("SpawnFriendlyHardy".ToHaxeString())
                        || self.hero.inventory.hasItem("ExplodeFriendlyHardy".ToHaxeString()))
                    && !self.hero.cd.fastCheck.exists(778043392))
                {
                    if (HeadCheckHelper.Class.unlockHead("MushroomBoi".ToHaxeString()))
                        self.delayer.addS(null, new HlAction(() => { }), 2.0);
                }
            }

            if (self.curLevel != null)
                self.data.saveLevelGameTime(self.curLevel.map.id);

            for (int i = 0; i < self.twitchVotes.length; i++)
            {
                var vote = self.twitchVotes.getDyn(i);
                if (vote != null) vote.initGfx();
            }

            for (int i = self.subLevels.length - 1; i >= 0; i--)
            {
                var sub = self.subLevels.getDyn(i);
                if (sub != null)
                {
                    if (sub.root?.parent != null) sub.root.parent.removeChild(sub.root);
                    sub.disposeImmediately();
                }
            }
            self.subLevels = (ArrayObj)ArrayUtils.CreateDyn().array;
            self.curLevel = null;

            self.gameplayMods = (self.data.cgData == null || isRichterCastle)
                ? (ArrayObj)ArrayUtils.CreateDyn().array
                : self.data.cgData.getGameplayMods();

            if (self.data._twitchMode && self.nextTwitchGameplayMod != null!)
            {
                var arr = (ArrayObj)ArrayUtils.CreateDyn().array;
                arr.push(self.nextTwitchGameplayMod);
                self.gameplayMods = self.gameplayMods.concat(arr);
            }

            if (cid == "PrisonStart" && !self.isScoring() && self.user.br_getDifficulty() >= 3)
                RollBonusQuarterScrollLevels(self);

            if (!isSubMode)
            {
                if (self.nextCursedLevels.indexOf(id, null) != -1
                    || dc.ui.Console.Class.ME.flags.exists("allCursedLevels".ToHaxeString()))
                {
                    self.data.currentCursedLevel = id;
                    self.cursedLevelsCount++;
                    self.cursedChestsBonusChance = 0.1;
                }
                if (self.data.currentCursedLevel != id)
                    self.chooseNextCursedLevels(levelInfo);
            }

            if (cid == "PrisonStart" && !self.isScoring())
                self.shopTypeChance = 0;

            HandleShopMimic(self, levelInfo);
        }
        #endregion
        #region 关卡生成后阶段
        /// <summary>关卡生成后阶段</summary>
        private static void RunPostGenPhase(dc.pr.Game self, dc.String id, string cid,
            ArrayObj levelMaps, int seed, LevelIfor_Virtual customLevelInfo,
            bool isSubMode, LevelTransition cine, bool incentivized,
            bool skipLootGen = false)
        {
            if (isSubMode)
            {
                for (int i = 0; i < self.subLevels.length; i++)
                {
                    var sub = self.subLevels.getDyn(i);
                    if (sub != null) { sub.map.lootLevel++; sub.isCursed = true; }
                }
            }

            if (!skipLootGen)
            {
                var nonSecretLevels = new List<Level>();
                for (int i = 0; i < self.subLevels.length; i++)
                {
                    var lvl = self.subLevels.getDyn(i);
                    if (lvl != null && !lvl!.isSecret) nonSecretLevels.Add(lvl);
                }
                var mapsForLoot = nonSecretLevels.Select(l => l.map).ToArrayObj();
                new LootGen(self.user, mapsForLoot, seed,
                    self.data.tierDistribution, self.hero,
                    Ref<bool>.In(incentivized), Ref<bool>.Null);
                Boot.Class.tryRender();
            }

            for (int i = 0; i < self.subLevels.length; i++)
            {
                var lvl = self.subLevels.getDyn(i);
                if (lvl != null) { lvl.finalizeCreation(); Boot.Class.tryRender(); }
            }

            if (cid == "PrisonStart" && self.isScoring() && self.hero != null)
                self.hero.awake = false;

            var mainMap = levelMaps.length > 0 ? (LevelMap)levelMaps.getDyn(0) : null;
            self.activateSubLevel(mainMap, null, Ref<bool>.In(incentivized), Ref<bool>.Null);
            Boot.Class.tryRender();

            if (self.isBossRush()) self.bossRush.onLevelActivated();

            for (int i = 0; i < self.gameplayMods.length; i++)
            {
                var mod = self.gameplayMods.getDyn(i);
                if (mod == self.nextTwitchGameplayMod || cid == "PrisonStart")
                    self.delayer.addS(null, new HlAction(() => { }), 1.5 + 2 * i);
            }

            if (((HaxeProxyBase)customLevelInfo).GetDynamicMemberNames().Contains("dlc"))
            {
                double delay = (customLevelInfo.dlc.ToString() == "Purple" && customLevelInfo.group != 1)
                    ? 3.5 : 1.0;
                if (!self.isBossRush())
                {
#pragma warning disable CS0618
                    var ctx = new dc.pr.Game.loadMainLevelContext_20414(id, self, customLevelInfo, incentivized, false);
#pragma warning restore CS0618

                    self.delayer.addS(null, new HlAction(ctx.ArrowFunctionEntry_34173), delay);
                }
            }

            if (cid == "PrisonStart" && !self.isScoring())
            {
                var story = self.user.story;
                if (story.counters.get("BankUnlockPopUp".ToHaxeString()) != 1
                    && (self.getBiomeVisitCount("Throne".ToHaxeString()) > 0
                        || self.getBiomeVisitCount("QueenArena".ToHaxeString()) > 0))
                    self.endGamePopUps.push(new HlAction(() => { }));
                if (story.counters.get("BRUnlockPopUp".ToHaxeString()) != 1
                    && (self.getBiomeVisitCount("Throne".ToHaxeString()) > 0
                        || self.getBiomeVisitCount("QueenArena".ToHaxeString()) > 0))
                    self.endGamePopUps.push(new HlAction(() => { }));
                if (story.counters.get("1BCPopUp".ToHaxeString()) == 1)
                    self.endGamePopUps.push(new HlAction(() => { }));

                for (int i = 0; i < self.endGamePopUps.length; i++)
                    self.endGamePopUps.getDyn(i)?.Invoke();
            }

            if (self.isScoring()) self.scoring.initScore();
            if (self.infection != null && self.user.br_hasInfection()) self.infection.loadMobAtlas();

            Assets.Class.lib.flushCache();
            Boot.Class.tryRender();

            if (self.hasGameplayMod(new GameplayMod.BloodThirst()) && self.curLevel != null)
            {
                var heroes = self.curLevel.entitiesByClass.get(60929) as ArrayObj;
                if (heroes != null)
                {
                    for (int i = 0; i < heroes.length; i++)
                    {
                        var hero = heroes.getDyn(i) as Hero;
                        if (hero != null) hero.cd.fastCheck.set(673185792, 10.0 * hero.cd.baseFps);
                    }
                }
            }

            if (Main.Class.ME.options.assistMode.continueEnabled)
                Main.Class.ME.writeSave();
        }
        #endregion
    }
}
