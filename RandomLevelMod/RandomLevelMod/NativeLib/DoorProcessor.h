#pragma once

#ifdef DOORPROCESSOR_EXPORTS
#define MOD_API __declspec(dllexport)
#else
#define MOD_API __declspec(dllimport)
#endif

#pragma pack(push, 1)
typedef struct {
    int doubleUps;
} GameItem;
#pragma pack(pop)

extern "C" {
MOD_API int RandomRange(int min, int max);

MOD_API void ProcessIntArray(int* arr, int length);

MOD_API void LeveldoubleUps(GameItem* arr, int length);
}