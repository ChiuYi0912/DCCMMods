using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebugMod.Core.Configuration
{
    public class CoreCfig
    {
        public bool debugMode { get; set; } = false;

        public string DebugUILogPATH = @"D:\steam\steamapps\common\Dead Cells\coremod\logs";

        public double LogTextSize = 0.8;
    }
}