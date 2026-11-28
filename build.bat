@echo off
rem Publishes a self-contained Intento for x64 into dist\. Double-click, or run: build.bat [Release^|Debug]
setlocal
cd /d "%~dp0"

set "CONFIG=%~1"
if "%CONFIG%"=="" set "CONFIG=Release"

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
