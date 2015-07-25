@echo off

SET slnDir=%~dp0

del /S /Q "nugets\*.*" 2>nul
mkdir nugets 2>nul

msbuild Core-Libraries.sln /p:Configuration=NuGet;SolutionDir=%slnDir% /t:rebuild