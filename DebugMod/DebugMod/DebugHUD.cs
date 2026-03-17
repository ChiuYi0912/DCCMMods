using dc;
using dc.h2d;
using dc.h3d;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = System.IO.File;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Core.Extensions;

namespace DebugMod
{
    public class DebugHUD : IEventReceiver
    {
        private readonly List<dc.h2d.Text> logTexts = new();
        private readonly List<string> lastLogContent = new();
        private DateTime lastFileModTime = DateTime.MinValue;
        private string lastLogFilePath = "";
        public dc.ui.DebugHud debug { get; set; } = null!;

        public double TextSize = DebugModMod.GetConfig.Value.LogTextSize;
        public int TextColor = CreateColor.ColorFromHex(DebugModMod.GetConfig.Value.LogTextColor);

        private static string[] ReadLogFileSafe(string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var lines = new List<string>();
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines.ToArray();
            }
            catch (IOException)
            {
                return Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static List<string> SplitTextForViewport(string text)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(text)) return result;

            int maxCharsPerLine = 100;

            if (Game.Class.ME != null && Game.Class.ME.curLevel != null && Game.Class.ME.curLevel.viewport != null)
            {
                var winwid = dc.hxd.Window.Class.getInstance().get_width();
                var viewport = Game.Class.ME.curLevel.viewport;
                double availableWidth = viewport.wid;
                double charWidth = 3.6;
                maxCharsPerLine = System.Math.Max(40, (int)(availableWidth / charWidth));
            }


            for (int i = 0; i < text.Length; i += maxCharsPerLine)
            {
                int length = System.Math.Min(maxCharsPerLine, text.Length - i);
                result.Add(text.Substring(i, length));
            }

            return result;
        }

        public DebugHUD()
        {
            EventSystem.AddReceiver(this);
            Hook_Game.init += Hook_Game_init;
            Hook_DebugHud.postUpdate += Hook_DebugHud_postUpdate;
            Hook__DebugHud.__constructor__ += Hook_DebugHud_initialize;
            Hook_DebugHud.initLogsUi += Hook_DebugHud_initlogui;
            Hook_Game.onDispose += hook_Game_onDispose;
        }

        private void hook_Game_onDispose(Hook_Game.orig_onDispose orig, Game self)
        {
            orig(self);
            debug.destroy();
        }

        private void Hook_Game_init(Hook_Game.orig_init orig, dc.pr.Game self)
        {
            orig(self);
            debug = new DebugHud();
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
            {
                string[] lines = ReadLogFileSafe(filePath);
                if (lines.Length > 0)
                    return string.Join("\n", lines);
                else
                    return "日志文件为空或无法读取";
            }
            else
                return "未获取到路径中的Log";
        }




        private void Hook_DebugHud_initlogui(Hook_DebugHud.orig_initLogsUi orig, DebugHud self)
        {
            if (self.logsFlow != null)
            {
                self.logsFlow.remove();
                self.logsFlow = null;
            }

            foreach (var text in logTexts)
            {
                text.remove();
            }
            logTexts.Clear();

            self.logsFlow = new Flow(self.root);

            self.logsFlow.set_minHeight(50);
            self.logsFlow.set_paddingLeft(10);
            self.logsFlow.set_paddingRight(10);
            self.logsFlow.set_paddingTop(5);
            self.logsFlow.set_paddingBottom(5);
            self.logsFlow.set_horizontalSpacing(25);
            self.logsFlow.set_horizontalAlign(new FlowAlign.Left());
            self.logsFlow.set_verticalAlign(new FlowAlign.Middle());
            self.logsFlow.isVertical = true;

            var bgTile = Tile.Class.fromColor(0, null, null, 0.4, null);
            self.logsFlow.set_backgroundTile(bgTile);
            self.logsFlow.set_visible(true);
            self.logsTexts = new EnumValueMap();

            string logFilePath = System.IO.Path.Combine(DebugModMod.GetConfig.Value.DebugUILogPATH, "log_latest.log");
            lastLogFilePath = logFilePath;
            if (File.Exists(logFilePath))
            {
                string[] lines = ReadLogFileSafe(logFilePath);
                if (lines.Length > 0)
                {
                    int startIndex = System.Math.Max(0, lines.Length - 10);
                    for (int i = startIndex; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        var splitLines = SplitTextForViewport(line);
                        foreach (var splitLine in splitLines)
                        {
                            var logText = new dc.h2d.Text(Assets.Class.font18, self.logsFlow);

                            logText.set_text(splitLine.AsHaxeString());
                            logText.scaleX = TextSize;
                            logText.scaleY = TextSize;
                            logText.set_textColor(TextColor);
                            logTexts.Add(logText);
                        }
                    }
                }
                else
                {
                    var infoLines = SplitTextForViewport("日志文件为空或无法读取");
                    foreach (var infoLine in infoLines)
                    {
                        var infoText = new dc.h2d.Text(Assets.Class.font18, self.logsFlow);

                        infoText.set_text(infoLine.AsHaxeString());
                        infoText.set_textColor(TextColor);
                        logTexts.Add(infoText);
                    }
                }
            }
            else
            {
                var errorLines = SplitTextForViewport("未找到日志文件");
                foreach (var errorLine in errorLines)
                {
                    var errorText = new dc.h2d.Text(Assets.Class.font18, self.logsFlow);
                    errorText.set_text(errorLine.AsHaxeString());
                    errorText.scaleX = TextSize;
                    errorText.scaleY = TextSize;
                    errorText.set_textColor(TextColor);
                    logTexts.Add(errorText);
                }
            }
            self.onResize();
        }

        private void UpdateLogTexts(DebugHud debugHud)
        {
            if (string.IsNullOrEmpty(lastLogFilePath)) return;

            if (!File.Exists(lastLogFilePath))
            {
                foreach (var text in logTexts)
                {
                    text.remove();
                }
                logTexts.Clear();

                var errorLines = SplitTextForViewport("未找到日志文件");
                foreach (var errorLine in errorLines)
                {
                    var errorText = new dc.h2d.Text(Assets.Class.font18, debugHud.logsFlow);
                    errorText.set_text(errorLine.AsHaxeString());
                    errorText.scaleX = TextSize;
                    errorText.scaleY = TextSize;
                    errorText.set_textColor(TextColor);
                    logTexts.Add(errorText);
                }
                return;
            }

            DateTime currentModTime;
            try
            {
                currentModTime = File.GetLastWriteTime(lastLogFilePath);
                if (currentModTime <= lastFileModTime && lastLogContent.Count > 0)
                    return;
            }
            catch
            {
                currentModTime = DateTime.MinValue;
            }

            string[] lines = ReadLogFileSafe(lastLogFilePath);

            if (lines.Length == 0)
            {
                if (lastLogContent.Count > 0)
                    return;

                foreach (var text in logTexts)
                {
                    text.remove();
                }
                logTexts.Clear();

                var infoLines = SplitTextForViewport("日志文件无法读取或为空");
                foreach (var infoLine in infoLines)
                {
                    var infoText = new dc.h2d.Text(Assets.Class.font18, debugHud.logsFlow);
                    infoText.set_text(infoLine.AsHaxeString());
                    infoText.scaleX = TextSize;
                    infoText.scaleY = TextSize;
                    infoText.set_textColor(TextColor);
                    logTexts.Add(infoText);
                }
                return;
            }

            int startIndex = System.Math.Max(0, lines.Length - 10);
            var currentContent = new List<string>();
            for (int i = startIndex; i < lines.Length; i++)
            {
                currentContent.Add(lines[i]);
            }

            if (lastLogContent.SequenceEqual(currentContent))
            {
                lastFileModTime = currentModTime;
                return;
            }

            lastLogContent.Clear();
            lastLogContent.AddRange(currentContent);
            lastFileModTime = currentModTime;

            var splitLines = new List<string>();
            foreach (var line in currentContent)
            {
                var splitLineList = SplitTextForViewport(line);
                splitLines.AddRange(splitLineList);
            }

            while (logTexts.Count < splitLines.Count)
            {
                var newText = new dc.h2d.Text(Assets.Class.font18, debugHud.logsFlow);
                newText.scaleX = TextSize;
                newText.scaleY = TextSize;
                newText.set_textColor(TextColor);
                logTexts.Add(newText);
            }

            while (logTexts.Count > splitLines.Count)
            {
                var text = logTexts[^1];
                text.remove();
                logTexts.RemoveAt(logTexts.Count - 1);
            }

            for (int i = 0; i < splitLines.Count; i++)
            {
                logTexts[i].set_text(splitLines[i].AsHaxeString());
            }
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
                UpdateLogTexts(self);
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