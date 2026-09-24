<#
.SYNOPSIS
    Applies persisted component-tree deduplication mappings to OpenAPI JSON.

.DESCRIPTION
    Requires a mapping reconciled by Update-OpenApiComponentDeduplicationMap.ps1.
    Equivalent acyclic component trees are collapsed to their persisted canonical
    names, all local reference-valued pointers are relocated, and explicit
    discriminator aliases preserve pre-deduplication component-name identities.

    The mapping is authoritative and is never modified by this command. New,
    missing, split, merged, or divergent groups fail before output is written.

.PARAMETER InputPath
    The componentized OpenAPI document. Defaults to
    specification/openapi/responses.componentized.openapi.json.

.PARAMETER MappingPath
    The persisted mapping JSON. Defaults to
    specification/openapi/responses.component-deduplication-map.json.

.PARAMETER OutputPath
    The deduplicated OpenAPI destination. Defaults to
    specification/openapi/responses.componentized.deduplicated.openapi.json.

.EXAMPLE
    ./scripts/Update-OpenApiComponentDeduplicationMap.ps1
    ./scripts/Deduplicate-OpenApiComponents.ps1
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$InputPath,

    [Parameter()]
    [string]$MappingPath,

    [Parameter()]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$helperPath = Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1"
$deduplicationHelperPath = Join-Path $PSScriptRoot "_OpenApiComponentDeduplication.Helpers.ps1"
foreach ($path in @($helperPath, $deduplicationHelperPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "The required helper script was not found: $path"
    }
    . $path
}

$script:ComponentDeduplicatorRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultComponentDeduplicatorInputPath = Join-Path $script:ComponentDeduplicatorRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"
$script:DefaultComponentDeduplicatorMappingPath = Join-Path $script:ComponentDeduplicatorRepositoryRoot "specification" "openapi" "responses.component-deduplication-map.json"
$script:DefaultComponentDeduplicatorOutputPath = Join-Path $script:ComponentDeduplicatorRepositoryRoot "specification" "openapi" "responses.componentized.deduplicated.openapi.json"

# Keep path behavior aligned with the componentizer: CLI paths are repository-
# relative by default, while tests and external automation may pass absolutes.
function Resolve-ComponentDeduplicatorRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:ComponentDeduplicatorRepositoryRoot $Path))
}

# Records one discriminator compatibility alias at its final discriminator
# location. Several source discriminators can collapse into the same destination
# when their containing component trees are deduplicated, so plans are merged by
# destination pointer rather than stored per source occurrence.
#
# Reusing an alias is allowed only when it names the same canonical destination.
# Conflicting meanings fail loudly instead of silently changing deserialization.
function Add-OpenApiDiscriminatorAlias {
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$AliasPlans,

        [Parameter(Mandatory = $true)]
        [string]$DiscriminatorPointer,

        [Parameter(Mandatory = $true)]
        [string]$Alias,

        [Parameter(Mandatory = $true)]
        [string]$DestinationPointer
    )

    if (-not $AliasPlans.ContainsKey($DiscriminatorPointer)) {
        $AliasPlans[$DiscriminatorPointer] = [ordered]@{}
    }
    $aliases = $AliasPlans[$DiscriminatorPointer]
    if (Test-JsonProperty -Object $aliases -Name $Alias) {
        $existing = Get-JsonPropertyValue -Object $aliases -Name $Alias
        if (-not [string]::Equals($existing, $DestinationPointer, [System.StringComparison]::Ordinal)) {
            throw "Discriminator alias '$Alias' at '$DiscriminatorPointer' has conflicting destinations '$existing' and '$DestinationPointer'."
        }
        return
    }
    $aliases[$Alias] = $DestinationPointer
}

# Discovers discriminator aliases before refs and component roots are rewritten.
# OpenAPI permits a discriminator without an explicit mapping; many tools then
# use each direct branch component name as an implicit discriminator identity.
# Removing `ParentB` in favor of `ParentA`, for example, could otherwise make the
# old `ParentB` identity disappear. The generated plan preserves both names:
#
#   "ParentA": "#/components/schemas/SharedParent"
#   "ParentB": "#/components/schemas/SharedParent"
#
# Only direct component-root branches participate. A descendant pointer is not a
# component-name discriminator identity and is therefore deliberately ignored.
function Add-OpenApiDiscriminatorAliasPlans {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, string]]$Relocations,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$AliasPlans,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Discriminator alias discovery exceeds the supported depth of $MaximumDepth at '$Pointer'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Add-OpenApiDiscriminatorAliasPlans `
                -Value $Value[$index] `
                -Pointer "$Pointer/$index" `
                -Relocations $Relocations `
                -AliasPlans $AliasPlans `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    $discriminator = Get-JsonPropertyValue -Object $Value -Name "discriminator"
    if (Test-JsonObjectValue -Value $discriminator) {
        # oneOf is the common case; anyOf is included because OpenAPI tooling can
        # pair either composition with discriminator metadata.
        $branches = [System.Collections.Generic.List[object]]::new()
        foreach ($compositionName in @("oneOf", "anyOf")) {
            $composition = Get-JsonPropertyValue -Object $Value -Name $compositionName
            if (-not (Test-JsonArrayValue -Value $composition)) {
                continue
            }
            foreach ($branch in $composition) {
                if (-not (Test-JsonObjectValue -Value $branch)) {
                    continue
                }
                $reference = Get-JsonPropertyValue -Object $branch -Name '$ref'
                if ($reference -isnot [string]) {
                    continue
                }
                $pointerInfo = Get-OpenApiSchemaComponentPointerInfo -Pointer $reference
                if ($null -ne $pointerInfo -and [string]::IsNullOrEmpty($pointerInfo.Suffix)) {
                    $destination = Get-RelocatedOpenApiPointer `
                        -Pointer $pointerInfo.Pointer `
                        -Relocations $Relocations
                    $destinationInfo = Get-OpenApiSchemaComponentPointerInfo -Pointer $destination
                    $branches.Add([PSCustomObject]@{
                        SourceName = $pointerInfo.ComponentName
                        SourcePointer = $pointerInfo.Pointer
                        DestinationName = $destinationInfo.ComponentName
                        DestinationPointer = $destination
                    })
                }
            }
        }

        # A discriminator needs aliases when either its containing object moves
        # (duplicate parent trees) or any direct branch target moves (duplicate
        # children). Unaffected discriminators are left byte-for-byte alone.
        $destinationObjectPointer = Get-RelocatedOpenApiPointer `
            -Pointer $Pointer `
            -Relocations $Relocations
        $affected = -not [string]::Equals($Pointer, $destinationObjectPointer, [System.StringComparison]::Ordinal)
        foreach ($branch in $branches) {
            if (-not [string]::Equals($branch.SourcePointer, $branch.DestinationPointer, [System.StringComparison]::Ordinal)) {
                $affected = $true
                break
            }
        }
        if ($affected -and $branches.Count -gt 0) {
            $destinationDiscriminatorPointer = "$destinationObjectPointer/discriminator"
            foreach ($branch in $branches) {
            # Preserve the old implicit identity and also make the canonical
            # identity explicit. Multiple aliases intentionally share a target.
                Add-OpenApiDiscriminatorAlias `
                    -AliasPlans $AliasPlans `
                    -DiscriminatorPointer $destinationDiscriminatorPointer `
                    -Alias $branch.SourceName `
                    -DestinationPointer $branch.DestinationPointer
                Add-OpenApiDiscriminatorAlias `
                    -AliasPlans $AliasPlans `
                    -DiscriminatorPointer $destinationDiscriminatorPointer `
                    -Alias $branch.DestinationName `
                    -DestinationPointer $branch.DestinationPointer
            }
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        Add-OpenApiDiscriminatorAliasPlans `
            -Value $property.Value `
            -Pointer "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Relocations $Relocations `
            -AliasPlans $AliasPlans `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Applies the precomputed alias plans without otherwise reordering the document.
# Existing explicit mappings are copied first, then generated aliases are appended
# in source branch encounter order. This makes output deterministic and keeps
# source-authored mapping keys visually prominent.
function ConvertTo-OpenApiDiscriminatorAliases {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$AliasPlans,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Discriminator alias application exceeds the supported depth of $MaximumDepth at '$Pointer'."
    }
    if ($null -eq $Value) {
        return $null
    }
    if (Test-JsonArrayValue -Value $Value) {
        [object[]]$items = [object[]]::new($Value.Count)
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-OpenApiDiscriminatorAliases `
                -Value $Value[$index] `
                -Pointer "$Pointer/$index" `
                -AliasPlans $AliasPlans `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $propertyPointer = "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if ([string]::Equals($property.Name, "discriminator", [System.StringComparison]::Ordinal) -and
            $AliasPlans.ContainsKey($propertyPointer) -and
            (Test-JsonObjectValue -Value $property.Value)) {
            $discriminator = [ordered]@{}
            $existingMapping = Get-JsonPropertyValue -Object $property.Value -Name "mapping"
            foreach ($discriminatorProperty in (Get-JsonProperties -Value $property.Value)) {
                if ([string]::Equals($discriminatorProperty.Name, "mapping", [System.StringComparison]::Ordinal)) {
                    continue
                }
                $discriminator[$discriminatorProperty.Name] = Copy-JsonValue -Value $discriminatorProperty.Value
            }
            $mapping = [ordered]@{}
            if (Test-JsonObjectValue -Value $existingMapping) {
                foreach ($mappingProperty in (Get-JsonProperties -Value $existingMapping)) {
                    $mapping[$mappingProperty.Name] = $mappingProperty.Value
                }
            }
            foreach ($aliasProperty in (Get-JsonProperties -Value $AliasPlans[$propertyPointer])) {
                # Relocation has already updated existing mapping values. A key
                # conflict here means the source assigned that discriminator
                # identity to a different schema, which cannot be reconciled safely.
                if ($mapping.Contains($aliasProperty.Name) -and
                    -not [string]::Equals($mapping[$aliasProperty.Name], $aliasProperty.Value, [System.StringComparison]::Ordinal)) {
                    throw "Existing discriminator mapping '$($aliasProperty.Name)' at '$propertyPointer' conflicts with deduplication alias '$($aliasProperty.Value)'."
                }
                $mapping[$aliasProperty.Name] = $aliasProperty.Value
            }
            $discriminator["mapping"] = $mapping
            $result[$property.Name] = $discriminator
            continue
        }

        $result[$property.Name] = ConvertTo-OpenApiDiscriminatorAliases `
            -Value $property.Value `
            -Pointer $propertyPointer `
            -AliasPlans $AliasPlans `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

# Applies a fully reconciled persisted mapping to one componentized document.
# The function is intentionally pure: neither the source document nor the mapping
# is mutated, and all validation happens before the caller writes output bytes.
function ConvertTo-DeduplicatedOpenApiComponentsDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Mapping
    )

    Assert-OpenApiComponentDeduplicationMapping -Mapping $Mapping
    $analysis = Get-OpenApiComponentDeduplicationAnalysis -Document $Document
    $sourceNames = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$analysis.ComponentNames,
        [System.StringComparer]::Ordinal
    )
    [object[]]$groups = [object[]](Get-JsonPropertyValue -Object $Mapping -Name "groups")

    # The same entry point supports two valid shapes:
    #   * source shape: every historical member is still present;
    #   * collapsed shape: only each group's canonical name is present.
    # Anything between those states is rejected as a partial/manual application.
    $sourceShape = $true
    $collapsedShape = $true
    foreach ($group in $groups) {
        [object[]]$members = [object[]](Get-JsonPropertyValue -Object $group -Name "members")
        $canonicalName = Get-JsonPropertyValue -Object $group -Name "canonicalName"
        foreach ($member in $members) {
            if (-not $sourceNames.Contains($member)) {
                $sourceShape = $false
            }
            if (-not [string]::Equals($member, $canonicalName, [System.StringComparison]::Ordinal) -and
                $sourceNames.Contains($member)) {
                $collapsedShape = $false
            }
        }
        if (-not $sourceNames.Contains($canonicalName)) {
            $collapsedShape = $false
        }
    }

    if ($collapsedShape -and -not $sourceShape) {
        # Idempotent reapplication is a validation-only no-op. Residual duplicate
        # groups indicate a corrupt or stale collapsed artifact and are not accepted.
        if ($analysis.DuplicateGroupCount -ne 0) {
            throw "Already-deduplicated input still contains $($analysis.DuplicateGroupCount) eligible duplicate groups."
        }
        $copy = Copy-JsonValue -Value $Document
        Assert-OpenApiReferencesResolve -Document $copy
        Assert-AdditionalOpenApiReferencesResolve -Document $copy
        return [PSCustomObject]@{
            Document = $copy
            ComponentCount = $analysis.ComponentNames.Count
            DuplicateGroupCount = 0
            RemovedComponentCount = 0
            RelocationCount = 0
            DiscriminatorAliasCount = 0
            AppliedGroups = [object[]]@()
            SkippedCycleComponents = [object[]]$analysis.CycleDependentComponents
        }
    }
    if (-not $sourceShape) {
        throw "The componentized input is partially deduplicated or does not match the persisted mapping."
    }

    # Apply never updates policy. Re-run reconciliation in memory and require an
    # exact no-op, forcing users to review persisted mapping changes separately.
    $reconciliation = Update-OpenApiComponentDeduplicationMapping `
        -Analysis $analysis `
        -Mapping $Mapping
    if ($reconciliation.Changed) {
        throw "The component deduplication mapping is stale. Run Update-OpenApiComponentDeduplicationMap.ps1 and review its changes before applying it."
    }

    # Build root-prefix relocations before changing any JSON. Longest-prefix
    # relocation also preserves descendant suffixes, e.g.:
    #   LeafB/properties/value -> SharedLeaf/properties/value
    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    $memberToGroup = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $representativeToGroup = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($group in $groups) {
        $canonicalName = Get-JsonPropertyValue -Object $group -Name "canonicalName"
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        $destination = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $canonicalName)"
        $representativeToGroup[$representative] = $group
        foreach ($member in [object[]](Get-JsonPropertyValue -Object $group -Name "members")) {
            $memberToGroup[$member] = $group
            $source = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $member)"
            if (-not [string]::Equals($source, $destination, [System.StringComparison]::Ordinal)) {
                Add-OpenApiPointerRelocation `
                    -Relocations $relocations `
                    -SourcePrefix $source `
                    -DestinationPrefix $destination
            }
        }
    }

    # Alias discovery must inspect the pre-relocation names; after refs move we
    # could no longer infer every removed implicit discriminator identity.
    $aliasPlans = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    Add-OpenApiDiscriminatorAliasPlans `
        -Value $Document `
        -Pointer "#" `
        -Relocations $relocations `
        -AliasPlans $aliasPlans

    # Rewrite every supported reference-valued keyword across the complete
    # document before deleting duplicate definitions. This avoids a transient
    # output containing dangling refs and also relocates source mapping values.
    $relocated = ConvertTo-RelocatedOpenApiPointers `
        -Value $Document `
        -Relocations $relocations
    $relocatedSchemas = Get-OpenApiComponentSchemas -Document $relocated
    # Rebuild schemas in original encounter order. Unmapped components retain
    # their position. A mapped group is emitted exactly once, at its persisted
    # representative's position, under the possibly user-edited canonical name.
    # This separates stable output placement from canonical naming policy.
    $schemas = [ordered]@{}
    foreach ($componentName in $analysis.ComponentNames) {
        if (-not $memberToGroup.ContainsKey($componentName)) {
            $schemas[$componentName] = Get-JsonPropertyValue -Object $relocatedSchemas -Name $componentName
            continue
        }
        if (-not $representativeToGroup.ContainsKey($componentName)) {
            # Non-representative members have already had all incoming refs
            # redirected; omitting them is the actual deduplication operation.
            continue
        }
        $group = $representativeToGroup[$componentName]
        $canonicalName = Get-JsonPropertyValue -Object $group -Name "canonicalName"
        if ($schemas.Contains($canonicalName)) {
            throw "Canonical component '$canonicalName' would overwrite an existing output component."
        }
        $schemas[$canonicalName] = Get-JsonPropertyValue -Object $relocatedSchemas -Name $componentName
    }

    # Only components.schemas is rebuilt. Security schemes and any future
    # non-schema component sections are preserved without interpretation.
    $relocatedComponents = Get-JsonPropertyValue -Object $relocated -Name "components"
    $components = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $relocatedComponents)) {
        if ([string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)) {
            $components[$property.Name] = $schemas
        }
        else {
            $components[$property.Name] = $property.Value
        }
    }
    $rebuilt = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $relocated)) {
        if ([string]::Equals($property.Name, "components", [System.StringComparison]::Ordinal)) {
            $rebuilt[$property.Name] = $components
        }
        else {
            $rebuilt[$property.Name] = $property.Value
        }
    }

    # Alias metadata is added last because its destinations must already use the
    # canonical component names. Existing mappings were relocated in the prior pass.
    $result = ConvertTo-OpenApiDiscriminatorAliases `
        -Value $rebuilt `
        -Pointer "#" `
        -AliasPlans $aliasPlans
    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result

    # Postconditions verify both graph safety and completeness: references must
    # resolve, no eligible acyclic duplicate group may remain, and the component
    # count must drop by exactly sum(group member count - 1).
    $resultAnalysis = Get-OpenApiComponentDeduplicationAnalysis -Document $result
    if ($resultAnalysis.DuplicateGroupCount -ne 0) {
        throw "Deduplicated output retained $($resultAnalysis.DuplicateGroupCount) eligible duplicate component groups."
    }
    $expectedCount = $analysis.ComponentNames.Count - $analysis.RemovableComponentCount
    if ($resultAnalysis.ComponentNames.Count -ne $expectedCount) {
        throw "Deduplicated output has $($resultAnalysis.ComponentNames.Count) components; expected $expectedCount."
    }

    $aliasCount = 0
    foreach ($aliases in $aliasPlans.Values) {
        $aliasCount += @(Get-JsonProperties -Value $aliases).Count
    }
    return [PSCustomObject]@{
        Document = $result
        ComponentCount = $resultAnalysis.ComponentNames.Count
        DuplicateGroupCount = $analysis.DuplicateGroupCount
        RemovedComponentCount = $analysis.RemovableComponentCount
        RelocationCount = $relocations.Count
        DiscriminatorAliasCount = $aliasCount
        AppliedGroups = [object[]]$groups
        SkippedCycleComponents = [object[]]$analysis.CycleDependentComponents
    }
}

# File-oriented command boundary. Mapping reconciliation is intentionally not
# implicit here: this command consumes reviewed state, writes one output, prints
# an audit-friendly group list, and returns the same statistics structurally.
function Invoke-OpenApiComponentDeduplicator {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$MappingPath,

        [Parameter()]
        [string]$OutputPath
    )

    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $script:DefaultComponentDeduplicatorInputPath
    }
    else {
        $InputPath = Resolve-ComponentDeduplicatorRepositoryPath -Path $InputPath
    }
    if ([string]::IsNullOrWhiteSpace($MappingPath)) {
        $MappingPath = $script:DefaultComponentDeduplicatorMappingPath
    }
    else {
        $MappingPath = Resolve-ComponentDeduplicatorRepositoryPath -Path $MappingPath
    }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = $script:DefaultComponentDeduplicatorOutputPath
    }
    else {
        $OutputPath = Resolve-ComponentDeduplicatorRepositoryPath -Path $OutputPath
    }

    # Resolve both required inputs before conversion so path/parse failures cannot
    # leave a partially written destination.
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    $resolvedMappingPath = (Resolve-Path -LiteralPath $MappingPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The componentized OpenAPI input path is not a file: $resolvedInputPath"
    }
    if (-not [System.IO.File]::Exists($resolvedMappingPath)) {
        throw "The component deduplication mapping path is not a file: $resolvedMappingPath"
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $mapping = Read-OpenApiComponentDeduplicationMapping -Path $resolvedMappingPath
    $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
        -Document $document `
        -Mapping $mapping
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Mapping: $resolvedMappingPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Duplicate groups applied: $($result.DuplicateGroupCount)"
    Write-Host "Components removed: $($result.RemovedComponentCount)"
    Write-Host "Components retained: $($result.ComponentCount)"
    Write-Host "Pointer prefixes relocated: $($result.RelocationCount)"
    Write-Host "Discriminator aliases materialized: $($result.DiscriminatorAliasCount)"
    Write-Host "Cycle-dependent components skipped: $($result.SkippedCycleComponents.Count)"
    # The console list makes the persisted decisions visible in CI logs without
    # requiring consumers to inspect the generated OpenAPI diff.
    foreach ($group in $result.AppliedGroups) {
        Write-Host "$($group.canonicalName) <- $($group.members -join ', ')"
    }

    return [PSCustomObject]@{
        Document = $result.Document
        OutputPath = $writtenPath
        ComponentCount = $result.ComponentCount
        DuplicateGroupCount = $result.DuplicateGroupCount
        RemovedComponentCount = $result.RemovedComponentCount
        RelocationCount = $result.RelocationCount
        DiscriminatorAliasCount = $result.DiscriminatorAliasCount
        AppliedGroups = $result.AppliedGroups
        SkippedCycleComponents = $result.SkippedCycleComponents
    }
}

if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiComponentDeduplicator `
        -InputPath $InputPath `
        -MappingPath $MappingPath `
        -OutputPath $OutputPath
}
