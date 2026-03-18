using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Utilities;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.libs.heaps;
using dc.tool;
using HaxeProxy.Runtime;
using Serilog;
using Math = System.Math;

namespace CoreLibrary.Core.Extensions
{
    public static class FxExtensions
    {
        public static void DebugTile(this Fx fx, ArrayObj? tiles, double x, double y, double width, double height, int color, double duration = 0.1)
        {
            var fxTile = Assets.Class.fxTile;

            FxTile? tile;
            if (tiles != null && tiles.length > 0)
            {
                int index = Std.Class.random(tiles.length);

                tile = tiles.getDyn(index) as FxTile;
                if (tile == null)
                    tile = fxTile._fxDebugSquare.getDyn(0) as FxTile;
            }
            else
            {
                tile = fxTile._fxDebugSquare.getDyn(0) as FxTile;
            }

            if (tile == null)
                return;

            var particle = fx.allocTop(tile, x, y, Ref<bool>.Null, null, Ref<bool>.Null);
            if (particle == null)
                return;

            particle.scaleX = width / particle.t.width;
            particle.scaleY = height / particle.t.height;

            double r = ((color >> 16) & 0xFF) / 255.0;
            double g = ((color >> 8) & 0xFF) / 255.0;
            double b = (color & 0xFF) / 255.0;
            particle.r = r;
            particle.g = g;
            particle.b = b;

            particle.setFadeS(1.0, 0.0, duration);
            particle.set_lifeS(duration);
        }



        public static void DebugAllTiles(this Fx fx, LevelDisp disp, double duration = 10)
        {
            var groups = new StaticGeometryGroup[]
            {
                disp.groupBackWalls,
                disp.groupBackWallProps,
                disp.groupBackWallProps2,
                disp.groupBackProps,
                disp.groupMainProps,
                disp.groupGameplayProps,
                disp.groupBgFilterProps,
                disp.groupFrontWalls,
                disp.groupFrontWallProps,
            };



            foreach (var group in groups)
            {
                var iterator = group.getTileGroups();
                while (iterator.hasNext.Invoke())
                {
                    var tg = iterator.next.Invoke();

                    var content = tg.content;
                    if (content == null) continue;

                    var tmp = content.tmp;
                    if (tmp == null) continue;

                    int stride = 32;
                    for (int j = 0; j < tmp.length / stride; j++)
                    {
                        int b = j * stride;

                        float x0 = (float)tmp.getDyn(b + 0);
                        float y0 = (float)tmp.getDyn(b + 1);

                        float x1 = (float)tmp.getDyn(b + 8);
                        float y1 = (float)tmp.getDyn(b + 9);

                        float x2 = (float)tmp.getDyn(b + 16);
                        float y2 = (float)tmp.getDyn(b + 17);

                        float x3 = (float)tmp.getDyn(b + 24);
                        float y3 = (float)tmp.getDyn(b + 25);

                        float minX = Math.Min(Math.Min(x0, x1), Math.Min(x2, x3));
                        float minY = Math.Min(Math.Min(y0, y1), Math.Min(y2, y3));
                        float maxX = Math.Max(Math.Max(x0, x1), Math.Max(x2, x3));
                        float maxY = Math.Max(Math.Max(y0, y1), Math.Max(y2, y3));
                        var g = new dc.h2d.Graphics(fx.root);
                        g.lineStyle(Ref<double>.In(1), Ref<int>.In(CreateColor.ColorFromHex("#ffffff")), Ref<double>.In(1));
                        g.drawRect(minX, minY, maxX - minX, maxY - minY);

                    }
                }
            }
        }
    }
}