@echo OFF

:: Read migration name from argument or prompt
set "scriptName=%~1"
if "%scriptName%"=="" (
    set /p "scriptName=Enter script name: "
)

:: Generate EF migration
dotnet ef migrations add "%scriptName%" ^
    --project .\BOTAI ^
    --startup-project .\BOTAI ^
    --output-dir Generated\Migrations ^
    --context BOTAIContext

pause
