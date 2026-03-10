using LightOpt.Core.Utilities;
using Midjourney.Core.Interfaces;
using ModCore;
using ModCore.Events;
using ModCore.Mods;
namespace LightOpt;

public class LightOptEntry(ModInfo Info) : ModBase(Info)
{
    public static ModCore.Storage.Config<LightOpt.Core.Configuration.CoreCfig> GetConfig = new("LightOptCoreConfig");
    public override void Initialize()
    {
        Logger.LogInformation("LightOpt:Hello DCCM!");
        _ = new LevelManager(this);
        EventSystem.BroadcastEvent<IOnHookInitialize, LightOptEntry>(this);
    }
}
