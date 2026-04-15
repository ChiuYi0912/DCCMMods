using System;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.h2d;
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
        public bool Enabled { get; private set; } = true;
        protected ModBase MainMod { get; private set; } = null!;
        public virtual OptionsMenuHelper<MainConfig> menuHelper { get; set; } = default!;
        public virtual Flow scrollerFlow { get; private set; } = default!;
        public virtual object config { get; set; } = null!;

        public virtual void Initialize(ModBase mainMod)
        {
            MainMod = mainMod;
            EventSystem.AddReceiver(this);
        }

        public virtual void BuildMenu(dc.ui.Options options, string Separator)
        {
            menuHelper = new(options, SettingsMain.ModConfig);
            scrollerFlow = options.scrollerFlow;

            options.addSeparator(GetText.Instance.GetString(Separator).ToHaxeString(), scrollerFlow);

            // menuHelper.AddConfigToggle(
            //     "启用此设置模块",
            //     "",
            //     () => Enabled,
            //     v =>
            //     {
            //         bool oldValue = Enabled;
            //         Enabled = v;
            //         if (v && !oldValue)
            //         {
            //             RegisterHooks();
            //         }
            //         else if (!v && oldValue)
            //         {
            //             UnregisterHooks();
            //         }

            //         SaveConfig();
            //     },
            //     scrollerFlow
            // );


            // if (!Enabled)
            // {
            //     return;
            // }
        }

        public virtual void RegisterHooks() { if (!Enabled) return; }
        public virtual void UnregisterHooks() { }


        public virtual void SaveConfig()
        {
            SettingsMain.ModConfig.Save();
            Logger.Information("配置已保存");
        }
    }
}