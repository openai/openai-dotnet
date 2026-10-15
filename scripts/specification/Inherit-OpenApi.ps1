<#
.SYNOPSIS
    Refactors one feature-area OpenAPI document into discriminator inheritance.

.DESCRIPTION
    Reads the feature alias by default, refactors componentized discriminator
    oneOf schemas into base/derived allOf inheritance, canonically sorts the
    result, retains numbered stage artifacts 15 and 16, and atomically
    republishes artifact 16 to the feature alias.

.PARAMETER FeatureArea
    Lowercase kebab-case feature name used for the output directory and artifact
    prefix.

.PARAMETER InputPath
    Componentized OpenAPI input. Defaults to the feature alias beneath
    <OutputRoot>/<FeatureArea>.

.PARAMETER OutputRoot
    Root directory beneath which the feature-area directory is created. Defaults
    to specification/openapi.

.EXAMPLE
    ./scripts/specification/Inherit-OpenApi.ps1 -FeatureArea responses

.EXAMPLE
    ./scripts/specification/Inherit-OpenApi.ps1 `
        -FeatureArea responses `
        -InputPath ./artifacts/source/responses.openapi.json `
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

function Invoke-OpenApiInheritance {
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
        throw "The componentized '$FeatureArea' OpenAPI input path is not a file: $resolvedInputPath"
    }

    $stageScriptNames = @(
        "Refactor-OpenApiDiscriminatedInheritance.ps1"
        "Sort-OpenApiDocument.ps1"
    )
    $stageScriptPaths = [ordered]@{}
    foreach ($stageScriptName in $stageScriptNames) {
        $stageScriptPath = Join-Path $script:PipelineDirectory $stageScriptName
        if (-not (Test-Path -LiteralPath $stageScriptPath -PathType Leaf)) {
            throw "The OpenAPI stage script was not found: $stageScriptPath"
        }
        $stageScriptPaths[$stageScriptName] = $stageScriptPath
    }

    $null = & $stageScriptPaths["Refactor-OpenApiDiscriminatedInheritance.ps1"] `
        -InputPath $resolvedInputPath `
        -OutputPath $paths.DiscriminatorInherited
    $null = & $stageScriptPaths["Sort-OpenApiDocument.ps1"] `
        -InputPath $paths.DiscriminatorInherited `
        -OutputPath $paths.DiscriminatorInheritedSorted

    foreach ($artifactPath in @($paths.DiscriminatorInherited, $paths.DiscriminatorInheritedSorted)) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The '$FeatureArea' OpenAPI inheritance pipeline did not produce '$artifactPath'."
        }
    }
    $null = Publish-OpenApiArtifact `
        -SourcePath $paths.DiscriminatorInheritedSorted `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "OpenAPI inheritance complete for '$FeatureArea'."
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Inherited output: $($paths.DiscriminatorInherited)"
    Write-Host "Inherited sorted output: $($paths.DiscriminatorInheritedSorted)"
    Write-Host "Published output: $($paths.Final)"

    return $paths
}

if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($FeatureArea)) {
        throw "FeatureArea is required."
    }
    if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
        $OutputRoot = $script:DefaultOutputRoot
    }
    $null = Invoke-OpenApiInheritance `
        -FeatureArea $FeatureArea `
        -InputPath $InputPath `
        -OutputRoot $OutputRoot
}
