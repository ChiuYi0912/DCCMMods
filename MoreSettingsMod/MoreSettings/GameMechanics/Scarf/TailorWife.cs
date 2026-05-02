using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.inter;
using dc.level;
using dc.libs;
using dc.libs.heaps.slib;
using dc.pr;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics.Scarf
{
    public class TailorWife : Npc
    {
        public bool headFlip { get; set; }
        public bool isInMobsRoom { get; set; }
        public class TailorWifeNPC : NpcId.Berserk
        {

        }

        public TailorWife(Level lvl, NpcId id) : base(lvl, id)
        {
        }


        public override void initGfx()
        {
            base.initGfx();


            var lib = Assets.Class.lib.get("atlas/ghost.atlas".ToHaxeString());
            base.initSprite(lib, "npcGhostIdle".ToHaxeString(), 0.5, 1.0, Const.Class.DP_ROOM_MAIN, null, null, null);
            base.spr.get_anim().registerStateAnim("npcGhostIdle".ToHaxeString(), 0, 0.2, null, Ref<bool>.Null, null);

        }


        public override void initSpeechDeck()
        {
            base.speechSfxDeck = new RandDeck(null, Ref<int>.Null);

        }


        public override void onActivate(Hero by, bool lp)
        {
            base.onActivate(by, lp);

            if (progress == 0)
            {
                SayFirstMeeting();
                progress = 1;
            }
            else
            {
                SayHelloAgain();
            }
        }


        public override void onGreet(Hero h)
        {
            base.onGreet(h);
            if (progress == 0)
                SayFirstMeeting();
            else
                SayHelloAgain();
        }


        private void SayFirstMeeting()
        {
            string msg = isInMobsRoom
                ? "欢迎来到战斗房间！你可以在这里安全地挑战怪物。"
                : "你好，我是训练骑士。你可以在这里练习战斗技巧。";
            base.say(msg.ToHaxeString(), null, null, null);
        }


        private void SayHelloAgain()
        {
            string msg = isInMobsRoom
                ? "又见面了，继续磨练吧！"
                : "坚持训练，你会变强的！";
            base.say(msg.ToHaxeString(), null, null, null);
        }


        public override void fixedUpdate()
        {
            base.fixedUpdate();
            Hero hero = base._level.game.hero;
            if (hero != null && !base._level.game.hasCinematic())
            {

                bool flip = hero.cx < this.cx;
                this.headFlip = flip;
            }
        }


        public override void unserializeInit()
        {
            base.unserializeInit();
            this.headFlip = false;
            this.isInMobsRoom = false;
        }
    }
}