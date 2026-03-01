using dc.h3d.mat;
using dc.hxsl;


namespace ShaderTest.shaders
{
    public class RGBReplaceShader : dc.h3d.shader.ScreenShader
    {
        public Texture texture__ = null!;
        public RGBReplaceShader(Texture texture)
        {
            texture__ = texture;
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
                    return this.texture__;
                default:
                    return null;
            }
        }
        public override double getParamFloatValue(int index)
        {
            return base.getParamFloatValue(index);
        }
    }
}