# Update-Version.ps1
# Usage: ./update-version.ps1 <version>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Define paths
$csprojPath = Join-Path $PSScriptRoot ".." "src" "YnabMcpServer" "YnabMcpServer.csproj"
$dirBuildPropsPath = Join-Path $PSScriptRoot ".." "Directory.Build.props"

# Update version in csproj file
Write-Host "Updating version in $csprojPath to $Version"
try {
    # Load the csproj file
    $xml = [xml](Get-Content $csprojPath)
    
    # Check if the Version property group exists, if not create it
    $versionElement = $xml.Project.PropertyGroup.Version
    
    if ($null -eq $versionElement) {
        # Get the first PropertyGroup or create one if it doesn't exist
        $propertyGroup = $xml.Project.PropertyGroup
        if ($null -eq $propertyGroup) {
            $propertyGroup = $xml.CreateElement("PropertyGroup")
            $xml.Project.AppendChild($propertyGroup)
        } else {
            # Make sure we're using the first PropertyGroup
            if ($propertyGroup -is [array]) {
                $propertyGroup = $propertyGroup[0]
            }
        }
        
        # Create Version element
        $versionElement = $xml.CreateElement("Version")
        $versionElement.InnerText = $Version
        $propertyGroup.AppendChild($versionElement)
    } else {
        # Update the existing Version element
        $versionElement.InnerText = $Version
    }
    
    # Save the changes
    $xml.Save($csprojPath)
    
    Write-Host "Successfully updated version to $Version in $csprojPath"
} catch {
    Write-Error "An error occurred updating the version in csproj: $_"
    exit 1
}

# Create or update Directory.Build.props file
Write-Host "Updating version in $dirBuildPropsPath to $Version"
try {
    if (Test-Path $dirBuildPropsPath) {
        # Update existing Directory.Build.props
        $xml = [xml](Get-Content $dirBuildPropsPath)
        $versionElement = $xml.Project.PropertyGroup.Version
        
        if ($null -eq $versionElement) {
            $propertyGroup = $xml.Project.PropertyGroup
            if ($null -eq $propertyGroup) {
                $propertyGroup = $xml.CreateElement("PropertyGroup")
                $xml.Project.AppendChild($propertyGroup)
            } elseif ($propertyGroup -is [array]) {
                $propertyGroup = $propertyGroup[0]
            }
            
            $versionElement = $xml.CreateElement("Version")
            $versionElement.InnerText = $Version
            $propertyGroup.AppendChild($versionElement)
        } else {
            $versionElement.InnerText = $Version
        }
    } else {
        # Create new Directory.Build.props
        $xml = [xml]"<Project><PropertyGroup><Version>$Version</Version></PropertyGroup></Project>"
    }
    
    # Save the changes
    $xml.Save($dirBuildPropsPath)
    
    Write-Host "Successfully updated version to $Version in $dirBuildPropsPath"
} catch {
    Write-Error "An error occurred updating the version in Directory.Build.props: $_"
    exit 1
}