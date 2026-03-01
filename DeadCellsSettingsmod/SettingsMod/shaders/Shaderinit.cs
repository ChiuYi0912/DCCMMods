using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.h3d.mat;
using ModCore.Utilities;
using Serilog;
using static dc.hxsl.Type;
using static dc.hxsl.VarKind;

namespace SettingsMod.shaders
{
    public static class Shaderinit
    {
        public static RGBReplaceShader shaderinit(Texture texture)
        {
            RGBReplaceShader shader = new RGBReplaceShader(null!);

            // var shaderdata = shader.shader.data;

            // for (int i = 0; i < shader.shader.data.vars.length; i++)
            // {
            //     dynamic v = shaderdata.vars.getDyn(i);
            //     if (v == null)
            //         continue;
            //     if (v.name.ToString() == "texture")
            //     {
            //         Reflect.Class.setProperty(shader, "texture".AsHaxeString(), texture);

            //         Log.Debug($"已经加入：{v.name.ToString()}");
            //     }

            // }

            return shader;
        }
    }
}