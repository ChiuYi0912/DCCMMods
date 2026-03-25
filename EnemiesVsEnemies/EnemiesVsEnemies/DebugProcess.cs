using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.en;
using dc.level;
using dc.pr;
using dc.ui.pause;
using dc.ui.sel;
using EnemiesVsEnemies.Inter;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.UI.Utilities;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Utilities;
using Serilog;

namespace EnemiesVsEnemies.Debug
{
    public static class DebugProcess
    {
        [ConsoleMethod("ui", "EnemiesVsEnemiesUI Debug")]
        public static void buildprocess(TextWriter writer)
        {
            //var sel = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager());
            Hero hero = Game.Class.ME.hero;
            var gui = new TeamSelector(hero._level, hero.cx, hero.cy);
            gui.init();

        }
    }
}