#!/usr/bin/env pwsh
# Database Migration Script
# This script runs Entity Framework migrations

param(
    [ValidateSet("add", "update", "remove", "list")]
    [string]$Action = "update",

    [string]$MigrationName = "",

    [string]$Project = "CoreBackend"
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SolutionDir = Join-Path $ScriptDir ".."
$ProjectPath = Join-Path $SolutionDir "src\$Project\$Project.csproj"

Write-Host "Running migration action: $Action" -ForegroundColor Cyan

switch ($Action) {
    "add" {
        if ([string]::IsNullOrWhiteSpace($MigrationName)) {
            Write-Error "Migration name is required for 'add' action"
            exit 1
        }
        Write-Host "Adding migration: $MigrationName" -ForegroundColor Yellow
        dotnet ef migrations add $MigrationName --project $ProjectPath --startup-project $ProjectPath
    }
    "update" {
        Write-Host "Updating database..." -ForegroundColor Yellow
        dotnet ef database update --project $ProjectPath --startup-project $ProjectPath
    }
    "remove" {
        Write-Host "Removing last migration..." -ForegroundColor Yellow
        dotnet ef migrations remove --project $ProjectPath --startup-project $ProjectPath
    }
    "list" {
        Write-Host "Listing migrations..." -ForegroundColor Yellow
        dotnet ef migrations list --project $ProjectPath --startup-project $ProjectPath
    }
}

Write-Host "Migration action completed successfully" -ForegroundColor Green
