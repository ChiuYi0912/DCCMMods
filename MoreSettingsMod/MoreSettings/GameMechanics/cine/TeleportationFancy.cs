using System;
using dc;
using dc.en;
using dc.en.inter;
using dc.hl.types;
using dc.pr;
using dc.ui;
using dc.hxd.snd;
using dc.hxd.res;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics.cine
{
    public class TeleportationFancy : GameCinematic
    {
        private readonly Hero owen;
        private readonly Entity target;
        private readonly Teleport from;
        private readonly bool isimmediate;

        public TeleportationFancy(Hero hero, Teleport from, Entity to, bool immediate) : base()
        {
            owen = hero;
            target = to;
            this.from = from;
            isimmediate = immediate;

            HUD.Class.ME.fullscreenMap(false, Ref<bool>.In(false));

            owen.cancelVelocities();
            owen.lockControlsS(0.2);

            CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/inter/portal_use1.wav");

            Game.Class.ME.curLevel.fx.teleporterStart(from, 16022016, 53492);

            Move();
        }

        private void Move()
        {
            owen.setPosCase(target.cx, target.cy, null, null);
            owen.onTeleportation();

            Game.Class.ME.curLevel.viewport.track(owen, isimmediate);
            Game.Class.ME.curLevel.fx.customMask(16776658, 1.0, 0.2, 0.2, 0.2, false);

            var pets = owen._level.entitiesByClass.get(584) as ArrayObj;
            if (pets != null)
            {
                for (int i = 0; i < pets.length; i++)
                {
                    if (pets.array[i] is TwitchPet pet)
                        pet.onHeroTeleport();
                }
            }

            owen._level.game.user.userStats.teleportation++;

            CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/inter/portal_use2.wav");

            End();
        }

        private void End()
        {
            owen.visible = true;
            HUD.Class.ME.show(null);
            if (target is Teleport tp)
                Game.Class.ME.curLevel.fx.teleporterEnd(tp, 53492);
            destroy();
        }

        public override bool onExit()
        {
            Move();
            destroyed = true;
            return true;
        }
    }
}