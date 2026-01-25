using System;
using dc;
using dc.h2d;
using dc.h3d;
using dc.level;
using dc.level.disp;
using dc.libs.heaps.slib;
using dc.light;
using dc.pr;
using HaxeProxy.Runtime;
using ModCore.Utitities;

namespace Outside_Clock.level_clock;

public class Outside_clockBG : ClockTower
{
    public Outside_clockBG(Level m, LevelMap biome, dc.String fxLevelCandle) : base(m, biome, fxLevelCandle)
    {
    }

    public new virtual void renderBackground()
    {
        base.renderBackground();

        SpriteLib slib = base.level.slib;
        dc._String _String = dc.String.Class;
        dc.String px = _String.__add__("bg/".AsHaxeString(), "clockMoon".AsHaxeString());

        int scrollX = Const.Class.DP_BACKGROUND;
        double scrollY = 0.1;
        double p = 0.1;
        Parallax parallax2 = base.createParallax(scrollX, scrollY, p);

        double py = 0.5;
        var ptr = Ref<double>.From(ref py);

        double rndFunc = 0.5;
        var ptr2 = Ref<double>.From(ref rndFunc);

        var flipMode = new HlFunc<int, int>(base.rng.random);


        Scatterer scatterer = new Scatterer(parallax2);
        var bounds = base.lmap.bounds;
        parallax2.x = (bounds.xMax - bounds.xMin) * 0.4f;
        parallax2.y = (bounds.yMax - bounds.yMin) * 0.7f;
        parallax2.posChanged = true;

        Bitmap bitmap2 = new Bitmap(slib.getTileRandom(px, ptr, ptr2, flipMode, null), scatterer);

        dc.String string2 = base.lmap.biome.scatterConf;

        base.applyScatterConf(scatterer, string2);

        Vector color = bitmap2.color;
        color.x = 2.0;
        color.y = 0.9176470588235294;
        color.z = 0.7098039215686275;
        color.w = 1.0;

        var add = new BlendMode.Add();
        bitmap2.blendMode = add;

    }

}