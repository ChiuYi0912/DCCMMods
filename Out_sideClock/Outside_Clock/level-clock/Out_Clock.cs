using dc;
using dc.haxe;
using dc.hl.types;
using dc.level;
using dc.libs;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utitities;
using Serilog;
using Log = Serilog.Log;

namespace Outside_Clock;

public class Out_Clock : LevelStruct
{

    public Out_Clock(User user, virtual_baseLootLevel_biome_bonusTripleScrollAfterBC_cellBonus_dlc_doubleUps_eliteRoomChance_eliteWanderChance_flagsProps_group_icon_id_index_loreDescriptions_mapDepth_minGold_mobDensity_mobs_name_nextLevels_parallax_props_quarterUpsBC3_quarterUpsBC4_specificLoots_specificSubBiome_transitionTo_tripleUps_worldDepth_ lInfos, Rand rng) : base(user, lInfos, rng)
    {

    }


    public override RoomNode buildMainRooms()
    {

        Rand rng = base.rng;
        double randnumber = rng.seed * 16807.0 % 2147483647.0;
        rng.seed = randnumber;
        bool randbool = ((int)randnumber & 1073741823) % 100 < 70;

        #region 入口
        RoomNode entranceNode = base.createNode(null, "RoofEntrance".AsHaxeString(), null, "start".AsHaxeString());
        entranceNode.AddFlags(new RoomFlag.NoExitSizeCheck());
        entranceNode.setConstraint(new LinkConstraint.RightOnly());
        entranceNode.addNpc(new NpcId.PlagueDoctor());



        var forcedBiome = "ClockTower".AsHaxeString();
        var virtual_add = new virtual_specificBiome_();
        virtual_add.specificBiome = forcedBiome;
        var clockTowerGenData = virtual_add.ToVirtual<virtual_altarItemGroup_brLegendaryMultiTreasure_broken_cells_doorCost_doorCurse_flaskRefill_forcedMerchantType_forcePauseTimer_isCliffPath_itemInWall_itemLevelBonus_killsMultiTreasure_locked_maxPerks_mins_noHealingShop_shouldBeFlipped_specificBiome_subTeleportTo_timedMultiTreasure_zDoorLock_zDoorType_>();


        entranceNode.addGenData(clockTowerGenData);
        #endregion

        #region 入口向右连接
        RandList rand = new RandList(new HlFunc<int, int>(base.rng.random), (ArrayDyn)null!);
        double randomValue = base.rng.random(100) / 100.0;
        RoomNode combatNode;
        if (randomValue < 0.4)
        {
            combatNode = base.createNode(null, "TU_teleportSecret".AsHaxeString(), null, "right".AsHaxeString())
                        .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
                        .setConstraint(new LinkConstraint.RightOnly());

            combatNode.set_parent(entranceNode);
        }
        else
        {
            combatNode = base.createNode("Combat".AsHaxeString(), null, null, "right".AsHaxeString())
                        .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
                        .setConstraint(new LinkConstraint.RightOnly());

            combatNode.set_parent(entranceNode);
        }

        RoomNode roomNode = base.createNode("Corridor".AsHaxeString(), null, 102, null)
        .addFlag(new RoomFlag.Outside())
        .addFlag(new RoomFlag.Holes());
        RoomNode roomNode2 = roomNode.set_parent(combatNode);

        RoomNode roomNode1 = base.createNode("Corridor".AsHaxeString(), null, 101, null)
        .addFlag(new RoomFlag.Outside())
        .addFlag(new RoomFlag.Holes());
        RoomNode roomNode21 = roomNode1.set_parent(roomNode);

        RoomNode roomNode11 = base.createNode("Teleport".AsHaxeString(), null, null, null)
        .addFlag(new RoomFlag.Outside())
        .addFlag(new RoomFlag.Holes());
        RoomNode roomNode211 = roomNode11.set_parent(roomNode1);
        #endregion




        #region 入口后向上连接

        RoomNode up = base.createNode("Teleport".AsHaxeString(), null, null, "upOnly".AsHaxeString())
           .setConstraint(new LinkConstraint.All());


        up.set_parent(entranceNode);

        #endregion






        #region 入口后向左连接

        RoomNode combatNode1 = base.createNode("Teleport".AsHaxeString(), null, null, "left".AsHaxeString())
            .addFlag(new RoomFlag.Outside())
            .addFlag(new RoomFlag.Holes())
            .setConstraint(new LinkConstraint.LeftOnly());

        combatNode1.set_parent(entranceNode);
        #endregion





        Log.Debug($"{base.all.array}");
        return base.nodes.get("start".AsHaxeString());
    }

    public override void addTeleports()
    {

    }

    #region 约束配置
    public override void finalize()
    {
        // int num = 0;
        // ArrayObj all = base.all;
        // for (; ; )
        // {
        //     int length = all.length;
        //     if (num >= length)
        //     {
        //         break;
        //     }
        //     length = all.length;
        //     RoomNode? roomNode;
        //     if (num >= length)
        //     {
        //         roomNode = null;
        //     }
        //     else
        //     {

        //         roomNode = all.array[num] as RoomNode;
        //     }
        //     num++;
        //     if (roomNode != null && roomNode.parent != null)
        //     {
        //         if ((roomNode.flags & 8) != 0)
        //         {
        //             roomNode.childPriority = 1;

        //             RoomNode roomNode2 = roomNode.setConstraint(new LinkConstraint.NeverUp());
        //         }
        //         else if ((roomNode.parent.flags & 8) != 0)
        //         {

        //             RoomNode roomNode2 = roomNode.setConstraint(new LinkConstraint.NeverUp());
        //         }
        //     }
        // }


    }
    #endregion



    public override void buildSecondaryRooms()
    {
        base.buildSecondaryRooms();

        #region 配置
        dc.String upOnlyId = "upOnly".AsHaxeString();
        dc.String a2Id = "left".AsHaxeString();
        dc.String boss = "bossbefore".AsHaxeString();

        RoomNode upOnlyNode = base.getId(upOnlyId);
        RoomNode a2Node = base.getId(a2Id);

        #endregion


        RoomNode roomNode = base.createNode("Combat".AsHaxeString(), null, null, null)
            .setConstraint(new LinkConstraint.All());

        // #region 出口前支线
        // roomNode = base.createNode("Combat".AsHaxeString(), null, null, null)
        //     .addFlag(new RoomFlag.Outside())
        //     .addBefore(exitNode, null);


        // int[] exitCombatData = new int[] { 69, 1, 7, 3, 2, 6, 1 };
        // int i = 0;
        // for (; ; )
        // {
        //     if (i >= exitCombatData.Length) break;
        //     int data = exitCombatData[i];
        //     i++;

        //     roomNode = base.createNode(null, "TU_teleportSecret".AsHaxeString(), null, null)
        //     .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
        //         .addBefore(exitNode, null);

        //     roomNode = base.createNode("Combat".AsHaxeString(), null, null, null)
        //        .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
        //         .addBefore(exitNode, null);
        // }


        // roomNode = base.createNode("Shop".AsHaxeString(), null, null, null)
        //    .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
        //     .addBefore(exitNode, null);
        // #endregion




        #region 入口向上连接支线
        int i = 0;
        for (; ; )
        {
            if (i >= 3) break;
            i++;

            RoomNode roomNodebetween1 = base.createNode("Teleport".AsHaxeString(), null, null, null)
            .AddFlags(new RoomFlag.Holes())
            .setConstraint(new LinkConstraint.UpOnly())
            .addBefore(upOnlyNode, null);


            RoomNode roomNodebetween = base.createNode("Combat".AsHaxeString(), null, null, null)
            .AddFlags(new RoomFlag.Holes())
            .setConstraint(new LinkConstraint.UpOnly())
            .addBefore(upOnlyNode, null);



            double randomValue = base.rng.random(100) / 100.0;
            if (randomValue > 0.5)
            {
                RoomNode roomNodebetween2 = base.createNode(null, "CT_VSpacer".AsHaxeString(), null, null)
                           .AddFlags(new RoomFlag.Holes())
                           .setConstraint(new LinkConstraint.UpOnly())
                           .addBefore(upOnlyNode, null);

            }
            else
            {
                RoomNode roomNodebetween2 = base.createNode("WallJumpGate".AsHaxeString(), null, null, null)
                           .AddFlags(new RoomFlag.Holes())
                           .setConstraint(new LinkConstraint.UpOnly())
                           .addBefore(upOnlyNode, null);
            }

        }


        roomNode = base.createNode(null, "BR_BossTimeKeeper".AsHaxeString(), null, "bossbefore".AsHaxeString())
         .AddFlags(new RoomFlag.Holes())
         .setConstraint(new LinkConstraint.All())
         .addBefore(upOnlyNode, null);




        RoomNode bossbefoer = base.getId(boss);
        roomNode = base.createNode(null, "CT_Key".AsHaxeString(), null, null)
           .addFlag(new RoomFlag.Holes())
           .setConstraint(new LinkConstraint.All())
           .addAfter(bossbefoer, null);

        #endregion




        RandList categoryList = new RandList(new HlFunc<int, int>(base.rng.random), null);
        categoryList.add("Combat", 2);
        categoryList.add("CursedTreasure", 2);



        roomNode = base.createNode(null, "TU_teleport0".AsHaxeString(), null, null)
            .setConstraint(new LinkConstraint.LeftOnly())
            .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())

              .addBefore(a2Node, null);



        roomNode = base.createNode(null, "SwCombat1".AsHaxeString(), null, null)
            .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
            .setConstraint(new LinkConstraint.LeftOnly())

            .addBefore(a2Node, null);


        #region 左连接支线
        int[] a2NodeLeftOnly = new int[] { 93, 93, 93 };
        i = 0;
        for (; ; )
        {
            if (i >= a2NodeLeftOnly.Length) break;
            int data = a2NodeLeftOnly[i];
            i++;
            dc.String category = categoryList.draw(null);

            roomNode = base.createNode(category, /*"TU_teleport0"*/null, null, null)
            .setConstraint(new LinkConstraint.LeftOnly())
            .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())

            .addBefore(a2Node, null);


            roomNode = base.createNode("Combat".AsHaxeString(), null, data, null)
                .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
                .setConstraint(new LinkConstraint.LeftOnly())

                .addBefore(a2Node, null);

            roomNode = base.createNode(null, "TU_teleport0".AsHaxeString(), null, null)
                .setConstraint(new LinkConstraint.LeftOnly())
                .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())

            .addBefore(a2Node, null);

        }

        roomNode = base.createNode("CursedTreasure".AsHaxeString(), null, null, null)
            .AddFlags(new RoomFlag.Outside(), new RoomFlag.Holes())
            .setConstraint(new LinkConstraint.LeftOnly())
            .addBefore(a2Node, null);

        #endregion


    }
}

