using dc;
using ModCore.Mods;
using ModCore.Events;
using MoreHeadsandskins.customHeadEntry;
using MoreHeadsandskins.customSkinEntry;
using MoreHeadsandskins.EntryHookInitialize;
using ModCore.Events.Interfaces.Game;
using ModCore.Modules;
using ModCore.Utilities;
using dc.tool.mod;


namespace MoreHeadsandskins;

public class Entry : ModBase,
    IOnAfterLoadingCDB,
    IOnEntryHookInitialize,
    IOnGameEndInit
{
    public Entry(ModInfo info) : base(info)
    {
    }

    public override void Initialize()
    {
        _ = new customHeadMian(this);
        _ = new customSkinMian(this);

        EventSystem.BroadcastEvent<IOnEntryHookInitialize, Entry>(this);
    }

    void IOnEntryHookInitialize.HookInitialize(Entry entry)
    {

    }

    void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
    {
       
    }

    void IOnGameEndInit.OnGameEndInit()
    {
        var res2 = Info.ModRoot!.GetFilePath("res.pak");
        FsPak.Instance.FileSystem.loadPak(res2.AsHaxeString());
        var json = CDBManager.Class.instance.getAlteredCDB();
        dc.Data.Class.loadJson(
           json,
           default);
    }
}
