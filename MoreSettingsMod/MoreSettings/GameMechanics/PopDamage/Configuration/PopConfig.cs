using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class PopConfig
    {
        public double HotlineSpeedMultiplier { get; set; } = 0.8;
        public double StsSpeedMultiplier { get; set; } = 0.8;
        public double RevealSpeedMultiplier { get; set; } = 2.0;
        public bool RevealPop { get; set; } = false;
        public bool GenuinePopDamage { get; set; } = false;
        public int index = 0;
    }
}