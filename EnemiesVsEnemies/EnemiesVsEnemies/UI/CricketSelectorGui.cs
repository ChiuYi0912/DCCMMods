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

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public dc.ui.Text nameText = null!;
        public NumberInput GetU = null!;
        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;
        public TeamManager GetTeam = null!;
        public FlowBox teamFlowBox = null!;
        public string GetSelectedteamid = null!;

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

            nameText = Assets.Class.makeText(Lang.Class.t.untranslated(""), null, true, null);
            nameText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            nameText.set_textAlign(new Align.MultilineCenter());

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
            base.rightFlow.addChild(configTitle);

            double teamPadH = 5.0;
            double teamPadV = 5.0;
            teamFlowBox = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref teamPadH), Ref<double>.From(ref teamPadV), Ref<bool>.Null, null);
            teamFlowBox.set_isVertical(true);
            teamFlowBox.set_horizontalAlign(new FlowAlign.Middle());
            teamFlowBox.set_verticalAlign(new FlowAlign.Middle());

            var teamsTitle = Assets.Class.makeText(Lang.Class.t.untranslated("当前队伍:"), null, true, null);
            teamsTitle.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            teamFlowBox.addChild(teamsTitle);


            Tile selectionTile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);
            var selectionTM = new ScaleGrid(selectionTile, 8, 8, null);


            foreach (var team in config.Teams.Values)
            {
                string teamInfo = $"- {team.Name} (ID: {team.Id})";
                var teamText = Assets.Class.makeText(Lang.Class.t.untranslated(teamInfo), null, true, null);
                teamText.set_textColor(team.TeamColor);
                teamFlowBox.addChild(teamText);
                teamText.alpha = 0.5;

                var interactive = new Interactive(teamText.textWidth, teamText.textHeight, teamText, null);
                interactive.onClick = (e) =>
                {
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
                    string allmob = string.Join("\n", parts);

                    string enemiesInfo = $"队伍名单: \n   {allmob}";
                    var enemiesText = Assets.Class.makeText(enemiesInfo.ToHaxeString(), null, true, null);
                    enemiesText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
                    teamFlowBox.addChild(enemiesText);
                }


                if (team.OpposingTeamIds != null && team.OpposingTeamIds.Count > 0)
                {
                    string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                    var opposingText = Assets.Class.makeText(Lang.Class.t.untranslated(opposingInfo), null, true, null);
                    opposingText.set_textColor(CreateColor.ColorFromHex("#ffffff"));
                    teamFlowBox.addChild(opposingText);
                }
            }

            if (config.Teams.Count > 0)
            {
                base.rightFlow.addChild(teamFlowBox);
            }
        }

        public override void onValidate()
        {
            int curX = this.curX;
            int curY = this.curY;

            bool isLocked = getEntryAt(curX, curY).isLocked;
            string soundPath = isLocked ? "sfx/ui/menu_error2.wav" : "sfx/ui/menu_click1.wav";
            CoreLibrary.Utilities.AudioHelper.LoadAudioFormString(soundPath);


            var entry = getEntryAt(curX, curY);
            if (entry != null)
            {
                string mobId = getmobs(entry.i).id.ToString();
                var args = new MonsterSelectionEventArgs
                {
                    Id = mobId,
                    Teamid = GetSelectedteamid
                };
                OnMonsterSelected.Invoke(args);
                UpdataTameConfig();
            }

            // if (closeOnValidate)
            // {
            //     close();
            // }
        }


        public void UpdataTameConfig()
        {
            OnMonsterSelected = (data) =>
            {
                if (EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(data.Id))
                    return;
                //if(team.IsNullOrEmpty())

                Log.Logger.Debug($"选择选中怪物：{data.Id},teamid:{data.Teamid}");
            };
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