# Local Gateway Run Script
# Use this script to start the Local Gateway on your local PC

param(
    [Parameter(Mandatory=$false)]
    [string]$ServerUrl = "https://localhost:5001",

    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

Write-Host "Starting Local Gateway..."
Write-Host "Server URL: $ServerUrl"

# Change to the LocalGateway project directory
Push-Location -Path "$PSScriptRoot\..\src\LocalGateway"

try {
    $env:SERVER_URL = $ServerUrl

    if ($Verbose) {
        dotnet run -- --verbosity detailed
    }
    else {
        dotnet run
    }
}
finally {
    Pop-Location
}
