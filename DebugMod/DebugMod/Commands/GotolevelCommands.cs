using dc.en;
using IngameDebugConsole;
using ModCore.Utilities;
using System.Collections.Generic;
using System.IO;

namespace DebugMod.Commands
{
    public static class GotolevelCommands
    {
        [ConsoleMethod("show-lvls", "显示关卡编号")]
        public static void ShowAllLevels(TextWriter writer)
        {
            if (allLevels.Count == 0)
            {
                getlvlid();
            }

            for (int i = 0; i < allLevels.Count; i++)
            {
                writer.Write($"{i + 1}. {allLevels[i]}");
            }

        }
        [ConsoleMethod("goto-lvl", "前往编号对应关卡", "关卡编号")]
        public static void Gotolevel(TextWriter writer, int levelIndex)
        {
            if (allLevels.Count == 0)
            {
                getlvlid();
            }


            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.Write($"请保证细胞人实例存在！");
                return;
            }

            int zeroBasedIndex = levelIndex - 1;
            if (zeroBasedIndex >= 0 && zeroBasedIndex < allLevels.Count)
            {
                string levelName = allLevels[zeroBasedIndex];
                writer.Write($"正在前往到关卡: {levelIndex}. {levelName}");

                dc.cine.LevelTransition.Class.@goto(levelName.AsHaxeString());


            }
            else
            {
                writer.Write($"关卡编号无效。请输入 1 到 {allLevels.Count} 之间的数字。");
            }

        }

        private static readonly List<string> allLevels = [];


        private static void getlvlid()
        {
            var data = dc.Data.Class.level.all.get_length();
            for (int i = 0; i < data; i++)
            {
                var levelid = dc.Data.Class.level.all.array.getDyn(i).id;
                allLevels.Add(levelid.ToString());
            }

        }
    }
}