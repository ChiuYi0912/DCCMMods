using System;
using System.Collections.Generic;

namespace MoreSettings.Configuration
{

    [Serializable]
    public abstract class SettingConfigBase
    {
        public bool Enabled { get; set; } = true;
        public int ConfigVersion { get; set; } = 1;
    }

    [Serializable]
    public class MainConfig : SettingConfigBase
    {
        public GameplayConfig Gameplay { get; set; } = new();
        public UIConfig UI { get; set; } = new();
        public ScarfConfig Scarf { get; set; } = new();
        public SkinConfig Skin { get; set; } = new();
        public TeleportConfig Teleport { get; set; } = new();
        public AudioConfig Audio { get; set; } = new();
    }


    [Serializable]
    public class GameplayConfig : SettingConfigBase
    {
        public bool Hitpause { get; set; } = false;
        public bool LoreBankMimicRoom { get; set; } = false;
        public bool SpeedTier { get; set; } = false;
    }


    [Serializable]
    public class UIConfig : SettingConfigBase
    {
        public bool NewsPanel { get; set; } = false;
        public bool HasBottomBar { get; set; } = false;
        public bool NoVignette { get; set; } = false;
        public bool HaslightTip { get; set; } = false;
        public bool HasNoPopText { get; set; } = false;
        public bool HasNoStatusIcon { get; set; } = false;
        public bool NowTimeVisible { get; set; } = false;
        public double LifeBarcolor { get; set; } = 0;
        public double PlayerCameraSpeed { get; set; }
    }


    [Serializable]
    public class ScarfConfig : SettingConfigBase
    {
        public bool UseScarfGray { get; set; } = true;
        public bool Allscarf { get; set; } = false;

        public Dictionary<int, SingleScarfConfig> ScarfConfigs { get; set; } = new();
    }


    [Serializable]
    public class SingleScarfConfig
    {
        public string SprId { get; set; } = "scarfGlow";
        public double CosOffset { get; set; } = 3;
        public double AttachOffX { get; set; } = -4;
        public double AttachOffY { get; set; } = 2;
        public double MaxLength { get; set; } = 2;
        public double Friction { get; set; } = 0.6;
        public double MinLength { get; set; } = 2;
        public bool OnFront { get; set; } = false;
        public int Color { get; set; } = 8724512;
        public int Count { get; set; } = 6;
        public double Gravity { get; set; } = 1.5;
        public double Thickness { get; set; } = 2;
        public Dictionary<string, object> Props { get; set; } = [];
    }


    [Serializable]
    public class SkinConfig : SettingConfigBase
    {
        public bool SkinEnabled { get; set; } = false;
        public bool Skinkatana { get; set; } = false;
    }


    [Serializable]
    public class TeleportConfig : SettingConfigBase
    {
        public bool Teleport { get; set; } = false;
        public bool DIYFlashTeleport { get; set; } = false;
    }


    [Serializable]
    public class AudioConfig : SettingConfigBase
    {
        public bool Pop { get; set; } = false;
        public bool Rsty { get; set; } = false;
    }
}