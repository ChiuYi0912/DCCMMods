using CoreLibrary.Core.Utilities;
using dc;
using dc.hl.types;
using dc.level;
using dc.pr;
using Serilog;

namespace CoreLibrary.Core.Extensions
{
    public static class MapExtensions
    {
        public static void AddMobsFromArray(this LevelMap map, ArrayObj moblist, TextWriter? writer)
        {
            foreach (Entity entity in moblist.AsEnumerable())
            {
                Game.Class.ME.curLevel.entities.push(entity);
            }
            foreach (var roommob in map.rooms.AsEnumerable())
            {
                ArrayObj mobsArray = roommob.mobs;
                foreach (var m in mobsArray.AsEnumerable())
                {
                    if (m == null)
                        continue;
                    Level level = Game.Class.ME.curLevel;
                    level.attachMob(m);
                    if (writer == null)
                    {
                        Log.Logger.LogDebug($"添加：{m}");
                        return;
                    }
                    writer.WriteLine($"添加：{m}");
                }
            }
        }


        public static (int width, int height) GetDimensions(this LevelMap map)
        {
            return (map.wid, map.hei);
        }
    }
}