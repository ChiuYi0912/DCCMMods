using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.h3d.mat;
using dc.hl;
using dc.hxsl;
using ModCore.Utilities;
using PopDamage.shader.SRC;

namespace PopDamage.shader
{
    public class SimpleRevealShader : dc.h3d.shader.ScreenShader
    {
        public class SimpleRevealShaderStaticClass : Class
        {
            public SharedShader? _SHADER;
            public dc.String SRC = ShaderSRC.RevealSRC.AsHaxeString();
        }
        public new static SimpleRevealShaderStaticClass Class { get; } = new();

        public Texture texture__ = null!;
        public double progress__ = 0;
        public double depth__ = 0;
        public double glowIntensity__ = 0;
        public SimpleRevealShader(Texture texture_)
        {
            texture__ = texture_;
        }

        public override void updateConstants(Globals globals)
        {
            base.constBits = 0;
            base.updateConstantsFinal(globals);
        }
        public override object? getParamValue(int index)
        {
            switch (index)
            {
                case 0: return flipY__;
                case 1: return texture__;
                case 2: return progress__;
                case 3: return depth__;
                case 4: return glowIntensity__;
                default:
                    return null;
            }
            ;
        }

        public override double getParamFloatValue(int index)
        {
            switch (index)
            {
                case 0: return flipY__;
                case 2: return progress__;
                case 3: return depth__;
                case 4: return glowIntensity__;
                default:
                    return 0;
            }
        }
    }
}