using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using MoreSettings.API;
using MoreSettings.GameMechanics.CustomPopDamage;

namespace PopDamage.OtherPop
{
    internal class GradientPop : IPopDamage
    {
        public GradientPop() : base("gradient") { }
        public override int Priority => 5;

        public override double SpeedMultiplier
        {
            get => popconfig.RevealSpeedMultiplier;
            set
            {
                popconfig.RevealSpeedMultiplier = value;
                EntityPopDamage.Config.Save();
            }
        }
        public override string OptionsTitle => "GradientCritEffect";
        public override string SubStr => "GradientCritEffectDesc";
        public override bool CanHandle(AttackData a, Entity entity) => a.hasTag(2) && popconfig.RevealPop && EntityPopDamage.ForcedHandler == this;
        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            _ = new PopDamageGradient(entity, a, entity.dmgIdx, Ref<bool>.Null, EntityPopDamage.CreateFontData("hotline"));
        }
    }
}