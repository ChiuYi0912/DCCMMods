using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.hl.types;
using dc.hxd;
using dc.tool;

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
    }
}