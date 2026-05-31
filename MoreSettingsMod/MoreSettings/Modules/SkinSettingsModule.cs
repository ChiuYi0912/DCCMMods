using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.inter;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.cine;
using MoreSettings.GameMechanics.CustomPopDamage;

namespace MoreSettings.Modules
{
    public class SkinSettingsModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Skin");

        public override SkinConfig config => (SkinConfig)base.config;
        public override Enums.MenuCategory Type => Enums.MenuCategory.Skin;
        public EntityPopDamage entityPop = default!;
        public PopConfig popConfig = default!;

        public override void Initialize(ModBase mainMod)
        {
            entityPop = new EntityPopDamage();
            popConfig = entityPop.Config.Value;
            config = SettingsMain.ConfigValue.Skin;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            var PopmenuHelper = new CoreLibrary.Core.Utilities.OptionsMenuHelper<PopConfig>(options, entityPop.Config);
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;

            var widget = PopmenuHelper.AddConfigToggle(
                 GetText.Instance.GetString("GenuinePopDamage"),
                 "",
                 () => popConfig.GenuinePopDamage,
                 v => popConfig.GenuinePopDamage = v,
                 scrollerFlow
             );
            PopmenuHelper.CenterToggleWidget(widget, options, scrollerFlow);

            var nopopwidget =menuHelper.AddConfigToggle(
                GetText.Instance.GetString("NoPopTextDesc"),
                "",
                () => config.HasNoPopText,
                v =>
                {
                    HasUiSettingsModule.SetConsoleFlag(v, "NoPopText");
                    config.HasNoPopText = v;
                },
                scrollerFlow
            );
            PopmenuHelper.CenterToggleWidget(nopopwidget, options,scrollerFlow);


            PopmenuHelper.AddConfigToggle(
                GetText.Instance.GetString("StsCritEffect"),
                GetText.Instance.GetString("StsCritEffectDesc"),
                () => popConfig.StsPopDamage,
                v =>
                {
                    popConfig.StsPopDamage = v;
                    config.StsSkin = v;
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );

            if (popConfig.StsPopDamage)
            {
                PopmenuHelper.AddConfigSlider(
                GetText.Instance.GetString("CritEffectDuration"),
                () => popConfig.StsSpeedMultiplier,
                v => popConfig.StsSpeedMultiplier = v,
                step: 0.1,
                minValue: 0,
                maxValue: 10,
                scrollerFlow: scrollerFlow
                );
            }

            PopmenuHelper.AddConfigToggle(
                GetText.Instance.GetString("GradientCritEffect"),
                GetText.Instance.GetString("GradientCritEffectDesc"),
                () => popConfig.RevealPop,
                v =>
                {
                    popConfig.RevealPop = v;
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );

            if (popConfig.RevealPop)
            {
                PopmenuHelper.AddConfigSlider(
                  GetText.Instance.GetString("CritEffectDuration"),
                  () => popConfig.RevealSpeedMultiplier,
                  v => popConfig.RevealSpeedMultiplier = v,
                  step: 0.1,
                  minValue: 0,
                  maxValue: 10,
                  scrollerFlow: scrollerFlow
              );
            }


            PopmenuHelper.AddConfigToggle(
                GetText.Instance.GetString("HotlineCritEffect"),
                GetText.Instance.GetString("HotlineCritEffectDesc"),
                () => popConfig.HotlinePopDamage,
                v =>
                {
                    popConfig.HotlinePopDamage = v;
                    config.HotlineSkin = v;
                    options.setSection(options.curSection);
                },
                scrollerFlow
            );

            if (popConfig.HotlinePopDamage)
            {
                PopmenuHelper.AddConfigSlider(
                  GetText.Instance.GetString("CritEffectDuration"),
                  () => popConfig.HotlineSpeedMultiplier,
                  v => popConfig.HotlineSpeedMultiplier = v,
                  step: 0.1,
                  minValue: 0,
                  maxValue: 10,
                  scrollerFlow: scrollerFlow
              );
            }


            

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("RiskOfRainTeleport"),
                GetText.Instance.GetString("RiskOfRainTeleportDesc"),
                () => config.RiskOfRainSkin,
                v => config.RiskOfRainSkin = v,
                scrollerFlow: scrollerFlow
                );

            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("KatanaZeroOutfit"),
                GetText.Instance.GetString("KatanaZeroOutfitDesc"),
                () => config.KatanaSkin,
                v => config.KatanaSkin = v,
                scrollerFlow: scrollerFlow
                );

            
        }



        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Hero.hasSkin += Hook_Hero_hasSkin;
        }

        public override void PermanentlyRegisterHooks()
        {
            Hook_Teleport.startTeleport += Hook_Teleport_startTeleport;
        }


        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_Hero.hasSkin -= Hook_Hero_hasSkin;
        }

        private bool Hook_Hero_hasSkin(Hook_Hero.orig_hasSkin orig, Hero self, dc.String model, dc.String itemId)
        {
            if (itemId != null)
                if (config.KatanaSkin && itemId.ToString() == "KatanaZero") return true;
            return orig(self, model, itemId);
        }

        private void Hook_Teleport_startTeleport(Hook_Teleport.orig_startTeleport orig, Teleport self, Hero hero, Entity to)
        {
            if (to == null) return;
            if (hero.hasSkin(null, "RiskOfRain".ToHaxeString()) || config.RiskOfRainSkin)
            { _ = new TeleportationRiskOfRain(hero, self, to, !SettingsMain.ConfigValue.Viewport.TeleportImmediate); return; }
            _ = new Teleportation(hero, self, to, !SettingsMain.ConfigValue.Viewport.TeleportImmediate);
        }

    }
}