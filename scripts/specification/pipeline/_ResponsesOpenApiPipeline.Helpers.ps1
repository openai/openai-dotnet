#Requires -Version 7.0

function Get-ResponsesOpenApiArtifactPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OutputDirectory
    )

    $fullOutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
    return [PSCustomObject]@{
        Extracted = Join-Path $fullOutputDirectory "responses.extracted.openapi.json"
        Omitted = Join-Path $fullOutputDirectory "responses.extracted.omitted.openapi.json"
        Inlined = Join-Path $fullOutputDirectory "responses.inlined.openapi.json"
        Flattened = Join-Path $fullOutputDirectory "responses.inlined.flattened.openapi.json"
        Simplified = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.openapi.json"
        RecursiveSimplified = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.openapi.json"
        TypeEnumSplit = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.openapi.json"
        OneOfDiscriminated = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.openapi.json"
        OneOfNormalized = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.openapi.json"
        OneOfFlattened = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.openapi.json"
        SingletonTypeWrapped = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.singleton-type-wrapped.openapi.json"
        Sorted = Join-Path $fullOutputDirectory "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.singleton-type-wrapped.sorted.openapi.json"
        Componentized = Join-Path $fullOutputDirectory "responses.componentized.openapi.json"
        DiscriminatorInherited = Join-Path $fullOutputDirectory "responses.componentized.discriminator-inherited.openapi.json"
        DiscriminatorInheritedSorted = Join-Path $fullOutputDirectory "responses.componentized.discriminator-inherited.sorted.openapi.json"
        DefaultComponentMap = Join-Path $fullOutputDirectory "responses.component-default-map.json"
        PolicyComponentMap = Join-Path $fullOutputDirectory "responses.component-policy-map.json"
        Deduplicated = Join-Path $fullOutputDirectory "responses.componentized.deduplicated.openapi.json"
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

function Publish-ResponsesOpenApiArtifact {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourcePath,

        [Parameter(Mandatory = $true)]
        [string]$DestinationPath
    )

    $writtenPath = Copy-OpenApiFileAtomically `
        -SourcePath $SourcePath `
        -DestinationPath $DestinationPath
    [byte[]]$sourceBytes = [System.IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $SourcePath).ProviderPath)
    [byte[]]$publishedBytes = [System.IO.File]::ReadAllBytes($writtenPath)
    if (-not [System.Linq.Enumerable]::SequenceEqual($sourceBytes, $publishedBytes)) {
        throw "Published responses.openapi.json differs from retained artifact '$SourcePath'."
    }
    return $writtenPath
}
