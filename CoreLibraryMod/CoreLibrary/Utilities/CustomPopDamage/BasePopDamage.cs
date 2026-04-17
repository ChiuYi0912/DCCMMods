

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
using dc.ui.popd;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using HaxeProxy.Runtime.Internals;
using ModCore.Events;
using ModCore.Utilities;
using Serilog;
using Serilog.Core;


namespace CoreLibrary.Utilities.CustomPopDamage
{
    public class BasePopDamage :
    IEventReceiver
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

        private const double PixelScaleBase = 9.0;

        // Speed multiplier constants
        private const double HeroSpeedMultiplier = 2.0;
        private const double FastEntitySpeedMultiplier = 0.6;
        private const double DefaultSpeedMultiplier = 1.0;

        // Identifier constants
        private const int FastCheckIdentifier = 356515840;

        private const int DamageIndexBaseMultiplier = 2;

        private readonly IPopDamageHandlerProvider _handlerProvider;

        public BasePopDamage() : this(new StaticPopDamageHandlerProvider()) { }

        public BasePopDamage(IPopDamageHandlerProvider handlerProvider)
        {
            _handlerProvider = handlerProvider ?? throw new ArgumentNullException(nameof(handlerProvider));
            dc.ui.Hook__PopDamage.__constructor__ += Hook_PopDamage_initalize;
        }

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
            bool originalBig = big.value;
            bool isCriticalOrBleedExplosion = ad.hitResult is HitResult.Critical || ad.dmgType is DamageType.BleedExplosion;


            if (!big.IsNull)
            {
                big.value = isCriticalOrBleedExplosion;
            }

            var level = e._level;
            ui_ProcessInit(popDamage, level);



            var popDamageClass = dc.ui.PopDamage.Class;
            popDamageClass.popDamageCount = popDamageClass.popDamageCount + 1;

            popDamage.level = level;
            popDamage.level!.uiProcesses.push(popDamage);
            popDamage.tracked = e;

            int rootDpCtxUi = Const.Class.ROOT_DP_CTX_UI;
            popDamage.createRootInLayers(popDamage.level.root, rootDpCtxUi);

            popDamage.startIGX = popDamage.tracked.get_headX();
            popDamage.startIGY = popDamage.tracked.get_headY();

            popDamage.flow = new Flow(popDamage.root);

            int color = GetDamageColor(ad, e);
            if (ad.hitResult is HitResult.Critical)
                color = CriticalDamageColor;

            bool? isBig = isCriticalOrBleedExplosion;

            if (popDamage is PopDamageHotline)
                popDamage.text = new dc.ui.Text(null, isBig, null, Ref<double>.Null, null, customFont);
            else
                popDamage.text = new dc.ui.Text(popDamage.flow, isBig, null, Ref<double>.Null, null, customFont);
            popDamage.text.set_textAlign(new Align.Center());
            popDamage.text.posChanged = true;

            var damageText = Std.Class.@string(ad.finalDmg);
            if (ad.dmgBonusMul > 1.0 || ad.dmgScaledAdd > 0.0)
                damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());

            popDamage.text.set_text(damageText);
            popDamage.text.set_textColor(color);
            popDamage.text.canHaveBackground = false;


            popDamage.text.posChanged = true;
            popDamage.text.x = 0.0;
            popDamage.text.posChanged = true;
            popDamage.text.y = 0.0;


            popDamage.flow.posChanged = true;
            popDamage.flow.x = 0.0;
            popDamage.flow.posChanged = true;
            double pixelScale = popDamage.get_pixelScale.Invoke();
            int baseOffset = (int)(pixelScale * PixelScaleBase);
            double yOffset = GetYOffsetForDamageIndex(dmgIdx, baseOffset);
            popDamage.flow.y = yOffset;


            popDamage.doMovement(e, ad);


            CreateOriginalStyleFadeAnimation(popDamage, e, ad);

            if (e._level.isBlur)
                popDamage.blur(Ref<double>.Null, Ref<double>.Null);

        }

        private void CreateOriginalStyleFadeAnimation(dc.ui.PopDamage popDamage, Entity e, AttackData ad)
        {
            double speedMultiplier = GetSpeedMultiplier(e);
            bool isHero = Std.Class.@is(e, Hero.Class);

            if (isHero)
            {
                double duration = speedMultiplier * 450.0;
                double delay = speedMultiplier * ((ad.dmgBonusMul > 1.33 || ad.dmgScaledAdd > 0.0) ? 1000.0 : 600.0);
                CreateFadeTween(popDamage, duration, delay);
            }
            else
            {
                double duration = speedMultiplier * _handlerProvider.GetHandler(e).SpeedMultiplier;
                double delay = speedMultiplier * ((ad.dmgBonusMul > 1.33 || ad.dmgScaledAdd > 0.0) ? 700.0 : 350.0);
                CreateFadeTween(popDamage, duration, delay);
            }
        }

        private static void CreateFadeTween(dc.ui.PopDamage popDamage, double duration, double delay)
        {
            var hlGetter = new HlFunc<double>(() => popDamage.flow.alpha);
            var hlSetter = new HlAction<double>((value) =>
            {
                popDamage.flow.alpha = value;
                popDamage.flow.posChanged = true;
            });

            var tween = popDamage.tw.create_(hlGetter, hlSetter, 1.0, 0.0, null, duration, Ref<bool>.Null);
            tween.delayMs(delay);
            tween.end(new HlAction(() => popDamage.destroy()));
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


        private static double GetSpeedMultiplier(Entity e)
        {
            if (Std.Class.@is(e, Hero.Class))
                return HeroSpeedMultiplier;
            else if (e.cd.fastCheck.exists(FastCheckIdentifier))
                return FastEntitySpeedMultiplier;
            else
                return DefaultSpeedMultiplier;
        }
    }
}