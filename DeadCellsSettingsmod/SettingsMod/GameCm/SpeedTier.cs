using dc;
using dc.en;
using dc.en.inter;
using dc.tool;
using dc.ui;
using HaxeProxy.Runtime;

namespace ChiuYiUI.GameCm
{
    public class SpeedTier : GameCinematic
    {
        private bool Isdestory;

        public SpeedTier(Hero owen, Entity e, InventItem item)
        {
            owen.cancelVelocities();
            this.Isdestory = IsShrine(e);
            if (!this.Isdestory) e.visible = false;
            CreateCm(item,owen,e);
        }

        public void CreateCm(InventItem item,Hero hero ,Entity entity)
        {
            this.cm.__beginNewQueue();

            var actions = new HlAction[]
            {
                new(() => destroyItem(this.Isdestory, entity)),
                new(() => HUD.Class.ME.show(null)),
                new(() => this.hideBars(null)),
                new(() => hero.applyItemPickEffect(entity, item)),
                new(() => hero.setAffectS(5, 0.3, Ref<double>.Null, null)),
                new(() => this.destroyed = true)
            };

            foreach (var action in actions)
            {
                this.cm.__add(action, 0, null);
            }
        }


        public void destroyItem(bool fromShrine, Entity e)
        {
            if (!fromShrine)
            {
                e.destroy();
                return;
            }

            switch (e)
            {
                case UpgradeShrine upgradeShrine:
                    upgradeShrine.breakIt();
                    break;
                case RunicShrine runicShrine:
                    runicShrine.breakIt();
                    break;
                case ItemAltar itemAltar:
                    itemAltar.disable(true);
                    break;
            }
        }


        public override void onDispose()
        {
            base.onDispose();
        }


        public bool IsShrine(Entity entity)
        {
            return Std.Class.@is(entity, UpgradeShrine.Class) ||
                   Std.Class.@is(entity, ItemAltar.Class) ||
                   Std.Class.@is(entity, RunicShrine.Class);
        }
    }
}