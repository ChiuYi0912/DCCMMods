#define DOORPROCESSOR_EXPORTS
#include "DoorProcessor.h"

#include <algorithm>
#include <cstdlib>
#include <ctime>

#include "DoorProcessor.h"

static bool s_randInitialized = false;
static void InitRandomSeed() {
    if (!s_randInitialized) {
        std::srand(static_cast<unsigned int>(std::time(nullptr)));
        s_randInitialized = true;
    }
}

int RandomRange(int min, int max) {
    InitRandomSeed();

    if (min >= max) {
        return min;
    }
    return min + (std::rand() % (max - min + 1));
}

MOD_API void ProcessIntArray(int* arr, int length) {
    if (arr == nullptr || length <= 0) return;

    for (int i = 0; i < length; ++i) {
        arr[i] = arr[i] * 2 + 1;
    }
}

extern "C" MOD_API void LeveldoubleUps(GameItem* arr, int length) {
    if (arr == nullptr || length <= 0) return;

    for (int i = 0; i < length; i++) {
        arr[i].doubleUps *= 2;
    }
}