using System;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.h2d;
using dc.libs.heaps.slib;
using dc.ui;
using ModCore.Events;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;

namespace MoreSettings.Base.Modules
{

    public abstract class BaseModule : IEventReceiver
    {

        public virtual string Name => GetType().Name;
        public virtual string Description => string.Empty;
        public virtual bool Enabled { get; set; }
        protected ModBase MainMod { get; private set; } = null!;
        public virtual OptionsMenuHelper<MainConfig> menuHelper { get; set; } = default!;
        public virtual Flow scrollerFlow { get; private set; } = default!;
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
            options.title.set_text(GetText.Instance.GetString("DCCM模组开关(B站ChiuYi.秋)").ToHaxeString());
        }

        public virtual void BuildMenu(dc.ui.Options options, string Separator)
        {
            menuHelper = new(options, SettingsMain.ModConfig);
            scrollerFlow = options.scrollerFlow;

            options.addSeparator(GetText.Instance.GetString(Separator).ToHaxeString(), scrollerFlow);
            Enabled = config.Enabled;

            var widget = menuHelper.AddConfigToggle(
                "启用此设置模块",
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
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );
            var props = scrollerFlow.getProperties(widget);
            props.horizontalAlign = new FlowAlign.Middle();
            const int aligned = 6;
            double scale = options.get_pixelScale.Invoke();
            int dynamicOffset = (int)(scale * 15);

            foreach (var child in widget.children.AsEnumerable())
            {
                if (child is HSprite icon)
                {
                    icon.scaleX = icon.scaleY = icon.scaleY / 1.5;
                    icon.y = widget.get_innerHeight() / (aligned + 1);
                    icon.x += dynamicOffset * 1.5;
                }
                else if (child is Flow textFlow)
                {
                    textFlow.verticalAlign = new FlowAlign.Middle();
                    foreach (var sub in textFlow.children.AsEnumerable())
                    {
                        if (sub is dc.ui.Text txt)
                        {
                            txt.scaleX = txt.scaleY = 1;
                            txt.y = textFlow.get_innerHeight() / (aligned - 1);
                            txt.x += dynamicOffset;
                        }
                    }
                }
            }
        }

        public void BaseRegisterHooks() { if (!config.Enabled) return; RegisterHooks(); Logger.Information("registerHook"); }
        public virtual void RegisterHooks() { }
        public virtual void UnregisterHooks() { Logger.Information("UnregisterHooks"); }


        public virtual void SaveConfig()
        {
            SettingsMain.ModConfig.Save();
            Logger.Information("配置已保存");
        }
    }
}