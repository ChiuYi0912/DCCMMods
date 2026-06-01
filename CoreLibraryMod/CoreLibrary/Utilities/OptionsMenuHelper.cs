using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.h2d;
using dc.hl.types;
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

        public OptionWidget AddConfigListWidget(
            string title,
            string substr,
            Action<int> action,
            int curEntry,
            List<string> texts,
            dc.h2d.Flow parentFlow = null!,
            int offsetX = 5
            )
        {
            var haxeTexts = texts.Select(t => t.ToHaxeString()).ToList();
            var widget = opt.addListWidget(
                title.ToHaxeString(),
                substr.ToHaxeString(),
                new HlAction<int>(v => { action(v); config.Save(); }),
                curEntry,
                haxeTexts.Count,
                haxeTexts.ToArrayObj(),
                Ref<int>.In(offsetX),
                parentFlow
            );

            return widget;
        }

        public void AddConfigRadioWidget(
            string title,
            string substr,
            Action action,
            bool InitialBool,
            dc.h2d.Flow parentFlow = null!
            )
        {
            opt.addRadioWidget(
                title.ToHaxeString(),
                substr.ToHaxeString(),
                new(() => { action(); config.Save(); }),
                Ref<bool>.In(InitialBool),
                parentFlow);
        }

        public void AddConfigRadioGroup(
            List<(string title, string sub, Action onSelect, Action AfterAddButton)> items,
            int initiallySelectedIndex,
            Action<int> onIndexChanged,
            dc.h2d.Flow parentFlow = null!
        )
        {
            var stateRefs = new List<bool>();
            var widgets = new List<OptionWidget>();

            for (int i = 0; i < items.Count; i++)
            {
                int currentIndex = i;
                bool isSelected = i == initiallySelectedIndex;
                stateRefs.Add(isSelected);

                HlAction onVal = new HlAction(() =>
                {
                    for (int j = 0; j < stateRefs.Count; j++)
                        stateRefs[j] = false;
                    stateRefs[currentIndex] = true;
                    items[currentIndex].onSelect();
                    onIndexChanged(currentIndex);
                    config.Save();
                });

                var widget = opt.addRadioWidget(
                    items[i].title.ToHaxeString(),
                    items[i].sub.ToHaxeString(),
                    onVal,
                    Ref<bool>.In(isSelected),
                    parentFlow
                );
                if (isSelected)
                    items[i].AfterAddButton();
                    
                widgets.Add(widget);
            }
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