using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.pr;
using dc.ui;
using dc.ui.hud;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Mods;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics;
using MoreSettings.shaders;

namespace MoreSettings.Modules
{
    internal class HasUiSettingsModule : BaseModule
    {
        public override string Description => GetString("ModuleDesc_UI");

        private dc.ui.Text TimeText = null!;
        public override UIConfig config => (UIConfig)base.config;
        public override Enums.MenuCategory Type => Enums.MenuCategory.UI;


        private DateTime _lastUpdateTime = DateTime.MinValue;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        public CustomHeroLifeBar customHeroLife = null!;
        private dc.ui.hud.LifeBar cachedSbooslifebar = default!;

        private double pixelScale;

        public double getpixelScale
        {
            get
            {
                if (pixelScale != 0) return pixelScale;
                var hud = dc.ui.HUD.Class.ME;
                if (hud != null)
                    pixelScale = hud.get_pixelScale.Invoke();

                return pixelScale;
            }
            set => pixelScale = value;
        }

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


            Hook__HUD.__constructor__ += Hook_HUD___constructor__;
            Hook_HUD.onResize += Hook_HUD_onResize;
            Hook_HUD.postUpdate += Hook_HUD_PostUpdate;


            Hook_NewsPanel.updateVisible += Hook_NewsPanel_updateVisible;
            Hook_NewsPanel.focusIn += Hook_NewsPanel_focusIn;
            Hook_NewsPanel.update += Hook_NewsPanel_update;

            Hook_TitleScreen.update += Hook_TitleScreen_update;
            Hook_TitleScreen.addMenu += Hook_TitleScreen_addMenu;

            Hook_HUD.initLeftFlowT += Hook_HUD_initLeftFlowT;
            Hook_HUD.hide += Hook_HUD_hide;
            Hook_HUD.show += Hook_HUD_show;
        }

        public override void UnregisterHooks()
        {
            dc.pr.Hook_Game.init -= Hook_Game_init;
            Hook__GameCinematic.__constructor__ -= Hook__GameCinematic___constructor__;

            Hook__HUD.__constructor__ -= Hook_HUD___constructor__;
            Hook_HUD.onResize -= Hook_HUD_onResize;
            Hook_HUD.postUpdate -= Hook_HUD_PostUpdate;


            Hook_NewsPanel.updateVisible -= Hook_NewsPanel_updateVisible;
            Hook_NewsPanel.focusIn -= Hook_NewsPanel_focusIn;
            Hook_NewsPanel.update -= Hook_NewsPanel_update;

            Hook_TitleScreen.update -= Hook_TitleScreen_update;
            Hook_TitleScreen.addMenu -= Hook_TitleScreen_addMenu;

            Hook_HUD.initLeftFlowT -= Hook_HUD_initLeftFlowT;
            Hook_HUD.hide -= Hook_HUD_hide;
            Hook_HUD.show -= Hook_HUD_show;
        }

        public override void PermanentlyRegisterHooks()
        {
            dc.ui.hud.Hook_LifeBar.updateSize += Hook_LifeBar_updateSize;
        }



        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            if (!config.Enabled)
                return;

            menuHelper.AddConfigToggle(
                GetString("RemoveNewsPanel"),
                GetString("TakeEffectNextLaunch"),
                () => config.NewsPanel,
                v => config.NewsPanel = v,
                scrollerFlow);

            menuHelper.AddConfigToggle(
                GetString("RemoveUpdateNotes"),
                GetString(""),
                () => config.RemovalUpdateNotes,
                v => config.RemovalUpdateNotes = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("RemoveCinematicBars"),
                GetString("RemoveCinematicBarsDesc"),
                () => config.HasBottomBar,
                v => config.HasBottomBar = v,
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                GetString("DisableVignette"),
                GetString("DisableVignetteDesc"),
                () => config.NoVignette,
                v =>
                {
                    SetConsoleFlag(v, "noVignette");
                    config.NoVignette = v;
                },
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("LightweightDisplay"),
                GetString("LightweightDisplayDesc"),
                () => config.HaslightTip,
                v =>
                {
                    SetConsoleFlag(v, "lightTip");
                    config.HaslightTip = v;
                },
                scrollerFlow
            );


            menuHelper.AddConfigToggle(
                GetString("ClockDisplay"),
                GetString(""),
                () => config.NowTimeVisible,
                v => config.NowTimeVisible = v,
                scrollerFlow
            );

            menuHelper.AddConfigToggle(
                GetString("BossHealthBarText"),
                GetString(""),
                () => config.ShowBossHealthBar,
                v => config.ShowBossHealthBar = v,
                scrollerFlow
            );


            menuHelper.AddHSVColorWidget(
                GetString("HealthBarColor"),
                "",
                () =>
                {
                    config.isLifeBarcolor = !config.isLifeBarcolor;
                    SettingsMain.SaveConfig();
                    return config.isLifeBarcolor;
                },
                config.isLifeBarcolor,
                newColor =>
                {
                    config.LifeBarcolor = newColor;
                    var hud = HUD.Class.ME;
                    if (hud != null)
                    {
                        float r = ((newColor >> 16) & 0xFF) / 255.0f;
                        float g = ((newColor >> 8) & 0xFF) / 255.0f;
                        float b = (newColor & 0xFF) / 255.0f;
                        float a = ((newColor >> 24) & 0xFF) / 255.0f;
                        GradientColor gradient = (GradientColor)customHeroLife.fullBatch.getShader(GradientColor.Class);
                        if (gradient == null) return;
                        gradient.tint__ = new dc.h3d.Vector(Ref<double>.In(r), Ref<double>.In(g), Ref<double>.In(b), Ref<double>.In(1));
                        SettingsMain.SaveConfig();
                    }
                },
                config.LifeBarcolor,
                scrollerFlow
            );
            if (config.isLifeBarcolor)
            {
                int paddingleft = (int)(options.get_pixelScale.Invoke() * 40);
                var alpha = menuHelper.AddConfigSlider(
                 GetString("Alpha"),
                 () => config.LifeBarAlpha,
                 (v) =>
                 {
                     config.LifeBarAlpha = v;
                     var hud = HUD.Class.ME;
                     if (hud != null)
                     {
                         GradientColor gradient = (GradientColor)customHeroLife.fullBatch.getShader(GradientColor.Class);
                         if (gradient != null)
                         {
                             gradient.alpha__ = v;
                             return;
                         }
                     }
                 },
                 scrollerFlow: scrollerFlow,
                 maxValue: 1,
                 step: 0.01,
                 paddingLeft: paddingleft
                );

            }
        }

        #region Hooks
        private virtual_cb_help_inter_isEnable_t_<bool> Hook_TitleScreen_addMenu(Hook_TitleScreen.orig_addMenu orig, TitleScreen self, dc.String str, HlAction cb, dc.String help, bool? isEnable, Ref<int> color)
        {
            if (config.RemovalUpdateNotes)
            {
                string updateNotesText = Lang.Class.t.get("Notes de mise à jour".ToHaxeString(), null).ToString();
                if (str.ToString().EqualsIgnoreCase(updateNotesText))
                    return null!;
            }

            return orig(self, str, cb, help, isEnable, color);
        }


        private void Hook_TitleScreen_update(Hook_TitleScreen.orig_update orig, TitleScreen self)
        {
            if (config.NewsPanel)
                self.newsSelected = false;
            orig(self);
            if (config.NewsPanel)
                self.newsSelected = false;

        }

        private void Hook_Game_init(Hook_Game.orig_init orig, dc.pr.Game self)
        {
            orig(self);
            SetConsoleFlag(config.HasBottomBar, "noVignette");
            SetConsoleFlag(config.HaslightTip, "lightTip");
            SetConsoleFlag(SettingsMain.ConfigValue.Skin.HasNoPopText, "NoPopText");
        }

        private void Hook__GameCinematic___constructor__(Hook__GameCinematic.orig___constructor__ orig, GameCinematic arg1)
        {
            orig(arg1);
            arg1.bottomBar.set_visible(!config.HasBottomBar);
            arg1.topBar.set_visible(!config.HasBottomBar);
        }


        private void Hook_HUD_PostUpdate(Hook_HUD.orig_postUpdate orig, HUD self)
        {
            orig(self);
            UpdateNowTime();
            if (cachedSbooslifebar == null) return;
            AdjustBossLifebarLabel(cachedSbooslifebar);
        }

        private void UpdateNowTime()
        {
            if (TimeText == null) return;
            if (TimeText.visible != config.NowTimeVisible)
                TimeText.set_visible(config.NowTimeVisible);

            var now = DateTime.Now;
            if (now - _lastUpdateTime >= _updateInterval)
            {
                TimeText!.set_text($"{now}".ToHaxeString());

                _lastUpdateTime = now;
            }

        }
        private void AdjustBossLifebarLabel(dc.ui.hud.LifeBar lifeBar)
        {
            if (lifeBar.label == null) return;

            var label = lifeBar.label;

            label.scaleX = label.scaleY /= 1.5;
            double finalScale = label.scaleX;

            double padding = 3.0 * getpixelScale;
            double sbX = lifeBar.sb.x;
            double sbY = lifeBar.sb.y;

            label.x = (lifeBar.curState.outerWid - 2 * padding - label.textWidth * finalScale) * 0.5 + sbX;
            label.y = (lifeBar.curState.outerHei - 2 * padding - label.textHeight * finalScale) * 0.5 + sbY;
        }


        private void Hook_HUD_onResize(Hook_HUD.orig_onResize orig, HUD self)
        {
            orig(self);

            if (TimeText != null)
            {
                TimeText.set_textAlign(new Align.Right());
                TimeText.get_pixelScale = new HlFunc<double>(self.get_pixelScale);
                TimeText.maxWidthWanted = self.curSeed.maxWidthWanted;
                TimeText.onResize();
            }
        }



        private void Hook_HUD___constructor__(Hook__HUD.orig___constructor__ orig, HUD arg1, dc.pr.Game game)
        {
            orig(arg1, game);
            getpixelScale = arg1.get_pixelScale.Invoke();

            TimeText = new dc.ui.Text(null, null, null, Ref<double>.Null, null, null);
            TimeText.scaleY = TimeText.scaleX = 1;
            TimeText.set_textAlign(new Align.Right());
            TimeText.set_text($"{DateTime.Now}".ToHaxeString());
            arg1.rightFlowR.addChildAt(TimeText, 3);

            if (config.ShowBossHealthBar && arg1.bossLifebar != null)
            {
                cachedSbooslifebar = arg1.bossLifebar;
                cachedSbooslifebar.enableText();
                AdjustBossLifebarLabel(cachedSbooslifebar);
            }

            arg1.leftFlowT.set_isVertical(true);
            arg1.leftFlow.reflow();

            for (int i = 0; i < arg1.leftFlow.children.length; i++)
            {
                var obj = arg1.leftFlowT.children.getDyn(i);
                if (obj != null)
                {
                    FlowProperties propo = arg1.leftFlowT.getProperties(obj);
                }
            }

            arg1.onResize();
        }


        public void enableDrag(Flow flow, Interactive inter)
        {
            if (flow == null) return;
            inter.propagateEvents = true;
            var dragging = false;
            var startX = 0.0; var startY = 0.0;
            var origX = 0.0; var origY = 0.0;

            inter.onPush = (e) =>
            {
                dragging = true;
                startX = e.relX;
                startY = e.relY;
                origX = flow.x;
                origY = flow.y;
                // flow.parent.addChild(flow);
            };

            inter.onMove = (e) =>
            {
                inter.cursor = new dc.hxd.Cursor.Button();
                if (dragging)
                {
                    flow.x = origX + (e.relX - startX);
                    flow.y = origY + (e.relY - startY);
                    flow.posChanged = true;
                }
            };

            inter.onRelease = (e) =>
            {
                dragging = false;
            };

            inter.onOut = (e) =>
            {
                if (dragging) dragging = false;
            };
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



        private void Hook_HUD_initLeftFlowT(Hook_HUD.orig_initLeftFlowT orig, HUD self)
        {
            orig(self);
            self.heroLifeBar.remove();
            customHeroLife = (CustomHeroLifeBar)(self.heroLifeBar = new CustomHeroLifeBar(new LifeBarColorMode.Normal(), self.leftFlowB));
            self.heroLifeBar.get_pixelScale = self.get_pixelScale;
            self.heroLifeBar.enableText();
        }


        private void Hook_LifeBar_updateSize(dc.ui.hud.Hook_LifeBar.orig_updateSize orig, dc.ui.hud.LifeBar self)
        {
            orig(self);
            if (customHeroLife != null)
                customHeroLife.updatesbSize();
        }

        private void Hook_HUD_show(Hook_HUD.orig_show orig, HUD self, bool? instant)
        {
            orig(self, instant);
            self.heroLifeBar.visible = true;
        }

        private void Hook_HUD_hide(Hook_HUD.orig_hide orig, HUD self, bool? instant)
        {
            orig(self, instant);
            self.heroLifeBar.visible = false;
        }

        #endregion
        #region Helper



        public static void SetConsoleFlag(bool isSet, string flagName)
        {
            if (isSet)
                dc.ui.Console.Class.ME.flags.set(flagName.ToHaxeString(), null);

            else
                dc.ui.Console.Class.ME.flags.remove(flagName.ToHaxeString());
        }
        #endregion
    }
}