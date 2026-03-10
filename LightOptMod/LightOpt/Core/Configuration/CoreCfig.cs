using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightOpt.Core.Configuration
{
    public class CoreCfig
    {
        public bool DebugMode { get; set; } = true;


        public float SupersamplingFactor { get; set; } = 8.0f;
        public bool EnableAntiAliasing { get; set; } = true;
        public bool DisableBlur { get; set; } = false;
        public bool DisableChromaticAberration { get; set; } = false;
        public bool DisableVignette { get; set; } = false;
        public bool DisableGlitch { get; set; } = false;
        public bool DisableFog { get; set; } = true;
    }

}