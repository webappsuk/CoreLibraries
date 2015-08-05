@echo off

:: We need administrator permissions to run correctly
net session >nul 2>&1
if %errorLevel% == 0 (
	echo Administrative permissions confirmed.
) else (
	echo Current permissions inadequate, run as administrator.
	GOTO End
)

:: Set up VS2015 MSBuild environment.
@call "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"
cd "%~dp0"
SET slnDir=%~dp0

del /S /Q "nugets\*.*" 2>nul
mkdir nugets 2>nul

msbuild Core-Libraries.sln /p:Configuration=NuGet;SolutionDir=%slnDir% /t:rebuild

:End
echo.

if /I NOT "%1" == "nopause" pause