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
using PopDamage.OtherPop;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Modules
{
    public class SkinSettingsModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Skin");
        public override SkinConfig config => (SkinConfig)base.config;
        public override Enums.MenuCategory Type => Enums.MenuCategory.Skin;
        public EntityPopDamage entityPop = default!;
        public PopConfig popConfig = default!;

        public List<(string title, string sub, Action onSelect, Action after)> Teleportdata = [
        (
            "Defaults",
            "Auto",
            ()=>SettingsMain.ConfigValue.Skin.TeleportStyle = TeleportStyle.orig,
            ()=>{ }
        ),
        (
            "Classic",
            "",
            () => SettingsMain.ConfigValue.Skin.TeleportStyle = TeleportStyle.Default,
            () => {}
        ),
        (
            "RiskOfRainTeleport",
            "",
            () => SettingsMain.ConfigValue.Skin.TeleportStyle = TeleportStyle.RiskOfRain,
            () => {}
        ),
        (
            "Instant",
            "",
            () => SettingsMain.ConfigValue.Skin.TeleportStyle = TeleportStyle.Instant,
            () => {}
        )];

        public override void Initialize(ModBase mainMod)
        {
            entityPop = new EntityPopDamage();
            popConfig = EntityPopDamage.popconfig;
            config = SettingsMain.ConfigValue.Skin;
            base.Initialize(mainMod);
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            var PopmenuHelper = new CoreLibrary.Core.Utilities.OptionsMenuHelper<PopConfig>(options, EntityPopDamage.Config);
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;
            menuHelper.AddSubSeparator(GetString("PopDamage"), scrollerFlow);

            var widget = PopmenuHelper.AddConfigToggle(
                 GetString("GenuinePopDamage"),
                 "",
                 () => popConfig.GenuinePopDamage,
                 v => popConfig.GenuinePopDamage = v,
                 scrollerFlow
             );
            PopmenuHelper.CenterToggleWidget(widget, options, scrollerFlow);

            var nopopwidget = menuHelper.AddConfigToggle(
                GetString("NoPopTextDesc"),
                "",
                () => config.HasNoPopText,
                v =>
                {
                    HasUiSettingsModule.SetConsoleFlag(v, "NoPopText");
                    config.HasNoPopText = v;
                },
                scrollerFlow
            );
            PopmenuHelper.CenterToggleWidget(nopopwidget, options, scrollerFlow);


            var optionsdata = new List<(string title, string sub, Action onSelect, Action after)>
            {
                ("Defaults", "Auto", () => EntityPopDamage.ForcedHandler = null! ,()=>{ })
            };
            foreach (var handler in PopDamageHandlerRegistry.GetAll().Reverse())
            {
                var h = handler;
                optionsdata.Add((
                    GetString(h.OptionsTitle),
                    GetString(h.SubStr),
                    () => EntityPopDamage.ForcedHandler = h,
                    () =>
                    {
                        if (EntityPopDamage.ForcedHandler is DefaultPopDamageHandler) return;
                        PopmenuHelper.AddConfigSlider(
                            GetText.Instance.GetString("CritEffectDuration"),
                            () => h.SpeedMultiplier,
                            v => h.SpeedMultiplier = v,
                            step: 0.1,
                            minValue: 0,
                            maxValue: 3,
                            scrollerFlow: scrollerFlow
                        );
                    }
                ));
            }
            PopmenuHelper.AddConfigRadioGroup(
                optionsdata,
                popConfig.index,
                (v) => popConfig.index = v,
                scrollerFlow
            );


            menuHelper.AddSubSeparator(GetString("Teleport"), scrollerFlow);
            
            var Teleportwidget = menuHelper.AddConfigToggle(
                GetString("SmoothTeleport"),
                GetString(""),
                () => config.TeleportImmediate,
                v => config.TeleportImmediate = v,
                scrollerFlow: options.scrollerFlow
                );
            menuHelper.CenterToggleWidget(Teleportwidget, options, options.scrollerFlow);

            menuHelper.AddConfigRadioGroup(
                Teleportdata,
                (int)config.TeleportStyle,
                (v) => config.TeleportStyle = (TeleportStyle)v,
                scrollerFlow
            );


            menuHelper.AddSubSeparator(GetString("Others"), scrollerFlow);
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
            if (to == null)
                return;

            switch (config.TeleportStyle)
            {
                case TeleportStyle.orig:
                    if (hero.hasSkin(null, "RiskOfRain".ToHaxeString()))
                    {
                        _ = new TeleportationRiskOfRain(hero, self, to, !config.TeleportImmediate); return;
                    }
                    _ = new Teleportation(hero, self, to, !config.TeleportImmediate);
                    break;

                case TeleportStyle.Default:
                    _ = new Teleportation(
                        hero,
                        self,
                        to,
                        !config.TeleportImmediate);
                    break;

                case TeleportStyle.RiskOfRain:
                    _ = new TeleportationRiskOfRain(
                        hero,
                        self,
                        to,
                        !config.TeleportImmediate);
                    break;

                    case TeleportStyle.Instant:
                        _ = new TeleportationFancy(
                            hero,
                            self,
                            to,
                            !config.TeleportImmediate);
                        break;

            }
        }

    }
}