using ModCore.Events;
using ModCore.Menu;
using CoreLibrary.Core.Interfaces;
using ModCore.Modules;
using ModCore.Events.Interfaces.Game.Menu;
using dc.ui;
using static MoreSettings.Configuration.Enums;
using CoreLibrary.Utilities;

namespace MoreSettings
{
    internal class ModMenu :
    IEventReceiver,
    IModMenu,
    IOnHookInitialize,
    IOnAfterPauseMenuBuild
    {
        public SettingsMain SettingsMain = null!;
        public MenuCategory menu = MenuCategory.All;
        public ModMenu(SettingsMain main)
        {
            SettingsMain = main;
            EventSystem.AddReceiver(this);
        }

        void IModMenu.BuildMenu(dc.ui.Options options)
        {
            switch (menu)
            {
                case MenuCategory.All:
                    SettingsMain.ModuleManager.BuildAllMenus(options);
                    return;
                default:
                    SettingsMain.ModuleManager.BuildCustomMenus(options, menu);
                    return;
            }
        }

        string IModMenu.GetName() => SettingsMain.Info.Name;


        void IOnHookInitialize.HookInitialize()
        {
            Hook_OptionsBase.onQuit += Hook_OptionsBase_onQuit;
        }

        private void Hook_OptionsBase_onQuit(Hook_OptionsBase.orig_onQuit orig, OptionsBase self)
        {
            if (menu != MenuCategory.All)
            {
                AudioHelper.LoadAudioFormString("sfx/ui/menu_back.wav");
                menu = MenuCategory.All;
                MenuModule.Instance.SetSection(this);
                return;
            }
            orig(self);
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