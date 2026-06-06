using System.Runtime.CompilerServices;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.h2d;
using dc.shader;
using dc.tool._Cooldown;
using dc.tool.atk;
using dc.tool.weap;
using dc.ui;
using dc.ui.popd;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Storage;
using ModCore.Utilities;
using MoreSettings.API;
using PopDamage.OtherPop;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class EntityPopDamage
    {

        public static Config<PopConfig> Config = new("CustomPopDamage");
        public static PopConfig popconfig = null!;
        public static IPopDamage handler = null!;
        public static IPopDamage ForcedHandler = null!;

        public static readonly HashSet<string> StsItems = new()
        {
            "DiverseDeckJuggernaut",
            "DiverseDeckCatalyst",
            "DiverseDeckElectro",
            "DiverseDeckWatcher"
        };
        public static readonly HashSet<string> HotlineSkins = new()
        {
            "HotlineMiamiChicken",
            "HotlineMiamiHorse",
            "HotlineMiamiOwl"
        };

        public EntityPopDamage()
        {
            popconfig = Config.Value;

            PopDamageHandlerRegistry.Register(new HotlinePopDamageHandler());
            PopDamageHandlerRegistry.Register(new StsPopDamageHandler());
            PopDamageHandlerRegistry.Register(new DefaultPopDamageHandler());
            PopDamageHandlerRegistry.Register(new GradientPop());

            Hook_Entity.popDamage += Hook_Entity_PopDamage;
            Hook__PopDamage.__constructor__ += Hook_PopDamage_Initialize;
            Hook__PopDamageHotline.__constructor__ += Hook_PopDamageHotline_Initialize;
        }

        #region Hooks
        private void Hook_Entity_PopDamage(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
        {
            if (dc.ui.Console.Class.ME.flags.exists("NoPopText".ToHaxeString())) return;

            if (self is Hero)
            {
                handler = new DefaultPopDamageHandler();
                goto ishero;
            }

            if (ForcedHandler != null && (a.hasTag(2) || popconfig.ProhibitedHasTagTwo))
                handler = ForcedHandler;
            else
                handler = PopDamageHandlerRegistry.GetHandler(a, self);

        ishero:
            handler.CreatePopDamage(a, self);


            UpdateCooldown(356515840,
                CalculateCooldownFrames(self.cd.baseFps, 0.3), self);
            UpdateCooldown(272629760,
                CalculateCooldownFrames(self.cd.baseFps, 0.4), self);

            self.dmgIdx++;
            if (self.dmgIdx > 4)
                self.dmgIdx = 0;
        }


        private void Hook_PopDamageHotline_Initialize(
            Hook__PopDamageHotline.orig___constructor__ orig,
            PopDamageHotline popDamage, Entity e, AttackData attackData,
            int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont)
        {
            bool? originalBig = !big.IsNull && big.value;

            popDamage.jiggle = 1.0;
            popDamage.textLayer = (dc.hl.types.ArrayObj)ArrayUtils.CreateDyn().array;

            var ctor = (HlAction<PopDamageHotline, Entity, AttackData, int, Ref<bool>, virtual_chars_font_>)
                dc.ui.PopDamage.Class.__constructor__;
            ctor.Invoke(popDamage, e, attackData, dmgIdx, big, customFont);

            popDamage.text.addShader(new HotlineText());
            popDamage.text.posChanged = true;
            popDamage.text.x = 0.0;
            popDamage.text.posChanged = true;
            popDamage.text.y = 0.0;

            for (int i = 0; i < 5; i++)
            {
                bool showPlus = attackData.dmgBonusMul > 1.0 || attackData.dmgScaledAdd > 0.0;
                string damageText = showPlus
                    ? $"{attackData.finalDmg}+"
                    : attackData.finalDmg.ToString();

                if (popconfig.GenuinePopDamage)
                    damageText = showPlus
                        ? $"{attackData.inflictedDmg}+"
                        : attackData.inflictedDmg.ToString();

                bool isMedieval = attackData.hitResult is HitResult.Critical
                    || attackData.dmgType is DamageType.BleedExplosion;

                var text = new dc.ui.Text(
                    null,
                    big: originalBig,
                    isMedieval: isMedieval,
                    Ref<double>.Null,
                    null,
                    customFont: customFont);

                text.set_textAlign(new Align.Center());
                text.set_text(damageText.ToHaxeString());

                dynamic hotline = text.addShader(new HotlineText());
                hotline.depth__ = i;

                text.canHaveBackground = false;

                popDamage.textLayer.push(text);
                popDamage.flow.addChildAt(text, 0);
            }

            popDamage.flow.set_needReflow(false);
        }


        private void Hook_PopDamage_Initialize(
            Hook__PopDamage.orig___constructor__ orig,
            dc.ui.PopDamage popDamage, Entity e, AttackData ad,
            int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont)
        {
            if (dc.ui.Console.Class.ME.flags.exists(dc.ui.Console.Class.HIDE_UI)
                && !dc.ui.Console.Class.ME.flags.exists("forcePopDmg".AsHaxeString()))
                return;

            bool isCritOrBleed = ad.hitResult is HitResult.Critical
                || ad.dmgType is DamageType.BleedExplosion;

            if (!big.IsNull)
                big.value = isCritOrBleed;

            var level = e._level;
            var processCtor = (HlAction<dc.ui.Process, dc.libs.Process>)
                dc.ui.Process.Class.__constructor__;
            processCtor.Invoke(popDamage, level);

            var cls = dc.ui.PopDamage.Class;
            cls.popDamageCount = cls.popDamageCount + 1;

            popDamage.level = level;
            popDamage.level!.uiProcesses.push(popDamage);
            popDamage.tracked = e;
            popDamage.createRootInLayers(popDamage.level.root, Const.Class.ROOT_DP_CTX_UI);

            popDamage.startIGX = popDamage.tracked.get_headX();
            popDamage.startIGY = popDamage.tracked.get_headY();

            popDamage.flow = new Flow(popDamage.root);

            int color = GetDamageColor(ad, e);
            if (ad.hitResult is HitResult.Critical)
                color = 16760576;

            popDamage.text = new dc.ui.Text(
                popDamage.flow, isCritOrBleed, null,
                Ref<double>.Null, null, customFont);
            popDamage.text.set_textAlign(new Align.Center());
            popDamage.text.posChanged = true;

            string damageText = ad.finalDmg.ToString();
            if (popconfig.GenuinePopDamage)
                damageText = ad.inflictedDmg.ToString();

            if (ad.dmgBonusMul > 1.0 || ad.dmgScaledAdd > 0.0)
                damageText += "+";

            popDamage.text.set_text(damageText.ToHaxeString());
            popDamage.text.set_textColor(color);
            popDamage.text.canHaveBackground = false;

            popDamage.flow.posChanged = true;
            popDamage.flow.x = 0.0;
            popDamage.flow.posChanged = true;

            var pixelScale = popDamage.get_pixelScale.Invoke();
            popDamage.flow.y = GetYOffsetForDamageIndex(dmgIdx, pixelScale * 9.0);

            popDamage.doMovement(e, ad);
            CreateFadeAnimation(popDamage, e, ad, handler);

            if (e._level.isBlur)
                popDamage.blur(Ref<double>.Null, Ref<double>.Null);
        }

        #endregion

        #region Helper

        private static void CreateFadeAnimation(dc.ui.Process popDamage, Entity e, AttackData ad, IPopDamage currentHandler)
        {
            double speedMult = GetSpeedMultiplier(e);

            if (Std.Class.@is(e, Hero.Class))
            {
                double duration = speedMult * 600.0;
                double delay = speedMult * (ad.dmgBonusMul > 1.33 || ad.dmgScaledAdd > 0.0 ? 1000.0 : 600.0);
                CreateFadeTween((dc.ui.PopDamage)popDamage, duration, delay);
                return;
            }

            bool isDefaultHandler = currentHandler?.Priority == int.MaxValue;

            if (isDefaultHandler)
            {
                double duration = speedMult * 450;
                double delay = speedMult * (ad.dmgBonusMul > 1.33 || ad.dmgScaledAdd > 0.0 ? 700.0 : 350.0);
                CreateFadeTween((dc.ui.PopDamage)popDamage, duration, delay);
            }
            else
            {
                double ms = (currentHandler?.SpeedMultiplier ?? 0.5) * 1000.0;
                CreateFadeTween((dc.ui.PopDamage)popDamage, ms, ms + 100);
            }
        }



        private static void CreateFadeTween(dc.ui.PopDamage popDamage, double duration, double delay)
        {
            var getter = new HlFunc<double>(() => popDamage.flow.alpha);
            var setter = new HlAction<double>(value =>
            {
                popDamage.flow.alpha = value;
                popDamage.flow.posChanged = true;
            });

            var tween = popDamage.tw.create_(getter, setter, 1.0, 0.0, null, duration, Ref<bool>.Null);
            tween.delayMs(delay);
            tween.end(new HlAction(popDamage.destroy));
        }



        private static int GetDamageColor(AttackData ad, Entity e)
        {
            switch (ad.dmgType.RawIndex)
            {
                case 0:
                case 1:
                case 4:
                    return Std.Class.@is(e, Hero.Class) ? 16711680 : 13490656;
                case 2: return 9118538;
                case 3: return 2929779;
                case 5: return 16739108;
                case 6:
                case 7: return 11962345;
                case 8: return 6317211;
                case 9: return 16767232;
                case 10: return 4431062;
                case 11: return 14373706;
                default: return 16711680;
            }
        }

        private static double GetYOffsetForDamageIndex(int dmgIdx, double baseOffset)
        {
            switch (dmgIdx)
            {
                case 0: return 0.0;
                case 1: return baseOffset;
                case 2: return -baseOffset;
                case 3: return 2 * baseOffset;
                case 4: return -2 * baseOffset;
                default: return 0.0;
            }
        }

        private static double GetSpeedMultiplier(Entity e)
        {
            if (Std.Class.@is(e, Hero.Class))
                return 2.0;
            if (e.cd.fastCheck.exists(356515840))
                return 0.6;
            return 1.0;
        }



        private static double CalculateCooldownFrames(double baseFps, double multiplier)
            => System.Math.Floor(baseFps * multiplier * 1000.0) / 1000.0;

        private static void UpdateCooldown(int key, double frames, Entity entity)
        {
            var cd = entity.cd.fastCheck.get(key);
            if (cd != null)
            {
                cd.frames = frames;
            }
            else
            {
                var inst = new CdInst(key, frames);
                entity.cd.fastCheck.set(key, inst);
                entity.cd.cdList.push(inst);
            }
        }

        public static virtual_chars_font_ CreateFontData(string fontType) => new()
        {
            chars = "numbers".ToHaxeString(),
            font = fontType.ToHaxeString()
        };

        #endregion
    }
}
