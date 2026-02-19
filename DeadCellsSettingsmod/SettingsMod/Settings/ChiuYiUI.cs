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
using ChiuYiUI.Settings.HasUi;
using dc.level;
using dc.cine;
using ChiuYiUI.GameCm;
using ModCore.Menu;
using ModCore.Events;

namespace ChiuYiUI;

public class ChiuYiUI : IModMenu, IEventReceiver
{
    #region 主函数
    private readonly CHIUYIMain _mod;
    private readonly Scraf _scraf;
    private HasUiSetting _hasUi;


    public ChiuYiUI(CHIUYIMain mod)
    {
        EventSystem.AddReceiver(this);

        _mod = mod;
        _scraf = new Scraf(mod);
        _hasUi = new HasUiSetting();
        GetText.Instance.RegisterMod("SettingsLang");
    }

    public void BuildMenu(Options self)
    {
        self.bg.alpha = 0f;
        Graphics graphics = self.createScroller(1);
        AddCustomOptionsToSoundPage();
        Flow scrollerFlow = self.scrollerFlow;
        self.addSeparator(GetText.Instance.GetString("更多易用性设置").AsHaxeString(), scrollerFlow);
        addoverskin(self);
        UIsettings(self);
        Viewport(self);
        if (CHIUYIMain.config.Value.Allscarf)
        {
            scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("飘带布料设置").AsHaxeString(), scrollerFlow);
            _scraf.AddSprIdToggle();
            self.addSeparator(GetText.Instance.GetString("飘带一配置").AsHaxeString(), scrollerFlow);
            _scraf.AddScarfOption(0);
            scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("飘带二配置").AsHaxeString(), scrollerFlow);
            _scraf.AddScarfOption(1);
            scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("飘带三配置").AsHaxeString(), scrollerFlow);
            _scraf.AddScarfOption(2);
            scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("飘带四配置").AsHaxeString(), scrollerFlow);
            _scraf.AddScarfOption(3);
            scrollerFlow = self.scrollerFlow;
            self.addSeparator(GetText.Instance.GetString("飘带五配置").AsHaxeString(), scrollerFlow);
            _scraf.AddScarfOption(4);
            scrollerFlow = self.scrollerFlow;
        }
    }


    private void AddCustomOptionsToSoundPage()
    {
        var options = Options.Class.ME;
        options.title.set_text(GetText.Instance.GetString("DCCM模组开关(B站ChiuYi.秋)").AsHaxeString());
        _Assets _Assets = Assets.Class;
        //dc.ui.Text text = _Assets.makeText("这是一个空白页面".AsHaxeString(), dc.ui.Text.Class.COLORS.get("ST".AsHaxeString()), true, options.title);
    }
    #endregion

    #region 联动皮肤开关
    private void addoverskin(Options self)
    {
        var options = Options.Class.ME;
        var scrollerFlow = options.scrollerFlow;


        HlFunc<bool> toggleFunction = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.SkinEnabled;
            CHIUYIMain.config.Value.SkinEnabled = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool proxyValue = CHIUYIMain.config.Value.SkinEnabled;
        ref bool proxyRef = ref proxyValue;
        OptionWidget allskin = options.addToggleWidget(
            GetText.Instance.GetString("一键开启所有联动装束效果").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用所有装束效果（包括国王和其他特殊皮肤）").AsHaxeString(),
            toggleFunction,
            Ref<bool>.From(ref proxyRef),
            scrollerFlow
        );


        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> katanazero = static () =>
        {
            bool katanabool = !CHIUYIMain.config.Value.skinkatana;
            CHIUYIMain.config.Value.skinkatana = katanabool;
            CHIUYIMain.config.Save();
            return katanabool;
        };
        bool Katan1 = CHIUYIMain.config.Value.skinkatana;
        ref bool proxyRef1 = ref Katan1;
        OptionWidget katanaskin = options.addToggleWidget(
            GetText.Instance.GetString("武士零装束效果").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用武士零装束效果").AsHaxeString(),
            katanazero,
            Ref<bool>.From(ref proxyRef1),
            scrollerFlow
        );


        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> teleportToggle = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.teleport;
            CHIUYIMain.config.Value.teleport = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool teleportProxyValue = CHIUYIMain.config.Value.teleport;
        ref bool teleportProxyRef = ref teleportProxyValue;
        options.addToggleWidget(
            GetText.Instance.GetString("雨中冒险传送功能").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用传送功能").AsHaxeString(),
            teleportToggle,
            Ref<bool>.From(ref teleportProxyRef),
            scrollerFlow
        );


        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> popd = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.pop;
            CHIUYIMain.config.Value.pop = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool opod1 = CHIUYIMain.config.Value.pop;
        ref bool popDamage = ref opod1;
        options.addToggleWidget(
            GetText.Instance.GetString("杀戮尖塔暴击特效").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用杀戮尖塔暴击特效").AsHaxeString(),
            popd,
            Ref<bool>.From(ref popDamage),
            scrollerFlow
        );


        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> sty = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.rsty;
            CHIUYIMain.config.Value.rsty = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool styy = CHIUYIMain.config.Value.rsty;
        ref bool styy1 = ref styy;
        options.addToggleWidget(
            GetText.Instance.GetString("迈阿密热线暴击特效").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用迈阿密热线暴击特效").AsHaxeString(),
            sty,
            Ref<bool>.From(ref styy1),
            scrollerFlow
        );


        _hasUi.Markets(self);


        scrollerFlow = options.scrollerFlow;
        self.addSeparator(GetText.Instance.GetString("特殊设置").AsHaxeString(), scrollerFlow);
        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> Pause = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.Hitpause;
            CHIUYIMain.config.Value.Hitpause = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool Pause1 = CHIUYIMain.config.Value.Hitpause;
        options.addToggleWidget(
            GetText.Instance.GetString("删除击中停顿").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用攻击时命中的停顿感").AsHaxeString(),
            Pause,
            new Ref<bool>(ref Pause1),
            scrollerFlow
        );



        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> loreBankMimicRoom = static () =>
        {
            bool newValue = !CHIUYIMain.config.Value.LoreBankMimicRoom;
            CHIUYIMain.config.Value.LoreBankMimicRoom = newValue;
            CHIUYIMain.config.Save();
            return newValue;
        };
        bool hasloreBankMimicRoom = CHIUYIMain.config.Value.LoreBankMimicRoom;
        options.addToggleWidget(
            GetText.Instance.GetString("预知拟态魔剧情房间常驻").AsHaxeString(),
            GetText.Instance.GetString("开启时：预知拟态魔剧情房间生成不会被“关闭剧情房间影响”").AsHaxeString(),
            loreBankMimicRoom,
            new Ref<bool>(ref hasloreBankMimicRoom),
            scrollerFlow
        );



        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> scraf = () =>
        {
            bool newValue = !CHIUYIMain.config.Value.Allscarf;
            CHIUYIMain.config.Value.Allscarf = newValue;
            CHIUYIMain.config.Save();
            if (newValue)
            {
                scrollerFlow = self.scrollerFlow;
                self.addSeparator("飘带布料设置".AsHaxeString(), scrollerFlow);
                _scraf.AddSprIdToggle();
                self.addSeparator("飘带一配置".AsHaxeString(), scrollerFlow);
                _scraf.AddScarfOption(0);
                scrollerFlow = self.scrollerFlow;
                self.addSeparator("飘带二配置".AsHaxeString(), scrollerFlow);
                _scraf.AddScarfOption(1);
                scrollerFlow = self.scrollerFlow;
                self.addSeparator("飘带三配置".AsHaxeString(), scrollerFlow);
                _scraf.AddScarfOption(2);
                scrollerFlow = self.scrollerFlow;
                self.addSeparator("飘带四配置".AsHaxeString(), scrollerFlow);
                _scraf.AddScarfOption(3);
                scrollerFlow = self.scrollerFlow;
                self.addSeparator("飘带五配置".AsHaxeString(), scrollerFlow);
                _scraf.AddScarfOption(4);
                scrollerFlow = self.scrollerFlow;
            }
            return newValue;
        };
        bool scarf = CHIUYIMain.config.Value.Allscarf;
        ref bool sf = ref scarf;
        options.addToggleWidget(
            GetText.Instance.GetString("自定义飘带").AsHaxeString(),
            GetText.Instance.GetString("启用/禁用自定义飘带").AsHaxeString(),
            scraf,
            Ref<bool>.From(ref sf),
            scrollerFlow
        );



    }
    #endregion

    public void Viewporthook()
    {
        Hook_Viewport.bumpAng += Hook_Viewport_bumpang;
        Hook_Viewport.bumpDir += Hook_Viewport_bumpdir;
        Hook_Viewport.shakeS += Hook_Viewport_shakes;
        Hook_Viewport.shakeReversedS += Hook_Viewport_shakeReversedS;

        Hook__OptionsBase.__constructor__ += Hook_OptionsBase_Mian;


        Hook_Game.decreasingSlowMo += hook_game_decreasingSlowMo;
        Hook_LevelStruct.applyDifficulty += Hook__LevelStruct_applyDifficulty;
        Hook__TierItemFound.__constructor__ += Hook__TierItemFound__constructor__;

        Hook_NewsPanel.updateVisible += Hook_NewsPanel_updateVisible;
        Hook_NewsPanel.focusIn += Hook_NewsPanel_focusIn;
        Hook_NewsPanel.update += Hook_NewsPanel_update;
    }

    private void Hook__TierItemFound__constructor__(Hook__TierItemFound.orig___constructor__ orig, TierItemFound arg1, Hero hero, Entity e, InventItem item, double iconX, double iconY, HlAction<bool> onComplete)
    {
        if (CHIUYIMain.config.Value.SpeedTier)
        {
            new SpeedTier(hero, e, item);
            return;
        }
        orig(arg1, hero, e, item, iconX, iconY, onComplete);
    }

    private void Hook__LevelStruct_applyDifficulty(Hook_LevelStruct.orig_applyDifficulty orig, LevelStruct self)
    {
        if (dc.pr.Game.Class.ME.user.game.spawnMimicInNextLevel && CHIUYIMain.config.Value.LoreBankMimicRoom && Main.Class.ME.options.disableLoreRooms)
        {
            addMimicrooom(self);
        }
        orig(self);
    }
    public void addMimicrooom(LevelStruct Struct)
    {
        for (int i = 0; i < Data.Class.loreRoom.all.get_length(); i++)
        {
            dynamic lore = Data.Class.loreRoom.all.array.getDyn(i);
            if (lore == null) return;
            if (dc._Data.LoreRoom_Impl_.Class.get_room(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>()).id.ToString() == "MimicEscapedRoom")
            {
                Struct.tryAddLoreRoom(((HaxeProxyBase)lore).ToVirtual<virtual_arc_examinables_fxEmitters_Intention_levels_onlyUseOnce_rarity_requiredLore_requiredMeta_room_roomLoot_sprites_status_structMode_>());
            }
        }
    }


    private void Hook_NewsPanel_update(Hook_NewsPanel.orig_update orig, NewsPanel self)
    {
        if (CHIUYIMain.config.Value.NewsPanel) return;
        orig(self);
    }

    private void Hook_NewsPanel_focusIn(Hook_NewsPanel.orig_focusIn orig, NewsPanel self)
    {
        if (CHIUYIMain.config.Value.NewsPanel) return;

        orig(self);
    }

    private void Hook_NewsPanel_updateVisible(Hook_NewsPanel.orig_updateVisible orig, NewsPanel self)
    {
        if (CHIUYIMain.config.Value.NewsPanel) return;
        orig(self);

    }


    private void Hook_OptionsBase_Mian(Hook__OptionsBase.orig___constructor__ orig, OptionsBase pauseUI, Pause pressTxt)
    {
        orig(pauseUI, pressTxt);
    }

    private void hook_game_decreasingSlowMo(Hook_Game.orig_decreasingSlowMo orig, dc.pr.Game self, double durationS, double spd)
    {
        if (CHIUYIMain.config.Value.Hitpause) return;
        orig(self, durationS, spd);
    }

    private void Hook_Viewport_shakeReversedS(Hook_Viewport.orig_shakeReversedS orig, Viewport self, double yPow, double d, double xPow)
    {
        yPow = CHIUYIMain.config.Value.ViewportshakeReversedSX;
        d = CHIUYIMain.config.Value.ViewportshakeReversedSY;
        xPow = CHIUYIMain.config.Value.ViewportshakeReversedSD;
        orig(self, yPow, d, xPow);
    }

    private void Hook_Viewport_shakes(Hook_Viewport.orig_shakeS orig, Viewport self, double yPow, double d, double xPow)
    {
        yPow = CHIUYIMain.config.Value.ViewportshakesX;
        d = CHIUYIMain.config.Value.ViewportshakesY;
        xPow = CHIUYIMain.config.Value.ViewportshakesD;
        orig(self, yPow, d, xPow);
    }

    private void Hook_Viewport_bumpdir(Hook_Viewport.orig_bumpDir orig, Viewport self, int dir, double? pow)
    {
        pow = CHIUYIMain.config.Value.Viewportbumdir;
        orig(self, dir, pow);
    }

    private void Hook_Viewport_bumpang(Hook_Viewport.orig_bumpAng orig, Viewport self, double ang, double? pow)
    {
        pow = CHIUYIMain.config.Value.ViewportbumAng;
        orig(self, ang, pow);
    }

    public void Viewport(Options self)
    {
        var scrollerFlow = self.scrollerFlow;
        self.addSeparator(GetText.Instance.GetString("更多视角设置").AsHaxeString(), scrollerFlow);
        dc.String string3 = Lang.Class.t.get("U35_PLAYER_CAMERA_SPEED".AsHaxeString(), null);
        double num = Main.Class.ME.options.playerCameraSpeed;
        scrollerFlow = self.scrollerFlow;

        HlAction<double> defaultValue = new HlAction<double>((double v) =>
        {
            Main.Class.ME.options.playerCameraSpeed = v;
            CHIUYIMain.config.Save();
        });
        double b = 0.1;
        bool showRawValue = false;
        bool showRawValue1 = true;
        double minValue = -100;
        double maxValue = 100;
        OptionWidget optionWidget = self.addSliderWidget(
            string3,
            defaultValue,
            num,
            new Ref<double>(ref b),
            scrollerFlow,
            new Ref<bool>(ref showRawValue),
            new Ref<bool>(ref showRawValue1),
            new Ref<double>(ref minValue),
            new Ref<double>(ref maxValue),
            null,
            new Ref<int>());


        scrollerFlow = self.scrollerFlow;
        dc.String string4 = Lang.Class.t.get("屏幕震动调节".AsHaxeString(), null);
        self.addSeparator(string4, scrollerFlow);
        string3 = Lang.Class.t.get("角度震动强度".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportbumAng;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportbumAng = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = -100;
        maxValue = 100;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());


        string3 = Lang.Class.t.get("方向震动强度 ".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.Viewportbumdir;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.Viewportbumdir = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = -100;
        maxValue = 100;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());





        #region 持续震动起始强度
        scrollerFlow = self.scrollerFlow;
        string4 = Lang.Class.t.get("持续震动起始强度".AsHaxeString(), null);
        self.addSeparator(string4, scrollerFlow);
        string3 = Lang.Class.t.get("水平震动目标强度".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakesX;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakesX = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 1;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());




        string3 = Lang.Class.t.get("垂直震动目标强度".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakesY;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakesY = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 10;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());




        string3 = Lang.Class.t.get("震动持续时间".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakesD;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakesD = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 100;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());


        #endregion


        #region 持续震动衰减强度
        scrollerFlow = self.scrollerFlow;
        string4 = Lang.Class.t.get("持续震动衰减强度".AsHaxeString(), null);
        self.addSeparator(string4, scrollerFlow);
        string3 = Lang.Class.t.get("水平震动目标强度".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakeReversedSX;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakeReversedSX = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 1;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());

        string3 = Lang.Class.t.get("垂直震动目标强度".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakeReversedSY;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakeReversedSY = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 1;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());


        string3 = Lang.Class.t.get("衰减持续时间".AsHaxeString(), null);
        num = CHIUYIMain.config.Value.ViewportshakeReversedSD;
        scrollerFlow = self.scrollerFlow;

        defaultValue = new HlAction<double>((double v) =>
       {
           CHIUYIMain.config.Value.ViewportshakeReversedSD = v;
           CHIUYIMain.config.Save();
       });
        b = 0.1;
        showRawValue = false;
        showRawValue1 = true;
        minValue = 0;
        maxValue = 100;
        optionWidget = self.addSliderWidget(
           string3,
           defaultValue,
           num,
           new Ref<double>(ref b),
           scrollerFlow,
           new Ref<bool>(ref showRawValue),
           new Ref<bool>(ref showRawValue1),
           new Ref<double>(ref minValue),
           new Ref<double>(ref maxValue),
           null,
           new Ref<int>());

    }
    #endregion

    public void UIsettings(Options options)
    {
        Flow scrollerFlow = options.scrollerFlow;
        options.addSeparator(GetText.Instance.GetString("UI界面修改").AsHaxeString(), scrollerFlow);
        scrollerFlow = options.scrollerFlow;
        HlFunc<bool> scraf = () =>
        {
            bool newValue = !CHIUYIMain.config.Value.NewsPanel;
            CHIUYIMain.config.Value.NewsPanel = newValue;
            CHIUYIMain.config.Save();
            if (newValue)
            {

            }
            return newValue;
        };
        bool scarf = CHIUYIMain.config.Value.NewsPanel;
        ref bool sf = ref scarf;
        options.addToggleWidget(
            GetText.Instance.GetString("删除主页新闻steam面板").AsHaxeString(),
            GetText.Instance.GetString("下一次打开游戏时生效").AsHaxeString(),
            scraf,
            Ref<bool>.From(ref sf),
            scrollerFlow);
    }

    public string GetName()
    {
        return "ChiuYi Mod 设置";
    }
}