using dc;
using dc.en;
using dc.en.inter;
using dc.hl.types;
using dc.pr;
using dc.ui;
using HaxeProxy.Runtime;
using dc.libs.misc;

namespace MoreSettings.GameMechanics.cine
{
    internal class TeleportationFancy : GameCinematic
    {
        private const double TileSize = 24.0;
        private const double ZoomIn = 1.5f;
        private const double ZoomRestoreSeconds = 0.5;
        private const double ZoomNormal = 1.0;
        private const double CameraArriveDistance = TileSize * 5;

        private readonly Hero owen;
        private readonly Entity target;
        private readonly Teleport from;
        private readonly bool isimmediate;

        private bool cameraRestored;

        private bool moved;
        private bool waitingForCamera;
        private bool ended;

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

            Viewport vp = Game.Class.ME.curLevel.viewport;

            vp.tw.terminate_(GetZoomGetter(vp), GetZoomSetter(vp), false);
            vp.set_zoom(ZoomIn);
            vp.updateSizes();

            Game.Class.ME.curLevel.tw.create_(
                    GetChromaGetter(), GetChromaSetter(),
                    0.0, 4.0, null, 1000, Ref<bool>.Null);

            Move();
        }

        private void Move()
        {
            owen.setPosCase(target.cx, target.cy - 1, null, null);
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

            moved = true;
            waitingForCamera = true;
        }

        public override void update()
        {
            base.update();

            if (!moved || ended || !waitingForCamera)
                return;

            Viewport vp = Game.Class.ME.curLevel.viewport;

            double dx = vp.x - vp.tx;
            double dy = vp.y - vp.ty;
            double distSq = dx * dx + dy * dy;

            bool arrived = distSq <= CameraArriveDistance * CameraArriveDistance;

            if (arrived)
            {
                waitingForCamera = false;

                //vp.jumpToTarget();

                End();
            }
        }

        private void End()
        {
            if (ended)
                return;

            ended = true;
            waitingForCamera = false;

            RestoreCamera();

            owen.visible = true;
            HUD.Class.ME.show(null);

            if (target is Teleport tp)
                Game.Class.ME.curLevel.fx.teleporterEnd(tp, 53492);
            destroy();
        }

        public override bool onExit()
        {
            if (!ended)
            {
                ended = true;
                waitingForCamera = false;
                RestoreCamera();
            }

            destroyed = true;
            return true;
        }

        private void RestoreCamera()
        {
            if (cameraRestored)
                return;

            cameraRestored = true;

            Viewport vp = Game.Class.ME.curLevel.viewport;

            vp.tw.terminate_(GetZoomGetter(vp), GetZoomSetter(vp), false);
            vp.set_zoom(ZoomIn);
            vp.updateSizes();

            vp.tw.create_(
                    GetZoomGetter(vp),
                    GetZoomSetter(vp),
                    null,
                    ZoomNormal,
                    new TType.TEaseOut(),
                    ZoomRestoreSeconds * 1000,
                    Ref<bool>.Null)
                .update(new HlAction(vp.updateSizes));

            vp.tw.create_(GetChromaGetter(), GetChromaSetter(), null, 0.0, null, 1000, Ref<bool>.Null);
        }

        private HlFunc<double> GetZoomGetter(Viewport vp)
            => new HlFunc<double>(() => vp.zoom);

        private HlAction<double> GetZoomSetter(Viewport vp)
            => new HlAction<double>(z => { vp.set_zoom(z); });

        private HlFunc<double> GetChromaGetter()
            => new HlFunc<double>(() => Game.Class.ME.curLevel.scroller.chromaticAberration);

        private HlAction<double> GetChromaSetter()
            => new HlAction<double>(val => Game.Class.ME.curLevel.scroller.chromaticAberration = val);
    }
}