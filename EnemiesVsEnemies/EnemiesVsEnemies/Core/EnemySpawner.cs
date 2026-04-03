using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.en.inter;
using dc.en.mob;
using dc.en.mob.boss;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using dc.tool._Cooldown;
using dc.tool.utils;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Interfaces;
using EnemiesVsEnemies.UI.Utilities;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Modules;
using ModCore.Utilities;
using Serilog.Core;


namespace EnemiesVsEnemies.Core
{
    public class EnemySpawner :IOnBeforMobSprInit, IEventReceiver
    {
        private readonly TeamManager GetteamManager;
        private readonly ModConfig GetModconfig;
        public List<Mob> CreatedMobs = new();

        public EnemySpawner(TeamManager teamManager, ModConfig config)
        {
            EventSystem.AddReceiver(this);
            GetteamManager = teamManager;
            GetModconfig = config;
        }

        //(配置创建Mob)
        public void SpawnDefaultEnemiesForTeam(string teamId)
        {
            if (!GetModconfig.Teams.TryGetValue(teamId, out var teamConfig))
            {
                throw new KeyNotFoundException($"队伍配置 '{teamId}' 不存在");
            }

            foreach (var enemyPresetId in teamConfig.DefaultEnemies)
            {

                SpawnEnemy(teamId, enemyPresetId.Value);
            }
        }

        //(创建Mob)
        public void SpawnEnemy(string teamId, EnemySpawnConfig spawnConfig)
        {
            var hero = ModCore.Modules.Game.Instance.HeroInstance;
            if (hero == null)
                return;

            var team = GetteamManager.GetTeam(teamId);

            Level spawnLevel = hero._level;
            int spawnX = hero.cx;
            int spawnY = hero.cy;

            if (GetModconfig.Teams.TryGetValue(teamId, out var teamConfig))
            {
                if (teamConfig.HasTriggerPosition)
                {
                    string currentLevelId = hero._level.map.id.ToString();
                    if (!string.IsNullOrEmpty(currentLevelId) && currentLevelId == teamConfig.TriggerLevelId)
                    {
                        spawnX = teamConfig.TriggerX;
                        spawnY = teamConfig.TriggerY;
                    }
                    else
                    {
                        EnemiesVsEnemiesMod.GetLogger.Error($"队伍 {teamId} 触发器在关卡 {teamConfig.TriggerLevelId}，当前在 {currentLevelId}，使用玩家位置");
                    }
                }
            }

            for (int i = 0; i < spawnConfig.SpawnCount; i++)
            {
                var lifeTier = spawnConfig.LifeTier;
                var lifeTierRef = Ref<int>.From(ref lifeTier);

                var mob = Mob.Class.create(
                    spawnConfig.EnemyType.AsHaxeString(),
                    spawnLevel,
                    spawnX,
                    spawnY,
                    spawnConfig.DamageTier,
                    lifeTierRef
                );
                mob.init();
                mob.set_team(team);
                if (spawnConfig.IsElite)
                    mob.setElite(true);
                mob.spr.visible = false;
                if (mob is Boss boss)
                {
                    boss.setReady();
                }
                mob.cd.fastCheck.set(350312, null);
                CreatedMobs.Add(mob);
            }
        }

        void IOnBeforMobSprInit.BeforMobSprScaleUpdate(Mob mob)
        {
            if (!mob.cd.fastCheck.exists(350312))
                return;

            double sprx = mob.sprScaleX;
            double spry = mob.sprScaleY;
            mob.sprScaleX = mob.sprScaleY = 0;
            mob.spr.visible = true;
            UIAnimHelper.doScaleAnimation(mob.tw, mob, sprx, spry, 500);
            mob.cd.fastCheck.remove(350312);
        }



        public void SpawnEnemies(string teamId, List<EnemySpawnConfig> spawnConfigs)
        {
            foreach (var spawnConfig in spawnConfigs)
            {
                SpawnEnemy(teamId, spawnConfig);
            }
        }


        public void SpawnEnemyByPreset(string teamId, string presetId)
        {
            if (!GetModconfig.EnemyPresets.TryGetValue(presetId, out var spawnConfig))
            {
                EnemiesVsEnemiesMod.GetLogger.Error($"敌人生成预设 '{presetId}' 不存在");
                return;
            }
            SpawnEnemy(teamId, spawnConfig);
        }

        //(创建金块)
        public void SpawnGoldNugget(int value = 10)
        {
            var hero = ModCore.Modules.Game.Instance.HeroInstance;
            if (hero == null)
            {
                return;
            }

            var goldNugget = new GoldNugget(hero._level, hero.cx, hero.cy + 20, value);
            goldNugget.init();
        }

        //(创建引力场)
        public dc.en.inter.ForceField? SpawnForceField(bool open = false)
        {
            var hero = ModCore.Modules.Game.Instance.HeroInstance;
            if (hero == null)
            {
                return null;
            }

            var forceField = new ForceField(hero._level, hero.cx, hero.cy, false);
            forceField.init();

            if (open)
            {
                forceField.open();
            }

            return forceField;
        }


        public List<string> GetAvailableEnemyPresets()
        {
            return new List<string>(GetModconfig.EnemyPresets.Keys);
        }


        public void AddEnemyPreset(string presetId, EnemySpawnConfig spawnConfig)
        {
            GetModconfig.EnemyPresets[presetId] = spawnConfig;
        }

        public bool RemoveEnemyPreset(string presetId)
        {
            return GetModconfig.EnemyPresets.Remove(presetId);
        }

        public void UpdateEnemyPreset(string presetId, EnemySpawnConfig spawnConfig)
        {
            if (!GetModconfig.EnemyPresets.ContainsKey(presetId))
            {
                EnemiesVsEnemiesMod.GetLogger.Error($"敌人生成预设 '{presetId}' 不存在");
                return;
            }
            GetModconfig.EnemyPresets[presetId] = spawnConfig;
        }
    }
}