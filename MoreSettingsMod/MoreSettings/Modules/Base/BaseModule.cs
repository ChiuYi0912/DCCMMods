using System;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using ModCore.Events;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Configuration;
using MoreSettings.Utilities;

namespace MoreSettings.Base.Modules
{

    public abstract class BaseModule : IEventReceiver
    {

        public abstract string Name { get; }
        public virtual string Description => string.Empty;
        public bool Enabled { get; private set; }
        protected ModBase MainMod { get; private set; } = null!;
        public virtual OptionsMenuHelper<MainConfig> menuHelper { get; set; } = default!;
        public virtual object config { get; set; } = null!;

        public virtual void Initialize(ModBase mainMod)
        {
            MainMod = mainMod;
            EventSystem.AddReceiver(this);
        }

        public virtual void BuildMenu(dc.ui.Options options)
        {
            options.createScroller(1);
            options.title.set_text(GetText.Instance.GetString("DCCM模组开关(B站ChiuYi.秋)").ToHaxeString());
            menuHelper = new(options, SettingsMain.ModConfig);
        }

        public virtual void RegisterHooks() { if (!Enabled) return; }
        
        public virtual void SaveConfig()
        {
            SettingsMain.ModConfig.Save();
            Logger.Information("配置已保存");
        }
    }
}