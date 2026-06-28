using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Interfaces;
using CoreLibrary.Core.Utilities;
using dc;
using dc.en;
using dc.h2d;
using dc.h3d;
using dc.h3d.mat;
using dc.libs.heaps.slib;
using dc.tool;
using dc.tool.skill;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Events;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Menu;
using ModCore.Mods;
using ModCore.Modules;
using ModCore.Storage;
using ModCore.Utilities;
using MoreSettings.Configuration;
using MoreSettings.Core;
using MoreSettings.GameMechanics;
using MoreSettings.GameMechanics.Scarf;
using MoreSettings.Modules;
using MoreSettings.Utilities;
using Serilog;

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
      // --- Silk.NET smoke test ---
      TestSilkStack();

      
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

   // --- Silk.NET advanced smoke test ---

   static void TestSilkStack()
   {
       var log = instance.Logger;

       // 1. Maths: 矩阵变换管线
       // Matrix4X4 (无泛型) 是静态工厂类, Matrix4X4<T> 是值类型
       var rotate = Silk.NET.Maths.Matrix4X4.CreateRotationZ(
           Silk.NET.Maths.Scalar.DegreesToRadians(45f));
       var scale = Silk.NET.Maths.Matrix4X4.CreateScale(2f);
       var model = rotate * scale;
       var point = Silk.NET.Maths.Vector4D.Transform(
           new Silk.NET.Maths.Vector4D<float>(1, 0, 0, 1), model);
       log.Information("[SilkTest] Maths: (1,0,0) rot45° scaled×2 = ({0:F2},{1:F2},{2:F2})",
           point.X, point.Y, point.Z);

       // 2. SDL: 注册平台，枚举设备
       Silk.NET.Input.Sdl.SdlInput.RegisterPlatform();
       log.Information("[SilkTest] Input: SDL platform registered");

       // 3. Windowing: 平台窗口能力
       var opts = Silk.NET.Windowing.WindowOptions.Default;
       log.Information("[SilkTest] Window: API={0} VSync={1} size=({2},{3})",
           opts.API, opts.VSync, opts.Size.X, opts.Size.Y);

       // 4. OpenGL: 验证类型可用
       log.Information("[SilkTest] OpenGL: {0} OK",
           typeof(Silk.NET.OpenGL.GL).Name);
   }

}