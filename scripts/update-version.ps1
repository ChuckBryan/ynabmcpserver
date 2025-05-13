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

# Remove version from csproj file if it exists
Write-Host "Removing version from $csprojPath (will use Directory.Build.props instead)"
try {
    if (Test-Path $csprojPath) {
        # Load the csproj file
        $xml = [xml](Get-Content $csprojPath)
        
        # Check if the Version property exists
        $propertyGroups = $xml.Project.PropertyGroup
        $modified = $false
        
        if ($null -ne $propertyGroups) {
            # Handle both single PropertyGroup and multiple PropertyGroups
            if ($propertyGroups -is [array]) {
                foreach ($propertyGroup in $propertyGroups) {
                    if ($null -ne $propertyGroup.Version) {
                        $propertyGroup.RemoveChild($propertyGroup.Version)
                        $modified = $true
                    }
                }
            } else {
                if ($null -ne $propertyGroups.Version) {
                    $propertyGroups.RemoveChild($propertyGroups.Version)
                    $modified = $true
                }
            }
            
            # Save the changes if we removed a Version element
            if ($modified) {
                $xml.Save($csprojPath)
                Write-Host "Successfully removed Version element from $csprojPath"
            } else {
                Write-Host "No Version element found in $csprojPath"
            }
        }
    } else {
        Write-Warning "Project file $csprojPath does not exist"
    }
} catch {
    Write-Error "An error occurred removing the version from csproj: $_"
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