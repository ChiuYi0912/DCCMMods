using ModCore.Events;
using MoreHeadsandskins;
using MoreHeadsandskins.customHeadEntry;
using ModCore.Events.Interfaces.Game;
using dc;

namespace MoreHeadsandskins.customHeads
{
    public class Heads :
    IEventReceiver
    {
        public Heads(customHeadMian headMian)
        {
            EventSystem.AddReceiver(this);
            LoadAllHead();
        }

        public void LoadAllHead()
        {
            _ = new RainbowHead(this);
            _ = new RedToYellow(this);
        }


    }
}