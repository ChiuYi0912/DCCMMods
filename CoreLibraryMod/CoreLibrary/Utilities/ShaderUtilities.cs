using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.h3d.pass;
using dc.h3d.scene;
using dc.hl.types;
using dc.hxsl;
using dc.hxsl._CacheFile;
using HaxeProxy.Runtime;
using ModCore.Utilities;



namespace CoreLibrary.Utilities
{
    public class ShaderUtilities
    {
        private Globals globals = null!;
        private Shader outputShader = null!;
        public CacheFile? cache { get; private set; } = null!;

        public ShaderUtilities()
        {
            cache = Cache.Class.get() as CacheFile;
            //globals = Boot.Class.ME.s2d.ctx.manager.globals;
            DisableCompileLogs();
        }
        public void DisableCompileLogs()
        {
            if (cache != null)
                cache.onNewShader = new HlAction<RuntimeShader>((runtime) => { });
        }

        public ShaderList CompileShaderList(ShaderList userShaders)
        {
            if (cache == null) return null!;
            if (outputShader == null)
            {
                ArrayObj outputArray = (ArrayObj)ArrayUtils.CreateDyn().array;
                outputArray.push(new Output.Value("output.color".ToHaxeString(), null));
                outputShader = cache.getLinkShader(outputArray);
            }

            var nullShader = new NullShader();
            nullShader.updateConstants(globals);

            var chain = new ShaderList(nullShader, new ShaderList(outputShader, userShaders));

            cache.link(chain);

            return chain;
        }


        public ShaderList CompileShader(Shader s)
        {
            s.updateConstants(globals);
            return CompileShaderList(new ShaderList(s, null));
        }


        public void CompileShaders(params Shader[] shaders)
        {
            if (shaders == null || shaders.Length == 0) return;
            ShaderList list = null!;
            for (int i = 0; i < shaders.Length; i++)
            {
                if (shaders[i] != null)
                {
                    shaders[i].updateConstants(globals);
                    list = new ShaderList(shaders[i], list);
                }
            }
            if (list != null) CompileShaderList(list);
        }


        public void AddGlobalShader(RenderContext ctx, Shader shader)
        {
            if (ctx == null || shader == null) return;
            shader.updateConstants(globals);
            var node = new ShaderList(shader, null);
            if (ctx.extraShaders == null)
            {
                ctx.extraShaders = node;
            }
            else
            {
                var tail = ctx.extraShaders;
                while (tail.next != null) tail = tail.next;
                tail.next = node;
            }
        }

        public void RemoveGlobalShader(RenderContext ctx, Shader shader)
        {
            if (ctx?.extraShaders == null || shader == null) return;
            ShaderList prev = null!;
            var cur = ctx.extraShaders;
            while (cur != null)
            {
                if (cur.s == shader)
                {
                    if (prev == null) ctx.extraShaders = cur.next;
                    else prev.next = cur.next;
                    return;
                }
                prev = cur;
                cur = cur.next;
            }
        }

        public void ClearGlobalShaders(RenderContext ctx)
        {
            if (ctx != null) ctx.extraShaders = null;
        }
    }
}
