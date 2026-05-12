using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.hxd;
using dc.ui.pause;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Menu;
using CoreLibrary.Core.Interfaces;
using ModCore.Modules;
using ModCore.Events.Interfaces.Game.Menu;
using dc.ui;

namespace MoreSettings
{
    public class ModMenu :
    IEventReceiver,
    IModMenu,
    IOnHookInitialize,
    IOnAfterPauseMenuBuild
    {
        public SettingsMain SettingsMain = null!;
        public ModMenu(SettingsMain main)
        {
            SettingsMain = main;
            EventSystem.AddReceiver(this);
        }


        void IModMenu.BuildMenu(dc.ui.Options options)
        {
            SettingsMain.ModuleManager.BuildAllMenus(options);
        }

        string IModMenu.GetName() => SettingsMain.Info.Name;


        void IOnHookInitialize.HookInitialize()
        {
        }
        void IOnAfterPauseMenuBuild.OnAfterPauseMenuBuild(Pause pause)
        {
            MenuModule.Instance.AddCustomButtonToPause(GetText.Instance.GetString("Settings"), (e) =>
            {
                pause.root.set_visible(false);
                var opt = new dc.ui.Options(pause, null, null);
                MenuModule.Instance.SetSection(this);
                opt.killOnBack = true;

            }, pause, 2);
        }

        public string GetName() => GetText.Instance.GetString("Mod name");
    }
}