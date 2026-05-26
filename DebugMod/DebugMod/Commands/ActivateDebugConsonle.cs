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
using HaxeProxy.Runtime;
using IngameDebugConsole;
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





        [ConsoleMethod("test", "作者测试用的")]
        public static void ChangeHeroShaderColor(TextWriter writer)
        {
            Hero hero = Game.Class.ME.hero;
            // var p = new Portal(hero._level, hero.cx, hero.cy, hero._level.map, Ref<bool>.In(false));
            // p.init();
            // new SecretLever(hero._level, hero.cx, hero.cy, p).init();

            GetAllSecretChallenges(hero);
            //foreach (SecretChallengeInfo item in list)
            //{
            //Log.Debug($"room:{item.room.toString()},item:{item.requiredItem.GetType().Name}");
            //
        }

        public class SecretChallengeInfo
        {
            public Room room = null!;
            public InventItem requiredItem = null!;
        }

        public static void GetAllSecretChallenges(Hero hero)
        {
            ArrayObj rooms = hero._level.map.rooms;
            for (int i = 0; i < rooms.length; i++)
            {
                Room room = rooms.getDyn(i);
                if (room == null) continue;
                if (room.secretLevels == null) continue;
                for (int j = 0; j < room.secretLevels.length; j++)
                {
                    InventItem item = room.secretLevels.getDyn(j);
                    if (item == null) continue;
                    Log.Debug($"{room.toString()}\n");
                }
            }
        }
    }
}