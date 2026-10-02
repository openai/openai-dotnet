<#
.SYNOPSIS
    Finalizes the componentized /responses OpenAPI document.

.DESCRIPTION
    Starts from responses.componentized.openapi.json in the target output
    directory, reconciles the persisted component deduplication map, applies
    component deduplication, and atomically publishes the result as
    responses.openapi.json. Run Generate-ResponsesOpenApi.ps1 first whenever an
    upstream transformation or the source OpenAPI specification changes.

.PARAMETER OutputDirectory
    Directory containing responses.componentized.openapi.json and receiving the
    deduplication map, deduplicated document, and final published artifact.
    Defaults to specification/openapi.

.PARAMETER ResetDeduplicationMap
    Deletes the target component deduplication map after the componentized input
    and required stage scripts pass preflight, then bootstraps a fresh mapping.
    Omitting this switch preserves and reconciles existing reviewed mapping IDs.
    Reset is destructive and does not restore the old map if fresh analysis
    subsequently fails.

.EXAMPLE
    ./scripts/Finalize-ResponsesOpenApi.ps1

.EXAMPLE
    ./scripts/Finalize-ResponsesOpenApi.ps1 -ResetDeduplicationMap

.EXAMPLE
    ./scripts/Finalize-ResponsesOpenApi.ps1 -OutputDirectory ./artifacts/responses
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$OutputDirectory,

    [Parameter()]
    [switch]$ResetDeduplicationMap
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "_ResponsesOpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultOutputDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"

function Copy-OpenApiFileAtomically {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourcePath,

        [Parameter(Mandatory = $true)]
        [string]$DestinationPath
    )

    $resolvedSourcePath = (Resolve-Path -LiteralPath $SourcePath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedSourcePath)) {
        throw "The OpenAPI source path is not a file: $resolvedSourcePath"
    }

    $fullDestinationPath = [System.IO.Path]::GetFullPath($DestinationPath)
    $destinationDirectory = [System.IO.Path]::GetDirectoryName($fullDestinationPath)
    $null = [System.IO.Directory]::CreateDirectory($destinationDirectory)
    $temporaryPath = Join-Path $destinationDirectory ".$([System.IO.Path]::GetFileName($fullDestinationPath)).$([Guid]::NewGuid().ToString('N')).tmp"

    try {
        [System.IO.File]::Copy($resolvedSourcePath, $temporaryPath, $true)
        [System.IO.File]::Move($temporaryPath, $fullDestinationPath, $true)
    }
    finally {
        if ([System.IO.File]::Exists($temporaryPath)) {
            [System.IO.File]::Delete($temporaryPath)
        }
    }

    return $fullDestinationPath
}

function Reset-ResponsesOpenApiDeduplicationMap {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if (-not (Test-Path -LiteralPath $fullPath)) {
        return
    }
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        throw "Cannot reset the component deduplication map because its path is not a file: $fullPath"
    }
    [System.IO.File]::Delete($fullPath)
}

function Invoke-ResponsesOpenApiFinalization {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory,

        [Parameter()]
        [switch]$ResetDeduplicationMap
    )

    $stageScriptNames = @(
        "Update-OpenApiComponentDeduplicationMap.ps1"
        "Deduplicate-OpenApiComponents.ps1"
    )
    $stageScriptPaths = [ordered]@{}
    foreach ($stageScriptName in $stageScriptNames) {
        $stageScriptPath = Join-Path $PSScriptRoot $stageScriptName
        if (-not (Test-Path -LiteralPath $stageScriptPath -PathType Leaf)) {
            throw "The OpenAPI stage script was not found: $stageScriptPath"
        }
        $stageScriptPaths[$stageScriptName] = $stageScriptPath
    }

    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $OutputDirectory
    if (-not (Test-Path -LiteralPath $paths.Componentized)) {
        throw "The componentized Responses OpenAPI input was not found: $($paths.Componentized)"
    }
    if (-not (Test-Path -LiteralPath $paths.Componentized -PathType Leaf)) {
        throw "The componentized Responses OpenAPI input path is not a file: $($paths.Componentized)"
    }

    if ($ResetDeduplicationMap) {
        Reset-ResponsesOpenApiDeduplicationMap -Path $paths.DeduplicationMap
    }

    $null = & $stageScriptPaths["Update-OpenApiComponentDeduplicationMap.ps1"] `
        -InputPath $paths.Componentized `
        -MappingPath $paths.DeduplicationMap
    $null = & $stageScriptPaths["Deduplicate-OpenApiComponents.ps1"] `
        -InputPath $paths.Componentized `
        -MappingPath $paths.DeduplicationMap `
        -OutputPath $paths.Deduplicated

    foreach ($artifactPath in @($paths.DeduplicationMap, $paths.Deduplicated)) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The Responses OpenAPI finalization pipeline did not produce '$artifactPath'."
        }
    }

    $null = Copy-OpenApiFileAtomically `
        -SourcePath $paths.Deduplicated `
        -DestinationPath $paths.Final
    [byte[]]$deduplicatedBytes = [System.IO.File]::ReadAllBytes($paths.Deduplicated)
    [byte[]]$finalBytes = [System.IO.File]::ReadAllBytes($paths.Final)
    if (-not [System.Linq.Enumerable]::SequenceEqual($deduplicatedBytes, $finalBytes)) {
        throw "The final responses.openapi.json file differs from the retained deduplicated output."
    }

    Write-Host ""
    Write-Host "Responses OpenAPI finalization complete."
    Write-Host "Componentized input: $($paths.Componentized)"
    Write-Host "Deduplication map: $($paths.DeduplicationMap)"
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
        -OutputDirectory $OutputDirectory `
        -ResetDeduplicationMap:$ResetDeduplicationMap
}
