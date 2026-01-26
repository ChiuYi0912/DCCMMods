using System.Runtime.InteropServices;

namespace CppSharpDemoMod.Cppsharp
{
    public static class NativeInterop
    {
       
        private const string DLL_NAME = "ModCore.dll";


        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetModName();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetModVersion();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitializeMod();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShutdownMod();



        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddInt(int a, int b);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SubtractInt(int a, int b);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MultiplyInt(int a, int b);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern float DivideInt(int a, int b);


        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BatchAddInt(int[] inputA, int[] inputB, int[] output, int count);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BatchMultiplyFloat(float[] inputA, float[] inputB, float[] output, int count);

 


        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern float CalculateGameValue(float baseValue, float multiplier, float bonus);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RandomRange(int min, int max);



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ModState
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool isActive;

            public int modId;
            public float performanceScore;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string modName;
        }

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern ModState GetModState();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateModState(ref ModState state);


    }
}