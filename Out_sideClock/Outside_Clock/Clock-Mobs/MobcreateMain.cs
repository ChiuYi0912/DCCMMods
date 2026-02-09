using dc.en;
using dc.en.mob;
using dc.pr;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Storage;
using ModCore.Utilities;
using Outside_Clock.Interface.IOnAdvancedModuleInitializing;
using Serilog;

namespace Outside_Clock.Clock_Mobs;

public class MobcreateMain :
    IEventReceiver,
    IOnAdvancedModuleInitializing
{
    public MobcreateMain()
    {
        EventSystem.AddReceiver(this);
    }

    void IOnAdvancedModuleInitializing.OnAdvancedModuleInitializing(Outside_Main MODMAN)
    {
        MODMAN.Logger.Information("");
        dc.en.Hook__Mob.create += Hook__Mob_create;
    }
    public readonly SaveData<Data> save = new("miniLeapingDuelyst");

    public class Data
    {

    }
    private class sData
    {
        public int build = 0;
    }
    private sData data1 = new();
    private bool sprbool = false;
    public Mob Hook__Mob_create(Hook__Mob.orig_create orig, dc.String k, Level level, int cx, int cy, int dmgTier, Ref<int> lifeTier)
    {
        if (k.ToString().Equals("miniLeapingDuelyst", StringComparison.CurrentCultureIgnoreCase))
        {
            var mob1Leaping = new miniLeapingDuelyst(level, cx, cy, dmgTier, lifeTier.value);
            if (sprbool == false)
            {
                Hook_LeapingDuelyst.initGfx += mob1Leaping.Hook_LeapingDuelyst_initGfx;
                sprbool = true;
                Log.Information("已修改创造mob:minilea");
            }
            mob1Leaping.init();
            return mob1Leaping;
        }
        if (k.ToString().Equals("WhiteWolf", StringComparison.CurrentCultureIgnoreCase))
        {
            var WhiteWolf = new WhiteWolf.WhiteWolf(level, cx, cy, "WhiteWolf".AsHaxeString(), dmgTier, lifeTier.value);
            WhiteWolf.init();
            Log.Debug($"位置：x{WhiteWolf.cx} y:{WhiteWolf.cy}");
            return WhiteWolf;
        }
        return orig(k, level, cx, cy, dmgTier, lifeTier);
    }

}
