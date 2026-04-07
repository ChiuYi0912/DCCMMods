using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.NativeLib;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.haxe.format;
using dc.hl.types;
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
using Newtonsoft.Json.Linq;
using Serilog;

namespace EnemiesVsEnemies.Debug
{
    public static class DebugProcess
    {
        [ConsoleMethod("ui", "EnemiesVsEnemiesUI Debug")]
        public static void buildprocess(TextWriter writer)
        {
            var data = Data.Class.level.all.array as ArrayObj;

            if (data == null)
                return;
            var parallelDict = data.GetCachedDictionaryParallel();
            //var dat = data!.GetByIdParallel("PrisonStart");
            parallelDict.TryGetValue("PrisonStart", out var dynamic);
            dc.String str = JsonPrinter.Class.print(dynamic, null, null);
            string jsonstr = JToken.Parse(str.ToString()).ToString();
            EnemiesVsEnemiesMod.GetLogger.LogInformation($"{jsonstr}");
            JsonFormatter.jqFormatter(dynamic, EnemiesVsEnemiesMod.GetLogger);
        }

        [ConsoleMethod("cells", "移除关卡中的所有队伍触发器")]
        public static void removeTeamSelectorByList(TextWriter writer)
        {
            Hero hero = Game.Class.ME.hero;
            hero.cells = 10000;
        }

        public static SimpleSinPointer pointer = null!;

        [ConsoleMethod("sin", "启动正弦值生成器")]
        public static void StartSinPointer(TextWriter writer)
        {
            
        }
    }
}