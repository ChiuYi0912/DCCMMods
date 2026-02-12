using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModCore.Events;
using MoreHeadsandskins;

namespace MoreHeadsandskins.EntryHookInitialize
{
    [Event]
    public interface IOnEntryHookInitialize
    {
        void HookInitialize(Entry entry);
    }
}