param (
    [string]$target = "default"
 )

function ExecuteBuild {
    Write-Host "Starting the build..." -ForegroundColor Green
    (dotnet run --project targets --no-launch-profile -- $target)
}

$current_version = (dotnet --version)
if (!(Test-Path ".required-sdk") ) {
    Write-Host "No special SDK required for this build, will use '$current_version'." -ForegroundColor Green
    ExecuteBuild
    return
}

$required_version = (Get-Content ".required-sdk" -Raw)
Write-Host  "Build requires a specific SDK (found in '.required-sdk' file) version: '$required_version'." -ForegroundColor Green
if ($current_version -gt $required_version) {
    Write-Host "Current installed SDK version, '$current_version', is greater than required SDK version, '$required_version'. Use 'global.json' to specify a SDk version to use if current is not suitable, or remove the '.required-sdk' file." -ForegroundColor Red
    Write-Host "Build stopped." -ForegroundColor Red
    return
}
elseif($current_version -lt $required_version){
    Write-Host "Installed SDK version, '$current_version', doesn't match required one, '$required_version'. Proceeding to install required SDK." -ForegroundColor Yellow
    $ScriptToRun= $PSScriptRoot+"\targets\dotnet-install.ps1 -Version $required_version -InstallDir .dotnet"
    Invoke-Expression $ScriptToRun
}

ExecuteBuild