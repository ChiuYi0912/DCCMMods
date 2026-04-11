using dc.ui;
using ModCore.Events;
using ModCore.Menu;
using MoreSettings.Utilities;

namespace MoreSettings
{
    public class ModMenu :
    IEventReceiver,
    IModMenu
    {
        public SettingsMain SettingsMain = null!;
        public ModMenu(SettingsMain main)
        {
            SettingsMain = main;
            EventSystem.AddReceiver(this);
        }

        void IModMenu.BuildMenu(Options options)
        {
            SettingsMain.ModuleManager.BuildAllMenus(options);
        }

      string IModMenu.GetName() => SettingsMain.Info.Name;
   }
}