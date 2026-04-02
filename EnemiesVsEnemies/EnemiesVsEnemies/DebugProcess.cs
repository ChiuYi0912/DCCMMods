using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.level;
using dc.pr;
using dc.tool;
using dc.ui.icon;
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

            Hero hero = Game.Class.ME.hero;
        }

        [ConsoleMethod("remove-team-list", "移除关卡中的所有队伍触发器")]
        public static void removeTeamSelectorByList(TextWriter writer)
        {

        }

        [ConsoleMethod("remove-team-id", "移除关卡中指定id队伍触发器")]
        public static void removeTeamSelectorBykey(TextWriter writer, string id)
        {

        }
    }
}