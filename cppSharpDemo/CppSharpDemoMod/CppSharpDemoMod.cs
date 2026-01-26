using System.Runtime.InteropServices;
using CppSharpDemoMod.Cppsharp;
using ModCore.Mods;

namespace CppSharpDemoMod;

public class CppSharpDemo : ModBase
{
    public CppSharpDemo(ModInfo info) : base(info)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        NativeInterop.InitializeMod();
        Logger.Information("C++ 模组初始化完成");


        int result1 = NativeInterop.AddInt(5, 3);
        Logger.Debug($"[测试] 5 + 3 = {result1}");


        IntPtr modNamePtr = NativeInterop.GetModName();
        string modName = Marshal.PtrToStringAnsi(modNamePtr)!;
        Logger.Debug($"模组名为：{modName}");

        int[] ints = { 2, 2, 2, };
        int[] results = new int[3];
        NativeInterop.BatchAddInt(ints, ints, results, 3);

        Logger.Information($"模组基本信息：");

        int RandomRange = NativeInterop.RandomRange(0, 50);
        Logger.Debug($"测试随机数生成：{RandomRange}");
    }
}