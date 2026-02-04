# Database Migration Script
# Run this script to apply database migrations

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Development"
)

Write-Host "Running database migrations for $Environment environment..."

# Change to the CoreBackend project directory
Push-Location -Path "$PSScriptRoot\..\src\CoreBackend"

try {
    # Run EF Core migrations
    dotnet ef database update --environment $Environment
    Write-Host "Migrations completed successfully." -ForegroundColor Green
}
catch {
    Write-Host "Migration failed: $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}
