using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Enums;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.en;
using dc.h2d;
using IngameDebugConsole;
using ModCore.Modules;

namespace DebugMod.Commands
{
    public static class DebugTileConsonle
    {
        public static Graphics tilegt = null!;

        [ConsoleMethod("remove-tile-g", "移除绘制的矩形")]
        public static void RemoveGT(TextWriter writer)
        {
            if (tilegt == null)
            {
                writer.Write("没有可移除的矩形");
                return;
            }
            tilegt.remove();
        }

        [ConsoleMethod("show-tile-g", "根据字符串参数绘制指定类型的瓷砖组", "可用类型:BackWalls, BackWallProps, BackWallProps2, BackProps, MainProps, GameplayProps, BgFilterProps, FrontWalls, FrontWallProps, AllBackground, AllForeground, AllProps, AllWalls, All。支持逗号分隔多个类型,如:show-tile-g GameplayProps,FrontWalls")]
        public static void DrawTile(TextWriter writer, string type)
        {
            Hero hero = Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.WriteLine("无法找到英雄实例");
                return;
            }
            ValidationHelper.NotNull(hero, nameof(hero));

            var typeParts = type.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var validTypes = new List<TileGroupType>();

            foreach (var part in typeParts)
            {
                TileGroupType tileGroupType = ParseTileGroupType(part);
                if (tileGroupType != TileGroupType.None)
                    validTypes.Add(tileGroupType);
                else
                    writer.WriteLine($"警告: 忽略未知的瓷砖组类型: {part}");

            }

            if (validTypes.Count == 0)
            {
                writer.WriteLine($"错误: 没有有效的瓷砖组类型。可用类型: BackWalls, BackWallProps, BackWallProps2, BackProps, MainProps, GameplayProps, BgFilterProps, FrontWalls, FrontWallProps, AllBackground, AllForeground, AllProps, AllWalls, All");
                return;
            }

            tilegt = hero._level.fx.DebugTileGroups(hero._level.lDisp, validTypes.ToArray());
        }

        private static TileGroupType ParseTileGroupType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return TileGroupType.None;

            string normalized = type.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "none":
                    return TileGroupType.None;
                case "backwalls":
                    return TileGroupType.BackWalls;
                case "backwallprops":
                    return TileGroupType.BackWallProps;
                case "backwallprops2":
                    return TileGroupType.BackWallProps2;
                case "backprops":
                    return TileGroupType.BackProps;
                case "mainprops":
                    return TileGroupType.MainProps;
                case "gameplayprops":
                    return TileGroupType.GameplayProps;
                case "bgfilterprops":
                    return TileGroupType.BgFilterProps;
                case "frontwalls":
                    return TileGroupType.FrontWalls;
                case "frontwallprops":
                    return TileGroupType.FrontWallProps;
                case "allbackground":
                    return TileGroupType.AllBackground;
                case "allforeground":
                    return TileGroupType.AllForeground;
                case "allprops":
                    return TileGroupType.AllProps;
                case "allwalls":
                    return TileGroupType.AllWalls;
                case "all":
                    return TileGroupType.All;
                default:
                    if (Enum.TryParse<TileGroupType>(type, true, out var result))
                    {
                        return result;
                    }
                    return TileGroupType.None;
            }
        }

        [ConsoleMethod("show-col", "显示碰撞体积")]
        public static void DebugDrawCollisionBits(TextWriter writer)
        {
            Hero hero = Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.WriteLine("无法找到英雄实例");
                return;
            }
            ValidationHelper.NotNull(hero, "hero");

            hero._level.fx.DebugDrawCollisionBits();
        }
    }
}