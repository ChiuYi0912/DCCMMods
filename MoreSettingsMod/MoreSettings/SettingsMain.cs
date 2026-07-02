using System.Runtime.ExceptionServices;
using CoreLibrary.Core.Interfaces;
using dc;
using Hashlink.Proxy.Values;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Menu;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using MoreSettings.Configuration;
using MoreSettings.Core;
using MoreSettings.Modules;

namespace MoreSettings;

internal class SettingsMain(ModInfo info) : ModBase(info),
IModMenuProvider,
IOnGameEndInit,
IOnAfterLoadingCDB
{
   private static SettingsMain instance = default!;
   private static ModuleManager moduleManager = default!;
   private static Config<MainConfig> modConfig = new("MoreSettingsConfig");

   private ModMenu modMenu = default!;

   public static SettingsMain Instance => instance;
   public static ModuleManager ModuleManager => moduleManager;
   public static Config<MainConfig> ModConfig => modConfig;
   public static MainConfig ConfigValue => modConfig.Value;

   IModMenu IModMenuProvider.GetModMenu() => modMenu;
   public static ModMenu ModMenu() => Instance.modMenu;
   public static void SaveConfig() => modConfig.Save();


   public override void Initialize()
   {
      Silk.NET.Core.Loader.LibraryLoader.GetPlatformDefaultLoader();
      instance = this;

      
      Utilities.Logger.Initialize(Logger);
      GetText.Instance.RegisterMod("SettingsLang");
      Info.Version = "1.3.3";
      Info.RepositoryUrl = "https://github.com/ChiuYi0912/DCCMMods/tree/main";

      moduleManager = new ModuleManager(this);
      moduleManager.RegisterModule(new GameplayModule());
      moduleManager.RegisterModule(new WeaponSettingModule());
      moduleManager.RegisterModule(new LevelModule());
      moduleManager.RegisterModule(new HasUiSettingsModule());
      moduleManager.RegisterModule(new VisualCustomizationModule());
      moduleManager.RegisterModule(new ScarfSettingModule());
      moduleManager.RegisterModule(new ViewportSettingsModule());
      moduleManager.RegisterModule(new KeyBindingModule());

      modMenu = new ModMenu(this);

      EventSystem.BroadcastEvent<IOnHookInitialize>();

      //Common.Initialize();
      //Hook_Boot.mainLoop += Hook_Boot_loop;
   }


   private void Hook_Boot_loop(Hook_Boot.orig_mainLoop orig, Boot self)
   {
      #if DEBUG
      try
      {
         orig(self);
      }
      catch (Exception ex)
      {
         Logger.Error("{ex}", ex);
         //System.Diagnostics.Debugger.Break();
         ExceptionDispatchInfo.Capture(ex).Throw();
         throw;
      }
      #else
            orig(self);
      #endif


   }

   void IOnGameEndInit.OnGameEndInit()
   {
      var res = Info.ModRoot.GetFilePath("SettingRes.pak");
      FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
      var json = CDBManager.Instance.GetAlteredCDB();
      CDBManager.Instance.LoadJsonData(json);
   }

   void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb)
   {

   }


}