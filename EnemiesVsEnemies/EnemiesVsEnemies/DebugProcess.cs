using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.pr;
using EnemiesVsEnemies.UI;
using IngameDebugConsole;

namespace EnemiesVsEnemies.Debug
{
    public static class DebugProcess
    {
        [ConsoleMethod("ui", "EnemiesVsEnemiesUI Debug")]
        public static void buildprocess(TextWriter writer)
        {
            var ui = new CricketSelectorGui(Game.Class.ME.hero._level, null!);
        }
    }
}