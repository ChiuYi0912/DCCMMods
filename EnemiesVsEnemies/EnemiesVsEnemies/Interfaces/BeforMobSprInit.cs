using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc.en;
using ModCore.Events;

namespace EnemiesVsEnemies.Interfaces
{
    [Event]
    public interface IOnBeforMobSprInit
    {
        void BeforMobSprScaleUpdate(Mob mob);
    }
}