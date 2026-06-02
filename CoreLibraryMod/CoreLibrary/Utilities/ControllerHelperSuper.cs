using dc;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using HashlinkNET.Native.Impl;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace CoreLibrary.Utilities
{
    public class ControllerHelperSuper<T> where T : new()
    {
        private readonly Config<T> config = null!;
        private readonly Dictionary<int, ContorlLbleKeyConfig> keyConfig = null!;
        private static readonly HashSet<int> managedActions = new();
        public Controller gamecontroller = null!;

        public int ActionCount
        {
            get
            {
                var bindings = Boot.Class.ME.controller.normalBindings;
                return System.Math.Max(
                    System.Math.Max(bindings.primary.length, bindings.secondary.length),
                    bindings.third.length);
            }
        }

        private bool Lock
        {
            get => gamecontroller.isLocked;
            set => gamecontroller.isLocked = value;
        }

        public ControllerHelperSuper(Config<T> userconfig, Dictionary<int, ContorlLbleKeyConfig> lbleKeyConfig, Controller controller)
        {
            config = userconfig;
            keyConfig = lbleKeyConfig;
            gamecontroller = controller;

            RebuildTracking();

            dc.Hook_Options.setKeyMapping += Hook_Options_setKeyMapping;
            dc.Hook__Options.dumpControllerConfig += Hook__Options_dumpControllerConfig;
        }



        public int AddKey(string name, int? primary = null, int? secondary = null, int? third = null)
        {
            RemoveKeyInternal(name);

            int action = gamecontroller != null ? ActionCount + 1 : 100;
            while (managedActions.Contains(action))
                action++;

            keyConfig[action] = new ContorlLbleKeyConfig
            {
                Name = name,
                act = action,
                Primary = primary,
                Secondary = secondary,
                Third = third
            };

            managedActions.Add(action);
            SetBinding(keyConfig[action]);
            config.Save();
            return action;
        }

        public bool UpdateKey(string name, int? primary = null, int? secondary = null, int? third = null)
        {
            foreach (var kv in keyConfig)
            {
                if (kv.Value.Name == name && managedActions.Contains(kv.Key))
                {
                    var cfg = kv.Value;
                    if (primary.HasValue)
                        cfg.Primary = primary;
                    if (secondary.HasValue)
                        cfg.Secondary = secondary;
                    if (third.HasValue)
                        cfg.Third = third;
                    SetBinding(cfg);
                    config.Save();
                    return true;
                }
            }
            return false;
        }

        public bool RemoveKey(string name)
        {
            if (RemoveKeyInternal(name))
            {
                config.Save();
                return true;
            }
            return false;
        }

        public int? GetAction(string name)
        {
            foreach (var kv in keyConfig)
            {
                if (kv.Value.Name == name && managedActions.Contains(kv.Key))
                    return kv.Key;
            }
            return null;
        }

        public void ApplyBindings()
        {
            foreach (var item in keyConfig)
            {
                if (managedActions.Contains(item.Key))
                    SetBinding(item.Value);
            }
            config.Save();
        }


        public bool IsDown(int actionCode, bool ignorePause = false)
        {
            if (TryCheckManagedKey(actionCode, ignorePause, Key.Class.isDown, out var result))
                return result;
            return CheckInputFromProcess(gamecontroller, actionCode, ignorePause, gamecontroller.padIsDown, Key.Class.isDown);
        }

        public bool IsPressed(int actionCode, bool ignorePause = false)
        {
            if (TryCheckManagedKey(actionCode, ignorePause, Key.Class.isPressed, out var result))
                return result;
            return CheckInputFromProcess(gamecontroller, actionCode, ignorePause, gamecontroller.padIsPressed, Key.Class.isPressed);
        }

        public bool IsPressedFromProcess(Controller controller, int actionCode, bool ignorePause = false) =>
            CheckInputFromProcess(controller, actionCode, ignorePause, controller.padIsPressed, Key.Class.isPressed);

        public bool IsDownFromProcess(Controller controller, int actionCode, bool ignorePause = false) =>
            CheckInputFromProcess(controller, actionCode, ignorePause, controller.padIsDown, Key.Class.isDown);

        private bool TryCheckManagedKey(int actionCode, bool ignorePause, HlFunc<bool, int> keyCheck, out bool result)
        {
            if (!managedActions.Contains(actionCode) || !keyConfig.TryGetValue(actionCode, out var cfg))
            {
                result = false;
                return false;
            }

            if (gamecontroller.isLocked || gamecontroller.exclusiveId != null)
            {
                result = false;
                return true;
            }

            if (ignorePause && Lib_std.sys_time.Invoke() < gamecontroller.suspendTimer)
            {
                result = false;
                return true;
            }

            result = (cfg.Primary >= 0 && keyCheck(cfg.Primary.Value))
                  || (cfg.Secondary >= 0 && keyCheck(cfg.Secondary.Value))
                  || (cfg.Third >= 0 && keyCheck(cfg.Third.Value));
            return true;
        }


        private void RebuildTracking()
        {
            var conflicted = new List<int>();
            foreach (var kv in keyConfig)
            {
                if (!managedActions.Add(kv.Key))
                    conflicted.Add(kv.Key);
            }

            foreach (var oldKey in conflicted)
            {
                var cfg = keyConfig[oldKey];
                keyConfig.Remove(oldKey);

                int newAction = gamecontroller != null ? ActionCount + 1 : 100;
                while (managedActions.Contains(newAction))
                    newAction++;

                cfg.act = newAction;
                keyConfig[newAction] = cfg;
                managedActions.Add(newAction);
            }

            if (conflicted.Count > 0)
                config.Save();
        }

        private bool RemoveKeyInternal(string name)
        {
            foreach (var kv in keyConfig)
            {
                if (kv.Value.Name == name && managedActions.Contains(kv.Key))
                {
                    managedActions.Remove(kv.Key);
                    keyConfig.Remove(kv.Key);
                    return true;
                }
            }
            return false;
        }

        private bool CheckInputFromProcess(Controller controller, int actionCode, bool ignorePause,
            HlFunc<bool, int> padCheckFunc, HlFunc<bool, int> keyCheckFunc)
        {
            if (controller.isLocked || controller.exclusiveId != null)
                return false;

            if (Lib_std.sys_time.Invoke() < controller.suspendTimer && ignorePause)
                return false;

            var bindings = controller.get_bindings();

            if (CheckBinding(bindings.padA, actionCode, padCheckFunc) ||
                CheckBinding(bindings.padB, actionCode, padCheckFunc) ||
                CheckBinding(bindings.padC, actionCode, padCheckFunc))
                return true;

            if ((controller.mode & Controller.Class.ENABLE_KEY) != 0)
            {
                if (CheckBinding(bindings.primary, actionCode, keyCheckFunc) ||
                    CheckBinding(bindings.secondary, actionCode, keyCheckFunc) ||
                    CheckBinding(bindings.third, actionCode, keyCheckFunc))
                    return true;
            }

            return false;
        }

        private bool CheckBinding(ArrayBytes_Int bindingArray, int actionCode, HlFunc<bool, int> isDownFunc)
        {
            int length = bindingArray.length;
            if (actionCode >= length)
                return false;
            int mappedKey = bindingArray.getDyn(actionCode);
            return mappedKey >= 0 && isDownFunc(mappedKey);
        }

        private void SetBinding(ContorlLbleKeyConfig cfg)
        {
            BindingProfiles bp = gamecontroller.normalBindings;
            bp.primary.setDyn(cfg.act, cfg.Primary);
            bp.secondary.setDyn(cfg.act, cfg.Secondary);
            bp.third.setDyn(cfg.act, cfg.Third);
        }

        #region Hooks

        private void Hook__Options_dumpControllerConfig(dc.Hook__Options.orig_dumpControllerConfig orig, object _gamepad, object _keyboard, bool isNormalBindings)
        {
            orig(_gamepad, _keyboard, isNormalBindings);
            ApplyBindings();
        }

        private void Hook_Options_setKeyMapping(dc.Hook_Options.orig_setKeyMapping orig, dc.Options options, int action, int idx, int? key)
        {
            if (managedActions.Contains(action) && keyConfig.TryGetValue(action, out var cfg))
            {
                cfg.act = action;
                if (idx == 0)
                    cfg.Primary = key;
                else if (idx == 1)
                    cfg.Secondary = key;
                else if (idx == 2)
                    cfg.Third = key;
                SetBinding(cfg);
                config.Save();
                return;
            }
            orig(options, action, idx, key);
        }
        #endregion
    }
}
