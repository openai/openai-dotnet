#Requires -Version 7.0

# Private graph, fingerprint, and mapping helpers shared by the component
# deduplication scripts. The public scripts dot-source the document helpers first.
#
# The implementation has five stages:
#   1. discover component-to-component pointer edges;
#   2. classify cycles and every component that depends on one;
#   3. recursively fingerprint the remaining acyclic component trees;
#   4. group trees with identical canonical content;
#   5. reconcile those current groups with persisted, reviewed identities.
#
# Component encounter order is preserved for readable, deterministic artifacts.
# Object property order is not part of structural identity, so canonicalization
# sorts properties before hashing.

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Centralizes the required OpenAPI shape check used by both analysis and apply.
# An empty schemas object is valid; a missing/non-object container is not.
function Get-OpenApiComponentSchemas {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $components = Get-JsonPropertyValue -Object $Document -Name "components"
    if (-not (Test-JsonObjectValue -Value $components)) {
        throw "The OpenAPI document must contain a components object."
    }
    $schemas = Get-JsonPropertyValue -Object $components -Name "schemas"
    if (-not (Test-JsonObjectValue -Value $schemas)) {
        throw "The OpenAPI document must contain a components.schemas object."
    }
    return $schemas
}

# Parses only local pointers rooted at components.schemas and returns both the
# component root and any descendant suffix. Canonicalizing first handles escaped
# names consistently. For example:
#   #/components/schemas/A~1B/properties/value
# identifies component `A/B` with suffix `/properties/value`.
# External URLs and local pointers outside the schema map return null because
# they do not create edges in the component dependency graph.
function Get-OpenApiSchemaComponentPointerInfo {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer
    )

    if (-not $Pointer.StartsWith("#", [System.StringComparison]::Ordinal)) {
        return $null
    }

    $canonicalPointer = Get-CanonicalJsonPointer -Pointer $Pointer
    [string[]]$tokens = Get-JsonPointerTokens -Pointer $canonicalPointer
    if ($tokens.Count -lt 3 -or
        -not [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal) -or
        -not [string]::Equals($tokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        return $null
    }

    $suffix = ""
    if ($tokens.Count -gt 3) {
        $suffix = "/" + (($tokens[3..($tokens.Count - 1)] | ForEach-Object {
            ConvertTo-JsonPointerToken -Token $_
        }) -join "/")
    }

    return [PSCustomObject]@{
        Pointer = $canonicalPointer
        ComponentName = $tokens[2]
        ComponentPointer = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $tokens[2])"
        Suffix = $suffix
    }
}

# Walks arbitrary schema JSON and yields every supported component-pointer use.
# Ordinary refs, dynamic refs, recursive refs, and discriminator mapping values
# all affect whole-tree identity. The small context state machine is important:
# arbitrary strings elsewhere under `discriminator` are metadata, not pointers.
function Get-OpenApiComponentPointerOccurrences {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [ValidateSet("Default", "Discriminator", "Mapping")]
        [string]$Context = "Default",

        [Parameter()]
        [string]$Breadcrumb = "#",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Component reference discovery exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Get-OpenApiComponentPointerOccurrences `
                -Value $Value[$index] `
                -Context Default `
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
        $kind = $null
        if ([string]::Equals($Context, "Mapping", [System.StringComparison]::Ordinal) -and
            $property.Value -is [string]) {
            $kind = "DiscriminatorMapping"
        }
        elseif (@('$ref', '$dynamicRef', '$recursiveRef') -ccontains $property.Name -and
            $property.Value -is [string]) {
            $kind = $property.Name
        }

        if ($null -ne $kind) {
            $pointerInfo = Get-OpenApiSchemaComponentPointerInfo -Pointer $property.Value
            if ($null -ne $pointerInfo) {
                [PSCustomObject]@{
                    Kind = $kind
                    Pointer = $pointerInfo.Pointer
                    ComponentName = $pointerInfo.ComponentName
                    ComponentPointer = $pointerInfo.ComponentPointer
                    Suffix = $pointerInfo.Suffix
                    Breadcrumb = $propertyBreadcrumb
                }
            }
            continue
        }

        # Enter Mapping only along discriminator.mapping. This avoids treating
        # examples or unrelated object-valued properties as reference maps.
        $childContext = "Default"
        if ([string]::Equals($Context, "Default", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "discriminator", [System.StringComparison]::Ordinal)) {
            $childContext = "Discriminator"
        }
        elseif ([string]::Equals($Context, "Discriminator", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "mapping", [System.StringComparison]::Ordinal)) {
            $childContext = "Mapping"
        }

        Get-OpenApiComponentPointerOccurrences `
            -Value $property.Value `
            -Context $childContext `
            -Breadcrumb $propertyBreadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Iterative reachability is used for cycle classification. Starting from direct
# dependencies means asking CanReach(A, A) detects self-loops and larger strongly
# connected regions without recursion or a separate graph library.
function Test-OpenApiComponentCanReach {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Start,

        [Parameter(Mandatory = $true)]
        [string]$Target,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$Dependencies
    )

    $pending = [System.Collections.Generic.Stack[string]]::new()
    $visited = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($dependency in @($Dependencies[$Start])) {
        $pending.Push($dependency)
    }

    while ($pending.Count -gt 0) {
        $current = $pending.Pop()
        if ([string]::Equals($current, $Target, [System.StringComparison]::Ordinal)) {
            return $true
        }
        if (-not $visited.Add($current)) {
            continue
        }
        foreach ($dependency in @($Dependencies[$current])) {
            $pending.Push($dependency)
        }
    }
    return $false
}

# Produces an unambiguous, culture-invariant scalar encoding for canonical tree
# text. Type and string length delimiters distinguish values such as number 1,
# string "1", and concatenations that might otherwise serialize identically.
function Get-CanonicalOpenApiJsonScalarText {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    if ($null -eq $Value) {
        return "null"
    }
    if ($Value -is [string]) {
        return "string:$($Value.Length):$Value"
    }
    if ($Value -is [bool]) {
        if ($Value) {
            return "boolean:true"
        }
        return "boolean:false"
    }
    if ($Value -is [char]) {
        return "char:$([int]$Value)"
    }
    if ($Value -is [System.IFormattable]) {
        return "$($Value.GetType().FullName):$($Value.ToString($null, [System.Globalization.CultureInfo]::InvariantCulture))"
    }
    return "$($Value.GetType().FullName):$Value"
}

# Serializes one acyclic schema tree into deterministic structural text:
#   * array order is retained because JSON arrays are ordered;
#   * object properties are sorted ordinally because member order is not semantic;
#   * a component pointer embeds the target tree fingerprint, not its name;
#   * pointer keyword kind and descendant suffix remain part of the identity.
#
# The name-independent pointer rule is what recognizes whole-tree duplicates.
# If LeafA and LeafB are equal, then ParentA -> LeafA and ParentB -> LeafB can
# also be equal even though their literal `$ref` strings differ.
function Get-OpenApiComponentTreeCanonicalValueText {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [object]$State,

        [Parameter()]
        [ValidateSet("Default", "Discriminator", "Mapping")]
        [string]$Context = "Default",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Component tree canonicalization exceeds the supported depth of $MaximumDepth."
    }
    if ($null -eq $Value -or
        (-not (Test-JsonObjectValue -Value $Value) -and -not (Test-JsonArrayValue -Value $Value))) {
        return Get-CanonicalOpenApiJsonScalarText -Value $Value
    }
    if (Test-JsonArrayValue -Value $Value) {
        $items = [System.Collections.Generic.List[string]]::new()
        foreach ($item in $Value) {
            $items.Add((Get-OpenApiComponentTreeCanonicalValueText `
                -Value $item `
                -State $State `
                -Context Default `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth))
        }
        return "array:$($Value.Count):[" + ($items -join ",") + "]"
    }

    # Sorting here makes fingerprints stable across harmless source property
    # reordering while leaving array order untouched above.
    $properties = @(Get-JsonProperties -Value $Value)
    [string[]]$propertyNames = @($properties | ForEach-Object { $_.Name })
    [System.Array]::Sort($propertyNames, [System.StringComparer]::Ordinal)
    $parts = [System.Collections.Generic.List[string]]::new()
    foreach ($propertyName in $propertyNames) {
        $propertyValue = Get-JsonPropertyValue -Object $Value -Name $propertyName
        $kind = $null
        if ([string]::Equals($Context, "Mapping", [System.StringComparison]::Ordinal) -and
            $propertyValue -is [string]) {
            $kind = "DiscriminatorMapping"
        }
        elseif (@('$ref', '$dynamicRef', '$recursiveRef') -ccontains $propertyName -and
            $propertyValue -is [string]) {
            $kind = $propertyName
        }

        if ($null -ne $kind) {
            $pointerInfo = Get-OpenApiSchemaComponentPointerInfo -Pointer $propertyValue
            if ($null -ne $pointerInfo -and $State.Schemas.ContainsKey($pointerInfo.ComponentName)) {
            # Recursive identity lookup is safe because cycle-dependent
            # components were excluded before canonicalization starts.
                $identity = Get-OpenApiComponentTreeIdentity `
                    -ComponentName $pointerInfo.ComponentName `
                    -State $State
                $propertyText = "component-pointer:${kind}:$($identity.Fingerprint):$($pointerInfo.Suffix)"
            }
            else {
                # Non-component and external refs remain literal identity data.
                # Local pointers are canonicalized to normalize equivalent escapes.
                $pointerText = if ($propertyValue.StartsWith("#", [System.StringComparison]::Ordinal)) {
                    Get-CanonicalJsonPointer -Pointer $propertyValue
                }
                else {
                    $propertyValue
                }
                $propertyText = "pointer:${kind}:$pointerText"
            }
        }
        else {
            $childContext = "Default"
            if ([string]::Equals($Context, "Default", [System.StringComparison]::Ordinal) -and
                [string]::Equals($propertyName, "discriminator", [System.StringComparison]::Ordinal)) {
                $childContext = "Discriminator"
            }
            elseif ([string]::Equals($Context, "Discriminator", [System.StringComparison]::Ordinal) -and
                [string]::Equals($propertyName, "mapping", [System.StringComparison]::Ordinal)) {
                $childContext = "Mapping"
            }
            $propertyText = Get-OpenApiComponentTreeCanonicalValueText `
                -Value $propertyValue `
                -State $State `
                -Context $childContext `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        $parts.Add("$($propertyName.Length):$propertyName=$propertyText")
    }
    return "object:$($properties.Count):{" + ($parts -join ",") + "}"
}

# Memoizes each acyclic component identity so shared descendants are hashed once.
# SHA-256 is the compact persisted fingerprint; CanonicalText is retained in
# memory and later included in the grouping key as defense against a hash collision.
function Get-OpenApiComponentTreeIdentity {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ComponentName,

        [Parameter(Mandatory = $true)]
        [object]$State
    )

    if ($State.Identities.ContainsKey($ComponentName)) {
        return $State.Identities[$ComponentName]
    }
    if ($State.CycleDependent.Contains($ComponentName)) {
        throw "Cannot fingerprint cycle-dependent component '$ComponentName'."
    }
    if (-not $State.Visiting.Add($ComponentName)) {
        throw "Component graph cycle was not classified before fingerprinting '$ComponentName'."
    }

    try {
        $canonicalText = Get-OpenApiComponentTreeCanonicalValueText `
            -Value $State.Schemas[$ComponentName] `
            -State $State
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($canonicalText)
        $fingerprint = "sha256:$([System.Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant())"
        $identity = [PSCustomObject]@{
            ComponentName = $ComponentName
            Fingerprint = $fingerprint
            CanonicalText = $canonicalText
        }
        $State.Identities[$ComponentName] = $identity
        return $identity
    }
    finally {
        $null = $State.Visiting.Remove($ComponentName)
    }
}

# Builds the complete analysis consumed by both mapping reconciliation and apply.
# It preserves source component order while deriving dependencies, cycle safety,
# recursive identities, duplicate groups, and expected removal counts.
function Get-OpenApiComponentDeduplicationAnalysis {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $schemaMap = Get-OpenApiComponentSchemas -Document $Document
    $componentNames = [System.Collections.Generic.List[string]]::new()
    $schemas = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($property in (Get-JsonProperties -Value $schemaMap)) {
        $componentNames.Add($property.Name)
        $schemas[$property.Name] = $property.Value
    }

    # Dependency lists are unique but retain first pointer encounter order.
    # Missing component targets fail here, before recursive fingerprinting could
    # produce an identity for an invalid graph.
    $dependencies = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $occurrencesByComponent = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($componentName in $componentNames) {
        $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $dependencyList = [System.Collections.Generic.List[string]]::new()
        [object[]]$occurrences = @(Get-OpenApiComponentPointerOccurrences `
            -Value $schemas[$componentName] `
            -Breadcrumb "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $componentName)")
        foreach ($occurrence in $occurrences) {
            if (-not $schemas.ContainsKey($occurrence.ComponentName)) {
                throw "$($occurrence.Kind) at '$($occurrence.Breadcrumb)' targets missing schema component '$($occurrence.ComponentName)'."
            }
            if ($seen.Add($occurrence.ComponentName)) {
                $dependencyList.Add($occurrence.ComponentName)
            }
        }
        $dependencies[$componentName] = [object[]]$dependencyList.ToArray()
        $occurrencesByComponent[$componentName] = $occurrences
    }

    # A node belongs to a cycle exactly when it can reach itself through one or
    # more dependency edges.
    $cycleNodes = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($componentName in $componentNames) {
        if (Test-OpenApiComponentCanReach `
            -Start $componentName `
            -Target $componentName `
            -Dependencies $dependencies) {
            $null = $cycleNodes.Add($componentName)
        }
    }

    # Fingerprinting must also exclude acyclic ancestors of cycles: their tree
    # identity would still depend on a recursive subtree. Reverse traversal marks
    # the full conservative exclusion set, not just cycle members themselves.
    $reverseDependencies = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($componentName in $componentNames) {
        $reverseDependencies[$componentName] = [System.Collections.Generic.List[string]]::new()
    }
    foreach ($sourceName in $componentNames) {
        foreach ($targetName in @($dependencies[$sourceName])) {
            $reverseDependencies[$targetName].Add($sourceName)
        }
    }

    $cycleDependent = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $pending = [System.Collections.Generic.Queue[string]]::new()
    foreach ($cycleNode in $cycleNodes) {
        if ($cycleDependent.Add($cycleNode)) {
            $pending.Enqueue($cycleNode)
        }
    }
    while ($pending.Count -gt 0) {
        $current = $pending.Dequeue()
        foreach ($parent in $reverseDependencies[$current]) {
            if ($cycleDependent.Add($parent)) {
                $pending.Enqueue($parent)
            }
        }
    }

    $state = [PSCustomObject]@{
        Schemas = $schemas
        Dependencies = $dependencies
        CycleDependent = $cycleDependent
        Identities = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
        Visiting = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    }

    # Use fingerprint plus canonical text as the key. The hash keeps persisted
    # state compact; comparing canonical text here prevents accidental collapse
    # even in the theoretical event of a SHA-256 collision.
    $identityGroups = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($componentName in $componentNames) {
        if ($cycleDependent.Contains($componentName)) {
            continue
        }
        $identity = Get-OpenApiComponentTreeIdentity -ComponentName $componentName -State $state
        $groupKey = "$($identity.Fingerprint)`n$($identity.CanonicalText)"
        if (-not $identityGroups.ContainsKey($groupKey)) {
            $identityGroups[$groupKey] = [System.Collections.Generic.List[string]]::new()
        }
        $identityGroups[$groupKey].Add($componentName)
    }

    # Emit each group once, at its first member's source position. Members and
    # groups therefore remain deterministic and easy to review in generated JSON.
    $duplicateGroups = [System.Collections.Generic.List[object]]::new()
    foreach ($componentName in $componentNames) {
        if ($cycleDependent.Contains($componentName)) {
            continue
        }
        $identity = $state.Identities[$componentName]
        $groupKey = "$($identity.Fingerprint)`n$($identity.CanonicalText)"
        $members = $identityGroups[$groupKey]
        if ($members.Count -lt 2 -or
            -not [string]::Equals($members[0], $componentName, [System.StringComparison]::Ordinal)) {
            continue
        }
        $duplicateGroups.Add([PSCustomObject]@{
            Fingerprint = $identity.Fingerprint
            CanonicalText = $identity.CanonicalText
            GroupKey = $groupKey
            Members = [object[]]$members.ToArray()
        })
    }

    $removableCount = 0
    foreach ($group in $duplicateGroups) {
        $removableCount += $group.Members.Count - 1
    }

    return [PSCustomObject]@{
        ComponentNames = [object[]]$componentNames.ToArray()
        Schemas = $schemas
        Dependencies = $dependencies
        OccurrencesByComponent = $occurrencesByComponent
        Identities = $state.Identities
        DuplicateGroups = [object[]]$duplicateGroups.ToArray()
        DuplicateGroupCount = $duplicateGroups.Count
        RemovableComponentCount = $removableCount
        CycleComponents = [object[]]@($componentNames | Where-Object { $cycleNodes.Contains($_) })
        CycleDependentComponents = [object[]]@($componentNames | Where-Object { $cycleDependent.Contains($_) })
    }
}

# Version 1 stores only reviewed decisions needed across runs. Canonical tree text
# is deliberately omitted; it is recomputed from source and represented by the
# compact fingerprint.
function New-OpenApiComponentDeduplicationMapping {
    return [ordered]@{
        version = [long]1
        groups = [object[]]@()
    }
}

# Mapping JSON has a stricter contract than general OpenAPI JSON. Parse and
# validate at the boundary so downstream reconciliation can rely on its shape.
function Read-OpenApiComponentDeduplicationMapping {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    try {
        $mapping = Get-Content -LiteralPath $Path -Raw -Encoding utf8 |
            ConvertFrom-Json -AsHashtable -Depth 100
    }
    catch {
        throw "Could not parse component deduplication mapping JSON at '$Path': $($_.Exception.Message)"
    }
    Assert-OpenApiComponentDeduplicationMapping -Mapping $mapping
    return $mapping
}

# Validates both per-group fields and mapping-wide ownership invariants:
# unique ids, unique canonical names, globally disjoint members, a representative
# contained in its group, and a correctly shaped SHA-256 fingerprint.
function Assert-OpenApiComponentDeduplicationMapping {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Mapping
    )

    if (-not (Test-JsonObjectValue -Value $Mapping)) {
        throw "The component deduplication mapping must be a JSON object."
    }
    $version = Get-JsonPropertyValue -Object $Mapping -Name "version"
    if ($version -isnot [long] -and $version -isnot [int]) {
        throw "The component deduplication mapping version must be an integer."
    }
    if ([long]$version -ne 1) {
        throw "Unsupported component deduplication mapping version '$version'."
    }
    $groups = Get-JsonPropertyValue -Object $Mapping -Name "groups"
    if (-not (Test-JsonArrayValue -Value $groups)) {
        throw "The component deduplication mapping groups property must be an array."
    }

    # These sets enforce mapping-wide uniqueness while groupMembers below catches
    # duplicates within one group's ordered member list.
    $ids = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $canonicalNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $allMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($group in $groups) {
        if (-not (Test-JsonObjectValue -Value $group)) {
            throw "Every component deduplication mapping group must be an object."
        }
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $canonicalName = Get-JsonPropertyValue -Object $group -Name "canonicalName"
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        $fingerprint = Get-JsonPropertyValue -Object $group -Name "fingerprint"
        $members = Get-JsonPropertyValue -Object $group -Name "members"
        foreach ($value in @($id, $canonicalName, $representative, $fingerprint)) {
            if ($value -isnot [string] -or [string]::IsNullOrWhiteSpace($value)) {
                throw "Every mapping group must have nonempty string id, canonicalName, representative, and fingerprint values."
            }
        }
        if (-not $ids.Add($id)) {
            throw "Component deduplication mapping id '$id' is duplicated."
        }
        if (-not $canonicalNames.Add($canonicalName)) {
            throw "Component deduplication canonical name '$canonicalName' is duplicated."
        }
        if (-not (Test-JsonArrayValue -Value $members) -or $members.Count -lt 2) {
            throw "Mapping group '$id' must contain at least two members."
        }
        $groupMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        foreach ($member in $members) {
            if ($member -isnot [string] -or [string]::IsNullOrWhiteSpace($member)) {
                throw "Every member of mapping group '$id' must be a nonempty string."
            }
            if (-not $groupMembers.Add($member)) {
                throw "Mapping group '$id' contains duplicate member '$member'."
            }
            if (-not $allMembers.Add($member)) {
                throw "Component '$member' belongs to more than one mapping group."
            }
        }
        if (-not $groupMembers.Contains($representative)) {
            throw "Representative '$representative' is not a member of mapping group '$id'."
        }
        if (-not $fingerprint.StartsWith("sha256:", [System.StringComparison]::Ordinal) -or
            $fingerprint.Length -ne 71) {
            throw "Mapping group '$id' has invalid fingerprint '$fingerprint'."
        }
    }
}

function Get-OpenApiComponentNameSegmentCount {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if ([string]::IsNullOrWhiteSpace($Name)) {
        throw "An OpenAPI component name must be a nonempty string."
    }

    $segmentCount = 1
    foreach ($character in $Name.ToCharArray()) {
        if ($character -eq [char]'_') {
            $segmentCount++
        }
    }
    return $segmentCount
}

function Get-OpenApiComponentDeduplicationRepresentative {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Members
    )

    if ($Members.Count -eq 0) {
        throw "A duplicate component group must contain at least one member."
    }

    $representative = $null
    $fewestSegments = [int]::MaxValue
    foreach ($member in $Members) {
        if ($member -isnot [string] -or [string]::IsNullOrWhiteSpace($member)) {
            throw "Every duplicate component group member must be a nonempty string."
        }
        $segmentCount = Get-OpenApiComponentNameSegmentCount -Name $member
        if ($segmentCount -lt $fewestSegments) {
            $representative = $member
            $fewestSegments = $segmentCount
        }
    }
    return $representative
}

# Reconciles current analysis with persisted review state. Historical groups are
# matched by their members rather than fingerprint because legitimate whole-group
# evolution changes fingerprints. The conservative rules are:
#   * historical ids, canonical names, representatives, order, and members persist;
#   * newly equivalent members append in current source order;
#   * whole-group fingerprint evolution is accepted and reported;
#   * missing/diverged/split/merged/cycle-dependent history fails;
#   * wholly new groups append after all historical groups.
function Update-OpenApiComponentDeduplicationMapping {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Analysis,

        [Parameter()]
        [AllowNull()]
        [object]$Mapping
    )

    if ($null -eq $Mapping) {
        $Mapping = New-OpenApiComponentDeduplicationMapping
    }
    Assert-OpenApiComponentDeduplicationMapping -Mapping $Mapping

    # Every eligible component belongs to at most one current structural group,
    # making member lookup the bridge from historical identity to current shape.
    $currentGroupByMember = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($currentGroup in $Analysis.DuplicateGroups) {
        foreach ($member in $currentGroup.Members) {
            $currentGroupByMember[$member] = $currentGroup
        }
    }

    $sourceNames = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$Analysis.ComponentNames,
        [System.StringComparer]::Ordinal
    )
    $mappedMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $claimedCurrentGroups = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    $updatedGroups = [System.Collections.Generic.List[object]]::new()
    $statuses = [System.Collections.Generic.List[object]]::new()

    # Reconcile persisted groups first and in persisted order. This is what keeps
    # stable decisions intact when a newly discovered duplicate appears earlier
    # than the old representative in source order.
    $mappingGroups = Get-JsonPropertyValue -Object $Mapping -Name "groups"
    foreach ($group in $mappingGroups) {
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $canonicalName = Get-JsonPropertyValue -Object $group -Name "canonicalName"
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        $oldFingerprint = Get-JsonPropertyValue -Object $group -Name "fingerprint"
        [object[]]$oldMembers = [object[]](Get-JsonPropertyValue -Object $group -Name "members")

        foreach ($member in $oldMembers) {
            if (-not $sourceNames.Contains($member)) {
                throw "Mapped component '$member' in group '$id' is missing from the componentized source."
            }
            if ($Analysis.CycleDependentComponents -ccontains $member) {
                throw "Mapped component '$member' in group '$id' is now cycle-dependent and cannot be deduplicated."
            }
        }
        if (-not $sourceNames.Contains($representative)) {
            throw "Representative '$representative' in group '$id' is missing from the componentized source."
        }

        # All historical members must still land in one current duplicate group.
        # No group means divergence; different groups mean the history split.
        $currentGroup = $null
        foreach ($member in $oldMembers) {
            if (-not $currentGroupByMember.ContainsKey($member)) {
                throw "Mapped component '$member' in group '$id' no longer belongs to a duplicate group."
            }
            $memberGroup = $currentGroupByMember[$member]
            if ($null -eq $currentGroup) {
                $currentGroup = $memberGroup
            }
            elseif (-not [string]::Equals(
                $currentGroup.GroupKey,
                $memberGroup.GroupKey,
                [System.StringComparison]::Ordinal
            )) {
                throw "Mapped group '$id' split because its historical members are no longer equivalent."
            }
        }

        # Two historical groups resolving to one current group is a merge. It
        # requires human review because two persisted canonical identities now
        # compete for one structural group.
        if ($claimedCurrentGroups.ContainsKey($currentGroup.GroupKey)) {
            throw "Mapped groups '$($claimedCurrentGroups[$currentGroup.GroupKey])' and '$id' merged into one current duplicate group."
        }
        $claimedCurrentGroups[$currentGroup.GroupKey] = $id

        # Preserve historical member order and append only genuinely new members.
        # Example: [LeafA, LeafB] plus an earlier source member BeforeLeaf becomes
        # [LeafA, LeafB, BeforeLeaf], retaining longitudinal diff stability.
        $memberList = [System.Collections.Generic.List[string]]::new()
        foreach ($member in $oldMembers) {
            $memberList.Add($member)
            $null = $mappedMembers.Add($member)
        }
        $addedMembers = [System.Collections.Generic.List[string]]::new()
        foreach ($member in $currentGroup.Members) {
            if ($memberList -ccontains $member) {
                continue
            }
            if ($mappedMembers.Contains($member)) {
                throw "Component '$member' is claimed by multiple mapping groups."
            }
            $memberList.Add($member)
            $addedMembers.Add($member)
            $null = $mappedMembers.Add($member)
        }

        # A canonical name may be a member name or a fresh user-selected name. It
        # may not overwrite an unrelated source component at apply time.
        if ($sourceNames.Contains($canonicalName) -and -not ($memberList -ccontains $canonicalName)) {
            throw "Canonical name '$canonicalName' for group '$id' collides with an unrelated source component."
        }

        # Extension takes precedence over fingerprint evolution in the summary;
        # either way the current fingerprint is persisted below.
        $status = if ($addedMembers.Count -gt 0) {
            "Extended"
        }
        elseif (-not [string]::Equals($oldFingerprint, $currentGroup.Fingerprint, [System.StringComparison]::Ordinal)) {
            "FingerprintUpdated"
        }
        else {
            "Unchanged"
        }
        $updatedGroups.Add([ordered]@{
            id = $id
            canonicalName = $canonicalName
            representative = $representative
            fingerprint = $currentGroup.Fingerprint
            members = [object[]]$memberList.ToArray()
        })
        $statuses.Add([PSCustomObject]@{
            Id = $id
            Status = $status
            CanonicalName = $canonicalName
            AddedMembers = [object[]]$addedMembers.ToArray()
        })
    }

    # Any current group with no historical overlap is a new review decision. Its
    # first shortest member supplies the initial id, representative, and canonical
    # name; future runs preserve those defaults unless edited by a user.
    foreach ($currentGroup in $Analysis.DuplicateGroups) {
        $overlap = @($currentGroup.Members | Where-Object { $mappedMembers.Contains($_) })
        if ($overlap.Count -gt 0) {
            continue
        }
        $representative = Get-OpenApiComponentDeduplicationRepresentative -Members $currentGroup.Members
        $updatedGroups.Add([ordered]@{
            id = $representative
            canonicalName = $representative
            representative = $representative
            fingerprint = $currentGroup.Fingerprint
            members = [object[]]$currentGroup.Members
        })
        foreach ($member in $currentGroup.Members) {
            $null = $mappedMembers.Add($member)
        }
        $statuses.Add([PSCustomObject]@{
            Id = $representative
            Status = "New"
            CanonicalName = $representative
            AddedMembers = [object[]]$currentGroup.Members
        })
    }

    $updatedMapping = [ordered]@{
        version = [long]1
        groups = [object[]]$updatedGroups.ToArray()
    }
    Assert-OpenApiComponentDeduplicationMapping -Mapping $updatedMapping

    # Validate the assembled document again because newly discovered defaults
    # can collide with persisted ids/names even when the input mapping was valid.
    $canonicalOwners = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    foreach ($group in $updatedMapping.groups) {
        if ($canonicalOwners.ContainsKey($group.canonicalName)) {
            throw "Canonical name '$($group.canonicalName)' is shared by mapping groups '$($canonicalOwners[$group.canonicalName])' and '$($group.id)'."
        }
        $canonicalOwners[$group.canonicalName] = $group.id
    }

    # Structural JSON equality determines Changed. The file-oriented updater can
    # therefore skip writing and preserve exact bytes on an idempotent second run.
    return [PSCustomObject]@{
        Mapping = $updatedMapping
        Changed = -not (Test-JsonValuesEqual -Left $Mapping -Right $updatedMapping)
        Statuses = [object[]]$statuses.ToArray()
        NewGroupCount = @($statuses | Where-Object Status -eq "New").Count
        ExtendedGroupCount = @($statuses | Where-Object Status -eq "Extended").Count
        FingerprintUpdatedGroupCount = @($statuses | Where-Object Status -eq "FingerprintUpdated").Count
        UnchangedGroupCount = @($statuses | Where-Object Status -eq "Unchanged").Count
        SkippedCycleComponents = [object[]]$Analysis.CycleDependentComponents
    }
}
