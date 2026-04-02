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
                ValidationHelper.NotNull(entity, nameof(entity));

                Game.Class.ME.curLevel.entities.push(entity);
            }
            foreach (var roommob in map.rooms.AsEnumerable())
            {
                ValidationHelper.NotNull(roommob, nameof(roommob));
                ArrayObj mobsArray = roommob.mobs;
                foreach (var m in mobsArray.AsEnumerable())
                {
                    if (m == null)
                        continue;
                    ValidationHelper.NotNull(m, nameof(m));
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


        public static IEnumerable<Room> GetRooms(this LevelMap map)
        {
            ValidationHelper.NotNull(map, nameof(map));

            var rooms = map.rooms;
            if (rooms.IsNullOrEmpty()) yield break;

            for (int i = 0; i < rooms.length; i++)
            {
                var room = rooms.getDyn(i) as Room;
                if (room != null)
                {
                    yield return room;
                }
            }
        }


        public static (int width, int height) GetDimensions(this LevelMap map)
        {
            ValidationHelper.NotNull(map, nameof(map));
            return (map.wid, map.hei);
        }

        public static int GetSeed(this LevelMap map)
        {
            ValidationHelper.NotNull(map, nameof(map));
            return map.seed;
        }



    }
}