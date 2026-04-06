using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.level;
using dc.level.@struct;
using dc.libs;
using Hashlink.Virtuals;

namespace EnemiesVsEnemies.Level
{
    public class BattleLevel : DebugRTC
    {

        public const string ForcedBiome = "QueenArena";

        public BattleLevel(User user, virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ level, Rand rng) : base(user, level, rng)
        {
        }



        public override RoomNode buildMainRooms()
        {
            RoomNode entranceNode = createNode(null, "QueenEntrance".ToHaxeString(), null, "start".ToHaxeString());
            entranceNode.AddFlags(new RoomFlag.Outside());
            var virtual_add = new virtual_specificBiome_();
            virtual_add.specificBiome = ForcedBiome.ToHaxeString();
            entranceNode.addGenData(virtual_add.ToVirtual<virtual_altarItemGroup_brLegendaryMultiTreasure_broken_cells_doorCost_doorCurse_flaskRefill_forcedMerchantType_forcePauseTimer_isCliffPath_itemInWall_itemLevelBonus_killsMultiTreasure_locked_maxPerks_mins_noHealingShop_shouldBeFlipped_specificBiome_subTeleportTo_timedMultiTreasure_zDoorLock_zDoorType_>());


            RoomNode roomNode = createNode(null, "QueenArenaV3".ToHaxeString(), null, null);
            roomNode.AddFlags(new RoomFlag.Outside());
            roomNode.set_parent(entranceNode);

            return nodes.get("start".ToHaxeString());
        }
    }
}