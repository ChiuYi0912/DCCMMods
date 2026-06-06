using dc;
using dc.tool.atk;
using HaxeProxy.Runtime;
using MoreSettings.API;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class DefaultPopDamageHandler : IPopDamage
    {
        public DefaultPopDamageHandler() : base("default", 0) { damageData.unique = true; }
        public override int Priority => int.MaxValue;
        public override string OptionsTitle => "Classic";
        public override string SubStr => "";
        public override bool CanHandle(AttackData a, Entity entity) => true;

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            _ = new dc.ui.PopDamage(entity, a, entity.dmgIdx, Ref<bool>.Null, null);
        }
    }
}