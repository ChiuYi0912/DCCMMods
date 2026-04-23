using System;
using System.Collections.Generic;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en.mob;
using dc.h2d;
using dc.h2d.col;
using dc.hl.types;
using dc.hxd;
using dc.libs;
using dc.libs._Cooldown;
using dc.pr;
using dc.tool;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;


namespace MoreSettings.GameMechanics.Scarf
{
    public class CustomScarfUI : dc.ui.Process
    {
        public ControllerAccess controller = default!;
        public GetText getText = GetText.Instance;
        public dc.ui.Text title = null!;

        public Dictionary<FlowEnum, Flow> flows = new();
        public CustomScarfBase customScarf = new();

        private List<FlowBox> leftItems = new();
        private List<FlowBox> rightItems = new();


        private enum Side { Left, Right }
        private Side currentSide = Side.Left;
        private int currentIndex = 0;

        private ScaleGrid selectionHighlight = default!;
        public bool closeOnValidate = true;
        private Action<int, Side>? onItemSelected = default!;

        private Page NowPage = Page.NULL;

        public enum FlowEnum { MainFlow, RightFlow, LeftFlow }
        public enum Page { NULL, First, Scraf }

        public CustomScarfUI(dc.libs.Process parent) : base(parent)
        {
            controller = Boot.Class.ME.controller.createAccess("CustomScarfUI".ToHaxeString(), true);
            createRootInLayers(parent.root, Const.Class.ROOT_DP_MENU);

            title = new dc.ui.Text(null, false, true, Ref<double>.Null, null, null);
            title.set_text(getText.GetString("裁缝的爱人").ToHaxeString());
            root.addChild(title);

            HUD.Class.ME.hide(null);



            InitFlows(); OpenPageFirst(); CreateHighlight(); setMainFlowPos(); UpdateHighlightPosition();


        }

        #region UI 初始化

        private void InitFlows()
        {
            var main = flows[FlowEnum.MainFlow] = new Flow(root);
            main.set_verticalAlign(new FlowAlign.Middle());
            main.set_horizontalAlign(new FlowAlign.Middle());


            var right = flows[FlowEnum.RightFlow] = new Flow(main);
            right.set_horizontalAlign(new FlowAlign.Right());
            right.set_verticalAlign(new FlowAlign.Middle());
            right.isVertical = true;
            right.verticalSpacing = 50;

            var left = flows[FlowEnum.LeftFlow] = new Flow(main);
            left.set_horizontalAlign(new FlowAlign.Left());
            left.set_verticalAlign(new FlowAlign.Middle());
            left.isVertical = true;
            left.verticalSpacing = 50;
        }

        private void OpenPageFirst()
        {
            currentIndex = 0;
            NowPage = Page.First;
            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];

            const double PADH = 20;
            const double PADV = 20;
            FlowBox CreateFlowBox(dc.h2d.Object? parent = null)
            {
                var box = FlowBox.Class.createBoxValidationWithBiomeParam(parent, Ref<double>.In(PADH), Ref<double>.In(PADV));
                box.set_paddingLeft(50); box.set_paddingRight(50); box.set_horizontalSpacing(20); box.set_verticalSpacing(20);

                return box;
            }

            var leftbox = CreateFlowBox(leftFlow);
            var lefttext = new dc.ui.Text(leftbox, null, null, Ref<double>.Null, null, null);
            lefttext.set_text(GetText.Instance.GetString("创建新自定义飘带").ToHaxeString());
            leftItems.Add(leftbox);
            leftbox.interactive = new Interactive(leftbox.get_outerWidth(), leftbox.get_outerHeight(), null, null);
            leftbox.interactive.onClick = new HlAction<Event>(e =>
            {
                Clearpage(); OpenPageScraf(); onResize();
            });
            leftbox.interactive.onMove = new HlAction<Event>(e =>
            {

            });

            var rightbox = CreateFlowBox(rightFlow);
            var righttext = new dc.ui.Text(rightbox, null, null, Ref<double>.Null, null, null);
            righttext.set_text(getText.GetString("修改当前自定飘带").ToHaxeString());
            rightItems.Add(rightbox);
            rightbox.interactive = new Interactive(rightbox.get_outerWidth(), rightbox.get_outerHeight(), null, null);
            rightbox.interactive.onClick = new HlAction<Event>(e =>
            {

            });
            rightbox.interactive.onMove = new HlAction<Event>(e =>
            {

            });

        }


        private void OpenPageScraf()
        {
            currentIndex = 0;
            NowPage = Page.Scraf;
            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];

            const double PADH = 20;
            const double PADV = 5;
            FlowBox CreateFlowBox(dc.h2d.Object? parent = null)
            {
                var box = FlowBox.Class.createBoxValidationWithBiomeParam(parent, Ref<double>.In(PADH), Ref<double>.In(PADV));
                box.set_paddingLeft(50);
                box.set_paddingRight(50);
                box.set_horizontalSpacing(20);
                box.set_verticalSpacing(20);

                return box;
            }

            for (int i = 0; i < 7; i++)
            {
                var box = CreateFlowBox(leftFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                text.set_text($"Left Item {i + 1}".ToHaxeString());
                leftItems.Add(box);

                AddInteractiveToItem(text, Side.Left, i);
            }


            for (int i = 0; i < 7; i++)
            {
                var box = CreateFlowBox(rightFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                text.set_text($"Right Item {i + 1}".ToHaxeString());
                rightItems.Add(box);

                AddInteractiveToItem(text, Side.Right, i);
            }
        }


        private void AddInteractiveToItem(dc.ui.Text text, Side side, int idx)
        {
            var inter = new Interactive(text.textWidth, text.textHeight, text, null);
            inter.onClick = new HlAction<Event>(e =>
            {
                MoveFocusTo(side, idx);
                OnValidate();
            });
            inter.onMove = new HlAction<Event>(e =>
            {
                MoveFocusTo(side, idx);
            });
        }

        private void CreateHighlight()
        {
            var tile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);

            selectionHighlight = new ScaleGrid(tile, 8, 8, null);
            flows[FlowEnum.MainFlow].addChild(selectionHighlight);
            selectionHighlight.alpha = 0;
            UpdateHighlightPosition();
        }

        #endregion

        #region 焦点与高亮控制

        private void MoveFocusTo(Side side, int index)
        {

            var list = (side == Side.Left) ? leftItems : rightItems;
            if (index < 0) index = 0;
            if (index >= list.Count) index = list.Count - 1;
            if (list.Count == 0) return;

            currentSide = side;
            currentIndex = index;
            UpdateHighlightPosition();
            CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
        }

        private void MoveSelection(int dx, int dy)
        {
            if (dy != 0)
            {
                int newIndex = currentIndex + dy;
                var list = (currentSide == Side.Left) ? leftItems : rightItems;
                if (newIndex >= 0 && newIndex < list.Count)
                {
                    currentIndex = newIndex;
                    UpdateHighlightPosition();
                    CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                }
                else if (newIndex < 0 && currentSide == Side.Left && leftItems.Count > 0)
                {

                }
                else if (newIndex >= list.Count && currentSide == Side.Right && rightItems.Count > 0)
                {

                }
            }
            else if (dx != 0)
            {

                Side newSide = (dx > 0) ? Side.Right : Side.Left;
                if (newSide != currentSide)
                {
                    var newList = (newSide == Side.Left) ? leftItems : rightItems;
                    if (newList.Count > 0)
                    {
                        currentSide = newSide;
                        if (currentIndex >= newList.Count)
                            currentIndex = newList.Count - 1;
                        UpdateHighlightPosition();
                        CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                    }
                }
            }
        }

        private void UpdateHighlightPosition()
        {
            if (selectionHighlight == null) return;

            var target = GetCurrentSelectedItem();

            if (target == null) return;

            var main = flows[FlowEnum.MainFlow];
            main.reflow();
            target.reflow();

            double padding = get_pixelScale.Invoke() * 10.0;
            double targetW = target.get_outerWidth();
            double targetH = target.get_outerHeight();


            selectionHighlight?.set_width((int)(targetW + padding));
            selectionHighlight?.set_height((int)(targetH + padding));

            var bounds = target.getBounds(main, null);
            selectionHighlight?.x = bounds.xMin - padding / 2;
            selectionHighlight?.y = bounds.yMin - padding / 2;

            selectionHighlight?.posChanged = true;
        }
        private FlowBox GetCurrentSelectedItem()
        {
            var list = (currentSide == Side.Left) ? leftItems : rightItems;
            if (currentIndex >= 0 && currentIndex < list.Count)
                return list[currentIndex];
            return null!;
        }

        #endregion

        #region 输入处理

        public override void postUpdate()
        {
            if (selectionHighlight != null)
            {
                var target = GetCurrentSelectedItem();
                if (target == null)
                { selectionHighlight.alpha = 0; return; }
                double timeFactor = base.ftime * 0.1;
                string speedKey = "co_blinkCursorSpeed";
                var speedData = Data.Class.gui.byId.get(speedKey.ToHaxeString()).v0;

                var angle = timeFactor * speedData;
                var cosValue = System.Math.Cos(angle);
                var alphaOffset = 0.2 * cosValue;

                selectionHighlight.alpha = 0.8 + alphaOffset;
            }
            controlsUpdate();
            base.postUpdate();
        }

        public bool controlsUpdate()
        {
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 13)) // 右
            {
                MoveSelection(1, 0);
            }
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 11)) // 左
            {
                MoveSelection(-1, 0);
            }
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 10)) // 上
            {
                MoveSelection(0, -1);
            }
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 12)) // 下
            {
                MoveSelection(0, 1);
            }
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 14))
            {
                var item = GetCurrentSelectedItem();
                if (item?.interactive != null)
                {
                    var evt = new Event(new EventKind.EPush(), Ref<double>.In(0), Ref<double>.In(0));
                    item.interactive.onClick.Invoke(evt);
                }
                return true;
            }
            if (CoreLibrary.Utilities.ControllerHelper.ControlsUpdateFromProcess(controller.parent, 16))
            {
                if (NowPage == Page.Scraf)
                {
                    Clearpage(); OpenPageFirst(); onResize();
                    return true;
                }

                Close();
                return true;
            }


            return true;
        }






        #endregion

        #region 确认与关闭

        private void OnValidate()
        {
            var item = GetCurrentSelectedItem();
            if (item == null) return;
            CoreLibrary.Utilities.AudioHelper.LoadAudioFormString("sfx/ui/menu_select.wav");
            onItemSelected?.Invoke(currentIndex, currentSide);
            if (closeOnValidate)
                Close();
        }

        private void Close()
        {
            if (controller != null)
                controller.dispose(Ref<bool>.Null);
            controller = null!;

            dc.pr.Game.Class.ME.resume();
            destroyed = true;
        }



        #endregion

        #region 布局更新

        public override void onResize()
        {
            base.onResize();
            setMainFlowPos();
            UpdateHighlightPosition();



            title.posChanged = true;
        }

        public override void blur(Ref<double> radius, Ref<double> gain) { }
        public override void unblur() { }


        private void setMainFlowPos()
        {
            var main = flows[FlowEnum.MainFlow];

            int effectiveWidth = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_WIDTH
                : dc.hxd.Window.Class.getInstance().get_width();

            int effectiveHeight = dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT > 0
                ? dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT
                : dc.hxd.Window.Class.getInstance().get_height();

            main.set_minWidth(effectiveWidth);
            main.set_minHeight(effectiveHeight);
            main.x = 0;
            main.y = 0;
            main.posChanged = true;

            var left = flows[FlowEnum.LeftFlow];
            var right = flows[FlowEnum.RightFlow];

            left.reflow();
            right.reflow();

            FlowProperties leftProps = main.getProperties(left);
            leftProps.set_isAbsolute(true);

            FlowProperties rightProps = main.getProperties(right);
            rightProps.set_isAbsolute(true);

            double margin = 100 * get_pixelScale.Invoke();

            left.x = margin;
            left.y = (effectiveHeight - left.get_outerHeight()) / 2.0;

            right.x = effectiveWidth - right.get_outerWidth() - margin;
            right.y = (effectiveHeight - right.get_outerHeight()) / 2.0;

            double textwidth = title.textWidth * get_pixelScale.Invoke();
            title.x = (effectiveWidth - textwidth / 2) / 2.0;
            title.y = effectiveHeight * 0.05;

            left.posChanged = true;
            right.posChanged = true;
            title.posChanged = true;

            main.reflow();
        }

        #endregion

        #region 清理

        private void Clearpage()
        {
            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];



            foreach (var item in rightItems)
            {
                item.remove();
            }

            foreach (var item in leftItems)
            {
                item.remove();
            }

            leftFlow.removeChildren();
            rightFlow.removeChildren();

            leftItems.Clear();
            rightItems.Clear();

            leftFlow.posChanged = true;
            rightFlow.posChanged = true;

            // foreach (FlowBox left in leftFlow.children.AsEnumerable())
            // {
            //     left.remove();
            // }

            // foreach (FlowBox rigth in rightFlow.children.AsEnumerable())
            // {
            //     rigth.remove();
            // }
        }

        public override void onDispose()
        {
            base.onDispose();
            if (dc.pr.Game.Class.ME != null)
                dc.pr.Game.Class.ME.hud.show(null);
            if (controller != null)
                controller.dispose(Ref<bool>.Null);
            controller = null!;
        }

        #endregion
    }
}
