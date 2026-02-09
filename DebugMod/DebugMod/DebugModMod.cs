using ChiuYiUI.GameCm;
using dc;
using dc._Data;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.en.inter.npc;
using dc.h2d;
using dc.haxe.ds;
using dc.hl.types;
using dc.hxd;
using dc.level;
using dc.libs;
using dc.tool;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using IngameDebugConsole;
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

            Hook__LootGen.__constructor__ += Hook__LootGen__constructor__;
        }

        public static double isseed;
        private void Hook__LootGen__constructor__(Hook__LootGen.orig___constructor__ orig, LootGen arg1, User user, ArrayObj maps, int seed, TierDistribution tierDistrib, Hero hero, Ref<bool> _hasExplorationBonusIncentive, Ref<bool> forceNoGenerate)
        {
            orig(arg1, user, maps, seed, tierDistrib, hero, _hasExplorationBonusIncentive, forceNoGenerate);
            Logger.Debug($"原始随机数：{arg1.rseed.seed}");
            Logger.Debug($"种子：{seed}");
            isseed = arg1.rseed.seed;
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



        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
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
            for (int i = 0; i < cdb.level.all.get_length(); i++)
            {
                var data = cdb.level.all.array.getDyn(i);
                data.doubleUps = 5;
            }
        }



    }

}

