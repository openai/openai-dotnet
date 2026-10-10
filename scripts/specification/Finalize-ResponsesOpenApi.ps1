<#
.SYNOPSIS
    Finalizes the inherited /responses OpenAPI document.

.DESCRIPTION
    Reads responses.openapi.json by default, validates the optional manually
    maintained policy component map, regenerates the complete default component
    map, applies the resolved maps, and atomically republishes the deduplicated
    result as responses.openapi.json. Run Generate-ResponsesOpenApi.ps1 and
    Inherit-ResponsesOpenApi.ps1 first when upstream inputs change.

.PARAMETER InputPath
    The inherited and sorted Responses OpenAPI input. Defaults to
    responses.openapi.json in OutputDirectory.

.PARAMETER OutputDirectory
    Directory containing the default responses.openapi.json input and receiving
    the generated default map, deduplicated document, and published alias.
    Defaults to specification/openapi. An explicit InputPath may be elsewhere.

.EXAMPLE
    ./scripts/specification/Finalize-ResponsesOpenApi.ps1

.EXAMPLE
    ./scripts/specification/Finalize-ResponsesOpenApi.ps1 -OutputDirectory ./artifacts/responses
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$InputPath,

    [Parameter()]
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "pipeline" "_ResponsesOpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$script:PipelineDirectory = Join-Path $PSScriptRoot "pipeline"
$script:DefaultOutputDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"

function Invoke-ResponsesOpenApiFinalization {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $OutputDirectory
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $paths.Final
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The inherited Responses OpenAPI input path is not a file: $resolvedInputPath"
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
            throw "The Responses OpenAPI finalization pipeline did not produce '$artifactPath'."
        }
    }

    $null = Publish-ResponsesOpenApiArtifact `
        -SourcePath $paths.Deduplicated `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "Responses OpenAPI finalization complete."
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
    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = $script:DefaultOutputDirectory
    }
    $null = Invoke-ResponsesOpenApiFinalization `
        -InputPath $InputPath `
        -OutputDirectory $OutputDirectory
}
