﻿param($installPath, $toolsPath, $package, $project)

# This is the MSBuild targets file to remove
$targetsFile = [System.IO.Path]::Combine($toolsPath, 'WebApplicationsSignature.targets')

# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

# Grab the loaded MSBuild project for the project
$msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Remove imports to targets file
$msbuild.Xml.Imports | Where-Object {$_.Project.EndsWith("webapplicationssignature.targets") } | Foreach { 
	$_.Parent.RemoveChild( $_ ) 
	[string]::Format( "Removed import of '{0}'" , $_.Project )
}