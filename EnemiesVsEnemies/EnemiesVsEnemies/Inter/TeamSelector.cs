using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.UI.Utilities;
using HaxeProxy.Runtime;
using ModCore.Serialization;
using ModCore.Storage;
using static EnemiesVsEnemies.Inter.TeamSelector;

namespace EnemiesVsEnemies.Inter
{
    public class TeamSelector : Interactive, IHxbitSerializable<InterImportant>
    {
        private InterImportant data = new();
        public class InterImportant
        {
            public string teamId = string.Empty;
            public int SaveCx;
            public int SaveCy;
        }

        InterImportant IHxbitSerializable<InterImportant>.GetData()
        {
            if (data == null)
                data = new InterImportant();
            data.teamId = Teamid;
            data.SaveCx = cx;
            data.SaveCy = cy;
            return data;
        }

        void IHxbitSerializable<InterImportant>.SetData(InterImportant data)
        {
            Teamid = data.teamId;
            cx = data.SaveCx;
            cy = data.SaveCy;
        }

        public CricketSelectorGui gui = null!;
        public static Dictionary<string, TeamSelector> TeamSelectorkeys = new();
        public TeamSelector(Level lvl, int x, int y) : base(lvl, x, y) { }


        public string Teamid = string.Empty;


        public override void initGfx()
        {
            base.initGfx();
            SpriteLib gameElements = Assets.Class.gameElements;
            base.initSprite(gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
            spr.set_visible(false);
        }

        public override void onActivate(Hero by, bool longPress)
        {
            base.onActivate(by, longPress);

            if (!EnemiesVsEnemiesMod.GetConfig().Teams.TryGetValue(Teamid, out var team) && !Teamid.IsNullOrEmpty())
                Teamid = string.Empty;


            if (Teamid.IsNullOrEmpty())
            {
                var inpt = new NumberInput(HUD.Class.ME);
                Action<string> action = (userinputid) =>
                {
                    Teamid = userinputid;
                    var config = EnemiesVsEnemiesMod.GetConfig();
                    if (config.Teams.ContainsKey(Teamid))
                    {
                        var popup = new ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                        popup.text("添加失败：\n 请输入未注册的队伍！\n".ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
                        Teamid = string.Empty;
                        return;
                    }
                    EnsureTeamConfigWithPosition(config, Teamid);
                    gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
                };
                inpt.OpenNumberInput("输入", "触发器队伍id", "Team-", action);

                return;
            }

            gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
        }

        private void EnsureTeamConfigWithPosition(ModConfig config, string id)
        {
            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
            var teamConfig = new TeamConfig(id, $"触发器队伍 {id}", 0xFFFFFF);
            teamConfig.TriggerLevelId = hero._level.map.id.ToString();
            teamConfig.TriggerX = cx;
            teamConfig.TriggerY = cy;
            EnemiesVsEnemiesMod.GetTeamManager().AddTeam(teamConfig);
            EnemiesVsEnemiesMod.config.Save();
            TeamSelectorkeys.Add(id, this);

        }

        public override void postUpdate()
        {
            base.postUpdate();
        }

        public override void onFocus()
        {
            base.onFocus();
            var lightTip = createLightTip(null);
            lightTip.distance = 24.0;
            dc.String str = "设置团队".ToHaxeString();
            lightTip.addActivate(str, null, null);
        }

        public override void destroy()
        {
            AudioHelper.LoadAudioFormString("sfx/active/active_depop.wav");
            // Fx fx = base._level.fx;
            // double x = (base.cx + base.xr) * 24.0;
            // double y = (base.cy + base.yr) * 24.0 - base.hei * 0.5;
            // double radiusScale = 1;
            // fx.solidExplosion(x, y, 0x776D3F, 0x334A6C, Ref<double>.In(radiusScale), Ref<double>.Null);
            base.destroy();
        }

        public override void onDie()
        {
            base.onDie();
        }

    }
}