using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.hxd;
using dc.hxd.res;
using dc.level;
using dc.libs._Cooldown;
using dc.pr;
using dc.tool;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics
{
    public class EntranceTeleportationCustom : GameCinematic
    {
        private const int TeleportColor = 16101164;

        public EntranceTeleportationCustom(Hero hero,
            Entity teleporter,
            CPoint t,
            LevelMap map,
            int? linkId)
        {
            HlAction fullFxSequence = CreateFullFxSequence(hero, teleporter, t);
            HlAction subLevelSwitch = CreateSubLevelSwitch(hero, t, map, linkId, fullFxSequence);
            BuildMainQueue(hero, teleporter, map, linkId, fullFxSequence, subLevelSwitch);
        }

        private HlAction CreateFullFxSequence(Hero hero, Entity teleporter, CPoint t)
        {
            return new HlAction(() =>
            {
                cm.__beginNewQueue();

                var batch1 = new HlAction[]
                {
                    new(() => Game.Class.ME.curLevel.fx.electricPillar(
                        (teleporter.cx + teleporter.xr) * 24.0,
                        (teleporter.cy + teleporter.yr) * 24.0,
                        24.0, 60.0, 2071776)),
                    new(() => Game.Class.ME.curLevel.fx.handClap(
                        (hero.cx + hero.xr) * 24.0 + hero.dir * 6,
                        (hero.cy + hero.yr) * 24.0 - 50.0)),
                    new(() => hero.spr.get_anim().suspend()),
                    new(() => Game.Class.ME.curLevel.fx.customMask(59537, 1.0, 0.0, 0.0, 1.0, null)),
                    new(() => hero.onTeleportation()),
                    new(() => Game.Class.ME.curLevel.viewport.track(hero, true)),
                };

                foreach (var a in batch1)
                    cm.__add(a, 0, null);

                cm.__add(new HlAction(() => { }), 100, null);

                cm.__add(new HlAction(() =>
                    Game.Class.ME.curLevel.fx.entityTeleport(
                        (hero.cx + hero.xr) * 24.0,
                        (hero.cy + hero.yr) * 24.0 - hero.hei * 0.5,
                        (t.cx + 0.5) * 24.0,
                        (t.cy + 1) * 24 - hero.hei * 0.5,
                        TeleportColor, Ref<bool>.Null, Ref<bool>.Null, Ref<bool>.Null)), 0, null);

                cm.__add(new HlAction(() =>
                    hero.spr.get_anim().stopWithoutStateAnims("secretPortal".ToHaxeString(), 23)), 0, null);

                cm.__add(new HlAction(() => { }), 150, null);

                var batch2 = new HlAction[]
                {
                    new(() => hero.spr.get_anim().unsuspend()),
                    new(() => hero.spr.get_anim().stopWithStateAnims()),
                };

                foreach (var a in batch2)
                    cm.__add(a, 0, null);

                cm.__add(new HlAction(() => { }), 50, null);
                cm.__add(new HlAction(() => destroyed = true), 0, null);
            });
        }

        private HlAction CreateSubLevelSwitch(Hero hero, CPoint t, LevelMap map, int? linkId, HlAction fullFxSequence)
        {
            return new HlAction(() =>
            {
                Game.Class.ME.activateSubLevel(map, linkId,
                    Ref<bool>.In(true), Ref<bool>.In(false));
                hero.setPosCase(t.cx, t.cy, null, null);
                if (!SettingsMain.ConfigValue.Gameplay.NofadeIn)
                    Main.Class.ME.fadeOut(Ref<double>.In(0.5));
                fullFxSequence.Invoke();
            });
        }

        private HlAction CreateWalkToTeleporter(Hero hero, Entity teleporter)
        {
            return new HlAction(() =>
            {
                double tx = (teleporter.cx + teleporter.xr) * 24.0;
                double hx = (hero.cx + hero.xr) * 24.0;
                int offsetDir = hx < tx ? -1 : 1;
                double px = tx + 15 * offsetDir;
                heroWalkUntil(hero, px, -offsetDir,
                    Ref<double>.Null, new HlAction(() => cm.signal(null)));
            });
        }

        private void BuildMainQueue(Hero hero, Entity teleporter, LevelMap map, int? linkId,
            HlAction fullFxSequence, HlAction subLevelSwitch)
        {
            cm.__beginNewQueue();

            cm.__add(CreateWalkToTeleporter(hero, teleporter), 0, null);
            cm.__add(new HlAction(() => { }), 0, "".ToHaxeString());

            cm.__add(new HlAction(() =>
            {
                var snd = Res.Class.get_loader().loadCache(
                    "sfx/cine/rune_tp.wav".ToHaxeString(), Sound.Class);
                Game.Class.ME.curLevel.lAudio.playEvent((Sound)snd, null, null, null);
            }), 0, null);

            cm.__add(new HlAction(() => { }), 200, null);
            cm.__add(new HlAction(() => Game.Class.ME.curLevel.viewport.stopTracking()), 0, null);

            cm.__add(new HlAction(() =>
            {
                var anim = hero.spr.get_anim().play("secretPortal".ToHaxeString(), null, null);
                if (!anim.destroyed && anim.stack.length > 0)
                    anim.stack.getDyn(anim.stack.length - 1).speed = 1.4;
            }), 0, null);

            cm.__add(new HlAction(() => { }), 500, null);

            if (map == null || linkId == null)
            {
                cm.__add(fullFxSequence, 0, null);
            }
            else
            {
                cm.__add(new HlAction(() =>
                {
                    if (SettingsMain.ConfigValue.Gameplay.NofadeIn)
                        delayer.addF(null, subLevelSwitch, 1);
                    else
                        Main.Class.ME.fadeIn(null, null, Ref<double>.In(0), new HlAction(() =>
                            delayer.addF(null, subLevelSwitch, 1)));
                }), 0, null);
            }
        }

        public void setcd(Entity teleporter)
        {
            cm.__add(new HlAction(() =>
            {
                var cd = teleporter.cd;
                double frames = System.Math.Floor(cd.baseFps * 1000.0) / 1000.0;
                var ci = cd.fastCheck.get(641728512);
                if (ci != null)
                    ci.frames = frames;
                else
                {
                    var nci = new CdInst(641728512, frames);
                    cd.fastCheck.set(641728512, nci);
                    cd.cdList.push(nci);
                }
            }), 0, null);
        }
    }
}
