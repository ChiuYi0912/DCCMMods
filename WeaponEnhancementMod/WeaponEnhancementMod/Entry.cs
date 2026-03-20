using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.libs.heaps.slib;
using dc.tool;
using dc.tool.mod;
using dc.tool.skill;
using dc.tool.weap;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using WeaponEnhancementMod.Configuration;
using Math = System.Math;

namespace WeaponEnhancementMod;

public class Entry(ModInfo info) : ModBase(info),
    IOnGameEndInit

{
    public static Config<WeaponCDB> GetConfig = new("SnakeFang");


    private const double GridSize = 24.0;
    private const double HeightThreshold = 288.0;
    private const double HalfFactor = 0.5;
    private const double VerticalOffset = 2.4;
    private const int EffectId = 13;
    private const int EffectDuration = 2;
    private const double DefaultAlpha = 1.0;
    private const double DefaultSec = 1.0;
    private const int OffsetAmount = 5;
    private const double DirectionOffset = 1.2;
    private const double Friction = 0.87;
    private const double SlowMoTimescale = 0.1;
    private const double SlowMoDuration = 0.2;
    private const int DashParam = 2;
    private const double DistanceDivisor = 48.0;
    private const int DistanceSqBonus = 50;
    private string ColorRed = GetConfig.Value.ColorRed;
    private string ColorDarkRed = GetConfig.Value.ColorDarkRed;
    private string ColorLightPink = GetConfig.Value.ColorLightPink;
    private const double HeadOffsetFactor = 1.0;
    private const double One = 1.0;
    private const double DefaultVolume = 1.0;
    private const double DefaultPitch = 1.0;

    public override void Initialize()
    {
        base.Initialize();
        Hook_SnakeFang.teleportTo += Hook_SnakeFang_teleportTo;
        Hook_TargetHelper.filterBySight += Hook_TargetHelper_filterBySight;
        Hook_SnakeFang.onExecute += Hook_SnakeFang_onExecute;
    }


    private bool Hook_SnakeFang_onExecute(Hook_SnakeFang.orig_onExecute orig, SnakeFang self)
    {
        isnowall = true;
        return orig(self);
    }

    public bool isnowall = false;
    private void Hook_TargetHelper_filterBySight(Hook_TargetHelper.orig_filterBySight orig, TargetHelper self, Entity otherSource, Ref<bool> ignoreOneWay, int? ignoreSpotType)
    {
        if (isnowall)
            return;
        orig(self, otherSource, ignoreOneWay, ignoreSpotType);
    }

    private void Hook_SnakeFang_teleportTo(Hook_SnakeFang.orig_teleportTo orig, SnakeFang self, Entity e)
    {
        Hero hero = self.owner;

        double targetWorldX = ((double)e.cx + e.xr) * GridSize - (double)self.owner.dir * (e.radius + self.owner.radius);
        double targetWorldY;
        if (e.hei > HeightThreshold)
            targetWorldY = ((double)e.cy + e.yr) * GridSize - e.hei * HalfFactor - VerticalOffset;
        else
            targetWorldY = ((double)e.cy + e.yr) * GridSize - VerticalOffset;

        int gridX = (int)(targetWorldX / GridSize);
        int gridY = (int)(targetWorldY / GridSize);
        double offsetX = (targetWorldX - gridX * GridSize) / GridSize;
        double offsetY = (targetWorldY - gridY * GridSize) / GridSize;


        double oldHeadX = hero.get_headX();
        double oldHeadY = hero.get_headY();


        int newDir;
        if (e == null)
            newDir = hero.dir;
        else
        {
            double ownerCenterX = ((double)hero.cx + hero.xr) * GridSize;
            double targetCenterX = ((double)e.cx + e.xr) * GridSize;
            newDir = targetCenterX < ownerCenterX ? -1 : 1;
        }


        var map = hero._level.map;


        bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= map.wid || y < 0 || y >= map.hei) return false;
            int idx = y * map.wid + x;
            if (idx >= map.collisions.length) return false;
            int coll = map.collisions.getDyn(idx);
            return (coll & 1) == 0;
        }


        if (!IsWalkable(gridX, gridY))
        {

            if (IsWalkable(gridX, gridY - 1))
            {
                gridY--;
                offsetX = 0.5 + 0.4 * newDir;
            }
            else if (IsWalkable(gridX - newDir, gridY))
            {
                gridX -= newDir;
                offsetX = 0.5 + 0.4 * newDir;
            }
            else
                return;

        }

        if (IsWalkable(gridX, gridY + 1))
        {
            gridY++;
        }

        hero.safeTpTo(gridX, gridY, offsetX, offsetY, true);

        isnowall = true;

        hero.setAffectS(EffectId, EffectDuration, Ref<double>.Null, null);

        var animId = self.get_curSkillInf().animId;
        int hitFrame = self.get_curSkillInf().hitFrame - 1;
        var tile = hero.spr.lib.getTile(animId, Ref<int>.From(ref hitFrame), Ref<double>.Null, Ref<double>.Null, null).clone();
        tile.dx = (int)-(tile.width * HalfFactor);
        tile.dy = -tile.height;

        var onionSkin = OnionSkin.Class.fromEntity(hero, null, CreateColor.ColorFromHex(ColorRed), Ref<double>.In(DefaultAlpha), Ref<double>.In(DefaultSec), Ref<bool>.Null, Ref<bool>.Null, Ref<double>.Null);
        double offsetAmount = -hero.dir * OffsetAmount;
        onionSkin.offset(offsetAmount, 0.0);
        onionSkin.dx = hero.dir * DirectionOffset;
        onionSkin.ds = 0.0;
        onionSkin.frict = Friction;

        Boot.Class.ME.slowMo(SlowMoTimescale, SlowMoDuration, null, null);

        if (e != null)
            self.owner._level.fx.dash(self.owner, -self.owner.dir, CreateColor.ColorFromHex(ColorDarkRed), Ref<int>.In(GetDistanceSq(self.owner, e)), Ref<double>.In(DashParam));

        double startX = oldHeadX + hero.dir * GridSize;
        double startY = oldHeadY;
        double endX = targetWorldX;
        double endY = targetWorldY;

        self.owner._level.fx.entityTeleport(
            startX, startY,
            endX, endY,
            CreateColor.ColorFromHex(ColorLightPink),
            Ref<bool>.In(false),
            Ref<bool>.In(true),
            Ref<bool>.In(true)
        );

        self.owner._level.lAudio.playEventOn(self.tpSfx, self.owner, DefaultVolume, DefaultPitch, null);

        int GetDistanceSq(Entity a, Entity b)
        {
            double sub = One / DistanceDivisor;
            double dx = (a.cx + a.xr) - (b.cx + b.xr);
            double dy = (a.cy + a.yr - a.hei * sub) - (b.cy + b.yr - b.hei * sub);
            return (int)(dx * dx + dy * dy) + DistanceSqBonus;
        }
    }


    void IOnGameEndInit.OnGameEndInit()
    {
        var res = Info.ModRoot!.GetFilePath("res.pak");
        FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
        var json = CDBManager.Class.instance.getAlteredCDB();
        dc.Data.Class.loadJson(
           json,
           default);
    }


}
