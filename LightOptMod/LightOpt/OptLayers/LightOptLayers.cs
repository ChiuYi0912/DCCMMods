using System;
using dc.light;
using dc.pr;
using dc.h3d.mat;
using dc.h2d;
using dc.haxe.ds;
using HaxeProxy.Runtime;
using ModCore.Storage;
using dc.h3d;

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

    }
}