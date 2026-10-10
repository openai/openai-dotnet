<#
.SYNOPSIS
    Generates canonical inline and componentized OpenAPI documents for the
    /responses route.

.DESCRIPTION
    Runs the OpenAPI transformation stages in order for the createResponse
    operation: Extract the route, Remove omitted properties, Inline references,
    Flatten allOf, Simplify string variants, Simplify recursive schemas, Split
    multi-value object type enums, Add oneOf discriminators, Normalize oneOf
    metadata, Flatten nested oneOf wrappers, Wrap remaining singleton object type
    enums, Sort, and Componentize schemas. Each stage is retained under
    specification/openapi. The componentized result is published to
    responses.openapi.json. Run Inherit-ResponsesOpenApi.ps1 afterward.

    The generated files are:
    - responses.extracted.openapi.json
    - responses.extracted.omitted.openapi.json
    - responses.inlined.openapi.json
    - responses.inlined.flattened.openapi.json
    - responses.inlined.flattened.simplified.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.singleton-type-wrapped.openapi.json
    - responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.singleton-type-wrapped.sorted.openapi.json
    - responses.componentized.openapi.json
    - responses.openapi.json

.PARAMETER SpecificationPath
    Source OpenAPI specification. Defaults to specification/openapi/openapi.json.

.PARAMETER OutputDirectory
    Directory receiving every retained pipeline artifact. Defaults to
    specification/openapi. Pass the same directory to
    Inherit-ResponsesOpenApi.ps1 and Finalize-ResponsesOpenApi.ps1.

.EXAMPLE
    ./scripts/specification/Generate-ResponsesOpenApi.ps1

.EXAMPLE
    ./scripts/specification/Generate-ResponsesOpenApi.ps1 `
        -SpecificationPath ./specification/openapi/openapi.json `
        -OutputDirectory ./artifacts/responses
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$OutputDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "pipeline" "_ResponsesOpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$script:PipelineDirectory = Join-Path $PSScriptRoot "pipeline"
$script:DefaultSpecificationPath = Join-Path $script:RepositoryRoot "specification" "openapi" "openapi.json"
$script:DefaultOutputDirectory = Join-Path $script:RepositoryRoot "specification" "openapi"

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
        "Remove-OpenApiOmittedProperties.ps1"
        "Inline-OpenApiReferences.ps1"
        "Flatten-OpenApiAllOf.ps1"
        "Simplify-OpenApiStringVariants.ps1"
        "Simplify-OpenApiRecursiveSchemas.ps1"
        "Split-OpenApiMultiValueTypeEnums.ps1"
        "Add-OpenApiOneOfDiscriminators.ps1"
        "Normalize-OpenApiOneOf.ps1"
        "Flatten-OpenApiOneOf.ps1"
        "Wrap-OpenApiSingletonTypeEnums.ps1"
        "Sort-OpenApiDocument.ps1"
        "Componentize-OpenApiObjectSchemas.ps1"
    )
    $stageScriptPaths = [ordered]@{}
    foreach ($stageScriptName in $stageScriptNames) {
        $stageScriptPath = Join-Path $script:PipelineDirectory $stageScriptName
        if (-not (Test-Path -LiteralPath $stageScriptPath -PathType Leaf)) {
            throw "The OpenAPI stage script was not found: $stageScriptPath"
        }
        $stageScriptPaths[$stageScriptName] = $stageScriptPath
    }

    $paths = Get-ResponsesOpenApiArtifactPaths -OutputDirectory $OutputDirectory
    $fullOutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
    $null = [System.IO.Directory]::CreateDirectory($fullOutputDirectory)
    foreach ($obsoleteFileName in @(
        "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.openapi.json",
        "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.openapi.json",
        "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.oneof-flattened.openapi.json",
        "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.oneof-flattened.sorted.openapi.json",
        "responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.sorted.openapi.json",
        "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.sorted.openapi.json"
    )) {
        $obsoletePath = Join-Path $fullOutputDirectory $obsoleteFileName
        if ([System.IO.File]::Exists($obsoletePath)) {
            [System.IO.File]::Delete($obsoletePath)
        }
    }

    $null = & $stageScriptPaths["Extract-OpenApiRoute.ps1"] `
        -SpecificationPath $resolvedSpecificationPath `
        -OperationId "createResponse" `
        -Mode RequestBody `
        -OutputPath $paths.Extracted
    $null = & $stageScriptPaths["Remove-OpenApiOmittedProperties.ps1"] `
        -InputPath $paths.Extracted `
        -OutputPath $paths.Omitted
    $null = & $stageScriptPaths["Inline-OpenApiReferences.ps1"] `
        -InputPath $paths.Omitted `
        -OutputPath $paths.Inlined
    $null = & $stageScriptPaths["Flatten-OpenApiAllOf.ps1"] `
        -InputPath $paths.Inlined `
        -OutputPath $paths.Flattened
    $null = & $stageScriptPaths["Simplify-OpenApiStringVariants.ps1"] `
        -InputPath $paths.Flattened `
        -OutputPath $paths.Simplified
    $null = & $stageScriptPaths["Simplify-OpenApiRecursiveSchemas.ps1"] `
        -InputPath $paths.Simplified `
        -OutputPath $paths.RecursiveSimplified
    $null = & $stageScriptPaths["Split-OpenApiMultiValueTypeEnums.ps1"] `
        -InputPath $paths.RecursiveSimplified `
        -OutputPath $paths.TypeEnumSplit
    $null = & $stageScriptPaths["Add-OpenApiOneOfDiscriminators.ps1"] `
        -InputPath $paths.TypeEnumSplit `
        -OutputPath $paths.OneOfDiscriminated
    $null = & $stageScriptPaths["Normalize-OpenApiOneOf.ps1"] `
        -InputPath $paths.OneOfDiscriminated `
        -OutputPath $paths.OneOfNormalized
    $null = & $stageScriptPaths["Flatten-OpenApiOneOf.ps1"] `
        -InputPath $paths.OneOfNormalized `
        -OutputPath $paths.OneOfFlattened
    $null = & $stageScriptPaths["Wrap-OpenApiSingletonTypeEnums.ps1"] `
        -InputPath $paths.OneOfFlattened `
        -OutputPath $paths.SingletonTypeWrapped
    $null = & $stageScriptPaths["Sort-OpenApiDocument.ps1"] `
        -InputPath $paths.SingletonTypeWrapped `
        -OutputPath $paths.Sorted

    foreach ($artifactPath in @(
        $paths.Extracted,
        $paths.Omitted,
        $paths.Inlined,
        $paths.Flattened,
        $paths.Simplified,
        $paths.RecursiveSimplified,
        $paths.TypeEnumSplit,
        $paths.OneOfDiscriminated,
        $paths.OneOfNormalized,
        $paths.OneOfFlattened,
        $paths.SingletonTypeWrapped,
        $paths.Sorted
    )) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The responses OpenAPI pipeline did not produce '$artifactPath'."
        }
    }

    $null = & $stageScriptPaths["Componentize-OpenApiObjectSchemas.ps1"] `
        -InputPath $paths.Sorted `
        -OutputPath $paths.Componentized

    if (-not [System.IO.File]::Exists($paths.Componentized)) {
        throw "The responses OpenAPI generation pipeline did not produce '$($paths.Componentized)'."
    }
    $null = Publish-ResponsesOpenApiArtifact `
        -SourcePath $paths.Componentized `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "Responses OpenAPI generation through componentization complete."
    Write-Host "Sorted inline output: $($paths.Sorted)"
    Write-Host "Componentized output: $($paths.Componentized)"
    Write-Host "Published output: $($paths.Final)"

    return $paths
}

# Dot-sourcing exposes the orchestration functions to tests without running the
# generation pipeline.
if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($SpecificationPath)) {
        $SpecificationPath = $script:DefaultSpecificationPath
    }
    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = $script:DefaultOutputDirectory
    }
    $null = Invoke-ResponsesOpenApiGeneration `
        -SpecificationPath $SpecificationPath `
        -OutputDirectory $OutputDirectory
}
