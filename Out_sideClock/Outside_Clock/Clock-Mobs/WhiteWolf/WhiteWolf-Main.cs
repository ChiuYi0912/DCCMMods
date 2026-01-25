using System;
using dc;
using dc.en;
using dc.h3d.mat;
using dc.haxe;
using dc.hxd;
using dc.hxd.res;
using dc.libs.heaps;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using HaxeProxy.Runtime;
using ModCore.Utitities;
using _Math = dc._Math;
using Log = Serilog.Log;

namespace Outside_Clock.Clock_Mobs.WhiteWolf;

public class WhiteWolf : Mob
{
    public WhiteWolf(Level x, int y, int kind, dc.String dmgTier, int lifeTier, int dmgTier1) : base(x, y, kind, dmgTier, lifeTier, dmgTier1)
    {
    }

    public override void initGfx()
    {
        base.initGfx();
        SpriteLib lib = Assets.Class.lib.get("atlas/WhiteWolf.atlas".AsHaxeString());
        _Res _Res = Res.Class;
        Loader loader = _Res.get_loader();
        _Image @class = Image.Class;
        _ImageExtender _ImageExtender = ImageExtender.Class;
        Texture nrmTex = _ImageExtender.toNormalMap((Image)loader.loadCache("atlas/WhiteWolf_n.png".AsHaxeString(), @class));
        base.initSprite(lib, null, 0.5, 0.5, null, true, null, nrmTex);

        AnimManager anim4 = base.spr.get_anim();
        HlFunc<bool> loop1 = new HlFunc<bool>(() =>
        {
            return base.isWalking();
        });
        HlFunc<bool> loop = () => base.isWalking();

        anim4.registerStateAnim("walk".AsHaxeString(), 1, null, loop, HaxeProxy.Runtime.Ref<bool>.Null, null);
        base.spr.get_anim().registerStateAnim("idle".AsHaxeString(), 0, null, null, Ref<bool>.Null, null);
        dc._Math _Math = dc.Math.Class;
        double num = _Math.random() * 0.01;
        double num2 = 0.9 + num;
        base.sprScaleY = num2;
        base.sprScaleX = num2;

        Log.Debug("sper :load");

    }

}
