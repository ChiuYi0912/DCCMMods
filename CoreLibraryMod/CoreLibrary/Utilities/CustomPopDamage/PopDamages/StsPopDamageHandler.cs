using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.tool.atk;
using dc.ui.popd;
using HaxeProxy.Runtime;
using ModCore.Storage;

namespace CoreLibrary.Utilities.CustomPopDamage.PopDamages
{
    internal class StsPopDamageHandler : IPopDamage
    {
        private static readonly HashSet<string> StsItems = new()
        {
            "DiverseDeckJuggernaut",
            "DiverseDeckCatalyst",
            "DiverseDeckElectro",
            "DiverseDeckWatcher"
        };
        private const string StsSkin = "SlayTheSpire";

        public override int Priority => 20;

        public override double SpeedMultiplier => popconfig.StsSpeedMultiplier;

        public override bool CanHandle(AttackData a, Entity entity)
        {
            if (a.sourceItem != null)
            {
                string itemKind = a.sourceItem.getItemKind().ToString();
                if (StsItems.Contains(itemKind))
                    return true;
            }

            if (!a.hasTag(2)) return false;

            if (popconfig.StsPopDamage) return true;

            return entity._level.game.hero.hasSkin(null, StsSkin.ToHaxeString());
        }

        public override void CreatePopDamage(AttackData a, Entity entity)
        {
            var fontData = EntityPopDamage.CreateFontData("sts");
            bool isBig = a.hasTag(2);
            PopDamageSts.Class.create(entity, a, entity.dmgIdx, Ref<bool>.From(ref isBig), fontData);
        }
    }
}