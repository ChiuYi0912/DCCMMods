using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using dc;
using dc.en.mob;
using dc.h2d;
using dc.h3d.shader;
using dc.hl;
using dc.hl.types;
using dc.hxsl;
using dc.libs.misc;
using dc.shader;
using dc.tool.atk;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using PopDamage.shader;


namespace PopDamage.OtherPop
{
    public class PopDamageGradient : dc.ui.PopDamage
    {
        public double jiggle = 0;
        public ArrayObj texts = null!;
        private List<SimpleRevealShader> revealShaders = new();
        private double revealProgress = 0;
        private bool revealAnimating = true;

        // Constants for optimization
        private const double JiggleSpeed = 0.15;
        private const double Amplitude = 100.0;
        private const double RevealAnimationSpeed = 0.05;
        private const double VerticalDistance = -50.0;
        private const double HorizontalMultiplier = 0.5;
        private const double WaveFrequency1 = 0.3;
        private const double WaveFrequency2 = 0.5;
            

        public PopDamageGradient(Entity e, AttackData ad, int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont) : base(e, ad, dmgIdx, big, customFont)
        {
            jiggle = 1.0;
            texts = (ArrayObj)ArrayUtils.CreateDyn().array;


            var text_ = new dc.ui.Text(null, true, false, Ref<double>.Null, null, customFont);
            text_.blendMode = new BlendMode.Alpha();
            text_.alpha = 1.0;
            text_.set_textAlign(new Align.Center());

            dc.String damageText = Std.Class.@string(ad.finalDmg);
            if (ad.dmgBonusMul > 1.0)
                damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());
            
            else if (ad.dmgScaledAdd > 0.0)
                damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());
            
            text_.set_text(damageText);


            SimpleRevealShader shader = new SimpleRevealShader(text_.font.tile.innerTex);
            shader.progress__ = 0;
            text_.set_filter(new dc.h2d.filter.Shader(shader, "texture".AsHaxeString()));
            text_.canHaveBackground = false;
            texts.push(text_);
            flow.addChildAt(text, 0);
            revealShaders.Add(shader);


            SimpleRevealShader shader2 = new SimpleRevealShader(text.font.tile.innerTex);
            shader2.progress__ = 0;
            text.set_filter(new dc.h2d.filter.Shader(shader2, "texture".AsHaxeString()));
            text.blendMode = new BlendMode.Alpha();
            text.posChanged = true;
            text.x = 0.0;
            text.y = 0.0;
            flow.set_needReflow(true);
            revealShaders.Add(shader2);
        }


        public override void postUpdate()
        {

            if (revealAnimating && revealProgress < 1.0)
            {
                revealProgress += RevealAnimationSpeed * tmod;
                if (revealProgress > 1.0)
                {
                    revealProgress = 1.0;
                    revealAnimating = false;
                }


                foreach (var shader in revealShaders)
                {
                    shader.progress__ = revealProgress;
                }
            }

            double x = tracked._level.toGlobalX(startIGX);
            double y = tracked._level.toGlobalY(startIGY);
            root.posChanged = true;
            root.x = x;
            root.y = y;


            jiggle += JiggleSpeed * tmod;


            double baseX = text.x;
            double baseY = text.y;
            double pixelScale = get_pixelScale.Invoke();


            if (texts == null)
                return;

            int length = texts.length;
            for (int i = 0; i < length; i++)
            {
                if (texts.array[i] is not dc.ui.Text text)
                    continue;

                double wave = System.Math.Abs(System.Math.Sin(jiggle + i * WaveFrequency1));

                int verticalOffset = (int)(pixelScale * Amplitude * wave);
                int horizontalOffset = (int)(pixelScale * HorizontalMultiplier * System.Math.Sin(jiggle + i * WaveFrequency2) * (i % 3 - 1));

                text.x = baseX + horizontalOffset;
                text.y = baseY - verticalOffset;
                text.posChanged = true;
            }
        }

        private static Tween CreateTween(Tweenie tw, Func<double> getter, Action<double> setterAction, double targetValue, double? duration)
        {
            var hlGetter = new HlFunc<double>(getter);
            var hlAction = new HlAction<object>((_setV) => setterAction((double)_setV));
            var hlSetter = new HlAction<double>((dt) => hlAction.Invoke(dt));
            var tweenType = new TType.TEaseOut();
            return tw.create_(hlGetter, hlSetter, null, targetValue, tweenType, duration, Ref<bool>.Null);
        }

        public override void doMovement(Entity e, AttackData ad)
        {
            double pixelScale = get_pixelScale.Invoke(); // Cache pixel scale

            // Vertical movement tween
            double verticalTargetValue = flow.y + VerticalDistance * pixelScale;
            var verticalTween = CreateTween(tw,
                () => flow.y,
                value =>
                {
                    flow.y = value;
                    flow.posChanged = true;
                },
                verticalTargetValue,
                0);
        }

        public override void onDispose()
        {
            base.onDispose();
        }

        public static PopDamageGradient create(Entity e, AttackData ad, int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont)
        {
            return new PopDamageGradient(e, ad, dmgIdx, big, customFont);
        }
    }
}