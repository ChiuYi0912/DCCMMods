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

    public class DummyActive : Active, IHxbitSerializable<DummyActiveData>
    {
        public class DummyActiveData
        {
            public int Cx;
            public int Cy;
            public TeamSelector TeamSelector = null!;
        }

        private DummyActiveData data = new();

        DummyActiveData IHxbitSerializable<DummyActiveData>.GetData()
        {
            if (data == null)
                data = new DummyActiveData();
            data.Cx = cx;
            data.Cy = cy;
            data.TeamSelector = Selector;
            return data;
        }

        void IHxbitSerializable<DummyActiveData>.SetData(DummyActiveData data)
        {
            cx = data.Cx;
            cy = data.Cy;
            Selector = data.TeamSelector;
        }

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
            spr.constraintSize(3, 3);
        }
        public override void fixedUpdate() { base.fixedUpdate(); }
    }
}