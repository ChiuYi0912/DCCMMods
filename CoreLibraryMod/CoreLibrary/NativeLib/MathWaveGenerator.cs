using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;

namespace CoreLibrary.NativeLib
{
    public unsafe class MathWaveGenerator : IDisposable
    {
        public const string PointerAPI = "MathWaveGenerator.dll"; 

        [DllImport(PointerAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern void StartSin();

        [DllImport(PointerAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern float* GetSinPointer();

        [DllImport(PointerAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopSin();

        [DllImport(PointerAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeAddSinTarget(double* target);

        [DllImport(PointerAPI, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeRemoveSinTarget(double* target);
        


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
        }

        public void Dispose()
        {
            StopSin();
            _sinPtr = null;
        }

      public void SetTarget(double* target) => NativeAddSinTarget(target);
      public void RemoveTarget(double* target) => NativeRemoveSinTarget(target);
   }
}