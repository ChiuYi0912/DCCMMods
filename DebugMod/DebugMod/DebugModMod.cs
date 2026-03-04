using System.ComponentModel.DataAnnotations;
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
using dc.level.disp;
using dc.libs;
using dc.libs.tilemap;
using dc.light;
using dc.pow;
using dc.tool;
using dc.tool.vote;
using dc.ui;
using dc.ui.hud;
using DebugMod.Utitities;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.DynamicAccess;
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

        public override void Initialize()
        {
            base.Initialize();

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
            var vote = new TwitchVote();
            vote.setDesc("选择".AsHaxeString());
            vote.addVoteOption("!red".AsHaxeString(), "红色".AsHaxeString(), null);
            vote.addVoteOption("!blue".AsHaxeString(), "蓝色".AsHaxeString(), null);
            vote.addAlias("!r".AsHaxeString(), "!red".AsHaxeString());
            vote.oneVotePerUser = true;
            vote.showPct = true;
            vote.setExpireS(30.0);
            vote.keepOnNextLevel = true;
            vote.longVoteLabels = false;
            vote.init();

            var nextLevelVote = new NextLevel();
            nextLevelVote.showPct = true;
            nextLevelVote.blinkOnVote = false;
            nextLevelVote.longVoteLabels = true;
            nextLevelVote.locksTwitchDoor = false; 
            nextLevelVote.setForcedInitS(99999.0);
            var desc = Lang.Class.t.get("Ou voudriez-vous aller ensuite ?".AsHaxeString(), null);
            nextLevelVote.setDesc(desc);


            string deathReason = "被陷阱杀死";
            var tauntVote = new DeathTaunt(deathReason.AsHaxeString());
            tauntVote.showPct = true;
            tauntVote.oneVotePerUser = true;
            tauntVote.blinkOnVote = true;

            desc = Lang.Class.t.get("Should we open the secret door?".AsHaxeString(), null);
            var vote2 = new DoOrDont(desc);

            var vote3 = new dc.tool.vote.Taunt("test".AsHaxeString());

            var vote4 = new SecretTip();

            var vote5 = new Praise("test".AsHaxeString());

            // var chat = new TwitchChest(dc.pr.Game.Class.ME.curLevel, dc.pr.Game.Class.ME.hero.cx, dc.pr.Game.Class.ME.hero.cy);

            // var vote6 = new OpenChest(chat);

            var vote7 = new Encourage("test".AsHaxeString());



        }

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {

        }


    }




}

