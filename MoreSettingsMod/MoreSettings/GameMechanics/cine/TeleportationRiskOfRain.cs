using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.inter;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.libs._Cooldown;
using dc.libs.misc;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics.cine
{
    internal class TeleportationRiskOfRain : GameCinematic
    {
        private const double FancyZoomIn = 1.5;
        private const double FancyZoomNormal = 1.0;
        private const double FancyZoomInSeconds = 0.5;
        private const double FancyZoomRestoreSeconds = 0.5;

        private readonly Hero hero;
        private readonly Entity target;
        private readonly bool immediate;

        private bool cameraRestored;

        public TeleportationRiskOfRain(Hero hero, Teleport from, Entity target, bool immediate):base()
        {
            this.hero = hero;
            this.target = target;
            this.immediate = immediate;

            HUD.Class.ME.fullscreenMap(false, Ref<bool>.In(false));

            double baseTime = 0.75;
            hero.setAffectS(4, baseTime + 0.9, Ref<double>.Null, null);
            hero.disableRepellingForS(baseTime + 1.2, true);
            hero.lockControlsS(2.5);
            hero.cancelVelocities();


            Game.Class.ME.curLevel.fx.teleporterStart(from, 16022016, 53492);
            Audio.Class.ME.playUIEvent(
                (Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use1.wav".ToHaxeString(), Sound.Class),
                null);

            if (!immediate)
            {
                Viewport vp = Game.Class.ME.curLevel.viewport;

                vp.tw.terminate_(GetZoomGetter(vp), GetZoomSetter(vp), false);
                vp.tw.create_(
                        GetZoomGetter(vp),
                        GetZoomSetter(vp),
                        null,
                        FancyZoomIn,
                        new TType.TEaseOut(),
                        FancyZoomInSeconds * 1000,
                        Ref<bool>.Null)
                    .update(new HlAction(vp.updateSizes));
            }


            Delay(0.65, () =>
            {
                double x = ((double)hero.cx + hero.xr) * 24.0;
                double y = ((double)hero.cy + hero.yr) * 24.0 - hero.hei * 0.5 - 96.0;
                Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".ToHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);
            });


            Delay(0.75, () =>
            {
                hero.visible = false;
                Delay(0.6, () => Move());
            });

            Delay(1.05, () =>
            {
                Game.Class.ME.curLevel.fx.customMask(0, 1.0, 0.3, 0.4, 0.3, false);
            });
        }

        private void Move()
        {
            hero.setPosCase(target.cx, target.cy, null, null);
            hero.onTeleportation();
            Game.Class.ME.curLevel.viewport.track(hero, immediate);

            Delay(0.4, () =>
            {
                if (target is Teleport teleport)
                    Game.Class.ME.curLevel.fx.teleporterEnd(teleport, 53492);
            });


            Delay(0.7, () =>
            {
                double x = ((double)hero.cx + hero.xr) * 24.0;
                double y = ((double)hero.cy + hero.yr) * 24.0 - hero.hei * 0.5 - 96.0;
                Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".ToHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);
            });


            Delay(0.8, () => End());


            var pets = hero._level.entitiesByClass.get(584) as ArrayObj;
            if (pets != null)
            {
                for (int i = 0; i < pets.length; i++)
                {
                    if (pets.array[i] is TwitchPet pet)
                        pet.onHeroTeleport();
                }
            }

            hero._level.game.user.userStats.teleportation++;
        }

        private void End()
        {

            Audio.Class.ME.playUIEvent(
                (Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use2.wav".ToHaxeString(), Sound.Class),
                null);
            hero.visible = true;

            RestoreCamera();


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
            RestoreCamera();
            hero.visible = true;
            base.destroyed = true;
            return true;
        }

        private void RestoreCamera()
        {
            if (immediate || cameraRestored)
                return;

            cameraRestored = true;

            Viewport vp = Game.Class.ME.curLevel.viewport;

            vp.tw.terminate_(GetZoomGetter(vp), GetZoomSetter(vp), false);
            vp.set_zoom(FancyZoomIn);
            vp.updateSizes();

            vp.tw.create_(
                    GetZoomGetter(vp),
                    GetZoomSetter(vp),
                    null,
                    FancyZoomNormal,
                    new TType.TEaseOut(),
                    FancyZoomRestoreSeconds * 1000,
                    Ref<bool>.Null)
                .update(new HlAction(vp.updateSizes));
        }

        private HlFunc<double> GetZoomGetter(Viewport vp)
            => new HlFunc<double>(() => vp.zoom);

        private HlAction<double> GetZoomSetter(Viewport vp)
            => new HlAction<double>(z => { vp.set_zoom(z); });
    }
}