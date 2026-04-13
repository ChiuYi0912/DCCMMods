using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Core.Utilities;
using dc;
using dc.tool.mod;
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
using MoreSettings.Utilities;
using Serilog;

namespace MoreSettings;

public class SettingsMain(ModInfo info) : ModBase(info),
IModMenuProvider,
IOnGameEndInit,
IOnAfterLoadingCDB
{
   private static SettingsMain instance = default!;
   private static ModuleManager moduleManager = default!;
   private static Config<MainConfig> modConfig = new Config<MainConfig>("MoreSettingsConfig");
   
   private ModMenu modMenu = default!;

   public static SettingsMain Instance => instance;
   public static ModuleManager ModuleManager => moduleManager;
   public static Config<MainConfig> ModConfig => modConfig;
   public static MainConfig ConfigValue => modConfig.Value;

   IModMenu IModMenuProvider.GetModMenu() => modMenu;
   public static ModMenu GetModMenu() => Instance.modMenu;
   public static void SaveConfig() => modConfig.Save();


   public override void Initialize()
   {
      instance = this;
      Utilities.Logger.Initialize(Logger);

      moduleManager = new ModuleManager(this);
      moduleManager.RegisterModule(new GameplayModule());

      modMenu = new ModMenu(this);
   }

   void IOnGameEndInit.OnGameEndInit()
   {
      // var res = Info.ModRoot.GetFilePath("SettingRes.pak");
      // FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
   }

   void IOnAfterLoadingCDB.OnAfterLoadingCDB(_Data_ cdb){}
}