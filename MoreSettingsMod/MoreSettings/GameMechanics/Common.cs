using System.Reflection;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.hero;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.libs.heaps.slib;
using dc.libs.heaps.slib._AnimManager;
using dc.tool;
using dc.tool._Cooldown;
using dc.ui;
using Hashlink.Proxy;
using Hashlink.Reflection.Types;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace MoreSettings.GameMechanics
{
    public static class Common
    {
        const int inDeepWater = 129 << 21;
        const int blink = 722 << 21;
        const int blinkPhase = 748 << 21;
        const int newSubscriber = 394 << 21;
        const int subscriberFx = 750 << 21;
        const int subscriberOnion = 751 << 21;
        const int hDrip = 384 << 21;
        const int hDripTick = 749 << 21;
        const int FX = 202 << 21;
        const int oilExp = (3 << 21) | 315619;

        static Random _rng = null!;
        static BlendMode _blendAdd = null!;
        static BlendMode _blendNone = null!;
        static GameplayMod _forcedDarkness = null!;
        private static readonly List<double> _darknessDrs = new(4);
        public static void Initialize()
        {
            //Hook_Hero.postUpdate += Hook_Hero_postUpdate;
            // Hook_Entity.postUpdate += Hook_Entity_postUpdate;
            // _rng = new();
            // _blendAdd = new BlendMode.Add();
            // _blendNone = new BlendMode.None();
            // _forcedDarkness = new GameplayMod.ForcedDarkness();
        }


        private static double GetAffVal(int id, double val, ArrayObj array)
        {
            var aff = array.getDyn(id);
            return aff != null && aff!.length > 0 ? val : 0.0;
        }


        private static void Hook_Entity_postUpdate(Hook_Entity.orig_postUpdate orig, Entity self)
        {
            EntityPostupdate(self);
        }

        public static void EntityPostupdate(Entity h)
        {
            bool hasSpr = h.spr != null;
            if (hasSpr) h.spriteUpdate();

            if (h.life > 0 && !h.destroyed)
                h.postUpdateIcons();

            if (h.lifeBar != null)
            {
                var lb = h.lifeBar;
                bool showBar = h.life > 0 && h.life < h.maxLife
                    && !dc.ui.Console.Class.ME.flags.exists(dc.ui.Console.Class.HIDE_UI);
                lb.visible = showBar;
                lb.outline.visible = showBar;
                lb.lastMax.visible = showBar;
                lb.bar.visible = showBar;
                lb.bg.visible = showBar;
                for (int i = 0; i < lb.lines.length; i++)
                    ((HSpriteBE)lb.lines.getDyn(i)).visible = showBar;

                double lifeRatio = h.life / lb.max;
                if (lifeRatio < 0) lifeRatio = 0; else if (lifeRatio > 1) lifeRatio = 1;

                if (lb.lastV != h.life)
                {
                    lb.lastPause = 12.0;
                    double sx = lb.bar.scaleX;
                    if (lb.lastMax.scaleX < sx) lb.lastMax.scaleX = sx;
                }
                lb.lastV = h.life;

                double barW = (double)(lb.wid - lb.padding * 2);
                lb.bar.scaleX = lifeRatio * barW / lb.bar.t.width;
                double barH = (double)(lb.hei - lb.padding * 2) / lb.bar.t.height;
                lb.bar.scaleY = barH;
                lb.lastMax.scaleY = barH;

                bool above = h.lifeBarAbove;
                double sprY = h.spr!.y;
                double lbX = (int)(h.spr.x - lb.wid * 0.5);
                double lbY = (int)(above ? sprY - h.hei - 10.0 : sprY + 2.0);
                lb.x = lbX; lb.y = lbY;
                double padL = lbX + lb.padding;
                double padT = lbY + lb.padding;
                lb.outline.x = lbX; lb.outline.y = lbY;
                lb.bg.x = padL; lb.bg.y = padT;
                lb.bar.x = padL; lb.bar.y = padT;
                lb.lastMax.x = padL; lb.lastMax.y = padT;

                double space = (double)(lb.wid - lb.padding * 2) / (lb.lines.length + 1);
                int lc2 = lb.lines.length;
                for (int i = 0, n = 1; i < lc2; i++, n++)
                {
                    var line = (HSpriteBE)lb.lines.getDyn(i);
                    line.x = padL + space * n;
                    line.y = lbY;
                }

                lb.update(h.get_tmod());
            }

            if (hasSpr)
            {
                TryJitter(h, 23, 0.5);
                TryJitter(h, 46, 0.5);
            }

            TryAffectFx(h, 48, 211812352, 0.1, () => h._level.fx.invisibility(h));
            TryAffectFx(h, 28, 213909504, 0.06, h.globalShieldFx);
            TryAffectFxRandom(h, 79, 216006656, 0.3, () => h._level.fx.necromancySparkle(h));

            if (h.gameplayLabel != null)
            {
                var gl = h.gameplayLabel;
                gl.x = (int)h.get_globalUiX();
                gl.y = (int)(h.get_globalUiY() - (h.hei + 25.0) * h._level.scroller.scaleX);
                gl.posChanged = true;
                gl.set_visible(!dc.ui.Console.Class.ME.flags.exists(dc.ui.Console.Class.HIDE_UI) && h.visible);
                gl.alpha = h.sprAlpha;
            }

            double tmod = h.get_tmod();
            int ic = h.icons.length;
            for (int i = 0; i < ic; i++)
                ((StatusIcon)h.icons.getDyn(i)).update(tmod);
        }

        private static void TryJitter(Entity h, int affId, double threshold)
        {
            var arr = h.affects;
            var aff = arr.length > affId ? (ArrayObj)arr.getDyn(affId) : null;
            if (aff == null || aff.length == 0) return;
            if (h.getHighestAffectDurationS(affId) <= threshold)
            {
                h.spr.x += Std.Class.random(2) * (Std.Class.random(2) * 2 - 1);
                h.spr.posChanged = true;
            }
        }

        private static bool TryMicroCd(dc.tool.Cooldown cd, int key, double frames)
        {
            frames = System.Math.Floor(frames * 1000.0) / 1000.0;

            if (cd.fastCheck.exists(key))
            {
                return true;
            }
            else
            {
                var inst = cd.fastCheck.get(key);
                if (inst != null)
                {
                    inst.frames = frames;
                }
                else
                {
                    inst = new CdInst(key, frames);
                    cd.fastCheck.set(key, inst);
                    cd.cdList.push(inst);
                }
                return false;
            }
        }

        private static void TryAffectFx(Entity h, int affId, int cdKey, double fpsMul, Action fx)
        {
            var arr = h.affects;
            var aff = arr.length > affId ? (ArrayObj)arr.getDyn(affId) : null;
            if (aff == null || aff.length == 0) return;
            if (!TryMicroCd(h.cd, cdKey, fpsMul * h.cd.baseFps)) fx();
        }

        private static void TryAffectFxRandom(Entity h, int affId, int cdKey, double baseSec, Action fx)
        {
            var arr = h.affects;
            var aff = arr.length > affId ? (ArrayObj)arr.getDyn(affId) : null;
            if (aff == null || aff.length == 0) return;
            if (!TryMicroCd(h.cd, cdKey, (baseSec + _rng.NextDouble() * 0.3) * h.cd.baseFps)) fx();
        }

        private static bool TryCdTick(dc.tool.Cooldown cd, int key, double frames)
        {
            if (cd.fastCheck.exists(key)) return true;
            double cmp = System.Math.Floor(frames * 1000.0) / 1000.0;
            var inst = cd.fastCheck.get(key);
            if (inst == null)
            {
                inst = new CdInst(key, cmp);
                cd.fastCheck.set(key, inst);
                cd.cdList.push(inst);
            }
            else inst.frames = cmp;
            return false;
        }

        #region HeroUpdate



        private static void Hook_Hero_postUpdate(Hook_Hero.orig_postUpdate orig, Hero self)
        {
            if (!self._level.game.hasCinematic())
                self.heroHead.cineHeadMode = null;

            if (!self.awake)
                return;

            var anim = self.spr.get_anim();
            var affects = self.affects;

            // Run speed
            var runAnims = self.runAnims;
            for (int i = 0; i < runAnims.length; i++)
            {
                double speed = 1.0;
                double comboFactor = self.spdComboKills > self.get_infos().props.speedComboMaxMobs ? 1.0
                    : self.spdComboKills > self.get_infos().props.speedComboMaxMobs * 0.5 ? 0.5 : 0.0;
                speed += self.speedComboRun * comboFactor;
                speed += GetAffVal(116, self.getHighestAffectValue(116), affects);
                speed += GetAffVal(72, self.runSpeedOnTrap, affects);
                speed += GetAffVal(69, 0.4, affects);
                speed += GetAffVal(71, 0.2, affects);
                speed += GetAffVal(70, 0.2, affects);
                speed += self.activeSkillsManager.getRunSpeedMul();
                anim.setStateAnimSpeed(runAnims.getDyn(i), speed);
            }

            var ct = (HashlinkObjectType)self.HashlinkObj.Type;
            ct.Super!.Super!.FindProto("postUpdate")!.Function.DynamicInvoke(self);

            // Deep water walk speed
            bool isRunAnim = self.isPlayingRunAnim();
            if (self.cd.fastCheck.exists(inDeepWater))
            {
                if (!isRunAnim)
                {
                    var stack = anim.stack;
                    if (!anim.destroyed && stack.length > 0)
                        isRunAnim = ((AnimInstance)stack.getDyn(0)).group.ToString() == "walk";
                }
                if (isRunAnim && !anim.destroyed && anim.stack.length > 0)
                {
                    var stack = anim.stack;
                    ((AnimInstance)stack.getDyn(stack.length - 1)).speed = 0.6;
                }
            }

            // Wall run x offset
            if (!anim.destroyed && anim.stack.length > 0)
            {
                if (((AnimInstance)anim.stack.getDyn(0)).group.ToString() == "wallRun")
                {
                    self.spr.x += self.dir * 3;
                    self.spr.posChanged = true;
                }
            }

            // Alpha / BlendMode / Scarf
            double targetAlpha = self.sprAlpha;
            bool isInvis = false;
            var invisAffect = (ArrayObj)affects.getDyn(48);
            if (invisAffect != null && invisAffect.length > 0)
            {
                targetAlpha *= 0.5;
                isInvis = true;
            }

            if (self.cd.fastCheck.exists(blink))
            {
                if (!TryMicroCd(self.cd, blinkPhase, 2.0))
                    self.spr.alpha = self.spr.alpha == targetAlpha ? targetAlpha * 0.3 : targetAlpha;
            }
            else self.spr.alpha = targetAlpha;

            if (isInvis)
            {
                if (self.spr.blendMode is not BlendMode.Add) self.spr.blendMode = _blendAdd;
                self.scarf.overrideBlendMode(_blendAdd);
            }
            else
            {
                if (self.spr.blendMode is BlendMode.Add) self.spr.blendMode = _blendNone;
                self.scarf.restoreBlendMode();
            }

            // Affect 23: stop anims
            if (!self.controlsLocked(default))
            {
                if (anim.destroyed || anim.stack.length == 0)
                {
                    var affect23 = (ArrayObj)affects.getDyn(23);
                    if (affect23 != null && affect23.length == 0)
                        anim.stopWithStateAnims();
                }
            }

            // Homonculus head drip
            if (self.cd.fastCheck.exists(hDrip) && !TryMicroCd(self.cd, hDripTick, 0.03 * self.cd.baseFps))
            {
                int c = self.homonculusMainColor;
                double r = ((c >> 16) & 255) / 255.0, g = ((c >> 8) & 255) / 255.0, b = (c & 255) / 255.0;
                double max = r > g ? (r > b ? r : b) : (g > b ? g : b);
                double min = r < g ? (r < b ? r : b) : (g < b ? g : b);
                double delta = max - min;
                double hue = 0;
                if (delta > 0)
                {
                    if (r == max) hue = ((g - b) / delta + 6) % 6;
                    else if (g == max) hue = (b - r) / delta + 2;
                    else hue = (r - g) / delta + 4;
                }
                hue /= 6;
                self._level.fx.homunculusHeadDrip(self, HslToRgb(hue, 0.4, 0.4), self.cd._getRatio(hDrip));
            }

            self.tailUpdate?.Invoke();
            self.heroHead.postUpdate();

            // Affect 44: super power
            var affect44 = (ArrayObj)affects.getDyn(44);
            if (affect44 != null && affect44.length > 0 && !TryMicroCd(self.cd, FX, 0.06 * self.cd.baseFps))
            {
                int spCount = 3;
                self._level.fx.superPower(self, oilExp, Ref<int>.In(spCount));
            }

            self.updateLifeBar();
            self.mainSkillsManager.postUpdate();
            self.activeSkillsManager.postUpdate();
            self.weaponsManager.postUpdate();

            // Darkness system
            var lmap = self._level?.map;
            var fp = lmap?.infos.flagsProps;
            bool needDarkness = lmap != null && (fp!.gameplayFlags & (1 << 3)) != 0;
            if (!needDarkness)
            {
                if (dc.pr.Game.Class.ME != null && dc.pr.Game.Class.ME.hasGameplayMod(_forcedDarkness)
                    && lmap!.infos.group != 3 && lmap.infos.group != 1)
                    needDarkness = true;
            }

            if (!needDarkness)
            {
                if (self.darknessDeferred != null)
                {
                    if (self.darknessDeferred.parent != null)
                        self.darknessDeferred.parent.removeChild(self.darknessDeferred);
                    self.darknessDeferred = null;
                    self.darknessHoles = null;
                    _darknessDrs.Clear();
                }
            }
            else if (self.darknessDeferred == null)
            {
                self.darknessDeferred = new dc.light.DarknessRemover(null);
                self._level!.scroller.addChildAt(self.darknessDeferred, Const.Class.DP_FOREGROUND);
                self.darknessDeferred.x = self.get_headX();
                self.darknessDeferred.y = self.get_headY();
                self.darknessHoles = (ArrayObj)ArrayUtils.CreateDyn().array;
                _darknessDrs.Clear();
                for (int j = 0; j < 4; j++)
                {
                    var hs = new HSprite(Assets.Class.fx, "fxDarknessHalo".ToHaxeString(), Ref<int>.In(0), null);
                    hs.pivot.centerFactorX = 0.5; hs.pivot.centerFactorY = 0.5;
                    hs.pivot.usingFactor = true; hs.pivot.isUndefined = false;
                    hs.rotation = _rng.NextDouble() * 6.28;
                    self.darknessDeferred.addChild(hs);
                    self.darknessHoles.push(hs);
                    double dr = (0.0005 + _rng.NextDouble() * 0.0005) * (j + 1);
                    _darknessDrs.Add((j % 2 == 0) ? -dr : dr);
                }
            }

            if (self.darknessDeferred != null)
            {
                self.darknessDeferred.x += (self.get_headX() - self.darknessDeferred.x) * 0.18;
                self.darknessDeferred.y += (self.get_headY() - self.darknessDeferred.y) * 0.18;
                double dkRatio = self.darknessCounter / self.get_darknessCounterMax();
                if (dkRatio < 0) dkRatio = 0; else if (dkRatio > 1) dkRatio = 1;
                double visibility = 1.0 - dkRatio;
                double ftime = dc.pr.Game.Class.ME!.ftime;
                double tmod = self.get_tmod();
                for (int i = 0; i < self.darknessHoles!.length; i++)
                {
                    HSprite s = (HSprite)self.darknessHoles.getDyn(i);
                    s.x = System.Math.Cos(ftime * 0.02 + i * 1.5) * 8.0;
                    s.y = System.Math.Sin(ftime * 0.011 + i * 1.5) * 7.0;
                    s.rotation += _darknessDrs[i] * tmod;
                    if (visibility <= 0) { s.set_visible(false); continue; }
                    s.set_visible(true);
                    s.alpha = 0.1 + 0.5 * visibility;
                    double sc = (312.0 / s.rawTile.width) * (0.15 + 0.85 * visibility) * (1.0 - i * 0.2);
                    s.scaleX = sc; s.scaleY = sc;
                }
            }

            // Footstep sounds
            ArrayBytes_Int footFrames = null!;
            if (!anim.destroyed && anim.stack.length > 0)
            {
                string ag = ((AnimInstance)anim.stack.getDyn(0)).group.ToString();
                footFrames = ag switch
                {
                    "run" => self.footStepFramesRun,
                    "runB" => self.footStepFramesRunFast,
                    "runBInjured" => self.footStepFramesRunFastInjured,
                    "runDance" => self.footStepFramesRunDance,
                    "runInjured" => self.footStepFramesRunInjured,
                    "walk" => self.footStepFramesWalk,
                    _ => null!
                };
            }

            if (footFrames != null)
            {
                int curFrame = !anim.destroyed && anim.stack.length > 0
                    ? ((AnimInstance)anim.stack.getDyn(0)).animCursor
                    : anim.spr.frame == anim.spr.totalFrames.Invoke() - 1
                        ? anim.spr.totalFrames.Invoke() - 1 : 0;

                if (curFrame != self.lastFootStepFrame)
                {
                    if (footFrames.indexOf(curFrame, null) >= 0 || self.lastFootStepFrame == -1)
                    {
                        self.playStepSound();
                        self.get_fxRunSmoke().Invoke((self.cx + self.xr) * 24.0 + self.dir * 5,
                            (self.cy + self.yr) * 24.0, self.dir, 12629667);
                    }
                    self.lastFootStepFrame = curFrame;
                }
            }
            else self.lastFootStepFrame = -1;

            self.scarf?.postUpdate();

            // Flame trail + Onion skin
            if (self.cd.fastCheck.exists(newSubscriber))
            {
                if (!TryMicroCd(self.cd, subscriberFx, (0.4 + _rng.NextDouble() * 0.2) * self.cd.baseFps))
                {
                    self._level!.fx.firework(
                        (self.cx + self.xr) * 24.0,
                        (self.cy + self.yr) * 24.0 - self.hei * 0.5,
                        HslToRgb(_rng.NextDouble(), 0.4 + _rng.NextDouble() * 0.2, 1.0),
                        Std.Class.random(100) < 15 ? 2 + Std.Class.random(2) : 1);
                }

                if (System.Math.Abs(self.dx + self.bdx) >= 0.1 || System.Math.Abs(self.dy + self.bdy) >= 0.1)
                {
                    if (!TryMicroCd(self.cd, subscriberOnion, 0.06 * self.cd.baseFps))
                    {
                        double a = 0.4;
                        OnionSkin.Class.fromEntity(self, null,
                            HslToRgb(dc.pr.Game.Class.ME!.ftime * 0.01 % 1.0, 1.0, 1.0),
                            Ref<double>.In(a), Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
                    }
                }
            }

            if (self.curseLabel != null)
                self.curseLabel.offY = -20.0 * self._level!.scroller.scaleY;

            if (self.assistMapRevealed) return;
            if (!Main.Class.ME.options.assistMode.revealMap) return;
            HUD.Class.ME.minimap.revealAll();
            self.assistMapRevealed = true;
        }
        #endregion

        private static int HslToRgb(double h, double s, double l)
        {
            if (s == 0) { int v = (int)(l * 255); return (v << 16) | (v << 8) | v; }
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;
            return ((int)(HueToRgb(p, q, h + 1.0 / 3) * 255) << 16)
                 | ((int)(HueToRgb(p, q, h) * 255) << 8)
                 | (int)(HueToRgb(p, q, h - 1.0 / 3) * 255);
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1; if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }
    }
}