@echo off
echo C++ DLL...
g++ -shared -o ModCore.dll NativeMath.cpp -DMODCORE_EXPORTS -O2
if %errorlevel% equ 0 (
    echo build  ModCore.dll
) else (
    echo ???????
    pause
)