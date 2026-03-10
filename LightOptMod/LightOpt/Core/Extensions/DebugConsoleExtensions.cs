using dc;
using dc.en;
using dc.hl.types;
using dc.hxsl;
using dc.shader;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using LightOpt.Core.Utilities;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;

namespace LightOpt.Core.Extensions
{
    public static class DebugConsoleExtensions
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

        [ConsoleMethod("test", "修改英雄着色器颜色")]
        public static void ChangeHeroShaderColor(TextWriter writer)
        {
            Hero hero = Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.WriteLine("无法找到英雄实例");
                return;
            }
            ValidationHelper.NotNull(hero, nameof(hero));
        }
    }
}