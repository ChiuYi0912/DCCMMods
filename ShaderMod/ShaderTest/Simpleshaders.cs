using dc.en;
using dc.h3d.pass;
using dc.h3d.shader;
using dc.hl.types;
using dc.hxsl;
using dc.tool;
using dc.tool.mod;
using Hashlink.Proxy.Objects;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;
using ShaderTest.shaders;
using ShaderTest.Shaders;

namespace ShaderTest;

public class Simpleshaders : ModBase,
    IOnHeroUpdate,
    IOnGameEndInit
{
    public Simpleshaders(ModInfo info) : base(info)
    {
    }

    public override void Initialize()
    {
        base.Initialize();
        Hook_CacheFile.addNewShader += Hook__CacheFile_initialize;

    }

    private void Hook__CacheFile_initialize(Hook_CacheFile.orig_addNewShader orig, CacheFile self, RuntimeShader s)
    {

    }

    void IOnHeroUpdate.OnHeroUpdate(double dt)
    {
        if (Utils.DebungConsole.clowkey != null)
        {
            Utils.DebungConsole.clowkey.time__ = (float)dt;
        }
        if (Utils.DebungConsole.hotlie != null)
        {
            Utils.DebungConsole.hotlie.time__ = dt;
        }

    }

    void IOnGameEndInit.OnGameEndInit()
    {
        var res = Info.ModRoot!.GetFilePath("res.pak");
        FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
        // var json = CDBManager.Class.instance.getAlteredCDB();
        // dc.Data.Class.loadJson(
        //    json,
        //    default);
    }

}
