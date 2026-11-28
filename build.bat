@echo off
rem Builds Intento for x64. Double-click, or run: build.bat [Debug^|Release]
setlocal
cd /d "%~dp0"

set "CONFIG=%~1"
if "%CONFIG%"=="" set "CONFIG=Debug"

powershell -NoProfile -ExecutionPolicy Bypass -File build.ps1 -Configuration %CONFIG%
set "EXITCODE=%ERRORLEVEL%"

echo.
if not "%EXITCODE%"=="0" (
    echo Build failed with exit code %EXITCODE%.
) else (
    echo Build finished successfully.
)
pause
exit /b %EXITCODE%
