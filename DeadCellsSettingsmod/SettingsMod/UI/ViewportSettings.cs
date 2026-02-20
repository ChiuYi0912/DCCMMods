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
    public class ViewportSettings
    {
        public ViewportSettings()
        {
            InitializeHooks();
        }
        public void InitializeHooks()
        {
            Hook_Viewport.bumpAng += Hook_Viewport_bumpang;
            Hook_Viewport.bumpDir += Hook_Viewport_bumpdir;
            Hook_Viewport.shakeS += Hook_Viewport_shakes;
            Hook_Viewport.shakeReversedS += Hook_Viewport_shakeReversedS;
        }

        public void AddViewportSettings(Options self)
        {
            var scrollerFlow = self.scrollerFlow;
            self.addSeparator(Lang.Class.t.get("更多视角设置".AsHaxeString(), null), scrollerFlow);
            dc.String string3 = Lang.Class.t.get("U35_PLAYER_CAMERA_SPEED".AsHaxeString(), null);
            double num = Main.Class.ME.options.playerCameraSpeed;
            scrollerFlow = self.scrollerFlow;

            HlAction<double> defaultValue = new HlAction<double>((double v) =>
            {
                Main.Class.ME.options.playerCameraSpeed = v;
                ChiuYiMain.config.Save();
            });
            double b = 0.1;
            bool showRawValue = false;
            bool showRawValue1 = true;
            double minValue = -100;
            double maxValue = 100;
            OptionWidget optionWidget = self.addSliderWidget(
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


            scrollerFlow = self.scrollerFlow;
            dc.String string4 = Lang.Class.t.get("屏幕震动调节".AsHaxeString(), null);
            self.addSeparator(string4, scrollerFlow);
            string3 = Lang.Class.t.get("角度震动强度".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportbumAng;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportbumAng = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = -100;
            maxValue = 100;
            optionWidget = self.addSliderWidget(
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


            string3 = Lang.Class.t.get("方向震动强度 ".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.Viewportbumdir;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.Viewportbumdir = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = -100;
            maxValue = 100;
            optionWidget = self.addSliderWidget(
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




            #region 持续震动起始强度
            scrollerFlow = self.scrollerFlow;
            string4 = Lang.Class.t.get("持续震动起始强度".AsHaxeString(), null);
            self.addSeparator(string4, scrollerFlow);
            string3 = Lang.Class.t.get("水平震动目标强度".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakesX;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakesX = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 1;
            optionWidget = self.addSliderWidget(
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




            string3 = Lang.Class.t.get("垂直震动目标强度".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakesY;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakesY = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 10;
            optionWidget = self.addSliderWidget(
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




            string3 = Lang.Class.t.get("震动持续时间".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakesD;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakesD = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 100;
            optionWidget = self.addSliderWidget(
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


            #endregion


            #region 持续震动衰减强度
            scrollerFlow = self.scrollerFlow;
            string4 = Lang.Class.t.get("持续震动衰减强度".AsHaxeString(), null);
            self.addSeparator(string4, scrollerFlow);
            string3 = Lang.Class.t.get("水平震动目标强度".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakeReversedSX;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakeReversedSX = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 1;
            optionWidget = self.addSliderWidget(
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

            string3 = Lang.Class.t.get("垂直震动目标强度".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakeReversedSY;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakeReversedSY = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 1;
            optionWidget = self.addSliderWidget(
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


            string3 = Lang.Class.t.get("衰减持续时间".AsHaxeString(), null);
            num = ChiuYiMain.config.Value.ViewportshakeReversedSD;
            scrollerFlow = self.scrollerFlow;

            defaultValue = new HlAction<double>((double v) =>
           {
               ChiuYiMain.config.Value.ViewportshakeReversedSD = v;
               ChiuYiMain.config.Save();
           });
            b = 0.1;
            showRawValue = false;
            showRawValue1 = true;
            minValue = 0;
            maxValue = 100;
            optionWidget = self.addSliderWidget(
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

            #endregion
        }

        private void Hook_Viewport_shakeReversedS(Hook_Viewport.orig_shakeReversedS orig, Viewport self, double yPow, double d, double xPow)
        {
            yPow = ChiuYiMain.config.Value.ViewportshakeReversedSX;
            d = ChiuYiMain.config.Value.ViewportshakeReversedSY;
            xPow = ChiuYiMain.config.Value.ViewportshakeReversedSD;
            orig(self, yPow, d, xPow);
        }

        private void Hook_Viewport_shakes(Hook_Viewport.orig_shakeS orig, Viewport self, double yPow, double d, double xPow)
        {
            yPow = ChiuYiMain.config.Value.ViewportshakesX;
            d = ChiuYiMain.config.Value.ViewportshakesY;
            xPow = ChiuYiMain.config.Value.ViewportshakesD;
            orig(self, yPow, d, xPow);
        }

        private void Hook_Viewport_bumpdir(Hook_Viewport.orig_bumpDir orig, Viewport self, int dir, double? pow)
        {
            pow = ChiuYiMain.config.Value.Viewportbumdir;
            orig(self, dir, pow);
        }

        private void Hook_Viewport_bumpang(Hook_Viewport.orig_bumpAng orig, Viewport self, double ang, double? pow)
        {
            pow = ChiuYiMain.config.Value.ViewportbumAng;
            orig(self, ang, pow);
        }
    }
}