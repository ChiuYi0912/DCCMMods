using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Utilities;
using MoreSettings.Configuration;

namespace MoreSettings.API
{
    public class IInputApiService(ControllerHelperSuper<MainConfig> contro)
    {
        private readonly ControllerHelperSuper<MainConfig> controller = contro;

        public int RegisterAction(
            string name,
            int? primary = null,
            int? secondary = null,
            int? third = null)
        {
            int key = controller.AddKey(name, primary, secondary, third);
            controller.ApplyBindings();
            return key;
        }

        public bool RemoveAction(string name)
            => controller.RemoveKey(name);

        public int? GetAction(string name)
            => controller.GetAction(name);

        public bool IsPressed(int action)
            => controller.IsPressed(action);

        public bool IsDown(int action)
            => controller.IsDown(action);
    }
}