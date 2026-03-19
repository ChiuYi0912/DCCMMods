using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.libs;
using dc.pr;
using dc.tool;
using dc.ui;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using Midjourney.Entities.Mob.FlyMob;
using ModCore.Utilities;
using Serilog;

namespace DebugMod.Commands.activateDebug
{
    public static class Activate
    {
        [ConsoleMethod("perk", "打开变异菜单")]
        public static void openperks(TextWriter writer)
        {
            if (Game.Class.ME == null)
            {
                writer.Write("请保证细胞人实例存在！");
                return;
            }
            _ = new PerkSelect(Game.Class.ME.hero, Game.Class.ME.hero);
        }


        [ConsoleMethod("tires", "例:tires 2 4 5", "设置3种卷轴个数")]
        public static void settiers(TextWriter writer, int b, int s, int t)
        {
            if (Game.Class.ME == null)
            {
                return;
            }

            Hero hero = Game.Class.ME.hero;

            ProcessAttributeUpgrade(hero, "BrutalityUp", b);
            ProcessAttributeUpgrade(hero, "SurvivalUp", s);
            ProcessAttributeUpgrade(hero, "TacticUp", t);

            hero.computeTiers();
            HUD.Class.ME.setBrutalityTier(b, Ref<bool>.Null);
            HUD.Class.ME.setTacticTier(t, Ref<bool>.Null);
            HUD.Class.ME.setSurvivalTier(s, Ref<bool>.Null);
            hero.onEquipedItemsChange(Ref<bool>.Null, Ref<bool>.Null, Ref<bool>.Null);
        }


        public static void ProcessAttributeUpgrade(Hero hero, string itemName, int targetCount)
        {
            if (targetCount <= 0) return;


            hero.inventory.removeAll(itemName.AsHaxeString());

            for (int i = 0; i < targetCount - 1; i++)
                hero.inventory.add(InventItem.Class.fromItem(itemName.AsHaxeString(), null));

        }

        [ConsoleMethod("nt", "英雄不会被视为目标")]
        public static void Hero(TextWriter writer)
        {
            if (Game.Class.ME == null)
            {
                writer.Write("请保证细胞人实例存在！");
                return;
            }
            Hero hero = Game.Class.ME.hero;
            bool currentValue = hero._targetable;
            bool newValue = !currentValue;
            hero.set_targetable(newValue);
            writer.WriteLine($"已将targetable从 {currentValue} 切换为: {newValue}");
        }





        [ConsoleMethod("test", "作者测试用的")]
        public static void ChangeHeroShaderColor(TextWriter writer)
        {
            Hero hero = Game.Class.ME.hero;
            var debug = hero._level.GetLevelDisplaySafe().debug;
            debug.lineStyle(Ref<double>.In(1),Ref<int>.In(CreateColor.ColorFromHex("#ffffff")),Ref<double>.In(1));

            var x1 = (hero.cx - hero.xr) * 24;
            var y1 = (hero.cy - hero.yr) * 24;
            var x2 = (hero.cx + hero.xr) * 24;
            var y2 = (hero.cy + hero.yr) * 24;
            debug.drawRect(x1, y1, x2 - x1, y2 - y1);
        }
    }
}