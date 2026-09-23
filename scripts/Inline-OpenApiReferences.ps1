<#
.SYNOPSIS
    Inlines references in an extracted and omitted OpenAPI document.

.DESCRIPTION
    Expands surviving local component references after route extraction and
    contextual omission. Source schema compositions remain intact. True cycle
    back-edges are retained with minimal fallback components, discriminator
    mappings are rewritten to local branches, and only required security scheme
    components remain.

.PARAMETER InputPath
    The extracted and omitted OpenAPI JSON document to inline.

.PARAMETER OutputPath
    The destination JSON file. When omitted, an '.extracted.omitted' input is
    mapped to the corresponding '.inlined' sibling. Generic inputs receive an
    '.inlined' marker, and already-inlined input is atomically rewritten in place.

.EXAMPLE
    ./scripts/Inline-OpenApiReferences.ps1 `
        ./artifacts/openapi-extracts/responses.extracted.omitted.openapi.json
    ./scripts/Flatten-OpenApiAllOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.openapi.json
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$InputPath,

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

function Get-SingleExtractedOpenApiRoute {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $paths = Get-JsonPropertyValue -Object $Document -Name "paths"
    if (-not (Test-JsonObjectValue -Value $paths)) {
        throw "The extracted OpenAPI document must contain a paths object."
    }

    $pathProperties = @(Get-JsonProperties -Value $paths)
    if ($pathProperties.Count -ne 1) {
        throw "The extracted OpenAPI document must contain exactly one path; found $($pathProperties.Count)."
    }

    $pathItem = $pathProperties[0].Value
    if (-not (Test-JsonObjectValue -Value $pathItem)) {
        throw "The extracted path '$($pathProperties[0].Name)' must be an object."
    }

    $operations = @(Get-OpenApiOperations -PathItem $pathItem)
    if ($operations.Count -ne 1) {
        throw "The extracted path '$($pathProperties[0].Name)' must contain exactly one HTTP operation; found $($operations.Count)."
    }

    return [PSCustomObject]@{
        Route = $pathProperties[0].Name
        PathItem = $pathItem
        Method = $operations[0].Method
        Operation = $operations[0].Operation
    }
}

function ConvertTo-InlinedOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $selectedRoute = Get-SingleExtractedOpenApiRoute -Document $Document
    $routePointer = "#/paths/$(ConvertTo-JsonPointerToken -Token $selectedRoute.Route)"
    $cycleReferences = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $activeReferences = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $inlinedPathItem = Expand-OpenApiReferences `
        -Value $selectedRoute.PathItem `
        -Document $Document `
        -ActiveReferences $activeReferences `
        -CycleReferences $cycleReferences `
        -Breadcrumb $routePointer

    $fallbacks = New-InlinedFallbackComponents `
        -Document $Document `
        -CycleReferences $cycleReferences
    $componentPointers = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($pointer in $fallbacks.Keys) {
        $null = $componentPointers.Add($pointer)
    }

    $usedSecuritySchemes = Get-UsedSecuritySchemeNames `
        -Document $Document `
        -PathItem $inlinedPathItem
    Add-SecuritySchemeComponents `
        -Document $Document `
        -SecuritySchemeNames $usedSecuritySchemes `
        -ComponentPointers $componentPointers
    $selectedComponents = New-SelectedComponentSections `
        -Document $Document `
        -ComponentPointers $componentPointers `
        -Overrides $fallbacks

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Document)) {
        if ([string]::Equals($property.Name, "paths", [System.StringComparison]::Ordinal)) {
            $paths = [ordered]@{}
            $paths[$selectedRoute.Route] = $inlinedPathItem
            $result[$property.Name] = $paths
            continue
        }
        if ([string]::Equals($property.Name, "components", [System.StringComparison]::Ordinal)) {
            if ($selectedComponents.Count -gt 0) {
                $result[$property.Name] = $selectedComponents
            }
            continue
        }
        $result[$property.Name] = Copy-JsonValue -Value $property.Value
    }
    if (-not (Test-JsonProperty -Object $Document -Name "components") -and
        $selectedComponents.Count -gt 0) {
        $result["components"] = $selectedComponents
    }

    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result
    Assert-InlinedReferencePolicy `
        -Document $result `
        -CycleReferences $cycleReferences

    return [PSCustomObject]@{
        Document = $result
        DependencyCount = $componentPointers.Count
        CycleCount = $cycleReferences.Count
    }
}

function Get-DefaultInlinedOpenApiPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullInputPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullInputPath)
    $fileName = [System.IO.Path]::GetFileName($fullInputPath)
    if ($fileName.EndsWith(".inlined.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".inlined.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullInputPath
    }
    if ($fileName.EndsWith(".extracted.omitted.openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".extracted.omitted.openapi.json".Length)
        return Join-Path $directory "$baseName.inlined.openapi.json"
    }
    if ($fileName.EndsWith(".extracted.omitted.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".extracted.omitted.json".Length)
        return Join-Path $directory "$baseName.inlined.json"
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$baseName.inlined.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$baseName.inlined.json"
    }
    return Join-Path $directory "$fileName.inlined.json"
}

function Invoke-OpenApiReferenceInliner {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath,

        [Parameter()]
        [string]$OutputPath
    )

    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The OpenAPI input path is not a file: $resolvedInputPath"
    }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = Get-DefaultInlinedOpenApiPath -InputPath $resolvedInputPath
    }
    else {
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-InlinedOpenApiDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Component dependencies: $($result.DependencyCount)"
    Write-Host "Cycle references retained: $($result.CycleCount)"

    return $result
}

if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        throw "InputPath is required."
    }
    $null = Invoke-OpenApiReferenceInliner `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}
