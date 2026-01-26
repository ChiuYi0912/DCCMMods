using dc;
using dc.level;
using dc.libs;
using dc.tool.mod;
using dc.ui.hud;
using Hashlink.Virtuals;
using ModCore.Events.Interfaces.Game;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utitities;
using dc.h2d;
using dc.level.@struct;
using dc.pr;
using dc.cine;
using dc.en.inter;
using ModCore.Storage;
using Outside_Clock.Clock_Mobs;
using dc.libs.heaps.slib;
using dc.hl.types;
using dc.h2d.col;
using HaxeProxy.Runtime;
using dc.light;
using dc.h3d.col;
using dc.h3d;
using Serilog;
using dc.parallax;
using dc.en;

namespace Outside_Clock
{
    public class Mian : ModBase,
        IOnGameExit,
        IOnGameEndInit
    {
        public Mian(ModInfo info) : base(info)
        {

        }


        public override void Initialize()
        {
            Logger.Information("你好，世界");
            Hook__LevelStruct.get += Hook__LevelStruct_get;

            Hook_LevelLogos.getLevelLogo += Hook_LevelLogos_getLevelLogo;

            Hook_T_SewerShort.buildMainRooms += Hook_T_SewerShort_buildMainRooms;

            Hook_Level.attachSpecialEquipments += hook_Level_attachSpecialEquipments;

            Hook_LevelTransition.entranceWalk += Hook_LevelTransition_entranceWalk;

            //dc.pr.Hook_Level.init += Hook_Level_init;

            Hook_HiddenTrigger.trigger += CinematicOut_Clock_Main.Hook_HiddenTrigger_trigger;

            MobcreateMain mobcreateMain = new MobcreateMain();
            dc.en.Hook__Mob.create += mobcreateMain.Hook__Mob_create;

            //dc.level.disp.Hook_ClockTower.renderBackground += ClockTower_renderBackground;

            Hook_Hero.wakeup += hook_hero_wakeup;


        }

        private void hook_hero_wakeup(Hook_Hero.orig_wakeup orig, Hero self, Level lvl, int cx, int cy)
        {
            orig(self, lvl, cx, cy);
            LevelTransition.Class.@goto("Out_Clock".AsHaxeString());
        }

        private void ClockTower_renderBackground(dc.level.disp.Hook_ClockTower.orig_renderBackground orig, dc.level.disp.ClockTower self)
        {


            orig(self);
            SpriteLib slib = self.level.slib;

            _String _String = dc.String.Class;
            dc.String px = _String.__add__("bg/".AsHaxeString(), "paralax2".AsHaxeString());

            int scrollX = Const.Class.DP_BACKGROUND;
            double scrollY = 0.1;
            double p = 0.1;
            Parallax parallax2 = self.createParallax(scrollX, scrollY, p);

            double py = 0.5;
            var ptr = Ref<double>.From(ref py);

            double rndFunc = 0.5;
            var ptr2 = Ref<double>.From(ref rndFunc);

            var flipMode = new HlFunc<int, int>(self.rng.random);


            Scatterer scatterer = new Scatterer(parallax2);
            scatterer.alpha = 1.0;
            var bounds = self.lmap.bounds;
            parallax2.x = (bounds.xMax - bounds.xMin) * 0.4f;
            parallax2.y = (bounds.yMax - bounds.yMin) * 0.7f;
            parallax2.posChanged = true;

            Bitmap bitmap2 = new Bitmap(slib.getTileRandom(px, ptr, ptr2, flipMode, null), scatterer);
            dc.String string2 = self.lmap.biome.scatterConf;

            self.applyScatterConf(scatterer, string2);

            // Vector color = bitmap2.color;
            // color.x = 2.0;
            // color.y = 0.9176470588235294;
            // color.z = 0.7098039215686275;
            // color.w = 1.0;

            var add = new BlendMode.Add();
            bitmap2.blendMode = add;

            var length = self.bgScrolls.length;

            for (int i = 0; i < length; i++)
            {
                var Debug = self.bgScrolls.array[i];
                Log.Debug($"bgScrolls数组内容:{Debug}");
            }


        }

        private void Hook_Level_init(Hook_Level.orig_init orig, Level self)
        {

            dc.String id = self.map.id;
            LevelDisp Disp;
            if (id != null)
            {
                if (id.ToString() == "Outside_Clock")
                {
                    var map = self.map;
                    Disp = new level_clock.Outside_clockBG(self, map, id);
                }
            }
            orig(self);

        }

        private void Hook_LevelTransition_entranceWalk(Hook_LevelTransition.orig_entranceWalk orig, LevelTransition self, int xFrom, int xTo, Exit exit)
        {
            virtual_exit_from_to_ virtual_exit_from_to_ = new virtual_exit_from_to_();
            virtual_exit_from_to_.from = xFrom;
            virtual_exit_from_to_.to = xTo;
            virtual_exit_from_to_.exit = exit;
            self.walk = virtual_exit_from_to_;

        }

        private void hook_Level_attachSpecialEquipments(Hook_Level.orig_attachSpecialEquipments orig, Level self, Room rseed, Rand cineTrans, LevelTransition pt)
        {

            orig(self, rseed, cineTrans, pt);
            // if (rseed == null) return;
            // dc.String rtype = rseed.rType;
            // if (@rtype != null)
            // {
            //     if (rseed != null)
            //     {
            //         Marker? marker = null;
            //         if (marker == null)
            //         {
            //             try
            //             {
            //                 bool found = true;
            //                 marker = rseed.getMarker("SpecialEquipment".AsHaxeString(), null, new Ref<bool>(ref found));
            //                 Logger.Debug("maker:不为空");
            //             }
            //             catch
            //             {
            //                 Logger.Debug("majer:为空");
            //             }

            //             if (marker != null)
            //             {
            //                 if (marker.customId?.ToString() == "tower".ToString())
            //                 {
            //                     Logger.Debug("接管:tower");

            //                     Hero hero = self.game.hero;
            //                     Outside_Clock.EntrancOut_Clock entrancOut_ = new Outside_Clock.EntrancOut_Clock(hero);
            //                     miniLeapingDuelyst duelyst = new miniLeapingDuelyst(hero._level, hero.cx, hero.cy, 10, 10);
            //                 }
            //             }
            //         }
            //     }
            // }
        }


        private RoomNode Hook_T_SewerShort_buildMainRooms(Hook_T_SewerShort.orig_buildMainRooms orig, T_SewerShort self)
        {
            RoomNode parent = self.createNode(null, "EntranceSewerDown".AsHaxeString(), null, "start".AsHaxeString());
            RoomNode roomNode = self.createNode("Collector".AsHaxeString(), null, null, null);
            RoomNode roomNode2 = roomNode.set_parent(parent);
            roomNode2 = self.createNode(null, "PerkShop".AsHaxeString(), null, null);
            RoomNode roomNode3 = roomNode2.set_parent(roomNode);
            roomNode3 = self.createNode("Healing".AsHaxeString(), null, null, null);
            RoomNode roomNode4 = roomNode3.set_parent(roomNode2);
            dc.String transitionTo = self.lInfos.transitionTo;
            RoomNode roomNode5 = self.createExit(transitionTo, null, null, "exit".AsHaxeString()).set_parent(roomNode3);
            RoomNode roomNode6 = self.createExit("Out_Clock".AsHaxeString(), null, null, "exit1".AsHaxeString()).addBefore(self.getId("exit".AsHaxeString()), null);
            return self.getId("start".AsHaxeString());
        }

        private Tile Hook_LevelLogos_getLevelLogo(Hook_LevelLogos.orig_getLevelLogo orig, LevelLogos self, dc.String levelLogoCoordinate)
        {
            if (!self.textureCoordinateByLevelKind.exists.Invoke(levelLogoCoordinate))
            {
                return orig(self, "ClockTower".AsHaxeString());
            }
            else return orig(self, levelLogoCoordinate);

        }

        private LevelStruct Hook__LevelStruct_get(Hook__LevelStruct.orig_get orig, User user, virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ l, Rand rng)
        {

            var idStr = l.id.ToString();
            if (idStr.Equals("Out_Clock", StringComparison.CurrentCultureIgnoreCase))
            {
                return new Out_Clock(user, l, rng);
            }
            else return orig(user, l, rng);
        }



        void IOnGameEndInit.OnGameEndInit()
        {
            var res = Info.ModRoot!.GetFilePath("res.pak");
            FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
            var json = CDBManager.Class.instance.getAlteredCDB();
            dc.Data.Class.loadJson(
               json,
               default);
        }

        private static Config<Out_Clock_Config> config { get; } = new("Out_Clock_Enter");
        void IOnGameExit.OnGameExit()
        {
            config.Value.Out_Clock_Enter = false;
            config.Value.createminileaping = false;
            config.Save();

            Logger.Debug("已初始化过场动画");
        }
    }
}