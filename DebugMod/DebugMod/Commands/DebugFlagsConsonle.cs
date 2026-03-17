using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.en;
using dc.pr;
using IngameDebugConsole;

namespace DebugMod.Commands
{
    public static class DebugFlagsConsonle
    {
        [ConsoleMethod("add-flag-area", "近战攻击范围显示")]
        public static void Addarea(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("area".ToHaxeString(), null);

        [ConsoleMethod("add-flag-nofx", "移除关卡烟雾")]
        public static void Addnofx(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("nofx".ToHaxeString(), null);

        [ConsoleMethod("add-flag-pet", "宠物调试")]
        public static void Addper(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("pet".ToHaxeString(), null);

        [ConsoleMethod("add-flag-out", "输出怪物状态")]
        public static void Addout(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("out".ToHaxeString(), null);

        [ConsoleMethod("add-flag-itemid", "输出物品状态")]
        public static void Additemid(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("itemId".ToHaxeString(), null);

        [ConsoleMethod("add-flag-aggTel", "无效果")]
        public static void addaggTel(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("aggTel".ToHaxeString(), null);

        [ConsoleMethod("add-flag-rally", "输出攻击")]
        public static void Addrally(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("rally".ToHaxeString(), null);

        [ConsoleMethod("add-flag-active", "标记所有可交互实体（暂时无效果）")]
        public static void Addactive(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("active".ToHaxeString(), null);

        [ConsoleMethod("add-flag-allCursedLevels", "接下来的所有关卡都将被诅咒")]
        public static void AddallCursedLevels(TextWriter writer) => dc.ui.Console.Class.ME.flags.exists("allCursedLevels".ToHaxeString());

        [ConsoleMethod("add-flag-roll", "延长hero翻滚")]
        public static void Addroll(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("roll".ToHaxeString(), null);

        [ConsoleMethod("add-flag-noTrack", "禁用摄像机跟随")]
        public static void AddnoTrack(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("noTrack".ToHaxeString(), null);

        [ConsoleMethod("add-flag-hideCineBars", "隐藏过场动画黑条")]
        public static void AddhideCineBars(TextWriter writer) => dc.ui.Console.Class.ME.flags.set("hideCineBars".ToHaxeString(), null);


        [ConsoleMethod("addall-flags", "添加所有flags")]
        public static void Addallfalgs(TextWriter writer)
        {
            dc.ui.Console.Class.ME.flags.set("area".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("aggTel".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("out".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("nofx".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("roll".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("itemId".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("rally".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("pet".ToHaxeString(), null);
            dc.ui.Console.Class.ME.flags.set("active".ToHaxeString(), null);
        }

        [ConsoleMethod("removeall-flags", "移除所有flags")]
        public static void removeallfalgs(TextWriter writer)
        {
            dc.ui.Console.Class.ME.flags.remove("area".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("aggTel".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("out".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("nofx".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("roll".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("itemId".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("rally".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("pet".ToHaxeString());
            dc.ui.Console.Class.ME.flags.remove("active".ToHaxeString());
        }
    }
}