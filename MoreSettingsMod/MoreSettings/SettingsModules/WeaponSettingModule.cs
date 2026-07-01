using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using dc.tool.hero;
using dc.tool.weap;
using HashlinkNET.Native.Impl;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;

namespace MoreSettings.Modules
{
    internal class WeaponSettingModule : BaseModule
    {
        public override string Description => GetText.Instance.GetString("WeaponSettingModule");

        public override WeaponConfig config => (WeaponConfig)base.config;

        public override Enums.MenuCategory Type => Enums.MenuCategory.Weapon;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ConfigValue.Weapon;
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;

            var widget = menuHelper.AddConfigToggle(
                 GetText.Instance.GetString("NoKatanaHold"),
                 GetText.Instance.GetString("NoKatanaHoldDesc"),
                 () => config.DisableKatanaByHoldingDown,
                 v => config.DisableKatanaByHoldingDown = v,
                 scrollerFlow: options.scrollerFlow
            );

            menuHelper.AddSubSeparator(GetString("Shield"), scrollerFlow);

            menuHelper.AddHSVColorWidget(
                GetString("CustomShieldHitColor"),
                "",
                () =>
                {
                    config.HasCustomShieldHitColor = !config.HasCustomShieldHitColor;
                    SettingsMain.SaveConfig();
                    return config.HasCustomShieldHitColor;
                },
                config.HasCustomShieldHitColor,
                newColor => config.BaseShieldFrontShieldHitColor = newColor,
                config.BaseShieldFrontShieldHitColor,
                scrollerFlow
            );

            if (config.HasCustomShieldHitColor)
            {
                int paddingleft = (int)(options.get_pixelScale.Invoke() * 40);

                var showobviously = menuHelper.AddConfigToggle(
                 GetString("ShowMoreObviously"),
                 GetString(""),
                 () => config.ShowObviously,
                 v => config.ShowObviously = v,
                 scrollerFlow: options.scrollerFlow
            );
                scrollerFlow.getProperties(showobviously).paddingLeft = paddingleft;
            }
        }


        #region Hooks
        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_HeroWeaponsManager.canUseWeapon -= Hook_HeroWeaponsManager_canUseWeapon;
            Hook_BaseShield.triggerParryFeedbacks -= Hook_BaseShield_triggerParryFeedbacks;
        }
        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_HeroWeaponsManager.canUseWeapon += Hook_HeroWeaponsManager_canUseWeapon;
            Hook_BaseShield.triggerParryFeedbacks += Hook_BaseShield_triggerParryFeedbacks;
        }

        private void Hook_BaseShield_triggerParryFeedbacks(Hook_BaseShield.orig_triggerParryFeedbacks orig, BaseShield self)
        {
            self.owner.popText(Lang.Class.t.get("Parade !".ToHaxeString(), null), 65535);

            Fx fx = self.owner._level.fx;

            fx.customMask(9353156, 0.2, 0.04, 0.1, 0.15, null);

            //fx.frontShieldHit(self.owner, 6921449);

            int color = config.HasCustomShieldHitColor ? config.BaseShieldFrontShieldHitColor : 6921449;
            double lifeS = config.HasCustomShieldHitColor && config.ShowObviously ? 0.3 : 0.1;

            fx.FrontShieldHitPro(self.owner, color: color,
                    innerCount: 16, outerCount: 16,
                    alpha: 1.0, lifeS: lifeS);

            Hero owner = self.owner;
            double x = (owner.cx + owner.xr) * 24.0;
            double y = (owner.cy + owner.yr) * 24.0 - owner.hei * 0.5;

            int sparkLineCount = 20;
            int deflectLineCount = 25;
            fx.hitLines(x, y, owner.dir, 16777215, null, Ref<int>.In(sparkLineCount), Ref<int>.In(deflectLineCount));

            self.playHitSfx(null, null, default, null, null);
        }
        private bool Hook_HeroWeaponsManager_canUseWeapon(Hook_HeroWeaponsManager.orig_canUseWeapon orig, HeroWeaponsManager self, Weapon w, Hero hero, int posId, Ref<bool> ctrlOk, int? key)
        {
            bool isCtrlAllowed = ctrlOk.IsNull || ctrlOk.value;

            bool isInputAllowed = DetermineInputAllowed(w, hero, key, isCtrlAllowed);

            if (!w.isReady())
                return false;

            if (w is BaseShield && hero.cd.fastCheck.exists(1348468736))
                return false;

            if (!isCtrlAllowed && !isInputAllowed && !w.anticipateNext)
                return false;

            if (!w.canStartChargeEarly())
            {
                if (hero.controlsLocked(Ref<bool>.Null))
                {
                    if (!w.canCancel())
                        return false;
                    if (!self.tryToCancelCharge(true))
                        return false;
                }
            }
            else if (hero.isChargingSkill())
            {
                if (!w.canCancel())
                    return false;
                if (!self.tryToCancelCharge(true))
                    return false;
            }
            return ((ArrayObj)self.weaponControlLocks.get(posId)).length == 0;
        }

        private bool DetermineInputAllowed(Weapon w, Hero hero, int? key, bool isCtrlAllowed)
        {
            if (isCtrlAllowed)
                return false;

            if (w is Katana && config.DisableKatanaByHoldingDown)
                return false;

            if (!w.item.hasTag("RapidFire".ToHaxeString()) && !Main.Class.ME.options.holdToAttack)
                return false;

            if (hero.cd.fastCheck.exists(1447034880))
                return false;
            if (hero.cd.fastCheck.exists(1449132032))
                return false;

            if (key != null)
                return IsKeyPressed(hero, key.Value);

            return true;
        }
        #endregion


        #region 辅助方法
        private bool IsKeyPressed(Hero hero, int keyIndex)
        {
            ControllerAccess controller = hero.controller;
            Controller parent = controller.parent;

            if (controller.manualLock ||
                parent.isLocked ||
                (parent.exclusiveId != null && parent.exclusiveId != controller.id) ||
                Lib_std.sys_time.Invoke() < parent.suspendTimer)
                return false;

            var bindings = parent.get_bindings();
            if (CheckBinding(bindings.padA, keyIndex, parent.padIsDown) ||
                CheckBinding(bindings.padB, keyIndex, parent.padIsDown) ||
                CheckBinding(bindings.padC, keyIndex, parent.padIsDown))
                return true;

            if ((parent.mode & Controller.Class.ENABLE_KEY) != 0)
            {
                if (CheckBinding(bindings.primary, keyIndex, Key.Class.isDown) ||
                    CheckBinding(bindings.secondary, keyIndex, Key.Class.isDown) ||
                    CheckBinding(bindings.third, keyIndex, Key.Class.isDown))
                    return true;
            }

            return false;
        }

        private bool CheckBinding(ArrayBytes_Int bindingArray, int keyIndex, HlFunc<bool, int> isDownFunc)
        {
            int length = bindingArray.length;
            if (keyIndex >= length)
                return false;
            int mappedKey = bindingArray.getDyn(keyIndex);
            return mappedKey >= 0 && isDownFunc(mappedKey);
        }
        #endregion
    }
}