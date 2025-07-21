@echo off
setlocal

:: Migration name passed as an argument
set MIGRATION_NAME=%1

if "%MIGRATION_NAME%"=="" (
    echo Please provide a migration name as an argument.
    echo Example: ef-migration.bat InitialCreate
    exit /b 1
)

echo Creating migration: %MIGRATION_NAME%
dotnet ef migrations add %MIGRATION_NAME% --project src\examples\EFAcceleratorTools.Examples

if %errorlevel% neq 0 (
    echo Failed to create migration.
    exit /b %errorlevel%
)

echo Updating database...
dotnet ef database update --project src\examples\EFAcceleratorTools.Examples

if %errorlevel% neq 0 (
    echo Failed to update database.
    exit /b %errorlevel%
)

echo Migration '%MIGRATION_NAME%' successfully created and applied!
endlocal
