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

        public NumberInput inputTeamID = null!;
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
            var config = SelectorGui.GetConfig.Value;

            SelectorGui.mainFlow.set_horizontalAlign(new FlowAlign.Middle());
            SelectorGui.mainFlow.set_verticalAlign(new FlowAlign.Bottom());

            BuildAndAddTeamFlowBox(config);
            UpdateTeamDisplay();
        }


        private void BuildAndAddTeamFlowBox(ModConfig config)
        {
            SelectorGui.title.set_text($"EnemiesVsEnemiesMod-Virsion:{EnemiesVsEnemiesMod.GetVersion()}".ToHaxeString());

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
                    string teamInfo = $"- {team.Name} (ID: {team.Id})";
                    CreateAndAddText(teamInfo, team.Id, "#eeff00", 2);


                    string opposingInfo = $"仇恨队伍: {string.Join(", ", team.OpposingTeamIds)}";
                    CreateAndAddText(opposingInfo, team.Id + "opposingText", "#ff0000", 1.1);
                }
            }

            SelectorGui.rightFlow.addChild(SelectorGui.teamFlowBox);
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

            SelectorGui.teamFlowBox.reflow();
            SelectorGui.rightFlow.reflow();
        }






        public void AddOpposingTeamFromGui(TeamManager teamManager)
        {
            SelectorGui.LockController(true);

            inputTeamID = new NumberInput(SelectorGui);

            var action = new Action<string>((value) =>
            {
                SelectorGui.LockController(false);
                var teamconfig = SelectorGui.GetConfig.Value.Teams;
                if (teamconfig.TryGetValue(SelectorGui.UserSelectedteamid, out var team))
                {
                    if (!teamconfig.ContainsKey(value))
                    {
                        var popup = new dc.ui.ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                        popup.text("添加失败：\n 请添加已注册的队伍！\n".ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
                        return;
                    }
                    team.OpposingTeamIds.Add(value);
                    SelectorGui.GetConfig.Save();
                    teamManager.SetupTeamRelationships();
                }
                inputTeamID.Input.close();
                UpdateTeamDisplay();

            });
            dc.ui.TextInput inpu = inputTeamID.OpenNumberInput("输入", "该仇恨方队伍唯一ID", "Team-", action);

        }
    }
}