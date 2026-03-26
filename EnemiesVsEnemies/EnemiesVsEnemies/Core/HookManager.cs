using CoreLibrary.Core.Extensions;
using dc;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.en.mob;
using dc.en.mob.boss;
using dc.h2d.col;
using dc.hl.types;
using dc.hxd.res;
using dc.tool;
using dc.tool.bossRush;
using EnemiesVsEnemies.Configuration;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Modules;
using ModCore.Utilities;

namespace EnemiesVsEnemies.Core
{
    public class HookManager
    {
        private readonly TeamManager GetteamManager;
        private readonly Configuration.ModConfig GetModconfig;


        public HookManager(TeamManager teamManager, Configuration.ModConfig config)
        {
            GetteamManager = teamManager;
            GetModconfig = config;
        }

        public void InitializeHooks()
        {
            Hook_Hero.canBeHitBy += Hook_Hero_canBeHitBy;
            dc.en.Hook_Mob.init += Hook_Mob_init;
            Hook_MeetCollector.onComplete += Hook_MeetCollector_onComplete;
            Hook_Throne.inBossBattle += Hook_Throne_inBossBattle;
            Hook_Golem.setAttackTarget += Hook_Golem_setAttackTarget;
            Hook_Behemoth.behaviourAi += Hook_Behemoth_behaviourAi;
            dc.en.Hook_Mob.setNemesisTarget += Hook_Mob_setNemesisTarget;
            dc.en.Hook_Mob.tpHeroBackToTraining += Hook_Mob_tpHeroBackToTraining;
            Hook__MusicManager.get += Hook__MusicManager_get;
        }




        private Sound Hook__MusicManager_get(Hook__MusicManager.orig_get orig, dc.String musicName, dc.String folder)
        {
            if (musicName == null)
                musicName = "music\\default\\lighthouse_ambiance_bg.ogg".ToHaxeString();
            return orig(musicName, folder);
        }


        public void CleanupHooks()
        {
            Hook_Hero.canBeHitBy -= Hook_Hero_canBeHitBy;
            dc.en.Hook_Mob.init -= Hook_Mob_init;
            Hook_MeetCollector.onComplete -= Hook_MeetCollector_onComplete;
            Hook_Throne.inBossBattle -= Hook_Throne_inBossBattle;
            Hook_Golem.setAttackTarget -= Hook_Golem_setAttackTarget;
            Hook_Behemoth.behaviourAi -= Hook_Behemoth_behaviourAi;
            dc.en.Hook_Mob.setNemesisTarget -= Hook_Mob_setNemesisTarget;
            dc.en.Hook_Mob.tpHeroBackToTraining -= Hook_Mob_tpHeroBackToTraining;
        }





        private bool Hook_Hero_canBeHitBy(Hook_Hero.orig_canBeHitBy orig, Hero self, dc.Entity by)
        {
            return false;
        }

        private void Hook_Mob_init(dc.en.Hook_Mob.orig_init orig, dc.en.Mob self)
        {
            orig(self);
            if (GetModconfig.General.AutoSetEnemyTeams)
            {
                var heroTeam = Game.Instance.HeroInstance?._team;
                bool shouldSetTeam = self._team != heroTeam;


                foreach (var teamId in GetteamManager.GetTeamIds())
                {
                    var team = GetteamManager.GetTeam(teamId);
                    if (self._team == team)
                    {
                        shouldSetTeam = false;
                        break;
                    }
                }

                if (shouldSetTeam)
                {
                    if (GetteamManager.TryGetTeam("TeamB", out var defaultTeam))
                    {
                        self.set_team(defaultTeam);
                    }
                }
            }

            if (GetModconfig.General.BossCameraTrackingDisabled && self is Boss boss)
            {
                boss.cameraTrackingDisabled = true;
            }
        }

        private void Hook_MeetCollector_onComplete(Hook_MeetCollector.orig_onComplete orig, MeetCollector self)
        {
            orig(self);
        }

        private bool Hook_Throne_inBossBattle(Hook_Throne.orig_inBossBattle orig, Throne self)
        {
            throw new System.NotImplementedException();
        }

        private void Hook_Mob_tpHeroBackToTraining(dc.en.Hook_Mob.orig_tpHeroBackToTraining orig, dc.en.Mob self)
        {
            self.collisionMode = new CollisionMode.WallGrab();
            if (self is Behemoth || self is Queen || self is TimeKeeper)
            {
                return;
            }
            orig(self);
        }

        private void Hook_Mob_setNemesisTarget(dc.en.Hook_Mob.orig_setNemesisTarget orig, dc.en.Mob self, Entity e)
        {
            if (e == Game.Instance.HeroInstance)
            {
                var team = self._team;
                var th = team.get_targetHelper();
                th.filterUntargetables();
                e = th.getBest();

                orig(self, th.getBest());
                return;
            }
            orig(self, e);
        }

        private void Hook_Behemoth_behaviourAi(Hook_Behemoth.orig_behaviourAi orig, Behemoth self)
        {
            var at = self.aTarget;

            if (at == Game.Instance.HeroInstance)
            {
                self.aTarget = null;
                self.clearNemesisTarget();
                return;
            }

            self.nemesisTarget = at;
            self.cameraTrackingDisabled = true;
            orig(self);
        }

        private void Hook_Golem_setAttackTarget(Hook_Golem.orig_setAttackTarget orig, Golem self, dc.Entity t)
        {
            var on = self.nemesisTarget;
            orig(self, t);
            self.nemesisTarget = on;
        }

        public void ReinitializeHooks() => InitializeHooks();
    }
}