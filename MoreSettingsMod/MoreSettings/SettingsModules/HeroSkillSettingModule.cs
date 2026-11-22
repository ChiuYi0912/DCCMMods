using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.en;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using dc.tool.mainSkills;
using dc.ui;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;

namespace MoreSettings.SettingsModules
{
    internal class HeroSkillSettingModule : BaseModule
    {
        public override Enums.MenuCategory Type => Enums.MenuCategory.Skill;

        public override SkillConfig config => (SkillConfig)base.config;

        public override string Description => GetText.Instance.GetString("SkillModule");

        private static bool dodgeJumpConsumed = false;

        public override void Initialize(ModBase mainMod)
        {
            base.Initialize(mainMod);
            config = SettingsMain.ConfigValue.Skill;
        }

        public override void BuildMenu(Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;

            menuHelper.AddConfigToggle(
                GetString("CheckJumpKey"),
                GetString("CheckJumpKeyDesc"),
                () => config.CheckJumpKey,
                v =>
                {
                    config.CheckJumpKey = v;
                    options.onResize();
                },
                scrollerFlow
            );

            if (config.CheckJumpKey)
            {
                menuHelper.AddConfigToggle(
                    GetString("UseRollDown"),
                    GetString("UseRollDownDesc"),
                    () => config.UseRollDown,
                    v => config.UseRollDown = v,
                    scrollerFlow
                );
            }
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Jump.isUsingInputs += Hook_Jump_isUsingInput;
            Hook_Dodge.prepare += Hook_Dodge_prepare;

        }

        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_Jump.isUsingInputs -= Hook_Jump_isUsingInput;
            Hook_Dodge.prepare -= Hook_Dodge_prepare;
        }

        private bool Hook_Dodge_prepare(Hook_Dodge.orig_prepare orig, Dodge self)
        {
            bool onuse = orig(self);

            if (!config.CheckJumpKey)
            {
                return onuse;
            }

            dodgeJumpConsumed = false;
            return onuse;
        }

        private void TryDropThrough(Hero hero)
        {
            var jumpDown = hero.mainSkillsManager?.getMainSkill(JumpDown.Class);
            if (jumpDown != null)
            {
                jumpDown.prepare();
            }
        }

        private bool Hook_Jump_isUsingInput(Hook_Jump.orig_isUsingInputs orig, Jump self)
        {
            bool defaultResult = orig(self);
            if (!config.CheckJumpKey)
            {
                return defaultResult;
            }

            if (defaultResult)
            {
                return true;
            }

            if (!dodgeJumpConsumed)
            {
                var parent = self.hero?.controller?.parent;
                if (parent != null && DetectRawInput(parent))
                {
                    var hero = self.hero;

                    if (hero != null && IsOnPassablePlatform(hero))
                    {
                        dodgeJumpConsumed = true;
                        if (config.UseRollDown)
                        {
                            TryDropThrough(hero);
                            return false;
                        }
                        return true;
                    }

                    dodgeJumpConsumed = true;
                    return true;
                }

            }

            return defaultResult;
        }

        private bool IsOnPassablePlatform(Hero hero)
        {
            var level = hero._level;
            if (level == null) return false;

            var map = level.map;
            if (map == null) return false;

            int cx = hero.cx;
            int cy = hero.cy + 1;

            if (cx < 0 || cx >= map.wid || cy < 0 || cy >= map.hei)
                return false;

            int collision = map.collisions.getDyn(cy * map.wid + cx);

            return (collision & 8) != 0;
        }

        private bool DetectRawInput(Controller parent)
        {
            if (parent == null)
                return false;

            int index = 0;
            var bindings = parent.get_bindings();

            if (CheckPadDown(parent, bindings.padA, index)) return true;
            if (CheckPadDown(parent, bindings.padB, index)) return true;
            if (CheckPadDown(parent, bindings.padC, index)) return true;

            if (CheckKeyDown(parent, bindings.primary, index)) return true;
            if (CheckKeyDown(parent, bindings.secondary, index)) return true;
            if (CheckKeyDown(parent, bindings.third, index)) return true;

            return false;
        }

        private static bool CheckPadDown(Controller parent, ArrayBytes_Int bindings, int index)
        {
            int key = (index >= bindings.length) ? 0 : bindings.getDyn(index);
            return parent.padIsDown(key);
        }

        private static bool CheckKeyDown(Controller parent, ArrayBytes_Int bindings, int index)
        {
            if (index >= bindings.length) return false;
            int key = bindings.getDyn(index);
            if ((parent.mode & Controller.Class.ENABLE_KEY) == 0) return false;
            if (key < 0) return false;
            return Key.Class.isDown(key);
        }
    }
}