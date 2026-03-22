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
        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;
        public TeamManager GetTeam = null!;
        public FlowBox teamFlowBox = null!;
        public string GetSelectedteamid = null!;

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
            GetAllText.Clear();
            GetTeam = teamManager;
            OnMonsterSelected = (data) => { };
        }

        public bool onOut = false;

        public override int get_wid() => 10;
        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;


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
            var rightFlow = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref padH), Ref<double>.From(ref padV), Ref<bool>.Null, null);
            base.rightFlow = rightFlow;

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
                string teamInfo = $"- {team.Name} (ID: {team.Id})";
                var teamText = Assets.Class.makeText(Lang.Class.t.untranslated(teamInfo), null, true, null);
                teamText.set_textColor(team.TeamColor);
                GetAllText.Add(team.Id, teamText);
                teamFlowBox.addChild(teamText);
                teamText.alpha = 0.5;

                var interactive = new Interactive(teamText.textWidth, teamText.textHeight, teamText, null);
                interactive.onClick = (e) =>
                {
                    AudioHelper.LoadAudioFormString("sfx/ui/menu_click1.wav");
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


                if (team.OpposingTeamIds != null && team.OpposingTeamIds.Count > 0)
                {
                    string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                    var opposingText = Assets.Class.makeText(Lang.Class.t.untranslated(opposingInfo), null, true, null);
                    opposingText.set_textColor(CreateColor.ColorFromHex("#ffffff"));
                    opposingText.scaleX = 1.3;
                    opposingText.scaleY = 1.3;
                    GetAllText.Add(team.Id + "opposingText", opposingText);
                    teamFlowBox.addChild(opposingText);
                }




                if (team.DefaultEnemies != null && team.DefaultEnemies.Count > 0)
                {
                    var countDict = new Dictionary<string, int>();
                    foreach (var id in team.DefaultEnemies)
                    {
                        string lang = getmobnamebyid(id);
                        if (countDict.ContainsKey(lang))
                            countDict[lang]++;
                        else
                            countDict[lang] = 1;
                    }

                    var parts = new List<string>();
                    foreach (var kvp in countDict)
                    {
                        parts.Add(kvp.Value == 1 ? kvp.Key : $"{kvp.Key}+{kvp.Value}");
                    }
                    double screenWidth = dc.hxd.Window.Class.getInstance().get_width();
                    double maxWidth = screenWidth / 5;


                    var partWidths = new List<double>();
                    foreach (var part in parts)
                    {
                        var tempTextPart = Assets.Class.makeText(part.ToHaxeString(), null, true, null);
                        double width = tempTextPart.textWidth * 0.5;
                        tempTextPart.remove();
                        partWidths.Add(width);
                    }


                    var prefixText = Assets.Class.makeText("队伍名单:".ToHaxeString(), null, true, null);
                    double prefixWidth = prefixText.textWidth * 0.5;
                    prefixText.remove();


                    string allmobSpace = string.Join(" ", parts);
                    string testInfo = $"队伍名单:{allmobSpace}";
                    var tempText = Assets.Class.makeText(testInfo.ToHaxeString(), null, true, null);
                    double totalWidth = tempText.textWidth * 0.5;
                    tempText.remove();

                    string enemiesInfo;
                    if (totalWidth > maxWidth)
                    {

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
                        {
                            lines.Add(string.Join(" ", currentLine));
                        }


                        if (lines.Count == 1)
                        {
                            enemiesInfo = $"队伍名单:{lines[0]}";
                        }
                        else
                        {
                            enemiesInfo = $"队伍名单:{lines[0]}";
                            for (int i = 1; i < lines.Count; i++)
                            {
                                enemiesInfo += $"\n{new string(' ', "队伍名单:".Length)}{lines[i]}";
                            }
                        }
                    }
                    else
                    {
                        enemiesInfo = testInfo;
                    }
                    var enemiesText = Assets.Class.makeText(enemiesInfo.ToHaxeString(), null, true, null);
                    enemiesText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
                    enemiesText.scaleX = 1;
                    enemiesText.scaleY = 1;
                    teamFlowBox.addChild(enemiesText);
                }



            }

            if (config.Teams.Count > 0)
            {
                base.rightFlow.addChild(teamFlowBox);
            }
        }

        public override void onValidate()
        {
            var entry = getEntryAt(curX, curY);
            if (entry == null)
                return;
            string mobId = getmobs(entry.i).id.ToString();
            var args = new MonsterSelectionEventArgs { Id = mobId, Teamid = GetSelectedteamid };
            AddMonsterToTeam(args);
            doMovementIcon(entry.f, GetAllText[args.Teamid]);
        }

        private void AddMonsterToTeam(MonsterSelectionEventArgs args)
        {
            if (EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(args.Id))
                return;
            if (string.IsNullOrEmpty(args.Id) || string.IsNullOrEmpty(args.Teamid))
            {
                AudioHelper.LoadAudioFormString("sfx/ui/menu_error2.wav");
                return;
            }

            if (!GetConfig.Value.Teams.TryGetValue(args.Teamid, out var team))
            {
                AudioHelper.LoadAudioFormString("sfx/ui/menu_error2.wav");
                Log.Logger.Warning($"队伍 {args.Teamid} 不存在");
                return;
            }

            team.DefaultEnemies.Add(args.Id);
            GetConfig.Save();
            AudioHelper.LoadAudioFormString("sfx/ps5/curse_end_SE.wav");
            Log.Logger.Debug($"选择选中怪物：{args.Id}, teamid:{args.Teamid}");
        }

        public void doMovementIcon(Flow flow, Text text)
        {
            double pixelScale = get_pixelScale.Invoke();

            // Horizontal movement tween
            double horizontalTargetValue = text.x * pixelScale;
            var horizontalTween = CreateTween(tw,
                () => flow.x,
                value =>
                {
                    flow.x = value;
                    flow.posChanged = true;
                },
                horizontalTargetValue,
                0);

            // Vertical movement tween
            double verticalTargetValue = text.y * pixelScale;
            var verticalTween = CreateTween(tw,
                () => flow.y,
                value =>
                {
                    flow.y = value;
                    flow.posChanged = true;
                },
                verticalTargetValue,
                0);
        }

        private Tween CreateTween(Tweenie tw, Func<double> getter, Action<double> setterAction, double targetValue, double? duration)
        {
            var hlGetter = new HlFunc<double>(getter);
            var hlAction = new HlAction<object>((_setV) => setterAction((double)_setV));
            var hlSetter = new HlAction<double>((dt) => hlAction.Invoke(dt));
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
    }
}