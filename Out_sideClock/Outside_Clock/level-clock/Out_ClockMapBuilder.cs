using dc;
using dc.hl.types;
using dc.level;
using dc.level.gen;
using dc.libs;
using Hashlink.Virtuals;

namespace Outside_Clock;

public class Out_ClockMapBuilder : MapBuilder
{
    public Out_ClockMapBuilder(User user, virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ infos, int seed, virtual_lastError_placed_rects_root_score_tplRepeat_valid_ genMapData, Rand rnd) : base(user, infos, seed, genMapData, rnd)
    {
    }

    public override void removeCollisionOutOfRooms(LevelMap drooms, ArrayObj inRoomCells, ArrayObj hasError, bool lr)
    {
        base.removeCollisionOutOfRooms(drooms, inRoomCells, hasError, lr);

    }
}
