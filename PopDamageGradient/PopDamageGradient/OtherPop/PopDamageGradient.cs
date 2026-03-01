using System.Runtime.CompilerServices;
using dc;
using dc.en.mob;
using dc.h2d;
using dc.h3d.shader;
using dc.hl.types;
using dc.libs.misc;
using dc.shader;
using dc.tool.atk;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using PopDamage.shader;
using PopDamage.shader.ShaderSRC;


namespace PopDamage.OtherPop
{
    public class PopDamageGradient : dc.ui.PopDamage
    {
        public double jiggle = 0;
        public ArrayObj texts = null!;
        private List<SimpleRevealShader> revealShaders = new();
        private double revealProgress = 0;
        private bool revealAnimating = true;
        public PopDamageGradient(Entity e, AttackData ad, int dmgIdx, Ref<bool> big, virtual_chars_font_ customFont) : base(e, ad, dmgIdx, big, customFont)
        {
            dc.String RevealSRC = ShaderSRC.RevealSRC;
            HaxeProxy.Runtime.HaxeProxyUtils.GetClass<_ScreenShader>(typeof(SimpleRevealShader)).SRC = RevealSRC;

            this.jiggle = 1.0;
            this.texts = (ArrayObj)ArrayUtils.CreateDyn().array;

            for (int i = 0; i < 5; i++)
            {
                dc.ui.Text text_ = new dc.ui.Text(null, true, false, Ref<double>.Null, null, customFont);
                text_.blendMode = new BlendMode.Alpha();
                text_.alpha = 1.0;

                text_.set_textAlign(new Align.Center());

                dc.String damageText = Std.Class.@string(ad.finalDmg);

                if (ad.dmgBonusMul > 1.0)
                {
                    damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());
                }
                else if (ad.dmgScaledAdd > 0.0)
                {
                    damageText = dc.String.Class.__add__(damageText, "+".AsHaxeString());
                }
                text_.set_text(damageText);


                SimpleRevealShader shader = new SimpleRevealShader(text.font.tile.innerTex);
                shader.progress__ = 0;

                text_.set_filter(new dc.h2d.filter.Shader(shader, "texture".AsHaxeString()));
                text_.canHaveBackground = false;

                this.texts.push(text);
                this.flow.addChildAt(text, 0);

                revealShaders.Add(shader);
            }

            SimpleRevealShader shader2 = new SimpleRevealShader(this.text.font.tile.innerTex);
            shader2.progress__ = 0;
            revealShaders.Add(shader2);

            this.text.set_filter(new dc.h2d.filter.Shader(shader2, "texture".AsHaxeString()));
            this.text.blendMode = new BlendMode.Alpha();
            this.text.alpha = 1.0;

            this.text.posChanged = true;
            this.text.x = 0.0;
            this.text.y = 0.0;

            this.flow.set_needReflow(false);
        }


        public override void postUpdate()
        {

            if (revealAnimating && revealProgress < 1.0)
            {
                revealProgress += 0.1 * base.tmod;
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

            double x = this.tracked._level.toGlobalX(this.startIGX);
            double y = this.tracked._level.toGlobalY(this.startIGY);
            base.root.posChanged = true;
            base.root.x = x;
            base.root.y = y;


            this.jiggle += 0.15 * base.tmod;


            double baseX = base.text.x;
            double baseY = base.text.y;
            double pixelScale = base.get_pixelScale.Invoke();


            if (this.texts == null)
                return;

            for (int i = 0; i < this.texts.length; i++)
            {
                dc.ui.Text? text = this.texts.array[i] as dc.ui.Text;
                if (text == null)
                    continue;

                double amplitude = 100.0;

                double wave = System.Math.Abs(System.Math.Sin(this.jiggle + i * 0.3));

                int verticalOffset = (int)(pixelScale * amplitude * wave);
                int horizontalOffset = (int)(pixelScale * 0.5 * System.Math.Sin(this.jiggle + i * 0.5) * (i % 3 - 1));

                text.x = baseX + horizontalOffset;
                text.y = baseY - verticalOffset;
                text.posChanged = true;
            }
        }

        public override void doMovement(Entity e, AttackData ad)
        {
            Tweenie tw = base.tw;
            int direction = ad.dirSourceToTarget();


            int randomOffset1 = Std.Class.random(16);
            double baseDistance = 35 + randomOffset1;
            int mainOffset = direction * (int)(base.get_pixelScale.Invoke() * baseDistance);


            int randomRange = 5;
            double randomJitter = Std.Class.random(randomRange) * (Std.Class.random(2) * 2 - 1);
            double pixelJitter = base.get_pixelScale.Invoke() * randomJitter;
            int jitterInt = (int)pixelJitter;


            double targetValue = mainOffset + jitterInt - 20;

            HlFunc<double> basehlFunc = new HlFunc<double>(() => base.flow.x);
            HlAction<object> hlAction = new HlAction<object>((_setV) =>
            {
                Flow flow = base.flow;
                flow.x = (double)_setV;
                flow.posChanged = true;
            });
            HlAction<double> setter = new HlAction<double>((dt) => hlAction.Invoke(dt));

            TType tweenType = new TType.TEaseOut();
            double? duration = 0;
            Tween tween = tw.create_(basehlFunc, setter, null, targetValue, tweenType, duration, Ref<bool>.Null);


            // 向上移动的tween
            HlFunc<double> verticalGetter = new HlFunc<double>(() => base.flow.y);
            HlAction<object> verticalAction = new HlAction<object>((_setV) =>
            {
                Flow flow = base.flow;
                flow.y = (double)_setV;
                flow.posChanged = true;
            });
            HlAction<double> verticalSetter = new HlAction<double>((dt) => verticalAction.Invoke(dt));

            double verticalDistance = -20.0;
            duration = 0;
            double verticalTargetValue = base.flow.y + verticalDistance * base.get_pixelScale.Invoke();

            Tween verticalTween = tw.create_(verticalGetter, verticalSetter, null, verticalTargetValue, tweenType, duration, Ref<bool>.Null);


            // 缩放tween
            HlFunc<double> scaleGetter = new HlFunc<double>(() => base.flow.scaleX);
            HlAction<object> scaleAction = new HlAction<object>((_setV) =>
            {
                Flow flow = base.flow;
                flow.scaleX = (double)_setV;
                flow.scaleY = (double)_setV;
                flow.posChanged = true;
            });
            HlAction<double> scaleSetter = new HlAction<double>((dt) => scaleAction.Invoke(dt));

            double endScale = 1.5;
            duration = 200;
            double scaleTargetValue = endScale;

            Tween scaleTween = tw.create_(scaleGetter, scaleSetter, null, scaleTargetValue, tweenType, duration, Ref<bool>.Null);

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