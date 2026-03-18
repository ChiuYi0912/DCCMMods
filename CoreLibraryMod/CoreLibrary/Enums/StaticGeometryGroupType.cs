using System;

namespace CoreLibrary.Core.Enums
{
    [Flags]
    public enum TileGroupType
    {
        /// <summary>
        /// 无瓷砖组
        /// </summary>
        None = 0,

        // 背景相关

        /// <summary>
        /// 背景墙
        /// </summary>
        BackWalls = 1 << 0,

        /// <summary>
        /// 背景墙道具
        /// </summary>
        BackWallProps = 1 << 1,

        /// <summary>
        /// 背景墙道具2
        /// </summary>
        BackWallProps2 = 1 << 2,

        /// <summary>
        /// 背景道具
        /// </summary>
        BackProps = 1 << 3,

        // 主游戏区域

        /// <summary>
        /// 主道具
        /// </summary>
        MainProps = 1 << 4,

        /// <summary>
        /// 游戏玩法道具
        /// </summary>
        GameplayProps = 1 << 5,

        /// <summary>
        /// 背景滤镜道具
        /// </summary>
        BgFilterProps = 1 << 6,

        // 前景相关

        /// <summary>
        /// 前景墙
        /// </summary>
        FrontWalls = 1 << 7,

        /// <summary>
        /// 前景墙道具
        /// </summary>
        FrontWallProps = 1 << 8,

        // 组合类型

        /// <summary>
        /// 所有背景相关组：背景墙、背景墙道具、背景墙道具2、背景道具
        /// </summary>
        AllBackground = BackWalls | BackWallProps | BackWallProps2 | BackProps,

        /// <summary>
        /// 所有前景相关组：前景墙、前景墙道具
        /// </summary>
        AllForeground = FrontWalls | FrontWallProps,

        /// <summary>
        /// 所有道具相关组：背景墙道具、背景墙道具2、背景道具、主道具、游戏玩法道具、背景滤镜道具、前景墙道具
        /// </summary>
        AllProps = BackWallProps | BackWallProps2 | BackProps | MainProps | GameplayProps | BgFilterProps | FrontWallProps,

        /// <summary>
        /// 所有墙壁组：背景墙、前景墙
        /// </summary>
        AllWalls = BackWalls | FrontWalls,

        /// <summary>
        /// 所有瓷砖组
        /// </summary>
        All = AllBackground | MainProps | GameplayProps | BgFilterProps | AllForeground
    }
}