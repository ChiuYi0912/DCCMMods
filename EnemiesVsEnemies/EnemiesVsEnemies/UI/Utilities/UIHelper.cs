using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using dc.ui.icon;

namespace EnemiesVsEnemies.UI.Utilities
{
    public static class UIMobHelper
    {


        public static dynamic getmobs(int index)
        {
            var arr = Data.Class.mob.all.array;

            if (index < 0 || index >= arr.length)
                return null!;

            return arr.getDyn(index);
        }


        public static string getMobNamebyid(string id)
        {
            string data = Data.Class.mob.byId.get(id.ToHaxeString()).name.ToString();
            return Lang.Class.t.get(data.ToHaxeString(), null).ToString();
        }


        public static int getmoblength() => Data.Class.mob.all.array.length;
    }
}