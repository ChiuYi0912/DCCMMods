using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MoreSettings.API;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    public class PopConfig
    {
        public int index = 0;
        public bool GenuinePopDamage { get; set; } = false;
        public string PreviouslyType { get; set; } = string.Empty;
        public bool ProhibitedHasTagTwo { get; set; } = false;
        public Dictionary<string, PopDamageData> DATA = new();
    }
}