param($nupkgPath, $nuspec)

# Remove dependency on WebApplications.Utilities.Initializer
Write-Host "Remove dependency on WebApplications.Utilities.Initializer."
$specEditor = New-Object WebApplications.Utilities.PowerShell.NuSpecEditor $nuspec
$specEditor.RemoveDependencies("WebApplications.Utilities.Initializer")
$specEditor.Save()