using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Extensions;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.pr;
using dc.tool.quadTree;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Utilities;
using Serilog;

namespace DebugMod.Debugxtensions
{
    public class DebugGraphic :
    IEventReceiver
    {



        public DebugGraphic()
        {
            EventSystem.AddReceiver(this);
            Hook_QuadTree.initBoundaries += Hook_QuadTree_initBoundaries;
            Hook__LevelDisp.__constructor__ += Hook_LevelDisp_init;
        }



        private void Hook_LevelDisp_init(Hook__LevelDisp.orig___constructor__ orig, LevelDisp arg1, Level p, LevelMap map, ArrayObj parallaxInfo)
        {
            orig(arg1, p, map, parallaxInfo);
            arg1.debug = new dc.h2d.Graphics(null);
            p.scroller.addChildAt(arg1.debug, Const.Class.DP_DEBUG);
        }

        private void Hook_QuadTree_initBoundaries(Hook_QuadTree.orig_initBoundaries orig, QuadTree self, QtRectangle boundary, int capacity, int minimumSquareSize, Graphics _debugGraphic)
        {
            self.boundary = boundary;
            self.colorQuad = GenerateRandomColor();

            if (DebugModMod.GetConfig.Value.IsQuadTreeDrawingEnabled)
            {
                self.debugGraphic = _debugGraphic;
                double thickness = 2.0;
                int color = self.colorQuad;
                self.debugGraphic.lineStyle(Ref<double>.In(thickness), Ref<int>.In(color), Ref<double>.Null);
                self.debugGraphic.drawRect(boundary.x * 24, boundary.y * 24, boundary.w * 24, boundary.h * 24);
                self.debugGraphic.lineStyle(Ref<double>.Null, Ref<int>.Null, Ref<double>.Null);
            }

            self.capacity = capacity;
            self.minimumSquareSize = minimumSquareSize;

            self.entities = (ArrayObj)ArrayUtils.CreateDyn().array;
            self.points = (ArrayObj)ArrayUtils.CreateDyn().array;

            self.divided = false;
        }

        private int GenerateRandomColor()
        {
            double h = dc.Math.Class.random() * 100.0 / 100.0;
            const double s = 1.0;
            const double l = 1.0;

            double r, g, b;


            double hue = h * 6.0;
            int i = (int)System.Math.Floor(hue);
            double remainder = hue - i;

            double p = l * (1.0 - s);
            double q = l * (1.0 - s * remainder);
            double t = l * (1.0 - s * (1.0 - remainder));

            switch (i)
            {
                case 0: r = l; g = t; b = p; break;
                case 1: r = q; g = l; b = p; break;
                case 2: r = p; g = l; b = t; break;
                case 3: r = p; g = q; b = l; break;
                case 4: r = t; g = p; b = l; break;
                case 5: r = l; g = p; b = q; break;
                default: r = l; g = p; b = q; break;
            }


            int ir = (int)System.Math.Round(r * 255.0);
            int ig = (int)System.Math.Round(g * 255.0);
            int ib = (int)System.Math.Round(b * 255.0);

            return (ir << 16) | (ig << 8) | ib;
        }
    }
}