using System.Diagnostics;
using System.Reflection;
using dc;
using dc.en;
using dc.en.mob;
using dc.h3d.mat;
using dc.hl;
using dc.hxd;
using dc.hxd.res;
using dc.libs.heaps;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using dc.tool.skill;
using HaxeProxy.Runtime;
using ModCore.Serialization;
using ModCore.Storage;
using ModCore.Utilities;
using Log = Serilog.Log;
using Math = System.Math;

namespace Outside_Clock.Clock_Mobs;

public class miniLeapingDuelyst : LeapingDuelyst,
    IHxbitSerializeCallback,
    IHxbitSerializable<miniLeapingDuelyst.Data>
{
    private class Data
    {
        public miniLeapingDuelyst mobminiLeapingDuelyst = null!;
        public int maxlife;
        public double sprx;
        public double spry;
    }


    public miniLeapingDuelyst(Level lvl, int x, int y, int dmgTier, int lifeTier) : base(lvl, x, y, dmgTier, lifeTier)
    {

    }

    public static SpriteLib Hook_AssetsLibManager_get(Hook_AssetsLibManager.orig_get orig, AssetsLibManager self, dc.String o)
    {
        var mob_atlas = ToolsMob.Replace_atlas(orig, self, o, "atlas/LeapingDuelyst.atlas", "atlas/miniLeapingDuelyst.atlas");
        return mob_atlas;
    }
    public static dc.hxd.res.Resource Hook_Loader_loadCache(Hook_Loader.orig_loadCache orig, Loader self, dc.String c, Class res)
    {
        var mob_n_png = ToolsMob.Hook_Loader_loadCache(orig, self, c, res, "atlas/LeapingDuelyst_n.png", "atlas/miniLeapingDuelyst_n.png");
        return mob_n_png;
    }

    public override void initGfx()
    {
        base.initGfx();
        
    }

    public static TDelegate CreateClosure<TDelegate>(object instance, MethodInfo method) where TDelegate : Delegate
    {
        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), instance, method);
    }


    public override void init()
    {
        base.init();


    }
    private bool heroatargrt = true;
    private bool dodgebool = false;
    public override void behaviourAi()
    {

        if (this.life < maxLife)
        {


            double ownPixelX = ((double)this.cx + this.xr) * 24;

            if (this.aTarget != null && dodgebool == false)
            {

                double targetPixelX = ((double)aTarget.cx + aTarget.xr) * 24;

                int direction;
                if (targetPixelX > ownPixelX)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }

                heroatargrt = dodge.prepare(direction);
                OldSkill jumpSkill = base.getOldSkill("jump2".AsHaxeString());
                base.queueAttack((OldMobSkill)jumpSkill, false, null);
                dodgebool = true;
            }
        }

        if (aTarget != null && dodgebool == true)
        {
            double targetWorldX = (double)this.aTarget.cx + aTarget.xr;
            double ownWorldX = (double)this.cx + this.xr;

            double deltaX = Math.Abs(targetWorldX - ownWorldX);

            double targetWorldY = (double)this.aTarget.cy + aTarget.yr;
            double ownWorldY = (double)this.cy + this.yr;

            double deltaY = Math.Abs(targetWorldY - ownWorldY);

            if (deltaX < 20 && deltaY <= 2)
            {
                dodgebool = false;
                Log.Debug($"解除限制 X距离:{deltaX:F2} Y距离:{deltaY:F2}");
            }
        }

        base.behaviourAi();
    }

    private bool loadspr = false;
    private bool lifetow = false;
    public override void preUpdate()
    {
        base.preUpdate();

        if (life < maxLife / 2)
        {
            if (loadspr == false)
            {
                int length = 3;
                for (int i = 0; i < length; i++)
                {

                    sprScaleX += 0.1;
                    sprScaleY += 0.1;
                    baseMoveSpeedMul += 0.1;
                    data.maxlife = life;
                    createLight(100, 10, 100, 1);
                }
                loadspr = true;
            }
        }
        if (life < maxLife / 2 && lifetow == false)
        {
            this.elite = true;
            data.sprx = sprScaleX;
            data.spry = sprScaleY;
            life = maxLife * 2;
            lifetow = true;

        }

    }

    public override void onLand(double d)
    {
        base.onLand(d);
        //var list = Cooldown.Class.INDEXES.getDyn(75);
        dc.tool.Cooldown cd = this.cd;
        var cdframes = cd.cdList.array;

        Log.Debug(" 240-> " + Cooldown.Class.INDEXES.getDyn(240));
        Log.Debug(" 7-> " + Cooldown.Class.INDEXES.getDyn(7));
    }
    #region 序列化

    private Data data = new();


    Data IHxbitSerializable<Data>.GetData()
    {
        data.maxlife = maxLife;
        data.sprx = sprScaleX;
        data.spry = sprScaleY;
        return data;
    }


    void IHxbitSerializable<Data>.SetData(Data data)
    {
        maxLife = data.maxlife;
        sprScaleX = data.sprx;
        sprScaleY = data.spry;
        this.data = data;
    }

    void IHxbitSerializeCallback.OnBeforeSerializing()
    {

    }
    void IHxbitSerializeCallback.OnAfterDeserializing()
    {
        Debug.Assert(data != null);
    }
    #region 贴图
    public void Hook_LeapingDuelyst_initGfx(Hook_LeapingDuelyst.orig_initGfx orig, LeapingDuelyst self)
    {
        if (self._level.map.id.ToString().Equals("Out_Clock", StringComparison.CurrentCultureIgnoreCase))
        {
            Hook_AssetsLibManager.get += miniLeapingDuelyst.Hook_AssetsLibManager_get;
            Hook_Loader.loadCache += miniLeapingDuelyst.Hook_Loader_loadCache;
        }
        else
        {
            Hook_AssetsLibManager.get -= miniLeapingDuelyst.Hook_AssetsLibManager_get;
            Hook_Loader.loadCache -= miniLeapingDuelyst.Hook_Loader_loadCache;
        }
        orig(self);
    }
    #endregion
    #endregion
}
