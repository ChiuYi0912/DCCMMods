using dc;
using dc.cine;
using dc.en;
using dc.h2d;
using dc.haxe.ds;
using dc.hxd;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;

namespace DebugMod
{
    public class DebugModMod(ModInfo info) : ModBase(info),
        IOnBeforeGameInit,
        IOnHeroUpdate,
        IOnAfterLoadingCDB
    {
        public static ModCore.Storage.Config<DebugMod.Core.Configuration.CoreCfig> GetConfig =new("DebugMODCfig");
        public override void Initialize()
        {
            base.Initialize();
            _ = new DebugHUD();
        }

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


        private void Hook__Console__constructor__(dc.h2d.Hook__Console.orig___constructor__ orig, dc.h2d.Console nextLevelVote, Font font, dc.h2d.Object parent)
        {
            orig(nextLevelVote, font, parent);
            StringMap commands = nextLevelVote.commands;
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




        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            Hero owen = ModCore.Modules.Game.Instance.HeroInstance!;
            owen._level.fx.biomeCemetery(2, false);

            if (Key.Class.isPressed(37))
            {
                LevelTransition.Class.@goto("BackGarden".AsHaxeString());
            }
            if (Key.Class.isPressed(38))
            {
                Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
                hero._level.fx.customMask(12231073, 0.35, 0.04, 0.35, 2, null);

            }
            if (Key.Class.isPressed(39))
            {
                dc.cine.LevelTransition.Class.@goto("PrisonRoof".AsHaxeString());
            }
            if (Key.Class.isPressed(40))
            {
                Debugmethod();
            }
        }


        public void Debugmethod()
        {


        }

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {

        }


    }




}

