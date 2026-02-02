#!/usr/bin/env pwsh
# Local Gateway Runner Script
# This script is used to run the Local Gateway on a local PC

param(
    [string]$ServerUrl = "https://localhost:5001",
    [string]$GatewayId = $null
)

$ErrorActionPreference = "Stop"

Write-Host "Starting TextRpg Local Gateway..." -ForegroundColor Cyan

# Set environment variables
$env:TEXTRPG_SERVER_URL = $ServerUrl
if ($GatewayId) {
    $env:TEXTRPG_GATEWAY_ID = $GatewayId
}

# Navigate to the LocalGateway project
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Join-Path $ScriptDir "..\src\LocalGateway"

# Run the Local Gateway
Write-Host "Connecting to server: $ServerUrl" -ForegroundColor Yellow
dotnet run --project $ProjectDir
