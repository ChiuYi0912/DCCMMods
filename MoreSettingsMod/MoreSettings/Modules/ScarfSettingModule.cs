using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h3d.pass;
using dc.hl.types;
using dc.pr;
using dc.ui;
using dc.ui.pause;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.Scarf;
using MoreSettings.Utilities;

namespace MoreSettings.Modules
{
    public class ScarfSettingModule : BaseModule
    {
        public override string Description => base.Description;

        public override ScarfConfig config => (ScarfConfig)base.config;
        public CustomScarfBase scarfBase = default!;

        public override void Initialize(ModBase mainMod)
        {
            config = SettingsMain.ConfigValue.Scarf;
            scarfBase = new CustomScarfBase();
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;
            menuHelper.addSimpleWidget(
                "飘带",
                "",
                () =>
                {
                    dc.pr.Game game = dc.pr.Game.Class.ME;
                    if (game == null) goto erro;
                    if (game.hero == null) goto erro;
                    CloseAllUI();
                    _ = new CustomScarfUI(Main.Class.ME);
                    return;
                erro:
                    var popup = new dc.ui.ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
                    popup.text(GetText.Instance.GetString("请在游戏内打开!\n").ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
                },
                scrollerFlow
            );
        }

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            Hook_Hero.initScarf += Hook_Hero_initScarf;
        }



        private void Hook_Hero_initScarf(Hook_Hero.orig_initScarf orig, Hero self)
        {
            if (!config.Enabled)
            {
                orig(self);
                return;
            }

            if (self.scarf != null)
                self.scarf.dispose();
            self.scarf = scarfBase.CreateCustomScarfManager(self);
        }

        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_Hero.initScarf -= Hook_Hero_initScarf;
        }


        public void CloseAllUI()
        {
            var opt = dc.ui.Options.Class.ME;
            var hud = dc.ui.HUD.Class.ME;
            if (opt == null) return;
            opt.destroy();
            dc.pr.Game.Class.ME.paused = true;
            ArrayObj roots = Process.Class.ALL;
            hud.unblur();

            foreach (Process p in roots.AsEnumerable())
            {
                if (p != null && !p.destroyed)
                {
                    if (IsEssentialProcess(p))
                    {
                        p.destroyed = true;
                    }
                }
            }
            dc.pr.Game.Class.ME.resume();
        }
        private static bool IsEssentialProcess(Process p)
        {
            string typeName = p.GetType().Name;
            return typeName == "Options" || typeName == "DefaultPause" || typeName == "OptionsBase";
        }
    }
}