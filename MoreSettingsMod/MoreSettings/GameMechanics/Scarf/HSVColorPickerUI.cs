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
    public class ColorGridSelector : GridSelector
    {
        private List<uint> colorList = default!;
        private Action<int> onColorSelected;
        private const int COLOR_LEVELS = 8;

        public ColorGridSelector(Action<int> onColorSelectedCallback)
        {
            onColorSelected = onColorSelectedCallback;
        }



        private void GenerateFullColorPalette()
        {
            colorList = new List<uint>(COLOR_LEVELS * COLOR_LEVELS * COLOR_LEVELS);
            for (int r = 0; r < COLOR_LEVELS; r++)
            {
                byte red = (byte)((r * 255) / (COLOR_LEVELS - 1));
                for (int g = 0; g < COLOR_LEVELS; g++)
                {
                    byte green = (byte)((g * 255) / (COLOR_LEVELS - 1));
                    for (int b = 0; b < COLOR_LEVELS; b++)
                    {
                        byte blue = (byte)((b * 255) / (COLOR_LEVELS - 1));
                        uint color = 0xFF000000u | ((uint)red << 16) | ((uint)green << 8) | blue;
                        colorList.Add(color);
                    }
                }
            }
        }

        public override void initGrid()
        {
            GenerateFullColorPalette();
            curX = curY = 0;
            base.initEntries(colorList.Count);
        }

        public override int get_wid() => COLOR_LEVELS;
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