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
    Write-Host "Environment variables:"
    Write-Host "DOCKER_USERNAME: $env:DOCKER_USERNAME"
    Write-Host "Docker PAT: [redacted for security]"
      # Use .NET's built-in container support to publish with the version tag
    Write-Host "Running dotnet publish with version tag: $Version"
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:EnableSdkContainerSupport=true `
        -p:ContainerRegistry=docker.io `
        -p:ContainerRepository=$env:DOCKER_USERNAME/ynabmcp `
        -p:ContainerImageTag=$Version `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker image creation failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    # Verify the image was created
    Write-Host "Verifying Docker image was created"
    docker images "$env:DOCKER_USERNAME/ynabmcp:$Version"    # Also tag as latest
    Write-Host "Running dotnet publish with latest tag"
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:EnableSdkContainerSupport=true `
        -p:ContainerRegistry=docker.io `
        -p:ContainerRepository=$env:DOCKER_USERNAME/ynabmcp `
        -p:ContainerImageTag=latest `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker latest tag creation failed with exit code $LASTEXITCODE"
        exit 1
    }
    
    # Verify the latest image was created
    Write-Host "Verifying Docker latest image was created"
    docker images "$env:DOCKER_USERNAME/ynabmcp:latest"
    
    # Try to push the images explicitly to ensure they're uploaded
    Write-Host "Explicitly pushing Docker images to ensure they're uploaded"
    docker push "$env:DOCKER_USERNAME/ynabmcp:$Version"
    docker push "$env:DOCKER_USERNAME/ynabmcp:latest"
    
    Write-Host "Successfully published Docker image swampyfox/ynabmcp:$Version and swampyfox/ynabmcp:latest"
} catch {
    Write-Error "An error occurred publishing the Docker image: $_"
    exit 1
}