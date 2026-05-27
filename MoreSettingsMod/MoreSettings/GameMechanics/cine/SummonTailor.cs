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
using dc.tool._Cooldown;
using dc.tool._DecisionHelper;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;

namespace MoreSettings.GameMechanics.cine
{
    public class SummonTailor : GameCinematic
    {
        private const int PORTAL_COLOR_LOW = 8450041;
        private const int PORTAL_COLOR_HIGH = 16301458;

        public Hero owen = default!;
        public Portal bossPortal = default!;
        public TimeKeeper boss = default!;
        public List<Portal> portals = null!;
        public int spawnCx = 0;
        public int spawnCy = 0;
        public int cdtimeid = 1145140;
        public static List<FamilyTailors> familys = new();

        public SummonTailor(Hero hero, int cx, int cy)
        {
            (spawnCx, spawnCy) = GenerateOnFlatGround(hero._level, 30);
            owen = hero;

            var cd = owen.cd;
            double duration = 30;
            double frames = duration * cd.baseFps;
            var cdInst = new dc.libs._Cooldown.CdInst(cdtimeid, frames);
            cd.fastCheck.set(cdtimeid, cdInst);
            cd.cdList.push(cdInst);
            cdInst.cb = new HlAction(() =>
            {
                cd.fastCheck.remove(cdtimeid);
                cd.cdList.removeDyn(cdInst);
            });

            cm.__beginNewQueue();

            AddActions(0,
                () => DestroyFamilie(),
                () => Game.Class.ME.curLevel.fx.attackAnnounce(owen),
                () => AudioHelper.LoadAudioFormString("sfx/inter/clock_bell2.wav"));

            AddActions(1500,
                OpenBossPortal,
                () => CreateTimeKeeper(owen._level));

            cm.__add(TimeKeeperSay, 1000, null);
            Wait(2000);

            cm.__add(BossTauntAndShowFamily, 0, null);
            Wait(1000);

            cm.__add(ShowFamily, 0, null);
            Wait(2000);

            cm.__add(CloseFamilyPortals, 0, null);
            Wait(1000);

            cm.__add(DestroyBoss, 0, null);
            Wait(1000);

            cm.__add(FinalizeCinematic, 0, null);
        }


        private void OpenBossPortal()
        {
            Fx fx = Game.Class.ME.curLevel.fx;
            int portalCx = spawnCx + 8;
            bossPortal = CreatePortal(owen._level, owen, portalCx, spawnCy);
            StartPortalFx(bossPortal);
            AudioHelper.LoadAudioFormString("sfx/cine/rtc_assassin_portal_open.wav");
        }

        private void BossTauntAndShowFamily()
        {
            boss.say("没有头的家伙,你还挺臭美,不用谢ovo-->".ToHaxeString(), null, null, null);

            var anim = boss.spr.get_anim();
            anim.play("show".ToHaxeString(), null, null);
            if (anim.stack.length > 0)
            {
                var instance = anim.stack.getDyn(anim.stack.length - 1) as AnimInstance;
                if (instance != null)
                    instance.speed = 0.5;
            }

            foreach (var p in portals)
            {
                AudioHelper.LoadAudioFormString("sfx/cine/rtc_assassin_portal_open.wav");
                StartPortalFx(p);
            }
        }

        private void ShowFamily()
        {
            foreach (var p in portals)
            {
                var f = familys[0];
                f.Tailor.visible = f.Tailor.daughter.visible = f.Skinner.visible = true;
                p.show();
            }
        }

        private void CloseFamilyPortals()
        {
            foreach (var p in portals)
            {
                p.close();
                EndPortalFx(p);
            }
        }

        private void DestroyBoss()
        {
            boss.destroy();
            bossPortal.close();
            boss.destroyed = true;
            owen._level.viewport.track(owen, false);
        }

        private void FinalizeCinematic()
        {
            owen._level.fx.portalEnd(
                PixelX(bossPortal), PixelY(bossPortal) * 0.5, PORTAL_COLOR_LOW);
            destroy();
        }


        public void CreateTimeKeeper(Level level)
        {
            bossPortal.show();
            boss = new TimeKeeper(level, spawnCx + 8, spawnCy, 0, 0);
            boss.dir = owen.cx > boss.cx ? 1 : -1;
            boss.ready = false;
            boss.lockAiS(9999);
            boss.init();

            cm.__addParallel(() =>
            {
                level.viewport.track(bossPortal, false);
                boss.spr.get_anim().play("cast".ToHaxeString(), null, null).reverse();
            }, 0, null);
        }

        public void TimeKeeperSay()
        {
            boss.say("ciallo!".ToHaxeString(), null, null, null);
            cm.__addParallel(
                () => CreateTailor(owen._level, owen._level.map.getRoomAt(owen.cx, owen.cy)),
                300, null);
        }

        public void CreateTailor(Level level, Room room)
        {
            boss.dir = -1;
            int cx = spawnCx, cy = spawnCy;
            const int zero = 0;


            var tailor = new Tailor(level, room);
            tailor.visible = false;
            tailor.daughter.visible = false;
            tailor.init();
            tailor.cx = tailor.cy = zero;
            tailor.daughter.cx = tailor.daughter.cy = zero;


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
                (int)((skinner.cy + skinner.yr) * 24.0));


            var skinnerPortal = CreatePortal(level, skinner, skinner.cx, skinner.cy);
            skinnerPortal.setPosPixel(
                (int)((skinner.cx + skinner.xr) * 24.0),
                (int)((skinner.cy + skinner.yr) * 24.0 + 10.0));

            portals = new List<Portal>
            {
                CreatePortal(level, tailor,          tailor.cx,          tailor.cy),
                CreatePortal(level, tailor.daughter, tailor.daughter.cx, tailor.daughter.cy),
                skinnerPortal,
            };

            cm.__addParallel(() => level.viewport.track(tailor.daughter, false), 2000, null);

            familys.Add(new FamilyTailors { Tailor = tailor, Skinner = skinner });
        }

        #region helper

        private Portal CreatePortal(Level level, Entity e, int cx, int cy)
        {
            var portal = new Portal(level, cx, cy, null, Ref<bool>.In(false));
            portal.init();
            return portal;
        }

        private void StartPortalFx(Portal portal)
        {
            owen._level.fx.portalStart(
                PixelX(portal),
                PixelY(portal) - portal.hei * 0.5,
                PORTAL_COLOR_LOW, PORTAL_COLOR_HIGH);
        }

        private void EndPortalFx(Portal portal)
        {
            owen._level.fx.portalEnd(
                PixelX(portal), PixelY(portal) * 0.5, PORTAL_COLOR_LOW);
        }

        private static int PixelX(Portal p) => (int)((p.cx + p.xr) * 24.0);
        private static int PixelY(Portal p) => (int)((p.cy + p.yr) * 24.0);


        private void AddActions(int delayMs, params HlAction[] actions)
        {
            foreach (var action in actions)
                cm.__add(action, delayMs, null);
        }


        private void Wait(int delayMs) => cm.__add(() => { }, delayMs, null);

        public void DestroyFamilie()
        {
            ArrayObj ens = owen._level.entities;
            foreach (var item in ens)
            {
                if (item is Skinner && item != null)
                    Beautifuldestroy(item!);
                if (item is Tailor && item != null)
                    Beautifuldestroy(item!);
                if (item is TailorDaughter && item != null)
                    Beautifuldestroy(item!);
            }
            familys.Clear();

            void Beautifuldestroy(Entity entity)
            {
                var p = CreatePortal(owen._level, entity, entity.cx, entity.cy);
                p.show();
                cm.__addParallel(() => { p.close(); entity.destroy(); }, 500, null);
            }
        }


        public static (int, int) GenerateOnFlatGround(Level level, int minWidth, int radius = 10)
        {
            int requiredWidth = 2 * radius + 1;
            if (minWidth < requiredWidth) minWidth = requiredWidth;

            var candidates = new List<CPoint>();
            foreach (Platform platform in level.map.platforms)
            {
                if (platform.wid < requiredWidth) continue;

                int startX = platform.left + radius;
                int endX = platform.left + platform.wid - 1 - radius;

                for (int x = startX; x <= endX; x++)
                {
                    if (platform.occupations != null && platform.occupations.exists(x - platform.left))
                        continue;

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
            var rng = new Rand(level.map.seed);

            CPoint bestPoint = null!;
            double bestScore = double.MinValue;

            foreach (CPoint pt in candidates)
            {
                double dx = pt.cx - heroX;
                double dy = pt.cy - heroY;
                double dist = System.Math.Sqrt(dx * dx + dy * dy);
                double score = -dist + rng.rand() * 3.0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = pt;
                }
            }

            return bestPoint != null ? (bestPoint.cx, bestPoint.cy) : (0, 0);
        }
    }

    public class FamilyTailors
    {
        public Tailor Tailor = null!;
        public Skinner Skinner = null!;
    }

    #endregion
}
