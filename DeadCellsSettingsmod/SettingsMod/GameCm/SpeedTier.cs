using dc;
using dc.en;
using dc.en.inter;
using dc.en.mob;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.level;
using dc.libs.heaps.slib;
using dc.libs.heaps.slib._AnimManager;
using dc.libs.misc;
using dc.pr;
using dc.tool;
using dc.ui;
using dc.ui.icon;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace ChiuYiUI.GameCm
{
    public class SpeedTier : GameCinematic
    {
        private Icon icon = null!;
        private bool destory;
        private Entity entity = null!;
        private Hero hero = null!;
        public SpeedTier(Hero owen, Entity e, InventItem item)
        {
            this.entity = e;
            this.hero = owen;
            owen.cancelVelocities();
            this.destory = IsShrine(e);
            if (!this.destory)
            {
                e.visible = false;
            }
            createCm(item);
        }

        public void createCm(InventItem item)
        {
            this.cm.__beginNewQueue();
            HlAction ac1 = new HlAction(() => { destroyItem(this.destory, this.entity); });
            this.cm.__add(ac1, 0, null);

            HlAction ac29 = new(() => { HUD.Class.ME.show(null); });
            this.cm.__add(ac29, 0, null);

            HlAction ac30 = new(() => { this.hideBars(null); });
            this.cm.__add(ac30, 0, null);

            HlAction ac32 = new(() => { this.hero.applyItemPickEffect(this.entity, item); });
            this.cm.__add(ac32, 0, null);

            HlAction ac38 = new(() => { this.hero.setAffectS(5, 0.3, Ref<double>.Null, null); });
            this.cm.__add(ac38, 0, null);

            HlAction ac39 = new(() => { this.destroyed = true; });
            this.cm.__add(ac39, 0, null);
        }


        public void destroyItem(bool fromShrine, Entity e)
        {
            if (fromShrine)
            {
                if (Std.Class.@is(e, UpgradeShrine.Class))
                {
                    ((UpgradeShrine)e).breakIt();
                }
                else
                {
                    if (Std.Class.@is(e, RunicShrine.Class))
                    {
                        ((RunicShrine)e).breakIt();
                    }
                    else
                    {
                        if (Std.Class.@is(e, ItemAltar.Class))
                        {
                            ((ItemAltar)e).disable(true);
                        }
                    }
                }
                return;
            }
            e.destroy();
        }


        public override void onDispose()
        {
            base.onDispose();
            Icon icon = this.icon;
            if (icon == null || icon.parent == null)
            {
                this.icon = null!;
                Game.Class.ME.hero.hasGravity = true;
                return;
            }
            icon.parent.removeChild(icon);
            this.icon = null!;
            Game.Class.ME.hero.hasGravity = true;
        }


        public bool IsShrine(Entity entity)
        {
            return Std.Class.@is(entity, UpgradeShrine.Class) ||
                   Std.Class.@is(entity, ItemAltar.Class) ||
                   Std.Class.@is(entity, RunicShrine.Class);
        }



    }
}