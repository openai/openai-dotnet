#Requires -Version 7.0

function Assert-OpenApiFeatureArea {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FeatureArea
    )

    if (-not [System.Text.RegularExpressions.Regex]::IsMatch(
        $FeatureArea,
        "\A[a-z][a-z0-9-]*\z",
        [System.Text.RegularExpressions.RegexOptions]::CultureInvariant
    )) {
        throw "Feature area '$FeatureArea' must be lowercase kebab-case matching '^[a-z][a-z0-9-]*$'."
    }
}

function Get-OpenApiArtifactPaths {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FeatureArea,

        [Parameter(Mandatory = $true)]
        [string]$OutputRoot
    )

    Assert-OpenApiFeatureArea -FeatureArea $FeatureArea
    $fullOutputRoot = [System.IO.Path]::GetFullPath($OutputRoot)
    $outputDirectory = Join-Path $fullOutputRoot $FeatureArea
    return [PSCustomObject]@{
        FeatureArea = $FeatureArea
        OutputRoot = $fullOutputRoot
        OutputDirectory = $outputDirectory
        Extracted = Join-Path $outputDirectory "$FeatureArea.01.route-extracted.openapi.json"
        Omitted = Join-Path $outputDirectory "$FeatureArea.02.properties-omitted.openapi.json"
        Inlined = Join-Path $outputDirectory "$FeatureArea.03.references-inlined.openapi.json"
        Flattened = Join-Path $outputDirectory "$FeatureArea.04.allof-flattened.openapi.json"
        Simplified = Join-Path $outputDirectory "$FeatureArea.05.string-variants-simplified.openapi.json"
        RecursiveSimplified = Join-Path $outputDirectory "$FeatureArea.06.recursive-schemas-simplified.openapi.json"
        TypeEnumSplit = Join-Path $outputDirectory "$FeatureArea.07.multi-value-type-enums-split.openapi.json"
        OneOfDiscriminated = Join-Path $outputDirectory "$FeatureArea.08.oneof-discriminators-added.openapi.json"
        OneOfNormalized = Join-Path $outputDirectory "$FeatureArea.09.oneof-normalized.openapi.json"
        OneOfFlattened = Join-Path $outputDirectory "$FeatureArea.10.nested-oneof-flattened.openapi.json"
        SingletonTypeWrapped = Join-Path $outputDirectory "$FeatureArea.11.singleton-type-enums-wrapped.openapi.json"
        Sorted = Join-Path $outputDirectory "$FeatureArea.12.document-sorted.openapi.json"
        Componentized = Join-Path $outputDirectory "$FeatureArea.13.schemas-componentized.openapi.json"
        DiscriminatorInherited = Join-Path $outputDirectory "$FeatureArea.14.discriminator-inheritance-refactored.openapi.json"
        DiscriminatorInheritedSorted = Join-Path $outputDirectory "$FeatureArea.15.inherited-document-sorted.openapi.json"
        DefaultComponentMap = Join-Path $outputDirectory "$FeatureArea.component-default-map.json"
        PolicyComponentMap = Join-Path $outputDirectory "$FeatureArea.component-policy-map.json"
        Deduplicated = Join-Path $outputDirectory "$FeatureArea.16.components-deduplicated.openapi.json"
        Final = Join-Path $outputDirectory "$FeatureArea.openapi.json"
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

function Publish-OpenApiArtifact {
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
        throw "Published OpenAPI artifact '$writtenPath' differs from retained artifact '$SourcePath'."
    }
    return $writtenPath
}