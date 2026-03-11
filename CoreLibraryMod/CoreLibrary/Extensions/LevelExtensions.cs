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
        public static bool IsLevel(this dc.pr.Level level, string levelId)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNullOrWhiteSpace(levelId, nameof(levelId));
            ValidationHelper.NotNull(level.map, nameof(level.map));

            var biomeId = level.map.biome?.id;
            if (biomeId == null) return false;

            return biomeId.ToString().EqualsIgnoreCase(levelId);
        }


        public static string GetBiomeId(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNull(level.map, nameof(level.map));
            ValidationHelper.NotNull(level.map.biome, nameof(level.map.biome));

            return level.map.biome.id.ToString();
        }


        public static LevelDisp? GetLevelDisplaySafe(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            return level.lDisp;
        }


        public static LightedLayers? GetLightedLayersSafe(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            return level.scroller;
        }


        public static LevelMap? GetMapSafe(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            return level.map;
        }


        public static Layers? GetRootSafe(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            return level.root;
        }


        public static bool IsPaused(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));
            return level.paused;
        }

        public static void AddEntitySafe(this dc.pr.Level level, Entity entity)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNull(entity, nameof(entity));

            if (!level.destroyed)
            {
                level.entities.push(entity);
            }
        }


        public static void RemoveEntitySafe(this dc.pr.Level level, Entity entity)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNull(entity, nameof(entity));
            entity.destroy();
            level.unregisterEntity(entity);
        }


        public static List<Entity> RemoveAllMobsSafe(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));

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
            ValidationHelper.NotNull(level, nameof(level));
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




        public static void RegisterEntitySafe(this dc.pr.Level level, Entity entity)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNull(entity, nameof(entity));

            if (level.destroyed) return;

            try
            {
                level.registerEntity(entity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to register entity: {ex.Message}");
                throw;
            }
        }


        public static IEnumerable<Entity> GetEntitiesByClass(this dc.pr.Level level, int classId)
        {
            ValidationHelper.NotNull(level, nameof(level));

            var entitiesByClass = level.entitiesByClass;
            if (entitiesByClass == null) yield break;

            var entities = entitiesByClass.get(classId) as ArrayObj;
            if (entities!.IsNullOrEmpty()) yield break;

            foreach (var entity in entities!.AsEnumerable())
            {
                if (entity is Entity e)
                {
                    yield return e;
                }
            }
        }


        public static IEnumerable<Marker> GetMarkers(this Room room)
        {
            ValidationHelper.NotNull(room, nameof(room));

            var markers = room.markers;
            if (markers.IsNullOrEmpty()) yield break;

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
            ValidationHelper.NotNull(room, nameof(room));
            ValidationHelper.NotNullOrWhiteSpace(kind, nameof(kind));

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
            ValidationHelper.NotNull(room, nameof(room));
            ValidationHelper.NotNullOrWhiteSpace(customId, nameof(customId));

            foreach (var marker in room.GetMarkers())
            {
                var markerCustomId = marker.customId;
                if (markerCustomId != null && markerCustomId.ToString().EqualsIgnoreCase(customId))
                {
                    yield return marker;
                }
            }
        }


        public static bool IsKind(this Marker marker, string kind)
        {
            ValidationHelper.NotNull(marker, nameof(marker));
            ValidationHelper.NotNullOrWhiteSpace(kind, nameof(kind));

            return marker.kind.ToString().EqualsIgnoreCase(kind);
        }


        public static bool IsCustomId(this Marker marker, string customId)
        {
            ValidationHelper.NotNull(marker, nameof(marker));
            ValidationHelper.NotNullOrWhiteSpace(customId, nameof(customId));

            var markerCustomId = marker.customId;
            if (markerCustomId == null) return false;

            return markerCustomId.ToString().EqualsIgnoreCase(customId);
        }


        public static (int x, int y) GetWorldPosition(this Marker marker, Room room)
        {
            ValidationHelper.NotNull(marker, nameof(marker));
            ValidationHelper.NotNull(room, nameof(room));

            return (room.x + marker.cx, room.y + marker.cy);
        }

        public static string GetId(this virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ levelData)
        {
            ValidationHelper.NotNull(levelData, nameof(levelData));
            return levelData.id.ToString();
        }


        public static bool IsLevelId(this virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ levelData, string levelId)
        {
            ValidationHelper.NotNull(levelData, nameof(levelData));
            ValidationHelper.NotNullOrWhiteSpace(levelId, nameof(levelId));

            return levelData.GetId().EqualsIgnoreCase(levelId);
        }


        public static string? GetBiomeSafe(this virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ levelData)
        {
            ValidationHelper.NotNull(levelData, nameof(levelData));
            return levelData.biome?.ToString();
        }


        public static double GetMobDensitySafe(this virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ levelData, double defaultValue = 1.0)
        {
            ValidationHelper.NotNull(levelData, nameof(levelData));
            return levelData.mobDensity;
        }


        public static HSpriteBatch CreateStandardBatch(this dc.pr.Level level, Tile tile, int depth)
        {
            ValidationHelper.NotNull(level, nameof(level));
            ValidationHelper.NotNull(tile, nameof(tile));

            var batch = new HSpriteBatch(tile, null);
            level.scroller?.addChildAt(batch, depth);
            return batch;
        }


        public static T AddShader<T>(this HSpriteBatch batch, T shader) where T : dc.hxsl.Shader
        {
            ValidationHelper.NotNull(batch, nameof(batch));
            ValidationHelper.NotNull(shader, nameof(shader));

            return (T)batch.addShader(shader);
        }


        public static void SetBlendMode(this HSpriteBatch batch, BlendMode blendMode)
        {
            ValidationHelper.NotNull(batch, nameof(batch));
            ValidationHelper.NotNull(blendMode, nameof(blendMode));

            batch.blendMode = blendMode;
        }


        public static bool IsOutdoorLevel(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));

            var biomeId = level.GetBiomeId();
            return biomeId.Contains("OutSide", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsIndoorLevel(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));

            var biomeId = level.GetBiomeId();
            return biomeId.Contains("int", StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsBossLevel(this dc.pr.Level level)
        {
            ValidationHelper.NotNull(level, nameof(level));

            var biomeId = level.GetBiomeId();
            return biomeId.Contains("boss", StringComparison.OrdinalIgnoreCase);
        }

       
    }
}