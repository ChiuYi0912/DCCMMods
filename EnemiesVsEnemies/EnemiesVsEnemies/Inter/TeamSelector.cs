using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using CoreLibrary.Utilities;
using dc;
using dc.en;
using dc.en.inter.button;
using dc.libs.heaps.slib;
using dc.pr;
using dc.ui;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.UI;
using EnemiesVsEnemies.UI.Utilities;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Serialization;
using ModCore.Storage;
using ModCore.Utilities;
using static EnemiesVsEnemies.Inter.TeamSelector;

namespace EnemiesVsEnemies.Inter
{
    public class TeamSelector : Interactive,
    IHxbitSerializable<InterImportant>,
    IHxbitSerializeCallback
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



        public static Dictionary<string, TeamSelector> TeamSelectorkeys = new();
        public TeamSelector(Level lvl, int x, int y) : base(lvl, x, y) { }

        public string Teamid = string.Empty;
        public const int Isdestroyed = 99;

        public CricketSelectorGui gui = null!;
        public NumberInput input = null!;


        public override void init()
        {
            base.init();
            if (!EnemiesVsEnemiesMod.GetConfig().Teams.ContainsKey(Teamid) && !Teamid.IsNullOrEmpty())
                Teamid = string.Empty;
        }
        public override void initGfx()
        {
            base.initGfx();
            base.initSprite(Assets.Class.gameElements, "switchBiomeMobs".ToHaxeString(), null, null, null, null, null, null);
            spr.set_visible(true);
        }

        public override void onActivate(Hero by, bool longPress)
        {
            base.onActivate(by, longPress);
            ShowEnemiesOptsions.LockContoreLible(true);
            _level.fx.activateATSwitch(this, CreateColor.ColorFromHex("#ff0000"));
            if (Teamid.IsNullOrEmpty())
            {
                input = new NumberInput();
                Action<string> action = (userinputid) =>
                {
                    Teamid = userinputid;
                    var config = EnemiesVsEnemiesMod.GetConfig();
                    if (config.Teams.ContainsKey(Teamid))
                    {
                        var popup = new ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                        popup.text(GetText.Instance.GetString("Add failed:\n Please enter unregistered team ID!\n").ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
                        Teamid = string.Empty;
                        return;
                    }
                    EnsureTeamConfigWithPosition(Teamid);
                    gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
                    input.Input.close();
                };
                input.OpenNumberInput(HUD.Class.ME, GetText.Instance.GetString("Input"), GetText.Instance.GetString("Trigger team id"), "Team-", action);
                return;
            }
            gui = new CricketSelectorGui(EnemiesVsEnemiesMod.GetTeamManager(), Teamid);
        }

        private void EnsureTeamConfigWithPosition(string id)
        {
            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
            var teamConfig = new TeamConfig(id, $"{GetText.Instance.GetString("Trigger team ")}{id}", 0xFFFFFF);
            teamConfig.TriggerLevelId = hero._level.map.id.ToString();
            teamConfig.TriggerX = cx;
            teamConfig.TriggerY = cy;
            EnemiesVsEnemiesMod.GetTeamManager().AddTeam(teamConfig);
            EnemiesVsEnemiesMod.config.Save();
            TeamSelectorkeys.Add(id, this);
        }

        public override void postUpdate()
        {
            if (ShowEnemiesOptsions.IsControllerLocked() || Teamid.IsNullOrEmpty())
                return;

            if (ControllerHelper.ControlsUpdateFromProcess(Boot.Class.ME.controller, ShowEnemiesOptsions.SpawnEnemyTriggerAct))
            {
                EnemiesVsEnemiesMod.GetEnemySpawner().SpawnDefaultEnemiesForTeam(Teamid);
            }
            base.postUpdate();
        }

        public override void onFocus()
        {
            base.onFocus();
            dc.String str = !Teamid.IsNullOrEmpty()
            ? GetText.Instance.GetString("Team id:").Add_TwoHaxeStrings(Teamid)
            : GetText.Instance.GetString("Set team").ToHaxeString();

            lightTip = createLightTip(null);
            lightTip.distance = 24.0;
            lightTip.addActivate(str, null, null);
        }

        public override void destroy()
        {
            base.destroy();
            if (cd.fastCheck.exists(Isdestroyed))
                return;

            AudioHelper.LoadAudioFormString("sfx/active/active_depop.wav");
            Fx fx = _level.fx;
            double x = (base.cx + base.xr) * 24.0;
            double y = (base.cy + base.yr) * 24.0 - base.hei * 0.5;
            double radiusScale = 1;
            fx.solidExplosion(x, y, 0x776D3F, 0x334A6C, Ref<double>.In(radiusScale), Ref<double>.Null);
        }

        void IHxbitSerializeCallback.OnBeforeSerializing()
        {
            cd.fastCheck.set(Isdestroyed, null);
            #if DEBUG
            EnemiesVsEnemiesMod.GetLogger.Information($"存在10086:{cd.fastCheck.exists(Isdestroyed)}");
            EnemiesVsEnemiesMod.GetLogger.Information("Before serializing TeamSelector.");
            #endif
        }

        void IHxbitSerializeCallback.OnAfterDeserializing()
        {

        }
    }
}