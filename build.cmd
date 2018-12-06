@echo Off
cd %~dp0

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '.\targets\ensure-required-sdk-version.ps1' -repo_root='" + %cd% + "'"

dotnet run --project targets --no-launch-profile -- %*