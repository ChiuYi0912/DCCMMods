using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.h3d.pass;
using dc.hl.types;
using dc.hxd;
using dc.pr;
using dc.ui;
using dc.ui.hud;
using dc.ui.pause;
using Hashlink.Proxy.Values;
using Hashlink.UnsafeUtilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Modules;
using MonoMod.Utils;
using MoreSettings.Base.Modules;
using MoreSettings.Configuration;
using MoreSettings.GameMechanics.Scarf;
using MoreSettings.Utilities;
using static dc.ui.pause._DefaultPause;

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
            scarfBase.Load();
        }

        public override void BuildMenu(dc.ui.Options options, string Separator)
        {
            base.BuildMenu(options, Separator);
            if (!config.Enabled)
                return;
            menuHelper.addSimpleWidget(
                "飘带管理器",
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
            Hook__DefaultPause.__constructor__ += Hook_DefaultPause_init;
        }

        internal static virtual_cb_inter_t_ GetEntryFromPause(DefaultPause pause)
        {
            ArrayObj obj = pause.options;
            if (obj == null) throw new ArgumentNullException(nameof(pause.options));
            virtual_cb_inter_t_ virtual_ = obj.getDyn(pause.curOptionId);
            //if (virtual_.cb == null) throw new ArgumentNullException(nameof(virtual_.cb));
            if (pause.curOptionId != 1) return virtual_;
            try
            {
                HlAction cb = virtual_.cb;
                cb.Invoke();
            }
            catch (IndexOutOfRangeException ex)
            {
                Logger.Information($"无法调用 entry.cb（可能是零参数闭包）: {ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                Logger.Information($"entry.cb 调用失败: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Error($"entry.cb 调用时发生未预期错误: {ex}");
            }
            return virtual_;

        }


        private void Hook_DefaultPause_init(Hook__DefaultPause.orig___constructor__ orig, DefaultPause arg1)
        {
            orig(arg1);
            AddCustomButtonToPause("飘带管理", () => { openScarfui(); }, arg1);
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
            popup.text(GetText.Instance.GetString("请在游戏内打开!\n").ToHaxeString(), CreateColor.ColorFromHex("#ffffff"), Ref<bool>.In(true));
        }

        public void AddCustomButtonToPause(string buttonText, Action onClick, DefaultPause pause)
        {
            dc.ui.Text text = Assets.Class.makeText("".ToHaxeString(), null, true, pause.botMenu);
            text.set_textAlign(new Align.Center());

            Flow flow = new(text);
            flow.set_horizontalAlign(new FlowAlign.Middle());
            dc.ui.Text text1 = Assets.Class.makeText(buttonText.ToHaxeString(), null, true, flow);
            text1.scaleX = text1.scaleY = text1.scaleY / 2;
            text1.posChanged = true;

            double width = text.get_textWidth();
            double height = text.get_textHeight();
            dc.h2d.Interactive interactive = new(width, height, text, null);
            interactive.onClick = new HlAction<Event>((e) => { });
            interactive.onOver = new HlAction<Event>((e) => { });

            var entry = new virtual_cb_inter_t_();
            entry.t = text;
            entry.inter = interactive;
            entry.cb = new HlAction(onClick);

            pause.botMenu.addChildAt(text, 2);
            ArrayObj obj = pause.options;
            pause.options.insertDyn(1, entry);


            FlowProperties properties = flow.getProperties(text1);
            properties.offsetX += 30;

            flow.reflow();
            pause.onResize();
        }

        public void ScarfOption()
        {
            Logger.Information("hello");
        }
    }
}