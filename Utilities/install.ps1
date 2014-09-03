param($installPath, $toolsPath, $package, $project)

# This is the MSBuild targets file to add
$targetsFile = [System.IO.Path]::Combine($toolsPath, 'Web Applications Signature.targets')

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Make the path to the targets file relative.
$projectUri = new-object Uri('file://' + $project.FullName)
$targetUri = new-object Uri('file://' + $targetsFile)
$relativePath = $projectUri.MakeRelativeUri($targetUri).ToString().Replace([System.IO.Path]::AltDirectorySeparatorChar, [System.IO.Path]::DirectorySeparatorChar)

# Remove previous imports to targets file
$msbuild.Xml.Imports | Where-Object {$_.Project.ToLowerInvariant().EndsWith("web applications signature.targets") } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	[string]::Format( "Removed import of '{0}'" , $_.Project )
}

# Add import to targets file
$import = $msbuild.Xml.AddImport($relativePath)
$import.Condition = "Exists('$relativePath') AND '`$(Configuration)|`$(Platform)' == 'Release|AnyCPU'"
[string]::Format("Added import of '{0}'.", $relativePath )