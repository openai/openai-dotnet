<#
.SYNOPSIS
    Generates canonical inline and componentized OpenAPI documents for one
    feature area and operation.

.DESCRIPTION
    Extracts the selected operation and runs the OpenAPI transformation stages
    through componentization. Every retained artifact is written beneath
    <OutputRoot>/<FeatureArea> with a feature prefix and sequence number from 01
    through 15. The componentized result is atomically published as
    <FeatureArea>.openapi.json. Run Inherit-OpenApi.ps1 afterward; its retained
    artifacts continue the sequence at 16 and 17.

.PARAMETER FeatureArea
    Lowercase kebab-case feature name used for the output directory and artifact
    prefix, for example 'responses' or 'chat-completions'.

.PARAMETER OperationId
    Exact operationId to extract from the source specification.

.PARAMETER Mode
    Extracted operation content: RequestBody, Responses, or Both. Defaults to
    RequestBody.

.PARAMETER SpecificationPath
    Source OpenAPI specification. Defaults to specification/openapi/openapi.json.

.PARAMETER OutputRoot
    Root directory beneath which the feature-area directory is created. Defaults
    to specification/openapi.

.EXAMPLE
    ./scripts/specification/Generate-OpenApi.ps1 `
        -FeatureArea responses `
        -OperationId createResponse

.EXAMPLE
    ./scripts/specification/Generate-OpenApi.ps1 `
        -FeatureArea chat-completions `
        -OperationId createChatCompletion `
        -Mode Both `
        -OutputRoot ./artifacts/openapi
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$FeatureArea,

    [Parameter()]
    [string]$OperationId,

    [Parameter()]
    [ValidateSet("RequestBody", "Responses", "Both")]
    [string]$Mode = "RequestBody",

    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$OutputRoot
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot "pipeline" "_OpenApiPipeline.Helpers.ps1")

$script:RepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".."))
$script:PipelineDirectory = Join-Path $PSScriptRoot "pipeline"
$script:DefaultSpecificationPath = Join-Path $script:RepositoryRoot "specification" "openapi" "openapi.json"
$script:DefaultOutputRoot = Join-Path $script:RepositoryRoot "specification" "openapi"

function Invoke-OpenApiGeneration {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FeatureArea,

        [Parameter(Mandatory = $true)]
        [string]$OperationId,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "RequestBody",

        [Parameter(Mandatory = $true)]
        [string]$SpecificationPath,

        [Parameter(Mandatory = $true)]
        [string]$OutputRoot
    )

    Assert-OpenApiFeatureArea -FeatureArea $FeatureArea
    if ([string]::IsNullOrWhiteSpace($OperationId)) {
        throw "OperationId is required."
    }

    $resolvedSpecificationPath = (Resolve-Path -LiteralPath $SpecificationPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedSpecificationPath)) {
        throw "The OpenAPI specification path is not a file: $resolvedSpecificationPath"
    }

    $stageScriptNames = @(
        "Extract-OpenApiRoute.ps1"
        "Remove-OpenApiOmittedProperties.ps1"
        "Inline-OpenApiReferences.ps1"
        "Normalize-OpenApiTypeArrays.ps1"
        "Flatten-OpenApiAllOf.ps1"
        "Simplify-OpenApiStringVariants.ps1"
        "Simplify-OpenApiRecursiveSchemas.ps1"
        "Require-OpenApiTypeProperties.ps1"
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

    $paths = Get-OpenApiArtifactPaths -FeatureArea $FeatureArea -OutputRoot $OutputRoot
    $null = [System.IO.Directory]::CreateDirectory($paths.OutputDirectory)

    $null = & $stageScriptPaths["Extract-OpenApiRoute.ps1"] `
        -SpecificationPath $resolvedSpecificationPath `
        -OperationId $OperationId `
        -Mode $Mode `
        -OutputPath $paths.Extracted
    $null = & $stageScriptPaths["Remove-OpenApiOmittedProperties.ps1"] `
        -InputPath $paths.Extracted `
        -OutputPath $paths.Omitted
    $null = & $stageScriptPaths["Inline-OpenApiReferences.ps1"] `
        -InputPath $paths.Omitted `
        -OutputPath $paths.Inlined
    $null = & $stageScriptPaths["Normalize-OpenApiTypeArrays.ps1"] `
        -InputPath $paths.Inlined `
        -OutputPath $paths.TypeArraysNormalized
    $null = & $stageScriptPaths["Flatten-OpenApiAllOf.ps1"] `
        -InputPath $paths.TypeArraysNormalized `
        -OutputPath $paths.Flattened
    $null = & $stageScriptPaths["Simplify-OpenApiStringVariants.ps1"] `
        -InputPath $paths.Flattened `
        -OutputPath $paths.Simplified
    $null = & $stageScriptPaths["Simplify-OpenApiRecursiveSchemas.ps1"] `
        -InputPath $paths.Simplified `
        -OutputPath $paths.RecursiveSimplified
    $null = & $stageScriptPaths["Require-OpenApiTypeProperties.ps1"] `
        -InputPath $paths.RecursiveSimplified `
        -OutputPath $paths.TypePropertiesRequired
    $null = & $stageScriptPaths["Split-OpenApiMultiValueTypeEnums.ps1"] `
        -InputPath $paths.TypePropertiesRequired `
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
        $paths.TypeArraysNormalized,
        $paths.Flattened,
        $paths.Simplified,
        $paths.RecursiveSimplified,
        $paths.TypePropertiesRequired,
        $paths.TypeEnumSplit,
        $paths.OneOfDiscriminated,
        $paths.OneOfNormalized,
        $paths.OneOfFlattened,
        $paths.SingletonTypeWrapped,
        $paths.Sorted
    )) {
        if (-not [System.IO.File]::Exists($artifactPath)) {
            throw "The '$FeatureArea' OpenAPI pipeline did not produce '$artifactPath'."
        }
    }

    $null = & $stageScriptPaths["Componentize-OpenApiObjectSchemas.ps1"] `
        -InputPath $paths.Sorted `
        -OutputPath $paths.Componentized

    if (-not [System.IO.File]::Exists($paths.Componentized)) {
        throw "The '$FeatureArea' OpenAPI generation pipeline did not produce '$($paths.Componentized)'."
    }
    $null = Publish-OpenApiArtifact `
        -SourcePath $paths.Componentized `
        -DestinationPath $paths.Final

    Write-Host ""
    Write-Host "OpenAPI generation through componentization complete for '$FeatureArea'."
    Write-Host "OperationId: $OperationId"
    Write-Host "Mode: $Mode"
    Write-Host "Sorted inline output: $($paths.Sorted)"
    Write-Host "Componentized output: $($paths.Componentized)"
    Write-Host "Published output: $($paths.Final)"

    return $paths
}

# Dot-sourcing exposes the orchestration functions to tests without running the
# generation pipeline.
if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($FeatureArea)) {
        throw "FeatureArea is required."
    }
    if ([string]::IsNullOrWhiteSpace($OperationId)) {
        throw "OperationId is required."
    }
    if ([string]::IsNullOrWhiteSpace($SpecificationPath)) {
        $SpecificationPath = $script:DefaultSpecificationPath
    }
    if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
        $OutputRoot = $script:DefaultOutputRoot
    }
    $null = Invoke-OpenApiGeneration `
        -FeatureArea $FeatureArea `
        -OperationId $OperationId `
        -Mode $Mode `
        -SpecificationPath $SpecificationPath `
        -OutputRoot $OutputRoot
}
