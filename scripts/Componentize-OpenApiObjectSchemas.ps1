<#
.SYNOPSIS
    Moves inline object and direct oneOf schemas into OpenAPI components.

.DESCRIPTION
    Finds every inline Schema Object that owns a direct nonempty oneOf or has the
    exact scalar type "object", creates one components.schemas entry per occurrence,
    and replaces each inline definition with a local $ref. A direct oneOf owner
    takes precedence when a schema satisfies both predicates. Nested candidates
    become separate components referenced by their containing component.

    Component names use typed underscore-delimited segments. Operation ids and
    structural roles are lower-initial, API property/schema tokens are
    PascalCase, and discriminator leaves use their actual property and accepted
    value. Object, dictionary, union, discriminated-object, array-item,
    dictionary-value, and tuple-item roles are explicit. Repeated anonymous
    sibling roles receive one-based shape ordinals before numeric collision
    suffixes beginning with `_1`.
    The complete oneOf-containing Schema Object moves with all of its siblings.
    Schemas whose relocation could change $id or anchor scope fail explicitly.
    After component names stabilize, every discriminator receives a canonical
    exhaustive mapping from accepted const/enum payload values to its branch
    component pointers. Structurally identical occurrences intentionally remain
    separate components. Run
    Update-OpenApiComponentDeduplicationMap.ps1 afterward, review its persisted
    decisions, then run Deduplicate-OpenApiComponents.ps1.

.PARAMETER InputPath
    The OpenAPI JSON document to componentize. When omitted, reads
    the retained sorted Responses artifact beneath specification/openapi.

.PARAMETER OutputPath
    The destination JSON file. When omitted for the canonical input, writes
    specification/openapi/responses.componentized.openapi.json. Other inputs use
    a sibling filename with '.componentized' inserted before the JSON suffix.

.EXAMPLE
    ./scripts/Componentize-OpenApiObjectSchemas.ps1
    ./scripts/Update-OpenApiComponentDeduplicationMap.ps1
    ./scripts/Deduplicate-OpenApiComponents.ps1

.EXAMPLE
    ./scripts/Componentize-OpenApiObjectSchemas.ps1 `
        ./artifacts/custom.openapi.json `
        -OutputPath ./artifacts/custom.componentized.openapi.json
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

$script:ComponentizerRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultComponentizerInputPath = Join-Path $script:ComponentizerRepositoryRoot "specification" "openapi" "responses.inlined.flattened.simplified.recursive-simplified.type-enum-split.oneof-discriminated.oneof-normalized.oneof-flattened.singleton-type-wrapped.sorted.openapi.json"
$script:DefaultComponentizerOutputPath = Join-Path $script:ComponentizerRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"

# Eligibility is deliberately narrower than "any JSON object with type=object".
# The traversal context must say that the value is a Schema Object. This keeps
# examples and ordinary OpenAPI objects such as request bodies from becoming
# components merely because they contain schema-looking property names.
function Test-IsInlineOpenApiObjectSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context
    )

    if (-not [string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -or
        -not (Test-JsonObjectValue -Value $Value)) {
        return $false
    }

    $type = Get-JsonPropertyValue -Object $Value -Name "type"
    return $type -is [string] -and
        [string]::Equals($type, "object", [System.StringComparison]::Ordinal)
}

# A composition candidate is the complete Schema Object that owns oneOf, not the
# oneOf array itself. Requiring a nonempty array avoids turning malformed or
# vacuous composition keywords into generated components.
function Test-IsInlineOpenApiOneOfSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context
    )

    if (-not [string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -or
        -not (Test-JsonObjectValue -Value $Value)) {
        return $false
    }

    $oneOf = Get-JsonPropertyValue -Object $Value -Name "oneOf"
    return (Test-JsonArrayValue -Value $oneOf) -and $oneOf.Count -gt 0
}

# Marks a componentized oneOf owner whose alternatives cannot be selected by a
# direct OpenAPI discriminator. The distinction is naming metadata only; both
# forms remain OneOf candidates and follow the same relocation behavior.
function Test-IsOpenApiNonDiscriminatedOneOfSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context
    )

    return (Test-IsInlineOpenApiOneOfSchema -Value $Value -Context $Context) -and
        -not (Test-JsonProperty -Object $Value -Name "discriminator")
}

# Finds the first scope-sensitive JSON Schema keyword within a schema subtree.
# Traversal remains schema-aware so a literal example containing a key such as
# `$id` is not mistaken for a resolution scope. Relocating any matching subtree
# is rejected until URI and anchor scope preservation is implemented explicitly.
function Get-OpenApiScopeSensitiveSchemaKeywordOccurrence {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Schema scope inspection exceeds the supported depth of $MaximumDepth at '$Pointer'."
    }
    if ($null -eq $Value) {
        return $null
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $occurrence = Get-OpenApiScopeSensitiveSchemaKeywordOccurrence `
                -Value $Value[$index] `
                -Context $itemContext `
                -Pointer "$Pointer/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            if ($null -ne $occurrence) {
                return $occurrence
            }
        }
        return $null
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $null
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal)) {
        foreach ($keyword in @('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor')) {
            if (Test-JsonProperty -Object $Value -Name $keyword) {
                return [PSCustomObject]@{
                    Keyword = $keyword
                    Pointer = "$Pointer/$(ConvertTo-JsonPointerToken -Token $keyword)"
                }
            }
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $occurrence = Get-OpenApiScopeSensitiveSchemaKeywordOccurrence `
            -Value $property.Value `
            -Context $childContext `
            -Pointer "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
        if ($null -ne $occurrence) {
            return $occurrence
        }
    }
    return $null
}

# Retains direct singleton `properties.type.enum` metadata for diagnostics and
# compatibility. Naming uses the immediate validated discriminator contract so
# an ordinary singleton enum is never mislabeled as a discriminator branch.
function Get-OpenApiObjectSchemaTypeEnumValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    $properties = Get-JsonPropertyValue -Object $Schema -Name "properties"
    if (-not (Test-JsonObjectValue -Value $properties)) {
        return $null
    }
    $typeSchema = Get-JsonPropertyValue -Object $properties -Name "type"
    if (-not (Test-JsonObjectValue -Value $typeSchema)) {
        return $null
    }
    $enum = Get-JsonPropertyValue -Object $typeSchema -Name "enum"
    if (-not (Test-JsonArrayValue -Value $enum) -or
        $enum.Count -ne 1 -or
        $enum[0] -isnot [string]) {
        return $null
    }

    return $enum[0]
}

# Performs one schema-aware, parent-first discovery pass and records every direct
# oneOf owner or exact object-schema occurrence before any document is rewritten.
# Direct oneOf takes precedence when a Schema Object satisfies both predicates,
# so one source occurrence always produces exactly one component candidate.
# Discovery and rewriting are kept separate so all source pointers remain valid
# while names and relocations are being planned.
#
# CandidateDepth is diagnostic information used by tests. ParentCandidatePointer
# is behavioral: it lets nested components inherit semantic context that exists
# only on their containing schema (for example `typeOfToolSearchOutput`).
function Add-InlineOpenApiSchemaCandidates {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.List[object]]$Candidates,

        [Parameter()]
        [switch]$ComponentsSchemaMap,

        [Parameter()]
        [switch]$SuppressCurrentCandidate,

        [Parameter()]
        [int]$CandidateDepth = 0,

        [Parameter()]
        [AllowNull()]
        [string]$ParentCandidatePointer,

        [Parameter()]
        [AllowNull()]
        [string]$InheritedIdPointer,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Inline schema discovery exceeds the supported depth of $MaximumDepth at '$Pointer'."
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
            Add-InlineOpenApiSchemaCandidates `
                -Value $Value[$index] `
                -Context $itemContext `
                -Pointer "$Pointer/$index" `
                -Candidates $Candidates `
                -CandidateDepth $CandidateDepth `
                -ParentCandidatePointer $ParentCandidatePointer `
                -InheritedIdPointer $InheritedIdPointer `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    # Existing entries under components.schemas are already component roots.
    # SuppressCurrentCandidate prevents promoting those roots again, while the
    # traversal still discovers inline schemas nested inside them.
    $candidateKind = $null
    if (-not $SuppressCurrentCandidate) {
        if (Test-IsInlineOpenApiOneOfSchema -Value $Value -Context $Context) {
            $candidateKind = "OneOf"
        }
        elseif (Test-IsInlineOpenApiObjectSchema -Value $Value -Context $Context) {
            $candidateKind = "Object"
        }
    }
    $isCandidate = $null -ne $candidateKind
    $childCandidateDepth = $CandidateDepth
    $childParentCandidatePointer = $ParentCandidatePointer
    if ($isCandidate) {
        $Candidates.Add([PSCustomObject]@{
            Pointer = $Pointer
            Schema = $Value
            Kind = $candidateKind
            IsNonDiscriminatedUnion = [string]::Equals(
                $candidateKind,
                "OneOf",
                [System.StringComparison]::Ordinal
            ) -and (Test-IsOpenApiNonDiscriminatedOneOfSchema -Value $Value -Context $Context)
            CandidateDepth = $CandidateDepth
            ParentCandidatePointer = $ParentCandidatePointer
            InheritedIdPointer = $InheritedIdPointer
            TypeEnumValue = Get-OpenApiObjectSchemaTypeEnumValue -Schema $Value
        })
        $childCandidateDepth++
        $childParentCandidatePointer = $Pointer
    }

    # Only `$id` establishes a base-URI scope inherited by descendants. Anchor
    # keywords are still rejected when they occur inside a moved candidate, but
    # they do not independently establish a descendant base URI.
    $childInheritedIdPointer = $InheritedIdPointer
    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        (Test-JsonProperty -Object $Value -Name '$id')) {
        $childInheritedIdPointer = "$Pointer/$(ConvertTo-JsonPointerToken -Token '$id')"
    }

    # Encounter order is retained throughout discovery. It later determines
    # component order and which occurrence receives an unsuffixed colliding name.
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childPointer = "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $childIsComponentsSchemaMap = [string]::Equals($Context, "Components", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)

        if ([string]::Equals($Context, "SchemaMap", [System.StringComparison]::Ordinal) -and
            $ComponentsSchemaMap) {
            Add-InlineOpenApiSchemaCandidates `
                -Value $property.Value `
                -Context $childContext `
                -Pointer $childPointer `
                -Candidates $Candidates `
                -SuppressCurrentCandidate `
                -CandidateDepth $childCandidateDepth `
                -ParentCandidatePointer $childParentCandidatePointer `
                -InheritedIdPointer $childInheritedIdPointer `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            continue
        }

        Add-InlineOpenApiSchemaCandidates `
            -Value $property.Value `
            -Context $childContext `
            -Pointer $childPointer `
            -Candidates $Candidates `
            -ComponentsSchemaMap:$childIsComponentsSchemaMap `
            -CandidateDepth $childCandidateDepth `
            -ParentCandidatePointer $childParentCandidatePointer `
            -InheritedIdPointer $childInheritedIdPointer `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Returns a snapshot array rather than the mutable List used during recursion.
# Callers can safely annotate candidate objects with generated names afterward.
function Get-InlineOpenApiSchemaCandidates {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $candidates = [System.Collections.Generic.List[object]]::new()
    Add-InlineOpenApiSchemaCandidates `
        -Value $Document `
        -Context OpenApi `
        -Pointer "#" `
        -Candidates $candidates
    [object[]]$candidateArray = [object[]]$candidates.ToArray()

    foreach ($candidate in $candidateArray) {
        if ($candidate.InheritedIdPointer -is [string] -and
            -not [string]::IsNullOrEmpty($candidate.InheritedIdPointer)) {
            throw "Cannot componentize schema at '$($candidate.Pointer)' because it inherits scope from `$id at '$($candidate.InheritedIdPointer)'."
        }
        $occurrence = Get-OpenApiScopeSensitiveSchemaKeywordOccurrence `
            -Value $candidate.Schema `
            -Context Schema `
            -Pointer $candidate.Pointer
        if ($null -ne $occurrence) {
            throw "Cannot componentize schema at '$($candidate.Pointer)' because scope-sensitive keyword '$($occurrence.Keyword)' at '$($occurrence.Pointer)' could change meaning after relocation."
        }
    }

    return $candidateArray
}

# Preserve the previous discovery helper as an object-only compatibility view.
# The componentizer itself uses Get-InlineOpenApiSchemaCandidates so direct oneOf
# owners and object candidates participate in one ordered naming pass.
function Get-InlineOpenApiObjectSchemaCandidates {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    return [object[]]@(
        Get-InlineOpenApiSchemaCandidates -Document $Document |
            Where-Object Kind -eq "Object"
    )
}

# Converts arbitrary API text into a portable PascalCase name token. Standard
# word splitting normalizes separators and acronym boundaries, while
# digit-leading words receive an `X` prefix.
function ConvertTo-OpenApiComponentNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $segments = [System.Text.RegularExpressions.Regex]::Matches(
        $Value,
        "[A-Z]+(?=[A-Z][a-z]|[0-9]|$)|[A-Z]?[a-z]+|[0-9]+"
    )
    $builder = [System.Text.StringBuilder]::new()
    foreach ($segmentMatch in $segments) {
        $segment = $segmentMatch.Value.ToLowerInvariant()
        if ($segment.Length -eq 0) {
            continue
        }
        if ([char]::IsDigit($segment[0]) -and $builder.Length -eq 0) {
            $null = $builder.Append("X")
        }
        $null = $builder.Append([char]::ToUpperInvariant($segment[0]))
        if ($segment.Length -gt 1) {
            $null = $builder.Append($segment.Substring(1))
        }
    }
    return $builder.ToString()
}

function ConvertTo-OpenApiLowerCamelNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $pascal = ConvertTo-OpenApiComponentNameToken -Value $Value
    if ([string]::IsNullOrWhiteSpace($pascal)) {
        return $pascal
    }
    $firstCharacter = [char]::ToLowerInvariant($pascal[0])
    return "$firstCharacter$($pascal.Substring(1))"
}

# Components generated from paths start with a normalized operationId. Requiring
# it avoids unstable names derived from mutable routes and HTTP methods.
function Get-OpenApiOperationComponentNamePrefix {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [string[]]$Tokens
    )

    if ($Tokens.Count -lt 3 -or
        -not [string]::Equals($Tokens[0], "paths", [System.StringComparison]::Ordinal)) {
        return $null
    }

    $paths = Get-JsonPropertyValue -Object $Document -Name "paths"
    $pathItem = Get-JsonPropertyValue -Object $paths -Name $Tokens[1]
    if (-not (Test-JsonObjectValue -Value $pathItem)) {
        return $null
    }
    $operation = Get-JsonPropertyValue -Object $pathItem -Name $Tokens[2]
    if (-not (Test-JsonObjectValue -Value $operation)) {
        return $null
    }
    $operationId = Get-JsonPropertyValue -Object $operation -Name "operationId"
    if ($operationId -isnot [string] -or [string]::IsNullOrWhiteSpace($operationId)) {
        $operationPointer = "#/paths/$(ConvertTo-JsonPointerToken -Token $Tokens[1])/$(ConvertTo-JsonPointerToken -Token $Tokens[2])"
        throw "HTTP operation at '$operationPointer' must define a nonempty operationId before component naming."
    }
    return ConvertTo-OpenApiLowerCamelNameToken -Value $operationId
}

# Returns a direct JSON shape label only when the schema states one scalar type.
# References, type arrays, and composition-only branches deliberately remain
# anonymous so naming never depends on reference resolution or inference.
function Get-OpenApiDirectSchemaShapeName {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    if (-not (Test-JsonObjectValue -Value $Schema) -or
        (Test-JsonProperty -Object $Schema -Name '$ref')) {
        return $null
    }
    $type = Get-JsonPropertyValue -Object $Schema -Name "type"
    if ($type -isnot [string]) {
        return $null
    }
    switch -CaseSensitive ($type) {
        "array" { return "Array" }
        "boolean" { return "Boolean" }
        "integer" { return "Integer" }
        "null" { return "Null" }
        "number" { return "Number" }
        "object" { return "Object" }
        "string" { return "String" }
        default { return $null }
    }
}

# Returns the one-based branch position when a candidate is a direct child of
# its nearest candidate parent's oneOf. Deeper descendants return null.
function Get-OpenApiDirectOneOfAlternativePosition {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentCandidate,

        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    [string[]]$parentTokens = Get-JsonPointerTokens -Pointer $ParentCandidate.Pointer
    [string[]]$candidateTokens = Get-JsonPointerTokens -Pointer $Candidate.Pointer
    if ($candidateTokens.Count -ne $parentTokens.Count + 2 -or
        -not [string]::Equals($candidateTokens[$parentTokens.Count], "oneOf", [System.StringComparison]::Ordinal)) {
        return $null
    }
    $index = 0
    if (-not [int]::TryParse(
            $candidateTokens[$parentTokens.Count + 1],
            [System.Globalization.NumberStyles]::None,
            [System.Globalization.CultureInfo]::InvariantCulture,
            [ref]$index)) {
        return $null
    }
    $oneOf = Get-JsonPropertyValue -Object $ParentCandidate.Schema -Name "oneOf"
    if (-not (Test-JsonArrayValue -Value $oneOf) -or $index -lt 0 -or $index -ge $oneOf.Count) {
        return $null
    }
    return $index + 1
}

function New-OpenApiComponentNameSegment {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet(
            "Operation",
            "Context",
            "Status",
            "SourceComponent",
            "Api",
            "Relation",
            "Role",
            "Discriminator",
            "Collision"
        )]
        [string]$Kind,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]$Value
    )

    return [PSCustomObject]@{
        Kind = $Kind
        Value = $Value
    }
}

function Join-OpenApiComponentNameSegments {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Segments
    )

    return @($Segments | ForEach-Object Value) -join "_"
}

function ConvertTo-OpenApiStatusNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $parts = [System.Text.RegularExpressions.Regex]::Matches($Value, "[A-Za-z0-9]+")
    return @($parts | ForEach-Object { $_.Value.ToLowerInvariant() }) -join ""
}

function Test-IsOpenApiPureDictionarySchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    if (-not (Test-JsonObjectValue -Value $Schema) -or
        -not (Test-JsonProperty -Object $Schema -Name "additionalProperties")) {
        return $false
    }
    $additionalProperties = Get-JsonPropertyValue -Object $Schema -Name "additionalProperties"
    if ($additionalProperties -is [bool] -and -not $additionalProperties) {
        return $false
    }
    $properties = Get-JsonPropertyValue -Object $Schema -Name "properties"
    return -not (Test-JsonObjectValue -Value $properties) -or
        @(Get-JsonProperties -Value $properties).Count -eq 0
}

function Get-OpenApiCandidatePrimaryRole {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    if ([string]::Equals($Candidate.Kind, "OneOf", [System.StringComparison]::Ordinal)) {
        if ($Candidate.IsNonDiscriminatedUnion) {
            return "union"
        }
        return "discriminatedObject"
    }
    if (Test-IsOpenApiPureDictionarySchema -Schema $Candidate.Schema) {
        return "dictionary"
    }
    return "object"
}

function Get-OpenApiDirectDiscriminatorIdentity {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentCandidate,

        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    $alternativePosition = Get-OpenApiDirectOneOfAlternativePosition `
        -ParentCandidate $ParentCandidate `
        -Candidate $Candidate
    if ($null -eq $alternativePosition) {
        return $null
    }
    $discriminator = Get-JsonPropertyValue -Object $ParentCandidate.Schema -Name "discriminator"
    if (-not (Test-JsonObjectValue -Value $discriminator)) {
        return $null
    }
    $propertyName = Get-JsonPropertyValue -Object $discriminator -Name "propertyName"
    if ($propertyName -isnot [string] -or [string]::IsNullOrWhiteSpace($propertyName)) {
        return $null
    }

    $plan = Get-OpenApiInlineDiscriminatorUnionPlan `
        -Schema $ParentCandidate.Schema `
        -PropertyName $propertyName `
        -Breadcrumb $ParentCandidate.Pointer `
        -RequireDiscriminator
    if ($null -eq $plan -or $alternativePosition -gt $plan.BranchPlans.Count) {
        return $null
    }
    $branchPlan = $plan.BranchPlans[$alternativePosition - 1]
    if (-not [string]::Equals($branchPlan.Kind, "Leaf", [System.StringComparison]::Ordinal)) {
        return $null
    }
    [string[]]$values = @($branchPlan.Values)
    if ($values.Count -ne 1) {
        return $null
    }

    $propertyToken = ConvertTo-OpenApiLowerCamelNameToken -Value $propertyName
    $valueToken = ConvertTo-OpenApiComponentNameToken -Value $values[0]
    if ([string]::IsNullOrWhiteSpace($propertyToken) -or
        [string]::IsNullOrWhiteSpace($valueToken)) {
        return $null
    }
    return [PSCustomObject]@{
        PropertyName = $propertyName
        Values = $values
        Segment = "${propertyToken}Of${valueToken}"
    }
}

function Get-OpenApiCandidateNamingContext {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Candidate,

        [Parameter()]
        [AllowNull()]
        [object]$ParentCandidate
    )

    [string[]]$candidateTokens = Get-JsonPointerTokens -Pointer $Candidate.Pointer
    $rootSegments = [System.Collections.Generic.List[object]]::new()
    $startIndex = 0
    if ($null -ne $ParentCandidate) {
        [string[]]$parentTokens = Get-JsonPointerTokens -Pointer $ParentCandidate.Pointer
        $startIndex = $parentTokens.Count
    }
    elseif ($candidateTokens.Count -ge 3 -and
        [string]::Equals($candidateTokens[0], "paths", [System.StringComparison]::Ordinal)) {
        $operationPrefix = Get-OpenApiOperationComponentNamePrefix `
            -Document $Document `
            -Tokens $candidateTokens
        $rootSegments.Add((New-OpenApiComponentNameSegment `
            -Kind Operation `
            -Value $operationPrefix))
        if ($candidateTokens -ccontains "requestBody") {
            $rootSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Context `
                -Value "request"))
        }
        elseif ($candidateTokens -ccontains "responses") {
            $rootSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Context `
                -Value "response"))
            $responsesIndex = [Array]::IndexOf($candidateTokens, "responses")
            if ($responsesIndex -lt 0 -or $responsesIndex + 1 -ge $candidateTokens.Count) {
                throw "Response schema candidate '$($Candidate.Pointer)' has no response status token."
            }
            $statusToken = ConvertTo-OpenApiStatusNameToken `
                -Value $candidateTokens[$responsesIndex + 1]
            if ([string]::IsNullOrWhiteSpace($statusToken)) {
                throw "Response schema candidate '$($Candidate.Pointer)' has an invalid response status token."
            }
            $rootSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Status `
                -Value $statusToken))
        }
        else {
            throw "Schema candidate '$($Candidate.Pointer)' is under an operation but not beneath requestBody or responses."
        }
        $schemaIndex = [Array]::IndexOf($candidateTokens, "schema")
        $startIndex = if ($schemaIndex -ge 0) { $schemaIndex + 1 } else { 3 }
    }
    elseif ($candidateTokens.Count -ge 3 -and
        [string]::Equals($candidateTokens[0], "components", [System.StringComparison]::Ordinal) -and
        [string]::Equals($candidateTokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        $rootSegments.Add((New-OpenApiComponentNameSegment `
            -Kind SourceComponent `
            -Value $candidateTokens[2]))
        $startIndex = 3
    }
    else {
        throw "Cannot derive a naming root for schema candidate '$($Candidate.Pointer)'."
    }

    [string[]]$relativeTokens = @()
    if ($startIndex -lt $candidateTokens.Count) {
        $relativeTokens = [string[]]$candidateTokens[$startIndex..($candidateTokens.Count - 1)]
    }
    $apiSegments = [System.Collections.Generic.List[object]]::new()
    $relationSegments = [System.Collections.Generic.List[object]]::new()
    for ($index = 0; $index -lt $relativeTokens.Count; $index++) {
        $token = $relativeTokens[$index]
        $isPropertyName = $index -gt 0 -and
            [string]::Equals($relativeTokens[$index - 1], "properties", [System.StringComparison]::Ordinal)
        if ($isPropertyName) {
            $apiToken = ConvertTo-OpenApiComponentNameToken -Value $token
            if (-not [string]::IsNullOrWhiteSpace($apiToken)) {
                $apiSegments.Add((New-OpenApiComponentNameSegment `
                    -Kind Api `
                    -Value $apiToken))
            }
            continue
        }

        if ([string]::Equals($token, "items", [System.StringComparison]::Ordinal)) {
            $relationSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Relation `
                -Value "array"))
            $relationSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Relation `
                -Value "item"))
            continue
        }
        if ([string]::Equals($token, "additionalProperties", [System.StringComparison]::Ordinal)) {
            $relationSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Relation `
                -Value "value"))
            continue
        }
        if ([string]::Equals($token, "prefixItems", [System.StringComparison]::Ordinal) -and
            $index + 1 -lt $relativeTokens.Count -and
            $relativeTokens[$index + 1] -match "^[0-9]+$") {
            $relationSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Relation `
                -Value "tuple"))
            $tuplePosition = [int]$relativeTokens[$index + 1] + 1
            $relationSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Relation `
                -Value "item$tuplePosition"))
            continue
        }
        if (@(
            "paths",
            "requestBody",
            "content",
            "schema",
            "properties",
            "components",
            "schemas",
            "oneOf",
            "anyOf",
            "allOf",
            "prefixItems",
            '$defs',
            "definitions",
            "dependentSchemas",
            "patternProperties"
        ) -ccontains $token -or
            $script:OpenApiHttpMethodNames -ccontains $token -or
            $token -match "^[0-9]+$" -or
            $token -match "^[^/]+/[^/]+$") {
            continue
        }

        $apiToken = ConvertTo-OpenApiComponentNameToken -Value $token
        if (-not [string]::IsNullOrWhiteSpace($apiToken)) {
            $apiSegments.Add((New-OpenApiComponentNameSegment `
                -Kind Api `
                -Value $apiToken))
        }
    }

    $discriminatorIdentity = if ($null -ne $ParentCandidate) {
        Get-OpenApiDirectDiscriminatorIdentity `
            -ParentCandidate $ParentCandidate `
            -Candidate $Candidate
    }
    else {
        $null
    }
    $primaryRole = Get-OpenApiCandidatePrimaryRole -Candidate $Candidate
    $compositionPointer = $null
    $repetitionRole = $null
    if ($null -ne $ParentCandidate -and
        $relativeTokens.Count -ge 2 -and
        [string]::Equals($relativeTokens[0], "oneOf", [System.StringComparison]::Ordinal) -and
        $relativeTokens[1] -match "^[0-9]+$" -and
        $apiSegments.Count -eq 0 -and
        $null -eq $discriminatorIdentity) {
        $compositionPointer = "$($ParentCandidate.Pointer)/oneOf"
        $relationValues = @($relationSegments | ForEach-Object Value)
        $repetitionRole = (@($relationValues) + $primaryRole) -join "_"
    }

    return [PSCustomObject]@{
        Pointer = $Candidate.Pointer
        RootSegments = [object[]]$rootSegments.ToArray()
        ApiSegments = [object[]]$apiSegments.ToArray()
        RelationSegments = [object[]]$relationSegments.ToArray()
        PrimaryRole = $primaryRole
        IsPureDictionary = Test-IsOpenApiPureDictionarySchema -Schema $Candidate.Schema
        DiscriminatorIdentity = $discriminatorIdentity
        CompositionPointer = $compositionPointer
        RepetitionRole = $repetitionRole
        RoleOrdinal = $null
        DirectShape = Get-OpenApiDirectSchemaShapeName -Schema $Candidate.Schema
    }
}

function Get-OpenApiCandidateLocalNameSegments {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Context
    )

    $segments = [System.Collections.Generic.List[object]]::new()
    foreach ($segment in $Context.ApiSegments) {
        $segments.Add($segment)
    }

    $ordinalAppliedToArray = $false
    for ($index = 0; $index -lt $Context.RelationSegments.Count; $index++) {
        $segment = $Context.RelationSegments[$index]
        $value = $segment.Value
        if ($index -eq 0 -and
            $null -ne $Context.RoleOrdinal -and
            [string]::Equals($value, "array", [System.StringComparison]::Ordinal)) {
            $value = "array$($Context.RoleOrdinal)"
            $ordinalAppliedToArray = $true
        }
        $segments.Add((New-OpenApiComponentNameSegment `
            -Kind Relation `
            -Value $value))
    }

    if ($null -ne $Context.DiscriminatorIdentity) {
        $segments.Add((New-OpenApiComponentNameSegment `
            -Kind Discriminator `
            -Value $Context.DiscriminatorIdentity.Segment))
    }
    else {
        $role = $Context.PrimaryRole
        if ($null -ne $Context.RoleOrdinal -and -not $ordinalAppliedToArray) {
            $role = "$role$($Context.RoleOrdinal)"
        }
        $segments.Add((New-OpenApiComponentNameSegment `
            -Kind Role `
            -Value $role))
    }
    return [object[]]$segments.ToArray()
}

# Assigns all names before any schemas are moved. Existing component names are
# reserved first, so generated names can never overwrite source-authored roots.
# Candidates arrive parent-before-child from discovery, which makes it possible
# to inherit the final (including collision suffix) parent name deterministically.
function Set-OpenApiSchemaComponentNames {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Candidates
    )

    $usedNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $components = Get-JsonPropertyValue -Object $Document -Name "components"
    if (Test-JsonObjectValue -Value $components) {
        $schemas = Get-JsonPropertyValue -Object $components -Name "schemas"
        if (Test-JsonObjectValue -Value $schemas) {
            foreach ($property in (Get-JsonProperties -Value $schemas)) {
                $null = $usedNames.Add($property.Name)
            }
        }
    }

    $candidateByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $Candidates) {
        $candidateByPointer[$candidate.Pointer] = $candidate
    }
    $contextByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $Candidates) {
        $parentCandidate = if ($candidate.ParentCandidatePointer -is [string] -and
            $candidateByPointer.ContainsKey($candidate.ParentCandidatePointer)) {
            $candidateByPointer[$candidate.ParentCandidatePointer]
        }
        else {
            $null
        }
        $contextByPointer[$candidate.Pointer] = Get-OpenApiCandidateNamingContext `
            -Document $Document `
            -Candidate $candidate `
            -ParentCandidate $parentCandidate
    }

    $repetitionGroups = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $Candidates) {
        $context = $contextByPointer[$candidate.Pointer]
        if ($context.CompositionPointer -isnot [string] -or
            $context.RepetitionRole -isnot [string]) {
            continue
        }
        $groupKey = "$($context.CompositionPointer)|$($context.RepetitionRole)"
        if (-not $repetitionGroups.ContainsKey($groupKey)) {
            $repetitionGroups[$groupKey] = [System.Collections.Generic.List[object]]::new()
        }
        $repetitionGroups[$groupKey].Add($context)
    }
    foreach ($group in $repetitionGroups.Values) {
        if ($group.Count -le 1) {
            continue
        }
        for ($index = 0; $index -lt $group.Count; $index++) {
            $group[$index].RoleOrdinal = $index + 1
        }
    }

    # Pointer lookup connects a nested candidate to the containing candidate's
    # complete final name, including any inherited collision suffix.
    $namedCandidateByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $Candidates) {
        $context = $contextByPointer[$candidate.Pointer]
        [object[]]$localSegments = @(Get-OpenApiCandidateLocalNameSegments -Context $context)
        [object[]]$baseSegments = [object[]]@()
        if ($candidate.ParentCandidatePointer -is [string] -and
            $namedCandidateByPointer.ContainsKey($candidate.ParentCandidatePointer)) {
            $parentCandidate = $namedCandidateByPointer[$candidate.ParentCandidatePointer]
            $baseSegments = [object[]](@($parentCandidate.NameSegments) + $localSegments)
        }
        else {
            $baseSegments = [object[]](@($context.RootSegments) + $localSegments)
        }
        $baseName = Join-OpenApiComponentNameSegments -Segments $baseSegments
        if ([string]::IsNullOrWhiteSpace($baseName)) {
            throw "Component naming produced an empty name for '$($candidate.Pointer)'."
        }

        $name = $baseName
        $collisionSuffix = $null
        # The first occurrence keeps the base name; conflicts use _1, _2, and so
        # on. Structurally identical schemas still receive separate components
        # here; persisted deduplication owns any structural consolidation.
        if (-not $usedNames.Add($name)) {
            $suffix = 1
            do {
                $name = "${baseName}_$suffix"
                $collisionSuffix = $suffix
                $suffix++
            } while (-not $usedNames.Add($name))
        }

        [object[]]$nameSegments = $baseSegments
        if ($null -ne $collisionSuffix) {
            $nameSegments = [object[]](@($baseSegments) + @(
                New-OpenApiComponentNameSegment `
                    -Kind Collision `
                    -Value $collisionSuffix.ToString([System.Globalization.CultureInfo]::InvariantCulture)
            ))
        }
        $namingPlan = [PSCustomObject]@{
            RootSegments = [object[]]$context.RootSegments
            LocalSegments = $localSegments
            BaseSegments = $baseSegments
            BaseName = $baseName
            CollisionSuffix = $collisionSuffix
            FinalSegments = $nameSegments
            FinalName = $name
        }

        $candidate | Add-Member -NotePropertyName Name -NotePropertyValue $name -Force
        $candidate | Add-Member -NotePropertyName NameSegments -NotePropertyValue $nameSegments -Force
        $candidate | Add-Member -NotePropertyName NamingPlan -NotePropertyValue $namingPlan -Force
        $candidate | Add-Member -NotePropertyName StructuralRole -NotePropertyValue $context.PrimaryRole -Force
        $candidate | Add-Member -NotePropertyName IsPureDictionary -NotePropertyValue $context.IsPureDictionary -Force
        $candidate | Add-Member -NotePropertyName DirectShape -NotePropertyValue $context.DirectShape -Force
        $candidate | Add-Member `
            -NotePropertyName DiscriminatorIdentity `
            -NotePropertyValue $context.DiscriminatorIdentity `
            -Force
        $candidate | Add-Member `
            -NotePropertyName RelationSegments `
            -NotePropertyValue ([object[]]$context.RelationSegments) `
            -Force
        $candidate | Add-Member -NotePropertyName RoleOrdinal -NotePropertyValue $context.RoleOrdinal -Force
        $candidate | Add-Member `
            -NotePropertyName DestinationPointer `
            -NotePropertyValue "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $name)" `
            -Force
        $namedCandidateByPointer[$candidate.Pointer] = $candidate
    }
}

# Copies a value while replacing candidate occurrences with refs. ComponentRootPointer
# is the one exception: while materializing a component definition, its root must
# remain a full schema and only nested candidates become refs.
function ConvertTo-ComponentizedOpenApiSchemaValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$CandidateByPointer,

        [Parameter()]
        [AllowNull()]
        [string]$ComponentRootPointer,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Inline schema componentization exceeds the supported depth of $MaximumDepth at '$Pointer'."
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
            $items[$index] = ConvertTo-ComponentizedOpenApiSchemaValue `
                -Value $Value[$index] `
                -Context $itemContext `
                -Pointer "$Pointer/$index" `
                -CandidateByPointer $CandidateByPointer `
                -ComponentRootPointer $ComponentRootPointer `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        $CandidateByPointer.ContainsKey($Pointer) -and
        -not [string]::Equals($Pointer, $ComponentRootPointer, [System.StringComparison]::Ordinal)) {
        return [ordered]@{
            '$ref' = $CandidateByPointer[$Pointer].DestinationPointer
        }
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $result[$property.Name] = ConvertTo-ComponentizedOpenApiSchemaValue `
            -Value $property.Value `
            -Context $childContext `
            -Pointer "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -CandidateByPointer $CandidateByPointer `
            -ComponentRootPointer $ComponentRootPointer `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

# Every generated component must be reachable after replacement. This catches a
# traversal/rewrite mismatch that would otherwise leave dead generated schemas.
function Assert-GeneratedOpenApiComponentsAreReferenced {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Candidates
    )

    $referencedPointers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Document)) {
        if ([string]::Equals($occurrence.Kind, "Reference", [System.StringComparison]::Ordinal)) {
            $null = $referencedPointers.Add((Get-CanonicalJsonPointer -Pointer $occurrence.Pointer))
        }
    }

    foreach ($candidate in $Candidates) {
        if (-not $referencedPointers.Contains($candidate.DestinationPointer)) {
            throw "Generated component '$($candidate.Name)' is not referenced."
        }
    }
}

# Orchestrates the immutable transformation:
#   1. discover and name candidates;
#   2. plan source-pointer relocations;
#   3. relocate source-authored refs/mappings;
#   4. replace inline occurrences;
#   5. materialize each generated component definition;
#   6. validate references and postconditions.
function ConvertTo-ComponentizedOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    [object[]]$candidates = @(Get-InlineOpenApiSchemaCandidates -Document $Document)
    if ($candidates.Count -eq 0) {
        $copy = Copy-JsonValue -Value $Document
        $mappingResult = ConvertTo-OpenApiDiscriminatorMappedDocument -Document $copy
        Assert-OpenApiReferencesResolve -Document $mappingResult.Document
        Assert-AdditionalOpenApiReferencesResolve -Document $mappingResult.Document
        return [PSCustomObject]@{
            Document = $mappingResult.Document
            DiscoveredSchemaCount = 0
            DiscoveredObjectSchemaCount = 0
            DiscoveredOneOfSchemaCount = 0
            GeneratedComponentCount = 0
            RelocationCount = 0
            DiscriminatorCount = $mappingResult.DiscriminatorCount
            ChangedDiscriminatorMappingCount = $mappingResult.ChangedMappingCount
            DiscriminatorMappingEntryCount = $mappingResult.MappingEntryCount
            Candidates = [object[]]@()
        }
    }

    Set-OpenApiSchemaComponentNames -Document $Document -Candidates $candidates
    $candidateByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $candidates) {
        $candidateByPointer[$candidate.Pointer] = $candidate
        Add-OpenApiPointerRelocation `
            -Relocations $relocations `
            -SourcePrefix $candidate.Pointer `
            -DestinationPrefix $candidate.DestinationPointer
    }

    # Relocate source-authored pointers before inserting generated refs. If this
    # order were reversed, a newly generated destination ref could accidentally
    # match a source-prefix relocation and be moved a second time.
    $relocatedSource = ConvertTo-RelocatedOpenApiPointers `
        -Value $Document `
        -Relocations $relocations
    $componentized = ConvertTo-ComponentizedOpenApiSchemaValue `
        -Value $relocatedSource `
        -Context OpenApi `
        -Pointer "#" `
        -CandidateByPointer $candidateByPointer

    $sourceComponents = Get-JsonPropertyValue -Object $componentized -Name "components"
    $sourceSchemas = if (Test-JsonObjectValue -Value $sourceComponents) {
        Get-JsonPropertyValue -Object $sourceComponents -Name "schemas"
    }
    else {
        $null
    }
    $schemas = [ordered]@{}
    if (Test-JsonObjectValue -Value $sourceSchemas) {
        foreach ($property in (Get-JsonProperties -Value $sourceSchemas)) {
            $schemas[$property.Name] = $property.Value
        }
    }

    # Materialize candidates in discovery order. Resolve each definition from
    # the relocated source so source-authored descendant refs already point at
    # their future component locations.
    foreach ($candidate in $candidates) {
        $sourceSchema = Resolve-LocalJsonPointer `
            -Document $relocatedSource `
            -Pointer $candidate.Pointer `
            -Context "component source"
        $schemas[$candidate.Name] = ConvertTo-ComponentizedOpenApiSchemaValue `
            -Value $sourceSchema `
            -Context Schema `
            -Pointer $candidate.Pointer `
            -CandidateByPointer $candidateByPointer `
            -ComponentRootPointer $candidate.Pointer
    }

    # Schema components are rebuilt as one ordered map. Other component sections
    # (for example securitySchemes) are copied without reinterpretation.
    $newComponents = [ordered]@{ schemas = $schemas }
    if (Test-JsonObjectValue -Value $sourceComponents) {
        foreach ($property in (Get-JsonProperties -Value $sourceComponents)) {
            if ([string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)) {
                continue
            }
            $newComponents[$property.Name] = $property.Value
        }
    }

    $result = [ordered]@{}
    $sawComponents = $false
    foreach ($property in (Get-JsonProperties -Value $componentized)) {
        if ([string]::Equals($property.Name, "components", [System.StringComparison]::Ordinal)) {
            $result[$property.Name] = $newComponents
            $sawComponents = $true
        }
        else {
            $result[$property.Name] = $property.Value
        }
    }
    if (-not $sawComponents) {
        $result["components"] = $newComponents
    }

    # Branch schemas now have stable component roots, so mappings can use actual
    # accepted payload values as keys and final local component pointers as values.
    $mappingResult = ConvertTo-OpenApiDiscriminatorMappedDocument -Document $result
    $result = $mappingResult.Document

    # These checks define the stage contract: all local references resolve, no
    # inline object/direct-oneOf candidates remain, and every generated root is used.
    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result
    [object[]]$remainingCandidates = @(Get-InlineOpenApiSchemaCandidates -Document $result)
    if ($remainingCandidates.Count -ne 0) {
        throw "Componentized output retained $($remainingCandidates.Count) inline schema candidates."
    }
    Assert-GeneratedOpenApiComponentsAreReferenced `
        -Document $result `
        -Candidates $candidates

    $objectCandidateCount = @($candidates | Where-Object Kind -eq "Object").Count
    $oneOfCandidateCount = @($candidates | Where-Object Kind -eq "OneOf").Count
    return [PSCustomObject]@{
        Document = $result
        DiscoveredSchemaCount = $candidates.Count
        DiscoveredObjectSchemaCount = $objectCandidateCount
        DiscoveredOneOfSchemaCount = $oneOfCandidateCount
        GeneratedComponentCount = $candidates.Count
        RelocationCount = $relocations.Count
        DiscriminatorCount = $mappingResult.DiscriminatorCount
        ChangedDiscriminatorMappingCount = $mappingResult.ChangedMappingCount
        DiscriminatorMappingEntryCount = $mappingResult.MappingEntryCount
        Candidates = $candidates
    }
}

# Default naming is additive and recognizes its own suffix, allowing a caller to
# rerun the command in place without producing `.componentized.componentized`.
function Get-DefaultComponentizedOpenApiPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullInputPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullInputPath)
    $fileName = [System.IO.Path]::GetFileName($fullInputPath)
    if ($fileName.EndsWith(".componentized.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".componentized.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullInputPath
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$baseName.componentized.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$baseName.componentized.json"
    }
    return Join-Path $directory "$fileName.componentized.json"
}

# Public paths are repository-relative for consistency with the other OpenAPI
# transformation scripts, while absolute paths remain supported for tooling/tests.
function Resolve-ComponentizerRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:ComponentizerRepositoryRoot $Path))
}

# File-oriented boundary: resolves defaults, reads once, performs the pure
# conversion, writes atomically, and returns the full structured result for tests
# and callers that need more than the host summary.
function Invoke-OpenApiObjectSchemaComponentizer {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$OutputPath
    )

    $usingCanonicalInput = [string]::IsNullOrWhiteSpace($InputPath)
    if ($usingCanonicalInput) {
        $InputPath = $script:DefaultComponentizerInputPath
    }
    else {
        $InputPath = Resolve-ComponentizerRepositoryPath -Path $InputPath
    }

    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The OpenAPI input path is not a file: $resolvedInputPath"
    }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = if ($usingCanonicalInput) {
            $script:DefaultComponentizerOutputPath
        }
        else {
            Get-DefaultComponentizedOpenApiPath -InputPath $resolvedInputPath
        }
    }
    else {
        $OutputPath = Resolve-ComponentizerRepositoryPath -Path $OutputPath
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-ComponentizedOpenApiDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Inline schemas discovered: $($result.DiscoveredSchemaCount)"
    Write-Host "Inline object schemas discovered: $($result.DiscoveredObjectSchemaCount)"
    Write-Host "Inline oneOf schemas discovered: $($result.DiscoveredOneOfSchemaCount)"
    Write-Host "Schema components generated: $($result.GeneratedComponentCount)"
    Write-Host "JSON Pointer prefixes relocated: $($result.RelocationCount)"
    Write-Host "Discriminators validated: $($result.DiscriminatorCount)"
    Write-Host "Discriminator mappings materialized: $($result.ChangedDiscriminatorMappingCount)"
    Write-Host "Discriminator mapping entries: $($result.DiscriminatorMappingEntryCount)"

    return $result
}

# Dot-sourcing exposes conversion functions to tests without running the CLI.
if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiObjectSchemaComponentizer `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}
