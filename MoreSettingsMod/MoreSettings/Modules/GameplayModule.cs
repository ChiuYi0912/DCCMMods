using System;
using CoreLibrary.Core.Extensions;
using dc;
using dc.cine;
using dc.en;
using dc.level;
using dc.tool;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Mods;
using ModCore.Utilities;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;
using Hook_Game = dc.pr.Hook_Game;

namespace MoreSettings.Modules
{
    public class GameplayModule : BaseModule
    {
        public override string Name => "Gameplay";
        public override string Description => "游戏玩法设置";
        public override GameplayConfig config => (GameplayConfig)base.config;

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
        }

        public override void BuildMenu(dc.ui.Options options)
        {
            base.BuildMenu(options);
            options.addSeparator(Description.AsHaxeString(), options.scrollerFlow);
            var scrollerFlow = options.scrollerFlow;

            menuHelper.AddConfigToggle(
                "禁用命中暂停",
                "禁用击中敌人时的慢动作效果",
                () => config.Hitpause,
                v => config.Hitpause = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                "启用速递层效果",
                "启用快速物品拾取动画",
                () => config.SpeedTier,
                v => config.SpeedTier = v,
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                "启用图书馆拟态房间",
                "在图书馆中生成拟态房间",
                () => config.LoreBankMimicRoom,
                v => config.LoreBankMimicRoom = v,
                scrollerFlow
            );
        }

        #region 钩子实现

        private void Hook__TierItemFound__constructor__(
            Hook__TierItemFound.orig___constructor__ orig,
            TierItemFound arg1, Hero hero, Entity e, InventItem item,
            double iconX, double iconY, HlAction<bool> onComplete)
        {
            if (config.SpeedTier)
            {

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
        #endregion
    }
}
