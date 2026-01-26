// NativeMath.h
#pragma once

#ifdef _WIN32
    #ifdef MODCORE_EXPORTS
        #define MOD_API __declspec(dllexport)
    #else
        #define MOD_API __declspec(dllimport)
    #endif
#else
    #define MOD_API __attribute__((visibility("default")))
#endif

extern "C" {

    MOD_API const char* GetModName();
    MOD_API int GetModVersion();
    MOD_API void InitializeMod();
    MOD_API void ShutdownMod();
    

    MOD_API int AddInt(int a, int b);
    MOD_API int SubtractInt(int a, int b);
    MOD_API int MultiplyInt(int a, int b);
    MOD_API float DivideInt(int a, int b);
    
    MOD_API void BatchAddInt(const int* inputA, const int* inputB, int* output, int count);
    MOD_API void BatchMultiplyFloat(const float* inputA, const float* inputB, float* output, int count);
    
    MOD_API float CalculateGameValue(float base, float multiplier, float bonus);
    MOD_API int RandomRange(int min, int max);
}


#pragma pack(push, 1)
struct ModState {
    bool isActive;
    int modId;
    float performanceScore;
    char modName[64];
};
#pragma pack(pop)

extern "C" {
    MOD_API ModState GetModState();
    MOD_API void UpdateModState(const ModState* newState);
}