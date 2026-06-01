using System;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.h2d;
using dc.libs.heaps.slib;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Base.Modules
{

    public abstract class BaseModule : IEventReceiver
    {

        public virtual string Name => GetType().Name;
        public virtual string Description => string.Empty;
        public virtual bool Enabled { get; set; }
        public abstract MenuCategory Type { get; }
        protected ModBase MainMod { get; private set; } = null!;
        public virtual OptionsMenuHelper<MainConfig> menuHelper { get; set; } = default!;
        public virtual Flow scrollerFlow { get; set; } = default!;
        public virtual OptionWidget TopWidget { get; private set; } = null!;
        public virtual Graphics Graphics { get; private set; } = default!;
        public virtual dynamic config { get; set; } = null!;

        public virtual void Initialize(ModBase mainMod)
        {
            MainMod = mainMod;
            EventSystem.AddReceiver(this);
        }

        public void BaseBuildMenu(dc.ui.Options options)
        {
            Graphics = options.createScroller(1);
            options.title.set_text(GetText.Instance.GetString("ModTitle").ToHaxeString());
        }

        public virtual void BuildMenu(dc.ui.Options options, string Separator)
        {
            menuHelper = new(options, SettingsMain.ModConfig);
            scrollerFlow = options.scrollerFlow;

            // #if DEBUG
            // scrollerFlow.debugGraphics = new Graphics(scrollerFlow);
            // scrollerFlow.debug = true;
            // scrollerFlow.getProperties(scrollerFlow.debugGraphics).isAbsolute = true;
            // #endif

            options.addSeparator(GetText.Instance.GetString(Description).ToHaxeString(), scrollerFlow);
            Enabled = config.Enabled;

            TopWidget = menuHelper.AddConfigToggle(
                GetText.Instance.GetString("EnableModule"),
                "",
                () => Enabled,
                v =>
                {
                    config.Enabled = Enabled = v;
                    if (Enabled)
                        RegisterHooks();
                    else
                        UnregisterHooks();
                    SaveConfig();
                    //options.setSection(options.curSection);
                    options.onResize();
                },
                scrollerFlow
            );
            menuHelper.CenterToggleWidget(TopWidget, options, scrollerFlow);
        }

        public void BaseRegisterHooks() { if (!config.Enabled) return; RegisterHooks(); Logger.Information("registerHook"); }
        public virtual void RegisterHooks() { }
        public virtual void PermanentlyRegisterHooks() { }
        public virtual void UnregisterHooks() { }


        public string GetString(string str) => GetText.Instance.GetString(str);
        public dc.String GetDcString(string str) => GetString(str).ToHaxeString();
        public virtual void SaveConfig()
        {
            SettingsMain.ModConfig.Save();
            Logger.Information("配置已保存");
        }


    }
}