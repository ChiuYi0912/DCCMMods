using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Extensions;
using dc;
using dc.cine;
using dc.en;
using dc.h2d;
using dc.haxe.ds;
using dc.hxd;
using DebugMod.Debugxtensions;
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
        IOnAfterLoadingCDB,
        IOnGameExit
    {
        public static ModCore.Storage.Config<DebugMod.Configuration.CoreCfig> GetConfig = new("DebugMODCfig");
        public DebugGraphic GetGraphic = null!;
        public override void Initialize()
        {
            base.Initialize();
            _ = new DebugHUD();
            GetGraphic = new DebugGraphic();

            Info.Version = "1.3.1";
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

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {

        }

        void IOnGameExit.OnGameExit()
        {
            GetConfig.Value.IsQuadTreeDrawingEnabled = false;
            GetConfig.Save();
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



        public dc.h2d.Graphics debugentity = null!;
        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (Key.Class.isPressed(37))
            {
                LevelTransition.Class.@goto("BackGarden".AsHaxeString());
            }
            if (Key.Class.isPressed(38))
            {
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



    }




}

