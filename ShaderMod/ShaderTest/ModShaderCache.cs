using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.hl.types;
using dc.hxsl;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using Serilog.Core;

namespace ShaderTest
{
    public class ModShaderCache
    {
        public dc.hxsl.CacheFile Cache = default!;
        public Config<TestConfig> Config = new("shaderCache");
        public void Initialize()
        {
            
            Cache = new dc.hxsl.CacheFile(true, Ref<bool>.In(true));
            Cache.file = Config.ConfigPath.Add_TwoHaxeStrings("_mod_shaders.cache");
            Cache.sourceFile = Config.ConfigPath.Add_TwoHaxeStrings("_mod_shaders.src");

            Cache.onNewShader = r =>
            {
                Logger.None.Debug($"[Mod] Compiled shader: {r.signature}");
            };
            Cache.onMissingShader = list =>
            {
                Logger.None.Debug("[Mod] Missing shader, falling back to default.");
                var errorDataFromShaderList = ShaderTool.Class.getErrorDataFromShaderList(list);
                dc.String shaderLinker = errorDataFromShaderList.shaderLinker;
                Output output = new Output.Value("output.color".ToHaxeString(), null);
                ArrayObj vars = (ArrayObj)ArrayUtils.CreateDyn().array;
                vars.pushDyn(output);
                ShaderList shaders = ShaderTool.Class.createFallbackShaderList(Cache.getLinkShader(vars));
                return Cache.link(shaders);
            };

            Cache.load();
            var shaderList = Cache.runtimeShaders.copy();
            Assets.Class.shaderQueue = shaderList;
            Cache.save();
        }

        public RuntimeShader GetShader(ShaderList list)
        {
            return Cache.compileRuntimeShader(list);
        }
    }
}