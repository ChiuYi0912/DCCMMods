using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.h3d;
using dc.h3d.mat;
using dc.hl.types;
using dc.hxd.impl;
using dc.hxsl;
using dc.shader;
using dc.tool.weap.sh;
using HaxeProxy.Runtime;
using HaxeProxy.Runtime.Internals;
using ModCore.Utilities;
using Serilog;

namespace SettingsMod.shaders
{
    public class RGBReplaceShader : dc.h3d.shader.ScreenShader
    {

        public Texture texture__ = null!;
        public RGBReplaceShader(Texture texture)
        {
            texture__ = texture;
        }
    }
}