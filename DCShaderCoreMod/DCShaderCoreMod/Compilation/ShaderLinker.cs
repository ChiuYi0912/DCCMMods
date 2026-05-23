using CoreLibrary.Core.Extensions;
using dc;
using dc.hl.types;
using dc.hxsl;
using dc.hxsl._CacheFile;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace DCShaderCore.Compilation;

public class ShaderLinker
{
    private readonly dc.hxsl.CacheFile cache;
    private Shader? outputShader;

    public ShaderLinker(dc.hxsl.CacheFile cachefile)
    {
        cache = cachefile;
    }

    public Shader GetOutputShader()
    {
        if (outputShader != null)
            return outputShader;

        ArrayObj outputArray = (ArrayObj)ArrayUtils.CreateDyn().array;
        outputArray.push(new Output.Value("output.color".ToHaxeString(), null));
        outputShader = cache.getLinkShader(outputArray);
        return outputShader;
    }

    public RuntimeShader CompileAndCache(Shader shader)
    {
        var output = GetOutputShader();
        var chain = new ShaderList(shader, new ShaderList(output, new ShaderList(new NullShader(), null)));
        return cache.compileRuntimeShader(chain);
    }
}
