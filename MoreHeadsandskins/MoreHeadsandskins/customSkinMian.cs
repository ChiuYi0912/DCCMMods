using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Events;
using MoreHeadsandskins;

namespace MoreHeadsandskins.customSkinEntry
{
    public class customSkinMian:
    IEventReceiver
        
    {
        public customSkinMian(Entry entry)
        {
            EventSystem.AddReceiver(this);
        }
    }
}