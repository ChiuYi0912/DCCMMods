using IngameDebugConsole;
using ModCore.Modules;
using ModCore.Utilities;

namespace EquipWeapons.Utility
{
    public static class DebungConsole
    {
        [ConsoleMethod("process", "")]
        public static void TEST(TextWriter writer)
        {
            var owen = Game.Instance.HeroInstance;
            var process = new EquipProcess(owen!._level, Game.Logger,owen);
        }
    }
}