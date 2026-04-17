using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public class PopConfig
    {
        public bool HotlinePopDamage { get; set; } = false;
        public double HotlineSpeedMultiplier { get; set; } = 450.0;
        public bool StsPopDamage { get; set; } = false;
        public double StsSpeedMultiplier { get; set; } = 600.0;
    }
}