$current_version = (dotnet --version)
if (!(Test-Path ".required-sdk") ) {
    Write-Host "No special SDK required for this build, will use '$current_version'."
    return
}

$required_version = (Get-Content ".required-sdk" -Raw)
Write-Host  "Required SDK for this build is '$required_version'."
if ($current_version -ge $required_version) {
    Write-Host "Current version is greater than required version, use 'global.json' to specify a version to use if current is not suitable."
}
else {
    Write-Host "Installed SDK version doesn't match required one. Proceeding to install required SDK."
    $ScriptToRun= $PSScriptRoot+"\dotnet-install.ps1 -Version $required_version -InstallDir .dotnet"
    Write-Host $ScriptToRun
    Invoke-Expression $ScriptToRun
}