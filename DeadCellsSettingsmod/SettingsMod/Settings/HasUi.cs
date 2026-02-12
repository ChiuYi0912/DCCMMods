using System.Security.Cryptography.X509Certificates;
using Amazon.Runtime.Internal.Util;
using dc;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;

namespace ChiuYiUI.Settings.HasUi
{
    public class HasUiSetting
    {
        public HasUiSetting()
        {
            Initialize();
        }
        public void Initialize()
        {
            Hook__GameCinematic.__constructor__ += Hook__GameCinematic___constructor__;
            Hook_Game.init += Hook_Game_init;
           
        }


        private void Hook_Game_init(Hook_Game.orig_init orig, dc.pr.Game self)
        {
            orig(self);
            bool Vignette = CHIUYIMain.config.Value.HasBottomBar;
            HasFlags(Vignette, "noVignette");
            bool haslightTip = CHIUYIMain.config.Value.HaslightTip;
            HasFlags(haslightTip, "lightTip");
            bool hasNoPopText = CHIUYIMain.config.Value.HasNoPopText;
            HasFlags(hasNoPopText, "NoPopText");
        }

        private void Hook__GameCinematic___constructor__(Hook__GameCinematic.orig___constructor__ orig, GameCinematic arg1)
        {

            orig(arg1);
            bool hasbottombar = !CHIUYIMain.config.Value.HasBottomBar;
            arg1.bottomBar.set_visible(hasbottombar);
            arg1.topBar.set_visible(hasbottombar);

        }

        public void Markets(dc.ui.Options options)
        {

            var scrollerFlow = options.scrollerFlow;
            options.addSeparator(GetText.Instance.GetString("更多UI设置").AsHaxeString(), scrollerFlow);

            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> Market = static () =>
                    {
                        bool newValue = !CHIUYIMain.config.Value.HasBottomBar;
                        CHIUYIMain.config.Value.HasBottomBar = newValue;
                        CHIUYIMain.config.Save();
                        return newValue;
                    };
            bool hasMarket = CHIUYIMain.config.Value.HasBottomBar;
            options.addToggleWidget(
                GetText.Instance.GetString("移除电影黑边").AsHaxeString(),
                GetText.Instance.GetString("移除过场动画电影黑边").AsHaxeString(),
                Market,
                new Ref<bool>(ref hasMarket),
                scrollerFlow
            );


            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> noVignette = () =>
                    {
                        bool newValue = !CHIUYIMain.config.Value.NoVignette;
                        CHIUYIMain.config.Value.NoVignette = newValue;
                        CHIUYIMain.config.Save();
                        this.HasFlags(newValue, "noVignette");
                        return newValue;
                    };
            bool hasnoVignette = CHIUYIMain.config.Value.NoVignette;
            options.addToggleWidget(
                GetText.Instance.GetString("禁用暗角效果").AsHaxeString(),
                GetText.Instance.GetString("例如：受伤时的暗角效果").AsHaxeString(),
                noVignette,
                new Ref<bool>(ref hasnoVignette),
                scrollerFlow
            );

            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> lightTip = () =>
                    {
                        bool newValue = !CHIUYIMain.config.Value.HaslightTip;
                        CHIUYIMain.config.Value.HaslightTip = newValue;
                        CHIUYIMain.config.Save();
                        this.HasFlags(newValue, "lightTip");
                        return newValue;
                    };
            bool haslightTip = CHIUYIMain.config.Value.HaslightTip;
            options.addToggleWidget(
                GetText.Instance.GetString("轻量显示").AsHaxeString(),
                GetText.Instance.GetString("禁用交互图标等等").AsHaxeString(),
                lightTip,
                new Ref<bool>(ref haslightTip),
                scrollerFlow
            );


            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> NoPopText = () =>
                    {
                        bool newValue = !CHIUYIMain.config.Value.HasNoPopText;
                        CHIUYIMain.config.Value.HasNoPopText = newValue;
                        CHIUYIMain.config.Save();
                        this.HasFlags(newValue, "NoPopText");
                        return newValue;
                    };
            bool hasNoPopText = CHIUYIMain.config.Value.HasNoPopText;
            options.addToggleWidget(
                GetText.Instance.GetString("无弹出文字").AsHaxeString(),
                GetText.Instance.GetString("禁用伤害弹出文字").AsHaxeString(),
                NoPopText,
                new Ref<bool>(ref hasNoPopText),
                scrollerFlow
            );


            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> SpeedTier = static () =>
            {
                bool newValue = !CHIUYIMain.config.Value.SpeedTier;
                CHIUYIMain.config.Value.SpeedTier = newValue;
                CHIUYIMain.config.Save();
                return newValue;
            };
            bool hasSpeedTier = CHIUYIMain.config.Value.SpeedTier;
            options.addToggleWidget(
                GetText.Instance.GetString("开启竞速卷轴拾取").AsHaxeString(),
                GetText.Instance.GetString("竞速模式快速拾取卷轴常驻").AsHaxeString(),
                SpeedTier,
                new Ref<bool>(ref hasSpeedTier),
                scrollerFlow
            );
        }

        public void HasFlags(bool isset, string name)
        {
            if (isset)
            {
                dc.ui.Console.Class.ME.flags.set(name.AsHaxeString(), null);
            }
            else
            {
                dc.ui.Console.Class.ME.flags.remove(name.AsHaxeString());
            }
        }

    }
}