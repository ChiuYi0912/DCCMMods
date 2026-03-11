
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Core.Utilities;
using ModCore;
using ModCore.Events;
using ModCore.Mods;
namespace LightOpt;

public class LightOptEntry(ModInfo Info) : ModBase(Info)
{
    public override void Initialize()
    {
        Logger.LogInformation("LightOpt:Hello DCCM!");
        _ = new LevelManager(this);
        EventSystem.BroadcastEvent<IOnHookInitialize, LightOptEntry>(this);
    }
}
