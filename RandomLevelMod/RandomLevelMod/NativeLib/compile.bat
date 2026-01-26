@echo off
echo C++ DLL...
g++ -shared -o DoorProcessor.dll DoorProcessor.cpp -DMODCORE_EXPORTS -O2
if %errorlevel% equ 0 (
    echo build  DoorProcessor.dll
) else (
    echo ???????
    pause
)