<#
.SYNOPSIS
    Finalizes an inherited feature-area OpenAPI document.

.DESCRIPTION
    Reads the feature alias by default, validates the optional manually
    maintained policy component map, regenerates the complete default component
    map, applies the resolved maps, retains the deduplicated document as numbered
    artifact 16, and atomically republishes it to the feature alias. Component
    maps remain stable unnumbered files.

.PARAMETER FeatureArea
    Lowercase kebab-case feature name used for the output directory and artifact
    prefix.

.PARAMETER InputPath
    Inherited and sorted OpenAPI input. Defaults to the feature alias beneath
    <OutputRoot>/<FeatureArea>.

.PARAMETER OutputRoot
    Root directory beneath which the feature-area directory is created. Defaults
    to specification/openapi. An explicit InputPath may be elsewhere.

.EXAMPLE
    ./scripts/specification/Finalize-OpenApi.ps1 -FeatureArea responses

.EXAMPLE
    ./scripts/specification/Finalize-OpenApi.ps1 `
        -FeatureArea responses `
        -OutputRoot ./artifacts/openapi
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$FeatureArea,

    [Parameter()]
    [string]$InputPath,

    [Parameter()]
    [string]$OutputRoot
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "pipeline" "_OpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$script:PipelineDirectory = Join-Path $PSScriptRoot "pipeline"
$script:DefaultOutputRoot = Join-Path $script:RepositoryRoot "specification" "openapi"

function Invoke-OpenApiFinalization {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FeatureArea,

        [Parameter()]
        [string]$InputPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputRoot
    )

    $paths = Get-OpenApiArtifactPaths -FeatureArea $FeatureArea -OutputRoot $OutputRoot
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $paths.Final
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The inherited '$FeatureArea' OpenAPI input path is not a file: $resolvedInputPath"
    }

    $stageScriptNames = @(
        "Generate-OpenApiComponentDefaultMap.ps1"
        "Deduplicate-OpenApiComponents.ps1"
    )
    $stageScriptPaths = [ordered]@{}
    foreach ($stageScriptName in $stageScriptNames) {
        $stageScriptPath = Join-Path $script:PipelineDirectory $stageScriptName
        if (-not (Test-Path -LiteralPath $stageScriptPath -PathType Leaf)) {
            throw "The OpenAPI stage script was not found: $stageScriptPath"
        }
        $stageScriptPaths[$stageScriptName] = $stageScriptPath
    }

    if (Test-Path -LiteralPath $paths.PolicyComponentMap -PathType Container) {
        throw "The component policy map path is not a file: $($paths.PolicyComponentMap)"
    }

    $null = & $stageScriptPaths["Generate-OpenApiComponentDefaultMap.ps1"] `
        -InputPath $resolvedInputPath `
        -DefaultMapPath $paths.DefaultComponentMap `
        -PolicyMapPath $paths.PolicyComponentMap
    $null = & $stageScriptPaths["Deduplicate-OpenApiComponents.ps1"] `
        -InputPath $resolvedInputPath `
        -DefaultMapPath $paths.DefaultComponentMap `
        -PolicyMapPath $paths.PolicyComponentMap `
        -OutputPath $paths.Deduplicated

    foreach ($artifactPath in @($paths.DefaultComponentMap, $paths.Deduplicated)) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The '$FeatureArea' OpenAPI finalization pipeline did not produce '$artifactPath'."
        }
    }

    $null = Publish-OpenApiArtifact `
        -SourcePath $paths.Deduplicated `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "OpenAPI finalization complete for '$FeatureArea'."
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Policy component map: $(if ([System.IO.File]::Exists($paths.PolicyComponentMap)) { $paths.PolicyComponentMap } else { '<none>' })"
    Write-Host "Default component map: $($paths.DefaultComponentMap)"
    Write-Host "Deduplicated output: $($paths.Deduplicated)"
    Write-Host "Final output: $($paths.Final)"

    return $paths
}

# Dot-sourcing exposes the orchestration functions to tests without running the
# finalization pipeline.
if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($FeatureArea)) {
        throw "FeatureArea is required."
    }
    if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
        $OutputRoot = $script:DefaultOutputRoot
    }
    $null = Invoke-OpenApiFinalization `
        -FeatureArea $FeatureArea `
        -InputPath $InputPath `
        -OutputRoot $OutputRoot
}
