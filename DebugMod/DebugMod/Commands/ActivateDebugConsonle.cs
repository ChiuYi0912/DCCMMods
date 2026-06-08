using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.en.inter;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.libs;
using dc.pr;
using dc.tool;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Storage;
using ModCore.Utilities;
using Serilog;
using Serilog.Core;

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

        [ConsoleMethod("cd", "heroCd")]
        public static void HeroCD(TextWriter writer, int cdkey)
        {
            Hero hero = Game.Class.ME.hero;
            var cd = hero.cd;
            var indexes = dc.tool.Cooldown.Class.INDEXES;

            int key = cdkey;
            int typeIndex = key >> 21;
            int subId = key & 0x1FFFFF;

            if (typeIndex >= 0 && typeIndex < indexes.length)
            {
                var typeName = indexes.getDyn(typeIndex);
                writer.Write($"type: {typeName}, typeIndex: {typeIndex}, subId: {subId}");
            }
            else
            {
                writer.Write($"类型索引超出范围: {typeIndex}");
            }
        }


    }
}