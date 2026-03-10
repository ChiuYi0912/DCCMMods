using System.Collections.Generic;

namespace LightOpt.Core.Utilities
{

    public static class GameConstants
    {
        public static class Mod
        {
            public const string ModId = "Midjourney";

            public const string ModName = "Midjourney";

            public const string ModMainClass = "Midjourney.EntryPoint.ModInitializer";

            public const string ResourcePack = "Midjourney.pak";

            public const string ConfigDirectory = "Configs/";
        }

        public static class Levels
        {
            public const string BackGarden = "BackGarden";

            public const string BackGardenDisplayName = "BackGarden";

            public const string BackGardenBiome = "BackGarden";

            public const string BackGardenDynamicBiome = "BackGarden_Outside";

            public const string BackGardenResourcePath = "BackGardenAsstes/";


            public const string AncientTemple = "AncientTemple";
            public const string CastleAlchemy = "CastleAlchemy";
            public const string CastleTorture = "CastleTorture";
            public const string GardenerStage = "GardenerStage";
            public const string LighthouseTop = "LighthouseTop";
            public const string PrisonCorrupt = "PrisonCorrupt";
            public const string RichterCastle = "RichterCastle";
            public const string SkinningBiome = "SkinningBiome";
            public const string TopClockTower = "TopClockTower";
            public const string Astrolab = "Astrolab";
            public const string Cemetery = "Cemetery";
            public const string SewerOld = "SewerOld";
            public const string Bank = "Bank";
            public const string BeholderPit = "BeholderPit";
            public const string CastleVegan = "CastleVegan";
            public const string CemeteryInt = "CemeteryInt";
            public const string DookuCastle = "DookuCastle";
            public const string Observatory = "Observatory";
            public const string PrisonStart = "PrisonStart";
            public const string SecretRooms = "SecretRooms";
            public const string BossRushZone = "BossRushZone";
            public const string PrisonDepths = "PrisonDepths";
            public const string PurpleGarden = "PurpleGarden";
            public const string StiltVillage = "StiltVillage";
            public const string Bridge = "Bridge";
            public const string Castle = "Castle";
            public const string Cavern = "Cavern";
            public const string Throne = "Throne";
            public const string BridgeBoatDock = "BridgeBoatDock";
            public const string Cliff = "Cliff";
            public const string Crypt = "Crypt";
            public const string Giant = "Giant";
            public const string Sewer = "Sewer";
            public const string Swamp = "Swamp";
            public const string ClockTower = "ClockTower";
            public const string DeathArena = "DeathArena";
            public const string Distillery = "Distillery";
            public const string DookuArena = "DookuArena";
            public const string Greenhouse = "Greenhouse";
            public const string PrisonRoof = "PrisonRoof";
            public const string QueenArena = "QueenArena";
            public const string SwampHeart = "SwampHeart";
            public const string LighthouseBottom = "LighthouseBottom";
            public const string PrisonCourtyard2 = "PrisonCourtyard2";
            public const string Ossuary = "Ossuary";
            public const string Tumulus = "Tumulus";
            public const string PhotoRoom = "PhotoRoom";
            public const string PrisonHub = "PrisonHub";
            public const string Shipwreck = "Shipwreck";
            public const string PrisonCourtyard = "PrisonCourtyard";
            public const string StiltVillageInt = "StiltVillageInt";
            public const string PrisonRoofCorrupt = "PrisonRoofCorrupt";
            public const string ShipwreckUnderground = "Shipwreck_underground";
            public const string Template = "Template";
            public const string TumulusInt = "TumulusInt";
        }


        public static class Biomes
        {
            public const string BackGarden = "BackGarden";
            public const string BackGardenOutside = "BackGarden_Outside";
            public const string ClockTower = "ClockTower";
            public const string Prison = "Prison";
            public const string Sewer = "Sewer";
            public const string Castle = "Castle";
            public const string Cavern = "Cavern";
            public const string Swamp = "Swamp";
            public const string Distillery = "Distillery";
            public const string Ossuary = "Ossuary";
            public const string Tumulus = "Tumulus";
            public const string Shipwreck = "Shipwreck";
            public const string StiltVillage = "StiltVillage";
            public const string Greenhouse = "Greenhouse";
            public const string Lighthouse = "Lighthouse";
            public const string Dooku = "Dooku";
            public const string Queen = "Queen";
            public const string Bank = "Bank";
            public const string Crypt = "Crypt";
            public const string Bridge = "Bridge";
            public const string Cliff = "Cliff";
            public const string DeathArena = "DeathArena";
            public const string Cemetery = "Cemetery";
            public const string AncientTemple = "AncientTemple";
            public const string Astrolab = "Astrolab";
            public const string BeholderPit = "BeholderPit";
            public const string BossRushZone = "BossRushZone";
            public const string Gardener = "Gardener";
            public const string Giant = "Giant";
            public const string Observatory = "Observatory";
            public const string PhotoRoom = "PhotoRoom";
            public const string PrisonCorrupt = "PrisonCorrupt";
            public const string PrisonCourtyard = "PrisonCourtyard";
            public const string PrisonDepths = "PrisonDepths";
            public const string PrisonRoof = "PrisonRoof";
            public const string PurpleGarden = "PurpleGarden";
            public const string RichterCastle = "RichterCastle";
            public const string SecretRooms = "SecretRooms";
            public const string SkinningBiome = "SkinningBiome";
            public const string SwampHeart = "SwampHeart";
            public const string Throne = "Throne";
            public const string TopClockTower = "TopClockTower";
        }


        public static class EntityTypes
        {
            public const string Boss = "Boss";
            public const string NPC = "NPC";
            public const string Item = "Item";
            public const string Weapon = "Weapon";
            public const string Critter = "Critter";
            public const string Mob = "Mob";
        }


        public static class MarkerTypes
        {
            public const string Spawn = "Spawn";
            public const string Loot = "Loot";
            public const string Trigger = "Trigger";
            public const string Camera = "Camera";
            public const string Light = "Light";
            public const string Sound = "Sound";
            public const string Event = "Event";
        }


        public static class CustomMarkerIds
        {

        }


        public static class CustomEntityIds
        {

        }

        public static class customRoomIds
        {

        }



        public static class Colors
        {
            public const int TransparentBlack = 0x00000000;

            public const int TransparentWhite = 0x00FFFFFF;

            public const int Black = 0xFF00000;

            public const int White = 0xFFFFFFF;

            public const int Red = 0xFFFF000;

            public const int Green = 0xFF00FF0;

            public const int Blue = 0xFF61D6D;

            public const int Yellow = 0xFFFFFF0;

            public const int Cyan = 0xFF00FFF;

            public const int Magenta = 0xFFFF00F;



            public const string LogBlue = "\x1b[38;2;97;214;214m";

            public const string LogGreen = "\x1b[38;2;34;139;34m";

            public const string LogYellow = "\x1b[33m";

            public const string LogRed = "\x1b[31m";

            public const string LogReset = "\x1b[0m";
        }


        public static class ConfigNames
        {
            public const string BackGardenConfig = "BackGardenConfig";
            public const string ModSettings = "ModSettings";
            public const string WeaponConfig = "WeaponConfig";
            public const string MobConfig = "MobConfig";
        }


        public static class ResourcePaths
        {
            public const string BackGardenAssets = "BackGardenAsstes/";
            public const string Atlas = "atlas/";
            public const string Tiled = "tiled/tmx/";
        }


        public static readonly List<string> AllLevelIds = new List<string>
        {
            Levels.BackGarden,
            Levels.AncientTemple,
            Levels.CastleAlchemy,
            Levels.CastleTorture,
            Levels.GardenerStage,
            Levels.LighthouseTop,
            Levels.PrisonCorrupt,
            Levels.RichterCastle,
            Levels.SkinningBiome,
            Levels.TopClockTower,
            Levels.Astrolab,
            Levels.Cemetery,
            Levels.SewerOld,
            Levels.Bank,
            Levels.BeholderPit,
            Levels.CastleVegan,
            Levels.CemeteryInt,
            Levels.DookuCastle,
            Levels.Observatory,
            Levels.PrisonStart,
            Levels.SecretRooms,
            Levels.BossRushZone,
            Levels.PrisonDepths,
            Levels.PurpleGarden,
            Levels.StiltVillage,
            Levels.Bridge,
            Levels.Castle,
            Levels.Cavern,
            Levels.Throne,
            Levels.BridgeBoatDock,
            Levels.Cliff,
            Levels.Crypt,
            Levels.Giant,
            Levels.Sewer,
            Levels.Swamp,
            Levels.ClockTower,
            Levels.DeathArena,
            Levels.Distillery,
            Levels.DookuArena,
            Levels.Greenhouse,
            Levels.PrisonRoof,
            Levels.QueenArena,
            Levels.SwampHeart,
            Levels.LighthouseBottom,
            Levels.PrisonCourtyard2,
            Levels.Ossuary,
            Levels.Tumulus,
            Levels.PhotoRoom,
            Levels.PrisonHub,
            Levels.Shipwreck,
            Levels.PrisonCourtyard,
            Levels.StiltVillageInt,
            Levels.PrisonRoofCorrupt,
            Levels.ShipwreckUnderground,
            Levels.Template,
            Levels.TumulusInt,
        };

        public static bool IsBackGardenLevel(string levelId)
        {
            return levelId.Equals(Levels.BackGarden, System.StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsKnownLevel(string levelId)
        {
            return AllLevelIds.Contains(levelId);
        }
    }
}