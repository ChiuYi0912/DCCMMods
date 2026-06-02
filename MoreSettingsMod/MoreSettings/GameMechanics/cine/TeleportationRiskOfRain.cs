using System;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.inter;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.libs._Cooldown;
using dc.libs.heaps;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics.cine
{
    internal class TeleportationRiskOfRain : GameCinematic
    {
        private readonly Hero _hero;
        private readonly Entity _target;
        private readonly bool immediate;

        public TeleportationRiskOfRain(Hero hero, Teleport from, Entity target, bool immediate):base()
        {
            _hero = hero;
            _target = target;
            this.immediate = immediate;

            HUD.Class.ME.fullscreenMap(false, Ref<bool>.In(false));

            double baseTime = 0.75;
            _hero.setAffectS(4, baseTime + 0.9, Ref<double>.Null, null);
            _hero.disableRepellingForS(baseTime + 1.2, true);
            _hero.lockControlsS(2.5);
            _hero.cancelVelocities();


            Game.Class.ME.curLevel.fx.teleporterStart(from, 16022016, 53492);
            Audio.Class.ME.playUIEvent(
                (Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use1.wav".ToHaxeString(), Sound.Class),
                null);


            Delay(0.65, () =>
            {
                double x = ((double)_hero.cx + _hero.xr) * 24.0;
                double y = ((double)_hero.cy + _hero.yr) * 24.0 - _hero.hei * 0.5 - 96.0;
                Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".ToHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);
            });


            Delay(0.75, () =>
            {
                _hero.visible = false;
                Delay(0.6, () => Move());
            });

            Delay(1.05, () =>
            {
                Game.Class.ME.curLevel.fx.customMask(0, 1.0, 0.3, 0.4, 0.3, false);
            });
        }

        private void Move()
        {
            _hero.setPosCase(_target.cx, _target.cy, null, null);
            _hero.onTeleportation();
            Game.Class.ME.curLevel.viewport.track(_hero, immediate);

            Delay(0.4, () =>
            {
                if (_target is Teleport teleport)
                    Game.Class.ME.curLevel.fx.teleporterEnd(teleport, 53492);
            });


            Delay(0.7, () =>
            {
                double x = ((double)_hero.cx + _hero.xr) * 24.0;
                double y = ((double)_hero.cy + _hero.yr) * 24.0 - _hero.hei * 0.5 - 96.0;
                Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".ToHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);
            });


            Delay(0.8, () => End());


            var pets = _hero._level.entitiesByClass.get(584) as ArrayObj;
            if (pets != null)
            {
                for (int i = 0; i < pets.length; i++)
                {
                    if (pets.array[i] is TwitchPet pet)
                        pet.onHeroTeleport();
                }
            }

            _hero._level.game.user.userStats.teleportation++;
        }

        private void End()
        {

            Audio.Class.ME.playUIEvent(
                (Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use2.wav".ToHaxeString(), Sound.Class),
                null);
            _hero.visible = true;


            Delay(0.2, () =>
            {
                destroy();
                HUD.Class.ME.show(null);
            });
        }


        private void Delay(double seconds, Action action)
        {
            if (action == null) return;

            double frames = base.cd.baseFps * seconds;
            double ms = System.Math.Floor(frames * 1000) / 1000;


            int id = (int)(DateTime.Now.Ticks & 0x7FFFFFFF) ^ action.GetHashCode();

            CdInst inst = base.cd._getCdObject(id);
            if (inst != null && ms < inst.frames)
                return;


            if (inst != null)
            {
                base.cd.cdList.remove(inst);
                base.cd.fastCheck.remove(id);
            }


            base.cd.fastCheck.set(id, 1);
            CdInst newInst = new CdInst(id, ms);
            base.cd.cdList.push(newInst);
            newInst.cb = new HlAction(action);
        }

        public override bool onExit()
        {
            Move();
            _hero.visible = true;
            base.destroyed = true;
            return true;
        }
    }
}