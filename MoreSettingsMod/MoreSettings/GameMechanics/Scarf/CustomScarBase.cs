using System;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using MoreSettings.Configuration;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;

namespace MoreSettings.GameMechanics.Scarf
{
    public class CustomScarfBase
    {
        public static Config<MainConfig> modConfig = SettingsMain.ModConfig;
        public Dictionary<int, ScarfData> Datakey = modConfig.Value.Scarf.Datakey;
        public BlendMode mode = default!;

        public CustomScarfBase()
        {

        }


        public ScarfData CreateScarf(int id)
        {
            var data = new ScarfData();
            Datakey[id] = data;

            return data;
        }

        public void RemoveScarf(int id)
        {
            Datakey.Remove(id);
        }


        public ScarfManager CreateScarfManagerBase(Entity entity, dc.String skinId)
        {
            var scarfManager = new ScarfManager(entity);
            var skinInfo = Cdb.Class.getSkinInfo(skinId)
                .ToVirtual<virtual_colorMap_consoleCmdId_glowData_group_head_incompatibleHeads_item_model_onlyDefaultHead_scarfBlendMode_scarfs_>(); // 用具体类型别名简化长名称


            BlendMode blendMode = skinInfo.scarfBlendMode switch
            {
                0 => new BlendMode.Alpha(),
                1 => new BlendMode.Add(),
                _ => new BlendMode.Alpha()
            };


            scarfManager.blendMode = blendMode;
            scarfManager.sbFront.blendMode = blendMode;
            scarfManager.sbBack.blendMode = blendMode;


            var scarfsArray = skinInfo.scarfs;
            if (scarfsArray != null)
            {
                for (int i = 0; i < scarfsArray.length; i++)
                {
                    var scarfInfo = ((HaxeProxyBase)scarfsArray.getDyn(i)).ToVirtual<ScarfData>();
                    var scarf = new dc.tool.Scarf(scarfManager, scarfInfo);
                    scarfManager.scarfs.push(scarf);
                }
            }

            var allScarfs = scarfManager.scarfs;
            for (int i = 0; i < allScarfs.length; i++)
            {
                var scarf = allScarfs.getDyn(i);
                scarf.init();
            }

            return scarfManager;
        }

        public ScarfManager CreateCustomScarfManager(Entity entity)
        {
            var scarfManager = new ScarfManager(entity);

            var hero = Game.Instance.HeroInstance!;
            if (Datakey.Count < 0 || !modConfig.Value.Scarf.Enabled)
                return CreateScarfManagerBase(hero, hero.getSkinInfo().item);

            scarfManager.blendMode = mode;
            scarfManager.sbFront.blendMode = mode;
            scarfManager.sbBack.blendMode = mode;


            foreach (var item in Datakey)
            {
                var scarf = new dc.tool.Scarf(scarfManager, item.Value);
                scarfManager.scarfs.push(scarf);
            }


            var allScarfs = scarfManager.scarfs;
            for (int i = 0; i < allScarfs.length; i++)
            {
                var scarf = allScarfs.getDyn(i);
                scarf.init();
            }

            return scarfManager;
        }
    }


}