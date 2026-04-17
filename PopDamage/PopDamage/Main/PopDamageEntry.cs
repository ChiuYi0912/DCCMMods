using dc;
using dc.tool.atk;
using ModCore.Mods;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using PopDamage.Override;
using ModCore.Menu;
using dc.h2d;
using ModCore.Modules;
using ModCore.Events;
using PopDamage.Main.lnterface;
using CoreLibrary.Utilities.CustomPopDamage;

namespace PopDamage;

public class PopDamageEntry(ModInfo info) : ModBase(info),
    IModMenu
{
    public string GetName() => "PopDamageSetting";
    public string? GetSubText() => $"version: {Info.Version = "0.9.12"},DCCMVersion:{Info.DCCMVersion}";
    public override void Initialize()
    {
        base.Initialize();
        // _ = new EntityPopDaamage();
        // _ = new Override.BasePopDamage();
        Logger.Information("Hello DCCM!");
        EventSystem.BroadcastEvent<IOnHookInitalize, PopDamageEntry>(this);

        _ = new EntityPopDamage(this);
        PopDamageHandlerRegistry.Register(new OtherPop.GradientPop());
    }


    public void BuildMenu(dc.ui.Options options)
    {
        options.createScroller(1);
        AddCustomOptionsToSoundPage();
        Flow flow = options.scrollerFlow;
        HlFunc<bool> Revealpop = () =>
        {
            bool newValue = !EntityPopDaamage.GetConfig.Value.RevealPop;
            EntityPopDaamage.GetConfig.Value.RevealPop = newValue;
            EntityPopDaamage.GetConfig.Save();
            return newValue;
        };
        bool isReveal = EntityPopDaamage.GetConfig.Value.RevealPop;
        options.addToggleWidget(
            GetText.Instance.GetString("开启/关闭 揭露暴击文字").AsHaxeString(),
            null,
            Revealpop,
            new Ref<bool>(ref isReveal),
            flow
        );
    }

    private void AddCustomOptionsToSoundPage()
    {
        var options = dc.ui.Options.Class.ME;
        options.title.set_text(GetText.Instance.GetString("模组设置").AsHaxeString());
    }


}
