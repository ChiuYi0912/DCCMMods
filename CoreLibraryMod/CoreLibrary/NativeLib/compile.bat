@echo off
echo C++ DLL...
g++ -shared -o SinPointer.dll SinPointer.cpp -DMODCORE_EXPORTS -O2
if %errorlevel% equ 0 (
    echo build  SinPointer.dll
) else (
    echo ???????
    pause
)