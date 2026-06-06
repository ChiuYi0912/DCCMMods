using System;
using System.Reflection.Emit;
using dc;
using dc.en;
using dc.h2d;
using dc.h3d;
using dc.h3d.mat;
using dc.libs.heaps.slib;
using dc.ui.hud;
using HaxeProxy.Runtime;
using ModCore.Modules;
using MoreSettings.shaders;
using MoreSettings.Utilities;

namespace MoreSettings.GameMechanics
{
    internal class CustomHeroLifeBar : LifeBar
    {
        public HSpriteBatch fullBatch = null!;
        private int currentColor;

        public CustomHeroLifeBar(LifeBarColorMode colorMode, dc.h2d.Object p) : base(colorMode, p)
        {
            var lib = Assets.Class.ui;
            var uitile = lib.pages.getDyn(0);

            fullBatch = new HSpriteBatch(uitile, null);
            fullBatch.hasRotationScale = true;
            fullBatch.tile.dx = this.sb.tile.dx;
            fullBatch.tile.dy = this.sb.tile.dy;

            addChildAt(fullBatch, 2);


            var oldBeLeft = this.beLeft;
            dc.String leftGroupName = oldBeLeft.groupName;
            int leftFrame = oldBeLeft.frame;
            var newBeLeft = new HSpriteBE(fullBatch, lib, leftGroupName, Ref<int>.In(leftFrame));
            newBeLeft.pivot.centerFactorX = oldBeLeft.pivot.centerFactorX;
            newBeLeft.pivot.centerFactorY = oldBeLeft.pivot.centerFactorY;
            newBeLeft.pivot.usingFactor = oldBeLeft.pivot.usingFactor;
            newBeLeft.pivot.isUndefined = false;
            newBeLeft.updateTile();
            newBeLeft.t.dx = oldBeLeft.t.dx;
            newBeLeft.t.dy = oldBeLeft.t.dy;
            newBeLeft.x = oldBeLeft.x;
            newBeLeft.y = oldBeLeft.y;
            oldBeLeft.remove();
            this.beLeft = newBeLeft;


            var oldBeRight = this.beRight;
            dc.String rightGroupName = oldBeRight.groupName;
            int rightFrame = oldBeRight.frame;
            var newBeRight = new HSpriteBE(fullBatch, lib, rightGroupName, Ref<int>.In(rightFrame));
            newBeRight.pivot.centerFactorX = oldBeRight.pivot.centerFactorX;
            newBeRight.pivot.centerFactorY = oldBeRight.pivot.centerFactorY;
            newBeRight.pivot.usingFactor = oldBeRight.pivot.usingFactor;
            newBeRight.pivot.isUndefined = false;
            newBeRight.updateTile();
            newBeRight.t.dx = oldBeRight.t.dx;
            newBeRight.t.dy = oldBeRight.t.dy;
            newBeRight.x = oldBeRight.x;
            newBeRight.y = oldBeRight.y;
            oldBeRight.remove();
            this.beRight = newBeRight;


            var oldBeFull = this.beFull;
            dc.String origGroupName = oldBeFull.groupName;
            int origFrame = oldBeFull.frame;
            double pivotX = oldBeFull.pivot.centerFactorX;
            double pivotY = oldBeFull.pivot.centerFactorY;
            bool usingFactor = oldBeFull.pivot.usingFactor;
            var newBeFull = new HSpriteBE(fullBatch, lib, origGroupName, Ref<int>.In(0));
            newBeFull.pivot.centerFactorX = pivotX;
            newBeFull.pivot.centerFactorY = pivotY;
            newBeFull.pivot.usingFactor = usingFactor;
            newBeFull.pivot.isUndefined = false;
            newBeFull.updateTile();
            newBeFull.t.dx = oldBeFull.t.dx;
            newBeFull.t.dy = oldBeFull.t.dy;
            oldBeFull.remove();
            this.beFull = newBeFull;

            currentColor = SettingsMain.ConfigValue.UI.LifeBarcolor;
            UpdateColor(currentColor);
            ResetInitialState();
        }

        private void ResetInitialState()
        {
            bool wasInit = this.isInit;
            this.isInit = true;

            if (this.curState.maxLife != 100.0)
            {
                this.curState.maxLife = 100.0;
                this.updateContent();
            }

            if (this.curState.life != 100.0)
            {
                double oldLifeVisual = (this.curState.life - this.oldState.life) * this.stateFade + this.oldState.life;
                this.curState.life = 100.0;
                this.updateContent();

                if (!wasInit)
                {
                    double newLifeVisual = (this.curState.life - this.oldState.life) * this.stateFade + this.oldState.life;
                    if (oldLifeVisual < newLifeVisual)
                        this.onHeal(oldLifeVisual, newLifeVisual);
                    else if (newLifeVisual < oldLifeVisual)
                        this.onDamage(oldLifeVisual, newLifeVisual);
                }
            }

            if (this.curState.recover != 25.0)
            {
                this.curState.recover = 25.0;
                this.updateContent();
            }

            this.healTrail = double.PositiveInfinity;

            this.isInit = wasInit;
        }


        public void SetColor(int newColor)
        {
            if (currentColor == newColor) return;
            currentColor = newColor;
            UpdateColor(newColor);
        }

        private void UpdateColor(int colorValue)
        {
            if (fullBatch == null) return;

            float r = ((colorValue >> 16) & 0xFF) / 255f;
            float g = ((colorValue >> 8) & 0xFF) / 255f;
            float b = (colorValue & 0xFF) / 255f;
            float a = ((colorValue >> 24) & 0xFF) / 255f;

            var shader = fullBatch.getShader(GradientColor.Class) as GradientColor;
            if (shader == null)
            {
                shader = new GradientColor(fullBatch.tile.innerTex);
                fullBatch.addShader(shader);
            }
            shader.tint__ = new Vector(Ref<double>.In(r), Ref<double>.In(g), Ref<double>.In(b), Ref<double>.In(a));
            shader.alpha__ = SettingsMain.ConfigValue.UI.LifeBarAlpha;
        }

        public void updatesbSize()
        {
            if (fullBatch != null && this.sb != null)
            {
                fullBatch.x = this.sb.x;
                fullBatch.y = this.sb.y;
                fullBatch.scaleX = this.sb.scaleX;
                fullBatch.scaleY = this.sb.scaleY;
            }
        }

        public override void sync(RenderContext ctx)
        {
            base.sync(ctx);
        }
    }
}