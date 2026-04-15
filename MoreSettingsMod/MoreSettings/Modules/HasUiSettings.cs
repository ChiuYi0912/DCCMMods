using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.pr;
using dc.tool.atk;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;

namespace MoreSettings.Modules
{
    public class HasUiSettings : BaseModule
    {
        public override string Description => "UI界面修改";

        public override UIConfig config => (UIConfig)base.config;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ConfigValue.UI;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            dc.pr.Hook_Game.init += Hook_Game_init;
            Hook__GameCinematic.__constructor__ += Hook__GameCinematic___constructor__;
            Hook_Entity.popDamage += Hook_Entity_popDamage;

            dc.ui.hud.Hook_LifeBar.getFullName += Hook_LifeBar_GetFullName;
            dc.ui.hud.Hook_LifeBar.getStartEndName += Hook_LifeBar_getStartEndName;


            Hook__HUD.__constructor__ += Hook_HUD___constructor__;
            Hook_HUD.onResize += Hook_HUD_onResize;
            Hook_HUD.postUpdate += Hook_HUD_PostUpdate;


            Hook_NewsPanel.updateVisible += Hook_NewsPanel_updateVisible;
            Hook_NewsPanel.focusIn += Hook_NewsPanel_focusIn;
            Hook_NewsPanel.update += Hook_NewsPanel_update;
        }



        public override void UnregisterHooks()
        {
            dc.pr.Hook_Game.init -= Hook_Game_init;
            Hook__GameCinematic.__constructor__ -= Hook__GameCinematic___constructor__;
            Hook_Entity.popDamage -= Hook_Entity_popDamage;

            dc.ui.hud.Hook_LifeBar.getFullName -= Hook_LifeBar_GetFullName;
            dc.ui.hud.Hook_LifeBar.getStartEndName -= Hook_LifeBar_getStartEndName;


            Hook__HUD.__constructor__ -= Hook_HUD___constructor__;
            Hook_HUD.onResize -= Hook_HUD_onResize;
            Hook_HUD.postUpdate -= Hook_HUD_PostUpdate;


            Hook_NewsPanel.updateVisible -= Hook_NewsPanel_updateVisible;
            Hook_NewsPanel.focusIn -= Hook_NewsPanel_focusIn;
            Hook_NewsPanel.update -= Hook_NewsPanel_update;
        }



        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            if (!config.Enabled)
                return;

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("删除主页新闻steam面板"),
                GetText.Instance.GetString("下一次打开游戏时生效"),
                () => config.NewsPanel,
                v => config.NewsPanel = v,
                scrollerFlow);


            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("移除电影黑边"),
                GetText.Instance.GetString("移除过场动画电影黑边"),
                () => config.HasBottomBar,
                v => config.HasBottomBar = v,
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("禁用暗角效果"),
                GetText.Instance.GetString("例如：受伤时的暗角效果"),
                () => config.NoVignette,
                v =>
                {
                    SetConsoleFlag(v, "noVignette");
                    config.NoVignette = v;
                },
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("轻量显示"),
                GetText.Instance.GetString("禁用交互图标等等"),
                () => config.HaslightTip,
                v =>
                {
                    SetConsoleFlag(v, "lightTip");
                    config.HaslightTip = v;
                },
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("无弹出文字"),
                GetText.Instance.GetString("禁用伤害弹出文字"),
                () => config.HasNoPopText,
                v =>
                {
                    SetConsoleFlag(v, "NoPopText");
                    config.HasNoPopText = v;
                },
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("Time"),
                GetText.Instance.GetString(""),
                () => config.NowTimeVisible,
                v => config.NowTimeVisible = v,
                scrollerFlow
            );

            options.addSeparator(GetText.Instance.GetString("血条颜色").ToHaxeString(), scrollerFlow);
            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("血条颜色描述"),
                () => config.LifeBarcolor,
                v => config.LifeBarcolor = v,
                maxValue: 6,
                scrollerFlow: scrollerFlow
            );
        }

        #region 钩子实现
        private void Hook_Game_init(Hook_Game.orig_init orig, dc.pr.Game self)
        {
            orig(self);
            SetConsoleFlag(config.HasBottomBar, "noVignette");
            SetConsoleFlag(config.HaslightTip, "lightTip");
            SetConsoleFlag(config.HasNoPopText, "NoPopText");
        }

        private void Hook__GameCinematic___constructor__(Hook__GameCinematic.orig___constructor__ orig, GameCinematic arg1)
        {
            orig(arg1);
            arg1.bottomBar.set_visible(!config.HasBottomBar);
            arg1.topBar.set_visible(!config.HasBottomBar);
        }

        private DateTime _lastUpdateTime = DateTime.MinValue;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);



        private void Hook_HUD_PostUpdate(Hook_HUD.orig_postUpdate orig, HUD self)
        {
            orig(self);
            if (NowTimeFlow == null)
                return;
            if (this.NowTimeFlow.visible != config.NowTimeVisible)
                this.NowTimeFlow.set_visible(config.NowTimeVisible);



            if (!this.NowTimeFlow.visible)
                return;

            DateTime now = DateTime.Now;
            if (now - _lastUpdateTime >= _updateInterval)
            {
                this.TimeText.set_text($"{now}".ToHaxeString());

                _lastUpdateTime = now;
            }
        }



        private void Hook_HUD_onResize(Hook_HUD.orig_onResize orig, HUD self)
        {
            orig(self);

            if (this.TimeText != null)
            {
                this.TimeText.set_textAlign(new Align.Right());
                this.TimeText.get_pixelScale = new HlFunc<double>(self.get_pixelScale);
                double iconWidth = (double)self.bossCellCount.widTile * self.bossCellCount.icon.scaleX;
                double textWidth = (int)((double)self.bossCellCount.text.get_textWidth() * self.bossCellCount.text.scaleX);
                double spacing = 3.0 * self.bossCellCount.get_pixelScale.Invoke();
                double bossCellWidth = iconWidth + textWidth + spacing;
                this.TimeText.maxWidthWanted = self.gameTime.maxWidthWanted;
                this.TimeText.onResize();
            }
        }

        private Flow NowTimeFlow = null!;
        public dc.ui.Text TimeText = null!;
        private void Hook_HUD___constructor__(Hook__HUD.orig___constructor__ orig, HUD arg1, dc.pr.Game game)
        {
            orig(arg1, game);
            this.NowTimeFlow = new Flow(null);
            this.NowTimeFlow.set_maxWidth(arg1.aboveMapFlow.maxWidth);
            this.NowTimeFlow.set_maxHeight(1);
            this.NowTimeFlow.set_verticalAlign(new FlowAlign.Top());
            this.NowTimeFlow.set_horizontalAlign(new FlowAlign.Middle());

            this.TimeText = new dc.ui.Text(this.NowTimeFlow, true, null, Ref<double>.Null, null, null);

            DateTime currentTime = DateTime.Now;
            this.TimeText.set_text($"{currentTime}".ToHaxeString());
            arg1.rightFlowR.addChild(this.NowTimeFlow);
        }



        private dc.String Hook_LifeBar_getStartEndName(dc.ui.hud.Hook_LifeBar.orig_getStartEndName orig, dc.ui.hud.LifeBar self)
        {
            switch (self.colorMode.RawIndex)
            {
                case 0:
                    dc.String iscolor = LifeBarStartEndColors[(int)config.LifeBarcolor].ToHaxeString();
                    return iscolor;
                case 1:
                    return "lifeBossStartEnd".ToHaxeString();
                case 2:
                    return "lifeBossModifiedStartEnd".ToHaxeString();
                default:
                    return null!;
            }
        }

        private dc.String Hook_LifeBar_GetFullName(dc.ui.hud.Hook_LifeBar.orig_getFullName orig, dc.ui.hud.LifeBar self)
        {

            switch (self.colorMode.RawIndex)
            {
                case 0:

                    dc.String iscolor = LifeBarFullColors[(int)config.LifeBarcolor].ToHaxeString();
                    return iscolor;
                case 1:
                    return "lifeFullBoss".ToHaxeString();
                case 2:
                    return "lifeFullBossModified".ToHaxeString();
                default:
                    return null!;
            }
        }

        private void Hook_NewsPanel_update(Hook_NewsPanel.orig_update orig, NewsPanel self)
        {
            if (config.NewsPanel) return;
            orig(self);
        }

        private void Hook_NewsPanel_focusIn(Hook_NewsPanel.orig_focusIn orig, NewsPanel self)
        {
            if (config.NewsPanel) return;

            orig(self);
        }

        private void Hook_NewsPanel_updateVisible(Hook_NewsPanel.orig_updateVisible orig, NewsPanel self)
        {
            if (config.NewsPanel) return;
            orig(self);
        }

        private void Hook_Entity_popDamage(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
        {
            if (dc.ui.Console.Class.ME.flags.exists("NoPopText".ToHaxeString())) return;
            orig(self, a);
        }
        #endregion


        public List<string> LifeBarFullColors = new()
        {
            "lifeFull",
            "gold_gradient_darker",
            "blue_gradient_darker",
            "pink_gradient_darker",
            "purple_gradient_darker",
            "red_gradient_darker",
            "white_gradient_darker"
        };

        public List<string> LifeBarStartEndColors = new()
        {
            "lifeStartEnd",
            "gold_gradient_transparent",
            "blue_gradient_transparent",
            "pink_gradient_transparent",
            "purple_gradient_transparent",
            "red_gradient_transparent",
            "white_gradient_transparent"
        };


        public void SetConsoleFlag(bool isSet, string flagName)
        {
            if (isSet)
                dc.ui.Console.Class.ME.flags.set(flagName.ToHaxeString(), null);

            else
                dc.ui.Console.Class.ME.flags.remove(flagName.ToHaxeString());
        }
    }
}