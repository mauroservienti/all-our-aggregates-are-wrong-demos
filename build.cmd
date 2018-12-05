@echo Off
cd %~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\targets\ensure-required-sdk-version.ps1'"

dotnet run --project targets --no-launch-profile -- %*