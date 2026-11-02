<#
.SYNOPSIS
    Normalizes oneOf-related schema metadata in an OpenAPI document.

.DESCRIPTION
    Promotes an anyOf schema composition to oneOf when it has a sibling
    discriminator and recursively resolves to compatible object leaves. Every
    leaf must require the discriminator property with finite, mutually exclusive
    string const/enum values. Compatible nested oneOf wrappers may contribute
    leaves and may omit type when their leaves are explicit objects. The stage
    then removes an exact discriminator schema keyword when its containing
    schema has no exact direct oneOf sibling.

    A discriminator paired with any direct oneOf property is preserved; the
    preceding discriminator-reconciliation stage validates that composition.

    Property-map entries named discriminator, string literals, examples, and
    ordinary OpenAPI objects are preserved. Encounter order is retained. Run
    Flatten-OpenApiOneOf.ps1, Wrap-OpenApiSingletonTypeEnums.ps1, and then
    Sort-OpenApiDocument.ps1 afterward.

.PARAMETER InputPath
    The OpenAPI JSON document to normalize.

.PARAMETER OutputPath
    The destination JSON file. When omitted, a sibling filename is generated
    by inserting '.oneof-normalized' before '.openapi.json' or the final '.json'
    suffix. If the input already has that suffix, it is atomically rewritten
    in place.

.EXAMPLE
    ./scripts/specification/pipeline/Normalize-OpenApiOneOf.ps1 `
        ./artifacts/custom.oneof-discriminated.openapi.json `
        -OutputPath ./artifacts/custom.oneof-normalized.openapi.json
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

function Get-DiscriminatedOpenApiAnyOfCandidate {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter()]
        [string]$Breadcrumb = "#"
    )

    if (-not (Test-JsonProperty -Object $Schema -Name "anyOf") -or
        -not (Test-JsonProperty -Object $Schema -Name "discriminator")) {
        return $null
    }

    $discriminator = Get-JsonPropertyValue -Object $Schema -Name "discriminator"
    if (-not (Test-JsonObjectValue -Value $discriminator)) {
        return $null
    }
    $propertyName = Get-JsonPropertyValue -Object $discriminator -Name "propertyName"
    if ($propertyName -isnot [string] -or [string]::IsNullOrWhiteSpace($propertyName)) {
        return $null
    }

    $plan = Get-OpenApiInlineDiscriminatorUnionPlan `
        -Schema $Schema `
        -PropertyName $propertyName `
        -Breadcrumb $Breadcrumb `
        -CompositionName "anyOf" `
        -RequireDiscriminator
    if ($null -eq $plan) {
        return $null
    }

    if (Test-JsonProperty -Object $Schema -Name "oneOf") {
        throw "Cannot promote discriminated anyOf at '$Breadcrumb' because the schema already contains oneOf."
    }

    return [PSCustomObject]@{
        PropertyName = $propertyName
        Branches = $plan.Branches
        Plan = $plan
    }
}

# Recursively copies an OpenAPI document while promoting eligible unions and
# removing orphaned discriminator keywords. Surviving values retain source order.
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

        [Parameter(Mandatory = $true)]
        [ref]$PromotedAnyOfCount,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, string]]$Relocations,

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
                -PromotedAnyOfCount $PromotedAnyOfCount `
                -Relocations $Relocations `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $isSchema = [string]::Equals(
        $Context,
        "Schema",
        [System.StringComparison]::Ordinal
    )
    $candidate = if ($isSchema) {
        Get-DiscriminatedOpenApiAnyOfCandidate -Schema $Value -Breadcrumb $Breadcrumb
    }
    else {
        $null
    }
    $removeDiscriminator = $isSchema -and
        $null -eq $candidate -and
        (Test-IsOrphanedOpenApiDiscriminator -Schema $Value)

    if ($null -ne $candidate) {
        $Value = ConvertTo-NormalizedOpenApiDiscriminatorUnionSchema `
            -Plan $candidate.Plan `
            -MaximumDepth ($MaximumDepth - $Depth)
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ($null -ne $candidate -and
            [string]::Equals($property.Name, "anyOf", [System.StringComparison]::Ordinal)) {
            Add-OpenApiPointerRelocation `
                -Relocations $Relocations `
                -SourcePrefix "$Breadcrumb/anyOf" `
                -DestinationPrefix "$Breadcrumb/oneOf"
            $childContext = Get-OpenApiSchemaChildContext `
                -ParentContext $Context `
                -PropertyName "oneOf" `
                -PropertyValue $property.Value
            $result["oneOf"] = ConvertTo-NormalizedOpenApiOneOfValue `
                -Value $property.Value `
                -Context $childContext `
                -Breadcrumb "$Breadcrumb/oneOf" `
                -RemovedDiscriminatorCount $RemovedDiscriminatorCount `
                -PromotedAnyOfCount $PromotedAnyOfCount `
                -Relocations $Relocations `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            $PromotedAnyOfCount.Value++
            continue
        }

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
            -PromotedAnyOfCount $PromotedAnyOfCount `
            -Relocations $Relocations `
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

function Assert-NoPromotableOpenApiAnyOf {
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
        throw "oneOf promotion postcondition exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
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
            Assert-NoPromotableOpenApiAnyOf `
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
        $null -ne (Get-DiscriminatedOpenApiAnyOfCandidate -Schema $Value -Breadcrumb $Breadcrumb)) {
        throw "Normalized output retained a promotable discriminated anyOf at '$Breadcrumb/anyOf'."
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        Assert-NoPromotableOpenApiAnyOf `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

function ConvertTo-NormalizedOpenApiOneOfDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    $removedDiscriminatorCount = 0
    $promotedAnyOfCount = 0
    $normalized = ConvertTo-NormalizedOpenApiOneOfValue `
        -Value $Document `
        -Context OpenApi `
        -Breadcrumb "#" `
        -RemovedDiscriminatorCount ([ref]$removedDiscriminatorCount) `
        -PromotedAnyOfCount ([ref]$promotedAnyOfCount) `
        -Relocations $relocations
    $relocated = ConvertTo-RelocatedOpenApiPointers `
        -Value $normalized `
        -Relocations $relocations
    Assert-OpenApiReferencesResolve -Document $relocated
    Assert-AdditionalOpenApiReferencesResolve -Document $relocated
    Assert-NoPromotableOpenApiAnyOf -Value $relocated
    Assert-NoOrphanedOpenApiDiscriminators -Value $relocated

    return [PSCustomObject]@{
        Document = $relocated
        PromotedAnyOfCount = $promotedAnyOfCount
        RemovedDiscriminatorCount = $removedDiscriminatorCount
        RelocationCount = $relocations.Count
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
    Write-Host "Discriminated anyOf promoted: $($result.PromotedAnyOfCount)"
    Write-Host "Orphaned discriminators removed: $($result.RemovedDiscriminatorCount)"
    Write-Host "JSON Pointer prefixes relocated: $($result.RelocationCount)"

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