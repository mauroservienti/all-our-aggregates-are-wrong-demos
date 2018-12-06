@echo Off
cd %~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\targets\ensure-required-sdk-version.ps1'"

echo %errorlevel%
if %errorlevel% EQU 0 goto build
if %errorlevel% EQU 1 goto add_env_var
if %errorlevel% EQU -1 goto SDK_mismatch

:add_env_var
set DOTNET_INSTALL_DIR=%cd%"\.dotnet"
echo Set DOTNET_INSTALL_DIR path
goto :build

:SDK_mismatch
echo SDK mismatch
Exit -1

:build
echo Ready to run the build
dotnet run --project targets --no-launch-profile -- %*