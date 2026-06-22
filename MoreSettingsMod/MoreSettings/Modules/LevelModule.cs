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
using MoreSettings.Configuration;
using CoreLibrary.Core.Extensions;
using MoreSettings.Base.Modules;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;
using HashlinkNET.Native.Impl;
using dc.en.inter;
using dc.chroma.effects;
using ModCore.Events.Interfaces.Game;
using ModCore.Storage;
using ModCore.Events;
using dc.libs.misc;
using CoreLibrary.Utilities;
using MoreSettings.Utilities;
using Serilog;
using dc.hxd;
using ModCore;
using Hashlink.Proxy.Objects;
using Hashlink.Proxy.DynamicAccess;
using Hashlink.Reflection.Types;
using Hashlink.Marshaling;
using Hashlink.Proxy.Clousre;
using Hashlink;
using HaxeProxy.Runtime.Internals;
using Hashlink.Proxy;

namespace MoreSettings.Modules
{
    internal class LevelModule : BaseModule,
    IOnGameExit,
    IHxbitSerializable,
    IEventReceiver,
    IOnAfterLoadingCDB
    {
        public override Enums.MenuCategory Type => Enums.MenuCategory.Level;
        public override string Description => GetText.Instance.GetString("LevelModule");
        public override LevelConfig config => (LevelConfig)base.config;
        public CDBLevelCache cdblevel = new();

        public override void Initialize(ModBase mainMod)
        {
            config = SettingsMain.ConfigValue.level;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            if (!config.Enabled) return;

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("NoFadeIn"),
                GetText.Instance.GetString("NofadeInDesc"),
                () => config.NofadeIn,
                v => config.NofadeIn = v,
                scrollerFlow);

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("MimicLoreRoom"),
                GetText.Instance.GetString(""),
                () => config.LoreBankMimicRoom,
                v => config.LoreBankMimicRoom = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("FaultLevelEffects"),
                GetString("FaultLevelEffectsDesc"),
                () => config.Faulteffects,
                v => config.Faulteffects = v,
                scrollerFlow
            );
        }



        #region Hooks
        public override void PermanentlyRegisterHooks()
        {
            base.PermanentlyRegisterHooks();
        }
        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_LevelStruct.applyDifficulty -= Hook__LevelStruct_applyDifficulty;
            Hook_Game.loadMainLevel -= Hook_Game_loadMainLevel;
            Hook__LevelTransition.__constructor__ -= Hook__LevelTransition__constructor__;
            //Hook_Game.activateSubLevel -= Hook_Game_activateSubLevel;
            // Hook_Level.resume -= Hook_Level_resume;
            // Hook_LevelDisp.render -= Hook_LevelDisp_render;
            //Hook_LevelTransition.loadNewLevel -= Hook_LevelTransition_loadNewLevel;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_LevelStruct.applyDifficulty += Hook__LevelStruct_applyDifficulty;
            Hook_Game.loadMainLevel += Hook_Game_loadMainLevel;
            Hook__LevelTransition.__constructor__ += Hook__LevelTransition__constructor__;
            //Hook_Game.activateSubLevel += Hook_Game_activateSubLevel;
            // Hook_Level.resume += Hook_Level_resume;
            // Hook_LevelDisp.render += Hook_LevelDisp_render;
            //Hook_LevelTransition.loadNewLevel += Hook_LevelTransition_loadNewLevel;
        }
        #endregion

        private void Hook__LevelStruct_applyDifficulty(
            Hook_LevelStruct.orig_applyDifficulty orig, LevelStruct self)
        {
            if (dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel &&
                config.LoreBankMimicRoom &&
                Main.Class.ME.options.disableLoreRooms)
            {
                AddMimicRoom(self);
            }
            orig(self);
        }

        #region Disprender
        private void Hook_LevelDisp_render(Hook_LevelDisp.orig_render orig, LevelDisp self)
        {
            if (self.rendered)
                throw new InvalidOperationException("Render called twice without a clear()");
            self.rendered = true;

            Boot.Class.tryRender();
            self.renderParallaxes();
            Boot.Class.tryRender();
            self.lmap.initDecoFlags();

            bool flag = (self.lmap.infos.flagsProps.genFlags & 1) != 0;
            if (flag)
            {
                Boot.Class.tryRender();
                self.renderRoofs();
                Boot.Class.tryRender();
                self.renderWallTransitions();
                Boot.Class.tryRender();
                self.renderFloorStamps();
            }
            else
            {
                Boot.Class.tryRender();
                self.renderFloorStamps();
            }

            ArrayObj rooms = self.lmap.rooms;
            if (rooms != null)
            {
                for (int i = 0; i < rooms.length; i++)
                {
                    Room r = rooms.getDyn(i);
                    if (r != null) { self.decorateRoom(r); Boot.Class.tryRender(); }
                }
            }
            Boot.Class.tryRender();
            self.decorateLevel();
            Boot.Class.tryRender();

            Boot.Class.tryRender();
            self.renderFrontWalls();
            Boot.Class.tryRender();
            self.renderSlopes();
            Boot.Class.tryRender();
            self.renderFrontCorners();

            ArrayObj decoZones = self.parseDecoZones();
            self.decoZones = decoZones;

            if (decoZones != null)
            {
                for (int i = 0; i < decoZones.length; i++)
                {
                    DecoZone zone = decoZones.getDyn(i);
                    if (zone == null) continue;
                    self.decorateZone(zone);
                    zone.updateFlags(self.lmap);
                    if ((zone.gFlags & 64) == 0 && (zone.gFlags & 256) == 0)
                        self.addJunk(zone);
                    if ((i + 1) % 20 == 0) Boot.Class.tryRender();
                }
            }

            Boot.Class.tryRender();
            self.renderBackWalls();
            Boot.Class.tryRender();
            self.renderFrontVegetation();
            Boot.Class.tryRender();
            self.renderGroundSmoke();
            Boot.Class.tryRender();
            self.renderWaterPools();
            Boot.Class.tryRender();
            self.addCliffLights();
            Boot.Class.tryRender();
            self.renderFakeBlackWalls();
            Boot.Class.tryRender();
            self.renderStructures();
            Boot.Class.tryRender();
            self.createLightWalls();
            Boot.Class.tryRender();
            self.initDecoEntities();
            Boot.Class.tryRender();
            self.addFrontProps();
            Boot.Class.tryRender();
        }

        #endregion

        #region Levelresume
        private void Hook_Level_resume(Hook_Level.orig_resume orig, Level self)
        {
            self.paused = false;

            if (!self.activated)
            {
                self.activated = true;
                self.onActivation();
            }

            self.updateBgDarkenerVisibility();
            self.slm?.onResume();
            self.atManager?.onResume();
            self.lAudio.resume();
            self.loadMinimap();
            self.loadReverb();

            int? color = 0;
            if (self.map.infos.props.HashlinkObj.GetDynamicMemberNames().Contains("chromaColor"))
                color = self.map.infos.props.chromaColor;
            ChromaEffectList.Class.setAllLevelEffects((int)color!);
            Boot.Class.tryRender();

            self.root.set_visible(true);
            Boot.Class.ME.frameProfiler.reset();
            dc.hxd.Timer.Class.reset();

            // Discord Rich Presence
            var hero = self.game.hero;
            string state = $"B: {hero.brutalityTier} T: {hero.tacticTier} S: {hero.survivalTier}";
            Lib_discord.update_state.Invoke(state.ToHaxeString().toUtf8(), false);

            string largeImage = self.map.infos.id.ToString().ToLower();
            Lib_discord.update_largeImageKey.Invoke(largeImage.ToHaxeString().toUtf8(), false);

            if (!string.IsNullOrEmpty(self.map.infos.name?.ToString()))
            {
                string name = Lang.Class.t.get(self.map.infos.name, null).ToString();
                Lib_discord.update_largeImageText.Invoke(name.ToHaxeString().toUtf8(), false);
            }

            int difficulty = self.game.user.br_getDifficulty();
            if (difficulty > 0)
            {
                string smallKey = "difficulty" + difficulty;
                Lib_discord.update_smallImageKey.Invoke(smallKey.ToHaxeString().toUtf8(), false);
                string smallText = Lang.Class.getDifficultyName(difficulty).ToString();
                Lib_discord.update_smallImageText.Invoke(smallText.ToHaxeString().toUtf8(), true);
            }
            else
            {
                Lib_discord.update_smallImageKey.Invoke("".ToHaxeString().toUtf8(), false);
                Lib_discord.update_smallImageText.Invoke("".ToHaxeString().toUtf8(), true);
            }
        }
        #endregion

        #region activateSubLevel
        private void Hook_Game_activateSubLevel(Hook_Game.orig_activateSubLevel orig, dc.pr.Game self,
            LevelMap map, int? linkId, Ref<bool> shouldSave, Ref<bool> outAnim)
        {
            bool doSave = shouldSave.IsNull || shouldSave.value;
            bool doOutAnim = outAnim.IsNull || outAnim.value;

            Level targetLevel = FindSubLevelByMap(self.subLevels, map);
            if (targetLevel == null) throw new Exception("SubLevel not found");

            self.log.clearAll();
            DestroyAllPointers();

            Level previousLevel = self.hero._level;
            var savedPetStates = self.saveHeroPetStates();
            self.hero.clean();
            self.controller.manualLock = false;

            if (self.curLevel != null) { self.curLevel.hide(); self.curLevel = null; }
            self.curLevel = targetLevel;
            self.curLevel.resume();

            if (!self.hero.destroyed
                && (self.hero.awake || self.curLevel.map.id.ToString() != "PrisonStart" || self.get_isInSubMode()))
            {
                int cx = self.curLevel.lastHeroCX;
                int cy = self.curLevel.lastHeroCY;
                if (linkId.HasValue)
                {
                    ZDoor door = FindZDoorByLinkId(self.curLevel, linkId.Value);
                    if (door != null) { cx = door.cx; cy = door.cy; }
                }
                self.initHero(self.curLevel, cx, cy, null, false, previousLevel);

                if (previousLevel != null)
                {
                    foreach (var power in GetPowers(previousLevel))
                        if (power?.owner == self.hero) power.onHeroLevelChanged(previousLevel);
                }
            }

            if (self.hero._level == self.curLevel) HUD.Class.ME.show(true);

            if (self.curLevel.map.infos.group == 1) self.hero.revealBlueprints();
            else self.hero.removeRevealedBlueprints();

            self.resetPetSavedState(savedPetStates);
            self.hero.updatePokemonableIcons();
            dc.libs.Process.Class.resizeAll();

            if (doSave && Main.Class.ME.options.debug == null && !self.get_isInSubMode())
                Main.Class.ME.writeSave();

            if (linkId.HasValue && doOutAnim) new AfterZDoor(self.hero);
        }

        private static Level FindSubLevelByMap(ArrayObj subLevels, LevelMap map)
        {
            if (subLevels == null) return null!;
            for (int i = 0; i < subLevels.length; i++)
                if (subLevels.getDyn(i) is Level lvl && lvl.map == map) return lvl;
            return null!;
        }

        private static void DestroyAllPointers()
        {
            var all = Pointer.Class.ALL;
            if (all == null) return;
            for (int i = 0; i < all.length; i++)
            {
                var ptr = all.getDyn(i) as Pointer;
                if (ptr != null) ptr.destroyed = true;
            }
        }

        private static ZDoor FindZDoorByLinkId(Level level, int linkId)
        {
            if (level == null) return null!;
            var doors = level.entitiesByClass?.get(3929) as ArrayObj;
            if (doors == null) return null!;
            for (int i = 0; i < doors.length; i++)
                if (doors.getDyn(i) is ZDoor door && door.linkId == linkId) return door;
            return null!;
        }

        private static IEnumerable<Power?> GetPowers(Level level)
        {
            if (level?.powers == null) yield break;
            for (int i = 0; i < level.powers.length; i++)
                yield return level.powers.getDyn(i) as Power;
        }
        #endregion

        #region TransitionInit
        private void Hook__LevelTransition__constructor__(
            Hook__LevelTransition.orig___constructor__ orig, LevelTransition arg1,
            dc.String mainId, LevelMap map, int? linkId,
            CPoint heroPosAfterBossRuneReload, Ref<bool> noLoadingData)
        {
            bool skipLoadingData = !noLoadingData.IsNull && noLoadingData.value;
            arg1.playAfterZDoorCine = true;
            HlAction<GameCinematic> hl = (HlAction<GameCinematic>)GameCinematic.Class.__constructor__;
            hl.Invoke(arg1);

            arg1.mainId = mainId;
            arg1.map = map;
            arg1.linkId = linkId;
            arg1.heroPosAfterBossRuneReload = heroPosAfterBossRuneReload;

            if (DLC.Class.levelIsPressHidden(mainId))
                throw new InvalidOperationException("Forbidden level reached.");

            DLC.Class.installMaskCacheDirty = true;

            double multFade = 0.5;
            if (mainId == null || heroPosAfterBossRuneReload != null) multFade *= 0.5;
            arg1.multFade = multFade;

            dc.pr.Game.Class.ME.controller.manualLock = true;
            var curLevel = dc.pr.Game.Class.ME.curLevel;
            if (curLevel == null) { arg1.loadNewLevel(); arg1.disableBars(); return; }

            curLevel.pause();

            virtual_bossRuneActivated_gameTimeS_level_ transitionData = null!;
            if (!skipLoadingData)
            {
                transitionData = new virtual_bossRuneActivated_gameTimeS_level_
                {
                    level = mainId,
                    gameTimeS = dc.pr.Game.Class.ME.data.gameTimeS,
                    bossRuneActivated = dc.pr.Game.Class.ME.user.br_numActivated()
                };
            }

            bool giveGentleman = false;
            if (mainId != null && mainId.ToString() != "PrisonStart"
                && curLevel.map.id.ToString() == "PrisonStart")
            {
                var doors = (ArrayObj)curLevel.entitiesByClass.get(30924);
                bool allIntact = true;
                foreach (var obj in doors)
                {
                    if (obj is Door door && door.broken) { allIntact = false; break; }
                }
                giveGentleman = allIntact;
            }
            arg1.giveGentlemanAchievement = giveGentleman;

            if (!config.NofadeIn || linkId == null)
            {
                Main.Class.ME.fadeIn(null, transitionData, Ref<double>.In(multFade),
                onEnd: new HlAction(() => { arg1.loadNewLevel(); }));
            }
            else
                arg1.loadNewLevel();

            arg1.disableBars();
        }
        #endregion

        #region loadNewLevel
        private void Hook_LevelTransition_loadNewLevel(
            Hook_LevelTransition.orig_loadNewLevel orig, LevelTransition self)
        {
            if (self.heroPosAfterBossRuneReload != null)
            {
                int? seed = dc.pr.Game.Class.ME.curLevel.map?.seed;
                dc.pr.Game.Class.ME.loadMainLevel(self, self.mainId, Ref<bool>.In(true), seed);
            }
            else if (self.mainId != null)
            {
                dc.pr.Game.Class.ME.loadMainLevel(self, self.mainId, default, null);
            }
            else
            {
                dc.pr.Game.Class.ME.activateSubLevel(self.map, self.linkId,
                    Ref<bool>.In(true), Ref<bool>.In(self.playAfterZDoorCine));
            }

            Lib_std.gc_major.Invoke();

            if (self.get_isADlcPLevel())
                self.onEnteredLevel = new HlAction(() =>
                    new PurpleLevelIntro(self.mainId, dc.pr.Game.Class.ME.hero));

            Main.Class.ME.fadeOut(Ref<double>.In(self.multFade));
            self.afterTransitionCine();
            if (self.walk == null && self.jump == null && self.climb == null && self.onEnteredLevel != null)
                self.onEnteredLevel.Invoke();

            if (self.heroPosAfterBossRuneReload != null)
            {
                Room flaskRoom = FindFlaskRoom(dc.pr.Game.Class.ME.curLevel.map?.rooms!);
                if (flaskRoom != null)
                {
                    dc.pr.Game.Class.ME.hero.setPosCase(
                        flaskRoom.x + self.heroPosAfterBossRuneReload.cx,
                        flaskRoom.y + self.heroPosAfterBossRuneReload.cy, null!, null!);
                    dc.pr.Game.Class.ME.curLevel.afterBossRuneReload();
                }
            }

            if (self.giveGentlemanAchievement)
                Achievements.Class.setAchievement(new EAchievement.FEAT_PRISON_NOBREAK_DOOR(), default);

            self.onLoad?.Invoke();
        }

        private static Room FindFlaskRoom(ArrayObj rooms)
        {
            if (rooms == null) return null!;
            for (int i = 0; i < rooms.length; i++)
            {
                Room room = rooms.getDyn(i);
                if (room != null && room.rType.ToString() == "FlaskRoom") return room;
            }
            return null!;
        }
        #endregion


        private void Hook_Game_loadMainLevel(
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


            string cid = id.ToString();
            var levelInfo = cdblevel.Get(cid);
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


            LevelIfor_Virtual customLevelInfo = null!;
            if (GameInfo.Platform == GameInfo.PlatformKind.Steam)
            {
                // ScriptManager removed from API, access via low-level reflection
                var smType = (HashlinkObjectType)HashlinkMarshal.Module.GetTypeByName("tool.mod.script.ScriptManager");
                var smClass = smType.GlobalValue;
                var sm = (HashlinkObject)((HashlinkClosure)((IHashlinkFieldObject)smClass).GetFieldValue("get_instance")!).DynamicInvoke(null)!;

                ((HashlinkClosure)((IHashlinkFieldObject)sm).GetFieldValue("loadLevel")!).DynamicInvoke([id]);
                Boot.Class.tryRender();

                var rawInfo = ((HashlinkClosure)((IHashlinkFieldObject)sm).GetFieldValue("getCustomLevelInfo")!).DynamicInvoke([levelInfo]);
                customLevelInfo = ((HaxeProxyBase)HaxeProxyHelper.GetProxy<HaxeProxyBase>(rawInfo)!).ToVirtual<LevelIfor_Virtual>();
            }
            


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
                if (GameInfo.Platform == GameInfo.PlatformKind.Steam)
                    RollBonusQuarterScrollLevels(self);
            }
            #endregion

            #region 激励and诅咒关卡

            bool incentivized = false;
            bool isCursedLevel = false;
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
                {
                    self.chooseNextCursedLevels(customLevelInfo);
                }
                else
                {
                    isCursedLevel = true;
                    self.chooseNextCursedLevels(customLevelInfo);
                }
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

            if (!isCursedLevel)
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

            if (isCursedLevel)
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

            var mapsForLoot = (ArrayObj)ArrayUtils.CreateDyn().array;
            for (int i = 0; i < self.subLevels.length; i++)
            {
                var level = self.subLevels.getDyn(i);
                if (level != null && !level?.isSecret)
                    mapsForLoot.push(level?.map);
            }
            int lootSeed = levelMaps.length > 0 ? ((LevelMap)levelMaps.getDyn(0))?.seed ?? seed : seed;
            var lootGen = new LootGen(self.user, mapsForLoot, lootSeed,
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
                double initialDelay = (customLevelInfo.dlc.ToString() == "Purple" && customLevelInfo.group != 1) ? 3.5 : 1.0;
                if (!self.isBossRush())
                {
                    self.delayer.addS(null, new HlAction(() =>
                    {
                        if (customLevelInfo.name == null || customLevelInfo.name.length == 0)
                            return;

                        dc.String lastid = null!;
                        if (customLevelInfo.group == 0)
                        {
                            if (cid == "PrisonStart")
                            {
                                self.user.userStats.reachBiome(id, lastid);
                            }
                            else
                            {
                                var times = self.data.gameTimePerLevel;
                                for (int ti = times.length - 1; ti >= 0; ti--)
                                {
                                    var entry = (virtual_id_t_)times.getDyn(ti);
                                    if (((LevelIfor_Virtual)((HaxeProxyBase)Data.Class.level.byId.get(entry.id)).ToVirtual<LevelIfor_Virtual>()).group == 0)
                                    {
                                        lastid = entry.id;
                                        break;
                                    }
                                }
                                self.user.userStats.reachBiome(id, lastid);
                            }
                        }
                        else
                        {
                            self.user.userStats.reachBiome(id, null);
                        }

                        if (self.isScoring())
                        {
                            self.log.textWithTitle(Lang.Class.t.get("Défi Quotidien".ToHaxeString(), null),
                                Lang.Class.t.get(customLevelInfo.name, null), null, null);
                            Audio.Class.ME.playUIEvent((Sound)Res.Class.get_loader().loadCache("sfx/gpfeedback/gong.wav".ToHaxeString(), Sound.Class), null);
                        }

                        if (incentivized)
                        {
                            self.log.textWithTitle(
                                Lang.Class.t.get("Incentivized biomes || Incentive popup on enter level".ToHaxeString(), null),
                                Lang.Class.t.get("Mobs will drop more cells and gold here.".ToHaxeString(), null), null, null);
                            Audio.Class.ME.playUIEvent((Sound)Res.Class.get_loader().loadCache("sfx/gpfeedback/gong.wav".ToHaxeString(), Sound.Class), null);
                        }
                    }), initialDelay);
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


            RoomTemplate.Class.ALL = null;
            Data.Class.room = null;

            #region 随机装备 (RandomEquipment)
            if (self.hero != null && self.hero.awake && self.curLevel != null
                && levelInfo.group == 1
                && self.hasGameplayMod(new GameplayMod.RandomEquipment()))
            {
                HlFunc<InventItem, int, InventItem> reroll = (posId, ii) =>
                {
                    if (ii == null) return null!;
                    var tier = self.hero.inventory.getMainTier(false);
                    var newItem = lootGen.getNewRandItemInGroup(ii, tier);
                    if (newItem == null) return null!;
                    self.hero.dropAndUpdateItem(ii);
                    self.hero.inventory.add(newItem);
                    newItem.posID = posId;
                    if (newItem._itemData == null)
                        newItem._itemData = Data.Class.item.byId.get(newItem.kind.Index.ToString().ToHaxeString());
                    var data = newItem._itemData;
                    if (data != null && (data.group == 4 || data.group == 5 || data.group == 6)
                        && (newItem.hasTag("DualWeaponBase".ToHaxeString()) || newItem.hasTag("DualWeaponOffhand".ToHaxeString())))
                    {
                        int otherPos = posId == 0 ? 1 : 0;
                        var existing = self.hero.inventory.getEquippedWeaponOn(otherPos);
                        if (existing != null) self.hero.dropAndUpdateItem(existing);
                        var paired = self.hero.inventory.add(newItem.get_pairedItem());
                        paired.posID = otherPos;
                    }
                    return newItem;
                };
                for (int wi = 0; wi < self.hero.inventory.nbWeapons; wi++)
                    reroll.Invoke(wi, self.hero.inventory.getEquippedWeaponOn(wi));
                for (int ai = 0; ai < self.hero.inventory.nbActives; ai++)
                    reroll.Invoke(ai, self.hero.inventory.getActiveOn(ai));
                self.hero.onEquipedItemsChange(Ref<bool>.In(true), Ref<bool>.In(true), Ref<bool>.Null);
            }
            #endregion

            #region GlassNinja
            if (self.hero != null && self.hero.awake && self.curLevel != null
                && levelInfo.group == 1
                && self.data.cgData != null
                && self.data.cgData.getPreset() is CGPreset.GlassNinja)
            {
                var tier = self.hero.inventory.getMainTier(false);
                HlAction<int, InventItem> updateGlass = (posId, ii) =>
                {
                    if (ii == null) return;
                    var newItem = lootGen.updateGlassNinjaItem(ii, tier);
                    if (newItem == null) return;
                    self.hero.dropAndUpdateItem(ii);
                    self.hero.inventory.add(newItem);
                    newItem.posID = posId;
                };
                for (int wi = 0; wi < self.hero.inventory.nbWeapons; wi++)
                    updateGlass.Invoke(wi, self.hero.inventory.getEquippedWeaponOn(wi));
                for (int ai = 0; ai < self.hero.inventory.nbActives; ai++)
                    updateGlass.Invoke(ai, self.hero.inventory.getActiveOn(ai));
                self.hero.onEquipedItemsChange(Ref<bool>.In(true), Ref<bool>.In(true), Ref<bool>.Null);
            }
            #endregion

            #region 自定义初始装备
            if (self.hero != null && !self.hero.awake
                && cid == "PrisonStart"
                && self.data.cgData != null
                && self.data.cgData.hasCustomStartEquipment())
            {
                HlAction<int, InventItem, InventItem> hlAction = new HlAction<int, InventItem, InventItem>((posId, ii, newIi) =>
                {
                    if (newIi == null) return;

                    if (ii != null)
                        self.hero.inventory.remove(ii);

                    InventItem inventItem = self.hero.inventory.add(newIi);
                    newIi.posID = posId;
                });
                bool pairedUsed = false;

                if (self.data.cgData.forcedLeftWeapon != null)
                {
                    var item = lootGen.customStartEquipment(
                        self.data.cgData.forcedLeftWeapon.id,
                        self.data.cgData.forcedLeftWeapon.forgeLevel, null);
                    hlAction.Invoke(0, self.hero.inventory.getEquippedWeaponOn(0), item);
                    if ((item.hasTag("DualWeaponBase".ToHaxeString()) || item.hasTag("DualWeaponOffhand".ToHaxeString()))
                        && item.get_pairedItem() != null)
                    {
                        hlAction.Invoke(1, self.hero.inventory.getEquippedWeaponOn(1), item.get_pairedItem());
                        pairedUsed = true;
                    }
                }
                if (self.data.cgData.forcedRightWeapon != null && !pairedUsed)
                {
                    var item = lootGen.customStartEquipment(
                        self.data.cgData.forcedRightWeapon.id,
                        self.data.cgData.forcedRightWeapon.forgeLevel, null);
                    hlAction.Invoke(1, self.hero.inventory.getEquippedWeaponOn(1), item);
                }
                if (self.data.cgData.forcedRightSkill != null)
                {
                    var item = lootGen.customStartEquipment(
                        self.data.cgData.forcedRightSkill.id,
                        self.data.cgData.forcedRightSkill.forgeLevel, null);
                    hlAction.Invoke(0, self.hero.inventory.getActiveOn(0), item);
                }
                if (self.data.cgData.forcedLeftSkill != null)
                {
                    var item = lootGen.customStartEquipment(
                        self.data.cgData.forcedLeftSkill.id,
                        self.data.cgData.forcedLeftSkill.forgeLevel, null);
                    hlAction.Invoke(1, self.hero.inventory.getActiveOn(1), item);
                }
            }
            #endregion

            #region 故障
            if (!self.isScoring())
            {
                int? kicked = self.user.story.counters.exists("kickedBackByBerserk".ToHaxeString())
                    ? self.user.story.counters.get("kickedBackByBerserk".ToHaxeString()) : 0;
                if (kicked == 1 || config.Faulteffects)
                {
                    var getter = new HlFunc<double>(() => self.curLevel!.scroller.glitchAmount);
                    var setter = new HlAction<double>(v => self.curLevel!.scroller.glitchAmount = v);
                    self.tw.create_(getter, setter, 0, 1.0, new TType.TLoop(), 1500.0, Ref<bool>.Null);
                }
            }
            #endregion

            #region KatanaZero
            if (self.hero != null)
            {
                if (!self.hero.hasSkin(null, "KatanaZero".ToHaxeString()))
                {
                    self.hero.activeSkillsManager.resetPetItems();
                }
                else
                {
                    var kzCd = self.cd;
                    double cdms = 0.65 * kzCd.baseFps;
                    double frames = System.Math.Floor(cdms * 1000.0) / 1000.0;
                    int key = 247463936;
                    var cdInst = kzCd._getCdObject(key);

                    kzCd.fastCheck.set(key, 1);
                    if (cdInst != null)
                        cdInst.frames = frames;
                    else
                        kzCd.cdList.push(new dc.libs._Cooldown.CdInst(key, frames));

                    cdInst = kzCd._getCdObject(key);
                    if (cdInst != null)
                    {
                        cdInst.cb = new HlAction(() =>
                        {
                            var getter = new HlFunc<double>(() => self.curLevel!.scroller.vhsHShift);
                            var setter = new HlAction<double>(v => self.curLevel!.scroller.vhsHShift = v);
                            self.tw.create_(getter, setter, 0.8, 0.0, null, 350.0, Ref<bool>.Null);
                        });
                    }
                    self.hero.activeSkillsManager.resetPetItems();
                }
            }
            #endregion

            #region 疫病低血量重置
            if (self.user.br_hasInfection())
                self.infection?.resetIsLessThanTenPrctMobsLeft();
            #endregion

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
                var raw = allLevels.getDyn(i);
                var info = ((HaxeProxyBase)raw).ToVirtual<LevelIfor_Virtual>();
                var infoFields = (IHashlinkFieldObject)info.HashlinkObj;
                var group = (int)infoFields.GetFieldValue("group")!;
                if (group == 0)
                {
                    var flagsProps = infoFields.GetFieldValue("flagsProps");
                    if (flagsProps is IHashlinkFieldObject fpFields
                        && ((int)fpFields.GetFieldValue("genFlags")! & (1 << 16)) == 0)
                        candidates.Add(info);
                }
            }

            var map = new StringMap();

            int bonusCount = 4;
            for (int i = 0; i < bonusCount && candidates.Count > 0; i++)
            {
                int idx = Std.Class.random(candidates.Count);
                var chosen = candidates[idx];
                candidates.RemoveAt(idx);
                map.set(chosen.id, 1);
            }


            var imapVirtualType = HashlinkMarshal.Module.Types
                .OfType<HashlinkVirtualType>()
                .FirstOrDefault(t => t.Fields.Length == 7
                    && t.HasField("exists")
                    && t.HasField("get")
                    && t.HasField("set")
                    && t.HasField("keys")
                    && t.HasField("iterator")
                    && t.HasField("remove")
                    && t.HasField("toString"))
                ?? throw new InvalidOperationException();

            unsafe
            {
                var vptr = HashlinkNative.hl_to_virtual(imapVirtualType.NativeType, (HL_vdynamic*)map.HashlinkPointer);
                var virtualObj = (HashlinkVirtual)HashlinkMarshal.ConvertHashlinkObject(vptr)!;
                ((IHashlinkFieldObject)HashlinkMarshal.ConvertHashlinkObject(
                    HashlinkObjPtr.Get(((IHashlinkPointer)self).HashlinkPointer))!).SetFieldValue(
                    "bonusQuarterScrollLevels", virtualObj);
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
            rand.seed = rand.seed * 16807.0 % 2147483647.0;
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

        public void AddMimicRoom(LevelStruct Struct)
        {
            for (int i = 0; i < Data.Class.loreRoom.all.get_length(); i++)
            {
                var lore = Data.Class.loreRoom.all.array.getDyn(i);
                if (lore == null) return;
                if (dc._Data.LoreRoom_Impl_.Class.get_room(
                    ((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>()).id.ToString().EqualsIgnoreCase("MimicEscapedRoom"))
                {
                    Struct.tryAddLoreRoom(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>());
                }
            }
        }
        #endregion


        void IOnGameExit.OnGameExit()
        {

        }

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {
            cdblevel.Refresh();
        }
    }
}
