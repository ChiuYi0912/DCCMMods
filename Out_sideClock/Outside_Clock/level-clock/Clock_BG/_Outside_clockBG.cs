using dc;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.level.disp;
using dc.pr;
using dc.tool.quadTree;
using ModCore.Utilities;
using Serilog;

namespace Outside_Clock.level_clock.Clock_BG
{
    public static class _Outside_clockBG
    {
        public static void _Outside_clockBG_Initialize(Outside_clockBG outclock, Level p, LevelMap map)
        {
            p.map.biome.id = "Template".AsHaxeString();
            // Log.Debug("测试成功！！！！");
            // JunkMode junkMode = new JunkMode.SameInsideOutside();
            // outclock.junkMode = junkMode;
            // outclock.fxTorch = "fxTorchTurquoise".AsHaxeString();
            // outclock.fxCauldron = "fxTorchYellow".AsHaxeString();
            // outclock.fxBrasero = "fxTorchYellow".AsHaxeString();
        }

        public static void HookInitialize()
        {
            dc.level.disp.Hook__Template.__constructor__ += DC_LEVEL_DISP_HOOK_TEMPLATE_INITIALIZE;
        }


        private static void DC_LEVEL_DISP_HOOK_TEMPLATE_INITIALIZE(Hook__Template.orig___constructor__ orig, dc.level.disp.Template arg1, Level p, LevelMap map)
        {
            Outside_clockBG outside = new Outside_clockBG(p, map, null);
            //orig(arg1, p, map);
        }
    }
}