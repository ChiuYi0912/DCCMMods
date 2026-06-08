using System.Collections.Generic;
using dc;
using dc.en;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using LevelIfor_Virtual = Hashlink.Virtuals.virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_;

namespace CoreLibrary.Utilities
{
    public class CDBLevelCache
    {
        private readonly Dictionary<string, LevelIfor_Virtual> byId = new();


        private readonly HashSet<string> bossLevels = new();
        private readonly HashSet<string> dlcLevels = new();
        private readonly HashSet<string> allIds = new();

        public void Refresh()
        {
            byId.Clear();
            bossLevels.Clear();
            dlcLevels.Clear();
            allIds.Clear();

            var levelData = Data.Class.level;
            int count = levelData.all.get_length();

            for (int i = 0; i < count; i++)
            {
                var raw = levelData.all.getDyn(i);
                var info = ((HaxeProxyBase)raw).ToVirtual<LevelIfor_Virtual>();
                string id = info.id.ToString();

                byId[id] = info;
                allIds.Add(id);

                if (info.group == 1)
                    bossLevels.Add(id);

                if (info.GetDynamicMemberNames().Contains("dlc"))
                    dlcLevels.Add(id);
            }
        }


        public bool Exists(string id) => allIds.Contains(id);

        public LevelIfor_Virtual Get(string id) =>
            byId.TryGetValue(id, out var info) ? info : null!;

        public bool IsBossLevel(string id)
        {
            if (byId.TryGetValue(id, out var info))
                return info.group == 1;
            return false;
        }

        public bool IsDLCLevel(string id) => dlcLevels.Contains(id);

        public int GetWorldDepth(string id)
        {
            if (byId.TryGetValue(id, out var info))
                return info.worldDepth;
            return -1;
        }

        public int GetGroup(string id)
        {
            if (byId.TryGetValue(id, out var info))
                return info.group;
            return -1;
        }

        public bool HasGenFlag(string id, int flag)
        {
            if (byId.TryGetValue(id, out var info))
                return (info.flagsProps.genFlags & flag) != 0;
            return false;
        }

    }
}