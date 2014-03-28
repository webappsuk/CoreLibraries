#*=============================================
#* FileName: Commit.ps1
#*=============================================
#* Script Name: [Commit]
#* Company: Web Applications UK Ltd
#* Web: http://www.webapplicationsuk.com
#* Reqrmnts:
#* Keywords:
#*=============================================
param([string]$message = "", [string]$solutionPathStr = "", [bool]$push = $true)

$ErrorActionPreference = "Stop"

if (($dte -eq $null) -or
	($dte.Solution -eq $null)) {
	throw "Must run from inside Visual Studio's package manager console"
}

$path = Split-Path $MyInvocation.MyCommand.Definition

# Get the path for the core functions
$library = Join-Path $path -ChildPath "WebApplications.PowerShell.Utilities.ps1"

# Load core functions
. $library

# Ensure nuget is up to date
if (!$skipNugetUpdate) { UpdateNuget }

Commit $solutionPathStr $message

if ($push) { Push $solutionPathStr }