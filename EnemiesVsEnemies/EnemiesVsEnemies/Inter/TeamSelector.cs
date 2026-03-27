using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.UI.Utilities;

namespace EnemiesVsEnemies.Inter
{
    public class TeamSelector : Interactive
    {
        public Level level = null!;
        public CricketSelectorGui gui = null!;
        public TeamSelector(Level lvl, int x, int y) : base(lvl, x, y)
        {
            level = lvl;
            xr = 0.5;
        }

        public string Teamid = string.Empty;

        public override void initGfx()
        {
            base.initGfx();
            SpriteLib gameElements = Assets.Class.gameElements;
            base.initSprite(gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
        }

        public override void onActivate(Hero by, bool longPress)
        {
            base.onActivate(by, longPress);
            if (Teamid.IsNullOrEmpty())
            {
                var inpt = new NumberInput(HUD.Class.ME);
                Action<string> action = (userinputid) =>
                {
                    Teamid = userinputid;
                    EnsureTeamConfigWithPosition(Teamid);
                    gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
                };
                inpt.OpenNumberInput("输入", "触发器队伍id", 0, action);

                return;
            }
            gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
        }

        private void EnsureTeamConfigWithPosition(string id)
        {
            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
            var config = EnemiesVsEnemiesMod.GetConfig();
            if (!config.Teams.ContainsKey(id))
            {
                var teamConfig = new TeamConfig(id, $"触发器队伍 {id}", 0xFFFFFF);
                teamConfig.TriggerLevelId = hero._level.map.id.ToString();
                teamConfig.TriggerX = cx;
                teamConfig.TriggerY = cy;
                config.Teams[id] = teamConfig;
                EnemiesVsEnemiesMod.GetTeamManager().AddTeam(teamConfig);
                EnemiesVsEnemiesMod.config.Save();
            }
        }

        public override void postUpdate()
        {
            base.postUpdate();
            if (gui == null)
                return;

            if (gui.isLockedController)
                return;

            EnemiesVsEnemiesMod.HandleKeyBindings();
            EnemiesVsEnemiesMod.Destroymobs();
        }

        public override void onFocus()
        {
            base.onFocus();
            var lightTip = base.createLightTip(null);
            lightTip.distance = 24.0;
            dc.String str = "设置团队".ToHaxeString();
            lightTip.addActivate(str, null, null);
        }
    }



}