using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.en.inter;
using dc.en.inter.npc;
using dc.hxbit.enumSer;
using dc.level;
using dc.pr;
using dc.tool.log;
using Serilog;
using NpcId = dc.NpcId;
using RoomFlagsProps = Hashlink.Virtuals.virtual_gameplayFlags_genFlags_lootFlags_metaFlags_visualFlags_;

namespace MoreSettings.GameMechanics
{
    internal class NPCManager
    {
        private static readonly Dictionary<NpcId.Indexes, Func<Level, Room, Npc>> customAttachFactories = new();
        private static readonly Dictionary<NpcId.Indexes, Action<Npc>> customAttachCallbacks = new();

        private static readonly Dictionary<string, Action<Level, Room, Marker, int, int>> customSpawnActions = new();

        public NPCManager() { Initialize(); }

        public static void RegisterAttachNpc(NpcId.Indexes id, Func<Level, Room, Npc> factory, Action<Npc> onCreated)
        {
            customAttachFactories[id] = factory;
            if (onCreated != null)
                customAttachCallbacks[id] = onCreated;
            else
                customAttachCallbacks.Remove(id);
        }

        public static void RegisterSpawnNpc(string markerId, Action<Level, Room, Marker, int, int> spawnAction, Action<Npc> onCreated)
        {
            customSpawnActions[markerId.ToLowerInvariant()] = spawnAction;
        }

        public void Initialize()
        {
            Hook_Level.attachNpcs += Hook_Level_attachNpcs;
            Hook_Level.spawnNpc += Hook_Level_spawnNpc;
        }

        private void Hook_Level_spawnNpc(Hook_Level.orig_spawnNpc orig, Level self, Room r, Marker m, int cx, int cy)
        {
            orig(self, r, m, cx, cy);
            //spawnNpc(self, r, m, cx, cy);
        }

        private void Hook_Level_attachNpcs(Hook_Level.orig_attachNpcs orig, Level self, Room r)
        {
            attachNpcs(r, self);
        }


        public void attachNpcs(Room r, Level level)
        {

            var npcFactories = new Dictionary<NpcId.Indexes, Func<Level, Room, Npc>>
            {
                [NpcId.Indexes.Knight] = (lvl, room) => new Knight(lvl, room),
                [NpcId.Indexes.Scribe] = (lvl, room) => new Scribe(lvl, room),
                [NpcId.Indexes.Collector] = (lvl, room) => new Collector(lvl, room),
                [NpcId.Indexes.Docker] = (lvl, room) => new Docker(lvl, room),
                [NpcId.Indexes.PlagueDoctor] = (lvl, room) => new PlagueDoctor(lvl, room),
                [NpcId.Indexes.SewerCreature] = (lvl, room) => new SewerCreature(lvl, room),
                [NpcId.Indexes.CryptDemon] = (lvl, room) => new CryptDemon(lvl, room),

                [NpcId.Indexes.Sign] = (lvl, room) => null!,
                [NpcId.Indexes.Berserk] = (lvl, room) => null!,
                [NpcId.Indexes.ScoringGuy] = (lvl, room) => new ScoringGuy(lvl, room),
                [NpcId.Indexes.InternMerchant] = (lvl, room) => new InternMerchant(lvl, room),
                [NpcId.Indexes.Blacksmith] = (lvl, room) => new Blacksmith(lvl, room),
                [NpcId.Indexes.PerkMaster] = (lvl, room) => new PerkMaster(lvl, room),
                [NpcId.Indexes.SmallBlacksmith] = (lvl, room) => new SmallBlacksmith(lvl, room),
                [NpcId.Indexes.ChallengeGuy] = (lvl, room) => new ChallengeGuy(lvl, room),
                [NpcId.Indexes.Tailor] = (lvl, room) => new Tailor(lvl, room),
                [NpcId.Indexes.GlitchedKnight] = (lvl, room) => new GlitchedKnight(lvl, room),
                [NpcId.Indexes.TickPriest] = (lvl, room) => new TickPriest(lvl, room),
                [NpcId.Indexes.CollectorIntern] = (lvl, room) => new CollectorIntern(lvl, room),

                [NpcId.Indexes.TrainingKnight] = (lvl, room) => new TrainingKnight(lvl, room, false),
                [NpcId.Indexes.TrainingKnightBoss] = (lvl, room) => new TrainingKnightBoss(lvl, room),
                [NpcId.Indexes.AspectMaster] = (lvl, room) => new AspectMaster(lvl, room),
                [NpcId.Indexes.Fisherfish] = (lvl, room) => new Fisherfish(lvl, room),
                [NpcId.Indexes.Banker] = (lvl, room) => new Banker(lvl, room),
                [NpcId.Indexes.PiggyBankKid] = (lvl, room) => new PiggyBankKid(lvl, room),
                [NpcId.Indexes.GuillainLibrarian] = (lvl, room) => new GuillainLibrarian(lvl, room),
                [NpcId.Indexes.GuillainMimic] = (lvl, room) => new GuillainMimic(lvl, room),
                [NpcId.Indexes.GuillainHidden] = (lvl, room) => new GuillainHidden(lvl, room),
                [NpcId.Indexes.SoulKnightBug] = (lvl, room) => new SoulKnightBug(lvl, room),
                [NpcId.Indexes.BossRushNPC] = (lvl, room) => new BossRushNPC(lvl, room),
                [NpcId.Indexes.Architect] = (lvl, room) => new Architect(lvl, room),
                [NpcId.Indexes.SlayTheSpireNeow] = (lvl, room) => new SlayTheSpireNeow(lvl, room),
                [NpcId.Indexes.AlucardNpc] = (lvl, room) => new AlucardNpc(lvl, room),
                [NpcId.Indexes.RichterNpc] = (lvl, room) => new RichterNpc(lvl, room),
                [NpcId.Indexes.CollectorShanoa] = (lvl, room) => new CollectorShanoa(lvl, room),
                [NpcId.Indexes.Maria] = (lvl, room) => new Maria(lvl, room),
                [NpcId.Indexes.TailorDaughter] = (lvl, room) => new TailorDaughter(lvl, room)
            };

            foreach (var kv in customAttachFactories)
                npcFactories[kv.Key] = kv.Value;

            foreach (NpcId npcId in r.npcs.AsEnumerable())
            {
                if (npcFactories.TryGetValue((NpcId.Indexes)npcId.RawIndex, out var factory))
                {
                    Npc npc = factory(level, r);
                    if (npc == null)
                        return;
                    npc.init();
                    level.map.addPlatformOccupation(npc.cx, npc.cy, 5);
                    if (customAttachCallbacks.TryGetValue((NpcId.Indexes)npcId.RawIndex, out var callback))
                    {
                        try { callback(npc); }
                        catch (Exception ex) { Log.Error(ex, "自定义NPC创建回调出错"); }
                    }
                }
            }
        }



        public void spawnNpc(Level level, Room r, Marker m, int cx, int cy)
        {
            if (m.customId == null)
                return;

            string id = m.customId.ToString();
            Log.Debug($"spawn:{id}");

            if (customSpawnActions.TryGetValue(id, out var customAction))
            {
                customAction(level, r, m, cx, cy);
                return;
            }

            switch (id)
            {
                case "architect":
                    new Architect(level, r).init();
                    break;

                case "aspect":
                    if (level.game.user.itemMeta.hasUnlockedItem("TrainingUnlock".ToHaxeString()))
                        new AspectMaster(level, r).init();
                    break;

                case "banker":
                    new Banker(level, r).init();
                    break;

                case "blacksmith":
                    new Blacksmith(level, r).init();
                    break;

                case "bossrushnpc":
                    new BossRushNPC(level, r).init();
                    break;

                case "collec":
                case "collector":
                    HandleCollectorSpawning(level, r, cx, cy);
                    break;

                case "fisherfish":
                    new Fisherfish(level, r).init();
                    break;

                case "ghost":
                    new ChallengeGuy(level, r).init();
                    break;

                case "guillainhidden":
                    new GuillainHidden(level, r).init();
                    break;

                case "guillainmimic":
                    new GuillainMimic(level, r).init();
                    break;

                case "librarian":
                    new GuillainLibrarian(level, r).init();
                    break;

                case "maria":
                    new Maria(level, r).init();
                    break;

                case "perk":
                    new PerkMaster(level, r).init();
                    break;

                case "piggybankkid":
                    new PiggyBankKid(level, r).init();
                    break;

                case "smallblacksmith":
                    if (level.game.data.cgData == null || !level.game.data.cgData.forgeUndergroundDisabled)
                    {
                        var story = level.game.user.story;
                        if (story.counters.get("sawForge".ToHaxeString()) == 1)
                            new SmallBlacksmith(level, r).init();
                    }
                    break;

                case "soulknightbug":
                    new SoulKnightBug(level, r).init();
                    break;

                case "stsneow":
                    new SlayTheSpireNeow(level, r).init();
                    break;

                case "tailor":
                    new Tailor(level, r).init();
                    break;

                case "tailordaughter":
                    new TailorDaughter(level, r).init();
                    break;

                case "trainingknightboss":
                    if (level.game.isTraining())
                        new TrainingKnightBoss(level, r).init();
                    break;

                default:
                    if (id.Contains("collec", StringComparison.OrdinalIgnoreCase))
                    {
                        HandleCollectorSpawning(level, r, cx, cy);
                    }else
                    {
                        Log.Error("An error occurred whilst creating the NPC");
                    }
                    break;
            }
        }

        private void HandleCollectorSpawning(Level level, Room r, int cx, int cy)
        {
            var flags = level.map.infos.flagsProps;
            bool hasAnyFlag = CheckAnyFlag(flags);

            if (hasAnyFlag)
            {
                double ox = cx;
                int cyAdjusted = level.map.toCeilY(cx, cy, true);
                int param = level.map.toCeilY(cx, cy, true);
                HiddenTrigger trigger = new HiddenTrigger(level, ox, cy - param, 5, param);
                trigger.init();
                trigger.genericEventId = "autoValidBP".ToHaxeString();
            }
            else if (level.map.infos.dlc.ToString() == "Purple")
            {
                new CollectorShanoa(level, r).init();
            }
            else if (level.game.user.br_numActivated() != 5)
            {
                new Collector(level, r).init();
            }
            else
            {
                new CollectorIntern(level, r).init();
            }
        }

        private bool CheckAnyFlag(RoomFlagsProps flags)
        {
            if (flags.genFlags != 0) return true;
            if (flags.lootFlags != 0) return true;
            if (flags.gameplayFlags != 0) return true;
            if (flags.metaFlags != 0) return true;
            if (flags.visualFlags != 0) return true;
            return false;
        }
    }

    public class CustomNPCId : dc.NpcId.Berserk
    {

    }
}