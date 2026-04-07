using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;

namespace CoreLibrary.NativeLib
{
    public unsafe class SimpleSinPointer : IDisposable
    {
        [DllImport("SinPointer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StartSin();

        [DllImport("SinPointer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float* GetSinPointer();

        [DllImport("SinPointer.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopSin();

        private float* _sinPtr = null;

        public float SinValue
        {
            get
            {
                return *_sinPtr;
            }
        }

        public static bool IsAvailable { get; private set; } = true;

        public void StartSinProcess()
        {
            StartSin();
            _sinPtr = GetSinPointer();
            Console.Write($"[SinPointer] Sin process started, pointer: {(IntPtr)_sinPtr:X}");
        }

        public void Dispose()
        {
            StopSin();
            _sinPtr = null;
            Console.Write("[SinPointer] Sin process stopped");
        }

        public void ReadSinValue()
        {
            int count = 0;
            while (count < 100)
            {
                float value = SinValue;
                Console.WriteLine($"Sin值: {value:F4}");
                count++;
            }
        }
    }
}