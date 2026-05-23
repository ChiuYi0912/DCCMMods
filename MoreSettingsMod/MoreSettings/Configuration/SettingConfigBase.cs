using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.h2d;
using dc.hxsl._Dce;
using Hashlink.Virtuals;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;

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
        public ViewportConfig Viewport { get; set; } = new();
    }


    [Serializable]
    public class GameplayConfig : SettingConfigBase
    {
        public bool Hitpause { get; set; } = false;
        public bool LoreBankMimicRoom { get; set; } = false;
        public bool SpeedTier { get; set; } = false;
        public bool NofadeIn { get; set; } = false;
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
        public double PlayerCameraSpeed { get; set; }
        public bool ShowBossHealthBar { get; set; } = false;
        public bool RemovalUpdateNotes { get; set; } = false;
        public bool isLifeBarcolor { get; set; } = false;
        public int LifeBarcolor { get; set; } = CreateColor.ColorFromHex("#03E07A");
        public double LifeBarAlpha { get; set; } = 1;
    }


    [Serializable]
    public class ScarfConfig : SettingConfigBase
    {
        public bool UseScarfGray { get; set; } = true;
        public bool Allscarf { get; set; } = false;


        public Dictionary<int, SerializableScarfData> SerializableScarfData { get; set; } = new();


        [JsonIgnore]
        public Dictionary<int, ScarfData> RuntimeScarfData = new();


        public void UpdateFromRuntime()
        {
            SerializableScarfData.Clear();
            foreach (var kv in RuntimeScarfData)
            {
                SerializableScarfData[kv.Key] = ConvertToSerializable(kv.Value);
            }
        }


        public void LoadToRuntime()
        {
            RuntimeScarfData.Clear();
            foreach (var kv in SerializableScarfData)
            {
                RuntimeScarfData[kv.Key] = ConvertToScarfData(kv.Value);
            }
        }

        public void Remove(int key)
        {
            SerializableScarfData.Remove(key);
            RuntimeScarfData.Remove(key);
            UpdateFromRuntime();
        }

        private ScarfData ConvertToScarfData(SerializableScarfData dto)
        {
            var data = new ScarfData();
            data.attachOffX = dto.attachOffX;
            data.attachOffY = dto.attachOffY;
            data.color = dto.color;
            data.cosOffset = dto.cosOffset;
            data.count = dto.count;
            data.extraSprLength = dto.extraSprLength;
            data.friction = dto.friction;
            data.gravity = dto.gravity;
            data.maxLength = dto.maxLength;
            data.minLength = dto.minLength;
            data.onFront = dto.onFront;
            data.sprId = dto.sprId.ToHaxeString();
            data.thickness = dto.thickness;

            data.props = new virtual_backColor_customAttach_depthScaleFactor_isCape_linkTo_lockBehind_oscilFactor_rotScale_();
            data.props.backColor = dto.backColor ?? null;
            data.props.customAttach = dto.customAttach!.ToHaxeString() ?? "".ToHaxeString();
            data.props.depthScaleFactor = dto.depthScaleFactor ?? 1.0;
            data.props.isCape = dto.isCape ?? false;
            data.props.linkTo = dto.linkTo ?? null;
            data.props.lockBehind = dto.lockBehind ?? false;
            data.props.oscilFactor = dto.oscilFactor ?? 0.0;
            data.props.rotScale = dto.rotScale ?? 1.0;

            return data;
        }

        private SerializableScarfData ConvertToSerializable(ScarfData data)
        {
            var dto = new SerializableScarfData();
            dto.attachOffX = data.attachOffX;
            dto.attachOffY = data.attachOffY;
            dto.color = data.color;
            dto.cosOffset = data.cosOffset;
            dto.count = data.count;
            dto.extraSprLength = data.extraSprLength;
            dto.friction = data.friction;
            dto.gravity = data.gravity;
            dto.maxLength = data.maxLength;
            dto.minLength = data.minLength;
            dto.onFront = data.onFront;
            dto.sprId = data.sprId?.ToString() ?? "scarfGray";
            dto.thickness = data.thickness;

            if (data.props == null)
            {
                data.props = new virtual_backColor_customAttach_depthScaleFactor_isCape_linkTo_lockBehind_oscilFactor_rotScale_();
                dto.backColor = data.props.backColor;
                dto.customAttach = data.props.customAttach?.ToString() ?? "";
                dto.depthScaleFactor = data.props.depthScaleFactor;
                dto.isCape = data.props.isCape;
                dto.linkTo = data.props.linkTo;
                dto.lockBehind = data.props.lockBehind;
                dto.oscilFactor = data.props.oscilFactor;
                dto.rotScale = data.props.rotScale;
            }

            return dto;
        }
    }

    [Serializable]
    public class SerializableScarfData
    {
        public double attachOffX { get; set; }
        public double attachOffY { get; set; }
        public int? color { get; set; }
        public double cosOffset { get; set; }
        public int count { get; set; }
        public int? extraSprLength { get; set; }
        public double friction { get; set; }
        public double gravity { get; set; }
        public double maxLength { get; set; }
        public double minLength { get; set; }
        public bool onFront { get; set; }
        public string sprId { get; set; } = "scarfGray";
        public double thickness { get; set; }


        public int? backColor { get; set; }
        public string? customAttach { get; set; } = "";
        public double? depthScaleFactor { get; set; }
        public bool? isCape { get; set; }
        public int? linkTo { get; set; }
        public bool? lockBehind { get; set; }
        public double? oscilFactor { get; set; }
        public double? rotScale { get; set; }
    }


    [Serializable]
    public class SkinConfig : SettingConfigBase
    {
        public bool SkinEnabled { get; set; } = false;
        public bool Skinkatana { get; set; } = false;
        public bool HotlineSkin { get; set; } = false;
        public bool StsSkin { get; set; } = false;
        public bool RiskOfRainSkin { get; set; } = false;
        public bool KatanaSkin { get; set; } = false;

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

    [Serializable]
    public class ViewportConfig : SettingConfigBase
    {
        public double ViewportbumAng { get; set; } = 0;
        public double Viewportbumdir { get; set; } = 0;
        public double ViewportshakesX { get; set; } = 0;
        public double ViewportshakesY { get; set; } = 0;
        public double ViewportshakesD { get; set; } = 0;
        public double ViewportshakeReversedSX { get; set; } = 0;
        public double ViewportshakeReversedSY { get; set; } = 0;
        public double ViewportshakeReversedSD { get; set; } = 0;
        public double Camerazoom { get; set; } = 1;

        public bool TeleportImmediate { get; set; } = false;
    }
}