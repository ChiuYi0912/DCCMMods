#include "NativeMath.h"
#include <cmath>
#include <cstring>
#include <ctime>
#include <algorithm>


extern "C"
{
    const char *GetModName()
    {
        return "CppSharpMathMod";
    }

    int GetModVersion()
    {
        return 100; 
    }

    void InitializeMod()
    {
        srand(static_cast<unsigned int>(time(nullptr)));
    }

    void ShutdownMod()
    {
        
    }
}


extern "C"
{
    int AddInt(int a, int b)
    {
        return a + b;
    }

    int SubtractInt(int a, int b)
    {
        return a - b;
    }

    int MultiplyInt(int a, int b)
    {
        return a * b;
    }

    float DivideInt(int a, int b)
    {
        if (b == 0)
            return 0.0f;
        return static_cast<float>(a) / static_cast<float>(b);
    }

    void BatchAddInt(const int *inputA, const int *inputB, int *output, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            output[i] = inputA[i] + inputB[i];
        }
    }

    void BatchMultiplyFloat(const float *inputA, const float *inputB, float *output, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            output[i] = inputA[i] * inputB[i];
        }
    }

    float CalculateGameValue(float base, float multiplier, float bonus)
    {
        float value = base * multiplier;
        value += bonus;
        value = std::max(0.0f, value);
        return value;
    }

    int RandomRange(int min, int max)
    {
        if (min >= max)
            return min;
        return min + (rand() % (max - min + 1));
    }
}


extern "C"
{
    ModState GetModState()
    {
        ModState state;
        state.isActive = true;
        state.modId = 12345;
        state.performanceScore = 95.5f;


#ifdef _WIN32
        strcpy_s(state.modName, "CppSharpMathMod");
#else
        strncpy(state.modName, "CppSharpMathMod", sizeof(state.modName) - 1);
        state.modName[sizeof(state.modName) - 1] = '\0';
#endif

        return state;
    }

    void UpdateModState(const ModState *newState)
    {
        if (newState)
        {
            
        }
    }
}