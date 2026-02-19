using ChiuYiUI.Core;
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
using ModCore.Utilities;

namespace ChiuYiUI.GameMechanics
{
    public class DIYFlashTeleport : GameCinematic
    {

        public readonly Hero hero;
        public readonly Entity to;
        public double viewportzoom = 0;

        public DIYFlashTeleport(Hero owen, Teleport from, Entity entity)
        {
            hero = owen;
            to = entity;
            bool restoreHUD = false;
            CHIUYIMain.config.Value.playerCameraSpeed = Main.Class.ME.options.playerCameraSpeed;
            CHIUYIMain.config.Save();

            dynamicviewport();
            HUD.Class.ME.fullscreenMap(false, Ref<bool>.From(ref restoreHUD));

            double baseTime = 0.75;
            hero.setAffectS(4, baseTime + 0.9, Ref<double>.Null, null);
            hero.disableRepellingForS(baseTime + 1.2, true);
            hero.lockControlsS(2.5);
            hero.cancelVelocities();
            viewportzoom = Game.Class.ME.curLevel.viewport.zoom;
            Game.Class.ME.curLevel.viewport.zoomFromTo(Game.Class.ME.curLevel.viewport.zoom, 0.5, 0.5, null);

            Game.Class.ME.curLevel.fx.teleporterStart(from, 16022016, 53492);
            Audio.Class.ME.playUIEvent((Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use1.wav".AsHaxeString(), Sound.Class), null);



            SetupCooldownAndCallback(this.cd, 788529152, baseTime - 0.1,
              new HlAction(() =>
              {
                  Fx fx = Game.Class.ME.curLevel.fx;
                  double x = ((double)this.hero.cx + this.hero.xr) * 24.0;
                  double y = ((double)this.hero.cy + this.hero.yr) * 24.0 - this.hero.hei * 0.5 - 96.0;
                  HParticle hparticle = fx.playAnimOld("fxTeleportRoR".AsHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);


              }),
                (cooldownSeconds) =>
                {
                    if (cooldownSeconds <= 0)
                    {
                        double x = (hero.cx + hero.xr) * 24.0;
                        double y = (hero.cy + hero.yr) * 24.0 - hero.hei * 0.5 - 96.0;
                        Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".AsHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);

                    }
                });



            SetupCooldownAndCallback(this.cd, 792723456, baseTime,
                new HlAction(() =>
                {
                    ArrowFunctionEntry_16340();
                }),
                (cooldownSeconds) =>
                {
                    if (cooldownSeconds <= 0)
                    {
                        hero.visible = false;
                        SetupCooldownAndCallback(this.cd, 796917760, 0.6,
                            new HlAction(this.move),
                            null!);
                    }
                });



            SetupCooldownAndCallback(this.cd, 809500672, baseTime + 0.3,
                new HlAction(() =>
                {
                    // Game.Class.ME.curLevel.fx.customMask(0, 1.0, 0.3, 0.4, 0.3, false);
                }),
                (cooldownSeconds) =>
                {
                    if (cooldownSeconds <= 0)
                    {
                        // Game.Class.ME.curLevel.fx.customMask(0, 1.0, 0.3, 0.4, 0.3, false);
                    }
                });
        }

        public void dynamicviewport()
        {

            double portalX = ((double)this.to.cx + this.to.xr) * 24.0;
            double portalY = ((double)this.to.cy + this.to.yr) * 24.0;

            double heroX = ((double)this.hero.cx + this.hero.xr) * 24.0;
            double heroY = ((double)this.hero.cy + this.hero.yr) * 24.0;
            double distance = System.Math.Sqrt(System.Math.Pow(portalX - heroX, 2) + System.Math.Pow(portalY - heroY, 2));

            double minDistance = 300.0;
            double maxDistance = 10000.0;
            double minSpeed = 2.0;
            double maxSpeed = 5.0;

            double cameraSpeed = minSpeed;
            if (distance <= minDistance)
            {
                cameraSpeed = minSpeed;
            }
            else if (distance >= maxDistance)
            {
                cameraSpeed = maxSpeed;
            }
            else
            {

                double t = (distance - minDistance) / (maxDistance - minDistance);
                cameraSpeed = minSpeed + t * (maxSpeed - minSpeed);
            }

            Main.Class.ME.options.playerCameraSpeed = cameraSpeed;
        }


        public void ArrowFunctionEntry_16340()
        {
            this.hero.visible = false;

            SetupCooldownAndCallback(this.cd, 796917760, 0.6,
                new HlAction(this.move),
                (cooldownSeconds) =>
                {
                    if (cooldownSeconds <= 0)
                    {
                        this.move();
                    }
                });
        }

        private static void SetupCooldownAndCallback(dc.libs.Cooldown cd, int id, double timeFactor, HlAction callback, Action<double> immediateAction)
        {
            double cooldownSeconds = System.Math.Floor(cd.baseFps * timeFactor * 1000) / 1000;
            CdInst cdInst = cd._getCdObject(id);


            if (cdInst != null && cooldownSeconds < cdInst.frames)
            {
                return;
            }


            if (cooldownSeconds <= 0 && cdInst != null)
            {
                cd.cdList.remove(cdInst);
                cdInst.frames = 0.0;
                cdInst.cb = null;
                cd.fastCheck.remove(cdInst.k);
            }
            else
            {
                cd.fastCheck.set(id, 1);

                if (cdInst != null)
                {
                    cdInst.frames = cooldownSeconds;
                }
                else
                {
                    cd.cdList.push(new CdInst(id, cooldownSeconds));
                }
            }

            if (callback == null) return;

            if (cooldownSeconds <= 0)
            {
                immediateAction?.Invoke(cooldownSeconds);
                return;
            }

            CdInst callbackCdInst = cd._getCdObject(id);
            if (callbackCdInst == null)
            {
                throw new Exception($"cannot bind onComplete({id}): cooldown {id} isn't running");
            }

            callbackCdInst.cb = callback;
        }






        public void ArrowFunction_move_16342()
        {
            if (!Std.Class.@is(this.to, Teleport.Class)) return;
            Game.Class.ME.curLevel.fx.teleporterEnd((Teleport)this.to, 53492);
        }

        public void ArrowFunction_move_16343()
        {
            Hero hero = this.hero;
            Hero hero2 = this.hero;
            Fx fx = Game.Class.ME.curLevel.fx;
            double x = ((double)hero.cx + hero.xr) * 24.0;
            double y = ((double)hero2.cy + hero2.yr) * 24.0 - hero2.hei * 0.5 - 96.0;
            HParticle hparticle = fx.playAnimOld("fxTeleportRoR".AsHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);

        }

        public void end()
        {
            Audio.Class.ME.playUIEvent((Sound)Res.Class.get_loader().loadCache("sfx/inter/portal_use2.wav".AsHaxeString(), Sound.Class), null);

            this.hero.visible = true;

            dc.libs.Cooldown cd = base.cd;
            double cooldownSeconds = System.Math.Floor(base.cd.baseFps * 0.2 * 1000) / 1000;
            double remainingTime = cooldownSeconds;

            HlAction onComplete = new(() =>
            {
                this.destroy();
            });

            const int COOLDOWN_ID = 268435456;
            CdInst cdInst = cd._getCdObject(COOLDOWN_ID);

            if (cdInst != null && cooldownSeconds < cdInst.frames)
            {
                HUD.Class.ME.show(null);
                return;
            }

            if (cdInst != null)
            {
                cd.cdList.remove(COOLDOWN_ID);
            }

            cd.fastCheck.set(COOLDOWN_ID, 1);

            if (cdInst != null)
            {
                cdInst.frames = cooldownSeconds;
            }
            else
            {
                cd.cdList.push(new CdInst(COOLDOWN_ID, cooldownSeconds));
            }

            if (onComplete == null)
            {
                HUD.Class.ME.show(null);
                return;
            }

            if (remainingTime <= 0)
            {
                onComplete.Invoke();
                HUD.Class.ME.show(null);
                return;
            }

            CdInst callbackCdInst = cd._getCdObject(COOLDOWN_ID);
            if (callbackCdInst == null)
            {
                throw new Exception($"cannot bind onComplete({COOLDOWN_ID}): cooldown {COOLDOWN_ID} isn't running");
            }

            callbackCdInst.cb = onComplete;
            HUD.Class.ME.show(null);
        }

        public void move()
        {

            Hero hero = this.hero;
            hero.setPosCase(this.to.cx, this.to.cy, null, null);
            hero.onTeleportation();
            Game.Class.ME.curLevel.viewport.track(hero, false);


            SetupCooldownAndCallback(801112064, 0.4, ArrowFunction_move_16342, (cdInst, cooldownSeconds, callback) =>
            {
                if (cooldownSeconds <= 0 && this.to is Teleport teleport)
                {
                    Game.Class.ME.curLevel.fx.teleporterEnd(teleport, 53492);
                }
            });

            SetupCooldownAndCallback(788529152, 0.7, ArrowFunction_move_16343, (cdInst, cooldownSeconds, callback) =>
            {
                if (cooldownSeconds <= 0)
                {
                    double x = (hero.cx + hero.xr) * 24.0;
                    double y = (hero.cy + hero.yr) * 24.0 - hero.hei * 0.5 - 96.0;
                    Game.Class.ME.curLevel.fx.playAnimOld("fxTeleportRoR".AsHaxeString(), x, y, 1, 1.5, null, null, Ref<bool>.Null);


                }
            });

            SetupCooldownAndCallback(805306368, 0.8, end, (cdInst, cooldownSeconds, callback) =>
            {
                if (cooldownSeconds <= 0 && callback != null)
                {
                    callback.Invoke();
                }
                var pets = this.hero._level.entitiesByClass.get(584) as ArrayObj;
                if (pets != null)
                {
                    for (int i = 0; i < pets.length; i++)
                    {
                        if (pets.array[i] is TwitchPet pet)
                        {
                            pet.onHeroTeleport();
                        }
                    }
                }

                UserStats stats = this.hero._level.game.user.userStats;
                stats.teleportation++;
            });
            Game.Class.ME.curLevel.viewport.zoomFromTo(Game.Class.ME.curLevel.viewport.zoom, this.viewportzoom, 0.5, null);
            Main.Class.ME.options.playerCameraSpeed = CHIUYIMain.config.Value.playerCameraSpeed;

        }

        private void SetupCooldownAndCallback(int id, double timeFactor, HlAction callback, Action<CdInst, double, HlAction> immediateAction)
        {
            dc.libs.Cooldown cd = base.cd;
            double cooldownSeconds = System.Math.Floor(cd.baseFps * timeFactor * 1000) / 1000;
            double remainingTime = cooldownSeconds;

            CdInst cdInst = cd._getCdObject(id);

            if (cdInst != null && cooldownSeconds < cdInst.frames)
            {
                return;
            }

            if (cooldownSeconds <= 0 && cdInst != null)
            {
                cd.cdList.remove(cdInst);
                cdInst.frames = 0.0;
                cdInst.cb = null;
                cd.fastCheck.remove(cdInst.k);
            }
            else
            {
                cd.fastCheck.set(id, 1);

                if (cdInst != null)
                {
                    cdInst.frames = cooldownSeconds;
                }
                else
                {
                    cd.cdList.push(new CdInst(id, cooldownSeconds));
                }
            }

            if (callback == null) return;

            if (cooldownSeconds <= 0)
            {
                immediateAction?.Invoke(cdInst!, cooldownSeconds, callback);
                return;
            }

            CdInst callbackCdInst = cd._getCdObject(id);
            if (callbackCdInst == null)
            {
                throw new Exception($"无法绑定 onComplete({id}): cooldown {id} 没运行");
            }

            callbackCdInst.cb = callback;
            immediateAction?.Invoke(callbackCdInst, cooldownSeconds, callback);
        }


        public override bool onExit()
        {
            this.move();
            this.hero.visible = true;
            base.destroyed = true;
            return true;
        }

    }
}