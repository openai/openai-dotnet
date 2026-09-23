<#
.SYNOPSIS
    Extracts one operation from an OpenAPI JSON document.

.DESCRIPTION
    Reads an OpenAPI document and selects one HTTP operation by operationId.
    References are expanded except where a reference is needed to represent
    recursion, while source allOf compositions are preserved. Run
    Flatten-OpenApiAllOf.ps1 against Inlined output to merge all allOf
    compositions, Simplify-OpenApiStringVariants.ps1 to consolidate eligible
    nested string unions, Simplify-OpenApiRecursiveSchemas.ps1 to replace
    recursive objects, Normalize-OpenApiOneOf.ps1 to remove orphaned
    discriminator metadata, then Sort-OpenApiDocument.ps1 to produce canonical
    output.

    Extraction retains path-level metadata and only the selected HTTP operation.
    It omits configured properties and every name that starts with x-, while
    preserving encounter order.

.PARAMETER SpecificationPath
    The OpenAPI JSON document to read. Relative paths are resolved from the
    repository root.

.PARAMETER OperationId
    The exact operationId to extract. When omitted, the script displays an
    interactive operationId menu.

.PARAMETER OutputPath
    The destination JSON file. When omitted, the script writes beneath
    artifacts/openapi-extracts using a name derived from the operationId.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1
    Interactively selects and extracts a route.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 -OperationId createSpeech
    Extracts and inlines the createSpeech operation without prompting.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 -OperationId createResponse
    ./scripts/Flatten-OpenApiAllOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.openapi.json
    ./scripts/Simplify-OpenApiStringVariants.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.openapi.json
    ./scripts/Simplify-OpenApiRecursiveSchemas.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.openapi.json
    ./scripts/Normalize-OpenApiOneOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.openapi.json
    ./scripts/Sort-OpenApiDocument.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-normalized.openapi.json
    Extracts, simplifies, normalizes, and sorts an operation in six explicit stages.
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$OperationId,

    [Parameter()]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$helperPath = Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1"
if (-not (Test-Path -LiteralPath $helperPath -PathType Leaf)) {
    throw "The shared OpenAPI helper script was not found: $helperPath"
}
. $helperPath

# Dot-sourcing exposes the implementation functions to tests without running
# the interactive command.
if ($MyInvocation.InvocationName -ne ".") {
    Invoke-OpenApiOperationExtractor `
        -SpecificationPath $SpecificationPath `
        -OperationId $OperationId `
        -OutputPath $OutputPath
}
