using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using MoreSettings.GameMechanics.CustomPopDamage;
using MoreSettings.GameMechanics.Customs;

namespace PopDamage.OtherPop
{
    public class GradientPop : IPopDamage
    {
        public override int Priority => 5;

        public override double SpeedMultiplier => 3000;

        public override bool CanHandle(AttackData a, Entity entity)
        {
            if (!a.hasTag(2)) return false;

            return popconfig.RevealPop;
        }

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            _ = new PopDamageGradient(entity, a, entity.dmgIdx, Ref<bool>.Null, EntityPopDamage.CreateFontData("hotline"));
        }
    }
}