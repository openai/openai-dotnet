<#
.SYNOPSIS
    Reconciles persisted duplicate component-tree mappings.

.DESCRIPTION
    Analyzes an OpenAPI components.schemas graph after object componentization.
    New acyclic duplicate-tree groups are appended to a persisted JSON mapping;
    existing groups retain their stable identity, representative, member order,
    and canonical name. Newly equivalent members are appended, while equivalent
    whole-group evolution updates only the fingerprint.

    Missing, diverged, split, merged, colliding, or cycle-dependent historical
    groups fail before the mapping file is changed. Incomplete or noncanonical
    discriminator mappings also fail before graph analysis. This command never
    rewrites an OpenAPI document; review the mapping before running
    Deduplicate-OpenApiComponents.ps1.

.PARAMETER InputPath
    The componentized OpenAPI JSON document. Defaults to
    specification/openapi/responses.componentized.openapi.json.

.PARAMETER MappingPath
    The persisted JSON mapping. Defaults to
    specification/openapi/responses.component-deduplication-map.json.

.EXAMPLE
    ./scripts/Update-OpenApiComponentDeduplicationMap.ps1

.EXAMPLE
    ./scripts/Update-OpenApiComponentDeduplicationMap.ps1 `
        ./artifacts/custom.componentized.openapi.json `
        -MappingPath ./artifacts/custom.component-deduplication-map.json

    Reconciles a mapping beside a non-default componentized document. A missing
    mapping starts at version 1; an existing mapping is treated as reviewed state.
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$InputPath,

    [Parameter()]
    [string]$MappingPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Load the general JSON/OpenAPI primitives before the deduplication helpers,
# which intentionally depend on those primitives rather than dot-sourcing them
# again. Failing here gives callers a direct setup error instead of a later
# missing-function failure.
$helperPath = Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1"
$deduplicationHelperPath = Join-Path $PSScriptRoot "_OpenApiComponentDeduplication.Helpers.ps1"
foreach ($path in @($helperPath, $deduplicationHelperPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "The required helper script was not found: $path"
    }
    . $path
}

$script:ComponentDeduplicationRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultComponentDeduplicationInputPath = Join-Path $script:ComponentDeduplicationRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"
$script:DefaultComponentDeduplicationMappingPath = Join-Path $script:ComponentDeduplicationRepositoryRoot "specification" "openapi" "responses.component-deduplication-map.json"

# Relative command-line paths are anchored at the repository root so invocation
# does not depend on the caller's working directory. Absolute paths remain useful
# for CI, temporary test fixtures, and external automation.
function Resolve-ComponentDeduplicationRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:ComponentDeduplicationRepositoryRoot $Path))
}

# File-oriented reconciliation boundary:
#   1. resolve and read the componentized source and optional persisted mapping;
#   2. analyze the complete component dependency graph;
#   3. reconcile all historical decisions in memory;
#   4. write only after every invariant succeeds.
#
# The function never modifies the OpenAPI source. Its output is reviewed policy
# consumed later by Deduplicate-OpenApiComponents.ps1.
function Invoke-OpenApiComponentDeduplicationMapUpdate {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$MappingPath
    )

    if ([string]::IsNullOrWhiteSpace($InputPath)) {
        $InputPath = $script:DefaultComponentDeduplicationInputPath
    }
    else {
        $InputPath = Resolve-ComponentDeduplicationRepositoryPath -Path $InputPath
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The componentized OpenAPI input path is not a file: $resolvedInputPath"
    }

    if ([string]::IsNullOrWhiteSpace($MappingPath)) {
        $MappingPath = $script:DefaultComponentDeduplicationMappingPath
    }
    else {
        $MappingPath = Resolve-ComponentDeduplicationRepositoryPath -Path $MappingPath
    }

    # Absence is valid bootstrap state, not an error. Once present, however, the
    # mapping is authoritative: reconciliation preserves its ids, representatives,
    # member order, and manually selected canonical names.
    $existingMapping = if ([System.IO.File]::Exists($MappingPath)) {
        Read-OpenApiComponentDeduplicationMapping -Path $MappingPath
    }
    else {
        $null
    }
    # Analysis and reconciliation are pure with respect to both inputs. A stale,
    # split, merged, or otherwise unsafe historical group throws before the file
    # system is touched.
    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $null = Assert-OpenApiDiscriminatorMappings -Document $document
    $analysis = Get-OpenApiComponentDeduplicationAnalysis -Document $document
    $result = Update-OpenApiComponentDeduplicationMapping `
        -Analysis $analysis `
        -Mapping $existingMapping

    # Do not rewrite an unchanged mapping. Besides avoiding timestamp churn, this
    # guarantees byte idempotence for a reviewed file on repeated updater runs.
    $writtenPath = [System.IO.Path]::GetFullPath($MappingPath)
    if ($result.Changed -or -not [System.IO.File]::Exists($writtenPath)) {
        $writtenPath = Write-OpenApiJsonFile `
            -Document $result.Mapping `
            -Path $writtenPath
    }

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Mapping: $writtenPath"
    Write-Host "Components scanned: $($analysis.ComponentNames.Count)"
    Write-Host "Duplicate tree groups: $($analysis.DuplicateGroupCount)"
    Write-Host "Potential component removals: $($analysis.RemovableComponentCount)"
    Write-Host "Cycle-dependent components skipped: $($analysis.CycleDependentComponents.Count)"
    # Unchanged groups are intentionally omitted from the console summary so a
    # reviewer can focus on new members and fingerprint evolution.
    foreach ($status in $result.Statuses) {
        if ([string]::Equals($status.Status, "Unchanged", [System.StringComparison]::Ordinal)) {
            continue
        }
        $suffix = if ($status.AddedMembers.Count -gt 0) {
            " (+ $($status.AddedMembers -join ', '))"
        }
        else {
            ""
        }
        Write-Host "[$($status.Status)] $($status.CanonicalName)$suffix"
    }

    return [PSCustomObject]@{
        Mapping = $result.Mapping
        MappingPath = $writtenPath
        Changed = $result.Changed
        Statuses = $result.Statuses
        NewGroupCount = $result.NewGroupCount
        ExtendedGroupCount = $result.ExtendedGroupCount
        FingerprintUpdatedGroupCount = $result.FingerprintUpdatedGroupCount
        UnchangedGroupCount = $result.UnchangedGroupCount
        ComponentCount = $analysis.ComponentNames.Count
        DuplicateGroupCount = $analysis.DuplicateGroupCount
        RemovableComponentCount = $analysis.RemovableComponentCount
        SkippedCycleComponents = $result.SkippedCycleComponents
    }
}

# Keep the script usable as both a CLI and a function library for focused tests.
# Dot-sourcing defines the functions but must never update the production map.
if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiComponentDeduplicationMapUpdate `
        -InputPath $InputPath `
        -MappingPath $MappingPath
}
