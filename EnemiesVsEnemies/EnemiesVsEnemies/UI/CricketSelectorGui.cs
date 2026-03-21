using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.h2d;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.hxd.snd;
using dc.libs;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using dc.ui;
using dc.ui.icon;
using dc.ui.sel;
using EnemiesVsEnemies.UI.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using Data = dc.Data;
using Text = dc.ui.Text;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public CricketSelectorGui() : base()
        {

        }

        public override int get_wid() => 6;
        public override int get_entryWid() => 32;
        public override int get_entryHei() => 32;

        public dynamic getmobs(int index)
        {
            var arr = Data.Class.mob.all.array;

            if (index < 0 || index >= arr.length)
                return null!;

            return arr.getDyn(index);
        }

        public int getmoblength()
        {
            return Data.Class.mob.all.array.length;
        }

        public override void initGrid()
        {
            this.curX = 0;
            this.curY = 0;

            base.initEntries(getmoblength());
        }

        public override bool isEntryLocked(int i) => false;

        public override void initRightFlow() { }
        public override void updateRightFlow() { }
        public override void beforeUpdateSelection() { }

        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            var mob = getmobs(i);

            if (mob == null)
                return new Bitmap(Tile.Class.fromColor(0xFF0000, 32, 32, null, null), parent);

            var icon = Icon.Class.createMobIcon(mob.id, parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());
            return icon;
        }
    }
}