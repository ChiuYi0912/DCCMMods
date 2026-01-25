using dc.en;
using HaxeProxy.Runtime;
using ModCore.Storage;
using ModCore.Utitities;
using Serilog;


namespace Outside_Clock;

public class _EntrancOut_Clock : IHxbitSerializable
{
    internal static void heroanimOut_Clock(Hero owen, EntrancOut_Clock heroanim)
    {
        var cm = heroanim.cm;
        HlAction hlAction = new HlAction(() =>
        {
            owen.cancelVelocities();
            Log.Debug("已经执行取消速度");
        });

        HlAction hlAction1 = new HlAction(() =>
        {
            owen.spr.get_anim().play("travolta".AsHaxeString(), null, null);
            owen.say("这难道是时守干的？？？！！！".AsHaxeString(), 1, owen.cx + 50, owen.cy - 50);
            Log.Debug("已经执行动画1");
        });
        HlAction hlAction2 = new HlAction(() =>
        {

            owen.spr.get_anim().play("fuckOffFast".AsHaxeString(), null, null);
            Log.Debug("已经执行等待");
        });
        HlAction hlActionExit = new HlAction(() =>
        {
            heroanim.destroyed = true;
            Log.Debug("已经执行等待");
        });
        HlAction hlActionExit1 = new HlAction(() =>
        {

            Log.Debug("已经执行等待");
        });

        cm.__beginNewQueue();
        cm.__add(hlAction, 0, null);
        cm.__add(hlActionExit1, 1000, null);
        cm.__add(hlAction1, 0, null);
        cm.__add(hlActionExit1, 2000, null);
        cm.__add(hlAction2, 0, null);
        cm.__add(hlActionExit, 0, null);
        // cm.__add(new HlAction(() => csAthion(owen)), 0, null);

    }


    public static void csAthion(Hero owen)
    {
        //owen.spr.get_anim().play("teleport".AsHaxeString(), null, null);
        Log.Debug("已经执行动画");
    }



}