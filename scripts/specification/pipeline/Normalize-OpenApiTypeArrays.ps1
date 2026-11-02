<#
.SYNOPSIS
    Normalizes OpenAPI Schema Object type arrays into scalar types and anyOf.

.DESCRIPTION
    Runs after reference inlining and before allOf flattening. Partitions finite
    enum and const constraints by JSON type without introducing excluded null
    values. Non-null object branches can then become ordinary model components.
    Schema-like literal data is preserved, and the input is never mutated.

.PARAMETER InputPath
    The inlined OpenAPI JSON document to normalize.

.PARAMETER OutputPath
    Optional destination. The default inserts '.type-arrays-normalized' before
    the JSON suffix and reuses an already-normalized filename.

.EXAMPLE
    ./scripts/specification/pipeline/Normalize-OpenApiTypeArrays.ps1 `
        ./artifacts/custom.inlined.openapi.json
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

. (Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1")

function Test-OpenApiTypeArrayValueMatches {
    param(
        [AllowNull()][object]$Value,
        [string]$TypeName
    )

    switch -CaseSensitive ($TypeName) {
        "null" { return $null -eq $Value }
        "object" { return Test-JsonObjectValue -Value $Value }
        "array" { return Test-JsonArrayValue -Value $Value }
        "string" { return $Value -is [string] }
        "boolean" { return $Value -is [bool] }
    }
    if ($null -eq $Value) { return $false }
    $isNumber = $Value -is [System.Numerics.BigInteger] -or
        [System.Type]::GetTypeCode($Value.GetType()) -in @(
            [System.TypeCode]::Byte, [System.TypeCode]::SByte,
            [System.TypeCode]::Int16, [System.TypeCode]::UInt16,
            [System.TypeCode]::Int32, [System.TypeCode]::UInt32,
            [System.TypeCode]::Int64, [System.TypeCode]::UInt64,
            [System.TypeCode]::Single, [System.TypeCode]::Double,
            [System.TypeCode]::Decimal
        )
    if (-not $isNumber) { return $false }
    if ($Value -is [double] -or $Value -is [single]) {
        if ([double]::IsNaN([double]$Value) -or [double]::IsInfinity([double]$Value)) {
            return $false
        }
    }
    if ($TypeName -ceq "number") { return $true }
    if ($TypeName -ceq "integer") {
        if ($Value -is [double] -or $Value -is [single]) {
            return [Math]::Truncate([double]$Value) -eq [double]$Value
        }
        if ($Value -is [decimal]) {
            return [decimal]::Truncate($Value) -eq $Value
        }
        return $true
    }
    return $false
}

function Test-OpenApiTypeArrayValuesEqual {
    param([AllowNull()][object]$Left, [AllowNull()][object]$Right)

    if ((Test-OpenApiTypeArrayValueMatches -Value $Left -TypeName "number") -and
        (Test-OpenApiTypeArrayValueMatches -Value $Right -TypeName "number")) {
        $leftInteger = Test-OpenApiTypeArrayValueMatches -Value $Left -TypeName "integer"
        $rightInteger = Test-OpenApiTypeArrayValueMatches -Value $Right -TypeName "integer"
        if ($leftInteger -ne $rightInteger) { return $false }
        if ($leftInteger) {
            return [System.Numerics.BigInteger]$Left -eq [System.Numerics.BigInteger]$Right
        }
        if ($Left.GetType() -eq $Right.GetType()) { return $Left -eq $Right }
        return [double]$Left -eq [double]$Right
    }
    if ((Test-JsonObjectValue -Value $Left) -and (Test-JsonObjectValue -Value $Right)) {
        $properties = @(Get-JsonProperties -Value $Left)
        if ($properties.Count -ne @(Get-JsonProperties -Value $Right).Count) { return $false }
        foreach ($property in $properties) {
            if (-not (Test-JsonProperty -Object $Right -Name $property.Name) -or
                -not (Test-OpenApiTypeArrayValuesEqual -Left $property.Value -Right (Get-JsonPropertyValue -Object $Right -Name $property.Name))) {
                return $false
            }
        }
        return $true
    }
    if ((Test-JsonArrayValue -Value $Left) -and (Test-JsonArrayValue -Value $Right)) {
        if ($Left.Count -ne $Right.Count) { return $false }
        for ($index = 0; $index -lt $Left.Count; $index++) {
            if (-not (Test-OpenApiTypeArrayValuesEqual -Left $Left[$index] -Right $Right[$index])) { return $false }
        }
        return $true
    }
    return Test-JsonValuesEqual -Left $Left -Right $Right
}

function Test-OpenApiTypeArrayKeywordApplies {
    param([string]$Name, [string]$TypeName)

    if (@("properties", "patternProperties", "additionalProperties", "unevaluatedProperties",
            "required", "dependentRequired", "dependentSchemas", "dependencies",
            "propertyNames", "minProperties", "maxProperties") -ccontains $Name) {
        return $TypeName -ceq "object"
    }
    if (@("items", "prefixItems", "additionalItems", "contains", "minContains", "maxContains",
            "minItems", "maxItems", "uniqueItems", "unevaluatedItems") -ccontains $Name) {
        return $TypeName -ceq "array"
    }
    if (@("minLength", "maxLength", "pattern", "contentEncoding", "contentMediaType", "contentSchema") -ccontains $Name) {
        return $TypeName -ceq "string"
    }
    if (@("minimum", "maximum", "exclusiveMinimum", "exclusiveMaximum", "multipleOf") -ccontains $Name) {
        return @("number", "integer") -ccontains $TypeName
    }
    return $true
}

function Test-OpenApiTypeArrayAnnotation {
    param([string]$Name)

    return @("title", "description", "default", "example", "examples", "deprecated",
        "readOnly", "writeOnly", '$comment', "externalDocs", "xml") -ccontains $Name -or
        $Name.StartsWith("x-", [System.StringComparison]::Ordinal)
}

function Get-OpenApiTypeArrayPlan {
    param([object]$Schema, [string]$Breadcrumb)

    $types = Get-JsonPropertyValue -Object $Schema -Name "type"
    if (-not (Test-JsonArrayValue -Value $types)) { return $null }
    if ($types.Count -eq 0) { throw "The type array at '$Breadcrumb/type' must be nonempty." }
    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($typeName in $types) {
        if ($typeName -isnot [string] -or
            @("object", "array", "string", "number", "integer", "boolean", "null") -cnotcontains $typeName) {
            throw "The type array at '$Breadcrumb/type' contains an unsupported JSON Schema type."
        }
        if (-not $seen.Add($typeName)) { throw "The type array at '$Breadcrumb/type' contains duplicate type '$typeName'." }
    }
    $hasEnum = Test-JsonProperty -Object $Schema -Name "enum"
    $enum = Get-JsonPropertyValue -Object $Schema -Name "enum"
    if ($hasEnum) {
        if (-not (Test-JsonArrayValue -Value $enum) -or $enum.Count -eq 0) {
            throw "The enum at '$Breadcrumb/enum' must be a nonempty array."
        }
        for ($index = 0; $index -lt $enum.Count; $index++) {
            for ($previous = 0; $previous -lt $index; $previous++) {
                if (Test-OpenApiTypeArrayValuesEqual -Left $enum[$index] -Right $enum[$previous]) {
                    throw "The enum at '$Breadcrumb/enum' contains duplicate value at index $index."
                }
            }
        }
    }
    $hasConst = Test-JsonProperty -Object $Schema -Name "const"
    $const = Get-JsonPropertyValue -Object $Schema -Name "const"
    $branches = [System.Collections.Generic.List[object]]::new()
    foreach ($typeName in @($types | Where-Object { $_ -cne "null" }) + @($types | Where-Object { $_ -ceq "null" })) {
        if ($hasConst -and -not (Test-OpenApiTypeArrayValueMatches -Value $const -TypeName $typeName)) { continue }
        [object[]]$enumIndexes = @(
            if ($hasEnum) {
                for ($index = 0; $index -lt $enum.Count; $index++) {
                    if ((Test-OpenApiTypeArrayValueMatches -Value $enum[$index] -TypeName $typeName) -and
                        (-not $hasConst -or (Test-OpenApiTypeArrayValuesEqual -Left $enum[$index] -Right $const))) { $index }
                }
            }
        )
        if ($hasEnum -and $enumIndexes.Count -eq 0) { continue }
        $branches.Add([PSCustomObject]@{ TypeName = $typeName; EnumIndexes = $enumIndexes })
    }
    return [PSCustomObject]@{ Branches = [object[]]$branches.ToArray(); Enum = $enum; HasEnum = $hasEnum }
}

function Add-OpenApiTypeArrayLocation {
    param([object]$State, [string]$Source, [string]$Destination)

    if ($State.Locations.ContainsKey($Source)) { return }
    $State.Locations[$Source] = $Destination
    if (-not [string]::Equals($Source, $Destination, [System.StringComparison]::Ordinal)) {
        Add-OpenApiPointerRelocation -Relocations $State.Relocations -SourcePrefix $Source -DestinationPrefix $Destination
    }
}

function Assert-OpenApiTypeArrayScopeSafe {
    param(
        [AllowNull()][object]$Value,
        [string]$Context = "Schema",
        [string]$Breadcrumb,
        [int]$Depth = 0,
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) { throw "Type-array scope inspection exceeds depth $MaximumDepth at '$Breadcrumb'." }
    if ($null -eq $Value -or $Context -ceq "Literal") { return }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = if ($Context -ceq "SchemaArray") { "Schema" } else { $Context }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-OpenApiTypeArrayScopeSafe -Value $Value[$index] -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return }
    if ($Context -ceq "SchemaArray") { $Context = "Schema" }
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if ($Context -ceq "Schema" -and @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor') -ccontains $property.Name) {
            throw "Cannot normalize type arrays with scope-sensitive keyword '$($property.Name)' at '$childBreadcrumb'."
        }
        $childContext = Get-OpenApiSchemaChildContext -ParentContext $Context -PropertyName $property.Name -PropertyValue $property.Value
        Assert-OpenApiTypeArrayScopeSafe -Value $property.Value -Context $childContext `
            -Breadcrumb $childBreadcrumb -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
    }
}

function Get-OpenApiTypeArrayReferenceOccurrences {
    param(
        [AllowNull()][object]$Value,
        [string]$Context = "OpenApi",
        [string]$Breadcrumb = "#",
        [int]$Depth = 0,
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) { throw "Type-array reference inspection exceeds depth $MaximumDepth at '$Breadcrumb'." }
    if ($null -eq $Value -or $Context -ceq "Literal") { return }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = if ($Context -ceq "SchemaArray") { "Schema" } else { $Context }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Get-OpenApiTypeArrayReferenceOccurrences -Value $Value[$index] -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return }
    if ($Context -ceq "SchemaArray") { $Context = "Schema" }
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if ($Context -cne "SchemaMap" -and @('$ref', '$dynamicRef', '$recursiveRef') -ccontains $property.Name -and $property.Value -is [string]) {
            [PSCustomObject]@{ Pointer = $property.Value; Breadcrumb = $childBreadcrumb }
        }
        if ($Context -ceq "Schema" -and $property.Name -ceq "discriminator" -and (Test-JsonObjectValue -Value $property.Value)) {
            $mapping = Get-JsonPropertyValue -Object $property.Value -Name "mapping"
            foreach ($entry in (Get-JsonProperties -Value $mapping)) {
                if ($entry.Value -is [string]) {
                    [PSCustomObject]@{ Pointer = $entry.Value; Breadcrumb = "$childBreadcrumb/mapping/$(ConvertTo-JsonPointerToken -Token $entry.Name)" }
                }
            }
        }
        $childContext = Get-OpenApiSchemaChildContext -ParentContext $Context -PropertyName $property.Name -PropertyValue $property.Value
        Get-OpenApiTypeArrayReferenceOccurrences -Value $property.Value -Context $childContext `
            -Breadcrumb $childBreadcrumb -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
    }
}

function ConvertTo-NormalizedOpenApiTypeArrayValue {
    param(
        [AllowNull()][object]$Value,
        [string]$Context,
        [string]$Breadcrumb,
        [string]$DestinationBreadcrumb,
        [object]$State,
        [string]$InheritedIdBreadcrumb,
        [int]$Depth = 0,
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) { throw "Type-array normalization exceeds depth $MaximumDepth at '$Breadcrumb'." }
    Add-OpenApiTypeArrayLocation -State $State -Source $Breadcrumb -Destination $DestinationBreadcrumb
    if ($null -eq $Value) { return $null }
    if ($Context -ceq "Literal") { return Copy-JsonValue -Value $Value }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = if ($Context -ceq "SchemaArray") { "Schema" } else { $Context }
        [object[]]$items = [object[]]::new($Value.Count)
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-NormalizedOpenApiTypeArrayValue -Value $Value[$index] -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" -DestinationBreadcrumb "$DestinationBreadcrumb/$index" `
                -State $State -InheritedIdBreadcrumb $InheritedIdBreadcrumb -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return $Value }
    if ($Context -ceq "SchemaArray") { $Context = "Schema" }
    $plan = if ($Context -ceq "Schema") { Get-OpenApiTypeArrayPlan -Schema $Value -Breadcrumb $Breadcrumb } else { $null }
    if ($null -ne $plan) {
        if (-not [string]::IsNullOrEmpty($InheritedIdBreadcrumb)) {
            throw "Cannot normalize type array at '$Breadcrumb' because it inherits URI scope from '$InheritedIdBreadcrumb'."
        }
        Assert-OpenApiTypeArrayScopeSafe -Value $Value -Breadcrumb $Breadcrumb -MaximumDepth ($MaximumDepth - $Depth)
        $null = $State.NormalizedSources.Add($Breadcrumb)
        $null = $State.RemovedPrefixes.Add("$Breadcrumb/type")
        if ($plan.Branches.Count -eq 0) {
            foreach ($property in (Get-JsonProperties -Value $Value)) {
                $null = $State.RemovedPrefixes.Add("$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)")
            }
            return $false
        }
        $result = [ordered]@{}
        $isUnion = $plan.Branches.Count -gt 1
        if ($isUnion) {
            foreach ($property in (Get-JsonProperties -Value $Value)) {
                if (Test-OpenApiTypeArrayAnnotation -Name $property.Name) {
                    $token = ConvertTo-JsonPointerToken -Token $property.Name
                    Add-OpenApiTypeArrayLocation -State $State -Source "$Breadcrumb/$token" -Destination "$DestinationBreadcrumb/$token"
                    $result[$property.Name] = Copy-JsonValue -Value $property.Value
                }
            }
        }
        $branches = [System.Collections.Generic.List[object]]::new()
        if ($plan.HasEnum) {
            $preferredEnumIndex = -1
            for ($index = 0; $index -lt $plan.Branches.Count; $index++) {
                if ($plan.Branches[$index].TypeName -cne "null" -and $plan.Branches[$index].EnumIndexes.Count -eq $plan.Enum.Count) {
                    $preferredEnumIndex = $index
                    break
                }
            }
            if ($preferredEnumIndex -lt 0) {
                $null = $State.InvalidExactPointers.Add("$Breadcrumb/enum")
            }
            else {
                $enumDestination = if ($isUnion) { "$DestinationBreadcrumb/anyOf/$preferredEnumIndex/enum" } else { "$DestinationBreadcrumb/enum" }
                Add-OpenApiTypeArrayLocation -State $State -Source "$Breadcrumb/enum" -Destination $enumDestination
            }
        }
        for ($branchIndex = 0; $branchIndex -lt $plan.Branches.Count; $branchIndex++) {
            $branchPlan = $plan.Branches[$branchIndex]
            $branchDestination = if ($isUnion) { "$DestinationBreadcrumb/anyOf/$branchIndex" } else { $DestinationBreadcrumb }
            $branch = [ordered]@{ type = $branchPlan.TypeName }
            foreach ($property in (Get-JsonProperties -Value $Value)) {
                if ($property.Name -ceq "type" -or
                    ($isUnion -and (Test-OpenApiTypeArrayAnnotation -Name $property.Name)) -or
                    -not (Test-OpenApiTypeArrayKeywordApplies -Name $property.Name -TypeName $branchPlan.TypeName)) { continue }
                if ($property.Name -ceq "enum") {
                    if ($branchPlan.TypeName -cne "null") {
                        $branch["enum"] = [object[]]@($branchPlan.EnumIndexes | ForEach-Object { Copy-JsonValue -Value $plan.Enum[$_] })
                        for ($enumIndex = 0; $enumIndex -lt $branchPlan.EnumIndexes.Count; $enumIndex++) {
                            Add-OpenApiTypeArrayLocation -State $State -Source "$Breadcrumb/enum/$($branchPlan.EnumIndexes[$enumIndex])" `
                                -Destination "$branchDestination/enum/$enumIndex"
                        }
                    }
                    continue
                }
                if ($property.Name -ceq "const" -and $branchPlan.TypeName -ceq "null") { continue }
                $branch[$property.Name] = Copy-JsonValue -Value $property.Value
            }
            $branches.Add((ConvertTo-NormalizedOpenApiTypeArrayValue -Value $branch -Context Schema `
                -Breadcrumb $Breadcrumb -DestinationBreadcrumb $branchDestination -State $State `
                -Depth ($Depth + 1) -MaximumDepth $MaximumDepth))
        }
        if ($plan.HasEnum) {
            for ($index = 0; $index -lt $plan.Enum.Count; $index++) {
                if (-not $State.Locations.ContainsKey("$Breadcrumb/enum/$index")) {
                    $null = $State.RemovedPrefixes.Add("$Breadcrumb/enum/$index")
                }
            }
        }
        foreach ($property in (Get-JsonProperties -Value $Value)) {
            $source = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
            if (-not $State.Locations.ContainsKey($source)) { $null = $State.RemovedPrefixes.Add($source) }
        }
        if (-not $isUnion) { return $branches[0] }
        $result["anyOf"] = [object[]]$branches.ToArray()
        return $result
    }
    if ($Context -ceq "Schema" -and (Test-JsonProperty -Object $Value -Name '$id')) {
        $InheritedIdBreadcrumb = "$Breadcrumb/`$id"
    }
    $copy = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext -ParentContext $Context -PropertyName $property.Name -PropertyValue $property.Value
        $copy[$property.Name] = ConvertTo-NormalizedOpenApiTypeArrayValue -Value $property.Value -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -DestinationBreadcrumb "$DestinationBreadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -State $State -InheritedIdBreadcrumb $InheritedIdBreadcrumb `
            -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
    }
    return $copy
}

function ConvertTo-NormalizedOpenApiTypeArrayDocument {
    param([Parameter(Mandatory = $true)][object]$Document)

    $state = [PSCustomObject]@{
        NormalizedSources = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        Locations = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
        Relocations = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
        RemovedPrefixes = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        InvalidExactPointers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    }
    $sourceReferences = @(Get-OpenApiTypeArrayReferenceOccurrences -Value $Document)
    $converted = ConvertTo-NormalizedOpenApiTypeArrayValue -Value $Document -Context OpenApi `
        -Breadcrumb "#" -DestinationBreadcrumb "#" -State $state
    foreach ($reference in $sourceReferences) {
        if (-not $reference.Pointer.StartsWith("#/", [System.StringComparison]::Ordinal)) { continue }
        $pointer = Get-CanonicalJsonPointer -Pointer $reference.Pointer
        if ($state.InvalidExactPointers.Contains($pointer)) {
            throw "Cannot preserve reference '$pointer' at '$($reference.Breadcrumb)' because normalization partitions that constraint."
        }
        foreach ($prefix in $state.RemovedPrefixes) {
            if ([string]::Equals($pointer, $prefix, [System.StringComparison]::Ordinal) -or
                $pointer.StartsWith("$prefix/", [System.StringComparison]::Ordinal)) {
                throw "Cannot preserve reference '$pointer' at '$($reference.Breadcrumb)' because normalization removes that target."
            }
        }
    }
    $relocated = ConvertTo-RelocatedOpenApiPointers -Value $converted -Relocations $state.Relocations -SinglePass -SchemaAware
    foreach ($reference in @(Get-OpenApiTypeArrayReferenceOccurrences -Value $relocated)) {
        if ($reference.Pointer.StartsWith("#/", [System.StringComparison]::Ordinal)) {
            $null = Resolve-LocalJsonPointer -Document $relocated -Pointer $reference.Pointer -Context "reference at $($reference.Breadcrumb)"
        }
    }
    Assert-NoOpenApiSchemaTypeArrays -Value $relocated
    return [PSCustomObject]@{
        Document = $relocated
        NormalizedTypeArrayCount = $state.NormalizedSources.Count
        RelocationCount = $state.Relocations.Count
    }
}

function Get-DefaultNormalizedOpenApiTypeArrayPath {
    param([Parameter(Mandatory = $true)][string]$InputPath)

    $fullPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullPath)
    $fileName = [System.IO.Path]::GetFileName($fullPath)
    foreach ($suffix in @(".openapi.json", ".json")) {
        if ($fileName.EndsWith(".type-arrays-normalized$suffix", [System.StringComparison]::OrdinalIgnoreCase)) { return $fullPath }
        if ($fileName.EndsWith($suffix, [System.StringComparison]::OrdinalIgnoreCase)) {
            return Join-Path $directory "$($fileName.Substring(0, $fileName.Length - $suffix.Length)).type-arrays-normalized$suffix"
        }
    }
    return Join-Path $directory "$fileName.type-arrays-normalized.json"
}

function Invoke-OpenApiTypeArrayNormalizer {
    param([Parameter(Mandatory = $true)][string]$InputPath, [string]$OutputPath)

    $resolvedInput = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInput)) { throw "The OpenAPI input path is not a file: $resolvedInput" }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) { $OutputPath = Get-DefaultNormalizedOpenApiTypeArrayPath -InputPath $resolvedInput }
    $result = ConvertTo-NormalizedOpenApiTypeArrayDocument -Document (Read-OpenApiDocument -Path $resolvedInput)
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath
    Write-Host ""
    Write-Host "Input: $resolvedInput"
    Write-Host "Output: $writtenPath"
    Write-Host "Type arrays normalized: $($result.NormalizedTypeArrayCount)"
    return $result
}

if ($MyInvocation.InvocationName -ne ".") {
    if ([string]::IsNullOrWhiteSpace($InputPath)) { throw "InputPath is required." }
    $null = Invoke-OpenApiTypeArrayNormalizer -InputPath $InputPath -OutputPath $OutputPath
}