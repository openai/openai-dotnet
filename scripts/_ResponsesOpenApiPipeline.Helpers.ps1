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
        DeduplicationMap = Join-Path $fullOutputDirectory "responses.component-deduplication-map.json"
        Deduplicated = Join-Path $fullOutputDirectory "responses.componentized.deduplicated.openapi.json"
        Final = Join-Path $fullOutputDirectory "responses.openapi.json"
    }
}
