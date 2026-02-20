using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiuYiUI.Core;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.pr;
using dc.tool;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;
using Serilog.Core;

namespace ChiuYiUI.UI
{
    public class PlayerSettings
    {
        public PlayerSettings()
        {
            Initialize();

        }

        public void Initialize()
        {
            dc.ui.hud.Hook_LifeBar.getFullName += Hook_LifeBar_GetFullName;
            dc.ui.hud.Hook_LifeBar.getStartEndName += Hook_LifeBar_getStartEndName;
            Hook__HUD.__constructor__ += Hook_HUD___constructor__;
            Hook_HUD.onResize += Hook_HUD_onResize;
            Hook_HUD.postUpdate += Hook_HUD_PostUpdate;
        }

        private DateTime _lastUpdateTime = DateTime.MinValue;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        private void Hook_HUD_PostUpdate(Hook_HUD.orig_postUpdate orig, HUD self)
        {
            orig(self);
            if (this.NowTimeFlow.visible!=Core.ChiuYiMain.config.Value.NowTimeVisible)
            {
                this.NowTimeFlow.set_visible(Core.ChiuYiMain.config.Value.NowTimeVisible);
            }
            
            if (!this.NowTimeFlow.visible)
                return;

            DateTime now = DateTime.Now;
            if (now - _lastUpdateTime >= _updateInterval)
            {
                this.TimeText.set_text($"{now}".AsHaxeString());

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
        private dc.ui.Text TimeText = null!;
        private void Hook_HUD___constructor__(Hook__HUD.orig___constructor__ orig, HUD arg1, Game game)
        {
            orig(arg1, game);
            this.NowTimeFlow = new Flow(null);
            this.NowTimeFlow.set_maxWidth(arg1.aboveMapFlow.maxWidth);
            this.NowTimeFlow.set_maxHeight(arg1.aboveMapFlow.maxHeight);
            this.NowTimeFlow.set_verticalAlign(new FlowAlign.Top());
            this.NowTimeFlow.set_horizontalAlign(new FlowAlign.Left());
            this.TimeText = new dc.ui.Text(this.NowTimeFlow, true, null, Ref<double>.Null, null, null);
            DateTime currentTime = DateTime.Now;
            this.TimeText.set_text($"{currentTime}".AsHaxeString());
            arg1.rightFlowL.addChild(this.NowTimeFlow);
        }
        public double get_pixelScale()
        {
            return Main.Class.ME.pixelScale * HUD.Class.ME.get_hudSize();
        }


        private dc.String Hook_LifeBar_getStartEndName(dc.ui.hud.Hook_LifeBar.orig_getStartEndName orig, dc.ui.hud.LifeBar self)
        {
            switch (self.colorMode.RawIndex)
            {
                case 0:
                    dc.String iscolor = LifeBarStartEndColors[(int)ChiuYiMain.config.Value.LifeBarcolor].AsHaxeString();
                    return iscolor;
                case 1:
                    return "lifeBossStartEnd".AsHaxeString();
                case 2:
                    return "lifeBossModifiedStartEnd".AsHaxeString();
                default:
                    return null!;
            }
        }

        private dc.String Hook_LifeBar_GetFullName(dc.ui.hud.Hook_LifeBar.orig_getFullName orig, dc.ui.hud.LifeBar self)
        {

            switch (self.colorMode.RawIndex)
            {
                case 0:

                    dc.String iscolor = LifeBarFullColors[(int)ChiuYiMain.config.Value.LifeBarcolor].AsHaxeString();
                    return iscolor;
                case 1:
                    return "lifeFullBoss".AsHaxeString();
                case 2:
                    return "lifeFullBossModified".AsHaxeString();
                default:
                    return null!;
            }
        }



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

        public void AddLifeBarColorSetting(dc.ui.Options self)
        {

            var scrollerFlow = self.scrollerFlow;
            var string4 = Lang.Class.t.get("血条颜色".AsHaxeString(), null);
            self.addSeparator(string4, scrollerFlow);
            var string3 = Lang.Class.t.get("血条颜色描述".AsHaxeString(), null);
            double num = ChiuYiMain.config.Value.LifeBarcolor;
            scrollerFlow = self.scrollerFlow;

            var defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.LifeBarcolor = v;
               ChiuYiMain.config.Save();
           });
            double b = 1;
            bool showRawValue = false;
            bool showRawValue1 = true;
            double minValue = 0;
            double maxValue = 6;
            var setting = self.addSliderWidget(
              string3,
              defaultValue,
              num,
              new Ref<double>(ref b),
              scrollerFlow,
              new Ref<bool>(ref showRawValue),
              new Ref<bool>(ref showRawValue1),
              new Ref<double>(ref minValue),
              new Ref<double>(ref maxValue),
              null,
              new Ref<int>());
        }
    }
}