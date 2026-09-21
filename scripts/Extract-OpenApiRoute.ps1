<#
.SYNOPSIS
    Extracts one route from an OpenAPI JSON document.

.DESCRIPTION
    Reads an OpenAPI document, groups routes by their first path segment, and
    guides the user through selecting a feature area, route, and extraction
    mode. Exact mode keeps references and copies their component definitions.
    Inlined mode expands references except where a reference is needed to
    represent recursion, but preserves source allOf compositions. Run
    Flatten-OpenApiAllOf.ps1 against Inlined output to produce a document with
    all allOf compositions merged into flat schemas.

    Both modes omit configured properties and every name that starts with x-,
    order schema keys, required arrays, and property maps using their configured
    priorities, and emit enum values in alphabetical order.

.PARAMETER SpecificationPath
    The OpenAPI JSON document to read. Relative paths are resolved from the
    repository root.

.PARAMETER Route
    The exact key under the OpenAPI paths object. When omitted, the script
    displays interactive feature-area and route menus.

.PARAMETER Mode
    The extraction mode: Exact or Inlined. When omitted, the script displays
    an interactive mode menu.

.PARAMETER OutputPath
    The destination JSON file. When omitted, the script writes beneath
    artifacts/openapi-extracts using a name derived from the route and mode.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1
    Interactively selects and extracts a route.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 -Route "/audio/speech" -Mode Exact
    Extracts /audio/speech without prompting for a route or mode.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 -Route "/responses" -Mode Inlined
    ./scripts/Flatten-OpenApiAllOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.openapi.json
    Extracts an Inlined intermediate and then flattens its allOf compositions.
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$Route,

    [Parameter()]
    [ValidateSet("Exact", "Inlined")]
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
    Invoke-OpenApiRouteExtractor `
        -SpecificationPath $SpecificationPath `
        -Route $Route `
        -Mode $Mode `
        -OutputPath $OutputPath
}
