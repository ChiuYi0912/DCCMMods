using dc;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;

namespace MoreHeadsandskins.customHeads.Rainbow
{
    public class RainbowHead:
    IEventReceiver,
    IOnAfterLoadingCDB
    {
        public RainbowHead(Heads heads)
        {
            EventSystem.AddReceiver(this);
        }
        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {
            Headcdbload(cdb);
        }

        public void Headcdbload(_Data_ cdb)
        {
            for (int i = 31; i <= 33; i++)
            {
                var particelconf = cdb.particleConf.all.getDyn(i);

                particelconf.colorProps.colorStart = 16737280;
                particelconf.colorProps.colorEnd = 16711807;
                particelconf.alphaProps.alphaMax = 1.0f;
                particelconf.alphaProps.alphaMin = 1.0f;
                if (i == 8 || i == 9)
                {
                    particelconf.lifeProps.lifeDurationMax = 1.0f;
                    particelconf.lifeProps.lifeDurationMin = 1.0f;
                }
            }
        }

        private static readonly List<int> PinkColors = new()
        {
            16777215,   // 纯白色
            16777184,   // 极浅粉色
            16777153,   // 很浅的粉色
            16777122,   // 浅粉色1
            16777091,   // 浅粉色2
            16777060,   // 浅粉色3
            16777029,   // 浅粉色4
            16776998,   // 浅粉色5
            16776967,   // 浅粉色6
            
           
            16767644,   // 淡粉色1 0xFFD1DC
            16767613,   // 淡粉色2
            16767582,   // 淡粉色3
            16767551,   // 淡粉色4
            
            
            16758465,   // 淡粉色5 0xFFB6C1
            16758434,   // 淡粉色6
            16758403,   // 淡粉色7
            16758372,   // 淡粉色8
            
            
            16761035,   // 标准粉色 0xFFC0CB
            16761004,   // 粉色2
            16760973,   // 粉色3
            16760942,   // 粉色4
            
           
            16738740,   // 珊瑚粉 0xFF7074
            16738709,   // 珊瑚粉2
            16738678,   // 珊瑚粉3
            16738647,   // 珊瑚粉4
            
            
            16711807,   // 亮粉色 0xFF00FF
            16711776,   // 亮粉色2
            16711745,   // 亮粉色3
            16711714,   // 亮粉色4
            
            
            16737280,   // 橙粉色 0xFF7F00
            16737249,   // 橙粉色2
            16737218,   // 橙粉色3
            16737187,   // 橙粉色4
            
            
            8388736,    // 深粉色1 0x800080
            8388705,    // 深粉色2
            8388674,    // 深粉色3
            8388643,    // 深粉色4
            
           
            8388643,    // 深粉色4
            8388674,    // 深粉色3
            8388705,    // 深粉色2
            8388736,    // 深粉色1
            
            16737187,   // 橙粉色4
            16737218,   // 橙粉色3
            16737249,   // 橙粉色2
            16737280,   // 橙粉色
            
            16711714,   // 亮粉色4
            16711745,   // 亮粉色3
            16711776,   // 亮粉色2
            16711807,   // 亮粉色
            
            16738647,   // 珊瑚粉4
            16738678,   // 珊瑚粉3
            16738709,   // 珊瑚粉2
            16738740,   // 珊瑚粉
            
            16760942,   // 粉色4
            16760973,   // 粉色3
            16761004,   // 粉色2
            16761035,   // 标准粉色
            
            16758372,   // 淡粉色8
            16758403,   // 淡粉色7
            16758434,   // 淡粉色6
            16758465,   // 淡粉色5
            
            16767551,   // 淡粉色4
            16767582,   // 淡粉色3
            16767613,   // 淡粉色2
            16767644,   // 淡粉色1
            
            16776967,   // 浅粉色6
            16776998,   // 浅粉色5
            16777029,   // 浅粉色4
            16777060,   // 浅粉色3
            16777091,   // 浅粉色2
            16777122,   // 浅粉色1
            16777153,   // 很浅的粉色
            16777184,   // 极浅粉色
            16777215,   // 白色
        };

        private int currentIndex = 0;

        public void UpdateColorsPerFrame()
        {
            currentIndex = (currentIndex + 1) % PinkColors.Count;

            int startColor = PinkColors[currentIndex];
            int endColor = PinkColors[(currentIndex + 1) % PinkColors.Count];
            for (int i = 31; i <= 33; i++)
            {
                var particleConf = Data.Class.particleConf.all.getDyn(i);
                particleConf.colorProps.colorStart = startColor;
                particleConf.colorProps.colorEnd = endColor;
            }
        }
    }
}