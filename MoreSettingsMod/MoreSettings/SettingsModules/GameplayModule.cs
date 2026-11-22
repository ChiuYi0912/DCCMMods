using dc;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.level;
using dc.pr;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics;
using MoreSettings.GameMechanics.cine;
using static MoreSettings.Configuration.Enums;
using Hook_Game = dc.pr.Hook_Game;

namespace MoreSettings.Modules
{
    internal class GameplayModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Gameplay");
        public override GameplayConfig config => (GameplayConfig)base.config;
        public override MenuCategory Type => MenuCategory.Gameplay;
        //判断玩家是否在电梯上
        private bool HeroIsSnappedToElevator = false;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ModConfig.Value.Gameplay;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Game.decreasingSlowMo += Hook_Game_decreasingSlowMo;

            Hook__TierItemFound.__constructor__ += Hook__TierItemFound__constructor__;
            Hook__EntranceTeleportation.__constructor__ += Hook__EntranceTeleportation__constructor__;
            Hook_Elevator.fixedUpdate += Hook_Elevator_fixedUpdate;
            Hook_Game.modalPause += Hook_Game_modalPause;
        }

        public override void UnregisterHooks()
        {
            Hook_Game.decreasingSlowMo -= Hook_Game_decreasingSlowMo;
            Hook__TierItemFound.__constructor__ -= Hook__TierItemFound__constructor__;
            Hook__EntranceTeleportation.__constructor__ -= Hook__EntranceTeleportation__constructor__;
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            menuHelper.addSimpleWidget(
              GetString("KeyBinding"),
               "",
               new Action(() =>
               {
                   var menu = SettingsMain.ModMenu();
                   menu.menu = MenuCategory.KeyBinding;
                   MenuModule.Instance.SetSection(menu);
               }),
               scrollerFlow
            );

            if (!config.Enabled)
                return;


            menuHelper.AddConfigToggle(
                GetString("HitPause"),
                GetString("HitPauseDesc"),
                () => config.Hitpause,
                v => config.Hitpause = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("SpeedTier"),
                GetString(""),
                () => config.SpeedTier,
                v => config.SpeedTier = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("ElevatorNoPause"),
                GetString("ElevatorNoPauseDesc"),
                () => config.NoPauseGameOnElevator,
                v => config.NoPauseGameOnElevator = v,
                scrollerFlow
            );
        }

        #region Hooks

        private void Hook_Game_modalPause(Hook_Game.orig_modalPause orig, dc.pr.Game self, Ref<bool> audioEffect)
        {
            if (config.NoPauseGameOnElevator && HeroIsSnappedToElevator)
            {
                return;
            }

            orig(self, audioEffect);
        }

        private void Hook_Elevator_fixedUpdate(Hook_Elevator.orig_fixedUpdate orig, Elevator self)
        {
            orig(self);

            if (!config.NoPauseGameOnElevator)
            {
                return;
            }

            HeroIsSnappedToElevator = false;

            var game = dc.pr.Game.Class.ME;
            if (game == null || game.hero == null || self == null)
                return;

            Hero hero = game.hero;

            if (self.snappeds != null)
            {
                for (int i = 0; i < self.snappeds.length; i++)
                {
                    Entity e = self.snappeds.getDyn(i);
                    if (e == null)
                        break;

                    if (e == hero)
                    {
                        HeroIsSnappedToElevator = true;
                        break;
                    }
                }
            }
        }

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



        private void Hook_Game_decreasingSlowMo(
            Hook_Game.orig_decreasingSlowMo orig, dc.pr.Game self,
            double durationS, double spd)
        {
            if (config.Hitpause) return;
            orig(self, durationS, spd);
        }


        private void Hook__EntranceTeleportation__constructor__(Hook__EntranceTeleportation.orig___constructor__ orig,
           EntranceTeleportation arg1, Hero hero, Entity teleporter, CPoint t, LevelMap map, int? linkId)
           => _ = new EntranceTeleportationCustom(hero, teleporter, t, map, linkId);



        #endregion

        #region 辅助方法
        public void TestRoom(LevelStruct Struct)
        {
            for (int i = 169; i <= 169; i++)
            {
                var lore = Data.Class.loreRoom.all.array.getDyn(i);
                if (lore == null) return;
                lore.levels.push("PrisonStart".AsHaxeString());
                Struct.tryAddLoreRoom(((HaxeProxyBase)lore).ToVirtual<virtual_Intention_arc_examinables_fxEmitters_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>());
            }
        }
        #endregion
    }
}
