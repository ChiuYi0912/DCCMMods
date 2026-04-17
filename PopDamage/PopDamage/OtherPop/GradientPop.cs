using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Utilities.CustomPopDamage;
using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using PopDamage.Override;

namespace PopDamage.OtherPop
{
    public class GradientPop : IPopDamage
    {
        public override int Priority => 5;

        public override double SpeedMultiplier => 3000;

        public override bool CanHandle(AttackData a, Entity entity)
        {
            return a.hasTag(2) && EntityPopDaamage.GetConfig.Value.RevealPop;
        }

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            var fontData = EntityPopDamage.CreateFontData("hotline");
            _ = new PopDamageGradient(entity, a, entity.dmgIdx, Ref<bool>.Null, fontData);
        }
    }
}