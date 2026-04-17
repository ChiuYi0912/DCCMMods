using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace CoreLibrary.Utilities.CustomPopDamage.PopDamages
{
    internal class DefaultPopDamageHandler : IPopDamageHandler
    {

        public override int Priority => int.MaxValue;

        public override double SpeedMultiplier => 450.0;

        public override bool CanHandle(AttackData a, Entity entity) => true;

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            dc.ui.PopDamage.Class.create(entity, a, entity.dmgIdx, Ref<bool>.Null, null);
        }
    }
}