using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.haxe.ds;
using dc.tool;
using dc.tool._Cooldown;
using dc.ui;
using dc.ui.icon;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.API.Interfaces;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.cine;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Modules
{
    internal class KeyBindingModule : BaseModule,
        IOnHeroUpdate,
        IOnGameEndInit
    {
        public override string Description => GetText.Instance.GetString("KeyBinding");
        public override KeyConfig config => (KeyConfig)base.config;
        public override MenuCategory Type => MenuCategory.KeyBinding;
        public ControllerHelperSuper<MainConfig> controller = null!;
        public Hero? Hero { get => Game.Instance.HeroInstance; }



        /// <summary>
        /// keys
        /// </summary>
        const int TYPE_WALK = 106;
        const int WALKING = TYPE_WALK << 21;

        const int TYPE_CUSTOMS = 300;
        const int CUSTOM_Tailor = (TYPE_CUSTOMS << 21) | 0;

        public static int Tailor_KEY = CUSTOM_Tailor;

        int Tailor; int WalkLeft; int WalkRight;

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

            foreach (var opt in config.ControlKeys)
            {
                options.addKeyboardWidget(
                 scrollerFlow,
                 options.cbmpScroller,
                 GetDcString(opt.Value.Name),
                 opt.Value.act
                );
            }
        }


        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {

            var h = Hero;
            if (h == null || controller == null) return;

            if (controller.IsPressed(Tailor) && config.Enabled)
            {
                if (CheckTailorCd(h))
                    _ = new SummonTailor(h, h.cx, h.cy);
            }

            if (!config.Enabled) return;

            if (controller.IsDown(WalkLeft))
            {
                var cd = h.cd;
                var fastCheck = cd.fastCheck;

                h.dir = -1;
                UpdateWalkCooldown(fastCheck, cd, WALKING);
            }
            else if (controller.IsDown(WalkRight))
            {
                var cd = h.cd;
                var fastCheck = cd.fastCheck;

                h.dir = 1;
                UpdateWalkCooldown(fastCheck, cd, WALKING);
            }
        }

        private static void UpdateWalkCooldown(IntMap fastCheck, Cooldown cd, int key)
        {
            if (fastCheck.exists(key))
            {
                fastCheck.get(key).frames = 1;
            }
            else
            {
                var cdInst = new CdInst(key, 1);
                fastCheck.set(key, cdInst);
                cd.cdList.push(cdInst);
            }
        }

        void IOnGameEndInit.OnGameEndInit()
        {
            const string Tailor_KEY_NAME = "Tailor";
            const string KingWalkLeft_KEY_NAME = "KingWalkLeft";
            const string KingWalkRight_KEY_NAME = "KingWalkRight";

            var info = SettingsMain.Instance.Info;


            controller = new ControllerHelperSuper<MainConfig>(SettingsMain.ModConfig, config.ControlKeys, Boot.Class.ME.controller);
            if (config.ConfigVersion <= 1)
            {
                controller.RemoveKey(Tailor_KEY_NAME);
                config.ConfigVersion = 1.1;
                SaveConfig();
            }


            Tailor = controller.GetAction(Tailor_KEY_NAME) ?? controller.AddKey(info, Tailor_KEY_NAME);
            WalkLeft = controller.GetAction(KingWalkLeft_KEY_NAME) ?? controller.AddKey(info, KingWalkLeft_KEY_NAME);
            WalkRight = controller.GetAction(KingWalkRight_KEY_NAME) ?? controller.AddKey(info, KingWalkRight_KEY_NAME);

            EventSystem.BroadcastEvent<IInputApi, ControllerHelperSuper<MainConfig>>(controller);
        }

        public bool CheckTailorCd(Hero hero)
        {
            if (hero.cd.fastCheck.exists(CUSTOM_Tailor))
            {
                LogManager log = dc.pr.Game.Class.ME.log;
                dc.String str = $"时守还有{(int)((double)hero.cd.fastCheck.get(CUSTOM_Tailor).frames / hero.cd.baseFps)}秒冷却".ToHaxeString();
                log.textBmp(str, null, Icon.Class.createMobIcon("TimeKeeper".ToHaxeString(), null), null);
                AudioHelper.LoadAudioFormString("sfx/inter/pick_book.wav");
                return false;
            }
            return true;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Hero.preUpdate += Hook_Hero_preUpdate;
        }

        private void Hook_Hero_preUpdate(Hook_Hero.orig_preUpdate orig, Hero self)
        {
            orig(self);
            if (self.cd.fastCheck.exists(WALKING) && config.Enabled && controller.gamecontroller.gc == null)
                self.frameWalkSpd = 0.5;
        }
    }

}