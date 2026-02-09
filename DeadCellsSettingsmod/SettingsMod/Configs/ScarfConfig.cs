using System;
using System.IO;
using System.Text.Json;
using dc;
using ModCore.Events.Interfaces.Game;
using Serilog;

namespace ChiuYiUI
{
    public class Config
    {

        public Dictionary<int, ScarfConfig> ScarfConfigs { get; set; } = new ()
        {

        };

    }



    #region 围巾配置类
    public class ScarfConfig
    {

        public string SprId { get; set; } = "scarfGlow";
        public int CosOffset { get; set; } = 3;
        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();
        public int AttachOffX { get; set; } = -4;
        public int AttachOffY { get; set; } = 2;
        public double MaxLength { get; set; } = 0;
        public double Friction { get; set; } = 0;
        public double MinLength { get; set; } = 0;
        public bool OnFront { get; set; } = false;
        public int Color { get; set; } = 8724512;
        public int Count { get; set; } = 0;
        public double Gravity { get; set; } = 0;
        public double Thickness { get; set; } = 0;

        public ScarfConfig() { }

        public ScarfConfig(string sprId, int maxLength, double friction, int color, int count, double gravity, int thickness)
        {
            SprId = sprId;
            MaxLength = maxLength;
            Friction = friction;
            Color = color;
            Count = count;
            Gravity = gravity;
            Thickness = thickness;
        }

        public ScarfConfig(ScarfConfig other)
        {
            if (other != null)
            {
                SprId = other.SprId;
                CosOffset = other.CosOffset;
                Props = new Dictionary<string, object>(other.Props);
                AttachOffX = other.AttachOffX;
                AttachOffY = other.AttachOffY;
                MaxLength = other.MaxLength;
                Friction = other.Friction;
                MinLength = other.MinLength;
                OnFront = other.OnFront;
                Color = other.Color;
                Count = other.Count;
                Gravity = other.Gravity;
                Thickness = other.Thickness;
            }
        }
    }
    #endregion

    public static class ConfigManager
    {
        private static string ConfigPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DeadCells", "Mods", "Skines", "config.json");

        public static Config LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<Config>(json) ?? new Config();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"加载配置失败: {ex.Message}");
            }

            return new Config();
        }

        public static void SaveConfig(Config config)
        {
            try
            {
#pragma warning disable CS8600
                string directory = Path.GetDirectoryName(ConfigPath);
#pragma warning restore CS8600
                if (!Directory.Exists(directory))
                {
#pragma warning disable CS8604
                    Directory.CreateDirectory(directory);
#pragma warning restore CS8604
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"保存配置失败: {ex.Message}");
            }
        }
    }
}