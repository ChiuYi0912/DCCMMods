using ModCore.Events;
using ModCore.Menu;
using CoreLibrary.Core.Interfaces;
using ModCore.Modules;
using ModCore.Events.Interfaces.Game.Menu;
using dc.ui;
using static MoreSettings.Configuration.Enums;
using CoreLibrary.Utilities;
using dc;
using CoreLibrary.Core.Extensions;
using dc.h2d;
using dc.hl.types;
using Hashlink.Virtuals;
using ModCore.Utilities;
using Hashlink.Proxy.Clousre;
using System.Diagnostics;
using Hashlink.Proxy.Values;
using Hashlink.Reflection.Types;
using CoreLibrary.Extensions;

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
            AddCustomButtonToPause(GetText.Instance.GetString("Settings"), (e) =>
            {
                pause.root.set_visible(false);
                var opt = new dc.ui.Options(pause, null, null);
                MenuModule.Instance.SetSection(this);
                opt.killOnBack = true;

            }, pause, 2);
        }


        public static void AddCustomButtonToPause(string buttonText, Action<Pause> onClick, Pause pause, int index)
        {
            dc.ui.Text text = Assets.Class.makeText(GetText.Instance.GetString(buttonText).ToHaxeString(), null, true, null);

            text.set_textAlign(HashlinkEnumExtensions.GetSingleton<Align.Center>());

            var options = (ArrayObj)((dynamic)pause).options;
            var botMenu = (Flow)((dynamic)pause).botMenu;

            Interactive? templateInteractive = null;

            foreach (virtual_cb_inter_t_? v in options)
            {
                var inter = v?.inter;
                if (inter == null)
                {
                    continue;
                }

                if (inter.onClick != null && inter.onOver != null)
                {
                    var clickCL = (HashlinkClosure)((dynamic)inter.HashlinkObj).onClick;


                    //Find native closure
                    if (clickCL?.BindingThis == null)
                    {
                        continue;
                    }

                    templateInteractive = inter;
                    break;
                }
            }

            double width = text.get_textWidth();
            double height = text.get_textHeight();

            Interactive interactive = new(width, height, text, null)
            {
                onClick = _ => { },
                onOver = _ => { }
            };

            var entry = new virtual_cb_inter_t_
            {
                t = text,
                inter = interactive,
                cb = () => onClick(pause)
            };

            if (templateInteractive != null)
            {
                interactive.onClick = templateInteractive.onClick;

                static HashlinkClosure NewArrowClosure(HashlinkClosure cl, virtual_cb_inter_t_ entry)
                {
                    Debug.Assert(cl.BindingThis != null);

                    var arrowCtx = (HashlinkEnum)cl.BindingThis;

                    var newCtx = new HashlinkEnum((HashlinkEnumType)arrowCtx.Type, 0);
                    newCtx[0] = arrowCtx[0];
                    newCtx[1] = entry;
                    return new HashlinkClosure((HashlinkFuncType)cl.Type, cl.FunctionPtr, newCtx.HashlinkPointer);
                }

                //Unsafe
                interactive.onOver = (dynamic)NewArrowClosure((HashlinkClosure)((dynamic)templateInteractive.HashlinkObj).onOver, entry);
            }



            botMenu.addChildAt(text, index + 1);
            options.insertDyn(index, entry);

            pause.onResize();
        }

        public string GetName() => GetText.Instance.GetString("Mod name");
    }
}