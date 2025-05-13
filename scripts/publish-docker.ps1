# Publish-Docker.ps1
# Usage: ./publish-docker.ps1 <version>
# Publishes a Docker image with the specified version tag

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing Docker image with version $Version"

# Verify the version in Directory.Build.props matches the passed version
try {
    $dirBuildPropsPath = Join-Path $PSScriptRoot ".." "Directory.Build.props"
    if (Test-Path $dirBuildPropsPath) {
        $xml = [xml](Get-Content $dirBuildPropsPath)
        $propsVersion = $xml.Project.PropertyGroup.Version
        Write-Host "Directory.Build.props version: $propsVersion"
        Write-Host "Passed version parameter: $Version"
        
        if ($propsVersion -ne $Version) {
            Write-Warning "Warning: Version mismatch between Directory.Build.props ($propsVersion) and parameter ($Version)"
        }
    }
    
    Write-Host "Environment variables:"
    Write-Host "DOCKER_USERNAME: $env:DOCKER_USERNAME"
    Write-Host "Docker PAT: [redacted for security]"    # Use .NET's built-in container support to publish with the version tag
    Write-Host "Running dotnet publish with version tag: $Version"
    
    # Print Directory.Build.props contents for debugging
    Write-Host "Directory.Build.props content:"
    Get-Content (Join-Path $PSScriptRoot ".." "Directory.Build.props")
    
    # Force MSBuild to use the correct version from command line
    dotnet publish src/YnabMcpServer/YnabMcpServer.csproj `
        --configuration Release `
        -p:Version=$Version `
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
        -p:Version=$Version `
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
    
    # Make sure both images are correctly tagged
    Write-Host "Re-tagging images to ensure proper naming"
    docker tag "swampyfox/ynabmcp:$Version" "$env:DOCKER_USERNAME/ynabmcp:$Version"
    docker tag "swampyfox/ynabmcp:latest" "$env:DOCKER_USERNAME/ynabmcp:latest"
    
    # Push both images
    Write-Host "Pushing version tag: $Version"
    docker push "$env:DOCKER_USERNAME/ynabmcp:$Version"
    
    Write-Host "Pushing latest tag"
    docker push "$env:DOCKER_USERNAME/ynabmcp:latest"
    
    # List all available images for verification
    Write-Host "Available Docker images:"
    docker images | Where-Object { $_ -like "*ynabmcp*" }
    
    Write-Host "Successfully published Docker image swampyfox/ynabmcp:$Version and swampyfox/ynabmcp:latest"
} catch {
    Write-Error "An error occurred publishing the Docker image: $_"
    exit 1
}