<#
.SYNOPSIS
    Moves inline object and direct oneOf schemas into OpenAPI components.

.DESCRIPTION
    Finds every inline Schema Object that owns a direct nonempty oneOf or has the
    exact scalar type "object", creates one components.schemas entry per occurrence,
    and replaces each inline definition with a local $ref. A direct oneOf owner
    takes precedence when a schema satisfies both predicates. Nested candidates
    become separate components referenced by their containing component.

    Component names combine underscore-separated source-path tokens and a
    TypeOf-prefixed singleton string enum from properties.type when available.
    Nested candidates inherit their containing component's semantic name before
    appending their relative path and enum tokens. Structural composition/item
    tokens are omitted. A direct oneOf without a discriminator receives a Union
    marker; otherwise anonymous descendants use ArrayItem, Value, a unique JSON
    shape, or AlternativeN before numeric collision suffixes beginning with `_1`.
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
    specification/openapi/responses.openapi.json beneath the repository root.

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
$script:DefaultComponentizerInputPath = Join-Path $script:ComponentizerRepositoryRoot "specification" "openapi" "responses.openapi.json"
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

# A singleton `properties.type.enum` is the strongest local semantic label we
# have for discriminated variants. For example, an object whose type enum is
# `function` contributes `TypeOfFunction` to its generated component name.
# Multi-value or non-string enums are intentionally ignored because selecting
# one value would make the generated name misleading.
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
# only on their containing schema (for example `TypeOfToolSearchOutput`).
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

# Converts arbitrary path/enum text into a portable PascalCase name token.
# Separators are intentionally discarded: `tool_search_output` becomes
# `ToolSearchOutput`, while digit-leading segments receive an `X` prefix so a
# generated name never begins one of its parts with a bare number.
function ConvertTo-OpenApiComponentNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $segments = [System.Text.RegularExpressions.Regex]::Matches($Value, "[A-Za-z0-9]+")
    $builder = [System.Text.StringBuilder]::new()
    foreach ($segmentMatch in $segments) {
        $segment = $segmentMatch.Value
        if ($segment.Length -eq 0) {
            continue
        }
        if ([char]::IsDigit($segment[0])) {
            $null = $builder.Append("X")
        }
        $null = $builder.Append([char]::ToUpperInvariant($segment[0]))
        if ($segment.Length -gt 1) {
            $null = $builder.Append($segment.Substring(1))
        }
    }
    return $builder.ToString()
}

# Components generated from paths start with the selected operationId, which is
# more stable and useful than an HTTP method plus escaped route. The fallback is
# retained for valid operations that happen not to declare operationId.
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
    if ($operationId -is [string] -and -not [string]::IsNullOrWhiteSpace($operationId)) {
        return ConvertTo-OpenApiComponentNameToken -Value $operationId
    }

    return (ConvertTo-OpenApiComponentNameToken -Value $Tokens[2]) +
        (ConvertTo-OpenApiComponentNameToken -Value $Tokens[1])
}

function Add-OpenApiComponentNamePart {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.List[string]]$Parts,

        [Parameter()]
        [AllowEmptyString()]
        [string]$Part
    )

    if ([string]::IsNullOrWhiteSpace($Part)) {
        return
    }
    if ($Parts.Count -eq 0 -or
        -not [string]::Equals($Parts[$Parts.Count - 1], $Part, [System.StringComparison]::Ordinal)) {
        $Parts.Add($Part)
    }
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

function Get-OpenApiUniqueOneOfAlternativeShapeName {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentCandidate,

        [Parameter(Mandatory = $true)]
        [int]$AlternativePosition
    )

    $oneOf = Get-JsonPropertyValue -Object $ParentCandidate.Schema -Name "oneOf"
    $shape = Get-OpenApiDirectSchemaShapeName -Schema $oneOf[$AlternativePosition - 1]
    if ($null -eq $shape) {
        return $null
    }
    $matchingShapeCount = 0
    foreach ($branch in $oneOf) {
        $branchShape = Get-OpenApiDirectSchemaShapeName -Schema $branch
        if ($branchShape -is [string] -and
            [string]::Equals($branchShape, $shape, [System.StringComparison]::Ordinal)) {
            $matchingShapeCount++
        }
    }
    if ($matchingShapeCount -eq 1) {
        return $shape
    }
    return $null
}

# Produces the semantic suffix below an already-named parent candidate. Generic
# storage roles are introduced only for a non-discriminated union owner or one
# of its otherwise anonymous descendants.
function Get-OpenApiNestedSchemaNameParts {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentCandidate,

        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    [string[]]$parentTokens = Get-JsonPointerTokens -Pointer $ParentCandidate.Pointer
    [string[]]$candidateTokens = Get-JsonPointerTokens -Pointer $Candidate.Pointer
    $relativeTokens = [System.Collections.Generic.List[string]]::new()
    $parts = [System.Collections.Generic.List[string]]::new()
    for ($index = $parentTokens.Count; $index -lt $candidateTokens.Count; $index++) {
        $token = $candidateTokens[$index]
        $relativeTokens.Add($token)
        if (@(
            "properties",
            "items",
            "additionalProperties",
            "oneOf",
            "anyOf",
            "allOf",
            "prefixItems"
        ) -ccontains $token -or $token -match "^[0-9]+$") {
            continue
        }
        Add-OpenApiComponentNamePart `
            -Parts $parts `
            -Part (ConvertTo-OpenApiComponentNameToken -Value $token)
    }

    $usesUnionRoles = $Candidate.IsNonDiscriminatedUnion -or
        $ParentCandidate.IsNonDiscriminatedUnion
    if ($parts.Count -eq 0 -and
        $usesUnionRoles -and
        $Candidate.TypeEnumValue -isnot [string]) {
        $role = $null
        if ($relativeTokens.Count -gt 0 -and
            [string]::Equals(
                $relativeTokens[$relativeTokens.Count - 1],
                "additionalProperties",
                [System.StringComparison]::Ordinal
            )) {
            $role = "Value"
        }
        elseif ($relativeTokens.Count -eq 3 -and
            [string]::Equals($relativeTokens[0], "oneOf", [System.StringComparison]::Ordinal) -and
            $relativeTokens[1] -match "^[0-9]+$" -and
            [string]::Equals($relativeTokens[2], "items", [System.StringComparison]::Ordinal)) {
            $role = "ArrayItem"
        }
        else {
            $alternativePosition = Get-OpenApiDirectOneOfAlternativePosition `
                -ParentCandidate $ParentCandidate `
                -Candidate $Candidate
            if ($null -ne $alternativePosition) {
                $role = Get-OpenApiUniqueOneOfAlternativeShapeName `
                    -ParentCandidate $ParentCandidate `
                    -AlternativePosition $alternativePosition
                if ($null -eq $role) {
                    $role = "Alternative$alternativePosition"
                }
            }
        }
        Add-OpenApiComponentNamePart -Parts $parts -Part $role
    }

    if ($Candidate.TypeEnumValue -is [string]) {
        $enumToken = ConvertTo-OpenApiComponentNameToken -Value $Candidate.TypeEnumValue
        if (-not [string]::IsNullOrWhiteSpace($enumToken)) {
            Add-OpenApiComponentNamePart -Parts $parts -Part "TypeOf$enumToken"
        }
    }
    if ($Candidate.IsNonDiscriminatedUnion) {
        Add-OpenApiComponentNamePart -Parts $parts -Part "Union"
    }
    return [string[]]$parts.ToArray()
}

# Builds the fallback name directly from a candidate's source pointer. Structural
# routing tokens (`properties`, `oneOf`, array indexes, media types, and so on)
# are omitted, while semantic property names and the candidate's own type enum
# are retained.
#
# Nested candidates use their already-named parent in
# Set-OpenApiSchemaComponentNames. This fallback remains necessary for
# top-level candidates.
function Get-OpenApiSchemaComponentBaseName {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    [string[]]$tokens = Get-JsonPointerTokens -Pointer $Candidate.Pointer
    $parts = [System.Collections.Generic.List[string]]::new()

    $startIndex = 0
    $operationPrefix = Get-OpenApiOperationComponentNamePrefix -Document $Document -Tokens $tokens
    if (-not [string]::IsNullOrWhiteSpace($operationPrefix)) {
        Add-OpenApiComponentNamePart -Parts $parts -Part $operationPrefix
        if ($tokens -ccontains "requestBody") {
            Add-OpenApiComponentNamePart -Parts $parts -Part "Request"
        }
        elseif ($tokens -ccontains "responses") {
            Add-OpenApiComponentNamePart -Parts $parts -Part "Response"
            $responsesIndex = [Array]::IndexOf($tokens, "responses")
            if ($responsesIndex -ge 0 -and $responsesIndex + 1 -lt $tokens.Count) {
                Add-OpenApiComponentNamePart `
                    -Parts $parts `
                    -Part (ConvertTo-OpenApiComponentNameToken -Value $tokens[$responsesIndex + 1])
            }
        }
        $schemaIndex = [Array]::IndexOf($tokens, "schema")
        $startIndex = if ($schemaIndex -ge 0) { $schemaIndex + 1 } else { 3 }
    }
    elseif ($tokens.Count -ge 3 -and
        [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal) -and
        [string]::Equals($tokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        Add-OpenApiComponentNamePart `
            -Parts $parts `
            -Part (ConvertTo-OpenApiComponentNameToken -Value $tokens[2])
        $startIndex = 3
    }

    for ($index = $startIndex; $index -lt $tokens.Count; $index++) {
        $token = $tokens[$index]
        if (@(
            "paths",
            "requestBody",
            "content",
            "schema",
            "properties",
            "components",
            "schemas",
            "items",
            "additionalProperties",
            "oneOf",
            "anyOf",
            "allOf",
            "prefixItems"
        ) -ccontains $token -or
            $script:OpenApiHttpMethodNames -ccontains $token -or
            $token -match "^[0-9]+$") {
            continue
        }
        if ($token -match "^[^/]+/[^/]+$") {
            continue
        }

        $nameToken = ConvertTo-OpenApiComponentNameToken -Value $token
        Add-OpenApiComponentNamePart -Parts $parts -Part $nameToken
    }

    if ($Candidate.TypeEnumValue -is [string]) {
        $enumToken = ConvertTo-OpenApiComponentNameToken -Value $Candidate.TypeEnumValue
        if (-not [string]::IsNullOrWhiteSpace($enumToken)) {
            Add-OpenApiComponentNamePart -Parts $parts -Part "TypeOf$enumToken"
        }
    }
    if ($Candidate.IsNonDiscriminatedUnion) {
        Add-OpenApiComponentNamePart -Parts $parts -Part "Union"
    }

    $baseName = $parts -join "_"
    if ([string]::IsNullOrWhiteSpace($baseName)) {
        return "Object"
    }
    return $baseName
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

    # Pointer lookup connects a nested candidate to the containing candidate's
    # final generated name. Example:
    #   parent: .../oneOf/10, type=tool_search_output
    #   child:  .../oneOf/10/properties/tools/items/oneOf/0, type=function
    # becomes:
    #   CreateResponse_Request_Input_TypeOfToolSearchOutput_Tools_TypeOfFunction
    $namedCandidateByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $Candidates) {
        $baseName = $null
        if ($candidate.ParentCandidatePointer -is [string] -and
            $namedCandidateByPointer.ContainsKey($candidate.ParentCandidatePointer)) {
            $parentCandidate = $namedCandidateByPointer[$candidate.ParentCandidatePointer]
            [string[]]$nestedParts = @(Get-OpenApiNestedSchemaNameParts `
                -ParentCandidate $parentCandidate `
                -Candidate $candidate)
            if ($nestedParts.Count -gt 0) {
                $baseName = "$($parentCandidate.Name)_$($nestedParts -join '_')"
            }
            else {
                $baseName = $parentCandidate.Name
            }
        }
        if ([string]::IsNullOrWhiteSpace($baseName)) {
            $baseName = Get-OpenApiSchemaComponentBaseName `
                -Document $Document `
                -Candidate $candidate
        }
        $name = $baseName
        # The first occurrence keeps the base name; conflicts use _1, _2, and so
        # on. Structurally identical schemas still receive separate components
        # here; persisted deduplication owns any structural consolidation.
        if (-not $usedNames.Add($name)) {
            $suffix = 1
            do {
                $name = "${baseName}_$suffix"
                $suffix++
            } while (-not $usedNames.Add($name))
        }

        $candidate | Add-Member -NotePropertyName Name -NotePropertyValue $name
        $candidate | Add-Member `
            -NotePropertyName DestinationPointer `
            -NotePropertyValue "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $name)"
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
