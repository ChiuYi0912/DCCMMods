using System.Runtime.CompilerServices;
using CoreLibrary.Core.Extensions;
using dc;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.hl.types;
using dc.level;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics;
using static MoreSettings.Configuration.Enums;
using Hook_Game = dc.pr.Hook_Game;

namespace MoreSettings.Modules
{
    public class GameplayModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Gameplay");
        public override GameplayConfig config => (GameplayConfig)base.config;

        public override MenuCategory Type => MenuCategory.Gameplay;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ModConfig.Value.Gameplay;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Game.decreasingSlowMo += Hook_Game_decreasingSlowMo;
            Hook_LevelStruct.applyDifficulty += Hook__LevelStruct_applyDifficulty;
            Hook__TierItemFound.__constructor__ += Hook__TierItemFound__constructor__;
            Hook__LevelTransition.__constructor__ += Hook__LevelTransition__constructor__;
            Hook__EntranceTeleportation.__constructor__ += Hook__EntranceTeleportation__constructor__;
        }



        public override void UnregisterHooks()
        {
            Hook_Game.decreasingSlowMo -= Hook_Game_decreasingSlowMo;
            Hook_LevelStruct.applyDifficulty -= Hook__LevelStruct_applyDifficulty;
            Hook__TierItemFound.__constructor__ -= Hook__TierItemFound__constructor__;
            Hook__LevelTransition.__constructor__ -= Hook__LevelTransition__constructor__;
            Hook__EntranceTeleportation.__constructor__ -= Hook__EntranceTeleportation__constructor__;
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            menuHelper.addSimpleWidget(
              GetText.Instance.GetString("KeyBinding"),
               "",
               new Action(() =>
               {
                   var menu = SettingsMain.ModMenu();
                   menu.menu = MenuCategory.KeyBinding;
                   MenuModule.Instance.SetSection(menu);
                   menu.menu = MenuCategory.All;
               }),
               scrollerFlow
            );

            if (!config.Enabled)
                return;


            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("HitPause"),
                GetText.Instance.GetString("HitPauseDesc"),
                () => config.Hitpause,
                v => config.Hitpause = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("SpeedTier"),
                GetText.Instance.GetString(""),
                () => config.SpeedTier,
                v => config.SpeedTier = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("MimicLoreRoom"),
                GetText.Instance.GetString(""),
                () => config.LoreBankMimicRoom,
                v => config.LoreBankMimicRoom = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("NoFadeIn"),
                GetText.Instance.GetString("NofadeInDesc"),
                () => config.NofadeIn,
                v => config.NofadeIn = v,
                scrollerFlow
            );
        }

        #region Hooks

        private void Hook__TierItemFound__constructor__(
            Hook__TierItemFound.orig___constructor__ orig,
            TierItemFound arg1, Hero hero, Entity e, InventItem item,
            double iconX, double iconY, HlAction<bool> onComplete)
        {
            if (config.SpeedTier)
            {
                _ = new SpeedTier(hero, e, item);
                return;
            }
            orig(arg1, hero, e, item, iconX, iconY, onComplete);
        }

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

        private void Hook_Game_decreasingSlowMo(
            Hook_Game.orig_decreasingSlowMo orig, dc.pr.Game self,
            double durationS, double spd)
        {
            if (config.Hitpause) return;
            orig(self, durationS, spd);
        }

        private void Hook__LevelTransition__constructor__(
            Hook__LevelTransition.orig___constructor__ orig,
            LevelTransition arg1,
            dc.String mainId,
            LevelMap map,
            int? linkId,
            CPoint heroPosAfterBossRuneReload,
            Ref<bool> noLoadingData)
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
                throw new InvalidOperationException("Forbidden level reached. This level should not be accessible.");

            DLC.Class.installMaskCacheDirty = true;

            double multFade = 0.5;
            if (mainId == null || heroPosAfterBossRuneReload != null)
                multFade *= 0.5;
            arg1.multFade = multFade;

            dc.pr.Game.Class.ME.controller.manualLock = true;

            var curLevel = dc.pr.Game.Class.ME.curLevel;
            if (curLevel != null)
            {
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
                if (mainId != null && mainId.ToString() != "PrisonStart" && curLevel.map.id.ToString() == "PrisonStart")
                {
                    var doors = (ArrayObj)curLevel.entitiesByClass.get(30924);
                    bool allIntact = true;
                    foreach (var obj in doors)
                    {
                        if (obj is Door door && door.broken)
                        {
                            allIntact = false;
                            break;
                        }
                    }
                    giveGentleman = allIntact;
                }
                arg1.giveGentlemanAchievement = giveGentleman;

                if (!config.NofadeIn || linkId == null)
                    Main.Class.ME.fadeIn(
                        null,
                        transitionData,
                        Ref<double>.In(multFade),
                        onEnd: new HlAction(() => arg1.loadNewLevel())
                    );
                else
                    arg1.loadNewLevel();

                arg1.disableBars();
            }
            else
            {
                arg1.loadNewLevel();
                arg1.disableBars();
            }
        }

        private void Hook__EntranceTeleportation__constructor__(Hook__EntranceTeleportation.orig___constructor__ orig,
           EntranceTeleportation arg1, Hero hero, Entity teleporter, CPoint t, LevelMap map, int? linkId)
           => _ = new EntranceTeleportationCustom(hero, teleporter, t, map, linkId);



        #endregion

        #region 辅助方法

        public void AddMimicRoom(LevelStruct Struct)
        {
            for (int i = 0; i < Data.Class.loreRoom.all.get_length(); i++)
            {
                dynamic lore = Data.Class.loreRoom.all.array.getDyn(i);
                if (lore == null) return;
                if (dc._Data.LoreRoom_Impl_.Class.get_room(
                    ((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>()).id.ToString().EqualsIgnoreCase("MimicEscapedRoom"))
                {
                    Struct.tryAddLoreRoom(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>());
                }
            }
        }

        public void TestRoom(LevelStruct Struct)
        {
            for (int i = 169; i <= 169; i++)
            {
                dynamic lore = Data.Class.loreRoom.all.array.getDyn(i);
                if (lore == null) return;
                lore.levels.push("PrisonStart".AsHaxeString());
                Struct.tryAddLoreRoom(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>());
            }
        }



        #endregion
    }
}
