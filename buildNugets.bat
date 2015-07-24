@echo off

rem Need custom build task/script to do the package as the built in one doesnt work that well
	rem Create .nuproj instead of .nuspec, .nuproj will output the nuspec at build to obj and package that

SET slnDir=%~dp0

del /S /Q "nugets\*.*" 2>nul
mkdir nugets 2>nul

msbuild Core-Libraries.sln /p:Configuration=NuGet;SolutionDir=%slnDir% /t:rebuild