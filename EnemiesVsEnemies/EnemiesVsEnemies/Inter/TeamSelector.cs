using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using EnemiesVsEnemies.UI;

namespace EnemiesVsEnemies.Inter
{
    public class TeamSelector : Interactive
    {
        public TeamSelector(Level lvl, int x, int y) : base(lvl, x, y)
        {
            xr = 0.5;
        }

        public override void initGfx()
        {
            base.initGfx();
            SpriteLib gameElements = Assets.Class.gameElements;
            base.initSprite(gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
        }

        public override void onActivate(Hero by, bool longPress)
        {
            base.onActivate(by, longPress);
            _ = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager());
        }

        public override void onFocus()
        {
            base.onFocus();
            var lightTip = base.createLightTip(null);
            lightTip.distance = 24.0;
            dc.String str = "设置团队".ToHaxeString();
            lightTip.addActivate(str, null, null);
        }
    }



}