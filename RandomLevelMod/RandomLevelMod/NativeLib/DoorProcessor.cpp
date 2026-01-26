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