using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;

namespace MoreSettings.Modules
{
    public class ViewportSettings : BaseModule
    {
        public override string Description => "视角设置";
        public override ViewportConfig config => (ViewportConfig)base.config;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ConfigValue.Viewport;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Viewport.bumpAng += Hook_Viewport_bumpang;
            Hook_Viewport.bumpDir += Hook_Viewport_bumpdir;
            Hook_Viewport.shakeS += Hook_Viewport_shakes;
            Hook_Viewport.shakeReversedS += Hook_Viewport_shakeReversedS;
        }



        public override void UnregisterHooks()
        {
            Hook_Viewport.bumpAng -= Hook_Viewport_bumpang;
            Hook_Viewport.bumpDir -= Hook_Viewport_bumpdir;
            Hook_Viewport.shakeS -= Hook_Viewport_shakes;
            Hook_Viewport.shakeReversedS -= Hook_Viewport_shakeReversedS;
        }


        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);

            if (!config.Enabled)
                return;

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("U35_PLAYER_CAMERA_SPEED"),
                () => SettingsMain.ConfigValue.UI.PlayerCameraSpeed,
                v => SettingsMain.ConfigValue.UI.PlayerCameraSpeed = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("角度震动强度"),
                () => config.ViewportbumAng,
                v => config.ViewportbumAng = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("方向震动强度 "),
                () => config.Viewportbumdir,
                v => config.Viewportbumdir = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            options.addSeparator(GetText.Instance.GetString("持续震动起始强度").ToHaxeString(), scrollerFlow);

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("水平震动目标强度"),
                () => config.ViewportshakesX,
                v => config.ViewportshakesX = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("垂直震动目标强度"),
                () => config.ViewportshakesY,
                v => config.ViewportshakesY = v,
                step: 0.1,
                minValue: 0,
                maxValue: 10,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("震动持续时间"),
                () => config.ViewportshakesD,
                v => config.ViewportshakesD = v,
                step: 0.1,
                minValue: 0,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            options.addSeparator(GetText.Instance.GetString("持续震动衰减强度").ToHaxeString(), scrollerFlow);

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("水平震动目标强度"),
                () => config.ViewportshakeReversedSX,
                v => config.ViewportshakeReversedSX = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("垂直震动目标强度"),
                () => config.ViewportshakeReversedSY,
                v => config.ViewportshakeReversedSY = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("衰减持续时间"),
                () => config.ViewportshakeReversedSD,
                v => config.ViewportshakeReversedSD = v,
                step: 0.1,
                minValue: 0,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );
        }

        #region 钩子实现




        private void Hook_Viewport_shakeReversedS(Hook_Viewport.orig_shakeReversedS orig, Viewport self, double yPow, double d, double xPow)
        {
            yPow = config.ViewportshakeReversedSX;
            d = config.ViewportshakeReversedSY;
            xPow = config.ViewportshakeReversedSD;
            orig(self, yPow, d, xPow);
        }


        private void Hook_Viewport_shakes(Hook_Viewport.orig_shakeS orig, Viewport self, double yPow, double d, double xPow)
        {
            yPow = config.ViewportshakesX;
            d = config.ViewportshakesY;
            xPow = config.ViewportshakesD;
            orig(self, yPow, d, xPow);
        }

        private void Hook_Viewport_bumpdir(Hook_Viewport.orig_bumpDir orig, Viewport self, int dir, double? pow)
        {
            pow = config.Viewportbumdir;
            orig(self, dir, pow);
        }

        private void Hook_Viewport_bumpang(Hook_Viewport.orig_bumpAng orig, Viewport self, double ang, double? pow)
        {
            pow = config.ViewportbumAng;
            orig(self, ang, pow);
        }
        #endregion
    }
}