using System.Security.Cryptography.X509Certificates;
using dc;
using ChiuYiUI.Core;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;

namespace ChiuYiUI.UI
{
    public class UISettings
    {
        public UISettings()
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
            bool Vignette = ChiuYiMain.config.Value.HasBottomBar;
            SetConsoleFlag(Vignette, "noVignette");
            bool haslightTip = ChiuYiMain.config.Value.HaslightTip;
            SetConsoleFlag(haslightTip, "lightTip");
            bool hasNoPopText = ChiuYiMain.config.Value.HasNoPopText;
            SetConsoleFlag(hasNoPopText, "NoPopText");
        }

        private void Hook__GameCinematic___constructor__(Hook__GameCinematic.orig___constructor__ orig, GameCinematic arg1)
        {

            orig(arg1);
            bool hasbottombar = !ChiuYiMain.config.Value.HasBottomBar;
            arg1.bottomBar.set_visible(hasbottombar);
            arg1.topBar.set_visible(hasbottombar);

        }

        public void AddUISettings(dc.ui.Options options)
        {

            var scrollerFlow = options.scrollerFlow;
            options.addSeparator(GetText.Instance.GetString("更多UI设置").AsHaxeString(), scrollerFlow);

            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> Market = static () =>
                    {
                        bool newValue = !ChiuYiMain.config.Value.HasBottomBar;
                        ChiuYiMain.config.Value.HasBottomBar = newValue;
                        ChiuYiMain.config.Save();
                        return newValue;
                    };
            bool hasMarket = ChiuYiMain.config.Value.HasBottomBar;
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
                        bool newValue = !ChiuYiMain.config.Value.NoVignette;
                        ChiuYiMain.config.Value.NoVignette = newValue;
                        ChiuYiMain.config.Save();
                        SetConsoleFlag(newValue, "noVignette");
                        return newValue;
                    };
            bool hasnoVignette = ChiuYiMain.config.Value.NoVignette;
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
                        bool newValue = !ChiuYiMain.config.Value.HaslightTip;
                        ChiuYiMain.config.Value.HaslightTip = newValue;
                        ChiuYiMain.config.Save();
                        SetConsoleFlag(newValue, "lightTip");
                        return newValue;
                    };
            bool haslightTip = ChiuYiMain.config.Value.HaslightTip;
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
                        bool newValue = !ChiuYiMain.config.Value.HasNoPopText;
                        ChiuYiMain.config.Value.HasNoPopText = newValue;
                        ChiuYiMain.config.Save();
                        SetConsoleFlag(newValue, "NoPopText");
                        return newValue;
                    };
            bool hasNoPopText = ChiuYiMain.config.Value.HasNoPopText;
            options.addToggleWidget(
                GetText.Instance.GetString("无弹出文字").AsHaxeString(),
                GetText.Instance.GetString("禁用伤害弹出文字").AsHaxeString(),
                NoPopText,
                new Ref<bool>(ref hasNoPopText),
                scrollerFlow
            );


            scrollerFlow = options.scrollerFlow;
            HlFunc<bool> NowTime = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.NowTimeVisible;
                ChiuYiMain.config.Value.NowTimeVisible = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool hasNowTime = ChiuYiMain.config.Value.NowTimeVisible;
            options.addToggleWidget(
                GetText.Instance.GetString("tiem").AsHaxeString(),
                GetText.Instance.GetString("").AsHaxeString(),
                NowTime,
                new Ref<bool>(ref hasNowTime),
                scrollerFlow
            );
        }

        public void SetConsoleFlag(bool isSet, string flagName)
        {
            if (isSet)
            {
                dc.ui.Console.Class.ME.flags.set(flagName.AsHaxeString(), null);
            }
            else
            {
                dc.ui.Console.Class.ME.flags.remove(flagName.AsHaxeString());
            }
        }

    }
}