
using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.ui;
using dc.ui.icon;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.cine;
using Serilog;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Modules
{
    public class KeyBindingModule : BaseModule,
        IOnHeroUpdate,
        IOnGameEndInit
    {
        public override string Description => GetText.Instance.GetString("KeyBinding");
        public override KeyConfig config => (KeyConfig)base.config;
        public override MenuCategory Type => MenuCategory.KeyBinding;
        public ControllerHelperSuper<MainConfig> controller = null!;


        public override void Initialize(ModBase mainMod)
        {
            config = SettingsMain.ConfigValue.Key;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            if (SettingsMain.ModMenu().menu != MenuCategory.KeyBinding) return;
            base.BuildMenu(options, Separator);

            int stageWidth = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                    ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                    : dc.hxd.Window.Class.getInstance().get_width();


            double pixelScale = (double)options.get_pixelScale.Invoke();
            int paddingLeft = (int)(stageWidth * 0.1) + (int)(pixelScale * 40.0);
            scrollerFlow.set_paddingLeft(paddingLeft);

            TopWidget.set_paddingRight((int)(paddingLeft * 1.75));
            TopWidget.set_paddingLeft(0);
            TopWidget.reflow();

            int? targetWidth = (int)(stageWidth * 0.8);
            scrollerFlow.set_maxWidth(targetWidth);
            scrollerFlow.set_minWidth(targetWidth);

            if (!config.Enabled)
                return;

            options.addKeyboardWidget(
                 scrollerFlow,
                 options.cbmpScroller,
                 GetText.Instance.GetString("Tailor").ToHaxeString(),
                 config.TheCurrentKey[KeyName.Tailor]
             );
        }

        public int GetAct(KeyName name)
        {
            return controller?.GetAction(name.ToString()) ?? int.MaxValue;
        }

        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (controller.IsPressed(GetAct(KeyName.Tailor)) && config.Enabled)
            {
                Hero hero = dc.pr.Game.Class.ME.hero;
                if (hero == null) return;
                if (CheckTailorCd(hero))
                    _ = new SummonTailor(hero, hero.cx, hero.cy);
            }
        }

        void IOnGameEndInit.OnGameEndInit()
        {
            controller = new ControllerHelperSuper<MainConfig>(SettingsMain.ModConfig, config.ControlKeys, Boot.Class.ME.controller);
            var act = controller.GetAction("Tailor") ?? controller.AddKey("Tailor", KeyHelper.T, KeyHelper.T, KeyHelper.T);
            config.TheCurrentKey[KeyName.Tailor] = act;
            controller.ApplyBindings();
        }

        public bool CheckTailorCd(Hero hero)
        {
            if (hero.cd.fastCheck.exists(1145140))
            {
                LogManager log = dc.pr.Game.Class.ME.log;
                dc.String str = $"时守还有{(int)((double)hero.cd.fastCheck.get(1145140).frames / hero.cd.baseFps)}秒冷却".ToHaxeString();
                log.textBmp(str, null, Icon.Class.createMobIcon("TimeKeeper".ToHaxeString(), null), null);
                AudioHelper.LoadAudioFormString("sfx/inter/pick_book.wav");
                return false;
            }
            return true;
        }
    }

    public enum KeyName
    {
        Tailor
    }
}