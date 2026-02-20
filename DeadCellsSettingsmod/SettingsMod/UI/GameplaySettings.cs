using dc;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Hook_Options = dc.ui.Hook_Options;
using Options = dc.ui.Options;
using System;
using dc.h2d;
using Serilog;
using dc.tool.mod;
using dc.tool;
using dc.pr;
using dc.libs.heaps.slib;
using dc.en;
using ModCore.Modules;
using dc.libs.heaps;
using ChiuYiUI.UI;
using ChiuYiUI.Core;
using dc.level;
using dc.cine;
using ChiuYiUI.GameMechanics;
using ModCore.Menu;
using ModCore.Events;
using dc.hl.types;

namespace ChiuYiUI.UI
{
    public class GameplaySettings
    {
        public GameplaySettings()
        {
            InitializeHooks();
        }
        public void InitializeHooks()
        {
            Hook_Game.decreasingSlowMo += Hook_Game_decreasingSlowMo;
            Hook_LevelStruct.applyDifficulty += Hook__LevelStruct_applyDifficulty;
            Hook__TierItemFound.__constructor__ += Hook__TierItemFound__constructor__;

            Hook_NewsPanel.updateVisible += Hook_NewsPanel_updateVisible;
            Hook_NewsPanel.focusIn += Hook_NewsPanel_focusIn;
            Hook_NewsPanel.update += Hook_NewsPanel_update;
        }

        public void AddGameplaySettings(Options self)
        {
            AddUIModificationSettings(self);
        }

        private void AddUIModificationSettings(Options self)
        {
            Flow scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("UI界面修改").AsHaxeString(), scrollerFlow);
            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> scraf = () =>
            {
                bool newValue = !ChiuYiMain.config.Value.NewsPanel;
                ChiuYiMain.config.Value.NewsPanel = newValue;
                ChiuYiMain.config.Save();
                if (newValue)
                {

                }
                return newValue;
            };
            bool scarf = ChiuYiMain.config.Value.NewsPanel;
            ref bool sf = ref scarf;
            self.addToggleWidget(
                GetText.Instance.GetString("删除主页新闻steam面板").AsHaxeString(),
                GetText.Instance.GetString("下一次打开游戏时生效").AsHaxeString(),
                scraf,
                Ref<bool>.From(ref sf),
                scrollerFlow);
        }

        private void Hook__TierItemFound__constructor__(Hook__TierItemFound.orig___constructor__ orig, TierItemFound arg1, Hero hero, Entity e, InventItem item, double iconX, double iconY, HlAction<bool> onComplete)
        {
            if (ChiuYiMain.config.Value.SpeedTier)
            {
                new SpeedTier(hero, e, item);
                return;
            }
            orig(arg1, hero, e, item, iconX, iconY, onComplete);
        }

        private void Hook__LevelStruct_applyDifficulty(Hook_LevelStruct.orig_applyDifficulty orig, LevelStruct self)
        {
            if (dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel && ChiuYiMain.config.Value.LoreBankMimicRoom && Main.Class.ME.options.disableLoreRooms)
            {
                AddMimicRoom(self);
            }
            orig(self);
        }

        public void AddMimicRoom(LevelStruct Struct)
        {
            for (int i = 0; i < Data.Class.loreRoom.all.get_length(); i++)
            {
                dynamic lore = Data.Class.loreRoom.all.array.getDyn(i);
                if (lore == null) return;
                if (dc._Data.LoreRoom_Impl_.Class.get_room(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>()).id.ToString() == "MimicEscapedRoom")
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

        private void Hook_NewsPanel_update(Hook_NewsPanel.orig_update orig, NewsPanel self)
        {
            if (ChiuYiMain.config.Value.NewsPanel) return;
            orig(self);
        }

        private void Hook_NewsPanel_focusIn(Hook_NewsPanel.orig_focusIn orig, NewsPanel self)
        {
            if (ChiuYiMain.config.Value.NewsPanel) return;

            orig(self);
        }

        private void Hook_NewsPanel_updateVisible(Hook_NewsPanel.orig_updateVisible orig, NewsPanel self)
        {
            if (ChiuYiMain.config.Value.NewsPanel) return;
            orig(self);
        }

        private void Hook_Game_decreasingSlowMo(Hook_Game.orig_decreasingSlowMo orig, dc.pr.Game self, double durationS, double spd)
        {
            if (ChiuYiMain.config.Value.Hitpause) return;
            orig(self, durationS, spd);
        }
    }
}