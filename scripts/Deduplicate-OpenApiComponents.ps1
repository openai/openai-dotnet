<#
.SYNOPSIS
    Applies persisted component-tree deduplication mappings to OpenAPI JSON.

.DESCRIPTION
    Requires a mapping reconciled by Update-OpenApiComponentDeduplicationMap.ps1.
    Equivalent acyclic component trees are collapsed to their persisted canonical
    names, and all local reference-valued pointers, including discriminator
    mapping targets, are relocated. Discriminator mapping keys remain the exact
    payload values materialized during componentization.

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
    $null = Assert-OpenApiDiscriminatorMappings -Document $Document
    $analysis = Get-OpenApiComponentDeduplicationAnalysis -Document $Document
    $sourceNames = [System.Collections.Generic.HashSet[string]]::new(
        [string[]]$analysis.ComponentNames,
        [System.StringComparer]::Ordinal
    )
    [object[]]$groups = [object[]](Get-JsonPropertyValue -Object $Mapping -Name "groups")

    # The same entry point supports two valid shapes:
    #   * source shape: every historical member is still present;
    #   * collapsed shape: only each group's id is present.
    # Anything between those states is rejected as a partial/manual application.
    $sourceShape = $true
    $collapsedShape = $true
    foreach ($group in $groups) {
        [object[]]$members = [object[]](Get-JsonPropertyValue -Object $group -Name "members")
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        foreach ($member in $members) {
            if (-not $sourceNames.Contains($member)) {
                $sourceShape = $false
            }
            if (-not [string]::Equals($member, $id, [System.StringComparison]::Ordinal) -and
                $sourceNames.Contains($member)) {
                $collapsedShape = $false
            }
        }
        if (-not $sourceNames.Contains($id)) {
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
        $outputDiscriminatorValidation = Assert-OpenApiDiscriminatorMappings -Document $copy
        return [PSCustomObject]@{
            Document = $copy
            ComponentCount = $analysis.ComponentNames.Count
            DuplicateGroupCount = 0
            RemovedComponentCount = 0
            RelocationCount = 0
            DiscriminatorCount = $outputDiscriminatorValidation.DiscriminatorCount
            DiscriminatorMappingEntryCount = $outputDiscriminatorValidation.MappingEntryCount
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
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        $representative = Get-JsonPropertyValue -Object $group -Name "representative"
        $destination = "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $id)"
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

    # Rewrite every supported reference-valued keyword across the complete
    # document before deleting duplicate definitions. This avoids a transient
    # output containing dangling refs and also relocates source mapping values.
    $relocated = ConvertTo-RelocatedOpenApiPointers `
        -Value $Document `
        -Relocations $relocations
    $relocatedSchemas = Get-OpenApiComponentSchemas -Document $relocated
    # Rebuild schemas in original encounter order. Unmapped components retain
    # their position. A mapped group is emitted exactly once, at its persisted
    # representative's position, under the possibly user-edited id. This
    # separates stable output placement from source-member selection.
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
        $id = Get-JsonPropertyValue -Object $group -Name "id"
        if ($schemas.Contains($id)) {
            throw "Deduplicated component '$id' would overwrite an existing output component."
        }
        $schemas[$id] = Get-JsonPropertyValue -Object $relocatedSchemas -Name $componentName
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

    $result = $rebuilt
    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result
    $outputDiscriminatorValidation = Assert-OpenApiDiscriminatorMappings -Document $result

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

    return [PSCustomObject]@{
        Document = $result
        ComponentCount = $resultAnalysis.ComponentNames.Count
        DuplicateGroupCount = $analysis.DuplicateGroupCount
        RemovedComponentCount = $analysis.RemovableComponentCount
        RelocationCount = $relocations.Count
        DiscriminatorCount = $outputDiscriminatorValidation.DiscriminatorCount
        DiscriminatorMappingEntryCount = $outputDiscriminatorValidation.MappingEntryCount
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
    Write-Host "Discriminators validated: $($result.DiscriminatorCount)"
    Write-Host "Discriminator mapping entries retained: $($result.DiscriminatorMappingEntryCount)"
    Write-Host "Cycle-dependent components skipped: $($result.SkippedCycleComponents.Count)"
    # The console list makes the persisted decisions visible in CI logs without
    # requiring consumers to inspect the generated OpenAPI diff.
    foreach ($group in $result.AppliedGroups) {
        Write-Host "$($group.id) <- $($group.members -join ', ')"
    }

    return [PSCustomObject]@{
        Document = $result.Document
        OutputPath = $writtenPath
        ComponentCount = $result.ComponentCount
        DuplicateGroupCount = $result.DuplicateGroupCount
        RemovedComponentCount = $result.RemovedComponentCount
        RelocationCount = $result.RelocationCount
        DiscriminatorCount = $result.DiscriminatorCount
        DiscriminatorMappingEntryCount = $result.DiscriminatorMappingEntryCount
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
