<#
.SYNOPSIS
    Extracts one operation from an OpenAPI JSON document.

.DESCRIPTION
    Reads an OpenAPI document and selects one HTTP operation by operationId.
    The selected operation can include its request body, responses, or both.
    References are expanded except where a reference is needed to represent
    recursion, while source allOf compositions are preserved. Run
    Flatten-OpenApiAllOf.ps1 against Inlined output to merge all allOf
    compositions, Simplify-OpenApiStringVariants.ps1 to consolidate eligible
    nested string unions, Simplify-OpenApiRecursiveSchemas.ps1 to replace
    recursive objects, Add-OpenApiOneOfDiscriminators.ps1 to infer missing type
    discriminators, Normalize-OpenApiOneOf.ps1 to remove orphaned discriminator
    metadata, Flatten-OpenApiOneOf.ps1 to promote matching nested wrappers, then
    Sort-OpenApiDocument.ps1 to produce canonical output.

    Extraction retains path-level metadata and only the selected HTTP operation.
    It omits configured metadata and every name that starts with x-, while
    preserving required OpenAPI members, schema-map data names, and encounter
    order.

.PARAMETER SpecificationPath
    The OpenAPI JSON document to read. Relative paths are resolved from the
    repository root.

.PARAMETER OperationId
    The exact operationId to extract. When omitted, the script displays an
    interactive operationId menu.

.PARAMETER Mode
    Selects RequestBody, Responses, or Both. When omitted, the script displays
    an interactive content menu. Both preserves the complete selected operation.
    RequestBody replaces responses with a reference-free default placeholder so
    the focused output remains a conformant OpenAPI operation.

.PARAMETER OutputPath
    The destination JSON file. When omitted, the script writes beneath
    artifacts/openapi-extracts using a name derived from the operationId.
    RequestBody and Responses outputs include `.request-body` and `.responses`
    markers respectively; Both retains the existing filename.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1
    Interactively selects an operation and its extraction content.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 `
        -OperationId createSpeech `
        -Mode Responses
    Extracts and inlines only the response side of createSpeech without prompting.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 `
        -OperationId createResponse `
        -Mode Both
    ./scripts/Flatten-OpenApiAllOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.openapi.json
    ./scripts/Simplify-OpenApiStringVariants.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.openapi.json
    ./scripts/Simplify-OpenApiRecursiveSchemas.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.openapi.json
    ./scripts/Add-OpenApiOneOfDiscriminators.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.openapi.json
    ./scripts/Normalize-OpenApiOneOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.openapi.json
    ./scripts/Flatten-OpenApiOneOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.openapi.json
    ./scripts/Sort-OpenApiDocument.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.oneof-flattened.openapi.json
    Noninteractively extracts both sides, then simplifies, normalizes, and sorts
    the operation in eight explicit stages.
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$OperationId,

    [Parameter()]
    [AllowNull()]
    [ValidateSet("RequestBody", "Responses", "Both")]
    [string]$Mode,

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
        -Mode $Mode `
        -OutputPath $OutputPath
}
