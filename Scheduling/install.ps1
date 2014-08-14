param($installPath, $toolsPath, $package, $project)

# This is the MSBuild targets file to add
$targetsFile = [System.IO.Path]::Combine($toolsPath, 'Scheduling.targets')

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $project.FullName)
$targetUri = new-object Uri('file://' + $targetsFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

# Remove previous imports to PerfSetup.targets
$msbuild.Xml.Imports | Where-Object {$_.Project.ToLowerInvariant().EndsWith("scheduling.targets") } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	[string]::Format( "Removed import of '{0}'" , $_.Project )
}

# Add import to PostSharp.targets
$import = $msbuild.Xml.AddImport($relativePath)
$import.Condition = "Exists('$relativePath')"
[string]::Format("Added import of '{0}'.", $relativePath )