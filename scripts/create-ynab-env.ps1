#!/usr/bin/env pwsh

# Script to create a .env file for YNAB API requests

# Read the API token from the user securely
$apiToken = Read-Host "Enter your YNAB API Token" -AsSecureString
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiToken)
$plainApiToken = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
[System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($BSTR)

# Prompt for budget ID (with default)
$budgetId = Read-Host "Enter your YNAB Budget ID (or leave blank for 'last-used')"
if ([string]::IsNullOrWhiteSpace($budgetId)) {
    $budgetId = "last-used"
}

# Create or update .env file
$envPath = Join-Path $PSScriptRoot "..\requests\.env"
$envContent = @"
# YNAB API environment variables
# This file should be excluded from source control

# Your YNAB API token
YNAB_API_TOKEN=$plainApiToken

# Budget ID
BUDGET_ID=$budgetId
"@

# Write to file
Set-Content -Path $envPath -Value $envContent -Force

# Print confirmation (without showing the key)
Write-Host "YNAB API .env file has been created at $envPath"
Write-Host "You can now use the REST Client with environment variables"
