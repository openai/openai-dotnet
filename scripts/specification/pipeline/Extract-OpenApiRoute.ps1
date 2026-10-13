<#
.SYNOPSIS
    Extracts one operation from an OpenAPI JSON document.

.DESCRIPTION
    Reads an OpenAPI document and selects one HTTP operation by operationId.
    The selected operation can include its request body, responses, or both.
    Extraction retains path-level metadata, only the selected HTTP operation,
    selected tag definitions, and the complete component registry. Metadata,
    extensions, references, and schema compositions remain unchanged. Run
    Remove-OpenApiOmittedProperties.ps1 and Inline-OpenApiReferences.ps1 before
    the semantic transformation stages.

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
    markers respectively; every output ends in `.extracted.openapi.json`.

.EXAMPLE
    ./scripts/specification/pipeline/Extract-OpenApiRoute.ps1
    Interactively selects an operation and its extraction content.

.EXAMPLE
    ./scripts/specification/pipeline/Extract-OpenApiRoute.ps1 `
        -OperationId createSpeech `
        -Mode Responses
    Extracts only the response side of createSpeech without prompting.

.EXAMPLE
    ./scripts/specification/pipeline/Extract-OpenApiRoute.ps1 `
        -OperationId createResponse `
        -Mode Both `
        -OutputPath ./artifacts/custom.extracted.openapi.json
    Noninteractively extracts both sides to a caller-selected standalone path.
    Generate-OpenApi.ps1 owns the numbered top-level pipeline artifacts.
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
