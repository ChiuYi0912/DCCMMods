using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class DefaultPopDamageHandler : IPopDamage
    {
        public DefaultPopDamageHandler() : base("default") { }

        public override int Priority => int.MaxValue;
        public override double SpeedMultiplier { get; set; } = 1.0;
        public override string OptionsTitle => "Classic";
        public override string SubStr => "";

        public override bool CanHandle(AttackData a, Entity entity) => true;

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            dc.ui.PopDamage.Class.create(entity, a, entity.dmgIdx, Ref<bool>.Null, null);
        }
    }
}