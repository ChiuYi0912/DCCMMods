using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Enums;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.hl.types;
using dc.level;
using dc.libs.heaps;
using dc.pr;
using dc.tool;
using HaxeProxy.Runtime;
using Serilog;
using Math = System.Math;

namespace CoreLibrary.Core.Extensions
{
    public static class FxExtensions
    {
        public static dc.h2d.Graphics DebugTileGroups(this Fx fx, LevelDisp disp, params TileGroupType[] groupTypes)
        {
            if (groupTypes == null || groupTypes.Length == 0)
                return DebugTileGroup(fx, disp, TileGroupType.None);

            TileGroupType combined = TileGroupType.None;
            foreach (var type in groupTypes)
                combined |= type;

            return DebugTileGroup(fx, disp, combined);
        }

        private static readonly Dictionary<TileGroupType, string> DebugTileGroupColorMap = new()
        {
                { TileGroupType.BackWalls, "#FF0000" },
                { TileGroupType.BackWallProps, "#00FF00" },
                { TileGroupType.BackWallProps2, "#0000FF" },
                { TileGroupType.BackProps, "#FFFF00" },
                { TileGroupType.MainProps, "#FF00FF" },
                { TileGroupType.GameplayProps, "#ff007b" },
                { TileGroupType.BgFilterProps, "#FFA500" },
                { TileGroupType.FrontWalls, "#ffffff" },
                { TileGroupType.FrontWallProps, "#00ffff" },
        };
        public static dc.h2d.Graphics DebugTileGroup(this Fx fx, LevelDisp disp, TileGroupType groupTypes = TileGroupType.None)
        {
            var groupsToDraw = new List<(StaticGeometryGroup group, TileGroupType type)>();

            if (groupTypes.HasFlag(TileGroupType.BackWalls))
                groupsToDraw.Add((disp.groupBackWalls, TileGroupType.BackWalls));

            if (groupTypes.HasFlag(TileGroupType.BackWallProps))
                groupsToDraw.Add((disp.groupBackWallProps, TileGroupType.BackWallProps));

            if (groupTypes.HasFlag(TileGroupType.BackWallProps2))
                groupsToDraw.Add((disp.groupBackWallProps2, TileGroupType.BackWallProps2));

            if (groupTypes.HasFlag(TileGroupType.BackProps))
                groupsToDraw.Add((disp.groupBackProps, TileGroupType.BackProps));

            if (groupTypes.HasFlag(TileGroupType.MainProps))
                groupsToDraw.Add((disp.groupMainProps, TileGroupType.MainProps));

            if (groupTypes.HasFlag(TileGroupType.GameplayProps))
                groupsToDraw.Add((disp.groupGameplayProps, TileGroupType.GameplayProps));

            if (groupTypes.HasFlag(TileGroupType.BgFilterProps))
                groupsToDraw.Add((disp.groupBgFilterProps, TileGroupType.BgFilterProps));

            if (groupTypes.HasFlag(TileGroupType.FrontWalls))
                groupsToDraw.Add((disp.groupFrontWalls, TileGroupType.FrontWalls));

            if (groupTypes.HasFlag(TileGroupType.FrontWallProps))
                groupsToDraw.Add((disp.groupFrontWallProps, TileGroupType.FrontWallProps));

            if (groupsToDraw.Count == 0)
                return null!;

            var g = new dc.h2d.Graphics(null);
            Game.Class.ME.curLevel.scroller.addChildAt(g, Const.Class.DP_DEBUG);
            foreach (var (group, groupType) in groupsToDraw)
            {
                if (!DebugTileGroupColorMap.TryGetValue(groupType, out var color))
                    color = "#0dff00";

                g.lineStyle(Ref<double>.In(1), Ref<int>.In(CreateColor.ColorFromHex(color)), Ref<double>.In(1));
                var iterator = group.getTileGroups();
                while (iterator.hasNext.Invoke())
                {
                    var tg = iterator.next.Invoke();

                    var content = tg.content;
                    if (content == null) continue;

                    var tmp = content.tmp;
                    if (tmp == null) continue;

                    int stride = 32;
                    for (int j = 0; j < tmp.length / stride; j++)
                    {
                        int b = j * stride;

                        float x0 = (float)tmp.getDyn(b + 0);
                        float y0 = (float)tmp.getDyn(b + 1);

                        float x1 = (float)tmp.getDyn(b + 8);
                        float y1 = (float)tmp.getDyn(b + 9);

                        float x2 = (float)tmp.getDyn(b + 16);
                        float y2 = (float)tmp.getDyn(b + 17);

                        float x3 = (float)tmp.getDyn(b + 24);
                        float y3 = (float)tmp.getDyn(b + 25);

                        float minX = Math.Min(Math.Min(x0, x1), Math.Min(x2, x3));
                        float minY = Math.Min(Math.Min(y0, y1), Math.Min(y2, y3));
                        float maxX = Math.Max(Math.Max(x0, x1), Math.Max(x2, x3));
                        float maxY = Math.Max(Math.Max(y0, y1), Math.Max(y2, y3));

                        g.drawRect(minX, minY, maxX - minX, maxY - minY);
                    }
                }
            }
            return g;
        }



        private static readonly (int bit, string color, string name)[] Collisionflags = new[]
        {
            (1,    "#ff00f7", "Solid"),
            (2,    "#00ff2a", "Bit2"),
            (8,    "#2b00ff", "OneWay?"),
            (16,    "#00d5ff", "Bit4"),
            (32,    "#ffffff", "Bit5"),
            (64,    "#e1ff00", "Bit6"),
            (128,    "#ff8000", "Bit7"),
            (256,    "#ff6ead", "Bit8"),
            (512,    "#51004f", "Bit9"),
        };
        public static void DebugDrawCollisionBits(this Fx fx)
        {
            Hero hero = Game.Class.ME.hero;
            var debug = hero._level.lDisp.debug;
            var lmap = hero._level.map;
            if (debug == null || lmap == null) return;

            debug.clear();

            int wid = lmap.wid;
            int hei = lmap.hei;
            ArrayBytes_Int collisions = lmap.collisions;


            foreach (var f in Collisionflags)
            {
                bool[,] visited = new bool[hei, wid];

                for (int y = 0; y < hei; y++)
                {
                    for (int x = 0; x < wid; x++)
                    {
                        int index = y * wid + x;
                        int value = collisions.getDyn(index);
                        bool hasFlag = (value & f.bit) != 0;

                        if (hasFlag && !visited[y, x])
                        {
                            var regionCells = new List<(int x, int y)>();
                            var queue = new Queue<(int x, int y)>();
                            queue.Enqueue((x, y));
                            visited[y, x] = true;

                            while (queue.Count > 0)
                            {
                                var (cx, cy) = queue.Dequeue();
                                regionCells.Add((cx, cy));

                                int[] dx = { -1, 1, 0, 0 };
                                int[] dy = { 0, 0, -1, 1 };
                                for (int d = 0; d < 4; d++)
                                {
                                    int nx = cx + dx[d];
                                    int ny = cy + dy[d];
                                    if (nx >= 0 && nx < wid && ny >= 0 && ny < hei && !visited[ny, nx])
                                    {
                                        int nIndex = ny * wid + nx;
                                        int nValue = collisions.getDyn(nIndex);
                                        if ((nValue & f.bit) != 0)
                                        {
                                            visited[ny, nx] = true;
                                            queue.Enqueue((nx, ny));
                                        }
                                    }
                                }
                            }


                            List<((int x1, int y1) topLeft, (int x2, int y2) bottomRight)> rectangles = new();

                            var rows = regionCells.GroupBy(cell => cell.y)
                                                  .OrderBy(g => g.Key)
                                                  .ToDictionary(g => g.Key, g => g.Select(c => c.x).OrderBy(x => x).ToList());

                            int minY = rows.Keys.Min();
                            int maxY = rows.Keys.Max();

                            var activeRects = new List<(int x1, int x2, int yStart)>();

                            for (int ry = minY; ry <= maxY; ry++)
                            {
                                if (!rows.TryGetValue(ry, out var xList))
                                {
                                    foreach (var rect in activeRects)
                                    {
                                        rectangles.Add(((rect.x1, rect.yStart), (rect.x2, ry - 1)));
                                    }
                                    activeRects.Clear();
                                    continue;
                                }

                                var runs = new List<(int x1, int x2)>();
                                int start = xList[0];
                                int prev = start;
                                for (int i = 1; i < xList.Count; i++)
                                {
                                    if (xList[i] == prev + 1)
                                    {
                                        prev = xList[i];
                                    }
                                    else
                                    {
                                        runs.Add((start, prev));
                                        start = xList[i];
                                        prev = start;
                                    }
                                }
                                runs.Add((start, prev));


                                var newActiveRects = new List<(int x1, int x2, int yStart)>();
                                foreach (var rect in activeRects)
                                {
                                    var matchedRun = runs.FirstOrDefault(r => r.x1 == rect.x1 && r.x2 == rect.x2);
                                    if (matchedRun != default)
                                    {
                                        newActiveRects.Add(rect);
                                        runs.Remove(matchedRun);
                                    }
                                    else
                                    {
                                        rectangles.Add(((rect.x1, rect.yStart), (rect.x2, ry - 1)));
                                    }
                                }

                                foreach (var run in runs)
                                {
                                    newActiveRects.Add((run.x1, run.x2, ry));
                                }

                                activeRects = newActiveRects;
                            }

                            foreach (var rect in activeRects)
                            {
                                rectangles.Add(((rect.x1, rect.yStart), (rect.x2, maxY)));
                            }

                            foreach (var rect in rectangles)
                            {
                                double px1 = rect.topLeft.x1 * 24.0;
                                double py1 = rect.topLeft.y1 * 24.0;
                                double px2 = (rect.bottomRight.x2 + 1) * 24.0;
                                double py2 = (rect.bottomRight.y2 + 1) * 24.0;
                                double w = px2 - px1;
                                double h = py2 - py1;
                                double alpha = 1;

                                debug.lineStyle(Ref<double>.In(1),
                                                Ref<int>.In(CreateColor.ColorFromHex(f.color)),
                                                Ref<double>.In(alpha));
                                debug.drawRect(px1, py1, w, h);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 绘制碰撞标志位的连通区域边框（调试用）。
        /// </summary>
        /// <remarks>
        /// 此方法执行大量计算，会显著影响性能，仅在 DEBUG 模式下有效。
        /// 建议通过 range 参数限制绘制范围。
        /// </remarks>
        public static void DebugDrawUpdateCollisionBits(this Fx fx, int range = 10)
        {
            Hero hero = Game.Class.ME.hero;
            if (hero == null) return;

            var debug = hero._level.lDisp.debug;
            var lmap = hero._level.map;
            if (debug == null || lmap == null) return;

            debug.clear();

            int wid = lmap.wid;
            int hei = lmap.hei;
            ArrayBytes_Int collisions = lmap.collisions;

            int heroCx = hero.cx;
            int heroCy = hero.cy;

            int mincX = Math.Max(0, heroCx - range);
            int maxcX = Math.Min(wid, heroCx + range + 1);
            int mincY = Math.Max(0, heroCy - range);
            int maxcY = Math.Min(hei, heroCy + range + 1);


            foreach (var f in Collisionflags)
            {
                bool[,] visited = new bool[hei, wid];

                for (int y = mincY; y < maxcY; y++)
                {
                    for (int x = mincX; x < maxcX; x++)
                    {
                        int index = y * wid + x;
                        int value = collisions.getDyn(index);
                        bool hasFlag = (value & f.bit) != 0;

                        if (hasFlag && !visited[y, x])
                        {
                            var regionCells = new List<(int x, int y)>();
                            var queue = new Queue<(int x, int y)>();
                            queue.Enqueue((x, y));
                            visited[y, x] = true;

                            while (queue.Count > 0)
                            {
                                var (cx, cy) = queue.Dequeue();
                                regionCells.Add((cx, cy));

                                int[] dx = { -1, 1, 0, 0 };
                                int[] dy = { 0, 0, -1, 1 };
                                for (int d = 0; d < 4; d++)
                                {
                                    int nx = cx + dx[d];
                                    int ny = cy + dy[d];
                                    if (nx >= mincX && nx < maxcX && ny >= mincY && ny < maxcY && !visited[ny, nx])
                                    {
                                        int nIndex = ny * wid + nx;
                                        int nValue = collisions.getDyn(nIndex);
                                        if ((nValue & f.bit) != 0)
                                        {
                                            visited[ny, nx] = true;
                                            queue.Enqueue((nx, ny));
                                        }
                                    }
                                }
                            }

                            List<((int x1, int y1) topLeft, (int x2, int y2) bottomRight)> rectangles = new();

                            var rows = regionCells.GroupBy(cell => cell.y)
                                                  .OrderBy(g => g.Key)
                                                  .ToDictionary(g => g.Key, g => g.Select(c => c.x).OrderBy(x => x).ToList());

                            int minY = rows.Keys.Min();
                            int maxY = rows.Keys.Max();

                            var activeRects = new List<(int x1, int x2, int yStart)>();

                            for (int ry = minY; ry <= maxY; ry++)
                            {
                                if (!rows.TryGetValue(ry, out var xList))
                                {
                                    foreach (var rect in activeRects)
                                    {
                                        rectangles.Add(((rect.x1, rect.yStart), (rect.x2, ry - 1)));
                                    }
                                    activeRects.Clear();
                                    continue;
                                }

                                var runs = new List<(int x1, int x2)>();
                                int start = xList[0];
                                int prev = start;
                                for (int i = 1; i < xList.Count; i++)
                                {
                                    if (xList[i] == prev + 1)
                                    {
                                        prev = xList[i];
                                    }
                                    else
                                    {
                                        runs.Add((start, prev));
                                        start = xList[i];
                                        prev = start;
                                    }
                                }
                                runs.Add((start, prev));


                                var newActiveRects = new List<(int x1, int x2, int yStart)>();
                                foreach (var rect in activeRects)
                                {
                                    var matchedRun = runs.FirstOrDefault(r => r.x1 == rect.x1 && r.x2 == rect.x2);
                                    if (matchedRun != default)
                                    {
                                        newActiveRects.Add(rect);
                                        runs.Remove(matchedRun);
                                    }
                                    else
                                    {
                                        rectangles.Add(((rect.x1, rect.yStart), (rect.x2, ry - 1)));
                                    }
                                }

                                foreach (var run in runs)
                                {
                                    newActiveRects.Add((run.x1, run.x2, ry));
                                }

                                activeRects = newActiveRects;
                            }

                            foreach (var rect in activeRects)
                            {
                                rectangles.Add(((rect.x1, rect.yStart), (rect.x2, maxY)));
                            }

                            foreach (var rect in rectangles)
                            {
                                double px1 = rect.topLeft.x1 * 24.0;
                                double py1 = rect.topLeft.y1 * 24.0;
                                double px2 = (rect.bottomRight.x2 + 1) * 24.0;
                                double py2 = (rect.bottomRight.y2 + 1) * 24.0;
                                double w = px2 - px1;
                                double h = py2 - py1;
                                double alpha = 1;

                                debug.lineStyle(Ref<double>.In(1),
                                                Ref<int>.In(CreateColor.ColorFromHex(f.color)),
                                                Ref<double>.In(alpha));
                                debug.drawRect(px1, py1, w, h);
                            }
                        }
                    }
                }
            }
        }
    }
}