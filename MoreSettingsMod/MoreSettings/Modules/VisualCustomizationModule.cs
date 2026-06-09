using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.hero;
using dc.en.inter;
using dc.h2d;
using dc.hl.types;
using dc.shader;
using dc.tool;
using dc.tool._Cooldown;
using Hashlink.Reflection.Types;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.API;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.cine;
using MoreSettings.GameMechanics.CustomPopDamage;
using MoreSettings.Utilities;
using static MoreSettings.Configuration.Enums;

namespace MoreSettings.Modules
{
    internal class VisualCustomizationModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("ModuleDesc_Visual");
        public override VisualConfig config => (VisualConfig)base.config;
        public override Enums.MenuCategory Type => Enums.MenuCategory.Visual;
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

            if (!string.IsNullOrEmpty(popConfig.PreviouslyType))
                EntityPopDamage.ForcedHandler = PopDamageHandlerRegistry.GetById(popConfig.PreviouslyType);


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
                ("Defaults", "Auto", () => { EntityPopDamage.ForcedHandler = null!; popConfig.PreviouslyType = string.Empty; }, () => { })
            };
            foreach (var handler in PopDamageHandlerRegistry.GetAll().Reverse())
            {
                var h = handler;
                if (h.damageData.unique) continue;
                optionsdata.Add((
                    GetString(h.OptionsTitle),
                    GetString(h.SubStr),
                    () => { EntityPopDamage.ForcedHandler = h; popConfig.PreviouslyType = h.Id; },
                    () =>
                    {
                        if (EntityPopDamage.ForcedHandler is DefaultPopDamageHandler || h is DefaultPopDamageHandler) return;


                        var NoTag = PopmenuHelper.AddConfigToggle(
                            GetString("无暴击时可触发"),
                            "",
                            () => popConfig.ProhibitedHasTagTwo,
                            v => popConfig.ProhibitedHasTagTwo = v,
                            scrollerFlow
                        );
                        PopmenuHelper.CenterToggleWidget(NoTag, options, scrollerFlow, false);

                        var crit = PopmenuHelper.AddConfigSlider(
                            GetText.Instance.GetString("CritEffectDuration"),
                            () => h.SpeedMultiplier,
                            v => h.SpeedMultiplier = v,
                            step: 0.1,
                            minValue: 0,
                            maxValue: 3,
                            scrollerFlow: scrollerFlow
                        );
                        menuHelper.CenterToggleWidget(crit, options, scrollerFlow, false);

                    }
                ));
            }
            while (popConfig.index > optionsdata.Count)
                popConfig.index--;

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







            menuHelper.AddSubSeparator(GetString("加速残影"), scrollerFlow);

            var OnionMode = menuHelper.AddConfigToggle(
                GetString("开启自定义残影"),
                GetString(""),
                () => config.CustomOnion,
                v => { config.CustomOnion = v; options.onResize(); },
                scrollerFlow: options.scrollerFlow
                );
            menuHelper.CenterToggleWidget(OnionMode, options, options.scrollerFlow);


            if (config.CustomOnion)
            {
                menuHelper.AddConfigToggle(
                    GetString("减弱残影动效"),
                    GetString(""),
                    () => config.OnionClosefeed,
                    v => config.OnionClosefeed = v,
                    scrollerFlow: options.scrollerFlow
                );


                List<(string title, string sub, Action onSelect, Action after)> OnionSkinColorData = new()
                {
                    ("自定义颜色", "", () => {config.OnionColorMode =OnionSkinColorMode.Custom; }, () => { }),
                    ("映射当前皮肤","", ()=> {config.OnionColorMode =OnionSkinColorMode.colorMap; }, () => { })
                };
                menuHelper.AddSubSeparator(GetString("残影配色"), scrollerFlow, false);
                menuHelper.AddConfigRadioGroup(
                    OnionSkinColorData,
                    (int)config.OnionColorMode,
                    (v) => { config.OnionColorMode = (OnionSkinColorMode)v; },
                    scrollerFlow
                );

                if (config.OnionColorMode == OnionSkinColorMode.Custom)
                    menuHelper.AddHSVColorWidget(
                      GetString("颜色设置"),
                      "",
                      () => config.OnionColorMode == OnionSkinColorMode.Custom,
                      config.OnionColorMode == OnionSkinColorMode.Custom,
                      newColor => config.OnionSkinColor = newColor,
                      config.OnionSkinColor,
                      scrollerFlow
                    );


                List<(string title, string sub, Action onSelect, Action after)> OnionSkinBlendData = new()
                {
                    ("Add", "", () => {config.OnionSkinBlendMode =OnionSkinBlendMode.Add; }, () => { }),
                    ("Alpha","", ()=> {config.OnionSkinBlendMode =OnionSkinBlendMode.Alpha; }, () => { })
                };
                menuHelper.AddSubSeparator(GetString("混合模式"), scrollerFlow, false);
                menuHelper.AddConfigRadioGroup(
                    OnionSkinBlendData,
                    (int)config.OnionSkinBlendMode,
                    (v) => { config.OnionSkinBlendMode = (OnionSkinBlendMode)v; },
                    scrollerFlow
                );

            }


            menuHelper.AddSubSeparator(GetString("Others"), scrollerFlow);
            menuHelper.AddConfigToggle(
                GetText.Instance.GetString("KatanaZeroOutfit"),
                GetText.Instance.GetString("KatanaZeroOutfitDesc"),
                () => config.KatanaSkin,
                v => config.KatanaSkin = v,
                scrollerFlow: scrollerFlow
                );
        }

        #region Hooks
        public override void PermanentlyRegisterHooks()
        {
            Hook_Teleport.startTeleport += Hook_Teleport_startTeleport;
        }

        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_Hero.hasSkin -= Hook_Hero_hasSkin;
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Hero.hasSkin += Hook_Hero_hasSkin;
            Hook_Beheaded.postUpdate += Hook_Beheaded_postUpdate;
        }

        private bool Hook_Hero_hasSkin(Hook_Hero.orig_hasSkin orig, Hero h, dc.String model, dc.String itemId)
        {
            if (itemId != null)
                if (config.KatanaSkin && itemId.ToString() == "KatanaZero") return true;
            return orig(h, model, itemId);
        }

        private void Hook_Teleport_startTeleport(Hook_Teleport.orig_startTeleport orig, Teleport h, Hero hero, Entity to)
        {
            if (to == null)
                return;

            switch (config.TeleportStyle)
            {
                case TeleportStyle.orig:
                    if (hero.hasSkin(null, "RiskOfRain".ToHaxeString()))
                    {
                        _ = new TeleportationRiskOfRain(hero, h, to, !config.TeleportImmediate); return;
                    }
                    _ = new Teleportation(hero, h, to, !config.TeleportImmediate);
                    break;

                case TeleportStyle.Default:
                    _ = new Teleportation(
                        hero,
                        h,
                        to,
                        !config.TeleportImmediate);
                    break;

                case TeleportStyle.RiskOfRain:
                    _ = new TeleportationRiskOfRain(
                        hero,
                        h,
                        to,
                        !config.TeleportImmediate);
                    break;

                case TeleportStyle.Instant:
                    _ = new TeleportationFancy(
                        hero,
                        h,
                        to,
                        !config.TeleportImmediate);
                    break;

            }
        }



        private void Hook_Beheaded_postUpdate(Hook_Beheaded.orig_postUpdate orig, Beheaded self)
        {
            ((HashlinkObjectType)self.HashlinkObj.Type).Super!.FindProto("postUpdate")!.Function.DynamicInvoke(self);

            double interval = 0.06 * self.cd.baseFps;
            bool skipSpeedFx = !self.hasAnySpeedBuff() || self.isChargingSkill()
                || (System.Math.Abs(self.dx + self.bdx) < 0.1 && System.Math.Abs(self.dy + self.bdy) < 0.1);

            // if (!TryCdTick(self.cd, HeroCooldown.SpeedOnion, interval))
            //     CreateHeroOnionSkin(self, default!);


            if (!skipSpeedFx)
            {
                double comboFactor = CalcComboFactor(self);
                const double comboLimitBase = 1.0;
                var aff = self.affects;

                double fullSpeed = 1.0
                    + self.speedComboRun * comboFactor
                    + GetAffVal(116, self.getHighestAffectValue(116), aff)
                    + GetAffVal(72, self.runSpeedOnTrap, aff)
                    + GetAffVal(69, 0.4, aff)
                    + GetAffVal(71, 0.2, aff)
                    + GetAffVal(70, 0.2, aff)
                    + self.activeSkillsManager.getRunSpeedMul();

                double comboLimit = comboLimitBase + self.speedComboRun;

                if (!TryCdTick(self.cd, HeroCooldown.SpeedOnion, interval))
                {
                    if (fullSpeed > comboLimit)
                    {
                        OnionSkin.Class.fromEntity(self, null,
                            LerpColor(16380422, 16405765, dc.pr.Game.Class.ME.ftime % 30.0 / 30.0),
                            Ref<double>.In(0.25), Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
                        CheckGroundSparks(self);
                    }
                    else
                    {
                        OnionSkin.Class.fromEntity(self, null, 2665431, Ref<double>.In(0.15),
                            Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
                        OnionSkin.Class.fromEntity(self, null,
                            LerpColor(13643567, 7165407, fullSpeed / comboLimit),
                            Ref<double>.In(0.05 + 0.15 * fullSpeed / comboLimit),
                            Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
                    }
                }

                if (!TryCdTick(self.cd, HeroCooldown.SpeedOnion, interval) && self.hasAnySpeedBuff()
                    && fullSpeed < comboLimit)
                {
                    OnionSkin.Class.fromEntity(self, null, 6984927, Ref<double>.In(0.25),
                        Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
                }
            }

            if (self.cd.fastCheck.exists(HeroCooldown.WallRunOnionSkin) && !TryCdTick(self.cd, HeroCooldown.WallOnion, interval))
                OnionSkin.Class.fromEntity(self, null, 13138521, Ref<double>.Null,
                    Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);




        }
        #endregion

        #region HeroHelper

        public OnionSkin CreateHeroOnionSkin(Hero hero, BlendMode mode = null!)
        {
            int color = config.OnionColorMode == OnionSkinColorMode.Custom ? config.OnionSkinColor : default;
            var onionSkin = OnionSkin.Class.fromEntity(hero, null, null, Ref<double>.In(0.25), Ref<double>.Null, Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);

            if (config.OnionClosefeed)
            {
                onionSkin.ds = 0;
                onionSkin.dx = 0;
                onionSkin.dy = 0;
            }

            if (config.OnionColorMode == OnionSkinColorMode.colorMap && color == default)
            {
                var colorMap = (ColorMap)hero.spr.getShader(ColorMap.Class);
                var glowKey = (GlowKey)hero.spr.getShader(GlowKey.Class);
                var colorBlend = new ColorBlend(0, 0.7);

                onionSkin.addAdditionnalShader(colorMap);
                onionSkin.addAdditionnalShader(colorBlend);
                onionSkin.addAdditionnalShader(glowKey);
            }

            mode = config.OnionSkinBlendMode switch
            {
                OnionSkinBlendMode.Add => new BlendMode.Add(),
                OnionSkinBlendMode.Alpha => new BlendMode.Alpha(),
                _ => new BlendMode.Alpha()
            };
            onionSkin.blendMode = mode;

            return onionSkin;
        }


        private static double CalcComboFactor(Hero self)
        {
            double kills = self.spdComboKills;
            double max = self.get_infos().props.speedComboMaxMobs;
            return kills > max ? 1.0 : kills > max * 0.5 ? 0.5 : 0.0;
        }

        private static double GetAffVal(int id, double val, ArrayObj array)
        {
            var aff = array.getDyn(id);
            return aff != null && aff!.length > 0 ? val : 0.0;
        }

        private static void CheckGroundSparks(Hero self)
        {
            if (System.Math.Abs(self.dx + self.bdx) < 0.1) return;

            var level = self._level;
            if (level == null) return;

            var lmap = level.map;
            if (lmap == null) return;

            int cx = self.cx, cy = self.cy;
            int wid = lmap.wid;
            int hei = lmap.hei;
            var collisions = lmap.collisions;

            if ((uint)(cy + 1) < (uint)hei && (uint)cx < (uint)wid)
            {
                int idx = (cy + 1) * wid + cx;
                if (idx < collisions.length && (collisions.getDyn(idx) & 1) != 0)
                {
                    double groundYr = lmap.getGroundYr(cx, cy, Ref<double>.In(self.xr), Ref<double>.In(self.yr));
                    if (self.yr > groundYr && self.dy == 0.0)
                    {
                        level.fx.groundSparks(self, 16477444, 3);
                        return;
                    }
                }
            }

            if (!self.ignoreSlopes && System.Math.Abs(self.dy) < 0.1 && (uint)cx < (uint)wid && (uint)cy < (uint)hei)
            {
                int idx = cy * wid + cx;
                if (idx < collisions.length && (collisions.getDyn(idx) & 512) != 0)
                {
                    level.fx.groundSparks(self, 16477444, 3);
                }
            }
        }


        private static int LerpColor(int from, int to, double t)
        {
            int fromR = (from >> 16) & 0xFF;
            int fromG = (from >> 8) & 0xFF;
            int fromB = from & 0xFF;
            int toR = (to >> 16) & 0xFF;
            int toG = (to >> 8) & 0xFF;
            int toB = to & 0xFF;

            int r = (int)(fromR + (toR - fromR) * t + 0.5);
            int g = (int)(fromG + (toG - fromG) * t + 0.5);
            int b = (int)(fromB + (toB - fromB) * t + 0.5);

            return (r << 16) | (g << 8) | b;
        }

        private static bool TryCdTick(dc.tool.Cooldown cd, int key, double frames)
        {
            if (cd.fastCheck.exists(key)) return true;
            double cmp = System.Math.Floor(frames * 1000.0) / 1000.0;
            var inst = cd.fastCheck.get(key);
            if (inst == null)
            {
                inst = new CdInst(key, cmp);
                cd.fastCheck.set(key, inst);
                cd.cdList.push(inst);
            }
            else inst.frames = cmp;
            return false;
        }
        #endregion
    }
}