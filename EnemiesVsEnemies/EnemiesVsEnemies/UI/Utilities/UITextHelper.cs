using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Utilities;
using dc;
using dc.h2d;
using dc.ui;
using EnemiesVsEnemies.Configuration;
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
            var teamsTitle = Assets.Class.makeText(Lang.Class.t.untranslated("当前队伍:"), null, true, null);
            teamsTitle.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            AlluiText.Add("teamsTitle", teamsTitle);
            SelectorGui.teamFlowBox.addChild(teamsTitle);
        }

        private void AddAllTeamsToBox(ModConfig config)
        {
            Tile selectionTile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);
            var selectionTM = new ScaleGrid(selectionTile, 8, 8, null);

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


        private void AddTeamEntry(TeamConfig team)
        {
            string teamInfo = $"- {team.Name} (ID: {team.Id})";
            var teamText = Assets.Class.makeText(Lang.Class.t.untranslated(teamInfo), null, true, null);
            teamText.alpha = 0.5;
            teamText.scaleX = 1.3;
            teamText.scaleY = 1.3;
            teamText.set_textColor(team.TeamColor);

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
                teamText.alpha = 1.0;
            };
            interactive.onOut = (e) =>
            {
                teamText.alpha = 0.5;
            };
        }


        private void AddOpposingTeamsInfo(TeamConfig team)
        {
            if (team.OpposingTeamIds != null && team.OpposingTeamIds.Count > 0)
            {
                string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                var opposingText = Assets.Class.makeText(Lang.Class.t.untranslated(opposingInfo), null, true, null);
                opposingText.set_textColor(CreateColor.ColorFromHex("#ffffff"));
                opposingText.scaleX = 1.1;
                opposingText.scaleY = 1.1;

                AlluiText.Add(team.Id + "opposingText", opposingText);
                SelectorGui.teamFlowBox.addChild(opposingText);
            }
        }


        private void AddDefaultEnemiesPlaceholder(TeamConfig team)
        {
            if (team.DefaultEnemies != null && team.DefaultEnemies.Count > 0)
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
                if (AlluiText.TryGetValue(enemiesKey, out var enemiesText))
                {
                    string newEnemiesInfo = GenerateEnemiesInfo(team);
                    double scale = CalculateFontScale(newEnemiesInfo);
                    enemiesText.scaleX = scale;
                    enemiesText.scaleY = scale;
                    enemiesText.set_text(Lang.Class.t.get(newEnemiesInfo.ToHaxeString(), null));
                    enemiesText.posChanged = true;
                }
            }


            SelectorGui.teamFlowBox.reflow();
            SelectorGui.rightFlow.reflow();
        }


        private string GenerateEnemiesInfo(TeamConfig team)
        {
            if (team.DefaultEnemies == null || team.DefaultEnemies.Count == 0)
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

        private static double CalculateFontScale(string text)
        {
            int length = text.Length;

            if (length <= 50)
                return 1.0;
            else if (length <= 600)
                return 0.9;
            else if (length <= 800)
                return 0.8;
            else if (length <= 1000)
                return 0.7;
            else
                return 0.6;
        }
    }
}