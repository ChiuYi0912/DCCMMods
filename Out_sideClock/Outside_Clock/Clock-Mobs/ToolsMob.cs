using dc;
using dc.hl;
using dc.hxd.res;
using ModCore.Utitities;
using Serilog;

namespace Outside_Clock;

public class ToolsMob
{

    public static dc.libs.heaps.slib.SpriteLib Replace_atlas(Hook_AssetsLibManager.orig_get orig, AssetsLibManager self, dc.String o, string oldatlas, string atlas_name)
    {

        if (o.ToString().Equals(oldatlas, StringComparison.CurrentCultureIgnoreCase))
        {
            Log.Debug($" 主图替换: {o.ToString()} -> {atlas_name}");
            return orig(self, atlas_name.AsHaxeString());
        }
        return orig(self, o);
    }

    public static dc.hxd.res.Resource Hook_Loader_loadCache(Hook_Loader.orig_loadCache orig, Loader self, dc.String c, Class res, string old_npng, string _npng_name)
    {

        if (c.ToString().Equals(old_npng, StringComparison.CurrentCultureIgnoreCase))
        {
            Log.Debug($" 法线图替换: {c.ToString()} -> {_npng_name}");
            return orig(self, _npng_name.AsHaxeString(), res);
        }
        return orig(self, c, res);
    }
}
