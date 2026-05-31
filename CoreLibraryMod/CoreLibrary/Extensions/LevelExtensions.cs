using System;
using System.Collections.Generic;
using dc;
using dc.level;
using dc.pr;
using dc.libs;
using HaxeProxy.Runtime;
using Hashlink.Virtuals;
using dc.libs.heaps.slib;
using dc.h2d;
using dc.h2d.filter;
using dc.hl.types;
using dc.light;
using dc.en;
using Hashlink.Proxy.DynamicAccess;
using dc.en.mob;
using CoreLibrary.Core.Utilities;

namespace CoreLibrary.Core.Extensions
{
    public static class LevelExtensions
    {
        public static void RemoveEntitySafe(this dc.pr.Level level, Entity entity)
        {
            entity.destroy();
            level.unregisterEntity(entity);
        }


        public static List<Entity> RemoveAllMobsSafe(this dc.pr.Level level)
        {
            var mobsToRemove = new List<Entity>();
            foreach (Entity entity in level.entities.AsEnumerable())
            {
                if (entity is dc.en.Mob)
                {
                    if (entity is Boss)
                        continue;
                    mobsToRemove.Add(entity);
                }
            }
            foreach (dc.en.Mob mob in mobsToRemove)
            {
                try
                {
                    mob.destroy();
                    level.unregisterEntity(mob);
                }
                catch { }
            }
            return mobsToRemove;
        }


        public static async Task ShowTheTransmission(this dc.pr.Level level)
        {
            var mobsToRemove = new List<Entity>();
            await foreach (Entity entity in level.entities.AsEnumerableAsync())
            {
                if (entity is dc.en.inter.Teleport)
                {
                    mobsToRemove.Add(entity);
                }
            }
            await foreach (dc.en.inter.Teleport teleport in mobsToRemove.ToAsyncEnumerable())
            {
                try
                {
                    if (!teleport.opened)
                        teleport.open();
                }
                catch { }
            }
        }

        public static IEnumerable<Marker> GetMarkers(this Room room)
        {
            var markers = room.markers;
            for (int i = 0; i < markers.length; i++)
            {
                var marker = markers.array[i] as Marker;
                if (marker != null)
                {
                    yield return marker;
                }
            }
        }


        public static IEnumerable<Marker> GetMarkersByKind(this Room room, string kind)
        {
            foreach (var marker in room.GetMarkers())
            {
                if (marker.kind.ToString().EqualsIgnoreCase(kind))
                {
                    yield return marker;
                }
            }
        }


        public static IEnumerable<Marker> GetMarkersByCustomId(this Room room, string customId)
        {
            foreach (var marker in room.GetMarkers())
            {
                var markerCustomId = marker.customId;
                if (markerCustomId != null && markerCustomId.ToString().EqualsIgnoreCase(customId))
                {
                    yield return marker;
                }
            }
        }


        public static bool IsCustomId(this Marker marker, string customId)
        {
            var markerCustomId = marker.customId;
            if (markerCustomId == null) return false;

            return markerCustomId.ToString().EqualsIgnoreCase(customId);
        }


        public static (int x, int y) GetWorldPosition(this Marker marker, Room room)
        {
            return (room.x + marker.cx, room.y + marker.cy);
        }
    }
}