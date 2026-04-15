using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.libs.misc;
using HaxeProxy.Runtime;

namespace MoreSettings.Utilities
{
    public static class AnimHelper
    {
        public static Tween CreateTween(Tweenie tween, Func<double> getter, Action<double> setterAction, double targetValue, double? duration)
        {
            var hlGetter = new HlFunc<double>(getter);
            var hlSetter = new HlAction<double>(setterAction);
            var tweenType = new TType.TEaseOut();
            return tween.create_(hlGetter, hlSetter, null, targetValue, tweenType, duration, Ref<bool>.Null);
        }
    }
}