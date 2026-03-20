using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.hxd;
using dc.hxd.res;
using dc.hxd.snd;
using dc.libs;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using dc.ui;
using EnemiesVsEnemies.UI.Utilities;
using HaxeProxy.Runtime;
using Text = dc.ui.Text;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui(dc.libs.Process parent) : dc.ui.Process(parent)
    {
        protected ControllerAccess controller = null!;
        protected Pause pauseUI = null!;
        protected int selectedIndex = 0;
        protected HSprite selectionSprite = null!;
        protected BG bg = null!;
        protected Flow mainFlow = null!;
        protected Text pressText = null!;
        protected UIHelper GetIHelper = null!;


        protected int curWidgetId = 0;
        protected List<OptionWidget> widgets = new ();

        public CricketSelectorGui(dc.libs.Process parent, Pause pause) : this(parent)
        {
            pauseUI = pause;
            if (pauseUI != null)
                parent = pauseUI;
            else
                parent = Main.Class.ME;


            int layerIndex = Const.Class.ROOT_DP_MENU;
            createRootInLayers(Main.Class.ME.root, layerIndex);


            if (Game.Class.ME != null)
            {
                Game.Class.ME.hud.hide(null);
                Game.Class.ME.modalPause(Ref<bool>.Null);
            }

            mainFlow = new Flow(null);
            root.addChildAt(mainFlow, 2);

            bg = new BG(this, Ref<bool>.Null, Ref<bool>.Null);
            root.addChildAt(bg, 0);


            controller = Boot.Class.ME.controller.createAccess("EVE".ToHaxeString(), true);
            GetIHelper = new UIHelper(controller);


            selectionSprite = new HSprite(Assets.Class.ui, "selectLeftRight".ToHaxeString(), Ref<int>.In(0), null!);
            selectionSprite.alpha = 0.0;
            root.addChildAt(selectionSprite, 1);

            dc.String pressTextStr = Lang.Class.t.get("info:CricketSelectorGui TEST".ToHaxeString(), null);
            pressText = new Text(null, true, null, Ref<double>.Null, null, null);
            pressText.set_text(pressTextStr);
            pressText.set_visible(true);
            root.addChildAt(pressText, 5);
        }

        public override void update()
        {
            base.update();

            if (GetIHelper == null) return;
            if (cd.fastCheck.exists(285212672)) return;

            if (GetIHelper.IsDown(10)) MoveSelection(-1); // 上
            if (GetIHelper.IsDown(12)) MoveSelection(1);  // 下


            if (GetIHelper.IsPressed(14)) OnValidate();   // 确认键
            if (GetIHelper.IsPressed(16)) OnQuit();        // 返回键(Esc)
        }

        protected virtual void OnValidate()
        {
            if (curWidgetId >= 0 && curWidgetId < widgets.Count)
            {
                widgets[curWidgetId].onValidate?.Invoke();
            }
        }

        protected virtual void OnQuit()
        {
            if (pauseUI != null)
                pauseUI.onLeavingOptionsMenu();

            this.destroy();
        }

        protected void MoveSelection(int delta)
        {
            if (widgets.Count == 0) return;
            curWidgetId = (curWidgetId + delta + widgets.Count) % widgets.Count;
        }
    }
}