using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.h2d;
using dc.h2d.col;
using dc.libs.misc;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using static EnemiesVsEnemies.UI.CricketSelectorGui;

namespace EnemiesVsEnemies.UI.Utilities
{
    public static class UIAnimHelper
    {
        private static List<Flow> remove = new();
        public static void doMovementIcon(CricketSelectorGui gui, Text text, virtual_cx_cy_f_i_isLocked_sectionIdx_ seledata, MonsterSelectionEventArgs args)
        {
            double pixelScale = gui.get_pixelScale.Invoke();
            Flow oldflow = seledata.f;


            Flow cloneflow = new Flow(null);
            var cloneicon = gui.getIconBmp(seledata.i, cloneflow);
            cloneicon.posChanged = true;
            cloneicon.scaleX = pixelScale;
            cloneicon.posChanged = true;
            cloneicon.scaleY = pixelScale;

            gui.mask.addChild(cloneflow);

            Main main = Main.Class.ME;
            const double speed = 800;


            Point oldflowGlobal = main.localToGlobal(oldflow, Ref<double>.Null, Ref<double>.Null);
            Point startLocal = gui.mask.globalToLocal(oldflowGlobal);


            Point textGlobal = main.localToGlobal(text, Ref<double>.Null, Ref<double>.Null);
            Point targetLocal = gui.mask.globalToLocal(textGlobal);


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