#include <atomic>
#include <cmath>
#include <thread>

static float g_sinValue = 0.0f;
static bool g_running = true;
static std::thread g_thread;

void UpdateLoop() {
    float time = 0.0f;
    float speed = 0.1f;
    float phase = 0.0f;

    while (g_running) {
        time += 0.1f;
        float sinVal = sin(time * speed + phase);
        g_sinValue = (sinVal + 1.0f) / 2.0f;
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
}

extern "C" __declspec(dllexport) void StartSin() {
    g_thread = std::thread(UpdateLoop);
    g_thread.detach();
}

extern "C" __declspec(dllexport) float* GetSinPointer() {
    return &g_sinValue;
}

extern "C" __declspec(dllexport) void StopSin() {
    g_running = false;
}