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

namespace MoreSettings
{
    public class ModMenu :
    IEventReceiver,
    IModMenu,
    IOnHookInitialize
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

        private void Hook_DefaultPause_init(Hook__DefaultPause.orig___constructor__ orig, DefaultPause arg1)
        {
            orig(arg1);
            AddCustomButtonToPause("Settings", () =>
            {
                arg1.root.set_visible(false);
                var opt = new dc.ui.Options(arg1, null, null);
                MenuModule.Instance.SetSection(this);
                opt.killOnBack = true;

            }, arg1, 2);
        }

        public static void AddCustomButtonToPause(string buttonText, Action onClick, DefaultPause pause, int index, int cx = 30)
        {
            dc.ui.Text text = Assets.Class.makeText(GetText.Instance.GetString(buttonText).ToHaxeString(), null, true, pause.botMenu);

            text.set_textAlign(new Align.Center());

            double width = text.get_textWidth();
            double height = text.get_textHeight();

            dc.h2d.Interactive interactive = new(width, height, text, null);
            interactive.onClick = new HlAction<Event>((e) => { });
            interactive.onOver = new HlAction<Event>((e) => { });

            var entry = new virtual_cb_inter_t_();
            entry.t = text;
            entry.inter = interactive;
            entry.cb = new HlAction(onClick);

            pause.botMenu.addChildAt(text, index + 1);
            pause.options.insertDyn(index, entry);

            pause.onResize();
        }


        void IOnHookInitialize.HookInitialize()
        {
            Hook__DefaultPause.__constructor__ += Hook_DefaultPause_init;
        }

    }
}