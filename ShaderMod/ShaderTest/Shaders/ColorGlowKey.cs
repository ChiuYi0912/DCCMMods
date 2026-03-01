using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.h3d.mat;
using dc.hxsl;

namespace ShaderTest.Shaders
{
    public class ColorGlowKey:dc.h3d.shader.ScreenShader
    {
        public float time__ = 0;
        public Texture texture__ = null!;

        public ColorGlowKey(Texture texture)
        {
            this.texture__ = texture;
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
                case 0:
                    return this.flipY__;
                case 1:
                    return this.time__;
                case 2:
                    return this.texture__;
                
                default:
                    return null;
            }
        }
        public override double getParamFloatValue(int index)
        {
            switch (index)
            {
                case 0: 
                    return this.flipY__;
                case 1:
                    return this.time__;
                default:
                    return 0.0;
            }
        }
    }
}