using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.h2d;
using dc.h3d.mat;
using dc.h3d.pass;
using dc.h3d.shader;
using dc.hxsl;
using dc.shader;
using ModCore.Events;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Utilities;

namespace ShaderTest.Shaders
{
    public class SimpleShader :
    ScreenShader

    {
        public dc.h3d.mat.Texture texture__ = null!;
        public float time__ = 0;
        public float waveStrength__ = 0;
        public float waveSpeed__ = 0;
        public SimpleShader(dc.h3d.mat.Texture texture)
        {
            texture__ = texture;
            this.waveSpeed__ = 10;
            this.waveStrength__ = 0.5f;
        }

        public override double getParamFloatValue(int index)
        {
            switch (index)
            {
                case 0: return this.flipY__;
                case 1: return this.time__;
                case 2: return this.waveStrength__;
                case 3: return this.waveSpeed__;
                default:
                    return 0;
            }
            ;
        }

        public override dynamic getParamValue(int index)
        {
            switch ((index))
            {
                case 0: return this.flipY__;
                case 1: return this.time__;
                case 2: return this.waveStrength__;
                case 3: return this.waveSpeed__;
                case 4: return this.texture__;
                default:
                    return null!;
            }
            ;
        }

        public override void updateConstants(Globals globals)
        {
            base.constBits = 0;
            base.updateConstantsFinal(globals);
        }


    }
}