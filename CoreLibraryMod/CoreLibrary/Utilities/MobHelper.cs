using System;
using System.Collections.Generic;
using System.Linq;
using dc;
using dc.en;
using ModCore.Events.Interfaces.Game;
using ModCore.Storage;

namespace CoreLibrary.Core.Utilities
{
    public class MobGroupHelper
    {
        private readonly List<string> groups = new List<string>
        {
            "Melee",
            "Ranged",
            "Flying",
            "Support",
            "MiniBoss",
            "Boss",
            "Other",
            "WIP",
            "SideKick"
        };


        private readonly Dictionary<string, List<string>> allMobs = new();


        private readonly Dictionary<string, string> mob2GroupLookup = new();

        public void Refresh()
        {
            allMobs.Clear();
            mob2GroupLookup.Clear();

            var mobData = Data.Class.mob;
            int count = mobData.all.get_length();

            for (int i = 0; i < count; i++)
            {
                var item = mobData.all.getDyn(i);
                var groupIndex = (int)item.group;
                var groupName = groups[groupIndex];
                var mobId = item.id.ToString();


                if (mob2GroupLookup.ContainsKey(mobId))
                    continue;


                if (!allMobs.TryGetValue(groupName, out var list))
                {
                    list = new List<string>();
                    allMobs[groupName] = list;
                }
                list.Add(mobId);


                mob2GroupLookup[mobId] = groupName;
            }
        }


        public bool IsBoss(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) &&
                   (group == "Boss" || group == "MiniBoss");
        }


        public bool IsRealBoss(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Boss";
        }

        public bool IsMelee(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Melee";
        }


        public bool IsRanged(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Ranged";
        }


        public bool IsFlying(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Flying";
        }


        public bool IsSupport(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Support";
        }


        public bool IsOther(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "Other";
        }


        public bool IsWIP(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "WIP";
        }


        public bool IsSideKick(string mobId)
        {
            if (string.IsNullOrEmpty(mobId))
                return false;

            return mob2GroupLookup.TryGetValue(mobId, out var group) && group == "SideKick";
        }

    }
}