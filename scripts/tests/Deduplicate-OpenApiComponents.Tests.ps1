<#
.SYNOPSIS
    Runs focused tests for scripts/Deduplicate-OpenApiComponents.ps1.

.DESCRIPTION
    Exercises persisted whole-tree deduplication, all supported local pointer
    kinds, discriminator mapping relocation, stale-state rejection, and
    structural/byte idempotence. Run directly under PowerShell 7:

        pwsh ./scripts/tests/Deduplicate-OpenApiComponents.Tests.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# The updater is loaded only to construct valid mappings from each test document;
# the command under test must consume those mappings without modifying them.
. (Join-Path $PSScriptRoot ".." "Update-OpenApiComponentDeduplicationMap.ps1")
. (Join-Path $PSScriptRoot ".." "Deduplicate-OpenApiComponents.ps1")
. (Join-Path $PSScriptRoot "_OpenApiComponentDeduplicationTest.Helpers.ps1")
Initialize-OpenApiComponentDeduplicationTestState

# The fixture contains three eligible acyclic groups: equivalent leaf schemas,
# equivalent parents that reference those leaves, and exact nested branches. It
# also contains a near miss, a reference cycle, and a cycle-dependent ancestor.
# This lets one compact document distinguish whole-tree equality from unsafe
# cycle handling.
$fixturePath = Get-TestDeduplicationFixturePath
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "deduplicate-components-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

function New-RenamedTestMapping {
    param([object]$Document)

    # Canonical names need not be existing member names. Using fresh `Shared*`
    # names verifies that persisted naming policy is independent of whichever
    # source member was selected as the group's structural representative.
    $mapping = New-TestDeduplicationMapping -Document $Document
    $mapping.groups[0].canonicalName = "SharedLeaf"
    $mapping.groups[1].canonicalName = "SharedParent"
    $mapping.groups[2].canonicalName = "SharedExact"
    return $mapping
}

try {
    # This is the end-to-end structural contract. Child groups collapse first in
    # the fingerprint model, allowing equivalent parent trees to collapse too;
    # cyclic and cycle-dependent schemas remain untouched because recursive tree
    # equality is intentionally outside this transformation's scope.
    Invoke-Test "whole duplicate trees collapse under persisted canonical names" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $sourceBefore = ConvertTo-Json -InputObject $source -Compress -Depth 100
        $mapping = New-RenamedTestMapping -Document $source
        $mappingBefore = ConvertTo-Json -InputObject $mapping -Compress -Depth 100
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
            -Document $source `
            -Mapping $mapping
        Assert-Equal 3 $result.DuplicateGroupCount "Applied group count was incorrect."
        Assert-Equal 3 $result.RemovedComponentCount "Removal count was incorrect."
        Assert-Equal 7 $result.ComponentCount "Retained component count was incorrect."
        Assert-Sequence `
            @("SharedLeaf", "SharedParent", "SharedExact", "NearMiss", "CycleA", "CycleB", "CycleAncestor") `
            @((Get-JsonProperties -Value $result.Document.components.schemas).Name) `
            "Canonical component names or placement were incorrect."
        Assert-Equal "#/components/schemas/SharedLeaf" $result.Document.components.schemas.SharedParent.properties.child.'$ref' "Parent tree did not relocate its child."
        Assert-True (Test-JsonProperty -Object $result.Document.components.schemas -Name "CycleA") "Cyclic component was removed."
        Assert-True (Test-JsonProperty -Object $result.Document.components.schemas -Name "CycleAncestor") "Cycle-dependent component was removed."
        Assert-Equal "apiKey" $result.Document.components.securitySchemes.ApiKey.type "Non-schema components changed."
        Assert-Equal $sourceBefore (ConvertTo-Json -InputObject $source -Compress -Depth 100) "Application mutated its source."
        Assert-Equal $mappingBefore (ConvertTo-Json -InputObject $mapping -Compress -Depth 100) "Application mutated its mapping."
    }

    # Root-prefix relocation must apply uniformly to ordinary, dynamic, and
    # recursive refs. A pointer below a removed component root must retain its
    # suffix: `LeafB/properties/value` becomes `SharedLeaf/properties/value`.
    Invoke-Test "all pointer kinds and descendant suffixes relocate" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $mapping = New-RenamedTestMapping -Document $source
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $source -Mapping $mapping
        $properties = $result.Document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        Assert-Equal "#/components/schemas/SharedLeaf/properties/value" $properties.descendantAlias.'$ref' "Descendant reference suffix was not preserved."
        Assert-Equal "#/components/schemas/SharedLeaf" $properties.dynamicAlias.'$dynamicRef' "`$dynamicRef was not relocated."
        Assert-Equal "#/components/schemas/SharedLeaf" $properties.recursiveAlias.'$recursiveRef' "`$recursiveRef was not relocated."
        Assert-Equal "#/components/schemas/SharedExact" $properties.exact.'$ref' "Exact duplicate reference was not relocated."
        Assert-OpenApiReferencesResolve -Document $result.Document
        Assert-AdditionalOpenApiReferencesResolve -Document $result.Document
    }

    # Generated oneOf wrappers participate in ordinary whole-tree deduplication.
    # Mapping keys remain payload values while only their component targets move.
    Invoke-Test "equivalent oneOf wrapper components collapse as complete trees" {
        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $properties = $source.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        $properties["unionA"] = [ordered]@{ '$ref' = "#/components/schemas/UnionA" }
        $properties["unionB"] = [ordered]@{ '$ref' = "#/components/schemas/UnionB" }
        $longRedName = "createResponse_request_object_Input_union_array_item_discriminatedObject_typeOfRed_Tools_array_item_object"
        $shortRedName = "createResponse_request_object_Tools_array_item_object"
        $source.components.schemas[$longRedName] = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{
                kind = [ordered]@{ type = "string"; enum = @("red") }
            }
        }
        $source.components.schemas["Blue"] = [ordered]@{
            type = "object"
            required = @("kind")
            properties = [ordered]@{
                kind = [ordered]@{ type = "string"; const = "blue" }
            }
        }
        $source.components.schemas[$shortRedName] = Copy-JsonValue -Value $source.components.schemas[$longRedName]
        $source.components.schemas["UnionA"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "#/components/schemas/$longRedName"
                    blue = "#/components/schemas/Blue"
                }
            }
            oneOf = @(
                [ordered]@{ '$ref' = "#/components/schemas/$longRedName" },
                [ordered]@{ '$ref' = "#/components/schemas/Blue" }
            )
        }
        $source.components.schemas["UnionB"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    red = "#/components/schemas/$shortRedName"
                    blue = "#/components/schemas/Blue"
                }
            }
            oneOf = @(
                [ordered]@{ '$ref' = "#/components/schemas/$shortRedName" },
                [ordered]@{ '$ref' = "#/components/schemas/Blue" }
            )
        }

        $mapping = New-RenamedTestMapping -Document $source
        $redGroup = @($mapping.groups | Where-Object { $_.members -ccontains $longRedName })[0]
        Assert-Sequence @($longRedName, $shortRedName) @($redGroup.members) "Red group member order changed."
        Assert-Equal $shortRedName $redGroup.representative "Shortest red representative was not selected."
        $redGroup.canonicalName = "SharedRed"
        $unionGroup = @($mapping.groups | Where-Object { $_.members -ccontains "UnionA" })[0]
        $unionGroup.canonicalName = "SharedUnion"
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $source -Mapping $mapping

        $componentNames = @((Get-JsonProperties -Value $result.Document.components.schemas).Name)
        $blueIndex = [Array]::IndexOf($componentNames, "Blue")
        Assert-Equal ($blueIndex + 1) ([Array]::IndexOf($componentNames, "SharedRed")) "Canonical red component was not emitted at its representative's position."
        Assert-True (-not (Test-JsonProperty -Object $result.Document.components.schemas -Name $longRedName)) "Non-representative red component remained."
        $outputProperties = $result.Document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        Assert-Equal "#/components/schemas/SharedUnion" $outputProperties.unionA.'$ref' "First wrapper ref was not relocated."
        Assert-Equal "#/components/schemas/SharedUnion" $outputProperties.unionB.'$ref' "Second wrapper ref was not relocated."
        $union = $result.Document.components.schemas.SharedUnion
        Assert-Sequence `
            @("#/components/schemas/SharedRed", "#/components/schemas/Blue") `
            @($union.oneOf.'$ref') `
            "Canonical wrapper branches changed order or destinations."
        Assert-Sequence `
            @("red", "blue") `
            @((Get-JsonProperties -Value $union.discriminator.mapping).Name) `
            "Wrapper mapping keys changed during deduplication."
        Assert-Sequence `
            @("#/components/schemas/SharedRed", "#/components/schemas/Blue") `
            @((Get-JsonProperties -Value $union.discriminator.mapping).Value) `
            "Wrapper mapping targets were not relocated."
        Assert-Equal 0 (New-TestDeduplicationAnalysis -Document $result.Document).DuplicateGroupCount "Wrapper deduplication left eligible groups."
    }

    # Persisted state is authoritative. Adding a new duplicate makes the mapping
    # stale and requires an explicit updater/review cycle. Removing only one
    # mapped member simulates an interrupted or hand-edited application and must
    # be rejected rather than guessed at.
    Invoke-Test "stale and partially applied mappings fail" {
        $source = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $mapping = New-TestDeduplicationMapping -Document $source
        $source.components.schemas["EarlierLeaf"] = Copy-JsonValue -Value $source.components.schemas.LeafA
        Assert-Throws {
            ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $source -Mapping $mapping
        } "*mapping is stale*"

        $partial = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
        $null = $partial.components.schemas.Remove("ExactB")
        $partial.paths["/test"].post.responses["200"].content["application/json"].schema.properties.exact.'$ref' = "#/components/schemas/ExactA"
        $partial.components.schemas.ParentA.properties.choice.oneOf[1].'$ref' = "#/components/schemas/ExactA"
        $partial.components.schemas.ParentB.properties.choice.oneOf[0].'$ref' = "#/components/schemas/ExactA"
        Assert-Throws {
            ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $partial -Mapping $mapping
        } "*partially deduplicated*"
    }

    # The first CLI pass verifies deterministic encoding as well as semantic
    # transformation. The second pass uses the collapsed artifact as both input
    # and output; recognizing that shape must yield a byte-for-byte no-op.
    Invoke-Test "CLI application is deterministic and byte-idempotent" {
        $source = Read-OpenApiDocument -Path $fixturePath
        $mapping = New-RenamedTestMapping -Document $source
        $mappingPath = Join-Path $temporaryDirectory "mapping.json"
        $outputPath = Join-Path $temporaryDirectory "deduplicated.openapi.json"
        $null = Write-OpenApiJsonFile -Document $mapping -Path $mappingPath
        $first = Invoke-OpenApiComponentDeduplicator `
            -InputPath $fixturePath `
            -MappingPath $mappingPath `
            -OutputPath $outputPath
        Assert-Equal 3 $first.RemovedComponentCount "First CLI application removed the wrong count."
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $hasBom = $firstBytes.Length -ge 3 -and $firstBytes[0] -eq 0xEF -and $firstBytes[1] -eq 0xBB -and $firstBytes[2] -eq 0xBF
        Assert-True (-not $hasBom) "Deduplicated output contains a UTF-8 BOM."
        Assert-Equal 10 $firstBytes[$firstBytes.Length - 1] "Deduplicated output lacks a trailing LF."

        $second = Invoke-OpenApiComponentDeduplicator `
            -InputPath $outputPath `
            -MappingPath $mappingPath `
            -OutputPath $outputPath
        Assert-Equal 0 $second.RemovedComponentCount "Second application removed components."
        Assert-Equal 0 $second.DuplicateGroupCount "Second application found duplicate groups."
        Assert-BytesEqual $firstBytes ([System.IO.File]::ReadAllBytes($outputPath)) "Second application changed bytes."
    }

    # CLI boundary failures should be actionable and occur before any output is
    # committed. Missing OpenAPI input and malformed mapping JSON exercise the
    # two independently resolved required inputs.
    Invoke-Test "missing directory and malformed mapping inputs fail clearly" {
        $mappingPath = Join-Path $temporaryDirectory "mapping.json"
        $null = Write-OpenApiJsonFile -Document (New-OpenApiComponentDeduplicationMapping) -Path $mappingPath
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicator `
                -InputPath (Join-Path $temporaryDirectory "missing.json") `
                -MappingPath $mappingPath `
                -OutputPath (Join-Path $temporaryDirectory "output.json")
        } "*cannot find path*"

        $invalidMappingPath = Join-Path $temporaryDirectory "invalid-mapping.json"
        [System.IO.File]::WriteAllText($invalidMappingPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicator `
                -InputPath $fixturePath `
                -MappingPath $invalidMappingPath `
                -OutputPath (Join-Path $temporaryDirectory "output.json")
        } "*Could not parse component deduplication mapping JSON*"
    }
}
finally {
    # GUID isolation permits concurrent runs; unconditional cleanup keeps failed
    # test runs from accumulating temporary mapping and output artifacts.
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiComponentDeduplicationTests
