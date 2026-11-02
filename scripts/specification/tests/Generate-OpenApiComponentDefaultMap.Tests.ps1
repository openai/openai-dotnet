<#
.SYNOPSIS
    Runs focused tests for scripts/specification/pipeline/Generate-OpenApiComponentDefaultMap.ps1.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot ".." "pipeline" "Generate-OpenApiComponentDefaultMap.ps1")
. (Join-Path $PSScriptRoot "_OpenApiComponentDeduplicationTest.Helpers.ps1")
Initialize-OpenApiComponentDeduplicationTestState

$cycleFixturePath = Get-TestDeduplicationFixturePath
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "generate-component-default-map-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

function New-AcyclicComponentMapTestDocument {
    $document = Read-TestJsonFixture -RelativePath "Deduplicate-OpenApiComponents/source.componentized.openapi.json"
    $properties = $document.paths["/test"].post.responses["200"].content["application/json"].schema.properties
    $null = $properties.Remove("cycle")
    foreach ($name in @("CycleA", "CycleB", "CycleAncestor")) {
        $null = $document.components.schemas.Remove($name)
    }
    return $document
}

function Get-TestIdentityGroup {
    param([object]$Plan, [string]$Member)

    return @($Plan.NormalizedAnalysis.IdentityGroups | Where-Object { $_.Members -ccontains $Member })[0]
}

try {
    Invoke-Test "fresh default map covers every acyclic component including singletons" {
        $document = New-AcyclicComponentMapTestDocument
        $plan = New-OpenApiComponentDefaultMapPlan -Document $document
        Assert-Equal 7 $plan.ComponentCount "Component count was incorrect."
        Assert-Equal 4 $plan.DefaultMap.groups.Count "Default group count was incorrect."
        Assert-Equal 1 $plan.SingletonDefaultGroupCount "Singleton group count was incorrect."
        Assert-Equal 3 $plan.RemovableComponentCount "Removal count was incorrect."
        Assert-Sequence @("LeafA", "ParentA", "ExactA", "NearMiss") @($plan.DefaultMap.groups.id) "Default group order was incorrect."
        Assert-Sequence @(
            @("LeafA", "LeafB"),
            @("ParentA", "ParentB"),
            @("ExactA", "ExactB"),
            @("NearMiss")
        ) @($plan.DefaultMap.groups | ForEach-Object { ,@($_.members) }) "Default group members were incorrect."
        foreach ($group in $plan.DefaultMap.groups) {
            Assert-Equal "duplicate" $group.type "Default group type was incorrect."
            Assert-True (-not (Test-JsonProperty -Object $group -Name "is_partial")) "Ordinary default group was marked partial."
        }
    }

    Invoke-Test "singleton enum groups preserve differences in values and constraints" {
        $plan = New-OpenApiComponentDefaultMapPlan -Document (New-TestSingletonEnumComponents)
        Assert-Equal 3 $plan.DefaultMap.groups.Count "Singleton enum identity groups were incorrect."
        $group = @($plan.DefaultMap.groups | Where-Object { $_.members -ccontains "ModeA" })[0]
        Assert-Sequence @("ModeA", "ModeB") @($group.members) "Equivalent singleton enums were not grouped together."
        foreach ($name in @("ModeImplicit", "ModeConstrained")) {
            $separate = @($plan.DefaultMap.groups | Where-Object { $_.members -ccontains $name })[0]
            Assert-Sequence @($name) @($separate.members) "Singleton enum '$name' was incorrectly merged."
        }
    }

    Invoke-Test "equivalent inheritance families share structural identities without cycles" {
        $document = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
        $document["components"] = [ordered]@{
            schemas = [ordered]@{
                Pet = [ordered]@{
                    type = "object"
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string" }
                    }
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
                    properties = [ordered]@{
                        kind = [ordered]@{ type = "string" }
                    }
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

        $analysis = Get-OpenApiComponentDeduplicationAnalysis -Document $document
        Assert-Equal 0 $analysis.CycleDependentComponents.Count "Inheritance metadata created a structural cycle."
        Assert-True ($analysis.Dependencies["Cat"] -ccontains "Pet") "Child allOf did not create a structural dependency."
        Assert-True (-not ($analysis.Dependencies["Pet"] -ccontains "Cat")) "Base mapping created a structural dependency."
        Assert-True (
            [string]::Equals(
                $analysis.Identities["Pet"].Fingerprint,
                $analysis.Identities["OtherPet"].Fingerprint,
                [System.StringComparison]::Ordinal
            )
        ) "Equivalent inheritance bases did not share a fingerprint."
        Assert-True (
            [string]::Equals(
                $analysis.Identities["Cat"].Fingerprint,
                $analysis.Identities["OtherCat"].Fingerprint,
                [System.StringComparison]::Ordinal
            )
        ) "Equivalent inherited children did not share a fingerprint."

        $document.components.schemas.OtherCat.properties["name"] = [ordered]@{ type = "string" }
        $differentAnalysis = Get-OpenApiComponentDeduplicationAnalysis -Document $document
        Assert-True (
            -not [string]::Equals(
                $differentAnalysis.Identities["Pet"].Fingerprint,
                $differentAnalysis.Identities["OtherPet"].Fingerprint,
                [System.StringComparison]::Ordinal
            )
        ) "Bases with structurally different mapped children shared a fingerprint."
        Assert-True (
            -not [string]::Equals(
                $differentAnalysis.Identities["Cat"].Fingerprint,
                $differentAnalysis.Identities["OtherCat"].Fingerprint,
                [System.StringComparison]::Ordinal
            )
        ) "Structurally different inherited children shared a fingerprint."
    }

    Invoke-Test "nested inheritance families fingerprint consistently across ancestor references" {
        foreach ($ancestorReference in @($false, $true)) {
            $document = New-TestNestedInheritanceFamilies -AncestorReference:$ancestorReference
            $plan = New-OpenApiComponentDefaultMapPlan -Document $document
            Assert-Equal 4 $plan.DefaultMap.groups.Count "Equivalent nested families did not produce four identity groups."
            foreach ($name in @("InputItem", "Message", "UserMessage", "AssistantMessage")) {
                $group = @($plan.DefaultMap.groups | Where-Object { $_.members -ccontains $name })[0]
                Assert-Sequence @($name, "Other$name") @($group.members) "Nested identity group '$name' was incorrect."
            }
            $document.components.schemas.OtherUserMessage.properties.content.type = "integer"
            $different = New-OpenApiComponentDefaultMapPlan -Document $document
            $rootGroup = @($different.DefaultMap.groups | Where-Object { $_.members -ccontains "InputItem" })[0]
            Assert-True ($rootGroup.members -cnotcontains "OtherInputItem") "Different nested payloads collapsed their root hierarchies."
            $messageGroup = @($different.DefaultMap.groups | Where-Object { $_.members -ccontains "Message" })[0]
            Assert-True ($messageGroup.members -cnotcontains "OtherMessage") "Different nested payloads collapsed their intermediate bases."
        }
    }

    Invoke-Test "file generation discards prior default edits and is deterministic" {
        $inputPath = Join-Path $temporaryDirectory "source.componentized.openapi.json"
        $defaultPath = Join-Path $temporaryDirectory "responses.component-default-map.json"
        $null = Write-OpenApiJsonFile -Document (New-AcyclicComponentMapTestDocument) -Path $inputPath
        [System.IO.File]::WriteAllText($defaultPath, "manually edited", [System.Text.UTF8Encoding]::new($false))

        $first = Invoke-OpenApiComponentDefaultMapGeneration `
            -InputPath $inputPath `
            -DefaultMapPath $defaultPath `
            -PolicyMapPath (Join-Path $temporaryDirectory "absent-policy.json")
        Assert-Equal 4 $first.DefaultGroupCount "Generated group count was incorrect."
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($defaultPath)
        $second = Invoke-OpenApiComponentDefaultMapGeneration `
            -InputPath $inputPath `
            -DefaultMapPath $defaultPath `
            -PolicyMapPath (Join-Path $temporaryDirectory "absent-policy.json")
        Assert-BytesEqual $firstBytes ([System.IO.File]::ReadAllBytes($defaultPath)) "Repeated generation changed default bytes."
        Assert-Equal 4 $second.DefaultGroupCount "Repeated generation changed group count."
    }

    Invoke-Test "top-level policy duplicate receives a same-id partial companion" {
        $document = New-AcyclicComponentMapTestDocument
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $leafIdentity = Get-TestIdentityGroup -Plan $baseline -Member "LeafA"
        $policy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "ReviewedLeaf"
                type = "duplicate"
                representative = "LeafA"
                fingerprint = $leafIdentity.Fingerprint
                members = @("LeafA")
            })
        }

        $plan = New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policy
        $partial = @($plan.DefaultMap.groups | Where-Object { $_.id -ceq "ReviewedLeaf" })[0]
        Assert-True ($null -ne $partial) "Policy partial companion was not generated."
        Assert-Equal $true $partial.is_partial "Policy companion was not marked partial."
        Assert-Sequence @("LeafB") @($partial.members) "Policy companion members were incorrect."
        $logical = @($plan.LogicalGroups | Where-Object { $_.Id -ceq "ReviewedLeaf" })[0]
        Assert-Equal "LeafA" $logical.Representative "Policy representative did not win after coalescing."
        Assert-Sequence @("LeafA", "LeafB") @($logical.Members) "Logical policy members were incorrect."
    }

    Invoke-Test "override primary and nested targets receive independent partial companions" {
        $document = New-AcyclicComponentMapTestDocument
        $document.components.schemas["NearMissB"] = Copy-JsonValue -Value $document.components.schemas.NearMiss
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $exactIdentity = Get-TestIdentityGroup -Plan $baseline -Member "ExactA"
        $nearIdentity = Get-TestIdentityGroup -Plan $baseline -Member "NearMiss"
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
        $primaryPartial = @($plan.DefaultMap.groups | Where-Object { $_.id -ceq "CountOverride" })[0]
        $nestedPartial = @($plan.DefaultMap.groups | Where-Object { $_.id -ceq "NumberOverrides" })[0]
        Assert-Sequence @("ExactB") @($primaryPartial.members) "Override-primary partial members were incorrect."
        Assert-Sequence @("NearMissB") @($nestedPartial.members) "Nested override partial members were incorrect."
        $logical = @($plan.LogicalGroups | Where-Object { $_.Id -ceq "CountOverride" })[0]
        Assert-Equal "ExactA" $logical.Representative "Override representative did not win."
        Assert-Sequence @("ExactA", "ExactB", "NearMiss", "NearMissB") @($logical.Members) "Override members did not roll up correctly."
    }

    Invoke-Test "policy normalization discovers transitive parent duplicates" {
        $document = New-AcyclicComponentMapTestDocument
        $document.components.schemas["IntegerParent"] = [ordered]@{
            type = "object"
            properties = [ordered]@{
                child = [ordered]@{ '$ref' = "#/components/schemas/ExactA" }
            }
        }
        $document.components.schemas["NumberParent"] = [ordered]@{
            type = "object"
            properties = [ordered]@{
                child = [ordered]@{ '$ref' = "#/components/schemas/NearMiss" }
            }
        }
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $exactIdentity = Get-TestIdentityGroup -Plan $baseline -Member "ExactA"
        $nearIdentity = Get-TestIdentityGroup -Plan $baseline -Member "NearMiss"
        Assert-True (
            -not [string]::Equals(
                $baseline.NormalizedAnalysis.Identities["IntegerParent"].Fingerprint,
                $baseline.NormalizedAnalysis.Identities["NumberParent"].Fingerprint,
                [System.StringComparison]::Ordinal
            )
        ) "Parent schemas were already equivalent before policy normalization."
        $policy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "CountOverride"
                type = "override"
                representative = "ExactA"
                fingerprint = $exactIdentity.Fingerprint
                members = @("ExactA", "ExactB")
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
        $parentGroup = @($plan.DefaultMap.groups | Where-Object { $_.members -ccontains "IntegerParent" })[0]
        Assert-Sequence @("IntegerParent", "NumberParent") @($parentGroup.members) "Policy-normalized parents were not grouped together."
    }

    Invoke-Test "new match against multiple policy targets fails explicitly" {
        $document = New-AcyclicComponentMapTestDocument
        $document.components.schemas["LeafC"] = Copy-JsonValue -Value $document.components.schemas.LeafA
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $leafIdentity = Get-TestIdentityGroup -Plan $baseline -Member "LeafA"
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

        Assert-Throws {
            New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policy
        } "*LeafC*multiple policy targets*FirstLeaf*SecondLeaf*"
    }

    Invoke-Test "invalid override shape and cycles fail before generation" {
        $document = New-AcyclicComponentMapTestDocument
        $baseline = New-OpenApiComponentDefaultMapPlan -Document $document
        $exactIdentity = Get-TestIdentityGroup -Plan $baseline -Member "ExactA"
        $policy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "InvalidOverride"
                type = "override"
                representative = "ExactA"
                fingerprint = $exactIdentity.Fingerprint
                members = @("ExactA")
                overrides = @([ordered]@{
                    id = "SameShape"
                    type = "duplicate"
                    representative = "ExactB"
                    fingerprint = $exactIdentity.Fingerprint
                    members = @("ExactB")
                })
            })
        }
        Assert-Throws {
            New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $policy
        } "*fingerprint must differ*"

        $mismatchedPolicy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "InvalidDuplicate"
                type = "duplicate"
                representative = "ExactA"
                fingerprint = $exactIdentity.Fingerprint
                members = @("ExactA", "NearMiss")
            })
        }
        Assert-Throws {
            New-OpenApiComponentDefaultMapPlan -Document $document -PolicyMap $mismatchedPolicy
        } "*NearMiss*does not match fingerprint*InvalidDuplicate*"

        $cycleDocument = Read-OpenApiDocument -Path $cycleFixturePath
        Assert-Throws {
            New-OpenApiComponentDefaultMapPlan -Document $cycleDocument
        } "*does not support cycle-dependent components*CycleA*CycleB*CycleAncestor*"

        $inducedCycleDocument = Read-TestJsonFixture -RelativePath "_Shared/minimal.openapi.json"
        $inducedCycleDocument["components"] = [ordered]@{
            schemas = [ordered]@{
                A = [ordered]@{ type = "object"; properties = [ordered]@{ child = [ordered]@{ '$ref' = "#/components/schemas/B" } } }
                B = [ordered]@{ type = "object"; properties = [ordered]@{ child = [ordered]@{ '$ref' = "#/components/schemas/C" } } }
                C = [ordered]@{ type = "object"; properties = [ordered]@{ value = [ordered]@{ type = "string" } } }
            }
        }
        $inducedBaseline = New-OpenApiComponentDefaultMapPlan -Document $inducedCycleDocument
        $inducedPolicy = [ordered]@{
            version = [long]1
            groups = @([ordered]@{
                id = "CycleOverride"
                type = "override"
                representative = "A"
                fingerprint = $inducedBaseline.NormalizedAnalysis.Identities["A"].Fingerprint
                members = @("A")
                overrides = @([ordered]@{
                    id = "COverrides"
                    type = "duplicate"
                    representative = "C"
                    fingerprint = $inducedBaseline.NormalizedAnalysis.Identities["C"].Fingerprint
                    members = @("C")
                })
            })
        }
        Assert-Throws {
            New-OpenApiComponentDefaultMapPlan -Document $inducedCycleDocument -PolicyMap $inducedPolicy
        } "*policy normalization creates cycle-dependent components*A*B*"
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Complete-OpenApiComponentDeduplicationTests
