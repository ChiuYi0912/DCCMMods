using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary.Core.Extensions;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Modules;
using MoreSettings.Utilities;

namespace MoreSettings.Core
{

    public class ModuleManager
    {
        private readonly Dictionary<string, BaseModule> modules = new();
        private ModBase mainMod = null!;

        public ModuleManager(ModBase basemainMod) => mainMod = basemainMod;

        public void RegisterModule(BaseModule module)
        {
            module.Initialize(mainMod);
            module.RegisterHooks();
            modules[module.Name] = module;
        }

        public void BuildAllMenus(dc.ui.Options options)
        {
            foreach (var module in modules.Values)
            {
                module.BuildMenu(options);
            }
        }
    }
}