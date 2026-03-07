using System;
using System.Collections.Generic;
using dc;
using dc.hxd;
using Serilog;
using dc.en;
using dc.hxd.res;
using dc.ui.icon;
using dc.hl.types;
using ModCore.Utilities;
using ModCore.Modules;
using dc.tool._AnimationTrack;
using dc.ui;
using HaxeProxy.Runtime;
using dc.h2d;
using dc.tool;


namespace EquipWeapons
{

    public class EquipProcess(dc.libs.Process parent) : dc.ui.Process(parent)
    {
        private const string TrackDataPath = "atlas/beheaded_tracks.json";
        private const double IconScale = 0.5;
        private const double IconYOffset = -5;

        private HashSet<string> ActiveItem = new();
        public List<Icon> WeaponIcons { get; } = [];
        public List<Icon> ActiveIcons { get; } = [];
        private Dictionary<string, ArrayBytes_Int> _trackCache = new();


        public Hero GetHero { get; } = null!;
        private Serilog.ILogger? GetLogger { get; }
        private ArrayObj items { get; } = null!;

        public EquipProcess(dc.libs.Process parent, ILogger log, Hero hero) : this(parent)
        {
            GetLogger = log;
            GetHero = hero;
            items = items = GetHero.inventory.items;
            CreateWeaponIcons();
            CreateActiveIcons();
        }

        public override void postUpdate()
        {
            base.postUpdate();

            if (Checkenitem())
                UpdateIcons();


            if (WeaponIcons.Count > 0)
            {
                var (leftX, leftY) = GetLPosition(BoneNames.FingerBones.Left.Finger0);
                var (rightX, rightY) = GetLPosition(BoneNames.FingerBones.Right.Finger0);

                if (WeaponIcons.Count >= 1)
                {
                    WeaponIcons[0].x = leftX + IconYOffset;
                    WeaponIcons[0].y = leftY + IconYOffset;
                }

                if (WeaponIcons.Count >= 2)
                {
                    WeaponIcons[1].x = rightX + IconYOffset;
                    WeaponIcons[1].y = rightY + IconYOffset;
                }
            }

            if (ActiveIcons.Count > 0)
            {
                var (neckX, neckY) = GetLPosition(BoneNames.FootBones.Left.Main);
                var (Thighx, Thighy) = GetLPosition(BoneNames.FootBones.Right.Main);

                if (ActiveIcons.Count >= 1)
                {
                    ActiveIcons[0].x = neckX + IconYOffset;
                    ActiveIcons[0].y = neckY + IconYOffset;
                }

                if (ActiveIcons.Count >= 2)
                {
                    ActiveIcons[1].x = Thighx + IconYOffset;
                    ActiveIcons[1].y = Thighy + IconYOffset;
                }
            }
        }

        public ArrayBytes_Int LoadTrack(string trackname)
        {
            var hero = GetHero.spr;
            var animationTracks = Assets.Class.getAnimationTracks(Res.Class.get_loader().loadCache(TrackDataPath.AsHaxeString(), Resource.Class));
            var animationGroup = animationTracks.get(hero.groupName);
            var track = animationGroup.get(trackname.AsHaxeString()) as ArrayBytes_Int;
            if (track == null)
                return null!;

            return track;
        }


        public (double X, double Y) GetLPosition(string BONENAME)
        {
            var spr = GetHero.spr;
            if (spr == null)
                return (0, 0);
            var frame = spr.frame;
            var frameData = spr.frameData;
            var pivot = spr.pivot;
            var dir = GetHero.dir;

            var headBoneTrack = LoadTrack(BONENAME);
            double baseX = spr.x - frameData.realWid * pivot.centerFactorX * dir;
            double baseY = spr.y - frameData.realHei * pivot.centerFactorY;

            double offsetX = AnimationTrack_Impl_.Class.x(headBoneTrack, frame) * dir;
            double offsetY = AnimationTrack_Impl_.Class.y(headBoneTrack, frame);

            return (baseX + offsetX, baseY + offsetY);
        }


        public bool Checkenitem()
        {
            var currentIds = new HashSet<string>();

            for (int i = 0; i < items.length; i++)
            {
                var item = items.getDyn(i);
                var itemData = item._itemData;
                var kind = item.kind;
                if (itemData == null)
                    continue;

                if (kind is not InventItemKind.Weapon && kind is not InventItemKind.Active)
                    continue;

                currentIds.Add(itemData.id.ToString());
            }

            if (currentIds.Count != ActiveItem.Count)
                return true;

            foreach (var id in currentIds)
                if (!ActiveItem.Contains(id))
                    return true;

            return false;
        }

        public void UpdateIcons()
        {
            DisposeIcons();
            CreateWeaponIcons();
            CreateActiveIcons();
        }


        public void CreateWeaponIcons()
        {
            for (int i = 0; i < items.length; i++)
            {
                var useweapon = items.getDyn(i)._itemData;

                if (useweapon == null)
                    continue;
                if (items.getDyn(i).kind is not InventItemKind.Weapon)
                    continue;

                var allicons = Icon.Class.createItemIcon(useweapon.id, null);
                allicons.scaleX = IconScale;
                allicons.scaleY = IconScale;

                GetHero._level.scroller.addChildAt(allicons, Const.Class.DP_ROOM_MAIN_FX);

                WeaponIcons.Add(allicons);
                ActiveItem.Add(useweapon.id.ToString());
            }
        }

        public void CreateActiveIcons()
        {
            for (int i = 0; i < items.length; i++)
            {
                var useweapon = items.getDyn(i)._itemData;

                if (useweapon == null)
                    continue;
                if (items.getDyn(i).kind is not InventItemKind.Active)
                    continue;

                var allicons = Icon.Class.createItemIcon(useweapon.id, null);
                allicons.scaleX = IconScale;
                allicons.scaleY = IconScale;

                GetHero._level.scroller.addChildAt(allicons, Const.Class.DP_ROOM_MAIN_FX);

                ActiveIcons.Add(allicons);
                ActiveItem.Add(useweapon.id.ToString());
            }
        }

        public override void onDispose()
        {
            base.onDispose();
            DisposeIcons();
        }

        public void DisposeIcons()
        {
            foreach (var icon in WeaponIcons)
                icon.parent?.removeChild(icon);
            foreach (var icon in ActiveIcons)
                icon.parent?.removeChild(icon);

            WeaponIcons.Clear();
            ActiveIcons.Clear();
            ActiveItem.Clear();
        }
    }
}