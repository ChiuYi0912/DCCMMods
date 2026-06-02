using CoreLibrary.Core.Extensions;
using dc;
using dc.tool.atk;
using dc.tool.weap;
using dc.ui.popd;
using HaxeProxy.Runtime;
using MoreSettings.API;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class HotlinePopDamageHandler : IPopDamage
    {
        public HotlinePopDamageHandler() : base("hotline") { }
        private static readonly HashSet<string> HotlineSkins = new()
        {
            "HotlineMiamiChicken",
            "HotlineMiamiHorse",
            "HotlineMiamiOwl"
        };

        public override int Priority => 10;

        public override double SpeedMultiplier
        {
            get => popconfig.HotlineSpeedMultiplier;
            set
            {
                popconfig.HotlineSpeedMultiplier = value;
                EntityPopDamage.Config.Save();
            }
        }

        public override string OptionsTitle => "HotlineCritEffect";

        public override string SubStr => "";

        public override bool CanHandle(AttackData a, Entity entity)
        {
            if (!a.hasTag(2)) return false;

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