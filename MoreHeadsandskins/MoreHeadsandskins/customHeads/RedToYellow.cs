using dc;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;

namespace MoreHeadsandskins.customHeads
{
    public class RedToYellow:
    IEventReceiver,
    IOnAfterLoadingCDB
    {
        public RedToYellow(Heads heads)
        {
            EventSystem.AddReceiver(this);
        }

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {
            Headcdbload(cdb);
        }
        public void Headcdbload(_Data_ cdb)
        {
           
        }

        
    }
}