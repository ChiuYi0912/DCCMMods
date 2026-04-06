using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Utilities;
using dc;
using dc.en;
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
        public const int DestroyMob = 43;
        public const int GotoMyQueenLevel = 44;

        public ShowEnemiesOptsions()
        {
            EventSystem.AddReceiver(this);
            int[] ints = [SpawnEnemyTriggerAct, DestroyMob, GotoMyQueenLevel];
            for (int i = 0; i < ints.Length; i++)
            {
                if (!config.ControlKeys.ContainsKey(ints[i]))
                {
                    config.defaultValues();
                    break;
                }
            }
            EnemiesVsEnemiesMod.config.Save();
        }

        void IModMenu.BuildMenu(dc.ui.Options options)
        {
            Graphics graphics = options.createScroller(1);
            Flow scrollerFlow = options.scrollerFlow;

            options.addSeparator(GetText.Instance.GetString("Button settings").ToHaxeString(), scrollerFlow);


            int stageWidth = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                : dc.hxd.Window.Class.getInstance().get_width();


            double pixelScale = (double)options.get_pixelScale.Invoke();
            int paddingLeft = (int)(stageWidth * 0.1) + (int)(pixelScale * 40.0);
            scrollerFlow.set_paddingLeft(paddingLeft);

            int? targetWidth = (int)(stageWidth * 0.8);
            scrollerFlow.set_maxWidth(targetWidth);
            scrollerFlow.set_minWidth(targetWidth);

            options.title.set_text(GetText.Instance.GetString("EnemiesVsEnemiesMod").ToHaxeString());

            options.addKeyboardWidget(scrollerFlow, options.cbmpScroller, GetText.Instance.GetString("Generate Trigger Mobs").ToHaxeString(), SpawnEnemyTriggerAct);
            options.addKeyboardWidget(scrollerFlow, options.cbmpScroller, GetText.Instance.GetString("Destroy the generated Mobs").ToHaxeString(), DestroyMob);
            options.addKeyboardWidget(scrollerFlow, options.cbmpScroller, GetText.Instance.GetString("Goto QueenLevel").ToHaxeString(), GotoMyQueenLevel);



            HlAction<double> defaultValue = new((double v) =>
           {
               config.General.Camerazoom = v;
               EnemiesVsEnemiesMod.config.Save();
               if (dc.pr.Game.Class.ME.curLevel == null)
                   return;
               dc.pr.Game.Class.ME.curLevel.viewport.zoom = v;
           });
            options.addSliderWidget(
               "CameraZoom".ToHaxeString(),
               defaultValue,
               config.General.Camerazoom,
               Ref<double>.In(0.01),
               scrollerFlow,
               Ref<bool>.In(false),
               Ref<bool>.In(true),
               Ref<double>.In(0.5),
               Ref<double>.In(1.5),
               null,
               Ref<int>.Null);
        }

        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (IsControllerLocked())
                return;

            if (ControllerHelper.ControlsUpdateFromProcess(Boot.Class.ME.controller, DestroyMob))
            {
                foreach (var mobs in EnemiesVsEnemiesMod.GetEnemySpawner().CreatedMobs)
                {
                    if (!mobs.destroyed)
                    {
                        mobs.kill();
                    }
                }
                EnemiesVsEnemiesMod.GetEnemySpawner().CreatedMobs.Clear();
            }

            if (ControllerHelper.ControlsUpdateFromProcess(Boot.Class.ME.controller, GotoMyQueenLevel))
            {
                Hero hero = Game.Instance.HeroInstance!;
                if (hero == null)
                    return;
                if (!hero._level.map.id.ToString().EqualsIgnoreCase("DebugRTC"))
                    dc.cine.LevelTransition.Class.@goto("DebugRTC".ToHaxeString());
            }
        }

        public static void LockContoreLible(bool lockState) { EnemiesVsEnemiesMod.GetConfig().General.IslockedController = lockState; EnemiesVsEnemiesMod.config.Save(); }
        public static bool IsControllerLocked() => EnemiesVsEnemiesMod.GetConfig().General.IslockedController;
        public string GetName() { return "EnemiesVsEnemies Settings"; }
        public string? GetSubText() { return $"version: {EnemiesVsEnemiesMod.GetVersion()}"; }


        void IOnHookInitialize.HookInitialize()
        {
            dc.Hook_Options.setKeyMapping += Hook_Options_setKeyMapping;
            dc.Hook__Options.dumpControllerConfig += Hook__Options_dumpControllerConfig;
            dc.ui.Hook_TextInput.cancel += Hook_TextInput_cancel;
            Hook_Viewport.update += Hook_Viewport_update;
        }

        private void Hook_Viewport_update(Hook_Viewport.orig_update orig, Viewport self)
        {
            orig(self);
            if (self.zoom != config.General.Camerazoom)
                self.zoom = config.General.Camerazoom;
        }

        private void Hook_TextInput_cancel(dc.ui.Hook_TextInput.orig_cancel orig, dc.ui.TextInput options)
        {
            orig(options);
            LockContoreLible(false);
        }

        private void Hook__Options_dumpControllerConfig(dc.Hook__Options.orig_dumpControllerConfig orig, object _gamepad, object _keyboard, bool isNormalBindings)
        {
            orig(_gamepad, _keyboard, isNormalBindings);
            BuildKeyMappingFormConfig();
        }

        private void Hook_Options_setKeyMapping(dc.Hook_Options.orig_setKeyMapping orig, dc.Options options, int action, int idx, int? key)
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
            orig(options, action, idx, key);
        }


        private void BuildKeyMappingFormConfig()
        {
            foreach (var item in config.ControlKeys)
            {
                ControllerHelper.SetKeyBindingHeroContorlLble(item.Value);
                EnemiesVsEnemiesMod.config.Save();
            }
        }
    }
}