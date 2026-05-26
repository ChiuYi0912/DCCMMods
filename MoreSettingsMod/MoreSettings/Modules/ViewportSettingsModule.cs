using CoreLibrary.Core.Extensions;
using dc;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;

namespace MoreSettings.Modules
{
    public class ViewportSettingsModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Viewport");
        public override ViewportConfig config => (ViewportConfig)base.config;

        public override Enums.MenuCategory Type => Enums.MenuCategory.Viewport;


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

            var widget = menuHelper.AddConfigToggle(
                GetText.Instance.GetString("SmoothTeleport"),
                 GetText.Instance.GetString(""),
                () => config.TeleportImmediate,
                v => config.TeleportImmediate = v,
                scrollerFlow: options.scrollerFlow
                );
            menuHelper.CenterToggleWidget(widget, options, options.scrollerFlow);

            if (!config.Enabled)
                return;

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("PlayerCameraSpeed"),
                () => SettingsMain.ConfigValue.UI.PlayerCameraSpeed,
                v => SettingsMain.ConfigValue.UI.PlayerCameraSpeed = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("AngleShakeIntensity"),
                () => config.ViewportbumAng,
                v => config.ViewportbumAng = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("DirShakeIntensity"),
                () => config.Viewportbumdir,
                v => config.Viewportbumdir = v,
                step: 0.1,
                minValue: -100,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            options.addSeparator(GetText.Instance.GetString("SustainedShakeStart").ToHaxeString(), scrollerFlow);

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("HorizontalShakeTarget"),
                () => config.ViewportshakesX,
                v => config.ViewportshakesX = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("VerticalShakeTarget"),
                () => config.ViewportshakesY,
                v => config.ViewportshakesY = v,
                step: 0.1,
                minValue: 0,
                maxValue: 10,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("ShakeDuration"),
                () => config.ViewportshakesD,
                v => config.ViewportshakesD = v,
                step: 0.1,
                minValue: 0,
                maxValue: 100,
                scrollerFlow: scrollerFlow
            );

            options.addSeparator(GetText.Instance.GetString("SustainedShakeDecay").ToHaxeString(), scrollerFlow);

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("HorizontalShakeTarget"),
                () => config.ViewportshakeReversedSX,
                v => config.ViewportshakeReversedSX = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("VerticalShakeTarget"),
                () => config.ViewportshakeReversedSY,
                v => config.ViewportshakeReversedSY = v,
                step: 0.1,
                minValue: 0,
                maxValue: 1,
                scrollerFlow: scrollerFlow
            );

            menuHelper.AddConfigSlider(
                GetText.Instance.GetString("DecayDuration"),
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