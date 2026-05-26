
using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities;
using dc;
using dc.haxe;
using dc.ui;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Modules
{
    public class KeyBindingModule : BaseModule,
        IOnHeroUpdate,
        IOnGameEndInit
    {
        public override string Description => GetText.Instance.GetString("KeyBinding");
        public override string Name => "KeyBinding";
        public override KeyConfig config => (KeyConfig)base.config;

        public override MenuCategory Type => MenuCategory.KeyBinding;

        private bool controllerInitialized;
        public ControllerHelperSuper<MainConfig> controller = null!;


        public override void Initialize(ModBase mainMod)
        {
            config = SettingsMain.ConfigValue.Key;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            if (SettingsMain.ModMenu().menu != MenuCategory.KeyBinding) return;

            menuHelper = new(options, SettingsMain.ModConfig);
            scrollerFlow = options.scrollerFlow;

            int stageWidth = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                    ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                    : dc.hxd.Window.Class.getInstance().get_width();


            double pixelScale = (double)options.get_pixelScale.Invoke();
            int paddingLeft = (int)(stageWidth * 0.1) + (int)(pixelScale * 40.0);
            scrollerFlow.set_paddingLeft(paddingLeft);

            int? targetWidth = (int)(stageWidth * 0.8);
            scrollerFlow.set_maxWidth(targetWidth);
            scrollerFlow.set_minWidth(targetWidth);

            options.addKeyboardWidget(
                 scrollerFlow,
                 options.cbmpScroller,
                 GetText.Instance.GetString("test").ToHaxeString(),
                 config.TheCurrentKey[KeyName.Tailor]
             );
        }

        public int GetAct(KeyName name)
        {
            return controller?.GetAction(name.ToString()) ?? int.MaxValue;
        }



        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (controller.IsPressed(GetAct(KeyName.Tailor)))
            {
                
            }
        }

        void IOnGameEndInit.OnGameEndInit()
        {
            if (controllerInitialized) return;
            controllerInitialized = true;

            controller = new ControllerHelperSuper<MainConfig>(SettingsMain.ModConfig, config.ControlKeys, Boot.Class.ME.controller);
            var act = controller.GetAction("Tailor") ?? controller.AddKey("Tailor", KeyHelper.T, KeyHelper.T, KeyHelper.T);
            config.TheCurrentKey[KeyName.Tailor] = act;
            controller.ApplyBindings();
        }
    }

    public enum KeyName
    {
        Tailor
    }
}