using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using HaxeProxy.Runtime;
using ModCore.Storage;
using static EnemiesVsEnemies.Inter.DummyActive;

namespace EnemiesVsEnemies.Inter
{

    public class DummyActive : Active
    {

        public TeamSelector Selector = null!;

        public DummyActive(Hero h, int cx, int cy, InventItem i) : base(h, cx, cy, i)
        {
            Selector = new TeamSelector(h._level, cx, cy);
            Selector.init();
            destroy();
        }

        public override void initGfx()
        {
            base.initGfx();
            base.initSprite(Assets.Class.gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
        }
    }
}