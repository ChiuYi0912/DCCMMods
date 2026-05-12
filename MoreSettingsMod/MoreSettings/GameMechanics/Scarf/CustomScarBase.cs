using System;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.h2d;
using dc.hl.types;
using dc.tool;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using MoreSettings.Configuration;
using MoreSettings.Utilities;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;

namespace MoreSettings.GameMechanics.Scarf
{
    public class CustomScarfBase
    {
        public static Config<MainConfig> modConfig = SettingsMain.ModConfig;
        public Dictionary<int, ScarfData> Datakey = new();
        public BlendMode mode = default!;
        public CustomScarfUI scarfUI = null!;

        public CustomScarfBase() { }

        public void Load()
        {
            var config = modConfig.Value.Scarf;
            if (config.SerializableScarfData.Count > 0)
            {
                config.LoadToRuntime();
                Datakey = config.RuntimeScarfData;
            }
        }


        private void InitDefaultValues(ScarfData scarfData)
        {
            scarfData.attachOffX = 0.0;
            scarfData.attachOffY = 0.0;
            scarfData.color = 0x801234;
            scarfData.cosOffset = 0;
            scarfData.count = 11;
            scarfData.extraSprLength = null;
            scarfData.friction = 0.6;
            scarfData.gravity = 0.9;
            scarfData.maxLength = 4.0;
            scarfData.minLength = 3.0;
            scarfData.onFront = false;
            scarfData.sprId = "scarfGray".ToHaxeString();
            scarfData.thickness = 1.0;

            scarfData.props.backColor = null;
            scarfData.props.customAttach = "".ToHaxeString();
            scarfData.props.depthScaleFactor = 1.0;
            scarfData.props.isCape = false;
            scarfData.props.linkTo = null;
            scarfData.props.lockBehind = false;
            scarfData.props.oscilFactor = 0.0;
            scarfData.props.rotScale = 1.0;
        }

        public ScarfData CreateScarf(int id)
        {
            var data = new ScarfData();
            Datakey[id] = data;
            Save();
            return data;
        }

        public void RemoveScarf(int id)
        {
            Datakey.Remove(id);
            Save();
        }

        public void Save()
        {
            var config = modConfig.Value.Scarf;
            config.RuntimeScarfData = Datakey;
            config.UpdateFromRuntime();
            modConfig.Save();
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

            mode = new BlendMode.Alpha();

            scarfManager.blendMode = mode;
            scarfManager.sbFront.blendMode = mode;
            scarfManager.sbBack.blendMode = mode;


            foreach (var item in Datakey)
            {
                var scarf = new dc.tool.Scarf(scarfManager, item.Value);
                scarfManager.scarfs.push(scarf);
                scarf.Meta.key = item.Key;
            }


            var allScarfs = scarfManager.scarfs;
            for (int i = 0; i < allScarfs.length; i++)
            {
                dc.tool.Scarf scarf = allScarfs.getDyn(i);
                scarf.init();
            }


            return scarfManager;
        }

        public void UpdateSarfs()
        {
            if (!SettingsMain.ConfigValue.Scarf.Enabled)
            {
                scarfUI.hero.initScarf(); 
                return;
            }

            var hero = Game.Instance.HeroInstance!;
            var scmanager = hero.scarf;
            foreach (dc.tool.Scarf item in scmanager.scarfs.AsEnumerable())
            {
                item.dispose();

                scmanager.scarfs.removeDyn(item);
            }

            foreach (var item in Datakey)
            {
                var scarf = new dc.tool.Scarf(scmanager, item.Value);
                scmanager.scarfs.push(scarf);
            }

            var allScarfs = hero.scarf.scarfs;
            for (int i = 0; i < allScarfs.length; i++)
            {
                var scarf = allScarfs.getDyn(i);
                scarf.init();
            }
        }

        public void UpdateSarf(int key)
        {
            var hero = Game.Instance.HeroInstance!;
            var scmanager = hero.scarf;

            int? scarfkay = null;
            foreach (dc.tool.Scarf item in scmanager.scarfs.AsEnumerable())
            {
                if (item.Meta.key == key)
                {
                    scarfkay = key;
                    item.dispose();
                    scmanager.scarfs.removeDyn(item);
                }
            }

            if (scarfkay == null)
                return;
            var scarf = new dc.tool.Scarf(scmanager, Datakey[key]);
            scmanager.scarfs.push(scarf);
            scarf.init();
        }
    }


}