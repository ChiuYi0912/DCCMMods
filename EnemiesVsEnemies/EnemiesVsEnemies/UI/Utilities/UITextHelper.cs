using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Utilities;
using dc;
using dc.h2d;
using dc.tool;
using dc.ui;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using HaxeProxy.Runtime;
using Serilog;
using Text = dc.ui.Text;

namespace EnemiesVsEnemies.UI.Utilities
{
    public class UITextHelper
    {

        public CricketSelectorGui SelectorGui = null!;

        public readonly Dictionary<string, dc.ui.Text> AlluiText = new();
        public readonly Dictionary<string, FlowBox> AllFlowBox = new();
        public UITextHelper(CricketSelectorGui gui)
        {
            SelectorGui = gui;
        }


        public void AddConfigInfoToRightFlow()
        {
            if (SelectorGui.GetConfig == null || SelectorGui.rightFlow == null || SelectorGui.mainFlow == null)
                return;

            var config = SelectorGui.GetConfig.Value;
            if (config == null)
                return;

            SelectorGui.mainFlow.set_horizontalAlign(new FlowAlign.Middle());
            SelectorGui.mainFlow.set_verticalAlign(new FlowAlign.Bottom());


            AddConfigTitle();


            BuildAndAddTeamFlowBox(config);


            UpdateTeamDisplay();
        }


        private void AddConfigTitle()
        {
            var configTitle = Assets.Class.makeText(Lang.Class.t.untranslated("斗蛐蛐MOD"), null, true, null);
            configTitle.set_textColor(CreateColor.ColorFromHex("#ffffff"));
            configTitle.set_textAlign(new Align.Center());

            AlluiText.Add("configTitle", configTitle);
            SelectorGui.rightFlow.addChild(configTitle);
        }

        private void BuildAndAddTeamFlowBox(ModConfig config)
        {
            double teamPadH = 5.0;
            double teamPadV = 5.0;
            SelectorGui.teamFlowBox = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref teamPadH), Ref<double>.From(ref teamPadV), Ref<bool>.Null, null);
            SelectorGui.teamFlowBox.set_isVertical(true);
            SelectorGui.teamFlowBox.set_horizontalAlign(new FlowAlign.Middle());
            SelectorGui.teamFlowBox.set_verticalAlign(new FlowAlign.Middle());

            AddTeamsTitleToBox();
            AddAllTeamsToBox(config);

            if (config.Teams.Count > 0)
            {
                SelectorGui.rightFlow.addChild(SelectorGui.teamFlowBox);
            }
        }


        private void AddTeamsTitleToBox()
        {
            var teamsTitle = Assets.Class.makeText(Lang.Class.t.untranslated("[队伍列表]"), null, true, null);
            teamsTitle.set_textColor(CreateColor.ColorFromHex("#ffffff"));
            AlluiText.Add("teamsTitle", teamsTitle);
            SelectorGui.teamFlowBox.addChild(teamsTitle);
        }

        private void AddAllTeamsToBox(ModConfig config)
        {
            foreach (var team in config.Teams.Values)
            {
                if (!team.Name.IsNullOrEmpty() && !team.Id.IsNullOrEmpty())
                {
                    AddTeamEntry(team);
                    AddOpposingTeamsInfo(team);
                    AddDefaultEnemiesPlaceholder(team);
                }
            }
        }


        public void RemoveTeamFromGui(string teamid, TeamManager teamManager)
        {
            if (SelectorGui.UserSelectedteamid == null || !AlluiText.ContainsKey(SelectorGui.UserSelectedteamid))
            {
                var popup = new dc.ui.ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                popup.text("注意：\n 请用鼠标选择要移除的队伍\n".ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));

                return;
            }

            teamManager.RemoveTeam(teamid);

            if (AlluiText.TryGetValue(teamid, out var TeamEntry))
            {
                TeamEntry.remove();
                AlluiText.Remove(teamid);
            }

            if (AlluiText.TryGetValue(teamid + "opposingText", out var TeamsInfo))
            {
                TeamsInfo.remove();
                AlluiText.Remove(teamid + "opposingText");
            }

            if (AlluiText.TryGetValue(teamid + "enemiesText", out var EnemiesPlaceholder))
            {
                EnemiesPlaceholder.remove();
                AlluiText.Remove(teamid + "enemiesText");
            }
            UpdateTeamDisplay();
        }




        public void AddTeamEntry(TeamConfig team)
        {
            string teamInfo = $"- {team.Name} (ID: {team.Id})";
            var teamText = Assets.Class.makeText(Lang.Class.t.untranslated(teamInfo), null, true, null);
            teamText.scaleX = 1.3;
            teamText.scaleY = 1.3;
            teamText.set_textColor(CreateColor.ColorFromHex("#eeff00"));

            AlluiText.Add(team.Id, teamText);
            SelectorGui.teamFlowBox.addChild(teamText);

            var interactive = new Interactive(teamText.textWidth, teamText.textHeight, teamText, null);
            interactive.onClick = (e) =>
            {
                AudioHelper.LoadAudioFormString(CricketSelectorGui.Audioclick);
                SelectorGui.UserSelectedteamid = team.Id;
                Log.Logger.Debug($"选中队伍：{SelectorGui.UserSelectedteamid}");
            };
            interactive.onOver = (e) =>
            {

            };
            interactive.onOut = (e) =>
            {

            };
        }


        public void AddOpposingTeamsInfo(TeamConfig team)
        {

            string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
            var opposingText = Assets.Class.makeText(Lang.Class.t.untranslated(opposingInfo), null, true, null);
            opposingText.set_textColor(CreateColor.ColorFromHex("#ff0000"));
            opposingText.scaleX = 1.1;
            opposingText.scaleY = 1.1;

            AlluiText.Add(team.Id + "opposingText", opposingText);
            SelectorGui.teamFlowBox.addChild(opposingText);

        }


        public void AddDefaultEnemiesPlaceholder(TeamConfig team)
        {
            if (team.DefaultEnemies != null)
            {
                var enemiesText = Assets.Class.makeText("".ToHaxeString(), null, true, null);
                enemiesText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
                enemiesText.scaleX = 1;
                enemiesText.scaleY = 1;

                SelectorGui.teamFlowBox.addChild(enemiesText);
                AlluiText.Add(team.Id + "enemiesText", enemiesText);
            }
        }



        public void UpdateTeamDisplay()
        {
            if (SelectorGui.GetConfig == null || SelectorGui.teamFlowBox == null) return;

            var config = SelectorGui.GetConfig.Value;
            if (config == null) return;

            foreach (var team in config.Teams.Values)
            {

                if (AlluiText.TryGetValue(team.Id, out var teamText))
                {
                    string teamInfo = $"- {team.Name} (ID: {team.Id})";
                    teamText.set_text(Lang.Class.t.untranslated(teamInfo.ToHaxeString()));
                }


                string opposingKey = team.Id + "opposingText";
                if (AlluiText.TryGetValue(opposingKey, out var opposingText))
                {
                    if (team.OpposingTeamIds != null)
                    {
                        string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                        opposingText.set_text(Lang.Class.t.untranslated(opposingInfo.ToHaxeString()));

                    }
                    else
                    {
                        string opposingInfo = $"仇恨队伍: 无";
                        opposingText.set_text(Lang.Class.t.untranslated(opposingInfo.ToHaxeString()));
                    }
                }


                string enemiesKey = team.Id + "enemiesText";
                if (AlluiText.TryGetValue(enemiesKey, out var enemiesText))
                {
                    string newEnemiesInfo = GenerateEnemiesInfo(team);
                    enemiesText.set_text(Lang.Class.t.get(newEnemiesInfo.ToHaxeString(), null));
                    enemiesText.posChanged = true;
                }
            }


            SelectorGui.teamFlowBox.reflow();
            SelectorGui.rightFlow.reflow();
        }


        private string GenerateEnemiesInfo(TeamConfig team)
        {
            if (team.DefaultEnemies == null)
                return "队伍名单: 无";

            var countDict = new Dictionary<string, int>();
            foreach (var id in team.DefaultEnemies)
            {
                string lang = UIMobHelper.getMobNamebyid(id);
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

        public void AddNewTeamFromGui(TeamManager teamManager)
        {

            Controller parent = SelectorGui.controller.parent;
            parent.isLocked = true;
            CricketSelectorGui.isLockedController = true;

            var inputTeamID = new NumberInput(SelectorGui);
            var createteam = new TeamConfig("", "");

            var action = new Action<string>((value) =>
            {
                parent.isLocked = false;
                CricketSelectorGui.isLockedController = false;

                createteam.Id = value;
                teamManager.AddTeam(createteam);
                inputTeamID.Input.close();
                var action = new Action<string>((value) =>
                {
                    createteam.Name = value;
                    SelectorGui.GetConfig.Save();
                    AddTeamEntry(createteam);
                    AddOpposingTeamsInfo(createteam);
                    AddDefaultEnemiesPlaceholder(createteam);

                });
                inputTeamID.Input = inputTeamID.OpenNumberInput("输入", "该队伍名称", 1, action);
            });
            dc.ui.TextInput inpu = inputTeamID.OpenNumberInput("输入", "该队伍唯一ID", 1, action);
        }


        public void AddOpposingTeamFromGui(TeamManager teamManager)
        {
            Controller parent = SelectorGui.controller.parent;
            parent.isLocked = true;
            CricketSelectorGui.isLockedController = true;

            var inputTeamID = new NumberInput(SelectorGui);

            var action = new Action<string>((value) =>
            {
                parent.isLocked = false;
                CricketSelectorGui.isLockedController = false;

                var targetTeamId = SelectorGui.UserSelectedteamid;
                var teamconfig = SelectorGui.GetConfig.Value.Teams;
                if (teamconfig.TryGetValue(targetTeamId, out var team))
                {
                    team.OpposingTeamIds.Add(value);
                    SelectorGui.GetConfig.Save();
                }
                else
                {
                    var popup = new dc.ui.ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                    popup.text("添加失败：\n 请先选择队伍！\n".ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
                }

                inputTeamID.Input.close();
                UpdateTeamDisplay();

            });
            dc.ui.TextInput inpu = inputTeamID.OpenNumberInput("输入", "该仇恨方队伍唯一ID", 1, action);
        }
    }
}