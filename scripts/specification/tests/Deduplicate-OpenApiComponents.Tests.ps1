<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Deduplicate-OpenApiComponents.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot ".." "pipeline" "Generate-OpenApiComponentDefaultMap.ps1")
. (Join-Path $PSScriptRoot ".." "pipeline" "Deduplicate-OpenApiComponents.ps1")
. (Join-Path $PSScriptRoot "_OpenApiComponentDeduplicationTest.Helpers.ps1")
Initialize-OpenApiComponentDeduplicationTestState

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "deduplicate-components-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

function New-AcyclicDeduplicationTestDocument {
    $document = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
    $properties = $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
    $null = $properties.Remove("cycle")
    foreach ($name in @("CycleA", "CycleB", "CycleAncestor")) {
        $null = $document.components.schemas.Remove($name)
    }
    return $document
}

function New-EquivalentInheritanceFamiliesTestDocument {
    $document = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
    $document.paths["/test"].post.responses["200"].content["application/json"].schema = [ordered]@{
        oneOf = @(
            [ordered]@{ '$ref' = "#/components/schemas/Pet" },
            [ordered]@{ '$ref' = "#/components/schemas/OtherPet" }
        )
    }
    $document["components"] = [ordered]@{
        schemas = [ordered]@{
            Pet = [ordered]@{
                type = "object"
                properties = [ordered]@{ kind = [ordered]@{ type = "string" } }
                discriminator = [ordered]@{
                    propertyName = "kind"
                    mapping = [ordered]@{ cat = "#/components/schemas/Cat" }
                }
            }
            Cat = [ordered]@{
                type = "object"
                required = @("kind")
                properties = [ordered]@{
                    kind = [ordered]@{ type = "string"; enum = @("cat") }
                }
                allOf = @([ordered]@{ '$ref' = "#/components/schemas/Pet" })
            }
            OtherPet = [ordered]@{
                type = "object"
                properties = [ordered]@{ kind = [ordered]@{ type = "string" } }
                discriminator = [ordered]@{
                    propertyName = "kind"
                    mapping = [ordered]@{ cat = "#/components/schemas/OtherCat" }
                }
            }
            OtherCat = [ordered]@{
                type = "object"
                required = @("kind")
                properties = [ordered]@{
                    kind = [ordered]@{ type = "string"; enum = @("cat") }
                }
                allOf = @([ordered]@{ '$ref' = "#/components/schemas/OtherPet" })
            }
        }
    }
    return $document
}

function Get-IdentityGroupForMember {
    param([object]$Plan, [string]$Member)

    return @($Plan.NormalizedAnalysis.IdentityGroups | Where-Object { $_.Members -ccontains $Member })[0]
}

try {
    Invoke-Test "comprehensive defaults collapse duplicate trees and preserve singleton groups" {
        $document = New-AcyclicDeduplicationTestDocument
        $documentBefore = ConvertTo-Json -InputObject $document -Compress -Depth 100
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
            -Document $document `
            -DefaultMap $plan.DefaultMap

        Assert-Equal 4 $result.ComponentCount "Retained component count was incorrect."
        Assert-Equal 3 $result.RemovedComponentCount "Removal count was incorrect."
        Assert-Equal 4 $result.AppliedGroupCount "Logical group count was incorrect."
        Assert-Sequence @("LeafA", "ParentA", "ExactA", "NearMiss") @(
            (Get-JsonProperties -Value $result.Document.components.schemas).Name
        ) "Output component names were incorrect."
        Assert-Equal $documentBefore (ConvertTo-Json -InputObject $document -Compress -Depth 100) "Application mutated its source."
        Assert-OpenApiReferencesResolve -Document $result.Document
        Assert-AdditionalOpenApiReferencesResolve -Document $result.Document
    }

    Invoke-Test "singleton enums deduplicate references without changing required values" {
        $document = New-TestSingletonEnumComponents
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $document -DefaultMap $plan.DefaultMap
        Assert-Equal 3 $result.ComponentCount "Singleton enum deduplication retained the wrong count."
        Assert-Equal 1 $result.RemovedComponentCount "Singleton enum duplicate was not removed."
        $schema = $result.Document.paths["/test"].post.responses["200"].content["application/json"].schema
        Assert-Equal "#/components/schemas/ModeA" $schema.properties.second.'$ref' "Singleton reference did not follow its representative."
        Assert-Sequence @("first", "second") @($schema.required) "Required-property membership changed during enum deduplication."
        Assert-Sequence @("explicit") @($result.Document.components.schemas.ModeA.enum) "Singleton value changed."
        Assert-Equal "#/components/schemas/ModeImplicit" $schema.properties.different.'$ref' "Different singleton value was merged."
        Assert-Equal "#/components/schemas/ModeConstrained" $schema.properties.constrained.'$ref' "Different constraints were merged."
    }

    Invoke-Test "all pointer kinds and descendant suffixes relocate" {
        $document = New-AcyclicDeduplicationTestDocument
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $document -DefaultMap $plan.DefaultMap
        $properties = $result.Document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        Assert-Equal "#/components/schemas/LeafA/properties/value" $properties.descendantAlias.'$ref' "Descendant reference suffix was not preserved."
        Assert-Equal "#/components/schemas/LeafA" $properties.dynamicAlias.'$dynamicRef' "`$dynamicRef was not relocated."
        Assert-Equal "#/components/schemas/LeafA" $properties.recursiveAlias.'$recursiveRef' "`$recursiveRef was not relocated."
        Assert-Equal "#/components/schemas/ExactA" $properties.exact.'$ref' "Exact duplicate reference was not relocated."
    }

    Invoke-Test "equivalent inheritance families collapse bases and children together" {
        $document = New-EquivalentInheritanceFamiliesTestDocument
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        Assert-Equal 2 $plan.DefaultMap.groups.Count "Equivalent inheritance family group count was incorrect."
        Assert-Sequence @("Pet", "Cat") @($plan.DefaultMap.groups.id) "Inheritance family representatives were incorrect."
        Assert-Sequence @(
            @("Pet", "OtherPet"),
            @("Cat", "OtherCat")
        ) @($plan.DefaultMap.groups | ForEach-Object { ,@($_.members) }) "Inheritance family groups were incorrect."

        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
            -Document $document `
            -DefaultMap $plan.DefaultMap
        Assert-Equal 2 $result.ComponentCount "Deduplicated inheritance family component count was incorrect."
        Assert-Equal 2 $result.RemovedComponentCount "Deduplicated inheritance family removal count was incorrect."
        Assert-Sequence @("Pet", "Cat") @(
            (Get-JsonProperties -Value $result.Document.components.schemas).Name
        ) "Deduplicated inheritance family names were incorrect."
        Assert-Equal "#/components/schemas/Cat" $result.Document.components.schemas.Pet.discriminator.mapping.cat "Base mapping did not relocate to the retained child."
        Assert-Equal "#/components/schemas/Pet" $result.Document.components.schemas.Cat.allOf[0].'$ref' "Child inheritance did not relocate to the retained base."
        Assert-Sequence @(
            "#/components/schemas/Pet",
            "#/components/schemas/Pet"
        ) @($result.Document.paths["/test"].post.responses["200"].content["application/json"].schema.oneOf.'$ref') "External family references did not relocate."
        $null = Assert-OpenApiDiscriminatorMappings -Document $result.Document
    }

    Invoke-Test "nested hierarchies deduplicate without breaking either discriminator level" {
        $document = New-TestNestedInheritanceFamilies -AncestorReference
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $document -DefaultMap $plan.DefaultMap
        $schemas = $result.Document.components.schemas
        Assert-Equal 4 $result.ComponentCount "Nested family component count was incorrect."
        Assert-Equal 4 $result.RemovedComponentCount "Nested family removal count was incorrect."
        Assert-Equal "#/components/schemas/Message" $schemas.InputItem.discriminator.mapping.message "Outer mapping was broken."
        Assert-Equal "#/components/schemas/UserMessage" $schemas.Message.discriminator.mapping.user "Inner mapping was broken."
        Assert-Equal "#/components/schemas/InputItem" $schemas.Message.allOf[0].'$ref' "Intermediate inheritance was broken."
        Assert-Equal "#/components/schemas/Message" $schemas.UserMessage.allOf[0].'$ref' "Leaf inheritance was broken."
        Assert-Equal "#/components/schemas/InputItem" $schemas.UserMessage.properties.ancestor.'$ref' "Ancestor reference was broken."
        $null = Assert-OpenApiDiscriminatorMappings -Document $result.Document
        $secondPlan = New-OpenApiComponentDefaultMapPlan -Document $result.Document
        $second = ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $result.Document -DefaultMap $secondPlan.DefaultMap
        Assert-True (Test-JsonValuesEqual -Left $result.Document -Right $second.Document) "Nested deduplication was not a fixed point."
    }

    Invoke-Test "override and nested partial members roll up to the parent representative" {
        $document = New-AcyclicDeduplicationTestDocument
        $document.components.schemas["NearMissB"] = Copy-JsonValue -Value $document.components.schemas.NearMiss
        $properties = $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        $properties["near"] = [ordered]@{ '$ref' = "#/components/schemas/NearMissB" }
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $exactIdentity = Get-IdentityGroupForMember -Plan $baseline -Member "ExactA"
        $nearIdentity = Get-IdentityGroupForMember -Plan $baseline -Member "NearMiss"
        $policy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "CountOverride"
                type = "override"
                representative = "ExactA"
                fingerprint = $exactIdentity.Fingerprint
                members = @("ExactA")
                overrides = @([ordered]@{
                    id = "NumberOverrides"
                    type = "duplicate"
                    representative = "NearMiss"
                    fingerprint = $nearIdentity.Fingerprint
                    members = @("NearMiss")
                })
            })
        }
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policy
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
            -Document $document `
            -PolicyMap $policy `
            -DefaultMap $plan.DefaultMap

        Assert-True (Test-JsonProperty -Object $result.Document.components.schemas -Name "CountOverride") "Override output was not emitted."
        Assert-True (-not (Test-JsonProperty -Object $result.Document.components.schemas -Name "NumberOverrides")) "Nested target was emitted as an output."
        Assert-Equal "integer" $result.Document.components.schemas.CountOverride.properties.count.type "Override did not retain the parent representative schema."
        $outputProperties = $result.Document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
        Assert-Equal "#/components/schemas/CountOverride" $outputProperties.exact.'$ref' "Override-primary member did not relocate to parent output."
        Assert-Equal "#/components/schemas/CountOverride" $outputProperties.near.'$ref' "Nested partial member did not relocate to parent output."
    }

    Invoke-Test "policy may preserve identical schemas as separate outputs" {
        $document = New-AcyclicDeduplicationTestDocument
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $leafIdentity = Get-IdentityGroupForMember -Plan $baseline -Member "LeafA"
        $policy = [ordered]@{
            version = [long]1
            groups = @(
                [ordered]@{
                    id = "FirstLeaf"
                    type = "duplicate"
                    representative = "LeafA"
                    fingerprint = $leafIdentity.Fingerprint
                    members = @("LeafA")
                },
                [ordered]@{
                    id = "SecondLeaf"
                    type = "duplicate"
                    representative = "LeafB"
                    fingerprint = $leafIdentity.Fingerprint
                    members = @("LeafB")
                }
            )
        }
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policy
        $result = ConvertTo-DeduplicatedOpenApiComponentsDocument `
            -Document $document `
            -PolicyMap $policy `
            -DefaultMap $plan.DefaultMap

        Assert-True (Test-JsonProperty -Object $result.Document.components.schemas -Name "FirstLeaf") "First policy output was not emitted."
        Assert-True (Test-JsonProperty -Object $result.Document.components.schemas -Name "SecondLeaf") "Second policy output was not emitted."
        Assert-Equal 1 (Get-OpenApiComponentDeduplicationAnalysis -Document $result.Document).DuplicateGroupCount "Separated policy outputs were unexpectedly merged."
    }

    Invoke-Test "edited or stale default map is rejected" {
        $document = New-AcyclicDeduplicationTestDocument
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $plan.DefaultMap.groups[0].id = "EditedDefault"
        Assert-Throws {
            ConvertTo-DeduplicatedOpenApiComponentsDocument -Document $document -DefaultMap $plan.DefaultMap
        } "*default map is stale or was edited*"
    }

    Invoke-Test "CLI application is deterministic and byte-idempotent" {
        $inputPath = Join-Path $temporaryDirectory "source.componentized.openapi.json"
        $defaultMapPath = Join-Path $temporaryDirectory "responses.component-default-map.json"
        $policyMapPath = Join-Path $temporaryDirectory "responses.component-policy-map.json"
        $outputPath = Join-Path $temporaryDirectory "responses.componentized.deduplicated.openapi.json"
        $document = New-AcyclicDeduplicationTestDocument
        $null = Write-OpenApiJsonFile -Document $document -Path $inputPath
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        $null = Write-OpenApiJsonFile -Document $plan.DefaultMap -Path $defaultMapPath

        $first = Invoke-OpenApiComponentDeduplicator `
            -InputPath $inputPath `
            -DefaultMapPath $defaultMapPath `
            -PolicyMapPath $policyMapPath `
            -OutputPath $outputPath
        Assert-Equal 3 $first.RemovedComponentCount "First CLI application removed the wrong count."
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $second = Invoke-OpenApiComponentDeduplicator `
            -InputPath $outputPath `
            -DefaultMapPath $defaultMapPath `
            -PolicyMapPath $policyMapPath `
            -OutputPath $outputPath
        Assert-Equal 0 $second.RemovedComponentCount "Second application removed components."
        Assert-BytesEqual $firstBytes ([System.IO.File]::ReadAllBytes($outputPath)) "Second application changed bytes."
    }

    Invoke-Test "missing input and malformed maps fail clearly" {
        $defaultMapPath = Join-Path $temporaryDirectory "valid-default-map.json"
        $null = Write-OpenApiJsonFile -Document (New-OpenApiComponentDefaultMap) -Path $defaultMapPath
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicator `
                -InputPath (Join-Path $temporaryDirectory "missing.json") `
                -DefaultMapPath $defaultMapPath `
                -PolicyMapPath (Join-Path $temporaryDirectory "absent-policy.json") `
                -OutputPath (Join-Path $temporaryDirectory "output.json")
        } "*cannot find path*"

        $invalidDefaultPath = Join-Path $temporaryDirectory "invalid-default-map.json"
        [System.IO.File]::WriteAllText($invalidDefaultPath, "{ invalid", [System.Text.UTF8Encoding]::new($false))
        $inputPath = Join-Path $temporaryDirectory "valid-source.json"
        $null = Write-OpenApiJsonFile -Document (New-AcyclicDeduplicationTestDocument) -Path $inputPath
        Assert-Throws {
            Invoke-OpenApiComponentDeduplicator `
                -InputPath $inputPath `
                -DefaultMapPath $invalidDefaultPath `
                -PolicyMapPath (Join-Path $temporaryDirectory "absent-policy.json") `
                -OutputPath (Join-Path $temporaryDirectory "output.json")
        } "*Could not parse component default map JSON*"
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiComponentDeduplicationTests
