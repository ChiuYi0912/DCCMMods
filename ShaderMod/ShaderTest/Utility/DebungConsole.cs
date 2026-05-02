using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.h3d.mat;
using dc.h3d.pass;
using dc.h3d.shader;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.hxsl;
using dc.libs.heaps.slib;
using dc.pr;
using dc.shader;
using dc.ui;
using IngameDebugConsole;
using ModCore.Modules;
using ModCore.Utilities;
using ShaderTest.shaders;
using ShaderTest.Shaders;
using ShaderTest.Utils;

namespace ShaderTest.Utils
{
    public static class DebungConsole
    {
        public static Bitmap? spr { get; set; }
        public static Tile? t { get; set; }
        public static SimpleShader? smpshader { get; set; }
        public static dc.h3d.mat.Texture? texture { get; set; }
        public static ColorGlowKey? clowkey { get; set; }
        public static MyHotlineText? hotlie { get; set; }

        [ConsoleMethod("add-bitmap", "")]
        public static void ApplyingBimapToHero(TextWriter writer)
        {
            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;

            SpriteLib spriteLib = Assets.Class.lib.get("atlas/testFlag.atlas".AsHaxeString());
            texture = Assets.Class.getNoiseTexture(new NoiseTexture.PerlinNoise());



            t = spriteLib.pages.getDyn(0) as Tile;

            spr = new Bitmap(t, HUD.Class.ME.topRightFlowT);

            // var outline = new MYOutline(spr., CreateColor.ColorFromHex("#ff00e6"));
            // spr.addShader(outline);
            //spr.addShader(new dc.shader.Base2d());
            spr.addShader(new TestShader());
        }


        [ConsoleMethod("add-shader", "")]
        public static void ApplyingShaderToHero(TextWriter writer)
        {
            if (t == null || spr == null)
            {
                writer.Write($"Tile是否为空:{t == null},bitmap是否为空:{spr == null}");
                return;
            }

            dc.ui.Text uit = Assets.Class.makeText("testshader".AsHaxeString(), null, null, spr);

            // hotlie = new MyHotlineText();
            // uit.addShader(hotlie);
        }
    }
}