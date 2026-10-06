#Requires -Version 7.0

# Private graph, fingerprint, and component-map helpers shared by the component
# processing scripts. The public scripts dot-source the document helpers first.
#
# The implementation has five stages:
#   1. discover component-to-component pointer edges;
#   2. classify cycles and every component that depends on one;
#   3. recursively fingerprint the remaining acyclic component trees;
#   4. group trees with identical canonical content;
#   5. validate policy, generate comprehensive defaults, and resolve outputs.
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

# Builds the complete analysis consumed by component-map generation and apply.
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
    $identityGroupList = [System.Collections.Generic.List[object]]::new()
    $duplicateGroups = [System.Collections.Generic.List[object]]::new()
    foreach ($componentName in $componentNames) {
        if ($cycleDependent.Contains($componentName)) {
            continue
        }
        $identity = $state.Identities[$componentName]
        $groupKey = "$($identity.Fingerprint)`n$($identity.CanonicalText)"
        $members = $identityGroups[$groupKey]
        if (-not [string]::Equals($members[0], $componentName, [System.StringComparison]::Ordinal)) {
            continue
        }
        $group = [PSCustomObject]@{
            Fingerprint = $identity.Fingerprint
            CanonicalText = $identity.CanonicalText
            GroupKey = $groupKey
            Members = [object[]]$members.ToArray()
        }
        $identityGroupList.Add($group)
        if ($members.Count -ge 2) {
            $duplicateGroups.Add($group)
        }
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
        IdentityGroups = [object[]]$identityGroupList.ToArray()
        DuplicateGroups = [object[]]$duplicateGroups.ToArray()
        DuplicateGroupCount = $duplicateGroups.Count
        RemovableComponentCount = $removableCount
        CycleComponents = [object[]]@($componentNames | Where-Object { $cycleNodes.Contains($_) })
        CycleDependentComponents = [object[]]@($componentNames | Where-Object { $cycleDependent.Contains($_) })
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

function New-OpenApiComponentPolicyMap {
    return [ordered]@{
        version = [long]1
        groups = [object[]]@()
    }
}

function New-OpenApiComponentDefaultMap {
    return [ordered]@{
        version = [long]1
        groups = [object[]]@()
    }
}

function Test-OpenApiComponentFingerprint {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    return $Value -is [string] -and [System.Text.RegularExpressions.Regex]::IsMatch(
        $Value,
        "\Asha256:[0-9a-f]{64}\z",
        [System.Text.RegularExpressions.RegexOptions]::CultureInvariant
    )
}

function Assert-OpenApiComponentMapRoot {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Map,

        [Parameter(Mandatory = $true)]
        [string]$Description
    )

    if (-not (Test-JsonObjectValue -Value $Map)) {
        throw "The $Description must be a JSON object."
    }
    $allowedProperties = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]@("version", "groups"),
        [System.StringComparer]::Ordinal
    )
    foreach ($property in (Get-JsonProperties -Value $Map)) {
        if (-not $allowedProperties.Contains($property.Name)) {
            throw "The $Description contains unsupported property '$($property.Name)'."
        }
    }
    $version = Get-JsonPropertyValue -Object $Map -Name "version"
    if (($version -isnot [long] -and $version -isnot [int]) -or [long]$version -ne 1) {
        throw "The $Description must have integer version 1."
    }
    $groups = Get-JsonPropertyValue -Object $Map -Name "groups"
    if (-not (Test-JsonArrayValue -Value $groups)) {
        throw "The $Description groups property must be an array."
    }
}

function Assert-OpenApiDuplicateGroupShape {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Group,

        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter()]
        [switch]$AllowPartial
    )

    if (-not (Test-JsonObjectValue -Value $Group)) {
        throw "$Path must be an object."
    }
    $allowedNames = [System.Collections.Generic.List[string]]::new()
    foreach ($name in @("id", "type", "representative", "fingerprint", "members")) {
        $allowedNames.Add($name)
    }
    if ($AllowPartial) {
        $allowedNames.Add("is_partial")
    }
    $allowedProperties = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$allowedNames.ToArray(),
        [System.StringComparer]::Ordinal
    )
    foreach ($property in (Get-JsonProperties -Value $Group)) {
        if (-not $allowedProperties.Contains($property.Name)) {
            throw "$Path contains unsupported property '$($property.Name)'."
        }
    }
    foreach ($requiredName in @("id", "type", "representative", "fingerprint", "members")) {
        if (-not (Test-JsonProperty -Object $Group -Name $requiredName)) {
            throw "$Path is missing required property '$requiredName'."
        }
    }
    $id = Get-JsonPropertyValue -Object $Group -Name "id"
    $type = Get-JsonPropertyValue -Object $Group -Name "type"
    $representative = Get-JsonPropertyValue -Object $Group -Name "representative"
    $fingerprint = Get-JsonPropertyValue -Object $Group -Name "fingerprint"
    $members = Get-JsonPropertyValue -Object $Group -Name "members"
    if ($id -isnot [string] -or [string]::IsNullOrWhiteSpace($id)) {
        throw "$Path must have a nonempty string id."
    }
    if (-not [string]::Equals($type, "duplicate", [System.StringComparison]::Ordinal)) {
        throw "$Path must have type 'duplicate'."
    }
    if ($representative -isnot [string] -or [string]::IsNullOrWhiteSpace($representative)) {
        throw "$Path must have a nonempty string representative."
    }
    if (-not (Test-OpenApiComponentFingerprint -Value $fingerprint)) {
        throw "$Path has invalid fingerprint '$fingerprint'."
    }
    if (-not (Test-JsonArrayValue -Value $members) -or $members.Count -lt 1) {
        throw "$Path must contain at least one member."
    }
    $memberSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($member in $members) {
        if ($member -isnot [string] -or [string]::IsNullOrWhiteSpace($member)) {
            throw "Every member of $Path must be a nonempty string."
        }
        if (-not $memberSet.Add($member)) {
            throw "$Path contains duplicate member '$member'."
        }
    }
    if (-not $memberSet.Contains($representative)) {
        throw "Representative '$representative' is not a member of $Path."
    }
    if (Test-JsonProperty -Object $Group -Name "is_partial") {
        $isPartial = Get-JsonPropertyValue -Object $Group -Name "is_partial"
        if (-not $AllowPartial -or $isPartial -isnot [bool] -or -not $isPartial) {
            throw "$Path may specify is_partial only as true in a default map."
        }
    }
}

function Assert-OpenApiComponentPolicyMap {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Map
    )

    Assert-OpenApiComponentMapRoot -Map $Map -Description "component policy map"
    $ids = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $allMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    [object[]]$groups = [object[]](Get-JsonPropertyValue -Object $Map -Name "groups")
    for ($groupIndex = 0; $groupIndex -lt $groups.Count; $groupIndex++) {
        $group = $groups[$groupIndex]
        $path = "Policy group[$groupIndex]"
        if (-not (Test-JsonObjectValue -Value $group)) {
            throw "$path must be an object."
        }
        $type = Get-JsonPropertyValue -Object $group -Name "type"
        if ([string]::Equals($type, "duplicate", [System.StringComparison]::Ordinal)) {
            Assert-OpenApiDuplicateGroupShape -Group $group -Path $path
            $nestedGroups = @()
        }
        elseif ([string]::Equals($type, "override", [System.StringComparison]::Ordinal)) {
            $allowedProperties = [System.Collections.Generic.HashSet[string]]::new(
                [string[]]@("id", "type", "representative", "fingerprint", "members", "overrides"),
                [System.StringComparer]::Ordinal
            )
            foreach ($property in (Get-JsonProperties -Value $group)) {
                if (-not $allowedProperties.Contains($property.Name)) {
                    throw "$path contains unsupported property '$($property.Name)'."
                }
            }
            foreach ($requiredName in @("id", "type", "representative", "fingerprint", "members", "overrides")) {
                if (-not (Test-JsonProperty -Object $group -Name $requiredName)) {
                    throw "$path is missing required property '$requiredName'."
                }
            }
            $id = Get-JsonPropertyValue -Object $group -Name "id"
            $representative = Get-JsonPropertyValue -Object $group -Name "representative"
            $fingerprint = Get-JsonPropertyValue -Object $group -Name "fingerprint"
            $members = Get-JsonPropertyValue -Object $group -Name "members"
            $nestedGroups = Get-JsonPropertyValue -Object $group -Name "overrides"
            if ($id -isnot [string] -or [string]::IsNullOrWhiteSpace($id)) {
                throw "$path must have a nonempty string id."
            }
            if ($representative -isnot [string] -or [string]::IsNullOrWhiteSpace($representative)) {
                throw "$path must have a nonempty string representative."
            }
            if (-not (Test-OpenApiComponentFingerprint -Value $fingerprint)) {
                throw "$path has invalid fingerprint '$fingerprint'."
            }
            if (-not (Test-JsonArrayValue -Value $members) -or $members.Count -lt 1) {
                throw "$path must contain at least one member."
            }
            if (-not (Test-JsonArrayValue -Value $nestedGroups) -or $nestedGroups.Count -lt 1) {
                throw "$path must contain at least one overrides group."
            }
            $memberSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
            foreach ($member in $members) {
                if ($member -isnot [string] -or [string]::IsNullOrWhiteSpace($member)) {
                    throw "Every member of $path must be a nonempty string."
                }
                if (-not $memberSet.Add($member)) {
                    throw "$path contains duplicate member '$member'."
                }
            }
            if (-not $memberSet.Contains($representative)) {
                throw "Representative '$representative' is not a member of $path."
            }
            for ($nestedIndex = 0; $nestedIndex -lt $nestedGroups.Count; $nestedIndex++) {
                $nested = $nestedGroups[$nestedIndex]
                $nestedPath = "$path overrides[$nestedIndex]"
                Assert-OpenApiDuplicateGroupShape -Group $nested -Path $nestedPath
                $nestedFingerprint = Get-JsonPropertyValue -Object $nested -Name "fingerprint"
                if ([string]::Equals($fingerprint, $nestedFingerprint, [System.StringComparison]::Ordinal)) {
                    throw "$nestedPath fingerprint must differ from its parent override fingerprint."
                }
            }
        }
        else {
            throw "$path must have type 'duplicate' or 'override'."
        }

        $groupId = Get-JsonPropertyValue -Object $group -Name "id"
        if (-not $ids.Add($groupId)) {
            throw "Component policy id '$groupId' is duplicated."
        }
        foreach ($member in [object[]](Get-JsonPropertyValue -Object $group -Name "members")) {
            if (-not $allMembers.Add($member)) {
                throw "Component '$member' belongs to more than one policy group."
            }
        }
        for ($nestedIndex = 0; $nestedIndex -lt @($nestedGroups).Count; $nestedIndex++) {
            $nested = $nestedGroups[$nestedIndex]
            $nestedId = Get-JsonPropertyValue -Object $nested -Name "id"
            if (-not $ids.Add($nestedId)) {
                throw "Component policy id '$nestedId' is duplicated."
            }
            foreach ($member in [object[]](Get-JsonPropertyValue -Object $nested -Name "members")) {
                if (-not $allMembers.Add($member)) {
                    throw "Component '$member' belongs to more than one policy group."
                }
            }
        }
    }
}

function Assert-OpenApiComponentDefaultMap {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Map
    )

    Assert-OpenApiComponentMapRoot -Map $Map -Description "component default map"
    $ids = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $allMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    [object[]]$groups = [object[]](Get-JsonPropertyValue -Object $Map -Name "groups")
    for ($groupIndex = 0; $groupIndex -lt $groups.Count; $groupIndex++) {
        $group = $groups[$groupIndex]
        $path = "Default group[$groupIndex]"
        Assert-OpenApiDuplicateGroupShape -Group $group -Path $path -AllowPartial
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        if (-not $ids.Add($id)) {
            throw "Component default id '$id' is duplicated."
        }
        foreach ($member in [object[]](Get-JsonPropertyValue -Object $group -Name "members")) {
            if (-not $allMembers.Add($member)) {
                throw "Component '$member' belongs to more than one default group."
            }
        }
    }
}

function Read-OpenApiComponentPolicyMap {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    try {
        $map = Get-Content -LiteralPath $Path -Raw -Encoding utf8 |
            ConvertFrom-Json -AsHashtable -Depth 100
    }
    catch {
        throw "Could not parse component policy map JSON at '$Path': $($_.Exception.Message)"
    }
    Assert-OpenApiComponentPolicyMap -Map $map
    return $map
}

function Read-OpenApiComponentDefaultMap {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    try {
        $map = Get-Content -LiteralPath $Path -Raw -Encoding utf8 |
            ConvertFrom-Json -AsHashtable -Depth 100
    }
    catch {
        throw "Could not parse component default map JSON at '$Path': $($_.Exception.Message)"
    }
    Assert-OpenApiComponentDefaultMap -Map $map
    return $map
}

function Get-OpenApiComponentPolicyState {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter()]
        [AllowNull()]
        [object]$PolicyMap
    )

    if ($null -eq $PolicyMap) {
        $PolicyMap = New-OpenApiComponentPolicyMap
    }
    Assert-OpenApiComponentPolicyMap -Map $PolicyMap

    $rawAnalysis = Get-OpenApiComponentDeduplicationAnalysis -Document $Document
    if ($rawAnalysis.CycleDependentComponents.Count -gt 0) {
        throw "Component map generation does not support cycle-dependent components: $($rawAnalysis.CycleDependentComponents -join ', ')."
    }
    $sourceNames = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$rawAnalysis.ComponentNames,
        [System.StringComparer]::Ordinal
    )
    $claimedMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $policyIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $targets = [System.Collections.Generic.List[object]]::new()
    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    [object[]]$groups = [object[]](Get-JsonPropertyValue -Object $PolicyMap -Name "groups")

    for ($groupIndex = 0; $groupIndex -lt $groups.Count; $groupIndex++) {
        $group = $groups[$groupIndex]
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $type = Get-JsonPropertyValue -Object $group -Name "type"
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        [object[]]$members = [object[]](Get-JsonPropertyValue -Object $group -Name "members")
        $null = $policyIds.Add($id)
        foreach ($member in $members) {
            if (-not $sourceNames.Contains($member)) {
                throw "Policy component '$member' in group '$id' is missing from the componentized source."
            }
            $null = $claimedMembers.Add($member)
        }
        $outputMembers = [System.Collections.Generic.List[string]]::new()
        foreach ($member in $members) {
            $outputMembers.Add($member)
        }

        $targets.Add([PSCustomObject]@{
            Id = $id
            Type = if ([string]::Equals($type, "override", [System.StringComparison]::Ordinal)) { "OverridePrimary" } else { "Duplicate" }
            Path = $id
            Fingerprint = Get-JsonPropertyValue -Object $group -Name "fingerprint"
            Representative = $representative
            Members = $members
            OutputId = $id
            OutputRepresentative = $representative
            ParentGroup = $group
        })

        [object[]]$nestedGroups = if ([string]::Equals($type, "override", [System.StringComparison]::Ordinal)) {
            [object[]](Get-JsonPropertyValue -Object $group -Name "overrides")
        }
        else {
            [object[]]@()
        }
        foreach ($nested in $nestedGroups) {
            $nestedId = Get-JsonPropertyValue -Object $nested -Name "id"
            [object[]]$nestedMembers = [object[]](Get-JsonPropertyValue -Object $nested -Name "members")
            $null = $policyIds.Add($nestedId)
            foreach ($member in $nestedMembers) {
                if (-not $sourceNames.Contains($member)) {
                    throw "Policy component '$member' in nested group '$nestedId' is missing from the componentized source."
                }
                $null = $claimedMembers.Add($member)
                $outputMembers.Add($member)
            }
            $targets.Add([PSCustomObject]@{
                Id = $nestedId
                Type = "OverrideNested"
                Path = "$id/overrides/$nestedId"
                Fingerprint = Get-JsonPropertyValue -Object $nested -Name "fingerprint"
                Representative = Get-JsonPropertyValue -Object $nested -Name "representative"
                Members = $nestedMembers
                OutputId = $id
                OutputRepresentative = $representative
                ParentGroup = $group
            })
        }

        if ($sourceNames.Contains($id) -and -not ($outputMembers -ccontains $id)) {
            throw "Policy output id '$id' collides with an unrelated source component."
        }
        $destination = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $representative)"
        foreach ($member in $outputMembers) {
            $source = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $member)"
            if (-not [string]::Equals($source, $destination, [System.StringComparison]::Ordinal)) {
                Add-OpenApiPointerRelocation `
                    -Relocations $relocations `
                    -SourcePrefix $source `
                    -DestinationPrefix $destination
            }
        }
    }

    $normalizedDocument = if ($relocations.Count -gt 0) {
        ConvertTo-RelocatedOpenApiPointers -Value $Document -Relocations $relocations
    }
    else {
        Copy-JsonValue -Value $Document
    }
    $analysis = Get-OpenApiComponentDeduplicationAnalysis -Document $normalizedDocument
    if ($analysis.CycleDependentComponents.Count -gt 0) {
        throw "Component policy normalization creates cycle-dependent components: $($analysis.CycleDependentComponents -join ', ')."
    }

    foreach ($target in $targets) {
        $representativeIdentity = $analysis.Identities[$target.Representative]
        if (-not [string]::Equals(
            $target.Fingerprint,
            $representativeIdentity.Fingerprint,
            [System.StringComparison]::Ordinal
        )) {
            throw "Policy target '$($target.Path)' fingerprint '$($target.Fingerprint)' does not match current fingerprint '$($representativeIdentity.Fingerprint)'."
        }
        foreach ($member in $target.Members) {
            $identity = $analysis.Identities[$member]
            if (-not [string]::Equals(
                $identity.Fingerprint,
                $target.Fingerprint,
                [System.StringComparison]::Ordinal
            ) -or -not [string]::Equals(
                $identity.CanonicalText,
                $representativeIdentity.CanonicalText,
                [System.StringComparison]::Ordinal
            )) {
                throw "Policy component '$member' does not match fingerprint '$($target.Fingerprint)' for target '$($target.Path)'."
            }
        }
    }

    return [PSCustomObject]@{
        PolicyMap = $PolicyMap
        RawAnalysis = $rawAnalysis
        NormalizedDocument = $normalizedDocument
        Analysis = $analysis
        Targets = [object[]]$targets.ToArray()
        ClaimedMembers = $claimedMembers
        PolicyIds = $policyIds
    }
}

function Get-OpenApiComponentLogicalGroups {
    param(
        [Parameter(Mandatory = $true)]
        [object]$PolicyMap,

        [Parameter(Mandatory = $true)]
        [object]$DefaultMap,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$ComponentNames
    )

    $partialById = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $ordinaryDefaults = [System.Collections.Generic.List[object]]::new()
    foreach ($group in [object[]](Get-JsonPropertyValue -Object $DefaultMap -Name "groups")) {
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        if (Test-JsonProperty -Object $group -Name "is_partial") {
            $partialById[$id] = $group
        }
        else {
            $ordinaryDefaults.Add($group)
        }
    }

    $logicalGroups = [System.Collections.Generic.List[object]]::new()
    $consumedPartials = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($group in [object[]](Get-JsonPropertyValue -Object $PolicyMap -Name "groups")) {
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $members = [System.Collections.Generic.List[string]]::new()
        foreach ($member in [object[]](Get-JsonPropertyValue -Object $group -Name "members")) {
            $members.Add($member)
        }
        if ($partialById.ContainsKey($id)) {
            foreach ($member in [object[]](Get-JsonPropertyValue -Object $partialById[$id] -Name "members")) {
                $members.Add($member)
            }
            $null = $consumedPartials.Add($id)
        }
        if ([string]::Equals(
            (Get-JsonPropertyValue -Object $group -Name "type"),
            "override",
            [System.StringComparison]::Ordinal
        )) {
            foreach ($nested in [object[]](Get-JsonPropertyValue -Object $group -Name "overrides")) {
                $nestedId = Get-JsonPropertyValue -Object $nested -Name "id"
                foreach ($member in [object[]](Get-JsonPropertyValue -Object $nested -Name "members")) {
                    $members.Add($member)
                }
                if ($partialById.ContainsKey($nestedId)) {
                    foreach ($member in [object[]](Get-JsonPropertyValue -Object $partialById[$nestedId] -Name "members")) {
                        $members.Add($member)
                    }
                    $null = $consumedPartials.Add($nestedId)
                }
            }
        }
        $logicalGroups.Add([PSCustomObject]@{
            Id = $id
            Type = Get-JsonPropertyValue -Object $group -Name "type"
            Representative = Get-JsonPropertyValue -Object $group -Name "representative"
            Members = [object[]]$members.ToArray()
        })
    }
    foreach ($group in $ordinaryDefaults) {
        $logicalGroups.Add([PSCustomObject]@{
            Id = Get-JsonPropertyValue -Object $group -Name "id"
            Type = "duplicate"
            Representative = Get-JsonPropertyValue -Object $group -Name "representative"
            Members = [object[]](Get-JsonPropertyValue -Object $group -Name "members")
        })
    }
    foreach ($partialId in $partialById.Keys) {
        if (-not $consumedPartials.Contains($partialId)) {
            throw "Partial default group '$partialId' does not match a policy target."
        }
    }

    $sourceNames = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$ComponentNames,
        [System.StringComparer]::Ordinal
    )
    $logicalIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $claimedMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($group in $logicalGroups) {
        if (-not $logicalIds.Add($group.Id)) {
            throw "Logical component id '$($group.Id)' is duplicated."
        }
        foreach ($member in $group.Members) {
            if (-not $sourceNames.Contains($member)) {
                throw "Logical group '$($group.Id)' contains missing component '$member'."
            }
            if (-not $claimedMembers.Add($member)) {
                throw "Component '$member' belongs to more than one logical output group."
            }
        }
        if (-not ($group.Members -ccontains $group.Representative)) {
            throw "Representative '$($group.Representative)' is not a member of logical group '$($group.Id)'."
        }
        if ($sourceNames.Contains($group.Id) -and -not ($group.Members -ccontains $group.Id)) {
            throw "Logical output id '$($group.Id)' collides with an unrelated source component."
        }
    }
    foreach ($componentName in $ComponentNames) {
        if (-not $claimedMembers.Contains($componentName)) {
            throw "Component '$componentName' is not covered by the policy and default maps."
        }
    }
    return [object[]]$logicalGroups.ToArray()
}

function New-OpenApiComponentDefaultMapPlan {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter()]
        [AllowNull()]
        [object]$PolicyMap
    )

    $policyState = Get-OpenApiComponentPolicyState -Document $Document -PolicyMap $PolicyMap
    $targetsByGroupKey = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($target in $policyState.Targets) {
        $identity = $policyState.Analysis.Identities[$target.Representative]
        $groupKey = "$($identity.Fingerprint)`n$($identity.CanonicalText)"
        if (-not $targetsByGroupKey.ContainsKey($groupKey)) {
            $targetsByGroupKey[$groupKey] = [System.Collections.Generic.List[object]]::new()
        }
        $targetsByGroupKey[$groupKey].Add($target)
    }

    $defaultGroups = [System.Collections.Generic.List[object]]::new()
    $ordinaryCount = 0
    $singletonCount = 0
    $partialCount = 0
    foreach ($identityGroup in $policyState.Analysis.IdentityGroups) {
        [object[]]$members = @($identityGroup.Members | Where-Object {
            -not $policyState.ClaimedMembers.Contains($_)
        })
        if ($members.Count -eq 0) {
            continue
        }
        [object[]]$candidates = @()
        if ($targetsByGroupKey.ContainsKey($identityGroup.GroupKey)) {
            $candidates = [object[]]$targetsByGroupKey[$identityGroup.GroupKey].ToArray()
        }
        if ($candidates.Count -gt 1) {
            $candidatePaths = @($candidates | ForEach-Object Path)
            throw "Components '$($members -join ', ')' match multiple policy targets: $($candidatePaths -join ', '). Promote them manually to one policy target."
        }

        $representative = Get-OpenApiComponentDeduplicationRepresentative -Members $members
        if ($candidates.Count -eq 1) {
            $target = $candidates[0]
            $defaultGroups.Add([ordered]@{
                id = $target.Id
                type = "duplicate"
                representative = $representative
                fingerprint = $identityGroup.Fingerprint
                members = [object[]]$members
                is_partial = $true
            })
            $partialCount++
        }
        else {
            $defaultGroups.Add([ordered]@{
                id = $representative
                type = "duplicate"
                representative = $representative
                fingerprint = $identityGroup.Fingerprint
                members = [object[]]$members
            })
            $ordinaryCount++
            if ($members.Count -eq 1) {
                $singletonCount++
            }
        }
    }

    $defaultMap = [ordered]@{
        version = [long]1
        groups = [object[]]$defaultGroups.ToArray()
    }
    Assert-OpenApiComponentDefaultMap -Map $defaultMap

    $defaultIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $defaultMembers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($group in $defaultMap.groups) {
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $isPartial = Test-JsonProperty -Object $group -Name "is_partial"
        if ($policyState.PolicyIds.Contains($id) -and -not $isPartial) {
            throw "Default group id '$id' collides with a policy id."
        }
        $null = $defaultIds.Add($id)
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        $identity = $policyState.Analysis.Identities[$representative]
        $fingerprint = Get-JsonPropertyValue -Object $group -Name "fingerprint"
        foreach ($member in [object[]](Get-JsonPropertyValue -Object $group -Name "members")) {
            if ($policyState.ClaimedMembers.Contains($member)) {
                throw "Default component '$member' is already claimed by policy."
            }
            $null = $defaultMembers.Add($member)
            $memberIdentity = $policyState.Analysis.Identities[$member]
            if (-not [string]::Equals($memberIdentity.Fingerprint, $fingerprint, [System.StringComparison]::Ordinal) -or
                -not [string]::Equals($memberIdentity.CanonicalText, $identity.CanonicalText, [System.StringComparison]::Ordinal)) {
                throw "Default component '$member' does not match group fingerprint '$fingerprint'."
            }
        }
    }
    foreach ($componentName in $policyState.Analysis.ComponentNames) {
        if (-not $policyState.ClaimedMembers.Contains($componentName) -and
            -not $defaultMembers.Contains($componentName)) {
            throw "Component '$componentName' was not emitted in the default map."
        }
    }

    [object[]]$logicalGroups = @(Get-OpenApiComponentLogicalGroups `
        -PolicyMap $policyState.PolicyMap `
        -DefaultMap $defaultMap `
        -ComponentNames $policyState.Analysis.ComponentNames)
    $removableCount = 0
    foreach ($group in $logicalGroups) {
        $removableCount += $group.Members.Count - 1
    }
    return [PSCustomObject]@{
        PolicyMap = $policyState.PolicyMap
        DefaultMap = $defaultMap
        LogicalGroups = $logicalGroups
        RawAnalysis = $policyState.RawAnalysis
        NormalizedAnalysis = $policyState.Analysis
        ComponentCount = $policyState.Analysis.ComponentNames.Count
        PolicyComponentCount = $policyState.ClaimedMembers.Count
        OrdinaryDefaultGroupCount = $ordinaryCount
        SingletonDefaultGroupCount = $singletonCount
        PartialDefaultGroupCount = $partialCount
        LogicalGroupCount = $logicalGroups.Count
        RemovableComponentCount = $removableCount
    }
}

function Resolve-OpenApiComponentMapPlan {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter()]
        [AllowNull()]
        [object]$PolicyMap,

        [Parameter(Mandatory = $true)]
        [object]$DefaultMap
    )

    Assert-OpenApiComponentDefaultMap -Map $DefaultMap
    $plan = New-OpenApiComponentDefaultMapPlan -Document $Document -PolicyMap $PolicyMap
    if (-not (Test-JsonValuesEqual -Left $DefaultMap -Right $plan.DefaultMap)) {
        throw "The component default map is stale or was edited. Regenerate it before applying component maps."
    }
    return $plan
}
