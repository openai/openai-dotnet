<#
.SYNOPSIS
    Validates API compatibility between different versions of a .NET NuGet package.

.DESCRIPTION
    This script performs API compatibility checks between a current version of a .NET package
    and a specified baseline version from NuGet.org. It uses the 'apicompat' tool to detect
    breaking changes and supports excluding specific namespaces from the comparison.

    The script performs these operations in sequence:
    1. Builds and packs the current version of the package
    2. Downloads the specified baseline version from NuGet.org
    3. Compares the two versions using the 'apicompat' tool
    4. Reports any breaking changes found in non-ignored namespaces

.PARAMETER ProjectPath
    The path to the .NET project file (.csproj) to build and analyze

.PARAMETER ReleasePath
    The output directory where the built package will be placed

.PARAMETER PackageName
    The name of the NuGet package to compare

.PARAMETER BaselineVersion
    The version number of the baseline package to compare against

.PARAMETER IgnoredNamespaces
    An optional array of namespace names to exclude from the compatibility check

.EXAMPLE
    .\Test-ApiCompatibility.ps1
    Runs the compatibility check using default parameters for the OpenAI SDK

.EXAMPLE
    Invoke-APICompat -ProjectPath "src\MyProject.csproj" -ReleasePath "src\bin\Release" -PackageName "MyPackage" -BaselineVersion "1.0.0"
    Runs a compatibility check between the current version and version 1.0.0 of MyPackage

.NOTES
    The script will generate warnings if breaking changes are detected in non-ignored namespaces.
#>

function Invoke-DotNetBuild {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )

    Write-Output "Building $($ProjectPath)..."
    Write-Output ""
    & dotnet build $ProjectPath
    Write-Output ""
}

function Invoke-DotNetPack {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )

    Write-Output "Packing $($ProjectPath)..."
    Write-Output ""
    & dotnet pack $ProjectPath
    Write-Output ""
}

function Invoke-APICompat {
    param (
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,

        [Parameter(Mandatory = $true)]
        [string]$ReleasePath,

        [Parameter(Mandatory = $true)]
        [string]$PackageName,

        [Parameter(Mandatory = $true)]
        [string]$BaselineVersion,

        [Parameter(Mandatory = $false)]
        [string[]]$IgnoredNamespaces
    )

    try
    {
        Invoke-DotNetBuild -ProjectPath $ProjectPath

        Invoke-DotNetPack -ProjectPath $ProjectPath

        # Extract the values of VersionPrefix and VersionSuffix from the .csproj XML file.
        $xml = [xml](Get-Content $ProjectPath)
        $versionPrefix = $($xml.Project.PropertyGroup[0].VersionPrefix)
        $versionSuffix = $($xml.Project.PropertyGroup[0].VersionSuffix)
        $currentVersion = [string]::IsNullOrEmpty($versionSuffix) ? "$($versionPrefix)" : "$($versionPrefix)-$($versionSuffix)"

        $currentNuGetPackagePath = Join-Path $ReleasePath "$($PackageName).$($currentVersion).nupkg"
        $currentNuGetSymbolsPath = Join-Path $ReleasePath "$($PackageName).$($currentVersion).snupkg"

        # Create temporary folder
        $tempFolderPath = Join-Path $PSScriptRoot "\TempApiCompatibility"
        New-Item -ItemType Directory -Path $tempFolderPath | Out-Null

        # Download OpenAI NuGet package
        $baselineNuGetPackageName = "$($PackageName).$($BaselineVersion).nupkg"
        $baselineNuGetPackagePath = Join-Path $tempFolderPath $baselineNuGetPackageName
        $baselineNuGetPackageUrl = "https://www.nuget.org/api/v2/package/$($PackageName)/$($BaselineVersion)"
        Invoke-RestMethod -Uri $baselineNuGetPackageUrl -OutFile $baselineNuGetPackagePath
        Sleep 10

        Write-Output "Testing API compatibility between versions $($currentVersion) (current) and $($BaselineVersion) (baseline)..."
        Write-Output ""

        # Run apicompat and redirect the error output to a variable
        $output = apicompat package $currentNuGetPackagePath --baseline-package $baselineNuGetPackagePath 2>&1

        # Individual warnings from apicompat have identifiers such as "CP0001", "CP0002", etc.
        $warningRegex = "CP\d\d\d\d"

        # Concatenate the ignored namespaces into a single string, delimiting them by "|" and escaping the "."
        $ignoredRegex = $IgnoredNamespaces -join "|" -creplace "\.", "\."

        Write-Output $excludedRegex

        $warningsFound = 0

        foreach ($line in $($output -split "`r`n")) {
            if ($line -cmatch $warningRegex) {
                if ($($line -cnotmatch $ignoredRegex)) {
                    $warningsFound++
                }
            }
        }

        if ($warningsFound -eq 0) {
            Write-Output "No API breaking changes found."
            Write-Output ""
        }
        else {
            foreach ($line in $($output -split "`r`n")) {
                if ($line -cmatch $warningRegex) {
                    if ($($line -cnotmatch $ignoredRegex)){
                        Write-Warning "$line"
                        Write-Output ""
                    }
                }
                else {
                    Write-Output "$line"
                    Write-Output ""
                }
            }
        }
    }
    finally {
        Remove-Item -Path $tempFolderPath -Recurse -Force
        Remove-Item -Path $currentNuGetPackagePath -Force
        Remove-Item -Path $currentNuGetSymbolsPath -Force
    }
}

$repoRootPath = Join-Path $PSScriptRoot .. -Resolve
$projectPath = Join-Path $repoRootPath "src\OpenAI.csproj"
$releasePath = Join-Path $repoRootPath "src\bin\Release"

Invoke-APICompat -ProjectPath $projectPath `
    -ReleasePath $releasePath `
    -PackageName "OpenAI" `
    -BaselineVersion "2.1.0" `
    -IgnoredNamespaces "OpenAI.Assistants", "OpenAI.Batch", "OpenAI.FineTuning", "OpenAI.Realtime", "OpenAI.VectorStores"