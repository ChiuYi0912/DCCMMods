using System;
using dc.light;
using dc.pr;
using dc.h3d.mat;
using dc.h2d;
using dc.haxe.ds;
using HaxeProxy.Runtime;
using LightOpt.Core.Configuration;
using ModCore.Storage;
using dc.h3d;

namespace LightOpt.OptLayers
{
    public class LightOptLayers : LightedLayers
    {
        private static CoreCfig Config => LightOptEntry.GetConfig.Value;

        public LightOptLayers(Level level, Ref<bool> hasReflect) : base(level, hasReflect)
        {

        }


        public new Tile render(int xmin, int ymin, int width, int height, int rWid, int rHei)
        {
            return base.render(xmin, ymin, width, height, rWid, rHei);
        }

        public new int set_blur(int v)
        {
            if (Config != null && Config.DisableBlur)
                v = 0;
            return base.set_blur(v);
        }
    }
}