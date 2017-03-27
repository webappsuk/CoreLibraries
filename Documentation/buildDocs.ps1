# Build Documentation Script:
#   This can be run as a standalone. The doc build is driven by docfx.json.
#   More info on that can be found at https://dotnet.github.io/docfx/tutorial/docfx.exe_user_manual.html#3-docfxjson-format
#
# Build dependencies:
#   Chocolatey
#   - nuget.commandline
#   - docfx
#
# These are checked for at build time, and if the Chocolatey packages are not present, it 
# will attempt to install them.

Write-Host '> Building documentation...'

$_cd = $PSScriptRoot
$docFxConfigPath = (Resolve-Path "$_cd\docfx.json").Path

# Get the docfx config as an object
$docFxJson = Get-Content $docFxConfigPath | Out-String | ConvertFrom-Json

if (([string]::IsNullOrWhiteSpace($_cd)) -OR ([string]::IsNullOrWhiteSpace($docFxJson.build.dest))) {
    Throw 'Path not specified for the docfx output destination. See docfx.json : build -> dest'
}

# Select and resolve the build destination for the documentation.
$docOutPath = "$_cd\$($docFxJson.build.dest)"

# Check Chocolatey is installed by invoking the exe
if (-NOT (Get-Command "choco" -errorAction SilentlyContinue)) {
    Throw @'
Chocolatey not found. It can be downloaded by running the following:
Invoke-WebRequest https://chocolatey.org/install.ps1 -UseBasicParsing | Invoke-Expression
'@
}

try {
    choco install nuget.commandline
}
catch {
    # Not sure if this would throw, catch in case
    $_.Exception.Message
    Write-Host ''
    Throw '> Install of NuGet.CLI package failed'
}

if ($LastExitCode -ne 0) {
    # If it swallowed the error and just returned exitcode
    Write-Host ''
    Throw '> NuGet.CLI install failed'
}

if (-NOT (Test-Path "$_cd\packages.config")) {
    Write-Host ''
    Throw '> NuGet restore for DocFx failed. Unable to find packages.config'
}

try {
    nuget restore "$_cd\packages.config" -outputdirectory "$_cd\packages" -noninteractive
}
catch {
    # Not sure if this would throw, catch in case
    $_.Exception.Message
    Write-Host ''
    Throw '> NuGet restore for DocFx failed'
}

if ($LastExitCode -ne 0) {
    # If it swallowed the error and just returned exitcode
    Write-Host ''
    Throw '> NuGet restore for DocFx failed'
}

# Once we know Chocolatey is installed, try get docfx
try {
    choco install docfx
}
catch {
    # Not sure if this would throw, catch in case
    $_.Exception.Message
    Write-Host ''
    Throw '> Install of DocFx package failed'
}

if ($LastExitCode -ne 0) {
    # If it swallowed the error and just returned exitcode
    Write-Host ''
    Throw '> DocFx install failed'
}

if (Test-Path "$docOutPath") {
    # Clear existing documentation
    #Remove-Item "$docOutPath\*" -Recurse
}

# Finally confirmed all packages required to build docs
#docfx "$_cd\docfx.json"

if ($LastExitCode -ne 0) {
    Write-Host ''
    Throw '> DocFx run failed'
}

# Resolve the output path
$docOutPath = Resolve-Path $docOutPath