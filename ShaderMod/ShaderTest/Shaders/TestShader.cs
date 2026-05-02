using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.hl;
using dc.hxsl;

namespace ShaderTest.Shaders
{
    public class TestShader : dc.h3d.shader.ScreenShader
    {
        public TestShader()
        {
        }
        public class TestShaderStaticClass : Class
        {
            public SharedShader? _SHADER;
            public dc.String SRC = src;
        }

        public static dc.String src = "oy4:namey18:shaders.TestShadery4:funsaoy3:retjy9:hxsl.Type:0:0y4:kindjy17:hxsl.FunctionKind:1:0y3:refoR0y8:fragmenty2:idi-74R5jy12:hxsl.VarKind:6:0y4:typejR4:14:1aoR3r3y4:argsahghgy4:exproy1:poy3:mini190y3:maxi314y4:filey29:src%2Fshaders%2FTestShader.hxgy1:tr3y1:ejy13:hxsl.TExprDef:4:1aoR14oR15i196R16i260R17R18gR19r3R20jR21:7:2oR0y10:brightnessR9i-75R5jR10:4:0R11jR4:3:0goR14oR15i213R16i259R17R18gR19r18R20jR21:8:2oR14oR15i213R16i216R17R18gR19jR4:14:1aoR3r18R12aoR0y1:aR11jR4:5:2i3jy12:hxsl.VecType:1:0goR0y1:bR11r28ghghR20jR21:2:1jy12:hxsl.TGlobal:29:0gaoR14oR15i217R16i231R17R18gR19jR4:5:2i3r27R20jR21:9:2oR14oR15i217R16i227R17R18gR19jR4:5:2i4r27R20jR21:1:1oR0y10:pixelColorR9i-73R5r17R11r39ggajy14:hxsl.Component:0:0jR28:1:0jR28:2:0hgoR14oR15i233R16i258R17R18gR19jR4:5:2i3r27R20jR21:8:2oR14oR15i233R16i237R17R18gR19jR4:14:1ahR20jR21:2:1jR26:39:0gaoR14oR15i238R16i243R17R18gR19r18R20jR21:0:1jy10:hxsl.Const:3:1d0.299goR14oR15i245R16i250R17R18gR19r18R20jR21:0:1jR29:3:1d0.587goR14oR15i252R16i257R17R18gR19r18R20jR21:0:1jR29:3:1d0.114ghghggoR14oR15i265R16i308R17R18gR19jR4:5:2i3r27R20jR21:5:3jy16:haxe.macro.Binop:4:0oR14oR15i265R16i279R17R18gR19r74R20jR21:9:2oR14oR15i265R16i275R17R18gR19r39R20jR21:1:1r40gar43r44r45hgoR14oR15i282R16i308R17R18gR19jR4:5:2i3r27R20jR21:8:2oR14oR15i282R16i286R17R18gR19r53R20jR21:2:1r54gaoR14oR15i287R16i297R17R18gR19r18R20jR21:1:1r16goR14oR15i299R16i302R17R18gR19r18R20jR21:0:1jR29:3:1d0goR14oR15i304R16i307R17R18gR19r18R20jR21:0:1jR29:3:1d0ghgghgR12ahghy4:varsaoR0y5:inputR9i-71R5jR10:1:0R11jR4:13:1aoR0y2:uvR9i-72y6:parentr106R5r107R11jR4:5:2i2r27ghgr40r5hg".ToHaxeString();

        public new static TestShaderStaticClass Class { get; } = new();

        public override dynamic getParamValue(int index)
        {
            return null!;
        }

        public override double getParamFloatValue(int index)
        {
            return 0;
        }

        public override void updateConstants(Globals globals)
        {
            this.constBits = 0;
            this.updateConstantsFinal(globals);
        }
    }
}