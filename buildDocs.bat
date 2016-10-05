@echo off

set thisDir=%~dp0
set pages=%thisDir%..\GitHubPages\
set solutionDir=%thisDir%..\CoreLibraries\
cd "%pages%"

rem delete all files except the listed ones
set protected=(.git .gitignore license.md readme.md)
for %%f in %protected% do attrib +r +s %%f
del *.* /S /Q
for %%f in %protected% do attrib -r -s %%f

rem Deletes ALL subdirectories 
for /D  %%G in (*) do rd /s /q %%G

cd "%solutionDir%"
@echo off
set vsdocman=%programfiles(x86)%\VSdocman\VSdocmanCmdLine.exe
if not exist "%vsdocman%" goto nodocs

echo Building Documentation... Please wait this can take up to 10 minutes to complete!
"%vsdocman%" /vs 2015 /operation compileSolutionToSingle /profile default "%solutionDir%CoreLibraries.Sln" 2>&1
if errorlevel 3 goto err3
if errorlevel 2 goto err2
if errorlevel 1 goto err1
if errorlevel 0 goto err0
 
Goto end
 
:nodocs
echo VS Docman command line was not found at "%vsdocman%".
Goto end
 
:err0
echo OK
Goto end
 
:err1
echo Visual Studio 2015 was not found.
Goto end
 
:err2
echo Incorrect command line parameters.
Goto end
 
:err3
echo Fatal error compiling docs.
Goto end
 
:end
cd "%thisDir%"
pause