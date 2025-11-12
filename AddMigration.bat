@echo OFF
set "scriptName=%~1"
goto :scriptNameCheck
:promptUser
set /p "scriptName=Enter script name: "
:scriptNameCheck
if "%scriptName%" == "" goto :promptUser
REM Generate Migration Script
dotnet ef migrations ^
	add "%scriptName%" ^
	--project .\src\BOTAI ^
	--startup-project .\src\BOTAI ^
	--output-dir Generated\Migrations ^
	--context BOTAIContext