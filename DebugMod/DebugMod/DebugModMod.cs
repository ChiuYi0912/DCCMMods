using System.Reflection;
using System.Runtime.CompilerServices;
using dc;
using dc.cine;
using dc.h2d;
using dc.haxe;
using dc.haxe.ds;
using dc.hl.types;
using dc.hxd;
using dc.hxsl;
using Hashlink.Marshaling;
using Hashlink.Proxy;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.DynamicAccess;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection;
using Hashlink.Reflection.Members;
using Hashlink.Reflection.Types;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utitities;

namespace DebugMod
{
    public class DebugModMod(ModInfo info) : ModBase(info),
        IOnBeforeGameInit,
        IOnHeroUpdate
    {
        private void Hook_Console_ctor(HashlinkClosure orig, HashlinkObject self)
        {
            orig.DynamicInvoke(self);
            var s = self.AsHaxe<dc.ui.Console>();
            var ss = dc.ui.Console.Class;
            ss.HIDE_UI = "FDMM_HIDE_UI".AsHaxeString();
            ss.HIDE_DEBUG = "FDMM_HIDE_DEBUG".AsHaxeString();
            ss.HIDE_CONSOLE = "FDMM_HIDE_CONSOLE".AsHaxeString();
            s.activateDebug();



        }
        void IOnBeforeGameInit.OnBeforeGameInit()
        {
            var hh = HashlinkHooks.Instance;

            dc.h2d.Hook_Console.handleCommand += Hook_Console_handleCommand1;
            dc.ui.Hook_Console.log += Hook_Console_log1;
            dc.h2d.Hook__Console.__constructor__ += Hook__Console__constructor__;

            hh.CreateHook("ui.$Console", "__constructor__", Hook_Console_ctor).Enable();
        }

        private void Hook__Console__constructor__(Hook__Console.orig___constructor__ orig, dc.h2d.Console arg1, Font font, dc.h2d.Object parent)
        {
            orig(arg1, font, parent);
            StringMap commands = arg1.commands;


        }

        private void Hook_Console_log1(dc.ui.Hook_Console.orig_log orig, dc.ui.Console self,
            dc.String text, int? color)
        {
            Logger.Information(text.ToString() ?? "");
            var ct = (HashlinkObjectType)self.HashlinkObj.Type;
            ct.Super!.FindProto("log")!.Function.DynamicInvoke(self, text, color);
        }

        private void Hook_Console_handleCommand1(dc.h2d.Hook_Console.orig_handleCommand orig,
            dc.h2d.Console self, dc.String command)
        {
            Logger.Information("Handle Command: {cmd}", command);
            orig(self, command);
        }

        public void debug()
        {
            Logger.Debug("成功！！！！");
        }


        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (Key.Class.isPressed(37))
            {
                dc.cine.LevelTransition.Class.@goto("PrisonCourtyard".AsHaxeString());
            }
            if (Key.Class.isPressed(38))
            {
                dc.cine.LevelTransition.Class.@goto("SewerShort".AsHaxeString());
            }
            if (Key.Class.isPressed(39))
            {
                dc.cine.LevelTransition.Class.@goto("PrisonRoof".AsHaxeString());
            }
            if (Key.Class.isPressed(40))
            {
                dc.cine.LevelTransition.Class.@goto("IllegalLevel".AsHaxeString());
            }
        }



    }

}

