using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Utilities;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.tool;
using dc.ui;
using EnemiesVsEnemies.Configuration;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Menu;
using ModCore.Modules;
using ModCore.Utilities;

namespace EnemiesVsEnemies.UI
{
    public class ShowEnemiesOptsions :
    IModMenu,
    IEventReceiver,
    IOnHookInitialize,
    IOnHeroUpdate
    {
        public ModConfig config = EnemiesVsEnemiesMod.GetConfig();

        public const int SpawnEnemyTriggerAct = 42;

        public ShowEnemiesOptsions()
        {
            EventSystem.AddReceiver(this);
            if (!config.ControlKeys.ContainsKey(SpawnEnemyTriggerAct))
                config.defaultValues();
            EnemiesVsEnemiesMod.config.Save();
        }

        void IModMenu.BuildMenu(dc.ui.Options options)
        {
            Graphics graphics = options.createScroller(0.75);
            Flow scrollerFlow = options.scrollerFlow;

            options.addSeparator(GetText.Instance.GetString("按键设置").ToHaxeString(), scrollerFlow);


            int stageWidth = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                : dc.hxd.Window.Class.getInstance().get_width();


            double pixelScale = (double)options.get_pixelScale.Invoke();
            int paddingLeft = (int)(stageWidth * 0.1) + (int)(pixelScale * 40.0);
            scrollerFlow.set_paddingLeft(paddingLeft);

            int? targetWidth = (int)(stageWidth * 0.8);
            scrollerFlow.set_maxWidth(targetWidth);
            scrollerFlow.set_minWidth(targetWidth);

            options.title.set_text(GetText.Instance.GetString("EnemiesVsEnemies模组设置").ToHaxeString());

            options.addKeyboardWidget(scrollerFlow, options.cbmpScroller, GetText.Instance.GetString("生成触发器暴徒").ToHaxeString(), 42);
        }

        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (IsControllerLocked())
                return;

            if (ControllerHelper.ControlsUpdateFromProcess(Boot.Class.ME.controller, SpawnEnemyTriggerAct))
            {
                foreach (var mobs in config.Teams)
                {
                    EnemiesVsEnemiesMod.GetEnemySpawner().SpawnDefaultEnemiesForTeam(mobs.Value.Id);
                }
                #if true
                EnemiesVsEnemiesMod.GetLogger.Information("Test key SpawnEnemyTriggerAct pressed!");
                #endif
            }

        }

        public static void LockContoreLible(bool lockState) { EnemiesVsEnemiesMod.GetConfig().General.IslockedController = lockState; EnemiesVsEnemiesMod.config.Save(); }
        public static bool IsControllerLocked() => EnemiesVsEnemiesMod.GetConfig().General.IslockedController;
        public string GetName() { return "EnemiesVsEnemies模组设置"; }
        public string? GetSubText() { return $"version: {EnemiesVsEnemiesMod.GetVersion()}"; }


        void IOnHookInitialize.HookInitialize()
        {
            dc.Hook_Options.setKeyMapping += Hook_Options_setKeyMapping;
            dc.Hook__Options.dumpControllerConfig += Hook__Options_dumpControllerConfig;
            dc.ui.Hook_TextInput.cancel += Hook_TextInput_cancel;
        }

        private void Hook_TextInput_cancel(dc.ui.Hook_TextInput.orig_cancel orig, dc.ui.TextInput self)
        {
            orig(self);
            LockContoreLible(false);
        }

        private void Hook__Options_dumpControllerConfig(dc.Hook__Options.orig_dumpControllerConfig orig, object _gamepad, object _keyboard, bool isNormalBindings)
        {
            orig(_gamepad, _keyboard, isNormalBindings);
            BuildKeyMappingFormConfig();
        }

        private void Hook_Options_setKeyMapping(dc.Hook_Options.orig_setKeyMapping orig, dc.Options self, int action, int idx, int? key)
        {
            if (config.ControlKeys.ContainsKey(action))
            {
                ContorlLbleKeyConfig contorlLbleKey = config.ControlKeys[action];
                contorlLbleKey.act = action;
                if (idx == 0)
                    contorlLbleKey.Primary = key;
                else if (idx == 1)
                    contorlLbleKey.Secondary = key;
                else if (idx == 2)
                    contorlLbleKey.Third = key;
                ControllerHelper.SetKeyBindingHeroContorlLble(contorlLbleKey);
                EnemiesVsEnemiesMod.config.Save();

                return;
            }
            orig(self, action, idx, key);
        }


        private void BuildKeyMappingFormConfig()
        {
            if (config.ControlKeys.TryGetValue(SpawnEnemyTriggerAct, out var spawnEnemyTriggerKey))
            {
                ControllerHelper.SetKeyBindingHeroContorlLble(spawnEnemyTriggerKey);
                EnemiesVsEnemiesMod.config.Save();
            }
        }
    }
}