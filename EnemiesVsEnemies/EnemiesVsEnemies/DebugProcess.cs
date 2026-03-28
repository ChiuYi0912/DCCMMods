using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.en;
using dc.level;
using dc.pr;
using dc.tool;
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

        [ConsoleMethod("remove-team-list", "移除关卡中的所有队伍触发器")]
        public static void removeTeamSelectorByList(TextWriter writer)
        {
            var list = EnemiesVsEnemiesMod.GetHookManager().TeamSelectordummies;
            foreach (var item in list)
            {
                item.destroy();
            }
            foreach (var teams in EnemiesVsEnemiesMod.config.Value.Teams)
            {
                EnemiesVsEnemiesMod.GetTeamManager().RemoveTeam(teams.Key);
            }
        }

        [ConsoleMethod("remove-team-id", "移除关卡中指定id队伍触发器")]
        public static void removeTeamSelectorBykey(TextWriter writer, string id)
        {
            var key = EnemiesVsEnemiesMod.GetHookManager().TeamSelectorkeys;
            if (key.TryGetValue(id, out var team))
            {
                team.destroy();
                EnemiesVsEnemiesMod.config.Value.Teams.Remove(id);
                EnemiesVsEnemiesMod.config.Save();
            }
            else
            {
                writer.Write("请输入有效id");
            }
        }
    }
}