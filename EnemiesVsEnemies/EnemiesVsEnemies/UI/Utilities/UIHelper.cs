using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using dc.hxd;
using dc.tool;

namespace EnemiesVsEnemies.UI.Utilities
{
    public class UIHelper
    {
        private double currentTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
        protected ControllerAccess controller = null!;
        public UIHelper(ControllerAccess controllerinit)
        {
            controller = controllerinit;
        }
        public bool IsDown(int dir)
        {
            if (controller.manualLock || controller.parent.isLocked) return false;
            if (currentTime < controller.parent.suspendTimer) return false;
            return controller.parent.padIsDown(dir) || Key.Class.isDown(dir);
        }

        public bool IsPressed(int dir)
        {
            if (controller.manualLock || controller.parent.isLocked) return false;
            if (currentTime < controller.parent.suspendTimer) return false;
            return controller.parent.padIsPressed(dir) || Key.Class.isPressed(dir);
        }


    }
}