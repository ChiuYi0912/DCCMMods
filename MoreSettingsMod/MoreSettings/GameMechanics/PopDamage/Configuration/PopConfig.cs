using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreSettings.API;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    public class PopConfig
    {
        public bool GenuinePopDamage { get; set; } = false;
        public int index = 0;
        public string PreviouslyType { get; set; } = string.Empty;
        public bool ProhibitedHasTagTwo { get; set; } = true;
        public bool Characteristics { get; set; } = true;
        public Dictionary<string, PopDamageData> DATA = new();
    }
}