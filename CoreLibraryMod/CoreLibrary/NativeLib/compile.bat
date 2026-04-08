@echo off
echo C++ DLL...
g++ -shared -o MathWaveGenerator.dll MathWaveGenerator.cpp -DMODCORE_EXPORTS -O2
if %errorlevel% equ 0 (
    echo build  MathWaveGenerator.dll
) else (
    echo ???????
    pause
)