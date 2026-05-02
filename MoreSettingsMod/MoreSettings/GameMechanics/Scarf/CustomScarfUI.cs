using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
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
using dc.libs.misc;
using dc.pr;
using dc.tool;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Utilities;
using MoreSettings.Utilities;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;


namespace MoreSettings.GameMechanics.Scarf
{
    public class CustomScarfUI : dc.ui.Process
    {
        public ControllerAccess controller = default!;
        public GetText getText = GetText.Instance;
        public dc.ui.Text title = null!;

        public Dictionary<FlowEnum, Flow> flows = new();
        public static CustomScarfBase customScarf = new();


        private List<FlowBox> leftItems = new();
        private List<FlowBox> rightItems = new();
        private List<FlowBox> bottomItems = new();

        private ScaleGrid selectionHighlight = default!;
        private Graphics persistentHighlight = default!;
        public bool closeOnValidate = false;
        private bool isAnimating = false;

        private Action<int, Side>? onItemSelected = default!;
        private Action Superaction = default!;


        public enum FlowEnum { MainFlow, RightFlow, LeftFlow, BottomFlow,InfoFlow }
        public enum Page { NULL, First, Scraf, Props }
        private enum Side { Left, Right, Bottom }

        private Page NowPage = Page.NULL;
        private Side currentSide = Side.Left;
        private Side lastSide = Side.Left;

        private int currentIndex = 0;
        private int bottomIndex = 0;
        private int CurrentScarf = 0;

        private Dictionary<string, (Action<ScarfData, object> setter, Func<ScarfData, object> getter)> propertyAccessors = new();
        private static Dictionary<int, ScarfListInitialisation> Attributes = new();




        public CustomScarfUI(dc.libs.Process parent) : base(parent)
        {
            controller = Boot.Class.ME.controller.createAccess("CustomScarfUI".ToHaxeString(), true);
            createRootInLayers(parent.root, Const.Class.ROOT_DP_MENU); setControlLabel();

            title = new dc.ui.Text(null, false, true, Ref<double>.Null, null, null);
            title.set_text(getText.GetString("裁缝的爱人").ToHaxeString());
            root.addChild(title);

            customScarf.Load();
            customScarf.scarfUI = this;

            HUD.Class.ME.hide(null);

            Flow CreateBottomFlow(Flow parent, FlowAlign horizontal, FlowAlign vertical, bool Vertical = false)
            {
                var flow = new Flow(parent);
                flow.set_horizontalAlign(horizontal);
                flow.set_verticalAlign(vertical);
                flow.isVertical = Vertical;
                return flow;
            }


            Attributes.Clear();

            foreach (var kv in customScarf.Datakey)
            {
                int key = kv.Key;
                var data = new ScarfListInitialisation();
                data.scarfData = kv.Value;
                data.InitAttributes();
                Attributes[key] = data;
            }

            if (Attributes.Count > 0 && !Attributes.ContainsKey(CurrentScarf))
            {
                CurrentScarf = Attributes.Keys.Min();
            }

            var main = flows[FlowEnum.MainFlow] = new Flow(root);
            main.set_verticalAlign(new FlowAlign.Middle());
            main.set_horizontalAlign(new FlowAlign.Middle());

            var left = flows[FlowEnum.LeftFlow] = CreateBottomFlow(main, new FlowAlign.Left(), new FlowAlign.Middle(), true);
            var right = flows[FlowEnum.RightFlow] = CreateBottomFlow(main, new FlowAlign.Right(), new FlowAlign.Middle(), true);
            var bottom = flows[FlowEnum.BottomFlow] = CreateBottomFlow(main, new FlowAlign.Middle(), new FlowAlign.Bottom());

            const int Spacing = 30;
            left.verticalSpacing = Spacing;
            right.verticalSpacing = Spacing;
            bottom.horizontalSpacing = Spacing;

            var tile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);
            selectionHighlight = new ScaleGrid(tile, 8, 8, null);
            flows[FlowEnum.MainFlow].addChild(selectionHighlight);

            persistentHighlight = new Graphics(root);
            persistentHighlight.alpha = 0;

            OpenPageFirst();

            var v = dc.pr.Game.Class.ME.curLevel.viewport;
            double zoom = v.zoom;
            double tozoom = zoom * 1.3;
            v.zoomFromTo(zoom, tozoom, 1, new TType.TEaseOut());

            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;


            Superaction = new Action(() =>
            {
                v.zoomFromTo(tozoom, zoom, 1, new TType.TEaseOut());
                v.delayer.addF(null, () =>
                {
                    v.updateRealPos();
                    v.updateSizes();
                }, delayer.fps * 2.0);
            });
            customScarf.UpdateSarfs();
        }


        #region UI 初始化

        private void OpenPageFirst()
        {
            AnimateOutItems(() =>
            {
                Clearpage();

                NowPage = Page.First;

                CreateButton(FlowEnum.LeftFlow, "修改当前自定义飘带", (e) => { });
                CreateButton(FlowEnum.RightFlow, "创建新自定义飘带", (e) =>
                {
                    if (Attributes.Count < 0) CeateScarfToKey(0);
                    OpenPageScraf();
                });
                AnimateInItems();
            });
        }

        private void OpenPageScraf()
        {

            AnimateOutItems(() =>
            {

                Clearpage();
                NowPage = Page.Scraf;

                var currentEditingData = Attributes[CurrentScarf];

                var leftFlow = flows[FlowEnum.LeftFlow];
                var rightFlow = flows[FlowEnum.RightFlow];
                var bottomFlow = flows[FlowEnum.BottomFlow];

                leftItems.AddRange(CreateAttributeEntries(Attributes[CurrentScarf].baseAttributes.left, leftFlow, Side.Left));
                rightItems.AddRange(CreateAttributeEntries(Attributes[CurrentScarf].baseAttributes.right, rightFlow, Side.Right));


                Bottomchild();

                AnimateInItems();
            });
        }

        private List<FlowBox> CreateAttributeEntries(List<AttributeEntry> attributes, Flow parentFlow, Side side, int startIndex = 0)
        {
            var boxes = new List<FlowBox>();
            for (int i = 0; i < attributes.Count; i++)
            {
                var attr = attributes[i];
                var box = CreateFlowBox(parentFlow);
                var text = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
                object val = attr.Getter(Attributes[CurrentScarf].scarfData);
                text.set_text($"{attr.Name}: {val}".ToHaxeString());
                boxes.Add(box);
                attr.Box = box;
                attr.Text = text;

                if (attr.Name == "props")
                {
                    box.interactive = new dc.h2d.Interactive(box.get_outerWidth(), box.get_outerHeight(), null, null);
                    box.interactive.onClick = new HlAction<Event>(e =>
                    {
                        MoveFocusTo(Side.Right, i);
                        AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
                        OpenPropsSubMenu();
                    });
                }
                else
                {
                    AddInteractiveToItem(box, side, startIndex + i);
                }
            }
            return boxes;
        }

        public void Bottomchild()
        {
            var sortedKeys = Attributes.Keys.OrderBy(k => k).ToList();
            int currentIndex = sortedKeys.IndexOf(CurrentScarf);

            if (currentIndex > 0)
            {
                int prevKey = sortedKeys[currentIndex - 1];
                CreateButton(FlowEnum.BottomFlow, "切换至上一个飘带", (e) =>
                {
                    CurrentScarf = prevKey;
                    OpenPageScraf();
                });
            }



            CreateButton(FlowEnum.BottomFlow, "删除当前飘带", (e) =>
            {
                RemoveScarf(CurrentScarf);
                customScarf.UpdateSarfs();
                OpenPageFirst();
            });

            CreateButton(FlowEnum.BottomFlow, $"当前飘带id:{CurrentScarf}", (e) =>
            {

            });

            CreateButton(FlowEnum.BottomFlow, "创建新飘带(默认配置)", (e) =>
            {
                int newKey = Attributes.Keys.Count == 0 ? 0 : Attributes.Keys.Max() + 1;
                CeateScarfToKey(newKey);
                CurrentScarf = newKey;
                OpenPageScraf();
            });

            if (currentIndex >= 0 && currentIndex < sortedKeys.Count - 1)
            {
                int nextKey = sortedKeys[currentIndex + 1];
                CreateButton(FlowEnum.BottomFlow, "切换至下一个飘带", (e) =>
                {
                    CurrentScarf = nextKey;
                    OpenPageScraf();
                });
            }
        }

        private FlowBox CreateButton(FlowEnum parent, string text, HlAction<Event> onClick)
        {
            var bottomFlow = flows[parent];
            var box = CreateFlowBox(bottomFlow);
            var txt = new dc.ui.Text(box, null, null, Ref<double>.Null, null, null);
            txt.set_text(text.ToHaxeString());
            box.reflow();
            box.interactive = new dc.h2d.Interactive(box.get_outerWidth(), box.get_outerHeight(), null, null);
            box.interactive.onClick = onClick;
            box.interactive.onMove = new HlAction<Event>(e => { });

            switch (parent)
            {
                case FlowEnum.MainFlow:
                    break;
                case FlowEnum.RightFlow:
                    rightItems.Add(box);
                    break;
                case FlowEnum.LeftFlow:
                    leftItems.Add(box);
                    break;
                case FlowEnum.BottomFlow:
                    bottomItems.Add(box);
                    break;
                default:
                    return null!;
            }

            return box;
        }


        private void OpenPropsSubMenu()
        {
            AnimateOutItems(() =>
            {
                Clearpage();
                NowPage = Page.Props;
                var leftFlow = flows[FlowEnum.LeftFlow];
                var rightFlow = flows[FlowEnum.RightFlow];

                leftItems.AddRange(CreateAttributeEntries(Attributes[CurrentScarf].propsAttributes.left, leftFlow, Side.Left));
                rightItems.AddRange(CreateAttributeEntries(Attributes[CurrentScarf].propsAttributes.right, rightFlow, Side.Right));

                Bottomchild();
                AnimateInItems();
            });
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
        }

        #endregion

        #region 飘带配置
        public static ScarfListInitialisation CeateScarfToKey(int key)
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
            customScarf.RemoveScarf(key);

            if (Attributes.Count == 0)
            {
                CeateScarfToKey(0);
                CurrentScarf = 0;
            }
            else
            {
                if (key == CurrentScarf)
                {
                    CurrentScarf = Attributes.Keys.Min();
                }
                else if (!Attributes.ContainsKey(CurrentScarf))
                {
                    CurrentScarf = Attributes.Keys.Min();
                }
            }
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
            FlowBox oldItem = null!;
            if (currentSide != Side.Bottom)
            {
                var oldList = (currentSide == Side.Left) ? leftItems : rightItems;
                if (currentIndex >= 0 && currentIndex < oldList.Count)
                    oldItem = oldList[currentIndex];
            }
            else
            {
                if (bottomIndex >= 0 && bottomIndex < bottomItems.Count)
                    oldItem = bottomItems[bottomIndex];
            }


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

            if (oldItem != null)
                CreateGhostEffect(oldItem);
        }

        private void UpdateHighlightPosition()
        {
            if (selectionHighlight == null || isAnimating) return;
            FlowBox target = null!;
            if (currentSide == Side.Bottom && bottomIndex < bottomItems.Count)
                target = bottomItems[bottomIndex];
            else
                target = GetCurrentSelectedItem();

            if (target == null) return;

            var main = flows[FlowEnum.MainFlow];
            main.reflow();
            target.reflow();

            double padding = get_pixelScale.Invoke() * 10.0;
            double targetW = target.get_outerWidth();
            double targetH = target.get_outerHeight();

            selectionHighlight.set_width((int)(targetW + padding));
            selectionHighlight.set_height((int)(targetH + padding));

            var bounds = target.getBounds(main, null);
            var targetx = bounds.xMin - padding / 2;
            var targety = bounds.yMin - padding / 2;
            selectionHighlight.x = targetx;
            selectionHighlight.y = targety;
            selectionHighlight.posChanged = true;

            persistentHighlight.clear();
            persistentHighlight.beginFill(Ref<int>.In(0x3366FF), Ref<double>.In(0.25));
            persistentHighlight.drawRect(0, 0, targetW + padding, targetH + padding);
            persistentHighlight.endFill();
            persistentHighlight.x = targetx;
            persistentHighlight.y = targety;
            persistentHighlight.alpha = 0.6;
            persistentHighlight.posChanged = true;
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
            if (selectionHighlight != null && controller.manualLock)
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
                    OpenPageFirst();
                    return;
                }
                if (NowPage == Page.Props)
                {
                    OpenPageScraf();
                    return;
                }

                Close();
            }
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

            if (attr.CustomValueChanger != null)
            {
                newVal = attr.CustomValueChanger(oldVal, delta);
            }

            else if (attr.ValueType == typeof(int?) && (attr.Name.ToLowerInvariant().Contains("color") || attr.Name.ToLowerInvariant().Contains("backColor")))
            {

                controller.manualLock = true;
                Hideandshow(false);
                var picker = new ColorGridSelector(selectedColor =>
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
                    }), 2.0);
                });
                return;
            }

            else if (attr.ValueType == typeof(double))
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
                int? val = (int?)oldVal;
                if (val == null) val = 0;
                newVal = val + delta;
            }
            else if (attr.ValueType == typeof(bool))
            {
                newVal = !(bool)oldVal;
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
            Superaction?.Invoke();
        }

        #endregion

        #region Anim
        private void CreateGhostEffect(FlowBox sourceBox)
        {
            if (sourceBox == null) return;
            var main = flows[FlowEnum.MainFlow];
            var bounds = sourceBox.getBounds(main, null);
            if (bounds == null) return;

            double w = bounds.xMax - bounds.xMin;
            double h = bounds.yMax - bounds.yMin;

            var ghost = new Graphics(root);
            ghost.beginFill(Ref<int>.In(0xFFFFFF), Ref<double>.In(0.5));
            ghost.drawRect(0, 0, w, h);
            ghost.endFill();
            ghost.x = bounds.xMin;
            ghost.y = bounds.yMin;

            var tween = AnimHelper.CreateTween(
                tw,
                () => ghost.alpha,
                (v) => { ghost.alpha = v; ghost.posChanged = true; },
                0.0,
                0.8
            );
            tween.onEnd = new HlAction(() => ghost.remove());
        }

        private void AnimateOutItems(Action onComplete)
        {
            if (isAnimating) { onComplete?.Invoke(); return; }
            isAnimating = true;
            controller.manualLock = true;

            int total = leftItems.Count + rightItems.Count + bottomItems.Count;
            if (total == 0)
            {
                isAnimating = false;
                onComplete?.Invoke();
                return;
            }

            double ww = dc.hxd.Window.Class.getInstance().get_width();
            double wh = dc.hxd.Window.Class.getInstance().get_height();
            double animationDuration = 0.3;

            foreach (var box in leftItems)
            {
                double targetX = box.x - ww;
                var tween = AnimHelper.CreateTween(tw, () => box.x, v => { box.x = v; box.posChanged = true; }, targetX, animationDuration);
            }
            foreach (var box in rightItems)
            {
                double targetX = box.x + ww;
                var tween = AnimHelper.CreateTween(tw, () => box.x, v => { box.x = v; box.posChanged = true; }, targetX, animationDuration);
            }
            foreach (var box in bottomItems)
            {
                double targetY = box.y + wh;
                var tween = AnimHelper.CreateTween(tw, () => box.y, v => { box.y = v; box.posChanged = true; }, targetY, animationDuration);
            }

            delayer.addF(null, new HlAction(() =>
            {
                isAnimating = false;
                controller.manualLock = false;
                onComplete?.Invoke();
            }), animationDuration);
        }

        private void AnimateInItems()
        {
            if (isAnimating) return;
            isAnimating = true;
            controller.manualLock = true;

            root.set_visible(false);
            setMainFlowPos();

            selectionHighlight.alpha = 0;
            persistentHighlight.alpha = 0;

            double ww = dc.hxd.Window.Class.getInstance().get_width();
            double wh = dc.hxd.Window.Class.getInstance().get_height();

            var items = bottomItems.Select(b => (b, "bottom"))
                    .Concat(rightItems.Select(r => (r, "right")))
                    .Concat(leftItems.Select(l => (l, "left")))
                    .ToList();

            if (items.Count == 0)
            {
                isAnimating = false;
                selectionHighlight.alpha = 0.8;
                persistentHighlight.alpha = 0.6;
                UpdateHighlightPosition();
                return;
            }

            var targets = new Dictionary<FlowBox, (double x, double y)>();
            foreach (var (box, _) in items)
                targets[box] = (box.x, box.y);

            foreach (var (box, type) in items)
            {
                var (tx, ty) = targets[box];
                if (type == "left")
                {
                    box.x = tx - ww;
                    box.y = ty;
                }
                else if (type == "right")
                {
                    box.x = tx + ww;
                    box.y = ty;
                }
                else if (type == "bottom")
                {
                    box.x = tx;
                    box.y = ty + wh;
                }
                box.posChanged = true;
            }



            const double delayStep = 0.2;
            const double animationDuration = 0.4;


            int leftIdx = 1, rightIdx = 1, bottomIdx = 2;

            for (int i = 0; i < items.Count; i++)
            {
                var (box, type) = items[i];
                double delaySec = 0;

                if (type == "left")
                {
                    delaySec = leftIdx * delayStep;
                    leftIdx++;
                }
                else if (type == "right")
                {
                    delaySec = rightIdx * delayStep;
                    rightIdx++;
                }
                else if (type == "bottom")
                {
                    delaySec = bottomIdx * delayStep;
                    bottomIdx++;
                }

                var (targetX, targetY) = targets[box];

                var tweenX = AnimHelper.CreateTween(tw, () => box.x, v => { box.x = v; box.posChanged = true; }, targetX, animationDuration);
                tweenX.delayMs(delaySec * 1000);

                var tweenY = AnimHelper.CreateTween(tw, () => box.y, v => { box.y = v; box.posChanged = true; }, targetY, animationDuration);
                tweenY.delayMs(delaySec * 1000);
            }
            root.set_visible(true);

            int maxCount = System.Math.Max(leftIdx, System.Math.Max(rightIdx, bottomIdx));
            double totalDuration = (maxCount - 1) * delayStep + animationDuration;
            delayer.addF(null, new HlAction(() =>
            {
                controller.manualLock = false;
                isAnimating = false;
            }), totalDuration);
        }
        #endregion
    }
}
