using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Utilities.CustomPopDamage.PopDamages;
using dc;
using dc.tool._Cooldown;
using dc.tool.atk;
using Hashlink.Virtuals;
using ModCore.Mods;
using ModCore.Storage;

namespace CoreLibrary.Utilities.CustomPopDamage
{
    public class EntityPopDamage
    {
        private const int CRITICAL_HIT_TAG = 2;
        private const int MAX_DAMAGE_INDEX = 3;
        private const int COOLDOWN_KEY_1 = 356515840;
        private const int COOLDOWN_KEY_2 = 272629760;
        private const double COOLDOWN_MULTIPLIER_1 = 0.3;
        private const double COOLDOWN_MULTIPLIER_2 = 0.4;
        private const double MILLISECONDS_FACTOR = 1000.0;
        private const double FLOOR_DIVISOR = 1000.0;
        private const double ZERO_FRAMES = 0.0;

        public Config<PopConfig> Config = new("CustomPopDamage");
        public static PopConfig popconfig =null!;
        public static IPopDamageHandler handler = null!;


        public EntityPopDamage(ModBase modBase)
        {
            popconfig = Config.Value;
            
            PopDamageHandlerRegistry.Register(new HotlinePopDamageHandler());
            PopDamageHandlerRegistry.Register(new StsPopDamageHandler());
            PopDamageHandlerRegistry.Register(new DefaultPopDamageHandler());

            Hook_Entity.popDamage += Hook_Entity_PopDamage;
            var basePop = new BasePopDamage();
        }

        private void Hook_Entity_PopDamage(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
        {
            if (dc.ui.Console.Class.ME.flags.exists(dc.ui.Console.Class.HIDE_UI) &&
                !dc.ui.Console.Class.ME.flags.exists("forcePopDmg".ToHaxeString()))
                return;

            handler = PopDamageHandlerRegistry.GetHandler(a, self);
            handler?.CreatePopDamage(a, self);

            UpdateCooldownsAndIndex(self);
        }


        public static void UpdateCooldownsAndIndex(Entity entity)
        {
            double frames1 = CalculateCooldownFrames(entity.cd.baseFps, COOLDOWN_MULTIPLIER_1);
            UpdateCooldown(COOLDOWN_KEY_1, frames1, entity);

            double frames2 = CalculateCooldownFrames(entity.cd.baseFps, COOLDOWN_MULTIPLIER_2);
            UpdateCooldown(COOLDOWN_KEY_2, frames2, entity);

            entity.dmgIdx++;
            if (entity.dmgIdx > MAX_DAMAGE_INDEX)
                entity.dmgIdx = 0;
        }

        private static double CalculateCooldownFrames(double baseFps, double multiplier)
        {
            double frames = baseFps * multiplier * MILLISECONDS_FACTOR;
            frames = System.Math.Floor(frames) / FLOOR_DIVISOR;
            return frames;
        }

        private static void UpdateCooldown(int cooldownKey, double frames, Entity entity)
        {
            var cdInst = entity.cd.fastCheck.get(cooldownKey);
            if (cdInst != null)
            {
                cdInst.frames = frames;
            }
            else
            {
                var newCdInst = new CdInst(cooldownKey, frames);
                entity.cd.fastCheck.set(cooldownKey, newCdInst);
                entity.cd.cdList.push(newCdInst);
            }
        }

        public static virtual_chars_font_ CreateFontData(string fontType) => new virtual_chars_font_
        {
            chars = "numbers".ToHaxeString(),
            font = fontType.ToHaxeString()
        };
    }
}