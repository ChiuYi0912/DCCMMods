using dc;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Hook_Options = dc.ui.Hook_Options;
using Options = dc.ui.Options;
using System;
using dc.h2d;
using Serilog;
using dc.tool.mod;
using dc.tool;
using dc.pr;
using dc.libs.heaps.slib;
using dc.en;
using ModCore.Modules;
using dc.libs.heaps;
using ChiuYiUI.UI;
using ChiuYiUI.Core;
using dc.level;
using dc.cine;
using ChiuYiUI.GameMechanics;
using ModCore.Menu;
using ModCore.Events;
using dc.hl.types;

namespace ChiuYiUI.UI
{
    public class SkinSettings
    {
        private readonly ScarfManager _scraf;

        public SkinSettings(ScarfManager scraf)
        {
            _scraf = scraf;
        }

        public void AddSkinSettings(Options self)
        {
            AddOverSkinSettings(self);
            AddSpecialSettings(self);
        }

        private void AddOverSkinSettings(Options self)
        {
            var scrollerFlow = self.scrollerFlow;


            HlFunc<bool> toggleFunction = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.SkinEnabled;
                ChiuYiMain.config.Value.SkinEnabled = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool proxyValue = ChiuYiMain.config.Value.SkinEnabled;
            ref bool proxyRef = ref proxyValue;
            OptionWidget allskin = self.addToggleWidget(
                GetText.Instance.GetString("一键开启所有联动装束效果").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用所有装束效果（包括国王和其他特殊皮肤）").AsHaxeString(),
                toggleFunction,
                Ref<bool>.From(ref proxyRef),
                scrollerFlow
            );



            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> katanazero = static () =>
            {
                bool katanabool = !ChiuYiMain.config.Value.skinkatana;
                ChiuYiMain.config.Value.skinkatana = katanabool;
                ChiuYiMain.config.Save();
                return katanabool;
            };
            bool Katan1 = ChiuYiMain.config.Value.skinkatana;
            ref bool proxyRef1 = ref Katan1;
            OptionWidget katanaskin = self.addToggleWidget(
                GetText.Instance.GetString("武士零装束效果").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用武士零装束效果").AsHaxeString(),
                katanazero,
                Ref<bool>.From(ref proxyRef1),
                scrollerFlow
            );


            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> teleportToggle = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.teleport;
                ChiuYiMain.config.Value.teleport = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool teleportProxyValue = ChiuYiMain.config.Value.teleport;
            ref bool teleportProxyRef = ref teleportProxyValue;
            self.addToggleWidget(
                GetText.Instance.GetString("雨中冒险传送功能").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用传送功能").AsHaxeString(),
                teleportToggle,
                Ref<bool>.From(ref teleportProxyRef),
                scrollerFlow
            );


            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> popd = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.pop;
                ChiuYiMain.config.Value.pop = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool opod1 = ChiuYiMain.config.Value.pop;
            ref bool popDamage = ref opod1;
            self.addToggleWidget(
                GetText.Instance.GetString("杀戮尖塔暴击特效").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用杀戮尖塔暴击特效").AsHaxeString(),
                popd,
                Ref<bool>.From(ref popDamage),
                scrollerFlow
            );


            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> sty = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.rsty;
                ChiuYiMain.config.Value.rsty = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool styy = ChiuYiMain.config.Value.rsty;
            ref bool styy1 = ref styy;
            self.addToggleWidget(
                GetText.Instance.GetString("迈阿密热线暴击特效").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用迈阿密热线暴击特效").AsHaxeString(),
                sty,
                Ref<bool>.From(ref styy1),
                scrollerFlow
            );
        }

        public void AddSpecialSettings(Options self)
        {
            var scrollerFlow = self.scrollerFlow;

            self.addSeparator(GetText.Instance.GetString("特殊设置").AsHaxeString(), scrollerFlow);
            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> Pause = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.Hitpause;
                ChiuYiMain.config.Value.Hitpause = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool Pause1 = ChiuYiMain.config.Value.Hitpause;
            self.addToggleWidget(
                GetText.Instance.GetString("删除击中停顿").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用攻击时命中的停顿感").AsHaxeString(),
                Pause,
                new Ref<bool>(ref Pause1),
                scrollerFlow
            );


            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> DIYFlashTeleport = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.DIYFlashTeleport;
                ChiuYiMain.config.Value.DIYFlashTeleport = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool hasDIYFlashTeleport = ChiuYiMain.config.Value.DIYFlashTeleport;
            self.addToggleWidget(
                GetText.Instance.GetString("开启平滑传送").AsHaxeString(),
                GetText.Instance.GetString("").AsHaxeString(),
                DIYFlashTeleport,
                new Ref<bool>(ref hasDIYFlashTeleport),
                scrollerFlow
            );



            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> loreBankMimicRoom = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.LoreBankMimicRoom;
                ChiuYiMain.config.Value.LoreBankMimicRoom = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool hasloreBankMimicRoom = ChiuYiMain.config.Value.LoreBankMimicRoom;
            self.addToggleWidget(
                GetText.Instance.GetString("预知拟态魔剧情房间常驻").AsHaxeString(),
                GetText.Instance.GetString("开启时：预知拟态魔剧情房间生成不会被“关闭剧情房间影响”").AsHaxeString(),
                loreBankMimicRoom,
                new Ref<bool>(ref hasloreBankMimicRoom),
                scrollerFlow
            );

            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> SpeedTier = static () =>
            {
                bool newValue = !ChiuYiMain.config.Value.SpeedTier;
                ChiuYiMain.config.Value.SpeedTier = newValue;
                ChiuYiMain.config.Save();
                return newValue;
            };
            bool hasSpeedTier = ChiuYiMain.config.Value.SpeedTier;
            self.addToggleWidget(
                GetText.Instance.GetString("开启竞速卷轴拾取").AsHaxeString(),
                GetText.Instance.GetString("竞速模式快速拾取卷轴常驻").AsHaxeString(),
                SpeedTier,
                new Ref<bool>(ref hasSpeedTier),
                scrollerFlow
            );




            scrollerFlow = self.scrollerFlow;
            HlFunc<bool> scraf = () =>
            {
                bool newValue = !ChiuYiMain.config.Value.Allscarf;
                ChiuYiMain.config.Value.Allscarf = newValue;
                ChiuYiMain.config.Save();
                if (newValue)
                {
                    _scraf.DisplayScarfasettings(self);
                }
                return newValue;
            };
            bool scarf = ChiuYiMain.config.Value.Allscarf;
            ref bool sf = ref scarf;
            self.addToggleWidget(
                GetText.Instance.GetString("自定义飘带").AsHaxeString(),
                GetText.Instance.GetString("启用/禁用自定义飘带").AsHaxeString(),
                scraf,
                Ref<bool>.From(ref sf),
                scrollerFlow
            );
        }
    }
}