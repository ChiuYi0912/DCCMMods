using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    public class PopConfig
    {
        public bool HotlinePopDamage { get; set; } = false;
        public double HotlineSpeedMultiplier { get; set; } = 0.8;

        public bool StsPopDamage { get; set; } = false;
        public double StsSpeedMultiplier { get; set; } = 0.8;

        public bool RevealPop { get; set; } = false;
        public double RevealSpeedMultiplier = 2.0;

        public bool GenuinePopDamage { get; set; } = false;
    }
}