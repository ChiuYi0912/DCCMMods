using ModCore.Mods;

namespace EquipWeapons;

public class EquipWeaponsEntry : ModBase
{
    public EquipWeaponsEntry(ModInfo info) : base(info)
    {
    }

    public override void Initialize()
    {
        Logger.Information("EquipWeapons:Hello DCCM!");
    }
}
