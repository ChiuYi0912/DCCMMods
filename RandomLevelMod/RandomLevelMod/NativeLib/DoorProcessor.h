#pragma once

#ifdef DOORPROCESSOR_EXPORTS
#define MOD_API __declspec(dllexport)
#else
#define MOD_API __declspec(dllimport)
#endif

extern "C" {
MOD_API int RandomRange(int min, int max);
}