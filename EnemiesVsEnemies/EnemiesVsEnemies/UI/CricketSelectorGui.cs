using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.mob;
using dc.h2d;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.hxd.snd;
using dc.libs;
using dc.libs.heaps.slib;
using dc.pr;
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
using dc.ui.hud;
using dc.libs.misc;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public NumberInput GetU = null!;
        public TeamManager GetTeam = null!;
        public FlowBox teamFlowBox = null!;
        public string GetSelectedteamid = null!;

        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;


        public static Dictionary<string, dc.ui.Text> GetAllText = new();
        public static Dictionary<string, FlowBox> GetallFlow = new();


        public class MonsterSelectionEventArgs
        {
            public string Id { get; set; } = null!;
            public string Teamid { get; set; } = null!;
        }
        public Action<MonsterSelectionEventArgs> OnMonsterSelected { get; set; }

        public CricketSelectorGui(TeamManager teamManager)
        {
            GetTeam = teamManager;
            OnMonsterSelected = (data) => { };
        }

        public bool onOut = false;

        public override int get_wid() => 10;
        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;

        public const string AudioError = "sfx/ui/menu_error2.wav";
        public const string Audiocurse = "sfx/ps5/curse_end_SE.wav";
        public const string Audioclick = "sfx/ui/menu_click1.wav";


        public override bool isEntryLocked(int i)
        {
            string data = Data.Class.mob.all.array.getDyn(i).id.ToString();
            return EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(data);
        }

        public dynamic getmobs(int index)
        {
            var arr = Data.Class.mob.all.array;

            if (index < 0 || index >= arr.length)
                return null!;

            return arr.getDyn(index);
        }


        public int getmoblength() => Data.Class.mob.all.array.length;


        public string getmobnamebyid(string id)
        {
            string data = Data.Class.mob.byId.get(id.ToHaxeString()).name.ToString();
            return Lang.Class.t.get(data.ToHaxeString(), null).ToString();
        }


        public override void initGrid()
        {
            curX = curY = 0;
            initEntries(getmoblength());
        }



        public override void initRightFlow()
        {
            double padH = 5.0;
            double padV = 5.0;
            base.rightFlow = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref padH), Ref<double>.From(ref padV), Ref<bool>.Null, null);
            base.rightFlow.set_isVertical(true);
            base.rightFlow.set_horizontalAlign(new FlowAlign.Right());
            base.rightFlow.set_verticalAlign(new FlowAlign.Bottom());


            base.mainFlow.addChild(base.rightFlow);


            var nameText = Assets.Class.makeText(Lang.Class.t.untranslated(""), null, true, null);
            nameText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            nameText.set_textAlign(new Align.MultilineCenter());


            GetAllText.Add("nameText", nameText);


            base.root.addChild(nameText);


            AddConfigInfoToRightFlow();

            // GetU = new NumberInput(this);
            // var action = new Action<int>((value) =>
            // {
            //     istest = value;
            // });
            // var inpu = GetU.OpenNumberInput("TEST", "hello", 1, action);
        }

        private void AddConfigInfoToRightFlow()
        {
            if (GetConfig == null || base.rightFlow == null || this.mainFlow == null)
                return;


            var config = GetConfig.Value;
            if (config == null)
                return;


            mainFlow.set_horizontalAlign(new FlowAlign.Middle());
            mainFlow.set_verticalAlign(new FlowAlign.Bottom());


            GetAllText.Clear();


            var configTitle = Assets.Class.makeText(Lang.Class.t.untranslated("斗蛐蛐MOD"), null, true, null);
            configTitle.set_textColor(CreateColor.ColorFromHex("#ffffff"));
            configTitle.set_textAlign(new Align.Center());


            GetAllText.Add("configTitle", configTitle);
            base.rightFlow.addChild(configTitle);


            double teamPadH = 5.0;
            double teamPadV = 5.0;
            teamFlowBox = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref teamPadH), Ref<double>.From(ref teamPadV), Ref<bool>.Null, null);
            teamFlowBox.set_isVertical(true);
            teamFlowBox.set_horizontalAlign(new FlowAlign.Middle());
            teamFlowBox.set_verticalAlign(new FlowAlign.Middle());


            var teamsTitle = Assets.Class.makeText(Lang.Class.t.untranslated("当前队伍:"), null, true, null);
            teamsTitle.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));


            GetAllText.Add("teamsTitle", teamsTitle);
            teamFlowBox.addChild(teamsTitle);


            Tile selectionTile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);
            var selectionTM = new ScaleGrid(selectionTile, 8, 8, null);



            foreach (var team in config.Teams.Values)
            {
                if (!team.Name.IsNullOrEmpty() && !team.Id.IsNullOrEmpty())
                {
                    string teamInfo = $"- {team.Name} (ID: {team.Id})";
                    var teamText = Assets.Class.makeText(Lang.Class.t.untranslated(teamInfo), null, true, null);
                    teamText.alpha = 0.5;
                    teamText.scaleX = 1.3;
                    teamText.scaleY = 1.3;
                    teamText.set_textColor(team.TeamColor);


                    GetAllText.Add(team.Id, teamText);
                    teamFlowBox.addChild(teamText);


                    var interactive = new Interactive(teamText.textWidth, teamText.textHeight, teamText, null);
                    interactive.onClick = (e) =>
                    {
                        AudioHelper.LoadAudioFormString(Audioclick);
                        GetSelectedteamid = team.Id;
                        Log.Logger.Debug($"选中队伍：{GetSelectedteamid}");
                    };
                    interactive.onOver = (e) =>
                    {
                        teamText.alpha = 1.0;
                    };
                    interactive.onOut = (e) =>
                    {
                        teamText.alpha = 0.5;
                    };

                }


                if (team.OpposingTeamIds != null && team.OpposingTeamIds.Count > 0)
                {
                    string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                    var opposingText = Assets.Class.makeText(Lang.Class.t.untranslated(opposingInfo), null, true, null);
                    opposingText.set_textColor(CreateColor.ColorFromHex("#ffffff"));
                    opposingText.scaleX = 1.1;
                    opposingText.scaleY = 1.1;


                    GetAllText.Add(team.Id + "opposingText", opposingText);
                    teamFlowBox.addChild(opposingText);
                }


                if (team.DefaultEnemies != null && team.DefaultEnemies.Count > 0)
                {
                    var enemiesText = Assets.Class.makeText("".ToHaxeString(), null, true, null);
                    enemiesText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
                    enemiesText.scaleX = 1;
                    enemiesText.scaleY = 1;


                    teamFlowBox.addChild(enemiesText);
                    GetAllText.Add(team.Id + "enemiesText", enemiesText);
                }
            }

            if (config.Teams.Count > 0)
            {
                base.rightFlow.addChild(teamFlowBox);
            }

            UpdateTeamDisplay();
        }

        public override void onValidate()
        {
            var entry = getEntryAt(curX, curY);
            if (entry == null)
                return;


            string mobId = getmobs(entry.i).id.ToString();
            var args = new MonsterSelectionEventArgs { Id = mobId, Teamid = GetSelectedteamid };
            if (string.IsNullOrEmpty(args.Id) || string.IsNullOrEmpty(args.Teamid) || entry.isLocked)
            {
                AudioHelper.LoadAudioFormString(AudioError);
                return;
            }


            AddMonsterToTeam(args);


            doMovementIcon(GetAllText[args.Teamid], entry, args);
        }



        private void AddMonsterToTeam(MonsterSelectionEventArgs args)
        {
            if (EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(args.Id))
                return;


            if (!GetConfig.Value.Teams.TryGetValue(args.Teamid, out var team))
            {
                AudioHelper.LoadAudioFormString(AudioError);
                Log.Logger.Warning($"队伍 {args.Teamid} 不存在");
                return;
            }


            team.DefaultEnemies.Add(args.Id);
            GetConfig.Save();
            UpdateTeamDisplay();


            AudioHelper.LoadAudioFormString(Audiocurse);

            Log.Logger.Debug($"选择选中怪物：{args.Id}, teamid:{args.Teamid}");
        }

        public List<Flow> remove = new();
        public void doMovementIcon(Text text, virtual_cx_cy_f_i_isLocked_sectionIdx_ seledata, MonsterSelectionEventArgs args)
        {
            double pixelScale = get_pixelScale.Invoke();
            Flow oldflow = seledata.f;


            Flow cloneflow = new Flow(null);
            var cloneicon = getIconBmp(seledata.i, cloneflow);
            cloneicon.posChanged = true;
            cloneicon.scaleX = pixelScale;
            cloneicon.posChanged = true;
            cloneicon.scaleY = pixelScale;

            mask.addChild(cloneflow);

            Main main = Main.Class.ME;
            const double speed = 800;


            Point oldflowGlobal = main.localToGlobal(oldflow, Ref<double>.Null, Ref<double>.Null);
            Point startLocal = mask.globalToLocal(oldflowGlobal);


            Point textGlobal = main.localToGlobal(text, Ref<double>.Null, Ref<double>.Null);
            Point targetLocal = mask.globalToLocal(textGlobal);


            cloneflow.x = startLocal.x;
            cloneflow.y = startLocal.y;


            var tweenX = CreateTween(
                () => cloneflow.x,
                value =>
                {
                    cloneflow.x = value;
                    cloneflow.posChanged = true;
                },
                targetLocal.x, speed);

            var tweenY = CreateTween(
                () => cloneflow.y,
                value =>
                {
                    cloneflow.y = value;
                    cloneflow.posChanged = true;
                },
                targetLocal.y, speed);


            if (remove.Count >= 3)
            {
                Flow oldest = remove[0];
                oldest.remove();
                remove.RemoveAt(0);
            }
            remove.Add(cloneflow);
        }

        private Tween CreateTween(Func<double> getter, Action<double> setterAction, double targetValue, double? duration)
        {
            var hlGetter = new HlFunc<double>(getter);
            var hlSetter = new HlAction<double>(setterAction);
            var tweenType = new TType.TEaseOut();
            return tw.create_(hlGetter, hlSetter, null, targetValue, tweenType, duration, Ref<bool>.Null);
        }




        public override void updateRightFlow() { }
        public override void beforeUpdateSelection() { }




        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            string name = getmobs(i).id.ToString();
            if (name.IsNullOrEmpty())
                return new Bitmap(Tile.Class.fromColor(0xFF0000, 32, 32, null, null), parent);


            var icon = Icon.Class.createMobIcon(name.ToHaxeString(), parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());


            return icon;
        }

        public override void postUpdate()
        {
            Main main = Main.Class.ME;
            int curX = this.curX;
            int curY = this.curY;


            Point entryGlobal = main.localToGlobal(this.getEntryAt(curX, curY).f, Ref<double>.Null, Ref<double>.Null);
            Point maskGlobal = main.localToGlobal(this.mask, Ref<double>.Null, Ref<double>.Null);


            double pixelScale = base.get_pixelScale.Invoke();
            double offset = (int)(pixelScale * 5.0);


            ScaleGrid selection = this.selectionSG;
            selection.posChanged = true;
            selection.x = entryGlobal.x - maskGlobal.x - offset;
            selection.y = entryGlobal.y - maskGlobal.y - offset;


            double timeFactor = base.ftime * 0.1;
            string speedKey = "co_blinkCursorSpeed";


            var speedData = Data.Class.gui.byId.get(speedKey.ToHaxeString()).v0;


            var angle = timeFactor * speedData;
            var cosValue = System.Math.Cos(angle);
            var alphaOffset = 0.2 * cosValue;


            this.selectionSG.alpha = 0.8 + alphaOffset;


            if (GetSelectedteamid.IsNullOrEmpty())
                return;


            if (GetAllText.TryGetValue(GetSelectedteamid, out var text))
                text.alpha = 0.8 + alphaOffset;

        }


        public void UpdateTeamDisplay()
        {
            if (GetConfig == null || teamFlowBox == null) return;

            var config = GetConfig.Value;
            if (config == null) return;

            foreach (var team in config.Teams.Values)
            {

                if (GetAllText.TryGetValue(team.Id, out var teamText))
                {
                    string teamInfo = $"- {team.Name} (ID: {team.Id})";
                    teamText.set_text(Lang.Class.t.untranslated(teamInfo.ToHaxeString()));
                }


                string opposingKey = team.Id + "opposingText";
                if (GetAllText.TryGetValue(opposingKey, out var opposingText))
                {
                    if (team.OpposingTeamIds != null && team.OpposingTeamIds.Count > 0)
                    {
                        string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                        opposingText.set_text(Lang.Class.t.untranslated(opposingInfo.ToHaxeString()));

                    }
                    else
                    {
                        opposingText.visible = false;
                    }
                }


                string enemiesKey = team.Id + "enemiesText";
                if (GetAllText.TryGetValue(enemiesKey, out var enemiesText))
                {
                    string newEnemiesInfo = GenerateEnemiesInfo(team);
                    enemiesText.set_text(Lang.Class.t.get(newEnemiesInfo.ToHaxeString(), null));
                    enemiesText.posChanged = true;
                }
            }


            teamFlowBox.reflow();
            base.rightFlow.reflow();
        }


        private string GenerateEnemiesInfo(TeamConfig team)
        {
            if (team.DefaultEnemies == null || team.DefaultEnemies.Count == 0)
                return "队伍名单: 无";

            var countDict = new Dictionary<string, int>();
            foreach (var id in team.DefaultEnemies)
            {
                string lang = getmobnamebyid(id);
                countDict[lang] = countDict.TryGetValue(lang, out int c) ? c + 1 : 1;
            }

            var parts = new List<string>();
            foreach (var kvp in countDict)
                parts.Add(kvp.Value == 1 ? kvp.Key : $"{kvp.Key}+{kvp.Value}");

            double screenWidth = dc.hxd.Window.Class.getInstance().get_width();
            double maxWidth = screenWidth / 6;


            string testInfo = $"队伍名单:{string.Join(" ", parts)}";
            var temp = Assets.Class.makeText(testInfo.ToHaxeString(), null, true, null);
            double totalWidth = temp.textWidth * 0.5;
            temp.remove();

            if (totalWidth <= maxWidth)
                return testInfo;


            var partWidths = new List<double>();
            foreach (var part in parts)
            {
                var t = Assets.Class.makeText(part.ToHaxeString(), null, true, null);
                partWidths.Add(t.textWidth * 0.5);
                t.remove();
            }

            var prefixText = Assets.Class.makeText("队伍名单:".ToHaxeString(), null, true, null);
            double prefixWidth = prefixText.textWidth * 0.5;
            prefixText.remove();

            var lines = new List<string>();
            var currentLine = new List<string>();
            double currentWidth = 0;

            for (int i = 0; i < parts.Count; i++)
            {
                double partWidth = partWidths[i];
                if (currentLine.Count == 0)
                {
                    if (currentWidth + partWidth + prefixWidth > maxWidth && currentLine.Count == 0)
                    {
                        lines.Add(parts[i]);
                        continue;
                    }
                    currentWidth += partWidth + prefixWidth;
                    currentLine.Add(parts[i]);
                }
                else
                {
                    double spaceWidth = 5.0;
                    if (currentWidth + spaceWidth + partWidth > maxWidth)
                    {
                        lines.Add(string.Join(" ", currentLine));
                        currentLine.Clear();
                        currentWidth = partWidth;
                        currentLine.Add(parts[i]);
                    }
                    else
                    {
                        currentWidth += spaceWidth + partWidth;
                        currentLine.Add(parts[i]);
                    }
                }
            }
            if (currentLine.Count > 0)
                lines.Add(string.Join(" ", currentLine));

            string enemiesInfo;
            if (lines.Count == 1)
                enemiesInfo = $"队伍名单:{lines[0]}";
            else
            {
                enemiesInfo = $"队伍名单:{lines[0]}";
                for (int i = 1; i < lines.Count; i++)
                    enemiesInfo += $"\n{new string(' ', "队伍名单:".Length)}{lines[i]}";
            }
            return enemiesInfo;
        }


        public override void initEntries(int size)
        {
            this.sections = (ArrayObj)ArrayUtils.CreateDyn().array;
            this.entries = (ArrayObj)ArrayUtils.CreateDyn().array;

            int column = 0;
            int row = 0;
            int width = get_wid();

            for (int index = 0; index < size; index++)
            {
                _ = this.addEntryAt(index, column, row, Ref<int>.Null);
                column++;

                if (column >= width)
                {
                    column = 0;
                    row++;
                }
            }
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
            btns.push(creataBCreateButton(KeyHelper.X, "X"));
            btns.push(creataBCreateButton(KeyHelper.C, "C"));


            createControlLabel(btns);
        }


        public override bool controlsUpdate()
        {
            bool handled = base.controlsUpdate();

            return handled;
        }
    }
}