using ModCore.Events;
using MoreHeadsandskins.customHeads;
using MoreHeadsandskins;

namespace MoreHeadsandskins.customHeadEntry
{
    public class customHeadMian :
    IEventReceiver
    {
        public Entry GetEntry;
        public customHeadMian(Entry entry)
        {
            GetEntry = entry;
            EventSystem.AddReceiver(this);
            _ = new Heads(this);
        }
    }
}