using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.mob;
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
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using EnemiesVsEnemies.UI.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Storage;
using Serilog;
using Data = dc.Data;
using Interactive = dc.h2d.Interactive;
using Text = dc.ui.Text;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public dc.ui.Text nameText = null!;
        public NumberInput GetU = null!;
        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;
        public TeamManager GetTeam = null!;
        public CricketSelectorGui(TeamManager teamManager)
        {
            GetTeam = teamManager;
        }

        public override int get_wid() => 6;
        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;

        public int istest = 1;

        public dynamic getmobs(int index)
        {
            var arr = Data.Class.mob.all.array;

            if (index < 0 || index >= arr.length)
                return null!;

            return arr.getDyn(index);
        }
        public int getmoblength() => Data.Class.mob.all.array.length;

        public override void initGrid()
        {
            curX = curY = 0;
            base.initEntries(getmoblength());
        }

        public override bool isEntryLocked(int i) => false;

        public override void initRightFlow()
        {
            double padH = 5.0;
            double padV = 5.0;
            var rightFlow = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref padH), Ref<double>.From(ref padV), Ref<bool>.Null, null);
            base.rightFlow = rightFlow;

            base.rightFlow.set_isVertical(true);
            base.rightFlow.set_horizontalAlign(new FlowAlign.Middle());
            base.rightFlow.set_verticalAlign(new FlowAlign.Middle());

            base.mainFlow.addChild(base.rightFlow);

            nameText = Assets.Class.makeText(Lang.Class.t.untranslated(""), null, true, null);
            nameText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            nameText.set_textAlign(new Align.MultilineCenter());

            base.root.addChild(nameText);


            //var info = Assets.Class.makeText();

            // GetU = new NumberInput(this);
            // var action = new Action<int>((value) =>
            // {
            //     istest = value;
            // });
            // var inpu = GetU.OpenNumberInput("TEST", "hello", 1, action);
        }

        public void MakeTeaminfo()
        {
        }
        public override void updateRightFlow() { }
        public override void beforeUpdateSelection() { }




        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            string name = getmobs(i).id.ToString();
            if (name.IsNullOrEmpty())
                return new Bitmap(Tile.Class.fromColor(0xFF0000, 32, 32, null, null), parent);

            var icon = Icon.Class.createMobIcon(name.ToHaxeString(), parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());
            return icon;
        }
    }
}