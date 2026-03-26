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


        private dc.ui.Text CreateAndAddText(string text, string dictionaryKey, string colorHex = "#ffffff",
                                          double scale = 1.0, FlowBox? parentContainer = null,
                                          bool useTranslation = true)
        {
            dc.String haxeText;
            if (useTranslation)
            {
                haxeText = Lang.Class.t.untranslated(text);
            }
            else
            {
                haxeText = text.ToHaxeString();
            }

            var uiText = Assets.Class.makeText(haxeText, null, true, null);
            uiText.set_textColor(CreateColor.ColorFromHex(colorHex));
            uiText.scaleX = scale;
            uiText.scaleY = scale;


            AlluiText.Add(dictionaryKey, uiText);

            var container = parentContainer ?? SelectorGui.teamFlowBox;
            if (container != null)
            {
                container.addChild(uiText);
            }

            return uiText;
        }




        public void AddConfigInfoToRightFlow()
        {
            if (SelectorGui.GetConfig == null || SelectorGui.rightFlow == null ||
            SelectorGui.mainFlow == null)
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
            var configTitle = Assets.Class.makeText(Lang.Class.t.untranslated("EnemiesVsEnemiesMod-" + $"Virsion:{EnemiesVsEnemiesMod.Virsion()}"), null, true, null);
            configTitle.set_textColor(CreateColor.ColorFromHex("#ffffff"));
            configTitle.set_textAlign(new Align.Center());
            configTitle.scaleX = 1;
            configTitle.scaleY = 1;

            AlluiText.Add("configTitle", configTitle);
            SelectorGui.rightFlow.addChild(configTitle);
        }


        private void BuildAndAddTeamFlowBox(ModConfig config)
        {
            double teamPadH = 5.0;
            double teamPadV = 5.0;
            SelectorGui.teamFlowBox = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref teamPadH),
            Ref<double>.From(ref teamPadV), Ref<bool>.Null, null);
            SelectorGui.teamFlowBox.set_isVertical(true);
            SelectorGui.teamFlowBox.set_horizontalAlign(new FlowAlign.Middle());
            SelectorGui.teamFlowBox.set_verticalAlign(new FlowAlign.Middle());

            string currentTeamId = SelectorGui.UserSelectedteamid;
            if (string.IsNullOrEmpty(currentTeamId))
                return;

            if (config.Teams.TryGetValue(currentTeamId, out var team))
            {
                if (!team.Name.IsNullOrEmpty() && !team.Id.IsNullOrEmpty())
                {
                    CreataTexts(team);
                }
            }

            SelectorGui.rightFlow.addChild(SelectorGui.teamFlowBox);
        }


        public void CreataTexts(TeamConfig team)
        {
            var teamsTitle = Assets.Class.makeText(Lang.Class.t.untranslated("[队伍列表]"), null, true, null);
            teamsTitle.set_textColor(CreateColor.ColorFromHex("#ffffff"));
            AlluiText.Add("teamsTitle", teamsTitle);
            SelectorGui.teamFlowBox.addChild(teamsTitle);

            string teamInfo = $"- {team.Name} (ID: {team.Id})";
            CreateAndAddText(teamInfo, team.Id, "#eeff00", 1.3);


            string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
            CreateAndAddText(opposingInfo, team.Id + "opposingText", "#ff0000", 1.1);


            string enemiesText = $"队伍暴徒: {string.Join(", ", team.OpposingTeamIds)}";
            CreateAndAddText(enemiesText, team.Id + "enemiesText", "#787cff", 1);
        }



        public void UpdateTeamDisplay()
        {
            if (SelectorGui.GetConfig == null || SelectorGui.teamFlowBox == null) return;

            var config = SelectorGui.GetConfig.Value;
            if (config == null) return;

            string currentTeamId = SelectorGui.UserSelectedteamid;
            if (string.IsNullOrEmpty(currentTeamId))
                return;

            if (!config.Teams.TryGetValue(currentTeamId, out var team))
                return;

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
                    teamManager.SetupTeamRelationships();
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