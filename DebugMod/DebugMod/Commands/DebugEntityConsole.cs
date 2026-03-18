using System;
using System.Collections.Generic;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Core.Enums;
using dc;
using dc.en;
using dc.hl.types;
using dc.hxsl;
using dc.shader;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;
using dc.spine.attachments;

namespace DebugMod.Commands.activateDebug
{
    public static class DebugEntityConsole
    {
        private static List<Entity>? Entities;
        [ConsoleMethod("remove-all-mobs", "移除所有怪物")]
        public static void RemoveAllMob(TextWriter writer)
        {
            Hero hero = Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.WriteLine("无法找到英雄实例");
                return;
            }
            ValidationHelper.NotNull(hero, nameof(hero));
            Entities = Game.Instance.HeroInstance!._level.RemoveAllMobsSafe();

        }

        [ConsoleMethod("show-removemobs", "显示所有移除的实体")]
        public static async Task ShowAllEntities(TextWriter writer)
        {
            if (Entities == null)
            {
                writer.WriteLine("没有可显示的实体");
                return;
            }
            Entities = (List<Entity>)ValidationHelper.NoNullElements(Entities, nameof(Entities));
            ArrayObj obj = Entities.ToArrayObj();
            await foreach (var entity in obj.AsEnumerableAsync())
            {
                writer.WriteLine($"实体: {entity}");
            }
        }

        [ConsoleMethod("show-port", "照亮所有传送门")]
        public static async Task ShowAllTeleports(TextWriter writer)
        {
            Hero hero = Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.WriteLine("无法找到英雄实例");
                return;
            }
            ValidationHelper.NotNull(hero, nameof(hero));
            await hero._level.ShowTheTransmission();
        }

        [ConsoleMethod("add-listmobs", "添加移除列表中的实体")]
        public static void AddListMobs(TextWriter writer)
        {
            var level = Game.Instance.HeroInstance!._level;
            ValidationHelper.NotNull(level, nameof(level));
            if (Entities == null)
            {
                writer.WriteLine("列表不存在实体");
                return;
            }
            ValidationHelper.NoNullElements(Entities!, nameof(Entities));

            ArrayObj obj = Entities!.ToArrayObj();

            level.map.AddMobsFromArray(obj, writer);
        }

        [ConsoleMethod("mimic", "下一关是否生成拟态商人")]
        public static void loreRoommim(TextWriter writer)
        {
            if (dc.pr.Game.Class.ME == null)
            {
                writer.Write("请保证细胞人实例存在！");
                return;
            }
            writer.Write($"下一关是否生成拟态商人：{dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel}");
            if (!dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel)
                dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel = true;

        }

        [ConsoleMethod("create-mob", "创建：(createmob 怪物id)")]
        public static void CreateMob(TextWriter writer, string id)
        {
            Hero hero = dc.pr.Game.Class.ME.hero;
            dc.en.Mob.Class.create(id.ToHaxeString(), hero._level, hero.cx, hero.cy, 100, Ref<int>.In(100));
        }

        [ConsoleMethod("tree", "实体四叉树,线条越多:所在区域实体越多(下个关卡开始生效)")]
        public static void QuadTreeDrawing(TextWriter writer)
        {
            bool isdraw = DebugModMod.GetConfig.Value.IsQuadTreeDrawingEnabled;
            if (isdraw)
                isdraw = false;
            isdraw = true;
        }


    }
}