<#
.SYNOPSIS
    Normalizes oneOf-related schema metadata in an OpenAPI document.

.DESCRIPTION
    Removes an exact discriminator schema keyword when its containing schema
    has no exact direct oneOf sibling. A discriminator paired with any direct
    oneOf property is preserved; this stage does not validate the oneOf value.

    Property-map entries named discriminator, string literals, examples, and
    ordinary OpenAPI objects are preserved. Encounter order is retained. Run
    Sort-OpenApiDocument.ps1 afterward to produce canonical ordering.

.PARAMETER InputPath
    The OpenAPI JSON document to normalize.

.PARAMETER OutputPath
    The destination JSON file. When omitted, a sibling filename is generated
    by inserting '.oneof-normalized' before '.openapi.json' or the final '.json'
    suffix. If the input already has that suffix, it is atomically rewritten
    in place.

.EXAMPLE
    ./scripts/Normalize-OpenApiOneOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.openapi.json
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

function Test-IsOrphanedOpenApiDiscriminator {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    return (Test-JsonProperty -Object $Schema -Name "discriminator") -and
        -not (Test-JsonProperty -Object $Schema -Name "oneOf")
}

# Recursively copies an OpenAPI document while removing orphaned discriminator
# keywords only from schema objects. All surviving values retain source order.
function ConvertTo-NormalizedOpenApiOneOfValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [ref]$RemovedDiscriminatorCount,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "oneOf normalization exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return $null
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        [object[]]$items = [object[]]::new($Value.Count)
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-NormalizedOpenApiOneOfValue `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" `
                -RemovedDiscriminatorCount $RemovedDiscriminatorCount `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $removeDiscriminator = [string]::Equals(
        $Context,
        "Schema",
        [System.StringComparison]::Ordinal
    ) -and (Test-IsOrphanedOpenApiDiscriminator -Schema $Value)

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ($removeDiscriminator -and
            [string]::Equals(
                $property.Name,
                "discriminator",
                [System.StringComparison]::Ordinal
            )) {
            $RemovedDiscriminatorCount.Value++
            continue
        }

        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $result[$property.Name] = ConvertTo-NormalizedOpenApiOneOfValue `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -RemovedDiscriminatorCount $RemovedDiscriminatorCount `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

function Assert-NoOrphanedOpenApiDiscriminators {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context = "OpenApi",

        [Parameter()]
        [string]$Breadcrumb = "#",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "oneOf normalization postcondition exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-NoOrphanedOpenApiDiscriminators `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        (Test-IsOrphanedOpenApiDiscriminator -Schema $Value)) {
        throw "Normalized output retained an orphaned discriminator at '$Breadcrumb/discriminator'."
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        Assert-NoOrphanedOpenApiDiscriminators `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Finds dynamic and recursive references not returned by the shared reference
# discovery helper.
function Get-OpenApiOneOfAdditionalReferenceOccurrences {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [string]$Breadcrumb = "#",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Additional reference discovery exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Get-OpenApiOneOfAdditionalReferenceOccurrences `
                -Value $Value[$index] `
                -Breadcrumb "$Breadcrumb/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $propertyBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if (@('$dynamicRef', '$recursiveRef') -ccontains $property.Name) {
            if ($property.Value -isnot [string]) {
                throw "The $($property.Name) value at '$propertyBreadcrumb' must be a string."
            }
            [PSCustomObject]@{
                Kind = $property.Name
                Pointer = $property.Value
                Breadcrumb = $propertyBreadcrumb
            }
        }
        Get-OpenApiOneOfAdditionalReferenceOccurrences `
            -Value $property.Value `
            -Breadcrumb $propertyBreadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

function Assert-AdditionalOpenApiReferencesResolveAfterOneOfNormalization {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    foreach ($occurrence in @(Get-OpenApiOneOfAdditionalReferenceOccurrences -Value $Document)) {
        if ([string]::Equals($occurrence.Pointer, "#", [System.StringComparison]::Ordinal)) {
            continue
        }
        $null = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $occurrence.Pointer `
            -Context "$($occurrence.Kind) at $($occurrence.Breadcrumb)"
    }
}

function ConvertTo-NormalizedOpenApiOneOfDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $removedDiscriminatorCount = 0
    $normalized = ConvertTo-NormalizedOpenApiOneOfValue `
        -Value $Document `
        -Context OpenApi `
        -Breadcrumb "#" `
        -RemovedDiscriminatorCount ([ref]$removedDiscriminatorCount)
    Assert-OpenApiReferencesResolve -Document $normalized
    Assert-AdditionalOpenApiReferencesResolveAfterOneOfNormalization `
        -Document $normalized
    Assert-NoOrphanedOpenApiDiscriminators -Value $normalized

    return [PSCustomObject]@{
        Document = $normalized
        RemovedDiscriminatorCount = $removedDiscriminatorCount
    }
}

function Get-DefaultNormalizedOpenApiOneOfPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullInputPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullInputPath)
    $fileName = [System.IO.Path]::GetFileName($fullInputPath)
    if ($fileName.EndsWith(".oneof-normalized.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".oneof-normalized.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullInputPath
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$baseName.oneof-normalized.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$baseName.oneof-normalized.json"
    }
    return Join-Path $directory "$fileName.oneof-normalized.json"
}

function Invoke-OpenApiOneOfNormalizer {
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
        $OutputPath = Get-DefaultNormalizedOpenApiOneOfPath -InputPath $resolvedInputPath
    }
    else {
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-NormalizedOpenApiOneOfDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Orphaned discriminators removed: $($result.RemovedDiscriminatorCount)"

    return $result
}

# Dot-sourcing exposes conversion functions to tests without running the CLI.
if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        throw "InputPath is required."
    }
    $null = Invoke-OpenApiOneOfNormalizer `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}