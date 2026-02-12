
namespace ChiuYiUI
{
    public class PothionsConfig
    {

        public bool SkinEnabled { get; set; } = false;
        public bool skinkatana { get; set; } = false;
        public bool teleport { get; set; } = false;
        public bool pop { get; set; } = false;
        public bool rsty { get; set; } = false;


        public bool UseScarfGray { get; set; } = true;

        public bool Allscarf { get; set; } = false;

        public double ViewportbumAng { get; set; }
        public double Viewportbumdir { get; set; }
        public double ViewportshakeReversedSX { get; set; }
        public double ViewportshakeReversedSY { get; set; }
        public double ViewportshakeReversedSD { get; set; }
        public double ViewportshakesX { get; set; }
        public double ViewportshakesY { get; set; }
        public double ViewportshakesD { get; set; }


        public bool Hitpause { get; set; } = false;
        public bool LoreBankMimicRoom { get; set; } = false;
        public bool SpeedTier { get; set; } = false;

        public bool NewsPanel { get; set; } = false;



        public bool HasBottomBar { get; set; } = false;
        public bool NoVignette { get; set; } = false;
        public bool HaslightTip { get; set; } = false;
        public bool HasNoPopText { get; set; } = false;
        public bool HasNoStatusIcon { get; set; } = false;

        public double LifeBarcolor { get; set; } = 0;
    }
}