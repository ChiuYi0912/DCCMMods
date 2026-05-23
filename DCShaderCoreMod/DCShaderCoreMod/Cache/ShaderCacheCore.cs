using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using dc;
using dc.haxe.ds;
using dc.hl.types;
using dc.hxsl;
using dc.tool;
using DCShaderCore.Compilation;
using DCShaderCore.Registry;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Storage;
using ModCore.Utilities;
using Serilog;
using Serilog.Core;

namespace DCShaderCore.Cache;

public class ShaderCacheCore :
    IEventReceiver,
    IOnHookInitialize
{
    public dc.hxsl.CacheFile Cache { get; private set; } = default!;
    public Config<ShaderCoreConfig> Config { get; } = new("shaderCore");
    public ILogger logger = ShaderCore.Modlogger;
    private readonly ShaderRegistry registry;
    private ShaderLinker? linker;

    public string MODPath = ShaderCore.modInfo.ModRoot.FullPath;
    public const string ShaderCacheName = "DCShaderCore.cache";
    public const string ShaderSRCName = "DCShaderCore.src";

    public ShaderCacheCore(ShaderRegistry Registry)
    {
        EventSystem.AddReceiver(this);
        registry = Registry;
    }

    public void InitializeShaderCache()
    {
        dc.String cac = System.IO.Path.Combine(MODPath, ShaderCacheName).ToHaxeString();
        dc.String src = System.IO.Path.Combine(MODPath, ShaderSRCName).ToHaxeString();
        dc.hxsl.CacheFile.Class.USE_RESOURCE = true;
        Cache = new dc.hxsl.CacheFile(true, Ref<bool>.In(true));
        // Cache.file = MODPath.ToHaxeString();
        // Cache.sourceFile = ShaderCacheName.Add_TwoHaxeStrings(".gl");
        linker = new ShaderLinker(Cache);
        //dc.hxsl.CacheFile.Class.FILENAME = ShaderCacheName.ToHaxeString();
    
        Cache.onNewShader = r =>
        {
            logger.Information($"[DCShaderCore] Compiled: {r.signature}");
        };

        Cache.onMissingShader = list =>
        {
            logger.Information("[DCShaderCore] Missing shader, falling back to default.");
            var errorData = ShaderTool.Class.getErrorDataFromShaderList(list);
            var shaderLinker = errorData.shaderLinker;
            var output = new Output.Value("output.color".ToHaxeString(), null);
            var vars = (ArrayObj)ArrayUtils.CreateDyn().array;
            vars.pushDyn(output);

            var fallback = ShaderTool.Class.createFallbackShaderList(Cache.getLinkShader(vars));
            return Cache.link(fallback);
        };
        dc.hxsl.Cache.Class.set(Cache);
        var shaderList = Cache.runtimeShaders.copy();
        Assets.Class.shaderQueue = shaderList;
        foreach (var entry in registry.GetEntries())
        {
            linker.CompileAndCache(entry.Shader);
        }
    }

    public RuntimeShader GetShader(ShaderList list) => Cache.compileRuntimeShader(list);

    public void Reload()
    {
        Cache.save();
        Cache.load();
        var shaderList = Cache.runtimeShaders.copy();
        Assets.Class.shaderQueue = shaderList;
    }

    void IOnHookInitialize.HookInitialize()
    {
        Hook__Assets.preloadShaders += PreloadShaders;
        Hook__CacheFile.__constructor__ += Hook__CacheFile__constructor__;
    }

    private void Hook__CacheFile__constructor__(Hook__CacheFile.orig___constructor__ orig, dc.hxsl.CacheFile arg1, bool allowCompile, Ref<bool> recompileRT)
    {
        bool recompileRT2 = recompileRT.IsNull == false && recompileRT.value;
        if (arg1.onMissingShader == null)
        {
            HlFunc<RuntimeShader, ShaderList> onMissingShader = new (arg1.onMissingShader!);
            arg1.onMissingShader = onMissingShader;
        }
        if (arg1.onNewShader == null)
        {
            HlAction<RuntimeShader> onNewShader = new(arg1.onNewShader!);
            arg1.onNewShader = onNewShader;
        }

        arg1.allSources = new StringMap();
        arg1.compiledSources = new StringMap();

        arg1.linkers = (ArrayObj)ArrayUtils.CreateDyn().array;
        arg1.runtimeShaders = (ArrayObj)ArrayUtils.CreateDyn().array;
        arg1.shaders = new StringMap();
        arg1.waitCount = 0;

        HlAction<dc.hxsl.Cache> hl = (HlAction<dc.hxsl.Cache>)dc.hxsl.Cache.Class.__constructor__;
        hl.Invoke(arg1);

        arg1.allowCompile = allowCompile;
        arg1.recompileRT = recompileRT2;

        dc.hxsl.CacheFile.Class.FILENAME = ShaderCacheName.ToHaxeString();
        dc.String file = dc.hxsl.CacheFile.Class.FILENAME;
        arg1.file = file;
        arg1.sourceFile = dc.String.Class.__add__(dc.String.Class.__add__(file, ".".ToHaxeString()), arg1.getPlatformTag());
        arg1.load();
    }

    private void PreloadShaders(Hook__Assets.orig_preloadShaders orig) { }

}

public class ShaderCoreConfig { }
