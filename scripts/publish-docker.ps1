# Publish-Docker.ps1
# Usage: ./publish-docker.ps1 <version>
# Publishes a Docker image with the specified version tag

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing Docker image with version $Version"

try {
    # Use .NET's built-in container support to publish with the version tag
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:EnableSdkContainerSupport=true `
        -p:ContainerRegistry=docker.io `
        -p:ContainerImageTag=$Version `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker image creation failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    # Also tag as latest
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:EnableSdkContainerSupport=true `
        -p:ContainerRegistry=docker.io `
        -p:ContainerImageTag=latest `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker latest tag creation failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    Write-Host "Successfully published Docker image swampyfox/ynabmcp:$Version and swampyfox/ynabmcp:latest"
} catch {
    Write-Error "An error occurred publishing the Docker image: $_"
    exit 1
}