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

namespace ChiuYiUI.UI;

public partial class ChiuYiUI : IModMenu, IEventReceiver
{
    #region 主函数
    private readonly ScarfManager _scarfManager;
    private readonly UISettings _uiSettings;
    private readonly PlayerSettings _lifeBarManager;
    private readonly ViewportSettings _viewportSettings;
    private readonly SkinSettings _skinSetting;
    private readonly GameplaySettings _gameplaySettings;

    public string GetName()
    {
        return "ChiuYi Mod 设置";
    }

    public string? GetSubText()
    {
        return $"version: {ChiuYiMain.Instance.Info.Version} 包含了各种各样的功能开关和数值调整，欢迎使用!";
    }


    public ChiuYiUI(ChiuYiMain mod)
    {
        EventSystem.AddReceiver(this);

        _scarfManager = new ScarfManager(mod);
        _uiSettings = new UISettings();
        _lifeBarManager = new PlayerSettings();
        _viewportSettings = new ViewportSettings();
        _skinSetting = new SkinSettings(_scarfManager);
        _gameplaySettings = new GameplaySettings();

        GetText.Instance.RegisterMod("SettingsLang");
    }

    public void BuildMenu(Options self)
    {
        self.bg.alpha = 0f;
        self.createScroller(1);
        AddCustomOptionsToSoundPage();
        Flow scrollerFlow = self.scrollerFlow;
        self.addSeparator(GetText.Instance.GetString("更多易用性设置").AsHaxeString(), scrollerFlow);
        _skinSetting.AddSkinSettings(self);
        _uiSettings.AddUISettings(self);
        _lifeBarManager.AddLifeBarColorSetting(self);
        _gameplaySettings.AddGameplaySettings(self);
        _viewportSettings.AddViewportSettings(self);
        if (ChiuYiMain.config.Value.Allscarf)
        {
            _scarfManager.DisplayScarfasettings(self);
        }
    }

    private void AddCustomOptionsToSoundPage()
    {
        var options = Options.Class.ME;
        options.title.set_text(GetText.Instance.GetString("DCCM模组开关(B站ChiuYi.秋)").AsHaxeString());
    }
    #endregion

}
