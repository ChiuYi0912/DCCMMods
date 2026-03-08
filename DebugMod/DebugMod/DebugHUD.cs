using dc;
using dc.h2d;
using dc.haxe.ds;
using dc.level;
using dc.libs.heaps;
using dc.light;
using dc.pr;
using dc.tool;
using dc.tool.log;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Utilities;
using File = System.IO.File;

namespace DebugMod
{
    public class DebugHUD : IEventReceiver
    {

        public DebugHUD()
        {
            EventSystem.AddReceiver(this);
            Hook_Game.init += Hook_Game_init;
            Hook_DebugHud.postUpdate += Hook_DebugHud_postUpdate;
            Hook__DebugHud.__constructor__ += Hook_DebugHud_initialize;
            Hook_DebugHud.initLogsUi += Hook_DebugHud_initlogui;
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

        public string GetLatestLog()
        {
            string filePath = System.IO.Path.Combine(DebugModMod.GetConfig.Value.DebugUILogPATH, "log_latest.log");
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            else
                return "未获取到路径中的Log";
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
    }
}