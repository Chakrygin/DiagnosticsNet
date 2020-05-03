@echo off

powershell ^
    -NoLogo ^
    -NoProfile ^
    -NonInteractive ^
    -ExecutionPolicy "Bypass" ^
    -File "%~dp0\build.ps1" %*
