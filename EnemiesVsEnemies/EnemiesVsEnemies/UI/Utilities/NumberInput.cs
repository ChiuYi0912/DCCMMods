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
        public dc.ui.Process GetProcess = null!;
        public NumberInput(dc.ui.Process p)
        {
            GetProcess = p;

        }

        public dc.ui.TextInput OpenNumberInput(string title, string subTitle, int initial, Action<int> onValidate, string okLabel = "回车确定", string cancelLabel = "EXC取消", Sound? sfx = null, int min = 1, int max = 9)
        {
            var text = new dc.ui.TextInput(
                 GetProcess,
                 title.ToHaxeString(),
                 subTitle.ToHaxeString(),
                 initial.ToString().ToHaxeString(),
                 new HlAction<dc.String>(s =>
                 {
                     string text = s?.ToString() ?? "";
                     if (int.TryParse(text, out int value))
                     {
                         value = Math.Clamp(value, min, max);
                         onValidate(value);
                     }
                     else
                     {

                         var str = "请输入有效数字".ToHaxeString();
                     }
                 }),
                 okLabel.ToHaxeString(),
                 cancelLabel.ToHaxeString(),
                 sfx);

            return text;
        }


    }
}