<#
.SYNOPSIS
    Runs focused tests for scripts/Update-OpenApiComponentDeduplicationMap.ps1.

.DESCRIPTION
    Exercises graph analysis, recursive tree identity, stable persisted mapping
    reconciliation, unsafe evolution failures, and deterministic file handling.
    The suite is dependency-free and runs directly under PowerShell 7:

        pwsh ./scripts/tests/Update-OpenApiComponentDeduplicationMap.Tests.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Dot-source the command so tests can call both its file boundary and the pure
# analysis/reconciliation functions supplied by its shared helper. The command's
# invocation guard prevents loading it from touching the production mapping.
. (Join-Path $PSScriptRoot ".." "Update-OpenApiComponentDeduplicationMap.ps1")
. (Join-Path $PSScriptRoot "_OpenApiComponentDeduplicationTest.Helpers.ps1")
Initialize-OpenApiComponentDeduplicationTestState

# The shared fixture contains equal leaves, equal parents that point to those
# leaves under different names, and equal exact branches. A near miss, a cycle,
# and an ancestor of that cycle provide negative controls for eligibility.
$fixturePath = Get-TestDeduplicationFixturePath
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "update-component-dedup-map-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    # Locks the complete analysis model: recursive child identity makes ParentA
    # and ParentB equal, while CycleAncestor is excluded even though it is not
    # itself in the CycleA/CycleB strongly connected region.
    Invoke-Test "analysis finds duplicate trees and skips cycle-dependent components" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $analysis = New-TestDeduplicationAnalysis -Document $source
        Assert-Equal 10 $analysis.ComponentNames.Count "Component count was incorrect."
        Assert-Equal 3 $analysis.DuplicateGroupCount "Duplicate group count was incorrect."
        Assert-Equal 3 $analysis.RemovableComponentCount "Removal count was incorrect."
        Assert-Sequence @("LeafA", "LeafB") @($analysis.DuplicateGroups[0].Members) "Leaf group was incorrect."
        Assert-Sequence @("ParentA", "ParentB") @($analysis.DuplicateGroups[1].Members) "Parent tree group was incorrect."
        Assert-Sequence @("ExactA", "ExactB") @($analysis.DuplicateGroups[2].Members) "Exact group was incorrect."
        Assert-Sequence @("CycleA", "CycleB", "CycleAncestor") @($analysis.CycleDependentComponents) "Cycle-dependent classification was incorrect."
    }

    # A first run has no historical policy to preserve. Each group's first source
    # member therefore seeds its id, representative, and canonical output name,
    # and group order follows source encounter order for deterministic review.
    Invoke-Test "initial mapping uses first encounter defaults" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $analysis = New-TestDeduplicationAnalysis -Document $source
        $result = Update-OpenApiComponentDeduplicationMapping -Analysis $analysis
        Assert-True $result.Changed "Initial reconciliation did not report a change."
        Assert-Equal 3 $result.NewGroupCount "New group count was incorrect."
        Assert-Sequence @("LeafA", "ParentA", "ExactA") @($result.Mapping.groups.id) "Group order was incorrect."
        foreach ($group in $result.Mapping.groups) {
            Assert-Equal $group.id $group.representative "Representative default was incorrect."
            Assert-Equal $group.id $group.canonicalName "Canonical-name default was incorrect."
        }
    }

    # Componentization emits one direct-oneOf wrapper per occurrence. Wrappers
    # whose corresponding branches reference equivalent component trees must
    # form their own persisted group even when the literal ref names differ.
    Invoke-Test "equivalent oneOf wrapper components form a mapping group" {
        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $source.components.schemas["RedA"] = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("red") } }
        }
        $source.components.schemas["RedB"] = Copy-JsonValue -Value $source.components.schemas.RedA
        $source.components.schemas["Blue"] = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{ kind = [ordered]@{ type = "string"; const = "blue" } }
        }
        $source.components.schemas["UnionA"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "#/components/schemas/RedA"
                    blue = "#/components/schemas/Blue"
                }
            }
            oneOf = @(
                [ordered]@{ '$ref' = "#/components/schemas/RedA" },
                [ordered]@{ '$ref' = "#/components/schemas/Blue" }
            )
        }
        $source.components.schemas["UnionB"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "#/components/schemas/RedB"
                    blue = "#/components/schemas/Blue"
                }
            }
            oneOf = @(
                [ordered]@{ '$ref' = "#/components/schemas/RedB" },
                [ordered]@{ '$ref' = "#/components/schemas/Blue" }
            )
        }

        $analysis = New-TestDeduplicationAnalysis -Document $source
        $unionGroup = @($analysis.DuplicateGroups | Where-Object { $_.Members -ccontains "UnionA" })
        Assert-Equal 1 $unionGroup.Count "Equivalent oneOf wrappers did not form exactly one group."
        Assert-Sequence @("UnionA", "UnionB") @($unionGroup[0].Members) "oneOf wrapper group members were incorrect."

        $result = Update-OpenApiComponentDeduplicationMapping -Analysis $analysis
        $mappedUnionGroup = @($result.Mapping.groups | Where-Object { $_.members -ccontains "UnionA" })[0]
        Assert-Equal "UnionA" $mappedUnionGroup.id "oneOf wrapper group id was incorrect."
        Assert-Equal "UnionA" $mappedUnionGroup.canonicalName "oneOf wrapper canonical default was incorrect."
    }

    # Longitudinal stability beats current encounter order. BeforeLeaf is inserted
    # ahead of LeafA in the document but appends after the persisted members, so
    # neither the reviewed identity nor existing JSON order churns.
    Invoke-Test "earlier new duplicate appends without renaming the group" {
        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $mapping = New-TestDeduplicationMapping -Document $source
        $schemas = [ordered]@{ BeforeLeaf = Copy-JsonValue -Value $source.components.schemas.LeafA }
        foreach ($property in (Get-JsonProperties -Value $source.components.schemas)) {
            $schemas[$property.Name] = $property.Value
        }
        $source.components.schemas = $schemas
        $analysis = New-TestDeduplicationAnalysis -Document $source
        $result = Update-OpenApiComponentDeduplicationMapping -Analysis $analysis -Mapping $mapping
        $leafGroup = $result.Mapping.groups[0]
        Assert-Equal "LeafA" $leafGroup.id "Stable group id changed."
        Assert-Equal "LeafA" $leafGroup.representative "Stable representative changed."
        Assert-Equal "LeafA" $leafGroup.canonicalName "Stable canonical name changed."
        Assert-Sequence @("LeafA", "LeafB", "BeforeLeaf") @($leafGroup.members) "New member was not appended."
        Assert-Equal "Extended" $result.Statuses[0].Status "Group status was incorrect."
    }

    # Changing both leaves identically is safe whole-group evolution. The leaf
    # fingerprint changes directly and the parent fingerprint changes transitively
    # because parent canonical text embeds its child tree fingerprint.
    Invoke-Test "whole-group evolution updates fingerprints without changing identity" {
        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $mapping = New-TestDeduplicationMapping -Document $source
        foreach ($name in @("LeafA", "LeafB")) {
            $source.components.schemas[$name]["additionalProperties"] = $false
        }
        $result = Update-OpenApiComponentDeduplicationMapping `
            -Analysis (New-TestDeduplicationAnalysis $source) `
            -Mapping $mapping
        Assert-Equal "LeafA" $result.Mapping.groups[0].id "Group id changed during whole-group evolution."
        Assert-Equal "FingerprintUpdated" $result.Statuses[0].Status "Leaf fingerprint was not updated."
        Assert-Equal "FingerprintUpdated" $result.Statuses[1].Status "Parent fingerprint was not updated transitively."
    }

    # canonicalName is explicitly user-editable policy, not a derived field to be
    # reset on every run. A fresh name is preserved; a name already owned by an
    # unrelated source component is rejected before deduplication could overwrite it.
    Invoke-Test "manual canonical names are preserved" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $mapping = New-TestDeduplicationMapping -Document $source
        $mapping.groups[0].canonicalName = "SharedLeaf"
        $result = Update-OpenApiComponentDeduplicationMapping `
            -Analysis (New-TestDeduplicationAnalysis $source) `
            -Mapping $mapping
        Assert-Equal "SharedLeaf" $result.Mapping.groups[0].canonicalName "Manual canonical name changed."
        Assert-True (-not $result.Changed) "Manual canonical name caused spurious reconciliation changes."

        $mapping.groups[0].canonicalName = "NearMiss"
        Assert-Throws {
            Update-OpenApiComponentDeduplicationMapping `
                -Analysis (New-TestDeduplicationAnalysis $source) `
                -Mapping $mapping
        } "*collides with an unrelated source component*"
    }

    # Persisted history is never guessed away. These mutations cover a removed
    # member, a member that ceases to be structurally equivalent, and two formerly
    # separate reviewed groups converging into one current group. Each requires
    # human review rather than silently deleting or combining identities.
    Invoke-Test "missing divergent split and merged groups fail before reconciliation" {
        $base = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $mapping = New-TestDeduplicationMapping -Document $base

        $missing = Copy-JsonValue -Value $base
        $null = $missing.components.schemas.Remove("ExactB")
        $missing.paths["/test"].post.responses["200"].content["application/json"].schema.properties.exact.'$ref' = "#/components/schemas/ExactA"
        $missing.components.schemas.ParentA.properties.choice.oneOf[1].'$ref' = "#/components/schemas/ExactA"
        $missing.components.schemas.ParentB.properties.choice.oneOf[0].'$ref' = "#/components/schemas/ExactA"
        Assert-Throws {
            Update-OpenApiComponentDeduplicationMapping `
                -Analysis (New-TestDeduplicationAnalysis $missing) `
                -Mapping $mapping
        } "*ExactB*missing*"

        $divergent = Copy-JsonValue -Value $base
        $divergent.components.schemas.LeafB.properties.value.type = "number"
        Assert-Throws {
            Update-OpenApiComponentDeduplicationMapping `
                -Analysis (New-TestDeduplicationAnalysis $divergent) `
                -Mapping $mapping
        } "*group 'LeafA'*no longer belongs*"

        $merged = Copy-JsonValue -Value $base
        $merged.components.schemas.ExactA = Copy-JsonValue -Value $merged.components.schemas.LeafA
        $merged.components.schemas.ExactB = Copy-JsonValue -Value $merged.components.schemas.LeafB
        Assert-Throws {
            Update-OpenApiComponentDeduplicationMapping `
                -Analysis (New-TestDeduplicationAnalysis $merged) `
                -Mapping $mapping
        } "*mapped groups*merged*"
    }

    # The CLI test covers the transaction boundary as well as JSON semantics:
    # creation uses UTF-8 without BOM plus trailing LF, an unchanged second pass
    # performs no write, and a failed third pass preserves the prior bytes exactly.
    Invoke-Test "CLI reconciliation is atomic deterministic and byte-idempotent" {
        $mappingPath = Join-Path $temporaryDirectory "mapping.json"
        $first = Invoke-OpenApiComponentDeduplicationMapUpdate `
            -InputPath $fixturePath `
            -MappingPath $mappingPath
        Assert-True $first.Changed "Initial CLI reconciliation did not report a change."
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($mappingPath)
        $hasBom = $firstBytes.Length -ge 3 -and $firstBytes[0] -eq 0xEF -and $firstBytes[1] -eq 0xBB -and $firstBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Mapping contains a UTF-8 BOM."
        Assert-Equal 10 $firstBytes[$firstBytes.Length - 1] "Mapping lacks a trailing LF."

        $second = Invoke-OpenApiComponentDeduplicationMapUpdate `
            -InputPath $fixturePath `
            -MappingPath $mappingPath
        Assert-True (-not $second.Changed) "Second reconciliation reported a change."
        Assert-BytesEqual $firstBytes ([System.IO.File]::ReadAllBytes($mappingPath)) "Second reconciliation changed mapping bytes."

        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $source.components.schemas.LeafB.properties.value.type = "number"
        $invalidSourcePath = Join-Path $temporaryDirectory "invalid-source.json"
        $null = Write-OpenApiJsonFile -Document $source -Path $invalidSourcePath
        [byte[]]$beforeFailure = [System.IO.File]::ReadAllBytes($mappingPath)
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicationMapUpdate `
                -InputPath $invalidSourcePath `
                -MappingPath $mappingPath
        } "*group 'LeafA'*no longer belongs*"
        Assert-BytesEqual $beforeFailure ([System.IO.File]::ReadAllBytes($mappingPath)) "Failed reconciliation changed mapping bytes."

        $invalidDiscriminator = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $invalidDiscriminator.components.schemas["Red"] = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{ kind = [ordered]@{ type = "string"; enum = @("red") } }
        }
        $invalidDiscriminator.components.schemas["Choice"] = [ordered]@{
            discriminator = [ordered]@{ propertyName = "kind" }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Red" })
        }
        $invalidDiscriminatorPath = Join-Path $temporaryDirectory "invalid-discriminator.json"
        $null = Write-OpenApiJsonFile -Document $invalidDiscriminator -Path $invalidDiscriminatorPath
        [byte[]]$beforeDiscriminatorFailure = [System.IO.File]::ReadAllBytes($mappingPath)
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicationMapUpdate `
                -InputPath $invalidDiscriminatorPath `
                -MappingPath $mappingPath
        } "*incomplete or noncanonical discriminator mappings*"
        Assert-BytesEqual $beforeDiscriminatorFailure ([System.IO.File]::ReadAllBytes($mappingPath)) "Invalid discriminator changed mapping bytes."
    }
}
finally {
    # A GUID-named directory makes concurrent test runs independent; cleanup also
    # removes the intentionally invalid source used by the atomicity assertion.
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiComponentDeduplicationTests
