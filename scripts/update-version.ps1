# Update-Version.ps1
# Usage: ./update-version.ps1 <version>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$csprojPath = Join-Path $PSScriptRoot ".." "src" "YnabMcpServer" "YnabMcpServer.csproj"

if (-not (Test-Path $csprojPath)) {
    Write-Error "Could not find .csproj file at $csprojPath"
    exit 1
}

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
    Write-Error "An error occurred updating the version: $_"
    exit 1
}

# Also update the AssemblyVersion and FileVersion if they exist
try {
    $xml = [xml](Get-Content $csprojPath)
    
    $assemblyVersionElement = $xml.Project.PropertyGroup.AssemblyVersion
    if ($null -ne $assemblyVersionElement) {
        $assemblyVersionElement.InnerText = $Version
    }
    
    $fileVersionElement = $xml.Project.PropertyGroup.FileVersion
    if ($null -ne $fileVersionElement) {
        $fileVersionElement.InnerText = $Version
    }
    
    # Save the changes
    $xml.Save($csprojPath)
} catch {
    Write-Error "An error occurred updating the assembly or file version: $_"
    exit 1
}
