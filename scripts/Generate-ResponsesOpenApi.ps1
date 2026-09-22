<#
.SYNOPSIS
    Generates the canonical inline OpenAPI document for the /responses route.

.DESCRIPTION
    Runs the OpenAPI transformation stages in order for the createResponse
    operation: Extract with references inlined, Flatten allOf, Simplify string
    variants, Normalize oneOf metadata, and Sort. Each stage is retained under
    specification/openapi, and the sorted result is also copied byte-for-byte
    to responses.openapi.json.

    The generated files are:
    - responses.inlined.openapi.json
    - responses.inlined.flattened.openapi.json
    - responses.inlined.flattened.simplified.openapi.json
    - responses.inlined.flattened.simplified.oneof-normalized.openapi.json
    - responses.inlined.flattened.simplified.oneof-normalized.sorted.openapi.json
    - responses.openapi.json

.EXAMPLE
    ./scripts/Generate-ResponsesOpenApi.ps1
#>

#Requires -Version 7.0

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultSpecificationPath = Join-Path $script:RepositoryRoot "specification" "openapi" "openapi.json"
$script:DefaultOutputDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"

function Get-ResponsesOpenApiArtifactPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $fullOutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
    return [PSCustomObject]@{
        Inlined = Join-Path $fullOutputDirectory "responses.inlined.openapi.json"
        Flattened = Join-Path $fullOutputDirectory "responses.inlined.flattened.openapi.json"
        Simplified = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.openapi.json"
        OneOfNormalized = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.oneof-normalized.openapi.json"
        Sorted = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.oneof-normalized.sorted.openapi.json"
        Final = Join-Path $fullOutputDirectory "responses.openapi.json"
    }
}

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

function Invoke-ResponsesOpenApiGeneration {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SpecificationPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $resolvedSpecificationPath = (Resolve-Path -LiteralPath $SpecificationPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedSpecificationPath)) {
        throw "The OpenAPI specification path is not a file: $resolvedSpecificationPath"
    }

    $stageScriptNames = @(
        "Extract-OpenApiRoute.ps1"
        "Flatten-OpenApiAllOf.ps1"
        "Simplify-OpenApiStringVariants.ps1"
        "Normalize-OpenApiOneOf.ps1"
        "Sort-OpenApiDocument.ps1"
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
    $null = [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetFullPath($OutputDirectory))

    $null = & $stageScriptPaths["Extract-OpenApiRoute.ps1"] `
        -SpecificationPath $resolvedSpecificationPath `
        -OperationId "createResponse" `
        -OutputPath $paths.Inlined
    $null = & $stageScriptPaths["Flatten-OpenApiAllOf.ps1"] `
        -InputPath $paths.Inlined `
        -OutputPath $paths.Flattened
    $null = & $stageScriptPaths["Simplify-OpenApiStringVariants.ps1"] `
        -InputPath $paths.Flattened `
        -OutputPath $paths.Simplified
    $null = & $stageScriptPaths["Normalize-OpenApiOneOf.ps1"] `
        -InputPath $paths.Simplified `
        -OutputPath $paths.OneOfNormalized
    $null = & $stageScriptPaths["Sort-OpenApiDocument.ps1"] `
        -InputPath $paths.OneOfNormalized `
        -OutputPath $paths.Sorted
    $null = Copy-OpenApiFileAtomically `
        -SourcePath $paths.Sorted `
        -DestinationPath $paths.Final

    foreach ($artifactPath in @(
        $paths.Inlined,
        $paths.Flattened,
        $paths.Simplified,
        $paths.OneOfNormalized,
        $paths.Sorted,
        $paths.Final
    )) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The responses OpenAPI pipeline did not produce '$artifactPath'."
        }
    }

    [byte[]]$sortedBytes = [System.IO.File]::ReadAllBytes($paths.Sorted)
    [byte[]]$finalBytes = [System.IO.File]::ReadAllBytes($paths.Final)
    if (-not [System.Linq.Enumerable]::SequenceEqual($sortedBytes, $finalBytes)) {
        throw "The final responses.openapi.json file differs from the retained Sort-stage output."
    }

    Write-Host ""
    Write-Host "Responses OpenAPI generation complete."
    Write-Host "Final output: $($paths.Final)"

    return $paths
}

# Dot-sourcing exposes the orchestration functions to tests without running the
# generation pipeline.
if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-ResponsesOpenApiGeneration `
        -SpecificationPath $script:DefaultSpecificationPath `
        -OutputDirectory $script:DefaultOutputDirectory
}