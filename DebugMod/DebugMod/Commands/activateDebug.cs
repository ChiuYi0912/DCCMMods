using dc.en;
using dc.hl.types;
using dc.level;
using dc.libs;
using dc.pr;
using dc.tool;
using dc.ui;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Utilities;

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
            PerkSelect perkSelect = new PerkSelect(Game.Class.ME.hero, Game.Class.ME.hero);
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
            {
                hero.inventory.add(InventItem.Class.fromItem(itemName.AsHaxeString(), null));
            }
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


        [ConsoleMethod("mimic", "下一关是否生成拟态商人")]
        public static void loreRoommim(TextWriter writer)
        {
            if (Game.Class.ME == null)
            {
                writer.Write("请保证细胞人实例存在！");
                return;
            }
            writer.Write($"下一关是否生成拟态商人：{Game.Class.ME.user.game.spawnMimicInNextLevel}");
            if (!Game.Class.ME.user.game.spawnMimicInNextLevel)
            {
                Game.Class.ME.user.game.spawnMimicInNextLevel = true;
            }
        }
    }
}