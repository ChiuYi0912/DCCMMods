

using System;
using System.Runtime.CompilerServices;
using dc;
using dc.en;
using dc.h2d;
using dc.hl;
using dc.libs;
using dc.libs.misc;
using dc.pr;
using dc.tool.atk;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using HaxeProxy.Runtime.Internals;
using ModCore.Events;
using ModCore.Utilities;
using PopDamage.Main.Config;
using PopDamage.Main.lnterface;
using PopDamage.OtherPop;

namespace PopDamage.Override
{
    public class BasePopDamage :
    IEventReceiver,
    IOnHookInitalize
    {
        // Color constants
        private const int CriticalDamageColor = 16760576;            // Orange color for critical hits
        private const int HeroDamageColor = 16711680;                // Red color for hero damage
        private const int EnemyDamageColor = 13490656;               // Color for enemy damage
        private const int PoisonDamageColor = 9118538;               // Green color for poison damage
        private const int RootDamageColor = 2929779;                 // Purple color for root damage
        private const int BleedDamageColor = 16739108;               // Orange color for bleed damage
        private const int ShockDamageColor = 11962345;               // Yellow color for shock damage
        private const int OilDamageColor = 6317211;                  // Brown color for oil damage
        private const int FireDamageColor = 16767232;                // Bright orange color for fire damage
        private const int IceDamageColor = 4431062;                  // Blue color for ice damage
        private const int StunDamageColor = 14373706;                // Pink color for stun damage
        private const int DefaultDamageColor = 16711680;             // Default red color

        // Time constants (in milliseconds)
        private const double HeroFadeDurationBase = 600.0;
        private const double EnemyFadeDurationBase = 450.0;
        private const double RevalFadeDurationBase = 600.0;
        private const double LongDelayThreshold = 1000.0;
        private const double ShortDelayThreshold = 600.0;
        private const double EnemyLongDelayThreshold = 700.0;
        private const double EnemyShortDelayThreshold = 350.0;

        // Threshold constants
        private const double DamageBonusMultiplierThreshold = 1.33;
        private const double PixelScaleBase = 9.0;

        // Speed multiplier constants
        private const double HeroSpeedMultiplier = 2.0;
        private const double FastEntitySpeedMultiplier = 0.6;
        private const double DefaultSpeedMultiplier = 1.0;

        // Identifier constants
        private const int FastCheckIdentifier = 356515840;
        
        private const int DamageIndexBaseMultiplier = 2;

        public BasePopDamage() => EventSystem.AddReceiver(this);
        private static ModCore.Storage.Config<CoreConfig> GetConfig = EntityPopDaamage.GetConfig;
        void IOnHookInitalize.HookInitalize(PopDamageEntry entry) => dc.ui.Hook__PopDamage.__constructor__ += Hook_PopDamage_initalize;

        public static void ui_ProcessInit(dc.ui.Process ui_process, dc.libs.Process parent)
        {
            if (ui_process.get_pixelScale == null)
                ui_process.get_pixelScale = new HlFunc<double>(ui_process.get_pixelScale!);

            libs_ProcessInit(ui_process, parent);
            dc.ui.Process.Class.ALL.push(ui_process);
        }

        public static void libs_ProcessInit(dc.libs.Process arg1, dc.libs.Process parent)
        {
            if (arg1.onUpdateCb == null)
                arg1.onUpdateCb = new HlAction(arg1.onUpdateCb!);
            if (arg1.onDisposeCb == null)
                arg1.onDisposeCb = new HlAction(arg1.onDisposeCb!);

            arg1.init();

            if (parent == null)
                dc.libs.Process.Class.ROOTS.push(arg1);
            else
                parent.addChild(arg1);

        }

        private void Hook_PopDamage_initalize(Hook__PopDamage.orig___constructor__ orig, dc.ui.PopDamage popDamage, Entity e, AttackData ad, int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont)
        {
            var level = e._level;
            ui_ProcessInit(popDamage, level);
            var popDamageClass = dc.ui.PopDamage.Class;
            popDamageClass.popDamageCount = popDamageClass.popDamageCount + 1;

            popDamage.level = level;
            popDamage.level.uiProcesses.push(popDamage);
            popDamage.tracked = e;

            var root = popDamage.level.root;
            int rootDpCtxUi = Const.Class.ROOT_DP_CTX_UI;
            popDamage.createRootInLayers(root, rootDpCtxUi);

            popDamage.startIGX = popDamage.tracked.get_headX();
            popDamage.startIGY = popDamage.tracked.get_headY();

            var flow = new Flow(popDamage.root);
            popDamage.flow = flow;

            int color = GetDamageColor(ad, e);

            if (ad.hitResult is HitResult.Critical)
                color = CriticalDamageColor;

            bool isCriticalOrBleedExplosion = ad.hitResult is HitResult.Critical || ad.dmgType is DamageType.BleedExplosion;
            bool? isBig = isCriticalOrBleedExplosion;

            var text = new dc.ui.Text(flow, isBig, null, Ref<double>.Null, null, customFont);
            popDamage.text = text;

            text.set_textAlign(new Align.Center());

            var damageText = Std.Class.@string(ad.finalDmg);
            if (ad.dmgBonusMul > 1.0 || ad.dmgScaledAdd > 0.0)
                damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());

            text.set_text(damageText);
            text.set_textColor(color);
            text.canHaveBackground = false;
            flow.posChanged = true;
            flow.x = 0.0;

            double pixelScale = popDamage.get_pixelScale.Invoke();
            int baseOffset = (int)(pixelScale * PixelScaleBase);
            double yOffset = GetYOffsetForDamageIndex(dmgIdx, baseOffset);
            flow.y = yOffset;

            popDamage.doMovement(e, ad);

            CreateFadeAnimation(popDamage, e, ad);

            if (e._level.isBlur)
                popDamage.blur(Ref<double>.Null, Ref<double>.Null);

        }

        private static int GetDamageColor(AttackData ad, Entity e)
        {
            int dmgTypeIndex = ad.dmgType.RawIndex;

            switch (dmgTypeIndex)
            {
                case 0:
                case 1:
                case 4:
                    return Std.Class.@is(e, Hero.Class) ? HeroDamageColor : EnemyDamageColor;
                case 2: return PoisonDamageColor;
                case 3: return RootDamageColor;
                case 5: return BleedDamageColor;
                case 6:
                case 7: return ShockDamageColor;
                case 8: return OilDamageColor;
                case 9: return FireDamageColor;
                case 10: return IceDamageColor;
                case 11: return StunDamageColor;
                default: return DefaultDamageColor;
            }
        }

        private static double GetYOffsetForDamageIndex(int dmgIdx, int baseOffset)
        {
            switch (dmgIdx)
            {
                case 0: return 0.0;
                case 1: return baseOffset;
                case 2: return -baseOffset;
                case 3: return DamageIndexBaseMultiplier * baseOffset;
                case 4: return -DamageIndexBaseMultiplier * baseOffset;
                default: return 0.0;
            }
        }

        private static void CreateFadeAnimation(dc.ui.PopDamage popDamage, Entity e, AttackData ad)
        {
            double speedMultiplier = GetSpeedMultiplier(e);
            bool isHero = Std.Class.@is(e, Hero.Class);

            if (isHero)
                CreateHeroFadeTween(popDamage, speedMultiplier, ad);
            else
                if (GetConfig.Value.RevealPop)
                    CreateReavalFadeTween(popDamage, speedMultiplier, ad, popDamage.text.text.length);
                else
                    CreateEnemyFadeTween(popDamage, speedMultiplier, ad);

        }

        private static double GetSpeedMultiplier(Entity e)
        {
            if (Std.Class.@is(e, Hero.Class))
                return HeroSpeedMultiplier;
            else if (e.cd.fastCheck.exists(FastCheckIdentifier))
                return FastEntitySpeedMultiplier;
            else
                return DefaultSpeedMultiplier;
        }

        private static void CreateHeroFadeTween(dc.ui.PopDamage popDamage, double speedMultiplier, AttackData ad)
        {
            double duration = speedMultiplier * HeroFadeDurationBase;
            var hlGetter = new HlFunc<double>(() => popDamage.flow.alpha);
            var hlSetter = new HlAction<double>((value) =>
            {
                popDamage.flow.alpha = value;
                popDamage.flow.posChanged = true;
            });


            var tweenType = new TType.TEaseOut();
            var tween = popDamage.tw.create_(hlGetter, hlSetter, 1.0, 0.0, tweenType, duration, Ref<bool>.Null);
            double delay = speedMultiplier * ((ad.dmgBonusMul > DamageBonusMultiplierThreshold || ad.dmgScaledAdd > 0.0) ? LongDelayThreshold : ShortDelayThreshold);
            tween.delayMs(delay);
            tween.end(new HlAction(() => popDamage.destroy()));
        }

        private static void CreateEnemyFadeTween(dc.ui.PopDamage popDamage, double speedMultiplier, AttackData ad)
        {
            double duration = speedMultiplier * EnemyFadeDurationBase;

            var hlGetter = new HlFunc<double>(() => popDamage.flow.alpha);
            var hlSetter = new HlAction<double>((value) =>
            {
                popDamage.flow.alpha = value;
                popDamage.flow.posChanged = true;
            });

            var tweenType = new TType.TEaseOut();
            var tween = popDamage.tw.create_(hlGetter, hlSetter, 1.0, 0.0, tweenType, duration, Ref<bool>.Null);
            double delay = speedMultiplier * ((ad.dmgBonusMul > DamageBonusMultiplierThreshold || ad.dmgScaledAdd > 0.0) ? EnemyLongDelayThreshold : EnemyShortDelayThreshold);
            tween.delayMs(delay);
            tween.end(new HlAction(() => popDamage.destroy()));
        }

        private static void CreateReavalFadeTween(dc.ui.PopDamage popDamage, double speedMultiplier, AttackData ad, double textLength)
        {

            double duration = speedMultiplier * RevalFadeDurationBase * textLength;
            var hlGetter = new HlFunc<double>(() => popDamage.flow.alpha);
            var hlSetter = new HlAction<double>((value) =>
            {
                popDamage.flow.alpha = value;
                popDamage.flow.posChanged = true;
            });


            var tweenType = new TType.TEaseOut();
            var tween = popDamage.tw.create_(hlGetter, hlSetter, 1.0, 0.0, tweenType, duration, Ref<bool>.Null);
            double delay = speedMultiplier * textLength;
            tween.delayMs(delay);
            tween.end(new HlAction(() => popDamage.destroy()));
        }
    }
}