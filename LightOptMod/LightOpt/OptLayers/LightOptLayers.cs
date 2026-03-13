using System;
using dc.light;
using dc.pr;
using dc.h3d.mat;
using dc.h2d;
using dc.haxe.ds;
using HaxeProxy.Runtime;
using ModCore.Storage;
using dc.h3d;
using dc.hxd;
using dc.hl.types;
using dc;
using dc.h3d.pass;
using dc.shader;
using CoreLibrary.Core.Extensions;
using dc.hxd.res;
using dc.level;
using dc.h3d.impl;
using dc.h2d.col;
using Serilog;
using CoreLibrary.Core.Utilities;
using dc.tool;
using dc.h3d.pass._Copy;
using dc.h3d.shader;
using Texture = dc.h3d.mat.Texture;

namespace LightOpt.OptLayers
{
    public class LightOptLayers : LightedLayers
    {
        public LightOptLayers(Level level, Ref<bool> hasReflect) : base(level, hasReflect)
        {

        }

        public new Tile render(int xmin, int ymin, int width, int height, int rWid, int rHei)
        {
            return base.render(xmin, ymin, width, height, rWid, rHei);
        }
        public void obidrawRec(dc.h2d.RenderContext ctx)
        {
            if (!visible) return;
            dc.h2d.Object? origobject = null!;
            ArrayObj child = children;
            if (posChanged)
            {
                calcAbsPos();
                for (int i = 0; i < child.length; i++)
                {
                    origobject = children.array[i] as dc.h2d.Object;
                    if (origobject == null)
                        continue;
                    origobject.posChanged = true;
                }
                posChanged = false;
            }

            if (filter != null)
            {
                drawFilters(ctx);
                return;
            }

            double originalAlpha = ctx.globalAlpha;
            ctx.globalAlpha = originalAlpha * alpha;

            if (ctx.front2back)
            {
                dc.h2d.Object? origobject1 = null!;
                for (int i = children.length - 1; i >= 0; i--)
                {
                    origobject1 = children.array[i] as dc.h2d.Object;
                    if (origobject1 == null)
                        continue;
                    origobject1.drawRec(ctx);
                }
                draw(ctx);
            }
            else
            {
                dc.h2d.Object? origobject2 = null!;
                draw(ctx);
                for (int i = 0; i < children.length; i++)
                {
                    origobject2 = children.array[i] as dc.h2d.Object;
                    if (origobject2 == null)
                        continue;
                    origobject2.drawRec(ctx);
                }
            }

            ctx.globalAlpha = originalAlpha;
        }


        public override void drawRec(dc.h2d.RenderContext ctx)
        {
            this.ctx = ctx;
            defaultState.manager = ctx.manager;

            int CeilToInt(double value) => (int)System.Math.Ceiling(value);

            int startX = -CeilToInt(base.absX / base.matA + 1.0);
            int startY = -CeilToInt(base.absY / base.matD + 1.0);
            int renderW = CeilToInt(ctx.curWidth / base.matA + 2.0);
            int renderH = CeilToInt(ctx.curHeight / base.matD + 2.0);
            int candidateMaxW = CeilToInt(ctx.curWidth / System.Math.Min(minScale, base.matA) + 2.0);
            int candidateMaxH = CeilToInt(ctx.curHeight / System.Math.Min(minScale, base.matD) + 2.0);

            if (maxWid < candidateMaxW) maxWid = candidateMaxW;
            if (maxHei < candidateMaxH) maxHei = candidateMaxH;
            int maxW = maxWid;
            int maxH = maxHei;

            var savedOnBeginDraw = ctx.onBeginDraw;
            var savedGlobalAlpha = ctx.globalAlpha;
            var savedInFilter = ctx.inFilter;

            dc.h3d.shader.Base2d baseShader = (dc.h3d.shader.Base2d)ctx.baseShader;
            var filterA = baseShader.filterMatrixA__;
            double savedAX = filterA.x, savedAY = filterA.y, savedAZ = filterA.z, savedAW = filterA.w;
            var filterB = baseShader.filterMatrixB__;
            double savedBX = filterB.x, savedBY = filterB.y, savedBZ = filterB.z, savedBW = filterB.w;

            try
            {
                double det = base.matA * base.matD - base.matB * base.matC;
                double invDet = 1.0 / det;
                double m11 = base.matD * invDet;
                double m12 = -base.matB * invDet;
                double m21 = -base.matC * invDet;
                double m22 = base.matA * invDet;
                double tx = -(base.absX * m11 + base.absY * m21);
                double ty = -(base.absX * m12 + base.absY * m22);

                var newFilterA = baseShader.filterMatrixA__;
                newFilterA.x = m11;
                newFilterA.y = m21;
                newFilterA.z = tx;
                newFilterA.w = 1.0;
                baseShader.filterMatrixA__ = newFilterA;

                var newFilterB = baseShader.filterMatrixB__;
                newFilterB.x = m12;
                newFilterB.y = m22;
                newFilterB.z = ty;
                newFilterB.w = 1.0;
                baseShader.filterMatrixB__ = newFilterB;

                ctx.globalAlpha = 1.0;
                ctx.onBeginDraw = new HlFunc<bool, Drawable>(onBeginDraw);
                ctx.inFilter = this;

                Tile tile = this.render(startX, startY, maxW, maxH, renderW, renderH);
                tile.dx = startX;
                tile.dy = startY;

                var restoreA = baseShader.filterMatrixA__;
                restoreA.x = savedAX;
                restoreA.y = savedAY;
                restoreA.z = savedAZ;
                restoreA.w = savedAW;
                baseShader.filterMatrixA__ = restoreA;

                var restoreB = baseShader.filterMatrixB__;
                restoreB.x = savedBX;
                restoreB.y = savedBY;
                restoreB.z = savedBZ;
                restoreB.w = savedBW;
                baseShader.filterMatrixB__ = restoreB;

                ctx.globalAlpha = savedGlobalAlpha * alpha;

                if (dc.h2d.Object.Class.nullDrawable == null)
                    dc.h2d.Object.Class.nullDrawable = new Drawable(null);

                var nullDrawable = dc.h2d.Object.Class.nullDrawable;
                nullDrawable.blendMode = new BlendMode.None();
                base.emitTile(ctx, tile);
                nullDrawable.blendMode = new BlendMode.Alpha();
            }
            finally
            {
                ctx.onBeginDraw = savedOnBeginDraw;
                ctx.inFilter = savedInFilter;
            }
        }

    }
}