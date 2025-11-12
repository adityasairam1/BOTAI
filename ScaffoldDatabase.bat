@echo OFF

REM Scaffold out our DB Context
dotnet ef dbcontext scaffold ^
	"name=Scaffolding" ^
	Microsoft.EntityFrameworkCore.SqlServer ^
	--project .\src\BOTAI ^
	--startup-project .\src\BOTAI ^
	--output-dir Generated ^
	--context BOTAIContext