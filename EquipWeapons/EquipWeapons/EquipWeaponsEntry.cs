using dc.en;
using ModCore.Events.Interfaces.Game;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;

namespace EquipWeapons;

public class EquipWeaponsEntry(ModInfo info) : ModBase(info),
    IOnGameEndInit
{
    public override void Initialize()
    {
        Logger.Information("EquipWeapons:Hello DCCM!");
        Hook_Hero.initGfx += Hook_Hero_initGfx;
    }

    private void Hook_Hero_initGfx(Hook_Hero.orig_initGfx orig, Hero self)
    {
        orig(self);
        _ = new EquipProcess(self._level, Logger, self);
    }

    void IOnGameEndInit.OnGameEndInit()
    {
        var res = Info.ModRoot.GetFilePath("res.pak");
        FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
    }
}
