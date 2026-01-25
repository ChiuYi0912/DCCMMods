using dc;
using dc.en;

namespace Outside_Clock;

public class EntrancOut_Clock : GameCinematic
{


    Hero owen = ModCore.Modules.Game.Instance.HeroInstance!;
    public EntrancOut_Clock(Hero owen)
    {
        _EntrancOut_Clock.heroanimOut_Clock(owen, this);
    }
    public override void init()
    {
        base.init();

    }

    public override void update()
    {
        base.update();

    }
}

