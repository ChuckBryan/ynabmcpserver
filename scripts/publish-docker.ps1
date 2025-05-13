# Publish-Docker.ps1
# Usage: ./publish-docker.ps1 <version>
# Uses .NET 9's built-in container support to publish directly to Docker Hub

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing Docker image with version $Version using .NET container support"

try {
    # Navigate to the project directory
    $projectDir = Join-Path $PSScriptRoot ".." "src" "YnabMcpServer"
    Push-Location $projectDir
    
    try {
        # Use .NET's built-in container publishing
        # This uses the container configuration from the .csproj file
        dotnet publish `
            -c Release `
            -p:ContainerRegistry=docker.io `
            -p:ContainerImageTag=$Version
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Container publish failed with exit code $LASTEXITCODE"
            exit 1
        }
        
        # Also tag as latest
        dotnet publish `
            -c Release `
            -p:ContainerRegistry=docker.io `
            -p:ContainerImageTag=latest
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Container publish (latest tag) failed with exit code $LASTEXITCODE"
            exit 1
        }
        
        Write-Host "Successfully published Docker image swampyfox/ynabmcp:$Version and swampyfox/ynabmcp:latest"
    }
    finally {
        # Restore original location
        Pop-Location
    }
} catch {
    Write-Error "An error occurred publishing the Docker image: $_"
    exit 1
}
