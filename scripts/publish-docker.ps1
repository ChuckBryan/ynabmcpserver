# Publish-Docker.ps1
# Usage: ./publish-docker.ps1 <version>
# Publishes a Docker image with the specified version tag

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing Docker image with version $Version"

try {    Write-Host "Environment variables:"
    Write-Host "DOCKER_USERNAME: $env:DOCKER_USERNAME"
    Write-Host "Docker PAT: [redacted for security]"
    # Use .NET's built-in container support to publish with the version tag
    Write-Host "Running dotnet publish with version tag: $Version"    
    # Update the version in Directory.Build.props before publishing
    # This ensures the container image tag uses the correct version
    $dirBuildPropsPath = Join-Path $PSScriptRoot ".." "Directory.Build.props"
    $xml = [xml](Get-Content $dirBuildPropsPath)
    $versionElement = $xml.Project.PropertyGroup.SelectSingleNode("Version")
    $versionElement.InnerText = $Version
    $xml.Save($dirBuildPropsPath)
    
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:DockerUsername=$env:DOCKER_USERNAME `
        -p:ContainerRegistry=docker.io `
        /t:PublishContainer `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker image creation failed with exit code $LASTEXITCODE"
        exit 1
    }    
    # Verify the image was created
    Write-Host "Verifying Docker image was created"
    docker images "$env:DOCKER_USERNAME/ynabmcp:$Version"
    # Also tag as latest
    Write-Host "Running dotnet publish with latest tag"
    # Update the ContainerImageTag to 'latest' in Directory.Build.props
    $xml = [xml](Get-Content $dirBuildPropsPath)
    $xml.Project.PropertyGroup.ContainerImageTag = "latest"
    $xml.Save($dirBuildPropsPath)
    
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:DockerUsername=$env:DOCKER_USERNAME `
        -p:ContainerRegistry=docker.io `
        /t:PublishContainer `
        --os linux
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker latest tag creation failed with exit code $LASTEXITCODE"
        exit 1    }
    
    # Verify the latest image was created
    Write-Host "Verifying Docker latest image was created"    
    docker images "$env:DOCKER_USERNAME/ynabmcp:latest"
    
    # Restore the version in Directory.Build.props (for next time)
    $xml = [xml](Get-Content $dirBuildPropsPath)
    $xml.Project.PropertyGroup.ContainerImageTag = '$(Version)'
    $xml.Save($dirBuildPropsPath)
    
    Write-Host "Successfully published Docker image $env:DOCKER_USERNAME/ynabmcp:$Version and $env:DOCKER_USERNAME/ynabmcp:latest"
} catch {
    Write-Error "An error occurred publishing the Docker image: $_"
    exit 1
}