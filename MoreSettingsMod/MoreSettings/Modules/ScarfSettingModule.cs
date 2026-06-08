using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.hl.types;
using dc.ui;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Events.Interfaces.Game.Menu;
using ModCore.Mods;
using ModCore.Modules;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.Scarf;

namespace MoreSettings.Modules
{
    internal class ScarfSettingModule : BaseModule,
    IEventReceiver,
    IOnAfterPauseMenuBuild
    {
        public override string Description => GetText.Instance.GetString("ScarfManager");

        public override ScarfConfig config => (ScarfConfig)base.config;

        public override Enums.MenuCategory Type => Enums.MenuCategory.Scarf;

        public CustomScarfBase scarfBase = default!;

        public override void Initialize(ModBase mainMod)
        {
            EventSystem.AddReceiver(this);
            config = SettingsMain.ConfigValue.Scarf;
            scarfBase = new CustomScarfBase();
            scarfBase.Load();
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;
            menuHelper.addSimpleWidget(
                GetText.Instance.GetString("ScarfManager"),
                "",
                new Action(openScarfui),
                scrollerFlow
            );
        }

        public override void UnregisterHooks()
        {
            base.UnregisterHooks();
            Hook_Hero.initScarf -= Hook_Hero_initScarf;
        }

        public override void RegisterHooks()
        {
            Hook_Hero.initScarf += Hook_Hero_initScarf;
        }

        public override void PermanentlyRegisterHooks()
        {
        }

        private void Hook_Hero_initScarf(Hook_Hero.orig_initScarf orig, Hero self)
        {
            if (!config.Enabled)
            {
                orig(self);
                return;
            }
            var h = self;
            if (h != null)
                h.scarf.dispose();
            h!.scarf = scarfBase.CreateCustomScarfManager(h);
        }

        public void CloseAllUI()
        {
            var opt = dc.ui.Options.Class.ME;
            var hud = dc.ui.HUD.Class.ME;
            if (opt == null) goto continued;
            opt.destroy();

        continued:
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

        public void openScarfui()
        {
            dc.pr.Game game = dc.pr.Game.Class.ME;
            if (game == null) goto erro;
            if (game.hero == null) goto erro;
            CloseAllUI();
            var scarfUI = new CustomScarfUI(Main.Class.ME, scarfBase);
            scarfUI.EnableAndDisable = () =>
            {
                if (config.Enabled)
                {
                    Hook_Hero.initScarf += Hook_Hero_initScarf;
                    scarfBase.UpdateSarfs();
                }
                else
                {
                    Hook_Hero.initScarf -= Hook_Hero_initScarf;
                    game.hero.initScarf();
                }
            };
            return;
        erro:
            var popup = new dc.ui.ModalPopUp(Ref<bool>.In(true), CreateColor.ColorFromHex("#000000"));
            popup.text(GetText.Instance.GetString("OpenInGame").ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
        }

        void IOnAfterPauseMenuBuild.OnAfterPauseMenuBuild(Pause pause) => MenuModule.Instance.AddCustomButtonToPause("Scarf", (e) => openScarfui(), pause, 2);
    }
}