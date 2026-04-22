using System;
using dc.en;
using dc.en.inter;
using dc.hxd;
using dc.hxd.res;
using dc.hxd.snd;
using dc.libs;
using dc.libs.misc;
using dc.libs._Cooldown;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;
using CoreLibrary.Core.Extensions;
using dc.hl.types;
using dc;

namespace CoreLibrary.Basedc.Cine
{
    public class Teleportation : GameCinematic
    {
        private readonly Hero _hero;
        private readonly Entity _target;
        private readonly Teleport _from;
        private readonly bool immediate;

        public Teleportation(Hero hero, Teleport from, Entity to, bool immediate) : base()
        {
            _hero = hero;
            _target = to;
            _from = from;
            this.immediate = immediate;


            HUD.Class.ME.fullscreenMap(false, Ref<bool>.In(false));


            double animDuration = 66.0 / 60.0;
            hero.spr.get_anim().play("teleport".ToHaxeString(), null, null);


            hero.setAffectS(4, animDuration + 0.9, Ref<double>.Null, null);
            hero.disableRepellingForS(animDuration + 1.2, true);
            hero.lockControlsS(2.5);
            hero.cancelVelocities();


            AddHeroCooldown(hero.cd, 1323302912, animDuration + 1.2);


            double fromWorldX = (from.cx + from.xr) * 24.0;
            double heroWorldX = (hero.cx + hero.xr) * 24.0;
            hero.dx = (fromWorldX - heroWorldX) * 0.03;


            Game.Class.ME.curLevel.fx.teleporterStart(from, 16022016, 53492);


            Viewport vp = Game.Class.ME.curLevel.viewport;
            if (animDuration > 0)
            {
                vp.tw.create_(GetZoomGetter(vp), GetZoomSetter(vp), null, 1.1, null, animDuration * 1000, Ref<bool>.Null)
                  .update(new HlAction(vp.updateSizes));
            }
            else
            {
                vp.set_zoom(1.1);
                vp.updateSizes();
            }


            double chromaDuration = (animDuration + 0.4) * 1000;
            Game.Class.ME.curLevel.tw.create_(GetChromaGetter(), GetChromaSetter(), 0.0, 4.0, null, chromaDuration, Ref<bool>.Null);


            Audio.Class.ME.playUIEvent(
                (Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use1.wav".ToHaxeString(), Sound.Class),
                null);


            Delay(base.cd, animDuration, () =>
            {
                _hero.visible = false;


                vp.tw.create_(GetZoomGetter(vp), GetZoomSetter(vp), null, 1.7, null, 400, Ref<bool>.Null)
                  .update(new HlAction(vp.updateSizes));


                double lightBeamX = (from.cx + from.xr) * 24.0;
                Game.Class.ME.curLevel.fx.teleportLightBeam(lightBeamX, Ref<int>.In(200), Ref<double>.In(0.4));


                Game.Class.ME.curLevel.fx.customMask(16776658, 1.0, 0.2, 0.2, 0.2, false);


                Delay(base.cd, 0.2, () => Move(), 796917760);
            }, 792723456);
        }


        private void Move()
        {
            _hero.setPosCase(_target.cx, _target.cy - 1, null, null);
            _hero.onTeleportation();
            Game.Class.ME.curLevel.viewport.track(_hero, immediate);


            Delay(base.cd, 0.35, End, 805306368);


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

            if (_target is Teleport teleport)
                Game.Class.ME.curLevel.fx.teleporterEnd(teleport, 53492);

            Delay(base.cd, 0.2, () => destroy(), 268435456);

            Viewport vp = Game.Class.ME.curLevel.viewport;
            vp.tw.create_(GetZoomGetter(vp), GetZoomSetter(vp), null, 1.0, new TType.TEaseOut(), 600, Ref<bool>.Null).update(new HlAction(vp.updateSizes));

            Game.Class.ME.curLevel.tw.create_(GetChromaGetter(), GetChromaSetter(), null, 0.0, null, 600, Ref<bool>.Null);

            HUD.Class.ME.show(null);
        }

        public override bool onExit()
        {
            Move();
            _hero.visible = true;
            destroyed = true;
            return true;
        }


        private void Delay(dc.libs.Cooldown cd, double seconds, Action action, int? id = null)
        {
            if (action == null) return;

            int cdId = id ?? (int)(DateTime.Now.Ticks & 0x7FFFFFFF) ^ action.GetHashCode();
            double frames = cd.baseFps * seconds;
            double targetFrames = System.Math.Floor(frames * 1000) / 1000;

            CdInst existing = cd._getCdObject(cdId);
            if (existing != null && targetFrames < existing.frames)
                return;

            if (existing != null)
            {
                cd.cdList.remove(existing);
                cd.fastCheck.remove(cdId);
            }

            cd.fastCheck.set(cdId, 1);
            CdInst newInst = new CdInst(cdId, targetFrames);
            cd.cdList.push(newInst);
            newInst.cb = new HlAction(action);
        }

        private void AddHeroCooldown(dc.tool.Cooldown cd, int id, double seconds)
        {
            double frames = cd.baseFps * seconds;
            double targetFrames = System.Math.Floor(frames * 1000) / 1000;

            CdInst existing = cd.fastCheck.get(id);
            if (existing != null && targetFrames < existing.frames)
                return;

            if (existing != null)
            {
                cd.cdList.remove(existing);
                cd.fastCheck.remove(id);
            }

            var newInst = new CdInst(id, targetFrames);
            cd.fastCheck.set(id, newInst);
            cd.cdList.push(newInst);
        }


        private HlFunc<double> GetZoomGetter(Viewport vp) => new HlFunc<double>(() => vp.zoom);

        private HlAction<double> GetZoomSetter(Viewport vp) => new HlAction<double>(z => { vp.set_zoom(z); });

        private HlFunc<double> GetChromaGetter() => new HlFunc<double>(() => Game.Class.ME.curLevel.scroller.chromaticAberration);

        private HlAction<double> GetChromaSetter() => new HlAction<double>(val => Game.Class.ME.curLevel.scroller.chromaticAberration = val);
    }
}