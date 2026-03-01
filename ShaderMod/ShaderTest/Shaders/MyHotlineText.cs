using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.h3d.mat;
using dc.hxd.impl;
using dc.hxsl;
using ModCore.Utilities;

namespace ShaderTest.Shaders
{
    public class MyHotlineText : dc.h3d.shader.ScreenShader
    {
        public new double flipY__ = 0.0f;
        public double time__ = 0.0f;
        public double depth__ = 0.0f;

        public MyHotlineText() { }
        public override void updateConstants(Globals globals)
        {
            base.constBits = 0;
            //globals.set("time".AsHaxeString(), this.time__);
            base.updateConstantsFinal(globals);
        }
        public override object? getParamValue(int index)
        {
            switch (index)
            {
                case 0: return this.flipY__;
                case 1: return this.time__;
                case 2: return this.depth__;
                default: return 0.0f;
            }
        }

        public override double getParamFloatValue(int index)
        {
            switch (index)
            {
                case 0: return this.flipY__;
                case 1: return this.time__;
                case 2: return this.depth__;
                default: return 0.0f;
            }
        }
    }
}