using dc;
using dc.cine;
using dc.pr;
using ModCore.Mods;
using ModCore.Utitities;
using RandomLevel.Utitities;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using System.Runtime.InteropServices;

namespace RandomLevel;

public class Main : ModBase,
    IOnHeroUpdate,
    IOnAfterLoadingCDB

{

    [DllImport("DoorProcessor.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int RandomRange(int min, int max);



    private static readonly List<string> allLevels = new List<string>
    {
        "PrisonCourtyard", "SewerShort", "PrisonDepths", "PrisonCorrupt", "PrisonRoof",
        "Ossuary", "SewerDepths", "Bridge", "BeholderPit", "StiltVillage",
        "AncientTemple", "Cemetery", "ClockTower", "Crypt", "TopClockTower",
        "Cavern", "Giant", "Castle", "Distillery", "Throne",
        "Astrolab", "Observatory", "BoatDock", "Greenhouse",
        "Swamp", "SwampHeart", "Tumulus", "Cliff", "GardenerStage",
        "Shipwreck", "Lighthouse", "QueenArena", "Bank", "PurpleGarden",
        "DookuCastle", "DookuCastleHard", "DeathArena", "DookuArena"
    };

    public Main(ModInfo info) : base(info)
    {
    }

    public override void Initialize()
    {
        Logger.Information("随机模组初始化......");
        dc.cine.Hook__LevelTransition.@goto += dc_cine_LevelTransition;
    }

    private LevelTransition dc_cine_LevelTransition(Hook__LevelTransition.orig_goto orig, dc.String id)
    {
        string levelId = id.ToString();
        if (allLevels.Contains(levelId))
        {
            int randomIndex = RandomRange(0, 38);
            string randomLevelId = allLevels[randomIndex];
            Logger.Information($"随机替换关卡: {levelId} -> {randomLevelId}");
            synchronisation(levelId, randomLevelId);
            return orig(randomLevelId.AsHaxeString());
        }

        return orig(id);
    }

    void IOnHeroUpdate.OnHeroUpdate(double dt)
    {
        if (dc.hxd.Key.Class.isPressed(39))
        {
        }
    }

    public class MobData
    {
        public dynamic mobAtkTier { get; set; } = null!;
        public dynamic mobLifeTier { get; set; } = null!;

        public dynamic extraAtkTiers { get; set; } = null!;
        public dynamic extraLifeTier { get; set; } = null!;
    }
    public MobData mobData = new();
    public void synchronisation(string original, string levelid)
    {
        var data = dc.Data.Class.difficulty;
        var dyn = data.all;

        int dif = Game.Class.ME.user.br_getDifficulty();

        Logger.Debug($"当前难度为：{dif}");
        dynamic levels = dyn.array.getDyn(dif);
        dynamic levelstatings = levels.levelSettings;

        for (int i = 0; i < levelstatings.length; i++)
        {
            var level = levelstatings.array[i];
            if (level.level.ToString() == original)
            {
                mobData.mobAtkTier = level.mobAtkTier;
                mobData.mobLifeTier = level.mobLifeTier;
                mobData.extraAtkTiers = level.extraAtkTiers;
                mobData.extraLifeTier = level.extraLifeTier;
                break;
            }
        }


        for (int i = 0; i < levelstatings.length; i++)
        {
            var level = levelstatings.array[i];
            if (level.level.ToString() == levelid)
            {
                level.mobAtkTier = mobData.mobAtkTier;
                level.mobLifeTier = mobData.mobLifeTier;
                level.extraAtkTiers = mobData.extraAtkTiers;
                level.extraLifeTier = mobData.extraLifeTier;
                break;
            }
        }

    }

    void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
    {
        var level = cdb.level.all.array;

        for (int i = 0; i < level.length; i++)
        {
            var door = cdb.level.all.getDyn(i);
            door.name = "????";
            var proprs = door.props;
            proprs.doorColor = CreateCL.ColorFromHex("#ffd6d6");
        }

    }
}