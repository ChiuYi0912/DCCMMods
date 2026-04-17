using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Utilities.CustomPopDamage;
using dc;
using dc.tool.atk;
using dc.ui;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics;

namespace MoreSettings.Modules
{
    public class SkinSettingsModule : BaseModule
    {
        public override string Description => "UI界面修改";

        public override SkinConfig config => (SkinConfig)base.config;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ConfigValue.Skin;
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            var PopmenuHelper = new CoreLibrary.Core.Utilities.OptionsMenuHelper<PopConfig>(options, SettingsMain.entityPop.Config);
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;


            PopmenuHelper.AddConfigToggle(
                GetText.Instance.GetString("杀戮尖塔暴击特效"),
                GetText.Instance.GetString("启用/禁用杀戮尖塔暴击特效"),
                () => SettingsMain.entityPop.Config.Value.StsPopDamage,
                v =>
                {
                    SettingsMain.entityPop.Config.Value.StsPopDamage = v;
                    config.StsSkin = v;
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );

            if (config.StsSkin)
            {
                PopmenuHelper.AddConfigSlider(
                GetText.Instance.GetString("暴击特效持续显示时间"),
                () => SettingsMain.entityPop.Config.Value.StsSpeedMultiplier,
                v => SettingsMain.entityPop.Config.Value.StsSpeedMultiplier = v,
                step: 10,
                minValue: 0,
                maxValue: 5000,
                scrollerFlow: scrollerFlow
                );
            }




            PopmenuHelper.AddConfigToggle(
                GetText.Instance.GetString("迈阿密热线暴击特效"),
                GetText.Instance.GetString("启用/禁用迈阿密热线暴击特效"),
                () => SettingsMain.entityPop.Config.Value.HotlinePopDamage,
                v =>
                {
                    SettingsMain.entityPop.Config.Value.HotlinePopDamage = v;
                    config.HotlineSkin = v;
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );

            if (config.HotlineSkin)
            {
                PopmenuHelper.AddConfigSlider(
                  GetText.Instance.GetString("暴击特效持续显示时间"),
                  () => SettingsMain.entityPop.Config.Value.HotlineSpeedMultiplier,
                  v => SettingsMain.entityPop.Config.Value.HotlineSpeedMultiplier = v,
                  step: 10,
                  minValue: 0,
                  maxValue: 5000,
                  scrollerFlow: scrollerFlow
              );
            }
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();

        }

        public override void UnregisterHooks()
        {
            base.UnregisterHooks();

        }


    }
}