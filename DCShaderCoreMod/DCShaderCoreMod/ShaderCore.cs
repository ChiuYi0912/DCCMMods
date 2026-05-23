using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using dc;
using dc.hxd;
using dc.hxsl;
using dc.tool.mod;
using dc.ui;
using DCShaderCore.Cache;
using DCShaderCore.Registry;
using DCShaderCore.Test;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Menu;
using ModCore.Mods;
using ModCore.Modules;
using Serilog;

namespace DCShaderCore;

public class ShaderCore(ModInfo info) : ModBase(info),
    IOnAfterPauseMenuBuild,
    IOnHookInitialize
{
    public static ShaderCore Instance { get; private set; } = default!;
    public static ILogger Modlogger = default!;
    public static ModInfo modInfo = default!;
    public ShaderCacheCore Cache { get; private set; } = default!;
    public ShaderRegistry Registry { get; private set; } = default!;

    public override void Initialize()
    {
        Info.Version = "0.0.1";
        modInfo = Info;
        Instance = this;
        Modlogger = Logger;
        Registry = new ShaderRegistry();
        Cache = new ShaderCacheCore(Registry);

        EventSystem.BroadcastEvent<IOnHookInitialize>();
    }

    public RuntimeShader GetShader(ShaderList list) => Cache.GetShader(list);

    public void RegisterShader(string id, Shader shader) => Registry.Register(id, shader);

    void IOnAfterPauseMenuBuild.OnAfterPauseMenuBuild(Pause pause)
    {
      
    }

    void IOnHookInitialize.HookInitialize()
    {
        Hook__Boot.initRes += Hook__Boot_initres;
    }

    private void Hook__Boot_initres(Hook__Boot.orig_initRes orig)
    {
        orig();
        Logger.Information("Load CachePak");
        var res = Info.ModRoot.GetFilePath("ShaderCacheRes.pak");
        FsPak.Instance.FileSystem.loadPak(res.ToHaxeString());
        Res.Class.load("DCShaderCore.cache".ToHaxeString());
    }
}
