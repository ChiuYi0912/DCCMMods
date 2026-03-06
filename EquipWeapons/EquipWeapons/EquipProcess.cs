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


namespace EquipWeapons
{

    public class EquipProcess(dc.libs.Process parent) : dc.ui.Process(parent)
    {

        private const string LeftHandBone = "Bip001 L Hand";
        private const string WaistBone = "Bip001 Spine2";
        private const string PropBone = "Bip001 Prop2";
        private const string HeadBone = "headBone";

        public const int HandBox = 1;


        private const string TrackDataPath = "atlas/beheaded_tracks.json";
        private const double IconScale = 0.5;
        private const double IconYOffset = -5;
        private readonly HashSet<string> Abandoned = [
            "PrisonerCloth-I<StartItem>", "LadderKey", "TeleportKey", "ScoringKey", "BreakableGroundKey", "WallJumpKey",
            "BossRune1", "BossRune2", "BossRune3", "BossRune4", "HomKey", "CustomKey", "BossRune5", "ExploKey",
            "TrainingUnlock","Talisman0","Talisman1", "Talisman2","Talisman3", "Talisman4","Talisman5"
        ];



        public Hero GetHero { get; } = Game.Instance.HeroInstance!;
        private Serilog.ILogger? GetLogger { get; }
        public List<Icon> Icons { get; } = [];
        public FlowBox? handbox { get; set; }

        public EquipProcess(dc.libs.Process parent, ILogger log) : this(parent)
        {
            GetLogger = log;

            handbox = FlowBox.Class.createBoxMain(null, HandBox, HandBox, null);
            handbox.box.alpha = 0;
            handbox.set_horizontalAlign(new FlowAlign.Middle());
            GetHero._level.scroller.addChildAt(handbox, Const.Class.DP_ROOM_MAIN_FX);
            CreateIcon();
        }

        public override void postUpdate()
        {
            base.postUpdate();
            if (handbox != null && Icons.Count > 0)
            {
                var (x, y) = GetLHandPosition();
                handbox.x = x;
                handbox.y = y + IconYOffset;
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


        public double GetLHandX()
        {
            var (x, _) = GetLHandPosition();
            return x;
        }

        public double GetLHandY()
        {
            var (_, y) = GetLHandPosition();
            return y;
        }

        public (double X, double Y) GetLHandPosition()
        {
            var spr = GetHero.spr;
            var frame = spr.frame;
            var frameData = spr.frameData;
            var pivot = spr.pivot;
            var dir = GetHero.dir;

            var headBoneTrack = LoadTrack(HeadBone);
            double baseX = spr.x - frameData.realWid * pivot.centerFactorX * dir;
            double baseY = spr.y - frameData.realHei * pivot.centerFactorY;

            double offsetX = AnimationTrack_Impl_.Class.x(headBoneTrack, frame) * dir;
            double offsetY = AnimationTrack_Impl_.Class.y(headBoneTrack, frame);

            return (baseX + offsetX, baseY + offsetY);
        }


        public void CreateIcon()
        {
            var items = GetHero.inventory.items;

            for (int i = 0; i < items.length; i++)
            {
                var useweapon = items.getDyn(i)._itemData;
                if (useweapon == null)
                    continue;
                if (Abandoned.Contains(useweapon.id.ToString()))
                    continue;

                GetLogger?.Debug($"{useweapon}");

                var allicons = Icon.Class.createItemIcon(useweapon.id, null);
                allicons.scaleX = IconScale;
                allicons.scaleY = IconScale;
                if (handbox != null)
                    handbox.addChild(allicons);
                Icons.Add(allicons);
            }
        }

        public override void onDispose()
        {
            base.onDispose();
            foreach (var icon in Icons)
            {
                icon.parent?.removeChild(icon);
            }
            Icons.Clear();

            if (handbox != null)
            {
                handbox.parent?.removeChild(handbox);
                handbox = null;
            }
        }
    }
}