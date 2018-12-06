@echo Off
cd %~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\targets\ensure-required-sdk-version.ps1'"

echo %errorlevel%
IF %errorlevel% EQU 0 GOTO build
IF %errorlevel% EQU 1 GOTO add_env_var
IF %errorlevel% EQU -1 GOTO SDK_mismatch

:add_env_var
SET DOTNET_INSTALL_DIR=%cd%\.dotnet
ECHO Set DOTNET_INSTALL_DIR path
%DOTNET_INSTALL_DIR%\dotnet run --project %cd%\targets --no-launch-profile -- %*
GOTO end

:SDK_mismatch
ECHO SDK mismatch
Exit -1

:build
ECHO Ready to run the build
dotnet run --project targets --no-launch-profile -- %*
GOTO end

:end
