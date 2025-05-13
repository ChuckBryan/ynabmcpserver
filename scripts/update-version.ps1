# Update-Version.ps1
# Usage: ./update-version.ps1 <version>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Define paths
$dirBuildPropsPath = Join-Path $PSScriptRoot ".." "Directory.Build.props"

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