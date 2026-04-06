using System.Runtime.InteropServices;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Core.Utilities;
using dc;
using dc.cine;
using dc.en;
using dc.en.inter;
using dc.en.mob;
using dc.en.mob.boss;
using dc.h2d.col;
using dc.hl.types;
using dc.hxbit;
using dc.hxd.res;
using dc.hxd.snd;
using dc.level;
using dc.libs;
using dc.pr;
using dc.tool;
using dc.tool.bossRush;
using dc.ui.pause;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Inter;
using EnemiesVsEnemies.Interfaces;
using EnemiesVsEnemies.Level;
using EnemiesVsEnemies.UI.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog;

namespace EnemiesVsEnemies.Core
{
    public class HookManager :
    IOnHookInitialize,
    IEventReceiver
    {
        private readonly TeamManager GetteamManager;
        private readonly ModConfig GetModconfig;


        public HookManager(TeamManager teamManager, ModConfig config)
        {
            EventSystem.AddReceiver(this);
            GetteamManager = teamManager;
            GetModconfig = config;
        }


        void IOnHookInitialize.HookInitialize()
        {
            Hook_Hero.canBeHitBy += Hook_Hero_canBeHitBy;
            dc.en.Hook_Mob.init += Hook_Mob_init;
            Hook_Throne.inBossBattle += Hook_Throne_inBossBattle;
            Hook_Golem.setAttackTarget += Hook_Golem_setAttackTarget;
            Hook_Behemoth.behaviourAi += Hook_Behemoth_behaviourAi;
            dc.en.Hook_Mob.setNemesisTarget += Hook_Mob_setNemesisTarget;
            dc.en.Hook_Mob.tpHeroBackToTraining += Hook_Mob_tpHeroBackToTraining;
            Hook__Active.create += Hook__Active_create;
            Hook_Entity.spriteUpdate += Hook_Entity_sprupdate;
            Hook_Team.unserialize += Hook_Team_unserialize;
            Hook__LevelStruct.get += Hook_LevelStruct__get;
            Hook_Boot.mainLoop += Hook_Boot_loop;
            Hook_Level.attachSpecialEquipments += hook_Level_attachSpecialEquipments;
            Hook_LevelAudio.addAmbientLoop += Hook_LevelAudio_addAmbientLoop;
        }

        private Channel Hook_LevelAudio_addAmbientLoop(Hook_LevelAudio.orig_addAmbientLoop orig, LevelAudio self, Sound snd, double? volume)
        {
            if (snd == null)
                snd = MusicManager.Class.get("music/default/lighthouse_ambiance_bg.ogg".ToHaxeString(), null);
            return orig(self, snd, volume);
        }

        private void hook_Level_attachSpecialEquipments(Hook_Level.orig_attachSpecialEquipments orig, dc.pr.Level self, Room r, Rand rseed, LevelTransition cineTrans)
        {
            orig(self, r, rseed, cineTrans);
            if (r == null) return;
            Marker marker = r.getMarker("SpecialEquipment".AsHaxeString(), null, Ref<bool>.In(false));
            if (marker != null && marker.customId != null)
            {
                if (marker.customId.ToString().EqualsIgnoreCase("queenspr"))
                {
                    EnemiesVsEnemiesMod.LogInfo("");
                }
            }
        }

        private void Hook_Boot_loop(Hook_Boot.orig_mainLoop orig, Boot self)
        {
            int type = 0;
            #if DEBUG
            type = 1;
            #endif
            switch (type)
            {
                case 0:
                    orig(self);
                    break;
                case 1:
                    try
                    {
                        orig(self);
                    }
                    catch (System.Exception ex)
                    {
                        EnemiesVsEnemiesMod.GetLogger.Error("{ex}", ex);
                        throw;
                    }
                    break;
                default:
                    break;
            }

        }

        private LevelStruct Hook_LevelStruct__get(Hook__LevelStruct.orig_get orig, User user, virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ l, Rand rng)
        {
            if (l.id.ToString().EqualsIgnoreCase("DebugRTC"))
                return new BattleLevel(user, l, rng);

            return orig(user, l, rng);
        }

        private void Hook_Team_unserialize(Hook_Team.orig_unserialize orig, Team self, Serializer __ctx)
        {
            orig(self, __ctx);
            if (self.opponentsIterator == null)
                self.init();
        }


        private void Hook_Entity_sprupdate(Hook_Entity.orig_spriteUpdate orig, Entity self)
        {
            if (self is dc.en.Mob)
                EventSystem.BroadcastEvent<IOnBeforMobSprInit, dc.en.Mob>((dc.en.Mob)self);
            orig(self);
        }

        private Active Hook__Active_create(Hook__Active.orig_create orig, Hero from, Grenade g, InventItem ii)
        {
            dynamic kind = ii.kind;
            string id = kind.Param0.ToString();
            if (id.EqualsIgnoreCase("DummyActive"))
            {
                var active = new DummyActive(from, g.cx, g.cy, ii);
                active.init();
                return active;
            }
            return orig(from, g, ii);
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
                var heroTeam = ModCore.Modules.Game.Instance.HeroInstance?._team;
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
            }

            if (GetModconfig.General.BossCameraTrackingDisabled && self is Boss boss)
            {
                boss.cameraTrackingDisabled = true;
            }
        }

        private bool Hook_Throne_inBossBattle(Hook_Throne.orig_inBossBattle orig, Throne self)
        {
            return true;
        }

        private void Hook_Mob_tpHeroBackToTraining(dc.en.Hook_Mob.orig_tpHeroBackToTraining orig, dc.en.Mob self)
        {
            if (self is Behemoth || self is Queen || self is TimeKeeper)
            {
                return;
            }
            orig(self);
        }

        private void Hook_Mob_setNemesisTarget(dc.en.Hook_Mob.orig_setNemesisTarget orig, dc.en.Mob self, Entity e)
        {
            if (e == ModCore.Modules.Game.Instance.HeroInstance)
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

            if (at == ModCore.Modules.Game.Instance.HeroInstance)
            {
                self.aTarget = null;
                self.clearNemesisTarget();
                return;
            }

            self.nemesisTarget = at;
            self.cameraTrackingDisabled = false;
            orig(self);
        }

        private void Hook_Golem_setAttackTarget(Hook_Golem.orig_setAttackTarget orig, Golem self, dc.Entity t)
        {
            var on = self.nemesisTarget;
            orig(self, t);
            self.nemesisTarget = on;
        }
    }
}