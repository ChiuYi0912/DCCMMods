using dc;
using dc.en;
using dc.en.inter;
using dc.en.mob;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.level;
using dc.libs.heaps.slib;
using dc.libs.heaps.slib._AnimManager;
using dc.libs.misc;
using dc.pr;
using dc.tool;
using dc.ui;
using dc.ui.icon;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace ChiuYiUI.GameCm
{
    public class OnDIYTire : GameCinematic
    {
        private Icon icon = null!;
        private bool destory;
        private Entity entity = null!;
        private Hero hero = null!;
        private int color2;
        public OnDIYTire(Hero owen, Entity e, InventItem item, double iconX, double iconY, HlAction<bool> onComplete)
        {
            this.entity = e;
            this.hero = owen;
            owen.cancelVelocities();
            var kind = item.kind;
            dc.String itemKind = null!;
            dc.String string2 = null!;

            switch (kind.RawIndex)
            {
                case 0:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 1:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 2:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 3:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 4:
                    {
                        dc.String @string = null!;
                        itemKind = @string;
                        break;
                    }
                case 5:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 6:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 7:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 8:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 9:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 10:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 11:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
                case 12:
                    {
                        dc.String @string = kind.Index.ToString().AsHaxeString();
                        string2 = @string;
                        itemKind = @string;
                        break;
                    }
            }


            this.icon = Icon.Class.createItemIcon("BrutalityUp".AsHaxeString(), null);

            double px = 0.5;
            double py = 0.5;
            this.icon.setCenterRatio(Ref<double>.From(ref px), Ref<double>.From(ref py));
            icon.posChanged = true;


            icon.x = iconX;
            icon.y = iconY - (double)this.icon.tile.height * 5;


            int color1;
            Dictionary<string, int> colorMap = new Dictionary<string, int>
            {
                { "BrutalityUp", 14492213 },
                { "SurvivalUp", 7971619 },
                { "TacticUp", 11763255 }
            };

            if (string2 != null && colorMap.TryGetValue(string2.ToString(), out int color))
            {
                color1 = color;
                color2 = color;
            }
            else
            {
                color1 = 4477883;
                color2 = color1;
            }


            this.destory = !IsShrine(e);
            if (this.destory)
            {
                e.visible = false;
            }
            createCm(item, onComplete);
        }

        public void createCm(InventItem item, HlAction<bool> onComplete)
        {
            this.cm.__beginNewQueue();
            HlAction ac1 = new HlAction(() => { destroyItem(this.destory, this.entity); });
            this.cm.__add(ac1, 0, null);

            HlAction ac2 = new HlAction(() => { Game.Class.ME.curLevel.scroller.addChildAt(this.icon, Const.Class.DP_FOREGROUND); });
            this.cm.__add(ac2, 0, null);

            bool flag2 = false;


            if (hero._level != null && hero._level.map != null)
            {
                LevelMap map = hero._level.map;


                bool isGroundBelow = false;
                int heroCx = hero.cx;
                int heroCyBelow = hero.cy + 1;


                if (heroCx >= 0 && heroCx < map.wid && heroCyBelow >= 0 && heroCyBelow < map.hei)
                {

                    ArrayBytes_Int collisions = map.collisions;
                    int index = heroCyBelow * map.wid + heroCx;
                    int collisionValue = 0;

                    if (index < collisions.length)
                    {
                        collisionValue = collisions.getDyn(2);
                    }


                    isGroundBelow = (collisionValue & 1) != 0;
                }


                if (isGroundBelow)
                {
                    double heroYr = hero.yr;


                    double tempXr = hero.xr;
                    double tempYr = hero.yr;
                    double groundYr = map.getGroundYr(hero.cx, hero.cy, Ref<double>.From(ref tempXr), Ref<double>.From(ref tempYr));

                    if (heroYr > groundYr && System.Math.Abs(hero.dy) < 0.0001)
                    {
                        flag2 = true;
                    }
                }

                else if (!hero.ignoreSlopes && System.Math.Abs(hero.dy) < 0.1)
                {
                    int heroCx2 = hero.cx;
                    int heroCy2 = hero.cy;


                    if (heroCx2 >= 0 && heroCx2 < map.wid && heroCy2 >= 0 && heroCy2 < map.hei)
                    {

                        ArrayBytes_Int collisions = map.collisions;
                        int index = heroCy2 * map.wid + heroCx2;
                        int collisionValue = 0;

                        if (index < collisions.length)
                        {
                            collisionValue = collisions.getDyn(2);
                        }


                        flag2 = (collisionValue & 512) != 0;
                    }
                }
            }

            if (flag2)
            {
                HlAction ac3 = new HlAction(() =>
                {
                    double px = 26.400000000000002;
                    this.heroWalkUntilEntity(this.hero, this.entity, Ref<double>.From(ref px), Ref<double>.Null, null);
                });
                this.cm.__add(ac3, 0, null);


                HlAction ac4 = new HlAction(() => { });
                this.cm.__add(ac4, 0, "walk".AsHaxeString());

            }


            HlAction ac5 = new HlAction(() => { this.hero.hasGravity = false; });
            this.cm.__add(ac5, 0, null);

            HlAction ac6 = new HlAction(() =>
            {
                AnimManager animManager = this.hero.spr.get_anim().play("rise".AsHaxeString(), null, null);
                if (animManager.destroyed) return;

                if (0 >= animManager.stack.length) return;

                ArrayObj stack = animManager.stack;
                int length = stack.length;
                AnimInstance animInstance;
                if (animManager.stack.length - 1 >= length)
                {
                    animInstance = null!;
                    animInstance.speed = 1.5;
                    return;
                }
                if (stack == null) return;
                animInstance = (dynamic)stack.array[animManager.stack.length - 1]!;
                animInstance.speed = 1.5;
            });
            this.cm.__add(ac6, 0, null);


            HlAction ac7 = new HlAction(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.x; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setV =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double x = (double)_setV;
                    icon.x = x;
                });
                HlAction<double> setter = new((al) =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });

                double? from = ((double)this.hero.cx + this.hero.xr) * 24.0 + (double)(this.hero.dir * 25);
                double to = ((double)this.hero.cx + this.hero.xr) * 24.0 + (double)(this.hero.dir * 20);
                TType tp = new TType.TEaseOut();
                Tween tween = tw.create_(getter, setter, from, to, tp, 250.0, Ref<bool>.Null);
            });
            this.cm.__add(ac7, 0, null);



            HlAction ac8 = new(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.y; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setY =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double y = (double)_setY;
                    icon.y = y;
                });
                HlAction<double> setter = new((al) =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });
                int height = this.icon.tile.height;
                double? from = ((double)this.hero.cy + this.hero.yr) * 24.0 + 9.0 - (double)height * 0.5;
                double to = ((double)this.hero.cy + this.hero.yr) * 24.0 - 20.0 - (double)height * 0.5;
                TType tp = new TType.TEaseIn();
                Tween tween = tw.create_(getter, setter, from, to, tp, 250.0, Ref<bool>.Null);
            });
            this.cm.__add(ac8, 0, null);

            HlAction ac10 = new(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.rotation; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setY =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double rotation = (double)_setY;
                    icon.rotation = rotation;
                });
                HlAction<double> setter = new((al) =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });
                int height = this.icon.tile.height;
                double from = (double)this.hero.dir * 0.6;
                double to = (double)this.hero.dir * 0.3;
                TType tp = new TType.TEaseIn();
                var tween = tw.create_(getter, setter, from, to, tp, 250.0, Ref<bool>.Null);
            });
            this.cm.__add(ac10, 0, null);


            HlAction ac11 = new(() =>
            {
                LevelAudio lAudio = Game.Class.ME.curLevel.lAudio;
                Loader loader = Res.Class.get_loader();
                _Sound @class = Sound.Class;
                dc.level._LevelAudio.Event @event = lAudio.playEvent((Sound)loader.loadCache("sfx/hero/jump.wav".AsHaxeString(), @class), 0.5, null, null);
            });
            this.cm.__add(ac11, 0, null);


            HlAction ac13 = new(() =>
            {
                this.cm.__addParallel(new(() =>
                {
                    Game.Class.ME.curLevel.fx.majorItemCharge(((double)this.hero.cx + this.hero.xr) * 24.0, ((double)this.hero.cy + this.hero.yr) * 24.0, this.color2, 0.7);
                }), 400, null);
            });
            this.cm.__add(ac13, 0, null);

            HlAction ac14 = new(() =>
            {
                this.cm.__addParallel(new(() =>
                {
                    LevelAudio lAudio = Game.Class.ME.curLevel.lAudio;
                    Loader loader = Res.Class.get_loader();
                    _Sound @class = Sound.Class;
                    dc.level._LevelAudio.Event @event = lAudio.playEvent((Sound)loader.loadCache("sfx/enm/enm_shield_charge.wav".AsHaxeString(), @class), 0.5, null, null);
                }), 400, null);
            });
            this.cm.__add(ac14, 0, null);


            HlAction ac15 = new(() => { });
            this.cm.__add(ac15, 250, null);


            HlAction ac16 = new(() =>
            {
                double move = (double)hero.dir * 0.1;


                if (hero.life <= 0 || hero.destroyed) return;


                int directionSign = move > 0 ? 1 : -1;

                double baseValue = 0.5;
                double combinedSpeed = hero.dx + hero.bdx;
                double absMove = System.Math.Abs(move);
                combinedSpeed = (combinedSpeed + absMove) * 5.0;
                baseValue += combinedSpeed;

                dc.level.Platform platform = hero.get_pf();
                double distanceToBoundary;

                if (platform == null)
                {
                    distanceToBoundary = 0.0;
                }
                else
                {
                    double currentX = (double)hero.cx + hero.xr;
                    int boundary = directionSign == -1
                        ? platform.left
                        : platform.left + platform.wid;
                    distanceToBoundary = System.Math.Abs(currentX - (double)boundary);
                }

                if (baseValue < distanceToBoundary)
                {
                    hero.bump(move, 0.0, true);
                }
            });
            this.cm.__add(ac16, 0, null);

            HlAction ac17 = new(() =>
            {
                this.cm.__addParallel(new(() =>
            {
                double move = -(double)this.hero.dir * 0.05;


                if (this.hero.life <= 0 || this.hero.destroyed)
                    return;

                int direction = move > 0 ? 1 : -1;

                double threshold = 0.5;
                double combinedSpeed = this.hero.dx + this.hero.bdx;
                double absMove = System.Math.Abs(move);
                combinedSpeed = (combinedSpeed + absMove) * 5.0;
                threshold += combinedSpeed;


                dc.level.Platform platform = this.hero.get_pf();
                double distanceToEdge;

                if (platform == null)
                {
                    distanceToEdge = 0.0;
                }
                else
                {
                    double currentX = (double)this.hero.cx + this.hero.xr;
                    int edgeX = direction == -1
                        ? platform.left
                        : platform.left + platform.wid;

                    distanceToEdge = System.Math.Abs(currentX - edgeX);
                }
                if (threshold < distanceToEdge)
                {
                    this.hero.bump(move, 0.0, true);
                }
            }), 100, null);
            });
            this.cm.__add(ac17, 0, null);


            HlAction ac18 = new(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.x; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setY =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double x = (double)_setY;
                    icon.x = x;
                });
                HlAction<double> setter = new(al =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });

                double to = ((double)this.hero.cx + this.hero.xr) * 24.0;
                TType tp = new TType.TEaseIn();
                var tween = tw.create_(getter, setter, null, to, tp, 200.0, Ref<bool>.Null);
            });
            this.cm.__add(ac18, 0, null);

            HlAction ac19 = new(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.y; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setY =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double y = (double)_setY;
                    icon.y = y;
                });
                HlAction<double> setter = new((al) =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });
                int height = this.icon.tile.height;
                double to = ((double)this.hero.cy + this.hero.yr) * 24.0 - this.hero.hei - 16.0 - (double)height * 0.5;
                TType tp = new TType.TEaseIn();
                Tween tween = tw.create_(getter, setter, null, to, tp, 200.0, Ref<bool>.Null);
            });
            this.cm.__add(ac19, 0, null);


            HlAction ac20 = new(() =>
            {
                Tweenie tw = this.tw;
                HlFunc<object> hlFunc = new(() => { return (object)this.icon.rotation; });
                HlFunc<double> getter = new(() => { return (double)hlFunc.Invoke(); });
                HlAction<object> hlAction = new(_setY =>
                {
                    Icon icon = this.icon;
                    icon.posChanged = true;
                    double rotation = (double)_setY;
                    icon.rotation = rotation;
                });
                HlAction<double> setter = new((al) =>
                {
                    object obj = (object)al;
                    hlAction.Invoke(obj);
                });
                TType tp = new TType.TEaseOut();
                var tween = tw.create_(getter, setter, null, 0.0, tp, 250.0, Ref<bool>.Null);
            });
            this.cm.__add(ac20, 0, null);


            HlAction ac21 = new(() =>
            {
                AnimManager animManager = this.hero.spr.get_anim().play("carryItem".AsHaxeString(), null, null);
                AnimManager animManager2;
                if (animManager.destroyed)
                {
                    animManager2 = animManager.stopOnLastFrame(Ref<bool>.Null);
                    return;
                }

                if (0 >= animManager.stack.length)
                {
                    animManager2 = animManager.stopOnLastFrame(Ref<bool>.Null);
                    return;
                }
                ArrayObj stack = animManager.stack;
                int num = animManager.stack.length - 1;
                int length = stack.length;
                AnimInstance animInstance;
                if (num >= length)
                {
                    animInstance = null!;
                    animInstance.speed = 2.0;
                    animManager2 = animManager.stopOnLastFrame(Ref<bool>.Null);
                    return;
                }
                animInstance = (dynamic)stack.array[num]!;
                animInstance.speed = 2.0;
                animManager2 = animManager.stopOnLastFrame(Ref<bool>.Null);
            });
            this.cm.__add(ac21, 0, null);

            HlAction ac22 = new(() => { });
            this.cm.__add(ac22, 350, null);

            HlAction ac23 = new(() => { Game.Class.ME.curLevel.fx.majorItemShine(this.icon.x, this.icon.y, this.color2); });
            this.cm.__add(ac23, 0, null);

            HlAction ac24 = new(() => { });
            this.cm.__add(ac24, 300, null);

            HlAction ac25 = new(() => { Game.Class.ME.curLevel.fx.majorItemPick(this.icon.x, this.icon.y, this.color2); });
            this.cm.__add(ac25, 0, null);

            HlAction ac26 = new(() =>
            {
                this.hero.setHeadMode(new HeadMode.Electric(this.color2, this.color2), 0.7, null);
            });
            this.cm.__add(ac26, 0, null);

            HlAction ac27 = new(() => { this.icon.set_visible(false); });
            this.cm.__add(ac27, 0, null);

            HlAction ac28 = new(() => { });
            this.cm.__add(ac28, 250, null);

            HlAction ac29 = new(() => { HUD.Class.ME.show(null); });
            this.cm.__add(ac29, 0, null);

            HlAction ac30 = new(() => { this.hideBars(null); });
            this.cm.__add(ac30, 0, null);

            HlAction ac31 = new(() => { });
            this.cm.__add(ac31, 250, null);

            HlAction ac32 = new(() => { this.hero.applyItemPickEffect(this.entity, item); });
            this.cm.__add(ac32, 0, null);

            if (onComplete != null)
            {
                HlAction ac33 = new(() => { onComplete.Invoke(true); });
                this.cm.__add(ac33, 0, null);
            }


            HlAction ac34 = new(() => { });
            this.cm.__add(ac34, 150, null);

            HlAction ac35 = new(() =>
            {
                HlAction<dc.String> action = new(this.cm.signal);
                HlAction cd = new(() =>
                {

                    action.Invoke("anim".AsHaxeString());

                });
                AnimManager animManager = this.hero.spr.get_anim().play("carryItem".AsHaxeString(), null, null).reverse().onEnd(cd);
            });
            this.cm.__add(ac35, 0, null);

            HlAction ac36 = new(() => { });
            this.cm.__add(ac36, 0, "anim".AsHaxeString());


            HlAction ac38 = new(() => { this.hero.setAffectS(5, 0.3, Ref<double>.Null, null); });
            this.cm.__add(ac38, 0, null);

            HlAction ac39 = new(() => { this.destroyed = true; });
            this.cm.__add(ac39, 0, null);
        }


        public void destroyItem(bool fromShrine, Entity e)
        {
            if (fromShrine)
            {
                if (Std.Class.@is(e, UpgradeShrine.Class))
                {
                    ((UpgradeShrine)e).breakIt();
                }
                else
                {
                    if (Std.Class.@is(e, RunicShrine.Class))
                    {
                        ((RunicShrine)e).breakIt();
                    }
                    else
                    {
                        if (Std.Class.@is(e, ItemAltar.Class))
                        {
                            ((ItemAltar)e).disable(true);
                        }
                    }
                }
                return;
            }
            e.destroy();
        }


        public override void onDispose()
        {
            base.onDispose();
            Icon icon = this.icon;
            if (icon == null || icon.parent == null)
            {
                this.icon = null!;
                Game.Class.ME.hero.hasGravity = true;
                return;
            }
            icon.parent.removeChild(icon);
            this.icon = null!;
            Game.Class.ME.hero.hasGravity = true;
        }


        public bool IsShrine(Entity entity)
        {
            return Std.Class.@is(entity, UpgradeShrine.Class) ||
                   Std.Class.@is(entity, ItemAltar.Class) ||
                   Std.Class.@is(entity, RunicShrine.Class);
        }



    }
}