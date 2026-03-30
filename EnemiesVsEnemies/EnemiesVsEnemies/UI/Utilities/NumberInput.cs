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
        public NumberInput()
        {
 
        }
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


    }
}