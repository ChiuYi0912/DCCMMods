using CoreLibrary.Core.Extensions;
using dc;
using dc.tool.atk;
using dc.ui.popd;
using HaxeProxy.Runtime;
using MoreSettings.API;

namespace MoreSettings.GameMechanics.CustomPopDamage
{
    internal class StsPopDamageHandler : IPopDamage
    {
        public StsPopDamageHandler() : base("sts", 0.8) { }

        private const string StsSkin = "SlayTheSpire";
        public override int Priority => 20;
        public override string OptionsTitle => "StsCritEffect";
        public override string SubStr => "";
        public override bool CanHandle(AttackData a, Entity entity)
        {
            if (a.sourceItem != null && EntityPopDamage.StsItems.Contains(a.sourceItem.getItemKind().ToString())) return true;

            if (!a.hasTag(2)) return false;

            return entity._level.game.hero.hasSkin(null, StsSkin.ToHaxeString());
        }
        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            var fontData = EntityPopDamage.CreateFontData("sts");
            _ = new PopDamageSts(entity, a, entity.dmgIdx, Ref<bool>.In(a.hasTag(2)), fontData);
        }
    }
}