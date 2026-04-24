using System;
using System.Collections.Generic;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities;
using dc;
using dc.en;
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
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Utilities;
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
        private List<FlowBox> bottomItems = new();

        private ScaleGrid selectionHighlight = default!;
        public bool closeOnValidate = false;
        private Action<int, Side>? onItemSelected = default!;


        public enum FlowEnum { MainFlow, RightFlow, LeftFlow, BottomFlow }
        public enum Page { NULL, First, Scraf, Props }
        private enum Side { Left, Right, Bottom }

        private Page NowPage = Page.NULL;
        private Side currentSide = Side.Left;
        private Side lastSide = Side.Left;

        private int currentIndex = 0;
        private int bottomIndex = 0;
        private int CurrentScarf = 0;

        private Dictionary<string, (Action<ScarfData, object> setter, Func<ScarfData, object> getter)> propertyAccessors = new();
        private Dictionary<int, ScarfListInitialisation> Attributes = new();




        public CustomScarfUI(dc.libs.Process parent) : base(parent)
        {
            controller = Boot.Class.ME.controller.createAccess("CustomScarfUI".ToHaxeString(), true);
            createRootInLayers(parent.root, Const.Class.ROOT_DP_MENU); setControlLabel();

            title = new dc.ui.Text(null, false, true, Ref<double>.Null, null, null);
            title.set_text(getText.GetString("裁缝的爱人").ToHaxeString());
            root.addChild(title);

            HUD.Class.ME.hide(null);
            CeateScarfToKey(CurrentScarf);

            InitFlows(); OpenPageFirst(); CreateHighlight(); setMainFlowPos(); UpdateHighlightPosition();
            Hook_Hero.initScarf += Hook_Hero_initScarf;
        }

        private void Hook_Hero_initScarf(Hook_Hero.orig_initScarf orig, Hero self)
        {
            if (self.scarf != null)
                self.scarf.dispose();
            self.scarf = customScarf.CreateCustomScarfManager(self);
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

            var bottom = flows[FlowEnum.BottomFlow] = new Flow(main);
            bottom.set_horizontalAlign(new FlowAlign.Middle());
            bottom.set_verticalAlign(new FlowAlign.Bottom());
            bottom.horizontalSpacing = 50;
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
            lefttext.set_text(GetText.Instance.GetString("修改当前自定义飘带").ToHaxeString());
            leftItems.Add(leftbox);
            leftbox.interactive = new dc.h2d.Interactive(leftbox.get_outerWidth(), leftbox.get_outerHeight(), null, null);
            leftbox.interactive.onClick = new HlAction<Event>(e =>
            {

            });
            leftbox.interactive.onMove = new HlAction<Event>(e =>
            {

            });

            var rightbox = CreateFlowBox(rightFlow);
            var righttext = new dc.ui.Text(rightbox, null, null, Ref<double>.Null, null, null);
            righttext.set_text(getText.GetString("创建新自定义飘带").ToHaxeString());
            rightItems.Add(rightbox);
            rightbox.interactive = new dc.h2d.Interactive(rightbox.get_outerWidth(), rightbox.get_outerHeight(), null, null);
            rightbox.interactive.onClick = new HlAction<Event>(e =>
            {
                if (!Attributes.ContainsKey(0))
                {
                    var newData = new ScarfListInitialisation();
                    newData.InitAttributes();
                    Attributes[CurrentScarf] = newData;
                }
                Clearpage(); OpenPageScraf(); onResize();
            });
            rightbox.interactive.onMove = new HlAction<Event>(e =>
            {

            });

        }


        private void OpenPageScraf()
        {
            NowPage = Page.Scraf;

            //int newId = GetNextScarfId();
            var currentEditingData = Attributes[CurrentScarf];

            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];
            var bottomFlow = flows[FlowEnum.BottomFlow];


            for (int i = 0; i < Attributes[CurrentScarf].baseAttributes.left.Count; i++)
            {
                var attr = Attributes[CurrentScarf].baseAttributes.left[i];
                var box = CreateFlowBox(leftFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                object val = attr.Getter(currentEditingData.scarfData);
                text.set_text($"{attr.Name}: {val}".ToHaxeString());
                leftItems.Add(box);
                attr.Box = box;
                attr.Text = text;
                AddInteractiveToItem(box, Side.Left, i);
            }


            for (int i = 0; i < Attributes[CurrentScarf].baseAttributes.right.Count - 1; i++)
            {
                var attr = Attributes[CurrentScarf].baseAttributes.right[i];
                var box = CreateFlowBox(rightFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                object val = attr.Getter(currentEditingData.scarfData);
                text.set_text($"{attr.Name}: {val}".ToHaxeString());
                rightItems.Add(box);
                attr.Box = box;
                attr.Text = text;
                AddInteractiveToItem(box, Side.Right, i);
            }

            int idx = Attributes[CurrentScarf].baseAttributes.right.Count - 1;
            var propsattr = Attributes[CurrentScarf].baseAttributes.right[idx];
            var propsbox = CreateFlowBox(rightFlow);
            var propstext = new dc.ui.Text(propsbox, null, null, Ref<double>.Null, null, null);
            object propsval = propsattr.Getter(currentEditingData.scarfData);
            propstext.set_text($"{propsattr.Name}: {propsval}".ToHaxeString());
            rightItems.Add(propsbox);
            propsattr.Box = propsbox;
            propsattr.Text = propstext;
            propsbox.interactive = new dc.h2d.Interactive(propsbox.get_outerWidth(), propsbox.get_outerHeight(), null, null);
            propsbox.interactive.onClick = new HlAction<Event>(e =>
            {
                MoveFocusTo(Side.Right, idx);
                AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                Clearpage();
                OpenPropsSubMenu();
                onResize();
            });
            propsbox.interactive.onMove = new HlAction<Event>(e =>
            {
                MoveFocusTo(Side.Right, idx);
            });



            var saveBox = CreateFlowBox(bottomFlow);
            var saveText = new dc.ui.Text(saveBox, null, null, Ref<double>.Null, null, null);
            saveText.set_text("保存".ToHaxeString());
            bottomItems.Add(saveBox);
            saveBox.interactive = new dc.h2d.Interactive(saveBox.get_outerWidth(), saveBox.get_outerHeight(), null, null);
            saveBox.interactive.onClick = new HlAction<Event>(e => { });
            saveBox.interactive.onMove = new HlAction<Event>(e => { });
        }


        private void OpenPropsSubMenu()
        {
            NowPage = Page.Props;

            //int newId = GetNextScarfId();

            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];
            var bottomFlow = flows[FlowEnum.BottomFlow];

            var currentEditingData = Attributes[CurrentScarf];


            for (int i = 0; i < Attributes[CurrentScarf].propsAttributes.left.Count; i++)
            {
                var attr = Attributes[CurrentScarf].propsAttributes.left[i];
                var box = CreateFlowBox(leftFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                object val = attr.Getter(currentEditingData.scarfData);
                text.set_text($"{attr.Name}: {val}".ToHaxeString());
                leftItems.Add(box);
                attr.Box = box;
                attr.Text = text;
                AddInteractiveToItem(box, Side.Left, i);
            }


            for (int i = 0; i < Attributes[CurrentScarf].propsAttributes.right.Count; i++)
            {
                var attr = Attributes[CurrentScarf].propsAttributes.right[i];
                var box = CreateFlowBox(rightFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                object val = attr.Getter(currentEditingData.scarfData);
                text.set_text($"{attr.Name}: {val}".ToHaxeString());
                rightItems.Add(box);
                attr.Box = box;
                attr.Text = text;
                AddInteractiveToItem(box, Side.Right, i);
            }


            var saveBox = CreateFlowBox(bottomFlow);
            var saveText = new dc.ui.Text(saveBox, null, null, Ref<double>.Null, null, null);
            saveText.set_text("保存".ToHaxeString());
            bottomItems.Add(saveBox);
            saveBox.interactive = new dc.h2d.Interactive(saveBox.get_outerWidth(), saveBox.get_outerHeight(), null, null);
            saveBox.interactive.onClick = new HlAction<Event>(e => { });
            saveBox.interactive.onMove = new HlAction<Event>(e => { });
        }


        private FlowBox CreateFlowBox(dc.h2d.Object parent)
        {
            const double PADH = 20;
            const double PADV = 5;
            var box = FlowBox.Class.createBoxValidationWithBiomeParam(parent, Ref<double>.In(PADH), Ref<double>.In(PADV));
            box.set_paddingLeft(50);
            box.set_paddingRight(50);
            box.set_horizontalSpacing(20);
            box.set_verticalSpacing(20);

            return box;
        }



        private void AddInteractiveToItem(FlowBox box, Side side, int idx)
        {
            box.interactive = new dc.h2d.Interactive(box.get_outerWidth(), box.get_outerHeight(), null, null);
            box.interactive.onClick = new HlAction<Event>(e =>
            {
                MoveFocusTo(side, idx);
                OnValidate();
            });
            box.interactive.onMove = new HlAction<Event>(e =>
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

        #region 飘带配置
        private ScarfListInitialisation CeateScarfToKey(int key)
        {
            if (Attributes.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var newData = new ScarfListInitialisation();
            if (customScarf.Datakey.TryGetValue(key, out var existingData))
            {
                newData.scarfData = existingData;
                newData.InitAttributes();
            }
            else
            {
                newData.InitAttributes();
                customScarf.Datakey[key] = newData.scarfData;
                customScarf.Save();
            }

            Attributes[key] = newData;
            return newData;
        }

        private ScarfListInitialisation SafeGetScarf(int key)
        {
            if (Attributes.TryGetValue(key, out var scarf))
            {
                return scarf;
            }
            return null!;
        }

        private void RemoveScarf(int key)
        {
            Attributes.Remove(key);
        }
        #endregion

        #region 焦点与高亮控制

        private void MoveFocusTo(Side side, int index)
        {
            if (side == Side.Bottom)
            {
                if (index < 0) index = 0;
                if (index >= bottomItems.Count) index = bottomItems.Count - 1;
                if (bottomItems.Count == 0) return;
                currentSide = Side.Bottom;
                bottomIndex = index;
                UpdateHighlightPosition();
                AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                return;
            }

            var list = (side == Side.Left) ? leftItems : rightItems;
            if (index < 0) index = 0;
            if (index >= list.Count) index = list.Count - 1;
            if (list.Count == 0) return;

            currentSide = side;
            currentIndex = index;
            UpdateHighlightPosition();
            AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
        }

        private void MoveSelection(int dx, int dy)
        {
            if (currentSide == Side.Bottom)
            {
                if (dx != 0)
                {
                    int newIndex = bottomIndex + dx;
                    if (newIndex >= 0 && newIndex < bottomItems.Count)
                    {
                        bottomIndex = newIndex;
                        UpdateHighlightPosition();
                        AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                    }
                }
                else if (dy < 0)
                {
                    currentSide = lastSide;
                    var list = (currentSide == Side.Left) ? leftItems : rightItems;
                    if (currentIndex >= list.Count) currentIndex = list.Count - 1;
                    if (currentIndex < 0) currentIndex = 0;
                    UpdateHighlightPosition();
                    AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                }
            }
            else
            {
                if (dy != 0)
                {
                    int newIndex = currentIndex + dy;
                    var list = (currentSide == Side.Left) ? leftItems : rightItems;
                    if (newIndex >= 0 && newIndex < list.Count)
                    {
                        currentIndex = newIndex;
                        UpdateHighlightPosition();
                        AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                    }
                    else if (dy > 0 && newIndex >= list.Count && bottomItems.Count > 0)
                    {
                        lastSide = currentSide;
                        currentSide = Side.Bottom;
                        bottomIndex = 0;
                        UpdateHighlightPosition();
                        AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
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
                            AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                        }
                    }
                }
            }
        }

        private void UpdateHighlightPosition()
        {
            if (selectionHighlight == null) return;
            FlowBox target = null!;
            if (currentSide == Side.Bottom && bottomIndex < bottomItems.Count)
                target = bottomItems[bottomIndex];
            else
                target = GetCurrentSelectedItem();

            if (target == null) { selectionHighlight.alpha = 0; return; }

            var main = flows[FlowEnum.MainFlow];
            main.reflow();
            target.reflow();

            double padding = get_pixelScale.Invoke() * 10.0;
            double targetW = target.get_outerWidth();
            double targetH = target.get_outerHeight();

            selectionHighlight.set_width((int)(targetW + padding));
            selectionHighlight.set_height((int)(targetH + padding));

            var bounds = target.getBounds(main, null);
            selectionHighlight.x = bounds.xMin - padding / 2;
            selectionHighlight.y = bounds.yMin - padding / 2;
            selectionHighlight.posChanged = true;
        }
        private FlowBox GetCurrentSelectedItem()
        {
            var list = (currentSide == Side.Left) ? leftItems : rightItems;
            if (currentIndex >= 0 && currentIndex < list.Count)
                return list[currentIndex];
            return null!;
        }

        private FlowBox GetCurrentBottomItem()
        {
            if (bottomIndex >= 0 && bottomIndex < bottomItems.Count)
                return bottomItems[bottomIndex];
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

        public void setControlLabel()
        {
            virtual_acts_cond_label_onAdd_ creataBCreateButton(int actionId, string labelText)
            {
                ArrayBytes_Int acts = ArrayUtils.CreateInt();
                acts.push(actionId);
                var buttonConfig = new virtual_acts_cond_label_
                {
                    acts = acts,
                    label = Lang.Class.t.get(labelText.ToHaxeString(), null),
                    cond = null
                };
                return buttonConfig.ToVirtual<virtual_acts_cond_label_onAdd_>();
            }

            ArrayObj btns = (ArrayObj)ArrayUtils.CreateDyn().array;
            btns.push(creataBCreateButton(14, "Valider"));
            btns.push(creataBCreateButton(16, "Retour"));
            btns.push(creataBCreateButton(17, "+"));
            btns.push(creataBCreateButton(18, "-"));

            base.createControlLabel(btns);
        }


        public void controlsUpdate()
        {
            if (controller.manualLock)
                return;

            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 13)) // 右
            {
                MoveSelection(1, 0);
            }
            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 11)) // 左
            {
                MoveSelection(-1, 0);
            }
            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 10)) // 上
            {
                MoveSelection(0, -1);
            }
            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 12)) // 下
            {
                MoveSelection(0, 1);
            }
            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 14))
            {
                if (currentSide == Side.Bottom)
                {
                    var bottomItem = GetCurrentBottomItem();
                    if (bottomItem?.interactive != null)
                    {
                        var evt = new Event(new EventKind.EPush(), Ref<double>.In(0), Ref<double>.In(0));
                        bottomItem.interactive.onClick.Invoke(evt);
                    }
                }
                else
                {
                    var item = GetCurrentSelectedItem();
                    if (item?.interactive != null)
                    {
                        var evt = new Event(new EventKind.EPush(), Ref<double>.In(0), Ref<double>.In(0));
                        item.interactive.onClick.Invoke(evt);
                    }
                }
                return;
            }

            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 17))
            {
                ModifyCurrentAttributeValue(1);
                return;
            }
            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 18))
            {
                ModifyCurrentAttributeValue(-1);
                return;
            }

            if (ControllerHelper.ControlsUpdateFromProcess(controller.parent, 16))
            {
                if (NowPage == Page.Scraf)
                {
                    Clearpage(); OpenPageFirst(); onResize();
                    return;
                }
                if (NowPage == Page.Props)
                {
                    Clearpage(); OpenPageScraf(); onResize();
                    return;
                }

                Close();
                return;
            }

            return;
        }

        private void ModifyCurrentAttributeValue(int delta)
        {
            if (currentSide == Side.Bottom) return;

            List<AttributeEntry> attrList;
            ScarfData currentData;

            if (NowPage == Page.Scraf)
            {
                attrList = (currentSide == Side.Left)
                    ? Attributes[CurrentScarf].baseAttributes.left
                    : Attributes[CurrentScarf].baseAttributes.right;
                currentData = Attributes[CurrentScarf].scarfData;
            }
            else if (NowPage == Page.Props)
            {
                attrList = (currentSide == Side.Left)
                    ? Attributes[CurrentScarf].propsAttributes.left
                    : Attributes[CurrentScarf].propsAttributes.right;
                currentData = Attributes[CurrentScarf].scarfData;
            }
            else
            {
                return;
            }

            if (currentIndex < 0 || currentIndex >= attrList.Count) return;
            var attr = attrList[currentIndex];

            object oldVal = attr.Getter(currentData);
            object newVal;

            if (attr.ValueType == typeof(double))
            {
                double step = (delta > 0) ? 0.5 : -0.5;
                newVal = (double)oldVal + step;
            }
            else if (attr.ValueType == typeof(int))
            {
                newVal = (int)oldVal + delta;
            }
            else if (attr.ValueType == typeof(int?))
            {
                if (attr.Name.ToLowerInvariant().Contains("color") || attr.Name.ToLowerInvariant().Contains("backColor"))
                {
                    controller.manualLock = true;
                    Hideandshow(false);

                    var picker = new ColorGridSelector((selectedColor) =>
                    {
                        attr.Setter(currentData, selectedColor);
                        attr.Text.set_text($"{attr.Name}: {selectedColor:X8}".ToHaxeString());
                        setMainFlowPos();
                        UpdateHighlightPosition();
                        AudioHelper.LoadAudioFormString("sfx/ui/menu_select.wav");
                    });
                    picker.onClose = new HlAction(() =>
                    {
                        delayer.addF(null, new HlAction(() =>
                        {
                            controller.manualLock = false;
                            Hideandshow(true);
                            setMainFlowPos();
                            UpdateHighlightPosition();
                        }), 1.0);
                    });
                    return;
                }
                else
                {
                    int? val = (int?)oldVal;
                    if (val == null) val = 0;
                    newVal = val + delta;
                }
            }
            else if (attr.ValueType == typeof(bool))
            {
                newVal = !(bool)oldVal;
            }
            else if (attr.ValueType == typeof(dc.String))
            {
                return;
            }
            else
            {
                return;
            }

            attr.Setter(currentData, newVal);
            attr.Text.set_text($"{attr.Name}: {newVal}".ToHaxeString());
            setMainFlowPos();
            UpdateHighlightPosition();
            AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
            customScarf.Save();
            customScarf.UpdateSarfs();
            
        }


        private void Hideandshow(bool isshow)
        {
            root.visible = isshow;
            root.posChanged = true;
        }



        #endregion

        #region 确认与关闭

        private void OnValidate()
        {
            var item = GetCurrentSelectedItem();
            if (item == null) return;
            AudioHelper.LoadAudioFormString("sfx/ui/menu_select.wav");
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
            var bottom = flows[FlowEnum.BottomFlow];

            left.reflow();
            right.reflow();
            bottom.reflow();

            FlowProperties leftProps = main.getProperties(left);
            leftProps.set_isAbsolute(true);

            FlowProperties rightProps = main.getProperties(right);
            rightProps.set_isAbsolute(true);

            FlowProperties bottomProps = main.getProperties(bottom);
            bottomProps.set_isAbsolute(true);

            double margin = 100 * get_pixelScale.Invoke();

            left.x = margin;
            left.y = (effectiveHeight - left.get_outerHeight()) / 2.0;

            right.x = effectiveWidth - right.get_outerWidth() - margin;
            right.y = (effectiveHeight - right.get_outerHeight()) / 2.0;

            bottom.x = (effectiveWidth - bottom.get_outerWidth()) / 2.0;
            bottom.y = effectiveHeight - bottom.get_outerHeight() - (margin / 2);

            double textwidth = title.textWidth * get_pixelScale.Invoke();
            title.x = (effectiveWidth - textwidth / 2) / 2.0;
            title.y = effectiveHeight * 0.05;

            left.posChanged = true;
            right.posChanged = true;
            bottom.posChanged = true;
            title.posChanged = true;

            main.reflow();
        }

        #endregion

        #region 清理

        private void Clearpage()
        {
            var leftFlow = flows[FlowEnum.LeftFlow];
            var rightFlow = flows[FlowEnum.RightFlow];
            var bottom = flows[FlowEnum.BottomFlow];

            leftFlow.removeChildren();
            rightFlow.removeChildren();
            bottom.removeChildren();

            leftItems.Clear();
            rightItems.Clear();
            bottomItems.Clear();

            leftFlow.posChanged = true;
            rightFlow.posChanged = true;
            bottom.posChanged = true;

            currentSide = Side.Left;
            lastSide = Side.Left;
            currentIndex = 0;
            bottomIndex = 0;
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
