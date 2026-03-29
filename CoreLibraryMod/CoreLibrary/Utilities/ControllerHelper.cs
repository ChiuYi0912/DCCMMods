using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using dc.ui;
using ModCore.Utilities;
using Serilog;

namespace CoreLibrary.Utilities
{
    public static class ControllerHelper
    {
        public static bool ControlsUpdateFromProcess(Controller controller, int actionCode)
        {

            Controller parent = controller;
            if (CheckBindings(parent, actionCode, parent.get_bindings().padA) ||
                CheckBindings(parent, actionCode, parent.get_bindings().padB) ||
                CheckBindings(parent, actionCode, parent.get_bindings().padC) ||
                CheckBindings(parent, actionCode, parent.get_bindings().primary) ||
                CheckBindings(parent, actionCode, parent.get_bindings().secondary) ||
                CheckBindings(parent, actionCode, parent.get_bindings().third))
                return true;


            return false;
        }


        private static bool CheckBindings(Controller parent, int actionCode, ArrayBytes_Int bindings)
        {
            int length = bindings.length;
            if (actionCode >= length) return false;

            int key = bindings.getDyn(actionCode);
            if (key >= 0 && parent.padIsPressed(key)) return true;

            if ((parent.mode & Controller.Class.ENABLE_KEY) != 0 && key >= 0 && Key.Class.isPressed(key))
                return true;
            return false;
        }

        public static void PrintAllBindings(Controller controller, ILogger logger)
        {
            var bindings = new[]
            {
                controller.get_bindings().padA,
                controller.get_bindings().padB,
                controller.get_bindings().padC,
                controller.get_bindings().primary,
                controller.get_bindings().secondary,
                controller.get_bindings().third
            };

            int maxLength = bindings.Max(b => b.length);

            for (int actionCode = 0; actionCode < maxLength; actionCode++)
            {
                logger.Information($"动作码 {actionCode}: ");
                bool any = false;
                for (int i = 0; i < bindings.Length; i++)
                {
                    if (actionCode < bindings[i].length)
                    {
                        int key = bindings[i].getDyn(actionCode);
                        if (key >= 0)
                        {
                            any = true;
                            string bindingName = i switch
                            {
                                0 => "padA",
                                1 => "padB",
                                2 => "padC",
                                3 => "primary",
                                4 => "secondary",
                                5 => "third",
                                _ => "?"
                            };
                            logger.Information($"[{bindingName}:{key}] ");
                        }
                    }
                }
                if (!any)
                {
                    logger.Information("无绑定");
                }
                logger.Information("");
            }
        }


        public static void SetKeyBindingHeroContorlLble(ContorlLbleKeyConfig contorlLbleKey)
        {
            Controller controller = Boot.Class.ME.controller;
            BindingProfiles bindingProfiles = controller.normalBindings;

            bindingProfiles.primary.setDyn(contorlLbleKey.act, contorlLbleKey.Primary);
            bindingProfiles.secondary.setDyn(contorlLbleKey.act, contorlLbleKey.Secondary);
            bindingProfiles.third.setDyn(contorlLbleKey.act, contorlLbleKey.Third);
        }
    }

    public class ContorlLbleKeyConfig
    {
        public string Name { get; set; } = string.Empty;
        public int act { get; set; }
        public int? Primary { get; set; }
        public int? Secondary { get; set; }
        public int? Third { get; set; }
    }
}