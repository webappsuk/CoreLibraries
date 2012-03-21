param($nupkgPath, $nuspec)

# Drop lib directory
$libPath = Join-Path $nupkgPath -ChildPath "lib"
if (Test-Path $libPath) {
    Write-Host "Removing lib directory"    
	Remove-Item $libPath -Recurse -Force 
}

# Remove dependency on mono
Write-Host "Remove dependency on Mono.Cecil."
$specEditor = New-Object WebApplications.Utilities.PowerShell.NuSpecEditor $nuspec
$specEditor.RemoveDependencies("Mono.Cecil")
$specEditor.Save()