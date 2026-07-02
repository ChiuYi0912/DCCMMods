using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.h2d;
using dc.libs;
using dc.libs.misc;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace MoreSettings.GameMechanics
{
    public class HUDPro : dc.ui.Process
    {
        public readonly HUD? hud;

        public readonly FlowBox mainbox;
        public HUDPro(dc.libs.Process parent, HUD hud) : base(parent)
        {
            this.hud = hud;


            createRootInLayers(HUD.Class.ME.root, dc.Const.Class.ROOT_DP_MENU);

            int ScreenW = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                : dc.hxd.Window.Class.getInstance().get_width();
            int ScreenH = dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT
                : dc.hxd.Window.Class.getInstance().get_height();

            double pixe = get_pixelScale.Invoke();

            mainbox = CreateBoxLegendaryOutline(root);
            mainbox.set_isVertical(true);
            mainbox.set_minWidth(ScreenW / 4);
            mainbox.set_minHeight((int?)(pixe * 50));


            int tagety = (int)(ScreenH * 0.05);

            mainbox.x = (double)(ScreenW / 2 - mainbox.minWidth / 2)!;
            mainbox.reflow();
            tw.create_(
                () => mainbox.y,
                (v) => { mainbox.y = v; mainbox.posChanged = true; },
                0 - mainbox.minHeight,
                tagety,
                new TType.TEaseOut(),
                100 * 5, default
            );


        }

        public override void postUpdate()
        {
            base.postUpdate();
        }

        public override void onResize()
        {
            base.onResize();
            int ScreenW = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                : dc.hxd.Window.Class.getInstance().get_width();
            int ScreenH = dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT
                : dc.hxd.Window.Class.getInstance().get_height();

            double pixe = get_pixelScale.Invoke();

            if (mainbox != null)
            {
                mainbox.set_isVertical(true);
                mainbox.set_minWidth(ScreenW / 4);
                mainbox.set_minHeight((int?)(pixe * 50));

                mainbox.x = (double)(ScreenW / 2 - mainbox.minWidth / 2)!;
                mainbox.y = (int)(ScreenH * 0.05);

                mainbox.reflow();
            }

        }


        public FlowBox CreateBoxLegendaryOutline(dc.h2d.Object? p, bool boxspr = true)
        {
            FlowBox flowBox = FlowBox.Class.createBoxValidation(p, default, default, Ref<bool>.In(true), null);

            var gui = Data.Class.gui.byId;
            var blue = gui.get("co_defaultOpacityBlue".AsHaxeString());
            var black = gui.get("co_defaultOpacityBlack".AsHaxeString());
            flowBox.box.alpha = blue != null ? ((HaxeProxyBase)blue).ToVirtual<virtual_biome_color_comment_id_v0_>().v0 ?? 0.8 : 0.8;
            flowBox.blackBG.alpha = black != null ? ((HaxeProxyBase)black).ToVirtual<virtual_biome_color_comment_id_v0_>().v0 ?? 0.5 : 0.5;
            flowBox.box.bgDuo.alpha = 0.5;

            flowBox.box.sg.onParentChanged();

            var mask = new Mask((int)flowBox.realMaxWidth, (int)flowBox.realMaxHeight, flowBox);
            flowBox.getProperties(mask).set_isAbsolute(true);


            return flowBox;
        }
    }
}