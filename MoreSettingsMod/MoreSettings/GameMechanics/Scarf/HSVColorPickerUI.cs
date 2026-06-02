using System;
using System.Collections.Generic;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.h2d;
using dc.h2d.col;
using dc.hxd;
using dc.pr;
using dc.tool;
using dc.ui;
using dc.ui.sel;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;

namespace MoreSettings.GameMechanics.Scarf
{
    internal class ColorGridSelector : GridSelector
    {
        private List<uint> colorList = default!;
        private Action<int> onColorSelected;
        private const int GRID_COLS = 8;
        private const int GRID_ROWS = 4;

        public ColorGridSelector(Action<int> onColorSelectedCallback)
        {
            onColorSelected = onColorSelectedCallback;
        }

        private void GenerateCommonColorPalette()
        {
            colorList = new List<uint>
            {
                // 红色系
                0xFFFF0000u, // 纯红
                0xFFDC143Cu, // 深红
                0xFFC71585u, // 胭脂红
                // 橙色/黄色系
                0xFFFFA500u, // 橙色
                0xFFFFD700u, // 金色
                0xFFFFFF00u, // 纯黄
                // 绿色系
                0xFFADFF2Fu, // 黄绿
                0xFF00FF00u, // 纯绿
                0xFF008000u, // 深绿
                0xFF3CB371u, // 海洋绿
                // 青色/蓝色系
                0xFF00FFFFu, // 青色
                0xFF00BFFFu, // 深天蓝
                0xFF0000FFu, // 纯蓝
                0xFF000080u, // 海军蓝
                0xFF4B0082u, // 靛蓝
                // 紫色/粉色系
                0xFF8A2BE2u, // 蓝紫色
                0xFF800080u, // 紫色
                0xFFEE82EEu, // 紫罗兰
                0xFFFF00FFu, // 品红
                0xFFFFC0CBu, // 粉色
                0xFFFF69B4u, // 亮粉
                // 棕色/土色系
                0xFFA0522Du, // 赭色
                0xFF8B4513u, // 棕色
                0xFFD2691Eu, // 巧克力色
                // 灰色/无彩色系
                0xFF000000u, // 黑色
                0xFF2F4F4Fu, // 暗瓦灰
                0xFF808080u, // 灰色
                0xFFA9A9A9u, // 深灰色
                0xFFC0C0C0u, // 银色
                0xFFE0E0E0u, // 浅灰色
                0xFFF5F5F5u, // 烟熏白
                0xFFFFFFFFu  // 白色
            };

            int expectedCount = GRID_COLS * GRID_ROWS;
            if (colorList.Count < expectedCount)
            {
                uint white = 0xFFFFFFFFu;
                while (colorList.Count < expectedCount)
                    colorList.Add(white);
            }
            else if (colorList.Count > expectedCount)
            {
                colorList.RemoveRange(expectedCount, colorList.Count - expectedCount);
            }
        }

        public override void initGrid()
        {
            GenerateCommonColorPalette();
            curX = curY = 0;
            base.initEntries(colorList.Count);
        }

        public override int get_wid() => GRID_COLS;

        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;

        public override bool isEntryLocked(int i) => false;

        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            var tile = Tile.Class.fromColor((int)colorList[i], get_entryWid(), get_entryHei(), null, null);
            return new Bitmap(tile, parent);
        }

        public override dc.String getTitleText() => "选择颜色".ToHaxeString();

        public override void onValidate()
        {
            int index = curY * get_wid() + curX;
            if (index < colorList.Count)
            {
                int selectedColor = (int)colorList[index];
                CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                onColorSelected?.Invoke(selectedColor);
                close();
            }
            else
            {
                CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_error2.wav");
            }
        }

        public override void onDispose()
        {
            if (entries != null && entries.length > 0)
            {
                for (int i = 0; i < entries.length; i++)
                {
                    var entry = entries.getDyn(i) as virtual_cx_cy_f_i_isLocked_sectionIdx_;
                    if (entry?.f != null)
                    {
                        foreach (var child in entry.f.children.AsEnumerable())
                        {
                            if (child is Bitmap bmp)
                            {
                                bmp.remove();
                            }
                        }
                        entry.f.removeChildren();
                    }
                }
            }
            Process.Class.ALL.remove(this);
            if (this.controller == null)
                return;
            this.controller.dispose(Ref<bool>.Null);
        }
    }
}