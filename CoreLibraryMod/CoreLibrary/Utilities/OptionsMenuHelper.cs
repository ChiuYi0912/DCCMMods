using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace CoreLibrary.Core.Utilities
{
    public class OptionsMenuHelper<T> where T : new()
    {
        private readonly Options opt;
        private readonly Config<T> config;

        public OptionsMenuHelper(Options options, Config<T> config)
        {
            this.opt = options;
            this.config = config;
        }

        public OptionWidget AddConfigToggle(
            string title,
            string description,
            Func<bool> getter,
            Action<bool> setter,
            dc.h2d.Flow scrollerFlow)
        {
           var widget = opt.addToggleWidget(
                title.ToHaxeString(),
                description.ToHaxeString(),
                () =>
                {
                    bool newValue = !getter();
                    setter(newValue);
                    config.Save();
                    return newValue;
                },
                Ref<bool>.In(getter()),
                scrollerFlow
            );

            return widget;
        }


        public void AddConfigSlider(
            string title,
            Func<double> getter,
            Action<double> setter,
            double step = 1.0,
            double minValue = 0.0,
            double maxValue = 100.0,
            bool showPercent = false,
            dc.h2d.Flow scrollerFlow = null!,
            string buttonText = null!,
            int paddingLeft = 0)
        {
            HlAction<double> onUpdate = new(v =>
            {
                setter(v);
                config.Save();
            });
            bool showRawValue = true;

            opt.addSliderWidget(
                title.ToHaxeString(),
                onUpdate,
                getter(),
                Ref<double>.In(step),
                scrollerFlow,
                Ref<bool>.In(showPercent),
                Ref<bool>.In(showRawValue),
                Ref<double>.In(minValue),
                Ref<double>.In(maxValue),
                buttonText?.ToHaxeString(),
                Ref<int>.In(paddingLeft)
            );
        }
    }
}