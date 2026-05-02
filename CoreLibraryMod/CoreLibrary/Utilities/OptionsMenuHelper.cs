using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.h2d;
using dc.libs.heaps.slib;
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

        public OptionWidget addSimpleWidget(
            string title,
            string description,
            Action action,
            dc.h2d.Flow parentFlow,
            int offsetX = 5)
        {
            var widget = opt.addSimpleWidget(
                title.ToHaxeString(),
                description.ToHaxeString(),
                new HlAction(action),
                Ref<int>.In(offsetX),
                parentFlow
            );
            return widget;
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


        public OptionWidget AddConfigSlider(
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

            var Slider = opt.addSliderWidget(
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

            return Slider;
        }


        public OptionWidget AddHSVColorWidget(
            string title,
            string subtitle,
            Func<bool> onVal,
            bool enabled,
            Action<int> onUpdate,
            int defaultValue,
            dc.h2d.Flow scrollerFlow)
        {

            HlAction<int> onUpdateWithSave = new(v =>
            {
                onUpdate(v);
                config.Save();
            });

            var widget = opt.addHSVColorWidget(
                title.ToHaxeString(),
                subtitle?.ToHaxeString(),
                new HlFunc<bool>(onVal),
                enabled,
                onUpdateWithSave,
                defaultValue,
                scrollerFlow
            );

            return widget;
        }



        public void CenterToggleWidget(Flow widget, Options options, Flow scrollerFlow)
        {
            var props = scrollerFlow.getProperties(widget);
            props.horizontalAlign = new FlowAlign.Middle();


            widget.maxWidth = scrollerFlow.get_innerWidth();
            widget.horizontalAlign = new FlowAlign.Middle();


            widget.paddingTop = widget.get_innerHeight() / 5;

            var pixel = options.get_pixelScale.Invoke();

            foreach (var child in widget.children.AsEnumerable())
            {
                if (child is HSprite icon)
                {
                    icon.scaleX = icon.scaleY = icon.scaleY / 2;
                }
                else if (child is Flow textFlow)
                {
                    textFlow.maxWidth = widget.maxWidth;
                    textFlow.horizontalAlign = new FlowAlign.Middle();
                    textFlow.verticalAlign = new FlowAlign.Middle();

                    textFlow.scaleX = textFlow.scaleY = textFlow.scaleY / 2;
                }
            }
        }
    }
}