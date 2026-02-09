using dc;
using ModCore.Events;
using ModCore.Utilities;
using Outside_Clock.Interface.IOnAdvancedModuleInitializing;

namespace Outside_Clock.CommonAtlas
{
    public class CommonAtlasClock :
    IEventReceiver,
    IOnAdvancedModuleInitializing
    {
        public CommonAtlasClock()
        {
            EventSystem.AddReceiver(this);
        }

        void IOnAdvancedModuleInitializing.OnAdvancedModuleInitializing(Outside_Main MODMAN)
        {
            Hook__Assets.getLevelCommonAtlasPath += Hook__Assets_CommonAtlasClock;
        }

        private dc.String Hook__Assets_CommonAtlasClock(Hook__Assets.orig_getLevelCommonAtlasPath orig, int atlas)
        {
            if (atlas == 3)
            {
                return "atlas/CommonAtlasClock.atlas".AsHaxeString();
            }
            return orig(atlas);
        }
    }
}