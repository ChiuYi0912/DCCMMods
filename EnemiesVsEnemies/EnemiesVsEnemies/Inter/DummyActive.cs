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
            SpriteLib gameElements = Assets.Class.gameElements;
            base.initSprite(gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
        }
        public override void fixedUpdate() { base.fixedUpdate(); }

        public override void destroy()
        {
            base.destroy();
        }
    }
}