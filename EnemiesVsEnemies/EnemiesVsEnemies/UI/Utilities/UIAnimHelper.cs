using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.h2d;
using dc.h2d.col;
using dc.libs.misc;
using dc.ui.icon;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using static EnemiesVsEnemies.UI.CricketSelectorGui;

namespace EnemiesVsEnemies.UI.Utilities
{
    public static class UIAnimHelper
    {
        private static List<Flow> remove = new();
        public static void doMovementIcon(CricketSelectorGui gui, dc.h2d.Object to, dc.h2d.Object from, Icon icon, bool add)
        {
            double pixelScale = gui.get_pixelScale.Invoke();

            Flow cloneflow = new Flow(null);
            var cloneicon = icon;
            cloneflow.addChild(cloneicon);
            if (add)
            {
                cloneicon.tile.scaleToSize(72, 72);
            }
            else
            {
                cloneicon.posChanged = true;
                cloneicon.scaleX = pixelScale;
                cloneicon.posChanged = true;
                cloneicon.scaleY = pixelScale;
            }

            gui.root.addChild(cloneflow);

            Main main = Main.Class.ME;
            const double speed = 800;


            Point oldflowGlobal = main.localToGlobal(from, Ref<double>.Null, Ref<double>.Null);
            Point textGlobal = main.localToGlobal(to, Ref<double>.Null, Ref<double>.Null);


            Point startLocal = gui.root.globalToLocal(oldflowGlobal);
            Point targetLocal = gui.root.globalToLocal(textGlobal);

            cloneflow.x = startLocal.x;
            cloneflow.y = startLocal.y;

            var tw = gui.tw;
            var tweenX = CreateTween(tw,
                () => cloneflow.x,
                value =>
                {
                    cloneflow.x = value;
                    cloneflow.posChanged = true;
                },
                targetLocal.x, speed);

            var tweenY = CreateTween(tw,
                () => cloneflow.y,
                value =>
                {
                    cloneflow.y = value;
                    cloneflow.posChanged = true;
                },
                targetLocal.y, speed);


            tweenX.end(() =>
            {
                cloneflow.remove();
                remove.Remove(cloneflow);
            });

            if (remove.Count >= 3)
            {
                Flow oldest = remove[0];
                oldest.remove();
                remove.RemoveAt(0);
            }
            remove.Add(cloneflow);
        }

        private static Tween CreateTween(Tweenie tween, Func<double> getter, Action<double> setterAction, double targetValue, double? duration)
        {
            var hlGetter = new HlFunc<double>(getter);
            var hlSetter = new HlAction<double>(setterAction);
            var tweenType = new TType.TEaseOut();
            return tween.create_(hlGetter, hlSetter, null, targetValue, tweenType, duration, Ref<bool>.Null);
        }
    }
}