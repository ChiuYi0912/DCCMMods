using System.ComponentModel.DataAnnotations;
using dc;
using dc._Data;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.en.inter.npc;
using dc.h2d;
using dc.haxe.ds;
using dc.hl.types;
using dc.hxd;
using dc.level;
using dc.level.disp;
using dc.libs;
using dc.libs.heaps;
using dc.libs.tilemap;
using dc.light;
using dc.pow;
using dc.pr;
using dc.tool;
using dc.tool.log;
using dc.tool.vote;
using dc.ui;
using dc.ui.hud;
using DebugMod.Utitities;
using Hashlink.Proxy.Clousre;
using Hashlink.Proxy.DynamicAccess;
using Hashlink.Proxy.Objects;
using Hashlink.Reflection.Types;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using IngameDebugConsole;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Utilities;
using Game = dc.pr.Game;

namespace DebugMod
{
    public class DebugModMod(ModInfo info) : ModBase(info),
        IOnBeforeGameInit,
        IOnHeroUpdate,
        IOnAfterLoadingCDB
    {

        public override void Initialize()
        {
            base.Initialize();

        }

        private void Hook_Console_ctor(HashlinkClosure orig, HashlinkObject self)
        {
            orig.DynamicInvoke(self);
            var s = self.AsHaxe<dc.ui.Console>();
            var ss = dc.ui.Console.Class;
            ss.HIDE_UI = "FDMM_HIDE_UI".AsHaxeString();
            ss.HIDE_DEBUG = "FDMM_HIDE_DEBUG".AsHaxeString();
            ss.HIDE_CONSOLE = "FDMM_HIDE_CONSOLE".AsHaxeString();
            s.activateDebug();


        }
        void IOnBeforeGameInit.OnBeforeGameInit()
        {
            var hh = HashlinkHooks.Instance;

            dc.h2d.Hook_Console.handleCommand += Hook_Console_handleCommand1;
            dc.ui.Hook_Console.log += Hook_Console_log1;
            dc.h2d.Hook__Console.__constructor__ += Hook__Console__constructor__;

            hh.CreateHook("ui.$Console", "__constructor__", Hook_Console_ctor).Enable();

            Hook_Game.init += Hook_Game_init;
            Hook_DebugHud.postUpdate += Hook_DebugHud_postUpdate;
            Hook__DebugHud.__constructor__ += Hook_DebugHud_initialize;
            Hook_DebugHud.initLogsUi += Hook_DebugHud_initlogui;
            Hook_DebugHudOutput.receiveLog += Hook_DebugHudOutput_receiveLog;

        }
        private void Hook_Game_init(Hook_Game.orig_init orig, dc.pr.Game self)
        {
            orig(self);
            dc.ui.DebugHud debug = new DebugHud();
            debug.initLogsUi();
            debug.initShaderCacheErrorUi();
            debug.initCollisionText();
            debug.initGraphicPerformanceText();
            debug.initGraphicTexts();
            debug.initCpuTexts();
        }

        private void Hook_DebugHudOutput_receiveLog(Hook_DebugHudOutput.orig_receiveLog orig, DebugHudOutput self, virtual_date_pos_severity_text_ logEntry)
        {
            virtual_count_textColor_ virtual_count_textColor_ = self.severityData.get(logEntry.severity).ToVirtual<virtual_count_textColor_>();
            virtual_count_textColor_.count = virtual_count_textColor_.count + 1;
        }


        private void Hook_DebugHud_initlogui(Hook_DebugHud.orig_initLogsUi orig, DebugHud self)
        {
            DebugHudOutput logOutput = (DebugHudOutput)LogUtils.Class.getOutput(DebugHudOutput.Class);
            if (logOutput == null) return;

            self.logsFlow = new Flow(self.root);

            self.logsFlow.set_minHeight(50);
            self.logsFlow.set_paddingLeft(10);
            self.logsFlow.set_paddingRight(10);
            self.logsFlow.set_paddingTop(5);
            self.logsFlow.set_paddingBottom(5);
            self.logsFlow.set_horizontalSpacing(25);
            self.logsFlow.set_horizontalAlign(new FlowAlign.Middle());
            self.logsFlow.set_verticalAlign(new FlowAlign.Middle());


            var bgTile = Tile.Class.fromColor(logOutput.bgColor, null, null, 0.9, null);
            self.logsFlow.set_backgroundTile(bgTile);

            self.logsFlow.set_visible(true);

            self.logsTexts = new EnumValueMap();

            var iterator = logOutput.severityData.keys().ToVirtual<virtual_hasNext_next_<HlFunc<Severity>>>();

            while (iterator.hasNext.Invoke())
            {
                Severity severity = iterator.next.Invoke();

                dc.h2d.Text severityText = new dc.h2d.Text(Assets.Class.font18, self.logsFlow);

                int textColor = logOutput.severityData.get(severity).ToVirtual<virtual_count_textColor_>().textColor;
                severityText.set_textColor(textColor);
                self.logsTexts.set(severity, severityText);
                logOutput.receiveLog(self.logsTexts.ToVirtual<virtual_date_pos_severity_text_>());
            }

            self.updateLogsDisplay();
            self.onResize();
        }


        private void Hook_DebugHud_initialize(Hook__DebugHud.orig___constructor__ orig, DebugHud arg1)
        {
            orig(arg1);
            arg1.root.set_visible(true);
        }


        private void Hook_DebugHud_postUpdate(Hook_DebugHud.orig_postUpdate orig, DebugHud self)
        {
            if (self.logsFlow != null)
            {
                self.logsFlow.set_visible(true);
                self.updateLogsDisplay();
            }
            if (!self.root.visible || Game.Class.ME == null || Game.Class.ME.curLevel == null)
                return;

            Level currentLevel = Game.Class.ME.curLevel;


            virtual_cx_cy_gx_gy_sx_sy_ mouseData = currentLevel.getMouse();
            if (mouseData != null)
            {
                LevelMap levelMap = currentLevel.map;
                Room currentRoom = levelMap.getRoomAt(mouseData.cx, mouseData.cy);

                if (currentRoom != null)
                {
                    int roomX = mouseData.cx - currentRoom.x;
                    int roomY = mouseData.cy - currentRoom.y;
                    self.mouse.set_text($"pixel={mouseData.sx}, {mouseData.sy} / case={mouseData.cx}, {mouseData.cy} / room={roomX}, {roomY}".AsHaxeString());
                }
            }


            int fxAlloc = currentLevel.fx.pool.nalloc;

            int activeCritters = currentLevel.countActiveCritters();
            int totalCritters = currentLevel.critters.length;

            int splattersCount = currentLevel.splatters.length;

            self.statsText.set_text($"fx={fxAlloc} critters={activeCritters}/{totalCritters} splatters={splattersCount}".AsHaxeString());


            int visibleLights = PointLight.Class.nvisible;
            int updatedLights = PointLight.Class.nupdated;
            self.lights.set_text($"lights: nvisible={visibleLights} nupdated={updatedLights}".AsHaxeString());


            int mobsLeft = currentLevel.get_mobsLeftCount();
            int totalMobs = currentLevel.get_totalMobCount();
            self.mobs.set_text($"Mobs count={mobsLeft}/{totalMobs} (exclude malaise spawned)".AsHaxeString());


            int averageFPS = self.fpsAverage.getAverage();
            self.fpsValueText.updateValue(averageFPS);


            int drawCalls = Boot.Class.ME.engine.drawCalls;
            self.drawCallsValueText.updateValue(drawCalls);


            int visibleObjects = ObjectHelper.Class.getVisibleObjectsCount(self.graphicsObjects);
            int totalObjects = self.graphicsObjects.length;
            int culledObjects = totalObjects - visibleObjects;
            self.objectVisibilityText.set_text($"Object visible: {visibleObjects} Object culled: {culledObjects}".AsHaxeString());

            int tileCount = self.getTileGroupTileCount();
            self.tileGroupCountText.set_text($"TileGroup count: {tileCount}".AsHaxeString());


            int textureCount = MemoryManagerExtender.Class.getTexturesCount(Boot.Class.ME.engine.mem);
            self.textureCountText.updateValue(textureCount);

            int textureMemoryKB = (int)(MemoryManagerExtender.Class.getTextureMemoryUsed(Boot.Class.ME.engine.mem) / 1000.0);
            self.textureMemText.updateValue(textureMemoryKB);
        }


       




        private void Hook__Console__constructor__(dc.h2d.Hook__Console.orig___constructor__ orig, dc.h2d.Console nextLevelVote, Font font, dc.h2d.Object parent)
        {
            orig(nextLevelVote, font, parent);
            StringMap commands = nextLevelVote.commands;
        }

        private void Hook_Console_log1(dc.ui.Hook_Console.orig_log orig, dc.ui.Console self,
            dc.String text, int? color)
        {
            Logger.Information(text.ToString() ?? "");
            var ct = (HashlinkObjectType)self.HashlinkObj.Type;
            ct.Super!.FindProto("log")!.Function.DynamicInvoke(self, text, color);
        }

        private void Hook_Console_handleCommand1(dc.h2d.Hook_Console.orig_handleCommand orig,
            dc.h2d.Console self, dc.String command)
        {
            Logger.Information("Handle Command: {cmd}", command);
            orig(self, command);
        }




        void IOnHeroUpdate.OnHeroUpdate(double dt)
        {
            if (Key.Class.isPressed(37))
            {
                LevelTransition.Class.@goto("BackGarden".AsHaxeString());
            }
            if (Key.Class.isPressed(38))
            {
                Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
                hero._level.fx.customMask(12231073, 0.35, 0.04, 0.35, 2, null);

            }
            if (Key.Class.isPressed(39))
            {
                dc.cine.LevelTransition.Class.@goto("PrisonRoof".AsHaxeString());
            }
            if (Key.Class.isPressed(40))
            {
                Debugmethod();
            }
        }


        public void Debugmethod()
        {


        }

        void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
        {

        }


    }




}

