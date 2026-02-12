using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiuYiUI;
using dc;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace SettingsMod.Settings
{
    public class DisplayUI
    {
        public DisplayUI()
        {
            Initialize();
        }
        public void Initialize()
        {
            dc.ui.hud.Hook_LifeBar.getFullName += Hook_LifeBar_getfullname;
            dc.ui.hud.Hook_LifeBar.getStartEndName += Hook_LifeBar_getStartEndName;
        }

        private dc.String Hook_LifeBar_getStartEndName(dc.ui.hud.Hook_LifeBar.orig_getStartEndName orig, dc.ui.hud.LifeBar self)
        {
            switch (self.colorMode.RawIndex)
            {
                case 0:
                    dc.String iscolor = transparent[(int)CHIUYIMain.config.Value.LifeBarcolor].AsHaxeString();
                    return iscolor;
                case 1:
                    return "lifeBossStartEnd".AsHaxeString();
                case 2:
                    return "lifeBossModifiedStartEnd".AsHaxeString();
                default:
                    return null!;
            }
        }

        private dc.String Hook_LifeBar_getfullname(dc.ui.hud.Hook_LifeBar.orig_getFullName orig, dc.ui.hud.LifeBar self)
        {

            switch (self.colorMode.RawIndex)
            {
                case 0:

                    dc.String iscolor = darker[(int)CHIUYIMain.config.Value.LifeBarcolor].AsHaxeString();
                    return iscolor;
                case 1:
                    return "lifeFullBoss".AsHaxeString();
                case 2:
                    return "lifeFullBossModified".AsHaxeString();
                default:
                    return null!;
            }
        }

        public List<string> darker = new()
        {
            "lifeFull",
            "gold_gradient_darker",
            "blue_gradient_darker",
            "pink_gradient_darker",
            "purple_gradient_darker",
            "red_gradient_darker",
            "white_gradient_darker"
        };

        public List<string> transparent = new()
        {
            "lifeStartEnd",
            "gold_gradient_transparent",
            "blue_gradient_transparent",
            "pink_gradient_transparent",
            "purple_gradient_transparent",
            "red_gradient_transparent",
            "white_gradient_transparent"
        };

        public void addSetting(dc.ui.Options self)
        {

            var scrollerFlow = self.scrollerFlow;
            var string4 = Lang.Class.t.get("血条颜色".AsHaxeString(), null);
            self.addSeparator(string4, scrollerFlow);
            var string3 = Lang.Class.t.get("0:原版,1:金色,2:蓝色,3:粉色,4:紫色,5:红色,6:白色".AsHaxeString(), null);
            double num = CHIUYIMain.config.Value.LifeBarcolor;
            scrollerFlow = self.scrollerFlow;

            var defaultValue = new HlAction<double>((double v) =>
           {
               CHIUYIMain.config.Value.LifeBarcolor = v;
               CHIUYIMain.config.Save();
           });
            double b = 1;
            bool showRawValue = false;
            bool showRawValue1 = true;
            double minValue = 0;
            double maxValue = 6;
            var setting = self.addSliderWidget(
              string3,
              defaultValue,
              num,
              new Ref<double>(ref b),
              scrollerFlow,
              new Ref<bool>(ref showRawValue),
              new Ref<bool>(ref showRawValue1),
              new Ref<double>(ref minValue),
              new Ref<double>(ref maxValue),
              null,
              new Ref<int>());
        }


    }
}