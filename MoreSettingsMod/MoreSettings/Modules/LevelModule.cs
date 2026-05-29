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
using HashlinkNET.Native.Impl;
using dc.en.inter;
using MoreSettings.GameMechanics.Preload;
using dc.chroma.effects;
using ModCore.Events.Interfaces.Game;
using MoreSettings.Utilities;
using ModCore.Serialization;
using ModCore.Storage;
using ModCore.Events;

namespace MoreSettings.Modules
{
    public class LevelModule : BaseModule,
    IOnGameExit,
    IHxbitSerializable,
    IEventReceiver
    {
        public override Enums.MenuCategory Type => Enums.MenuCategory.Level;
        public override string Description => GetText.Instance.GetString("LevelModule");
        public override string Name => "LevelModule";
        public override LevelConfig config => (LevelConfig)base.config;

        public override void Initialize(ModBase mainMod)
        {
            config = SettingsMain.ConfigValue.level;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("NoFadeIn"),
                GetText.Instance.GetString("NofadeInDesc"),
                () => config.NofadeIn,
                v => config.NofadeIn = v,
                scrollerFlow);
            if (!config.Enabled) return;
        }

        #region Hooks
        public override void PermanentlyRegisterHooks()
        {
            base.PermanentlyRegisterHooks();
            Hook_Game.loadMainLevel += Hook_Game_loadMainLevel;
            Hook_LevelTransition.loadNewLevel += Hook_LevelTransition_loadNewLevel;
            Hook__LevelTransition.__constructor__ += Hook__LevelTransition__constructor__;
            Hook_Game.activateSubLevel += Hook_Game_activateSubLevel;
            Hook_Level.resume += Hook_Level_resume;
            Hook_LevelDisp.render += Hook_LevelDisp_render;
        }
        #endregion

        #region Disprender
        private void Hook_LevelDisp_render(Hook_LevelDisp.orig_render orig, LevelDisp self)
        {
            if (self.rendered)
                throw new InvalidOperationException("Render called twice without a clear()");
            self.rendered = true;

            Boot.Class.tryRender();
            self.renderParallaxes();
            Boot.Class.tryRender();
            if (PreloadLevels.GetCachedDecoZones(self.lmap.id.ToString()) == null)
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

            // 优先使用预加载缓存
            ArrayObj decoZones = PreloadLevels.GetCachedDecoZones(self.lmap.id.ToString())
                                 ?? self.parseDecoZones();
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
            Main.Class.ME.fadeOut(Ref<double>.In(0));

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
                multFade = 0;
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
                bool activate = true;
                int? seed = dc.pr.Game.Class.ME.curLevel.map?.seed;
                dc.pr.Game.Class.ME.loadMainLevel(self, self.mainId, Ref<bool>.In(activate), seed);
            }
            else if (self.mainId != null)
            {
                dc.pr.Game.Class.ME.loadMainLevel(self, self.mainId, default, null);
            }
            else
            {
                bool activate = true;
                dc.pr.Game.Class.ME.activateSubLevel(self.map, self.linkId,
                    Ref<bool>.In(activate), Ref<bool>.In(self.playAfterZDoorCine));
            }

            if (self.get_isADlcPLevel())
                self.onEnteredLevel = new HlAction(() =>
                    new PurpleLevelIntro(self.mainId, dc.pr.Game.Class.ME.hero));

            //Main.Class.ME.fadeOut(Ref<double>.In(0));
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
            Hook_Game.orig_loadMainLevel orig, dc.pr.Game self,
            LevelTransition cine, dc.String id,
            Ref<bool> activate, int? forcedSeed)
        {
            PreloadLevels.loadMainLevel(orig, self, cine, id, activate, forcedSeed);
        }

        void IOnGameExit.OnGameExit()
        {
            PreloadLevels.ClearCache();
            Logger.Information("缓存已清除");
        }

    }
}
