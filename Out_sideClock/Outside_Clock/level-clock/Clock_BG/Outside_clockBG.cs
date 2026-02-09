using System;
using System.Runtime.CompilerServices;
using dc;
using dc.h2d;
using dc.h2d.col;
using dc.h3d;
using dc.hl.types;
using dc.level;
using dc.level.disp;
using dc.libs.heaps.slib;
using dc.light;
using dc.pr;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;

namespace Outside_Clock.level_clock.Clock_BG;

public class Outside_clockBG : BiomeDisp
{


    public Outside_clockBG(Level p, LevelMap m, ArrayObj obj) : base(p, m, obj)
    {
        //_Outside_clockBG._Outside_clockBG_Initialize(this, p, m);

    }

    public override void render()
    {
        base.render();
        //Renderbackground();

    }

    public void Renderbackground()
    {
        SpriteLib slib = this.level.slib;

        int backgroundClass = Const.Class.DP_BACKGROUND;
        double parallaxDepth = 0.02;

        Parallax parallax = base.createParallax(
            backgroundClass,
            parallaxDepth,
            parallaxDepth
        );


        dc.String tilePath = dc.String.Class.__add__("bg/".AsHaxeString(), "sky".AsHaxeString());
        HlFunc<int, int> randomFlip = new HlFunc<int, int>(base.rng.random);
        Bitmap skyBitmap = new Bitmap(
            slib.getTileRandom(tilePath, Ref<double>.Null, Ref<double>.Null, randomFlip, null),
            parallax
        );

        skyBitmap.posChanged = true;

        double viewportWidth = parallax.vwid;
        Bounds parallaxBounds = parallax.bounds;

        double horizontalScale = parallaxBounds.xMax - parallaxBounds.xMin - viewportWidth;
        horizontalScale *= parallax.scrollX;
        horizontalScale += viewportWidth;
        horizontalScale /= (double)skyBitmap.tile.width;
        skyBitmap.scaleX = horizontalScale;

        double viewportHeight = parallax.vhei;
        parallaxBounds = parallax.bounds;

        double verticalScale = parallaxBounds.yMax - parallaxBounds.yMin - viewportHeight;
        verticalScale *= parallax.scrollY;
        verticalScale += viewportHeight;
        verticalScale /= (double)skyBitmap.tile.height;
        skyBitmap.scaleY = verticalScale;


        skyBitmap.blendMode = new BlendMode.None();


        backgroundClass = Const.Class.DP_BACKGROUND;
        parallaxDepth = 0.04;

        Log.Debug("已添加背景");

    }
}