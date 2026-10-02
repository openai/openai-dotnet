<#
.SYNOPSIS
    Finalizes the componentized /responses OpenAPI document.

.DESCRIPTION
    Starts from responses.componentized.openapi.json in the target output
    directory, validates the optional manually maintained policy component map,
    regenerates the complete default component map, applies the resolved maps,
    and atomically publishes the result as responses.openapi.json. Run
    Generate-ResponsesOpenApi.ps1 first whenever an upstream transformation or
    the source OpenAPI specification changes.

.PARAMETER OutputDirectory
    Directory containing responses.componentized.openapi.json and receiving the
    generated default map, deduplicated document, and final published artifact.
    Defaults to specification/openapi.

.EXAMPLE
    ./scripts/Finalize-ResponsesOpenApi.ps1

.EXAMPLE
    ./scripts/Finalize-ResponsesOpenApi.ps1 -OutputDirectory ./artifacts/responses
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$OutputDirectory
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

function Invoke-ResponsesOpenApiFinalization {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $stageScriptNames = @(
        "Generate-OpenApiComponentDefaultMap.ps1"
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
    if (Test-Path -LiteralPath $paths.PolicyComponentMap -PathType Container) {
        throw "The component policy map path is not a file: $($paths.PolicyComponentMap)"
    }

    $null = & $stageScriptPaths["Generate-OpenApiComponentDefaultMap.ps1"] `
        -InputPath $paths.Componentized `
        -DefaultMapPath $paths.DefaultComponentMap `
        -PolicyMapPath $paths.PolicyComponentMap
    $null = & $stageScriptPaths["Deduplicate-OpenApiComponents.ps1"] `
        -InputPath $paths.Componentized `
        -DefaultMapPath $paths.DefaultComponentMap `
        -PolicyMapPath $paths.PolicyComponentMap `
        -OutputPath $paths.Deduplicated

    foreach ($artifactPath in @($paths.DefaultComponentMap, $paths.Deduplicated)) {
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
        -OutputDirectory $OutputDirectory
}
