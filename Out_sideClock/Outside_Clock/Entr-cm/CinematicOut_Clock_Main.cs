using dc;
using dc.cine;
using dc.en;
using dc.en.inter;
using ModCore.Events.Interfaces.Game;
using ModCore.Storage;
using Serilog;

namespace Outside_Clock;

public class CinematicOut_Clock_Main : IOnGameInit
{

    private static bool a = false;
    public static Config<Out_Clock_Config> Out_Clock_Enter_save { get; } = new("Out_Clock_Enter");

    internal static void Hook_HiddenTrigger_trigger(Hook_HiddenTrigger.orig_trigger orig, HiddenTrigger self, Entity dh)
    {

        Hero owen = ModCore.Modules.Game.Instance.HeroInstance!;
        orig(self, dh);
        if (self.genericEventId == null)
        {
            return;
        }
        if (self.genericEventId != null)
        {
            if (Out_Clock_Enter_save.Value.Out_Clock_Enter == false)
            {
                Log.Debug("genericEventId不为空");
                if (self.genericEventId.ToString() == "ow111")
                {
                    Log.Debug("获取到ow111");
                    // GotoClockTower gotoClockTower = new GotoClockTower((Hero)dh);
                    Out_Clock_Enter_save.Value.Out_Clock_Enter = true;
                    Out_Clock_Enter_save.Save();
                }
            }


            if (self.genericEventId.ToString() == "ow2")
            {
                if (a == false)
                {
                    if (self.genericEventId.ToString() == "ow2")
                    {

                        Log.Debug("获取到ow2");
                    }
                    a = true;
                }
            }
            return;
        }
    }


    void IOnGameInit.OnGameInit()
    {
        Out_Clock_Enter_save.Value.Out_Clock_Enter = false;
        Out_Clock_Enter_save.Save();
        Log.Debug("已初始化过场动画");
    }
}
