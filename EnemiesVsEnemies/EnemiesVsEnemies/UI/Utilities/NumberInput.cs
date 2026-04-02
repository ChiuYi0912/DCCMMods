using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.en.active;
using dc.h2d;
using dc.hxd.res;
using dc.ui;
using HaxeProxy.Runtime;
using Text = dc.ui.Text;

namespace EnemiesVsEnemies.UI.Utilities
{
    public class NumberInput
    {
        public dc.ui.TextInput Input = null!;


        public dc.ui.TextInput OpenNumberInput(dc.ui.Process process, string title, string subTitle, string initial, Action<string> onValidate, string okLabel = "回车确定", string cancelLabel = "鼠标点击取消", Sound? sfx = null)
        {
            var text = new dc.ui.TextInput(
                process,
                title.ToHaxeString(),
                subTitle.ToHaxeString(),
                initial.ToString().ToHaxeString(),
                new HlAction<dc.String>(s =>
                {
                    onValidate(s.ToString());
                }),
                okLabel.ToHaxeString(),
                cancelLabel.ToHaxeString(),
                sfx);

            Input = text;
            return text;
        }


        public dc.ui.TextInput OpenNumberInputInt(dc.ui.Process process, string title, string subTitle, string initial, Action<int> onValidate, string okLabel = "回车确定", string cancelLabel = "鼠标点击取消", Sound? sfx = null)
        {
            return OpenNumberInput(process, title, subTitle, initial, s =>
            {
                if (int.TryParse(s, out int result))
                {
                    onValidate(result);
                }
                else
                {
                    OpenNumberInputInt(process, title, subTitle, s, onValidate, okLabel, cancelLabel, sfx);
                }
            }, okLabel, cancelLabel, sfx);
        }


        public dc.ui.TextInput OpenNumberInputFloat(dc.ui.Process process, string title, string subTitle, string initial, Action<float> onValidate, string okLabel = "回车确定", string cancelLabel = "鼠标点击取消", Sound? sfx = null)
        {
            return OpenNumberInput(process, title, subTitle, initial, s =>
            {
                if (float.TryParse(s, out float result))
                {
                    onValidate(result);
                }
                else
                {
                    OpenNumberInputFloat(process, title, subTitle, s, onValidate, okLabel, cancelLabel, sfx);
                }
            }, okLabel, cancelLabel, sfx);
        }
    }
}