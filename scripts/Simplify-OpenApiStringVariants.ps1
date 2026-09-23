<#
.SYNOPSIS
    Simplifies nested string-only anyOf schema compositions.

.DESCRIPTION
    Reads allOf-flattened JSON produced by Flatten-OpenApiAllOf.ps1, replaces
    eligible nested string-only anyOf trees with at most one plain-string
    branch and one merged string-enum branch, and preserves encounter order.
    Run Simplify-OpenApiRecursiveSchemas.ps1,
    Add-OpenApiOneOfDiscriminators.ps1, Normalize-OpenApiOneOf.ps1, and then
    Sort-OpenApiDocument.ps1 afterward to replace recursive objects, infer
    missing type discriminators, remove orphaned discriminator metadata, and
    produce canonical ordering.

    A recursive anyOf tree is eligible only when every leaf is an
    unconstrained string or an enum containing only strings. Mixed or
    constrained unions, including their directly connected nested anyOf
    branches, are left unchanged. An eligible tree is rejected when a local
    reference targets a branch that would disappear during simplification.

.PARAMETER InputPath
    The allOf-flattened Inlined OpenAPI JSON document to simplify.

.PARAMETER OutputPath
    The destination JSON file. When omitted, a sibling filename is generated
    by inserting '.simplified' before '.openapi.json' or the final '.json'
    suffix. If the input already has that suffix, it is atomically rewritten
    in place.

.EXAMPLE
    ./scripts/Simplify-OpenApiStringVariants.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.openapi.json

.EXAMPLE
    ./scripts/Simplify-OpenApiStringVariants.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.openapi.json
    ./scripts/Simplify-OpenApiRecursiveSchemas.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.openapi.json
    ./scripts/Add-OpenApiOneOfDiscriminators.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.openapi.json
    ./scripts/Normalize-OpenApiOneOf.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.openapi.json
    ./scripts/Sort-OpenApiDocument.ps1 `
        ./artifacts/openapi-extracts/responses.inlined.flattened.simplified.recursive-simplified.oneof-discriminated.oneof-normalized.openapi.json
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

$script:DiscardableStringVariantAnnotationNames = @(
    'title'
    'description'
    'default'
    'deprecated'
    'example'
    'examples'
    '$comment'
    'readOnly'
    'writeOnly'
)

function New-IneligibleStringVariantAnalysis {
    return [PSCustomObject]@{
        IsEligible = $false
        ContainsNestedAnyOf = $false
        Leaves = [object[]]@()
    }
}

# Classifies one branch of a prospective recursive string-only anyOf tree.
function Get-OpenApiStringVariantBranchAnalysis {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "String variant analysis exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if (-not (Test-JsonObjectValue -Value $Schema)) {
        return New-IneligibleStringVariantAnalysis
    }

    $anyOf = Get-JsonPropertyValue -Object $Schema -Name "anyOf"
    if ($null -ne $anyOf) {
        foreach ($property in (Get-JsonProperties -Value $Schema)) {
            if (-not ([string]::Equals($property.Name, "anyOf", [System.StringComparison]::Ordinal) -or
                $script:DiscardableStringVariantAnnotationNames -ccontains $property.Name)) {
                return New-IneligibleStringVariantAnalysis
            }
        }
        if (-not (Test-JsonArrayValue -Value $anyOf) -or $anyOf.Count -eq 0) {
            return New-IneligibleStringVariantAnalysis
        }

        $leaves = [System.Collections.Generic.List[object]]::new()
        for ($index = 0; $index -lt $anyOf.Count; $index++) {
            $analysis = Get-OpenApiStringVariantBranchAnalysis `
                -Schema $anyOf[$index] `
                -Breadcrumb "$Breadcrumb/anyOf/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            if (-not $analysis.IsEligible) {
                return New-IneligibleStringVariantAnalysis
            }
            foreach ($leaf in $analysis.Leaves) {
                $leaves.Add($leaf)
            }
        }

        return [PSCustomObject]@{
            IsEligible = $true
            ContainsNestedAnyOf = $true
            Leaves = [object[]]$leaves.ToArray()
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Schema)) {
        if (-not (@('type', 'enum') -ccontains $property.Name -or
            $script:DiscardableStringVariantAnnotationNames -ccontains $property.Name)) {
            return New-IneligibleStringVariantAnalysis
        }
    }

    $type = Get-JsonPropertyValue -Object $Schema -Name "type"
    if ($type -isnot [string] -or
        -not [string]::Equals($type, "string", [System.StringComparison]::Ordinal)) {
        return New-IneligibleStringVariantAnalysis
    }

    $hasEnum = Test-JsonProperty -Object $Schema -Name "enum"
    [object[]]$enumValues = @()
    if ($hasEnum) {
        $enum = Get-JsonPropertyValue -Object $Schema -Name "enum"
        if (-not (Test-JsonArrayValue -Value $enum) -or $enum.Count -eq 0) {
            return New-IneligibleStringVariantAnalysis
        }
        foreach ($enumValue in $enum) {
            if ($enumValue -isnot [string]) {
                return New-IneligibleStringVariantAnalysis
            }
        }
        [object[]]$enumValues = @($enum)
    }

    return [PSCustomObject]@{
        IsEligible = $true
        ContainsNestedAnyOf = $false
        Leaves = [object[]]@(
            [PSCustomObject]@{
                Kind = if ($hasEnum) { "Enum" } else { "Plain" }
                EnumValues = $enumValues
                Breadcrumb = $Breadcrumb
            }
        )
    }
}

# Returns a candidate only when the root anyOf contains a nested anyOf and its
# complete recursive leaf set consists of plain strings or string enums.
function Get-OpenApiStringVariantCandidate {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter()]
        [string]$Breadcrumb = "#"
    )

    if (-not (Test-JsonObjectValue -Value $Schema)) {
        return $null
    }
    $anyOf = Get-JsonPropertyValue -Object $Schema -Name "anyOf"
    if (-not (Test-JsonArrayValue -Value $anyOf) -or $anyOf.Count -eq 0) {
        return $null
    }

    $containsNestedAnyOf = $false
    $leaves = [System.Collections.Generic.List[object]]::new()
    for ($index = 0; $index -lt $anyOf.Count; $index++) {
        $analysis = Get-OpenApiStringVariantBranchAnalysis `
            -Schema $anyOf[$index] `
            -Breadcrumb "$Breadcrumb/anyOf/$index"
        if (-not $analysis.IsEligible) {
            return $null
        }
        if ($analysis.ContainsNestedAnyOf) {
            $containsNestedAnyOf = $true
        }
        foreach ($leaf in $analysis.Leaves) {
            $leaves.Add($leaf)
        }
    }
    if (-not $containsNestedAnyOf) {
        return $null
    }

    return [PSCustomObject]@{
        Breadcrumb = $Breadcrumb
        Leaves = [object[]]$leaves.ToArray()
    }
}

# Finds dynamic and recursive references not returned by the shared reference
# discovery helper.
function Get-AdditionalOpenApiReferenceOccurrences {
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
            Get-AdditionalOpenApiReferenceOccurrences `
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
        Get-AdditionalOpenApiReferenceOccurrences `
            -Value $property.Value `
            -Breadcrumb $propertyBreadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

function Get-AllOpenApiReferenceOccurrences {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    return @(
        Get-OpenApiReferenceOccurrences -Value $Document
        Get-AdditionalOpenApiReferenceOccurrences -Value $Document
    )
}

function Test-JsonPointerIsAtOrBelow {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [string]$Prefix
    )

    return [string]::Equals($Pointer, $Prefix, [System.StringComparison]::Ordinal) -or
        ($Pointer.StartsWith($Prefix, [System.StringComparison]::Ordinal) -and
            $Pointer.Length -gt $Prefix.Length -and
            $Pointer[$Prefix.Length] -eq '/')
}

function Assert-OpenApiStringVariantCandidateIsUntargeted {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Candidate,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$ReferenceOccurrences
    )

    $anyOfPointer = "$($Candidate.Breadcrumb)/anyOf"
    foreach ($occurrence in $ReferenceOccurrences) {
        if ($occurrence.Pointer -isnot [string] -or
            -not $occurrence.Pointer.StartsWith("#/", [System.StringComparison]::Ordinal)) {
            continue
        }
        $canonicalPointer = Get-CanonicalJsonPointer -Pointer $occurrence.Pointer
        if (Test-JsonPointerIsAtOrBelow -Pointer $canonicalPointer -Prefix $anyOfPointer) {
            throw "Cannot simplify string variants at '$($Candidate.Breadcrumb)' because $($occurrence.Kind) at '$($occurrence.Breadcrumb)' targets '$canonicalPointer' inside the anyOf branches."
        }
    }
}

function New-SimplifiedOpenApiStringVariantAnyOf {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Candidate,

        [Parameter(Mandatory = $true)]
        [ref]$DeduplicatedEnumValueCount
    )

    $categoryOrder = [System.Collections.Generic.List[string]]::new()
    $seenCategories = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $enumValues = [System.Collections.Generic.List[object]]::new()
    $seenEnumValues = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )

    foreach ($leaf in $Candidate.Leaves) {
        if ($seenCategories.Add($leaf.Kind)) {
            $categoryOrder.Add($leaf.Kind)
        }
        if ([string]::Equals($leaf.Kind, "Enum", [System.StringComparison]::Ordinal)) {
            foreach ($enumValue in $leaf.EnumValues) {
                if ($seenEnumValues.Add($enumValue)) {
                    $enumValues.Add($enumValue)
                }
                else {
                    $DeduplicatedEnumValueCount.Value++
                }
            }
        }
    }

    [object[]]$branches = @(
        foreach ($category in $categoryOrder) {
            if ([string]::Equals($category, "Plain", [System.StringComparison]::Ordinal)) {
                [ordered]@{ type = "string" }
            }
            else {
                [ordered]@{
                    type = "string"
                    enum = [object[]]$enumValues.ToArray()
                }
            }
        }
    )
    return ,$branches
}

# Recursively copies the document and simplifies eligible schema values. When
# an outer anyOf is ineligible, directly connected nested anyOf wrappers are
# suppressed so a string-only subtree is not promoted out of a mixed union.
function ConvertTo-SimplifiedOpenApiStringVariantValue {
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
        [AllowEmptyCollection()]
        [object[]]$ReferenceOccurrences,

        [Parameter(Mandatory = $true)]
        [ref]$SimplifiedCount,

        [Parameter(Mandatory = $true)]
        [ref]$DeduplicatedEnumValueCount,

        [Parameter()]
        [switch]$SuppressCurrentAnyOf,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "String variant simplification exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
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
            $items[$index] = ConvertTo-SimplifiedOpenApiStringVariantValue `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" `
                -ReferenceOccurrences $ReferenceOccurrences `
                -SimplifiedCount $SimplifiedCount `
                -DeduplicatedEnumValueCount $DeduplicatedEnumValueCount `
                -SuppressCurrentAnyOf:$SuppressCurrentAnyOf `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $candidate = $null
    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        -not $SuppressCurrentAnyOf) {
        $candidate = Get-OpenApiStringVariantCandidate -Schema $Value -Breadcrumb $Breadcrumb
    }

    $replacementAnyOf = $null
    if ($null -ne $candidate) {
        Assert-OpenApiStringVariantCandidateIsUntargeted `
            -Candidate $candidate `
            -ReferenceOccurrences $ReferenceOccurrences
        $replacementAnyOf = New-SimplifiedOpenApiStringVariantAnyOf `
            -Candidate $candidate `
            -DeduplicatedEnumValueCount $DeduplicatedEnumValueCount
        $SimplifiedCount.Value++
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ($null -ne $candidate -and
            [string]::Equals($property.Name, "anyOf", [System.StringComparison]::Ordinal)) {
            $result[$property.Name] = $replacementAnyOf
            continue
        }

        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $suppressChildAnyOf = $false
        if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "anyOf", [System.StringComparison]::Ordinal)) {
            $suppressChildAnyOf = $true
        }
        $result[$property.Name] = ConvertTo-SimplifiedOpenApiStringVariantValue `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -ReferenceOccurrences $ReferenceOccurrences `
            -SimplifiedCount $SimplifiedCount `
            -DeduplicatedEnumValueCount $DeduplicatedEnumValueCount `
            -SuppressCurrentAnyOf:$suppressChildAnyOf `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

function Assert-NoOpenApiAllOfForStringVariantSimplification {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [string]$Breadcrumb = "#"
    )

    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-NoOpenApiAllOfForStringVariantSimplification `
                -Value $Value[$index] `
                -Breadcrumb "$Breadcrumb/$index"
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $propertyBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if ([string]::Equals($property.Name, "allOf", [System.StringComparison]::Ordinal)) {
            throw "String variant simplification requires allOf-flattened input; found allOf at '$propertyBreadcrumb'. Run Flatten-OpenApiAllOf.ps1 first."
        }
        Assert-NoOpenApiAllOfForStringVariantSimplification `
            -Value $property.Value `
            -Breadcrumb $propertyBreadcrumb
    }
}

function Assert-AdditionalOpenApiReferencesResolveAfterStringVariantSimplification {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    foreach ($occurrence in @(Get-AdditionalOpenApiReferenceOccurrences -Value $Document)) {
        if ([string]::Equals($occurrence.Pointer, "#", [System.StringComparison]::Ordinal)) {
            continue
        }
        $null = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $occurrence.Pointer `
            -Context "$($occurrence.Kind) at $($occurrence.Breadcrumb)"
    }
}

function Assert-NoSimplifiableOpenApiStringVariants {
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
        [switch]$SuppressCurrentAnyOf,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "String variant postcondition exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
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
            Assert-NoSimplifiableOpenApiStringVariants `
                -Value $Value[$index] `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" `
                -SuppressCurrentAnyOf:$SuppressCurrentAnyOf `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        -not $SuppressCurrentAnyOf) {
        $candidate = Get-OpenApiStringVariantCandidate `
            -Schema $Value `
            -Breadcrumb $Breadcrumb
        if ($null -ne $candidate) {
            throw "Simplified output retained an eligible nested string anyOf at '$Breadcrumb'."
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $suppressChildAnyOf = $false
        if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "anyOf", [System.StringComparison]::Ordinal)) {
            $suppressChildAnyOf = $true
        }
        Assert-NoSimplifiableOpenApiStringVariants `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -SuppressCurrentAnyOf:$suppressChildAnyOf `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

function ConvertTo-SimplifiedOpenApiStringVariantDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    Assert-NoOpenApiAllOfForStringVariantSimplification -Value $Document
    [object[]]$referenceOccurrences = @(Get-AllOpenApiReferenceOccurrences -Document $Document)
    $simplifiedCount = 0
    $deduplicatedEnumValueCount = 0
    $simplified = ConvertTo-SimplifiedOpenApiStringVariantValue `
        -Value $Document `
        -Context OpenApi `
        -Breadcrumb "#" `
        -ReferenceOccurrences $referenceOccurrences `
        -SimplifiedCount ([ref]$simplifiedCount) `
        -DeduplicatedEnumValueCount ([ref]$deduplicatedEnumValueCount)
    Assert-OpenApiReferencesResolve -Document $simplified
    Assert-AdditionalOpenApiReferencesResolveAfterStringVariantSimplification `
        -Document $simplified
    Assert-NoSimplifiableOpenApiStringVariants -Value $simplified

    return [PSCustomObject]@{
        Document = $simplified
        SimplifiedCount = $simplifiedCount
        DeduplicatedEnumValueCount = $deduplicatedEnumValueCount
    }
}

function Get-DefaultSimplifiedOpenApiPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullInputPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullInputPath)
    $fileName = [System.IO.Path]::GetFileName($fullInputPath)
    if ($fileName.EndsWith(".simplified.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".simplified.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullInputPath
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$baseName.simplified.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$baseName.simplified.json"
    }
    return Join-Path $directory "$fileName.simplified.json"
}

function Invoke-OpenApiStringVariantSimplifier {
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
        $OutputPath = Get-DefaultSimplifiedOpenApiPath -InputPath $resolvedInputPath
    }
    else {
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-SimplifiedOpenApiStringVariantDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Nested string anyOf compositions simplified: $($result.SimplifiedCount)"
    Write-Host "Duplicate enum values removed: $($result.DeduplicatedEnumValueCount)"

    return $result
}

# Dot-sourcing exposes conversion functions to tests without running the CLI.
if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        throw "InputPath is required."
    }
    $null = Invoke-OpenApiStringVariantSimplifier `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}