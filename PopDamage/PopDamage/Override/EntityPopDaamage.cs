using dc;
using dc.en;
using dc.tool.atk;
using dc.tool.weap;
using dc.ui.popd;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using dc.tool._Cooldown;
using PopDamage.Main.Config;
using PopDamage.OtherPop;

namespace PopDamage.Override
{
    public class EntityPopDaamage
    {
        // Constants
        private const int CRITICAL_HIT_TAG = 2;
        private const int MAX_DAMAGE_INDEX = 4;
        private const int COOLDOWN_KEY_1 = 356515840;
        private const int COOLDOWN_KEY_2 = 272629760;

        // Hardcoded strings stored in HashSet for O(1) lookups
        private static readonly HashSet<string> HOTLINE_SKINS_SET = new() {
            "HotlineMiamiChicken",
            "HotlineMiamiHorse",
            "HotlineMiamiOwl"
        };
        private static readonly HashSet<string> STS_ITEMS_SET = new() {
            "DiverseDeckJuggernaut",
            "DiverseDeckCatalyst",
            "DiverseDeckElectro",
            "DiverseDeckWatcher"
        };
        private const string STS_SKIN = "SlayTheSpire";
        private const string FORCE_POP_DMG_FLAG = "forcePopDmg";


        // Font constants
        private const string FONT_NUMBERS = "numbers";
        private const string FONT_HOTLINE = "hotline";
        private const string FONT_STS = "sts";
        private const string FONT_REVEAL = "hotline";
        
        // Cooldown calculation constants
        private const double COOLDOWN_MULTIPLIER_1 = 0.3;
        private const double COOLDOWN_MULTIPLIER_2 = 0.4;
        private const double MILLISECONDS_FACTOR = 1000.0;
        private const double FLOOR_DIVISOR = 1000.0;
        private const double ZERO_FRAMES = 0.0;

        private enum PopDamageType { Hotline, Sts, Default, Reveal }
        public static ModCore.Storage.Config<CoreConfig> GetConfig = new("CunstumPopDamage");

        public EntityPopDaamage() => Hook_Entity.popDamage += Hook_Entity_PopDamage;

        // Helper methods
        private static virtual_chars_font_ CreateFontData(string fontType) => new virtual_chars_font_
        {
            chars = FONT_NUMBERS.AsHaxeString(),
            font = fontType.AsHaxeString()
        };

        private static bool CheckHotlineSkins(Hero hero)
        {
            return HOTLINE_SKINS_SET.Any(skin => hero.hasSkin(null, skin.AsHaxeString()));
        }

        private static bool CheckStsItems(AttackData a)
        {
            if (a.sourceItem == null) return false;

            string itemKind = a.sourceItem.getItemKind().ToString();
            return STS_ITEMS_SET.Contains(itemKind);
        }



        private void Hook_Entity_PopDamage(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
        {
            if (dc.ui.Console.Class.ME.flags.exists(dc.ui.Console.Class.HIDE_UI) &&
              !dc.ui.Console.Class.ME.flags.exists(FORCE_POP_DMG_FLAG.AsHaxeString()))
                return;

            CreatePopDamageByType(a, self);
            UpdateCooldownsAndIndex(self);
        }

        private static void CreatePopDamageByType(AttackData a, Entity entity)
        {
            bool isHotline = IsHotlineDamage(a, entity);
            bool isSTS = !isHotline && IsStsDamage(a, entity);
            bool isReveal = !isHotline && !isSTS && IsRevealDamage(a);

            PopDamageType type;
            if (isHotline)
                type = PopDamageType.Hotline;
            else if (isSTS)
                type = PopDamageType.Sts;
            else if (isReveal)
                type = PopDamageType.Reveal;
            else
                type = PopDamageType.Default;

            switch (type)
            {
                case PopDamageType.Hotline:
                    var fontDataHotline = CreateFontData(FONT_HOTLINE);
                    bool HotlineisBig = a.hasTag(CRITICAL_HIT_TAG);
                    PopDamageHotline.Class.create(entity, a, entity.dmgIdx, Ref<bool>.From(ref HotlineisBig), fontDataHotline);
                    break;
                case PopDamageType.Sts:
                    var fontDataSts = CreateFontData(FONT_STS);
                    bool StsisBig = a.hasTag(CRITICAL_HIT_TAG);
                    PopDamageSts.Class.create(entity, a, entity.dmgIdx, Ref<bool>.From(ref StsisBig), fontDataSts);
                    break;
                case PopDamageType.Reveal:
                    var fontDataReveal = CreateFontData(FONT_REVEAL);
                    bool RevealisBig = a.hasTag(CRITICAL_HIT_TAG);
                    PopDamageGradient.create(entity, a, entity.dmgIdx, Ref<bool>.From(ref RevealisBig), fontDataReveal);
                    break;
                default:
                    dc.ui.PopDamage.Class.create(entity, a, entity.dmgIdx, Ref<bool>.Null, null);
                    break;
            }
        }

        private static bool IsRevealDamage(AttackData a)
        {
            if (a.hasTag(CRITICAL_HIT_TAG))
                return GetConfig.Value.RevealPop;
            else
                return false;


        }

        private static bool IsHotlineDamage(AttackData a, Entity entity)
        {
            if (!a.hasTag(CRITICAL_HIT_TAG)) return false;

            // Check for BaseballBat weapon
            if (a.sourceWeapon != null && Std.Class.@is(a.sourceWeapon, BaseballBat.Class))
                return true;

            // Check for Hotline skins
            Hero hero = entity._level.game.hero;
            return CheckHotlineSkins(hero);
        }

        private static bool IsStsDamage(AttackData a, Entity entity)
        {
            // Check for STS items
            if (CheckStsItems(a))
                return true;

            // Check for STS skin
            return entity._level.game.hero.hasSkin(null, STS_SKIN.AsHaxeString());
        }

        private static void UpdateCooldownsAndIndex(Entity entity)
        {
            double frames1 = CalculateCooldownFrames(entity.cd.baseFps, COOLDOWN_MULTIPLIER_1);
            UpdateCooldown(COOLDOWN_KEY_1, frames1, entity);

            double frames2 = CalculateCooldownFrames(entity.cd.baseFps, COOLDOWN_MULTIPLIER_2);
            UpdateCooldown(COOLDOWN_KEY_2, frames2, entity);

            UpdateDamageIndex(entity);
        }

        private static double CalculateCooldownFrames(double baseFps, double multiplier)
        {
            double frames = baseFps * multiplier * MILLISECONDS_FACTOR;
            frames = System.Math.Floor(frames) / FLOOR_DIVISOR;
            return frames;
        }

        private static void UpdateDamageIndex(Entity entity)
        {
            entity.dmgIdx++;
            if (entity.dmgIdx >= MAX_DAMAGE_INDEX)
                entity.dmgIdx = 0;

        }


        private static void UpdateCooldown(int cooldownKey, double frames, Entity entity)
        {
            CdInst cdInst = entity.cd.fastCheck.get(cooldownKey);

            if (cdInst != null)
            {
                if (ZERO_FRAMES > frames)
                {
                    entity.cd.cdList.remove(cdInst);
                    cdInst.frames = ZERO_FRAMES;
                    entity.cd.fastCheck.remove(cdInst.k);
                }
                else
                {
                    cdInst.frames = frames;
                }
            }
            else
            {
                CdInst newCdInst = new CdInst(cooldownKey, frames);
                entity.cd.fastCheck.set(cooldownKey, newCdInst);
                entity.cd.cdList.push(newCdInst);
            }
        }
    }
}