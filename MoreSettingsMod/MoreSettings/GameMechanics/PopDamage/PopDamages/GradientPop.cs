using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using MoreSettings.API;
using MoreSettings.GameMechanics.CustomPopDamage;

namespace PopDamage.OtherPop
{
    internal class GradientPop : IPopDamage
    {
        public GradientPop() : base("gradient", 0.8) { }
        public override int Priority => 5;
        public override string OptionsTitle => "GradientCritEffect";
        public override string SubStr => "GradientCritEffectDesc";
        public override bool CanHandle(AttackData a, Entity entity) => a.hasTag(2) && EntityPopDamage.ForcedHandler == this;
        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            _ = new PopDamageGradient(entity, a, entity.dmgIdx, Ref<bool>.Null, EntityPopDamage.CreateFontData("hotline"));
        }
    }
}