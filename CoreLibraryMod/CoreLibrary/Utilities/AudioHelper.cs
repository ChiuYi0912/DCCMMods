using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.hxd;
using dc.hxd.res;

namespace CoreLibrary.Utilities
{
    public class AudioHelper
    {
        public static void LoadAudioFormString(string PATH = "sfx/ui/menu_click1.wav") => _ = Audio.Class.ME.playUIEvent((Sound)Res.Class.get_loader().loadCache(PATH.ToHaxeString(), Sound.Class), null);
    }
}