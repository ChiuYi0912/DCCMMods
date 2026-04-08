#include <array>
#include <atomic>
#include <cmath>
#include <thread>

#define DLL_EXPORT extern "C" __declspec(dllexport)

static constexpr int MAX_TARGETS = 1000;
static std::array<double*, MAX_TARGETS> g_targets{nullptr};
static std::thread g_thread;

static float speed = 0.1f;
static float g_sinValue = 0.0f;
static bool g_running = true;

void UpdateLoop() {
    float time = 0.0f;
    float phase = 0.0f;

    while (g_running) {
        time += 0.1f;
        float sinVal = sin(time * static_cast<float>(speed) + phase);
        g_sinValue = (sinVal + 1.0f) / 2.0f;

        for (auto& target : g_targets) {
            if (target != nullptr) {
                *target = static_cast<double>(g_sinValue);
            }
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
}

DLL_EXPORT void StartSin() {
    g_thread = std::thread(UpdateLoop);
    g_thread.detach();
}

DLL_EXPORT float* GetSinPointer() {
    return &g_sinValue;
}

DLL_EXPORT void StopSin() {
    g_running = false;
}

DLL_EXPORT int NativeAddSinTarget(double* target) {
    for (int i = 0; i < MAX_TARGETS; i++) {
        if (g_targets[i] == nullptr) {
            g_targets[i] = target;
            return i;
        }
    }
    return -1;
}

DLL_EXPORT void NativeRemoveSinTarget(double* target) {
    for (auto& t : g_targets) {
        if (t == target) {
            t = nullptr;
            break;
        }
    }
}