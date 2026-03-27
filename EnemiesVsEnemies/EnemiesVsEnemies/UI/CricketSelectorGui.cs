
using CoreLibrary.Core.Extensions;
using dc;
using dc.h2d;
using dc.hl.types;
using dc.hxd;
using dc.tool;
using dc.ui;
using dc.ui.icon;
using dc.ui.sel;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using EnemiesVsEnemies.UI.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Storage;
using Serilog;
using Data = dc.Data;
using Interactive = dc.h2d.Interactive;
using Text = dc.ui.Text;
using CoreLibrary.Utilities;
using CoreLibrary.Core.Utilities;
using ModCore.Modules;
using dc.h2d.col;
using ModCore.Utilities;
using Math = System.Math;
using EnemiesVsEnemies.Inter;
using dc.en.mob;
using dc.h3d.shader;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public TeamManager teamManager = null!;
        public FlowBox teamFlowBox = null!;
        public ScaleGrid selectiononClickMob = null!;
        public UITextHelper textHelper = null!;
        public Mask rightMask = null!;
        private Interactive rightInter = null!;
        private Flow figthFlowContainer = null!;
        public dc.h2d.Object rightGridContainer = null!;
        private Icon RightIcon = null!;

        private Dictionary<string, Flow> mobFlowMap = new Dictionary<string, Flow>();
        private Dictionary<string, Text> mobCountLabelMap = new Dictionary<string, Text>();
        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;

        public bool isLockedController = false;
        public string UserSelectedteamid = null!;


        public class MonsterSelectionEventArgs
        {
            public string MobId { get; set; } = null!;
        }
        public Action<MonsterSelectionEventArgs> OnMonsterSelected { get; set; }

        public CricketSelectorGui(TeamManager teammanager, string teamId = "")
        {
            teamManager = teammanager;
            OnMonsterSelected = (data) => { };

            UserSelectedteamid = teamId;
            initMYRightFlow();
        }


        public override int get_wid() => 10;
        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;
        public static int getmoblength() => Data.Class.mob.all.array.length;
        public override void initGrid() { curX = curY = 0; initEntries(getmoblength()); }
        public override void initRightFlow() { }

        public const string AudioError = "sfx/ui/menu_error2.wav";
        public const string Audiocurse = "sfx/ps5/curse_end_SE.wav";
        public const string Audioclick = "sfx/ui/menu_click1.wav";
        public const string Audiocchestroom1 = "sfx/ps5/prison_chestroom1_SE.wav";


        public static dynamic getmobsbyIndex(int index)
        {
            var arr = Data.Class.mob.all.array;

            if (index < 0 || index >= arr.length)
                return null!;

            return arr.getDyn(index);
        }

        public override bool isEntryLocked(int i)
        {
            string data = Data.Class.mob.all.array.getDyn(i).id.ToString();
            return EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(data);
        }

        public void initMYRightFlow()
        {
            textHelper = new UITextHelper(this);

            double padH = 5.0;
            double padV = 5.0;
            base.rightFlow = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref padH),
            Ref<double>.From(ref padV), Ref<bool>.Null, null);
            base.rightFlow.set_isVertical(true);
            base.rightFlow.set_horizontalAlign(new FlowAlign.Middle());
            base.rightFlow.set_verticalAlign(new FlowAlign.Bottom());


            base.mainFlow.addChild(rightFlow);


            var nameText = Assets.Class.makeText(Lang.Class.t.untranslated(""), null, true, null);
            nameText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            nameText.set_textAlign(new Align.MultilineCenter());

            textHelper.AlluiText.Add("nameText", nameText);

            base.root.addChild(nameText);

            figthFlowContainer = new Flow(rightFlow);

            rightGridContainer = new dc.h2d.Object(null);
            rightGridContainer.x = get_pixelScale.Invoke() * 5;
            rightGridContainer.posChanged = true;

            rightMask = new Mask(200, 300, null);
            rightMask.addChild(rightGridContainer);

            figthFlowContainer.addChild(rightMask);

            rightInter = new Interactive(figthFlowContainer.get_outerWidth(), figthFlowContainer.get_outerHeight(), rightMask, null);
            rightInter.propagateEvents = true;
            rightInter.onWheel = new HlAction<Event>(OnRightWheel);
            rightInter.onMove = (e) => { };

            Tile tile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null,
            Ref<double>.Null, Ref<double>.Null, null);
            selectiononClickMob = new ScaleGrid(tile, 10, 10, rightMask);
            selectiononClickMob.alpha = 1.0;

            const double HEIGHT_Thereal = 1.5;
            double pixelScale = base.get_pixelScale.Invoke();
            double fbWidth = base.fbItems.get_outerWidth();
            double fbHeight = base.fbItems.get_outerHeight() / HEIGHT_Thereal;

            double screenWidth = dc.hxd.Window.Class.getInstance().get_width();
            const double WIDTH_Thereal = 2.85;
            double maxWidth = screenWidth / WIDTH_Thereal;


            double rightX = fbItems.x + fbWidth + 20 * pixelScale;
            double rightY = fbItems.y;

            rightMask.x = rightX;
            rightMask.y = rightY;
            rightMask.width = (int)maxWidth;
            rightMask.height = (int)fbHeight;

            rightInter.width = maxWidth;
            rightInter.height = fbHeight;


            textHelper.AddConfigInfoToRightFlow();

            initRightGrid();

            figthFlowContainer.reflow();
        }


        public void initRightGrid()
        {
            var team = GetConfig.Value.Teams[UserSelectedteamid];

            ClearRightGrid();

            var countDict = new Dictionary<string, int>();
            foreach (var id in team.DefaultEnemies)
                countDict[id] = countDict.TryGetValue(id, out int c) ? c + 1 : 1;

            const int cols = 7;
            double scale = get_pixelScale.Invoke();
            int spacing = (int)(scale * 8.0);
            int cellWidth = (int)(get_entryWid() * scale);
            int cellHeight = (int)(get_entryHei() * scale);

            int idx = 0;
            foreach (var kvp in countDict)
            {
                string mobId = kvp.Key;
                int count = kvp.Value;

                var flow = CreateMonsterFlow(mobId, count);
                int row = idx / cols;
                int col = idx % cols;
                flow.x = col * (cellWidth + spacing);
                flow.y = row * (cellHeight + spacing);

                mobFlowMap[mobId] = flow;
                idx++;
            }
        }

        private void ClearRightGrid()
        {
            foreach (var flow in mobFlowMap.Values)
                flow.remove();
            mobFlowMap.Clear();
            mobCountLabelMap.Clear();
        }

        private Flow CreateMonsterFlow(string mobId, int count)
        {
            var flow = new Flow(rightGridContainer);
            flow.isVertical = true;
            flow.set_horizontalAlign(new FlowAlign.Middle());

            var icon = Icon.Class.createMobIcon(mobId.ToHaxeString(), flow);
            icon.tile.scaleToSize(72, 72);

            var interactive = new Interactive(icon.tile.width, icon.tile.height, icon, null);
            interactive.propagateEvents = false;

            string onmoveMob = mobId;
            interactive.onClick = (e) =>
            {
                AudioHelper.LoadAudioFormString(Audiocurse);
                RemoveMonsterFromTeam(onmoveMob);

                var getvirtual = Data.Class.mob.byId.get(onmoveMob.ToHaxeString()).index;
                var newicon = Icon.Class.createMobIcon(mobId.ToHaxeString(), null);
                UIAnimHelper.doMovementIcon(this, entries.getDyn(getvirtual).f, flow, newicon, false);
            };
            interactive.onMove = (e) =>
            {
                RightIcon = icon;
            };
            interactive.onFocus = (e) =>
            {
                EnemiesVsEnemiesMod.GetLogger.LogError("onFocus");
            };


            if (count > 1)
            {
                var label = new Text(null, false, null, Ref<double>.Null, null, null);
                flow.addChild(label);
                label.addShader(new dc.shader.HotlineText());
                label.scaleX = label.scaleY = 1.5;
                label.textColor = CreateColor.ColorFromHex("#ffffff");
                label.set_text($"+{count}".ToHaxeString());
                mobCountLabelMap[mobId] = label;
            }
            else
            {
                if (mobCountLabelMap.ContainsKey(mobId))
                    mobCountLabelMap.Remove(mobId);
            }

            return flow;
        }


        private Flow AddMonsterToRightGrid(string mobId)
        {
            if (mobFlowMap.TryGetValue(mobId, out var existingFlow))
            {
                int newCount = GetCurrentCount(mobId);

                if (newCount > 1)
                {
                    if (mobCountLabelMap.TryGetValue(mobId, out var label))
                    {
                        label.set_text($"+{newCount}".ToHaxeString());
                    }
                    else
                    {
                        var newLabel = new Text(null, false, null, Ref<double>.Null, null, null);
                        existingFlow.addChild(newLabel);
                        newLabel.addShader(new dc.shader.HotlineText());
                        newLabel.scaleX = newLabel.scaleY = 1.5;
                        newLabel.textColor = CreateColor.ColorFromHex("#ffffff");
                        newLabel.set_text($"+{newCount}".ToHaxeString());
                        mobCountLabelMap[mobId] = newLabel;
                    }

                }
                else if (newCount == 1 && mobCountLabelMap.ContainsKey(mobId))
                {
                    var oldLabel = mobCountLabelMap[mobId];
                    oldLabel.remove();
                    mobCountLabelMap.Remove(mobId);

                    return (Flow)oldLabel.parent;
                }

                return existingFlow;
            }
            else
            {
                int newCount = GetCurrentCount(mobId);
                var newFlow = CreateMonsterFlow(mobId, newCount);

                mobFlowMap[mobId] = newFlow;

                ReflowRightGrid();
                return newFlow;
            }
        }

        public void RemoveMonsterFromTeam(string mobId)
        {
            var team = GetConfig.Value.Teams[UserSelectedteamid];
            if (team.DefaultEnemies.Remove(mobId))
            {
                GetConfig.Save();
                UpdateRightGridForRemove(mobId);
            }
        }

        private void UpdateRightGridForRemove(string mobId)
        {
            if (!mobFlowMap.TryGetValue(mobId, out var flow))
                return;

            int newCount = GetCurrentCount(mobId);
            if (newCount == 0)
            {
                flow.remove();
                mobFlowMap.Remove(mobId);
                mobCountLabelMap.Remove(mobId);
            }
            else
            {
                if (newCount == 1)
                {
                    if (mobCountLabelMap.TryGetValue(mobId, out var label))
                    {
                        label.remove();
                        mobCountLabelMap.Remove(mobId);
                    }
                }
                else
                {
                    if (mobCountLabelMap.TryGetValue(mobId, out var label))
                    {
                        label.set_text($"+{newCount}".ToHaxeString());
                    }
                    else
                    {
                        var newLabel = new Text(null, false, null, Ref<double>.Null, null, null);
                        flow.addChild(newLabel);
                        newLabel.addShader(new dc.shader.HotlineText());
                        newLabel.scaleX = newLabel.scaleY = 1.5;
                        newLabel.textColor = CreateColor.ColorFromHex("#ffffff");
                        newLabel.set_text($"+{newCount}".ToHaxeString());
                        mobCountLabelMap[mobId] = newLabel;
                    }
                }
            }

            ReflowRightGrid();
        }

        private int GetCurrentCount(string mobId)
        {
            var team = GetConfig.Value.Teams[UserSelectedteamid];
            return team.DefaultEnemies.Count(id => id == mobId);
        }

        private void ReflowRightGrid()
        {
            const int cols = 7;
            double scale = get_pixelScale.Invoke();
            int spacing = (int)(scale * 8.0);
            int cellWidth = (int)(get_entryWid() * scale);
            int cellHeight = (int)(get_entryHei() * scale);

            int idx = 0;
            foreach (var kvp in mobFlowMap)
            {
                int row = idx / cols;
                int col = idx % cols;
                kvp.Value.x = col * (cellWidth + spacing);
                kvp.Value.y = row * (cellHeight + spacing);
                idx++;
            }

            rightGridContainer.posChanged = true;
        }



        private void OnRightWheel(Event e)
        {
            if (controller == null) return;
            if (controller.parent.exclusiveId != controller.id) return;

            double delta = e.wheelDelta;
            if (delta == 0) return;

            const double step = 30;

            Bounds bounds = new Bounds();
            rightGridContainer.getBounds(null, bounds);
            double height = bounds.yMax - bounds.yMin;

            double maxScroll = Math.Max(0, height - rightMask.height);
            double newY = rightGridContainer.y - delta * step;
            newY = Math.Max(-maxScroll, Math.Min(0, newY));

            rightGridContainer.y = newY;
            rightGridContainer.posChanged = true;
        }



        public override void onValidate()
        {
            var entry = getEntryAt(curX, curY);
            if (entry == null)
                return;


            string mobId = getmobsbyIndex(entry.i).id.ToString();
            var args = new MonsterSelectionEventArgs { MobId = mobId };
            if (entry.isLocked)
            {
                AudioHelper.LoadAudioFormString(AudioError);
                return;
            }

            AddMonsterToTeam(args);
            var gotoflow = AddMonsterToRightGrid(args.MobId);
            UIAnimHelper.doMovementIcon(this, gotoflow, entry.f, (Icon)getIconBmp(entry.i, null!), true);

        }



        private void AddMonsterToTeam(MonsterSelectionEventArgs args)
        {
            if (EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(args.MobId))
                return;

            if (!GetConfig.Value.Teams.TryGetValue(UserSelectedteamid, out var team))
            {
                AudioHelper.LoadAudioFormString(AudioError);
                Log.Logger.Warning($"队伍 {UserSelectedteamid} 不存在");
                return;
            }

            team.DefaultEnemies.Add(args.MobId);
            GetConfig.Save();
            textHelper.UpdateTeamDisplay();

            AudioHelper.LoadAudioFormString(Audiocchestroom1);

            Log.Logger.Debug($"选择选中怪物：{args.MobId}, teamid:{UserSelectedteamid}");
        }

        public override void setMainFlowPos()
        {
            base.setMainFlowPos();
            mainFlow.reflow();
            mainFlow.x = 50;
            mainFlow.posChanged = true;
        }


        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            string name = getmobsbyIndex(i).id.ToString();
            var icon = Icon.Class.createMobIcon(name.ToHaxeString(), parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());
            return icon;
        }

        public override void postUpdate()
        {
            Main main = Main.Class.ME;
            Point entryGlobal = main.localToGlobal(this.getEntryAt(curX, curY).f, Ref<double>.Null, Ref<double>.Null);
            Point maskGlobal = main.localToGlobal(this.mask, Ref<double>.Null, Ref<double>.Null);


            double offset = (int)(get_pixelScale.Invoke() * 5.0);
            ScaleGrid selection = this.selectionSG;
            selection.posChanged = true;
            selection.x = entryGlobal.x - maskGlobal.x - offset;
            selection.y = entryGlobal.y - maskGlobal.y - offset;


            if (!UserSelectedteamid.IsNullOrEmpty())
                UptDateSelectiononClickMob();
        }

        public void UptDateSelectiononClickMob()
        {
            double timeFactor = base.ftime * 0.1;
            string speedKey = "co_blinkCursorSpeed";

            var speedData = Data.Class.gui.byId.get(speedKey.ToHaxeString()).v0;

            var angle = timeFactor * speedData;
            var cosValue = System.Math.Cos(angle);
            var alphaOffset = 0.2 * cosValue;

            this.selectionSG.alpha = 0.8 + alphaOffset;
            selectiononClickMob.alpha = 0.8 + alphaOffset;

            if (RightIcon == null)
                return;

            Point iconGlobal = Main.Class.ME.localToGlobal(RightIcon, Ref<double>.Null, Ref<double>.Null);
            Point localPos = rightMask.globalToLocal(iconGlobal);

            double w = RightIcon.tile.width;
            double h = RightIcon.tile.height;

            double padding = get_pixelScale.Invoke() * 10;
            double boxWidth = w + padding / 5;
            double boxHeight = h + padding / 5;

            selectiononClickMob.set_width((int)boxWidth);
            selectiononClickMob.set_height((int)boxHeight);


            double offsetX = (boxWidth - w) / 2;
            double offsetY = (boxHeight - h) / 2;

            selectiononClickMob.x = localPos.x - offsetX;
            selectiononClickMob.y = localPos.y - offsetY;

            selectiononClickMob.posChanged = true;

        }



        public override void setControlLabel()
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

            btns.push(creataBCreateButton(3, "添加仇恨队伍"));

            createControlLabel(btns);
        }

        public override bool controlsUpdate()
        {
            if (isLockedController)
                return false;

            Controller parent = controller.parent;


            if (ControllerHelper.ControlsUpdateFromProcess(parent, 3))
            {
                textHelper.AddOpposingTeamFromGui(teamManager);
                return true;
            }

            return base.controlsUpdate();
        }
    }
}