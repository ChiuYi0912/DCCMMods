using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Storage;

namespace DebugMod.Configuration
{
    public class CoreCfig
    {
        public string DebugUILogPATH = FolderInfo.Logs.FullPath;
        public string LogTextColor = "#61D6D6";
        public double LogTextSize = 0.8;
    }
}