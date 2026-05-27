using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.en.inter;
using dc.en.inter.npc;
using dc.en.mob.boss;
using dc.hl.types;
using dc.level;
using dc.libs;
using dc.libs.heaps.slib._AnimManager;
using dc.pr;
using dc.tool;
using dc.tool._DecisionHelper;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;

namespace MoreSettings.GameMechanics.cine
{
    public class SummonTailor : GameCinematic
    {
        public Hero owen = default!;
        public Portal boosPortal = default!;
        public TimeKeeper boos = default!;
        public List<Portal> portals = null!;

        public int bastcx = 0;
        public int bastcy = 0;

        public static List<FamilyTailors> familys = new();

        public SummonTailor(Hero hero, int cx, int cy)
        {
            DestroyFamilie();
            (int bastcx, int bastcy) = GenerateOnFlatGround(hero._level, 30);
            this.bastcx = bastcx;
            this.bastcy = bastcy;


            owen = hero;
            cm.__beginNewQueue();
            var actions = new HlAction[]
            {
                new(()=>Game.Class.ME.curLevel.fx.attackAnnounce(owen)),
                new(()=>AudioHelper.LoadAudioFormString("sfx/inter/clock_bell2.wav"))
            };

            foreach (var action in actions)
            {
                cm.__add(action, 0, null);
            }


            actions = new HlAction[]
            {
                new(() =>
                {
                    Fx fx = Game.Class.ME.curLevel.fx;
                    int x =bastcx + 8;
                    boosPortal = CreatePortal(hero._level, owen,x,bastcy);
                    fx.portalStart((boosPortal.cx + boosPortal.xr) * 24,
                    ((int)(boosPortal.cy + boosPortal.yr)* 24) - boosPortal.hei* 0.5,
                    8450041, 16301458);
                    AudioHelper.LoadAudioFormString("sfx/cine/rtc_assassin_portal_open.wav");
                }),
                new(()=>create_TimeKeeper(owen._level)),
            };

            foreach (var action in actions)
            {
                cm.__add(action, 1500, null);
            }

            cm.__add(() => TimeKeeper_Say(), 1000, null);

            cm.__add(() => { }, 2000, null);

            cm.__add(new(() =>
            {
                boos.say("没有头的家伙,你还挺臭美,不用谢ovo-->".ToHaxeString(), null, null, null);

                var anim = boos.spr.get_anim();
                anim.play("show".ToHaxeString(), null, null);
                if (anim.stack.length > 0)
                {
                    var instance = anim.stack.getDyn(anim.stack.length - 1) as AnimInstance;
                    if (instance != null)
                        instance.speed = 0.5;
                }

                portals.ForEach(p =>
                {
                    AudioHelper.LoadAudioFormString("sfx/cine/rtc_assassin_portal_open.wav");
                    owen._level.fx.portalStart((int)((p.cx + p.xr) * 24.0),
                        ((int)((p.cy + p.yr) * 24.0)) - p.hei * 0.5, 8450041, 16301458);
                });
            }), 0, null);

            cm.__add((() => { }), 1000, null);

            cm.__add(() => portals.ForEach(p =>
            {
                var f = familys[0];
                f.Tailor.visible = f.Tailor.daughter.visible = f.Skinner.visible = true;
                p.show();
            }), 0, null);

            cm.__add((() => { }), 2000, null);

            cm.__add(new(() => portals.ForEach(p =>
            {
                p.close();
                owen._level.fx.portalEnd((int)((p.cx + p.xr) * 24.0),
            ((int)((p.cy + p.yr) * 24.0)) * 0.5, 8450041);
            })), 0, null);

            cm.__add((() => { }), 1000, null);

            cm.__add(() =>
            {
                boos.destroy();
                boosPortal.close();
                boos.destroyed = true;
                owen._level.viewport.track(owen, false);
            }, 0, null);

            cm.__add((() => { }), 1000, null);

            cm.__add(() =>
            {
                hero._level.fx.portalEnd((int)((boosPortal.cx + boosPortal.xr) * 24.0),
                ((int)((boosPortal.cy + boosPortal.yr) * 24.0)) * 0.5, 8450041);
                destroyed = true;
            }, 0, null);
        }


        public void create_TimeKeeper(Level level)
        {
            boosPortal.show();
            boos = new TimeKeeper(level, bastcx + 8, bastcy, 0, 0);
            if (owen.cx > boos.cx)
                boos.dir = 1;
            else
                boos.dir = -1;
            boos.ready = false;
            boos.lockAiS(9999);
            boos.init();

            cm.__addParallel(() =>
            {
                level.viewport.track(boosPortal, false);
                boos.spr.get_anim().play("cast".ToHaxeString(), null, null).reverse();
            }, 0, null);
        }

        public void TimeKeeper_Say()
        {
            boos.say("ciallo!".ToHaxeString(), null, null, null);
            cm.__addParallel(() => create_Tailor(owen._level, owen._level.map.getRoomAt(owen.cx, owen.cy)), 300, null);
        }

        public void create_Tailor(Level level, Room room)
        {


            boos.dir = -1;
            int cx = bastcx, cy = bastcy;
            const int zero = 0;

            var tailor = new Tailor(level, room);
            tailor.visible = false;
            tailor.daughter.visible = false;
            tailor.init();
            tailor.cx = zero;
            tailor.cy = zero;
            tailor.daughter.cx = zero;
            tailor.daughter.cy = zero;

            var skinner = new Skinner(level, zero, zero);
            skinner.visible = false;
            skinner.init();

            tailor.cx = cx - 4;
            tailor.cy = cy;
            tailor.daughter.cx = cx;
            tailor.daughter.cy = cy;
            skinner.cx = cx + 4;
            skinner.cy = cy;
            skinner.setPosPixel(
                (int)((skinner.cx + skinner.xr) * 24.0 + 7.0),
                (int)((skinner.cy + skinner.yr) * 24.0)
            );

            var p = CreatePortal(level, skinner, skinner.cx, skinner.cy);
            p.setPosPixel(
                (int)((skinner.cx + skinner.xr) * 24.0),
                (int)((skinner.cy + skinner.yr) * 24.0 + 10.0)
            );
            portals = new List<Portal>
            {
                CreatePortal(level, tailor,tailor.cx,tailor.cy),
                CreatePortal(level, tailor.daughter,tailor.daughter.cx,tailor.daughter.cy),
            };
            portals.Add(p);

            cm.__addParallel(() => level.viewport.track(tailor.daughter, false), 2000, null);

            familys.Add(new FamilyTailors { Tailor = tailor, Skinner = skinner });
        }

        private Portal CreatePortal(Level level, Entity e, int cx, int cy)
        {
            var portal = new Portal(level, cx, cy, null, Ref<bool>.In(false));
            portal.init();
            return portal;
        }

        public void DestroyFamilie()
        {
            foreach (var family in familys)
            {
                if (family.Skinner == null) break;

                family.Tailor?.destroy();
                family.Tailor?.daughter?.destroy();
                family.Skinner?.destroy();
            }
            familys.Clear();
        }

        public static (int, int) GenerateOnFlatGround(Level level, int minWidth, int radius = 10)
        {
            // 半径检查要求：平台宽度至少为 2*radius + 1
            int requiredWidth = 2 * radius + 1;
            if (minWidth < requiredWidth) minWidth = requiredWidth;

            List<CPoint> candidates = new List<CPoint>();
            foreach (Platform platform in level.map.platforms)
            {
                // 平台总宽度必须足够容纳左右各 radius 格 + 自身 1 格
                if (platform.wid < requiredWidth) continue;

                // 有效 x 的范围：[left + radius, left + wid - 1 - radius]
                int startX = platform.left + radius;
                int endX = platform.left + platform.wid - 1 - radius;
                for (int x = startX; x <= endX; x++)
                {
                    // 检查是否被占用
                    if (platform.occupations != null && platform.occupations.exists(x - platform.left)) continue;

                    // 检查 fastSpots 禁止生成标记（例如第25位）
                    int cellIndex = x + platform.walkY * level.map.wid;
                    SpotFlags flags = level.map.fastSpots.getDyn(cellIndex);
                    if ((flags.low & (1 << 25)) != 0) continue;

                    candidates.Add(new CPoint(x, platform.walkY));
                }
            }

            if (candidates.Count == 0) return (0, 0);

            Hero hero = Game.Class.ME.hero;
            int heroX = hero.cx;
            int heroY = hero.cy;
            Rand rng = new Rand(level.map.seed);

            CPoint bestPoint = null!;
            double bestScore = double.MinValue; // 注意改为最小值，因为我们找最高分

            foreach (CPoint pt in candidates)
            {
                double dx = pt.cx - heroX;
                double dy = pt.cy - heroY;
                double dist = System.Math.Sqrt(dx * dx + dy * dy);
                // 距离越近分数越高（取负距离），再加上随机扰动
                double score = -dist + rng.rand() * 3.0;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = pt;
                }
            }

            if (bestPoint != null)
                return (bestPoint.cx, bestPoint.cy);
            return (0, 0);
        }
    }

    public class FamilyTailors
    {
        public Tailor Tailor = null!;
        public Skinner Skinner = null!;
    }
}