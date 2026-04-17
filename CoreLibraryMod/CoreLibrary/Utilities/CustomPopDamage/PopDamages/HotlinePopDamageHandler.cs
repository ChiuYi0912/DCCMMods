using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.tool.atk;
using dc.tool.weap;
using dc.ui.popd;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace CoreLibrary.Utilities.CustomPopDamage.PopDamages
{
    internal class HotlinePopDamageHandler : IPopDamage
    {
        private static readonly HashSet<string> HotlineSkins = new()
        {
            "HotlineMiamiChicken",
            "HotlineMiamiHorse",
            "HotlineMiamiOwl"
        };

        public override int Priority => 10;

        public override double SpeedMultiplier => popconfig.HotlineSpeedMultiplier;

        public override bool CanHandle(AttackData a, Entity entity)
        {
            if (!a.hasTag(2)) return false;

            if (popconfig.HotlinePopDamage) return true;

            if (a.sourceWeapon != null && Std.Class.@is(a.sourceWeapon, BaseballBat.Class)) return true;

            return HotlineSkins.Any(skin => entity._level.game.hero.hasSkin(null, skin.ToHaxeString()));
        }

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            var fontData = EntityPopDamage.CreateFontData("hotline");
            PopDamageHotline.Class.create(entity, a, entity.dmgIdx, Ref<bool>.Null, fontData);
        }
    }
}