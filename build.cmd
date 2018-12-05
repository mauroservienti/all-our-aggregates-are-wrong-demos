@echo Off
cd %~dp0

powershell.exe -noexit -file ".\targets\ensure-required-sdk-version.ps1"

dotnet run --project targets --no-launch-profile -- %*