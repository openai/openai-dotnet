<#
.SYNOPSIS
    Runs focused tests for the discriminated-inheritance refactor stage.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

. (Join-Path $PSScriptRoot ".." "pipeline" "Refactor-OpenApiDiscriminatedInheritance.ps1")
. (Join-Path $PSScriptRoot "_OpenApiTestFixture.Helpers.ps1")

$script:PassedTests = 0
$script:FailedTests = 0
$script:Failures = [System.Collections.Generic.List[string]]::new()

function Invoke-Test {
    param([string]$Name, [scriptblock]$Test)
    try {
        & $Test
        $script:PassedTests++
        Write-Host "[PASS] $Name"
    }
    catch {
        $script:FailedTests++
        $message = "[FAIL] $Name`: $($_.Exception.Message)"
        $script:Failures.Add($message)
        Write-Host $message -ForegroundColor Red
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Assert-Equal {
    param([AllowNull()][object]$Expected, [AllowNull()][object]$Actual, [string]$Message)
    if ($Expected -ne $Actual) { throw "$Message Expected '$Expected', got '$Actual'." }
}

function Assert-Sequence {
    param([object[]]$Expected, [object[]]$Actual, [string]$Message)
    $expectedJson = ConvertTo-Json -InputObject @($Expected) -Compress -Depth 100
    $actualJson = ConvertTo-Json -InputObject @($Actual) -Compress -Depth 100
    if (-not [string]::Equals($expectedJson, $actualJson, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$expectedJson', got '$actualJson'."
    }
}

function Assert-Throws {
    param([scriptblock]$Action, [string]$MessagePattern)
    try { & $Action }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

function Assert-BytesEqual {
    param([byte[]]$Expected, [byte[]]$Actual, [string]$Message)
    if (-not [System.Linq.Enumerable]::SequenceEqual($Expected, $Actual)) {
        throw $Message
    }
}

function New-InheritanceTestDocument {
    param(
        [Parameter()]
        [switch]$Singleton
    )

    $schemas = [ordered]@{
        Pet = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{
                    cat = "#/components/schemas/Cat"
                }
            }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Cat" })
        }
        Cat = [ordered]@{
            type = "object"
            required = @("kind", "meow")
            properties = [ordered]@{
                kind = [ordered]@{ type = "string"; enum = @("cat") }
                meow = [ordered]@{ type = "integer"; format = "int32" }
            }
        }
    }
    if (-not $Singleton) {
        $schemas.Pet.discriminator.mapping["dog"] = "#/components/schemas/Dog"
        $schemas.Pet.oneOf += [ordered]@{ '$ref' = "#/components/schemas/Dog" }
        $schemas["Dog"] = [ordered]@{
            type = "object"
            required = @("kind", "bark")
            properties = [ordered]@{
                kind = [ordered]@{ type = "string"; enum = @("dog") }
                bark = [ordered]@{ type = "string" }
            }
        }
    }
    return [ordered]@{
        openapi = "3.1.0"
        info = [ordered]@{ title = "test"; version = "1.0.0" }
        paths = [ordered]@{}
        components = [ordered]@{ schemas = $schemas }
    }
}

function New-HierarchyInheritanceTestDocument {
    $cases = Read-TestJsonFixture -RelativePath "Add-OpenApiOneOfDiscriminators/cases.schema.json"
    $schema = Get-TestFixtureCase -Cases $cases -Name "nestedHierarchy"
    $plan = Get-OpenApiInlineDiscriminatorUnionPlan -Schema $schema -PropertyName "type" -Breadcrumb "#" -RequireDiscriminator
    $hierarchy = ConvertTo-NormalizedOpenApiDiscriminatorUnionSchema -Plan $plan
    $schemas = [ordered]@{
        InputItem = Copy-JsonValue -Value $hierarchy
        Message = Copy-JsonValue -Value $hierarchy.oneOf[0]
        UserMessage = Copy-JsonValue -Value $hierarchy.oneOf[0].oneOf[0]
        AssistantMessage = Copy-JsonValue -Value $hierarchy.oneOf[0].oneOf[1]
        FunctionCall = Copy-JsonValue -Value $hierarchy.oneOf[1]
    }
    $schemas.InputItem.oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Message" }, [ordered]@{ '$ref' = "#/components/schemas/FunctionCall" })
    $schemas.InputItem.discriminator["mapping"] = [ordered]@{ message = "#/components/schemas/Message"; function_call = "#/components/schemas/FunctionCall" }
    $schemas.Message.oneOf = @([ordered]@{ '$ref' = "#/components/schemas/UserMessage" }, [ordered]@{ '$ref' = "#/components/schemas/AssistantMessage" })
    $schemas.Message.discriminator["mapping"] = [ordered]@{ user = "#/components/schemas/UserMessage"; assistant = "#/components/schemas/AssistantMessage" }
    $document = New-TestOpenApiDocument -Schema ([ordered]@{ '$ref' = "#/components/schemas/InputItem" })
    $document["components"] = [ordered]@{ schemas = $schemas }
    return $document
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "refactor-discriminated-inheritance-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)

try {
    Invoke-Test "basic discriminated union becomes base and derived schemas" {
        $document = New-InheritanceTestDocument
        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        $schemas = $result.Document.components.schemas
        Assert-Equal 1 $result.RefactoredParentCount "Parent count was incorrect."
        Assert-Equal 2 $result.InheritedChildCount "Child count was incorrect."
        Assert-True (-not $schemas.Pet.Contains("oneOf")) "Base retained oneOf."
        Assert-Equal "object" $schemas.Pet.type "Base type was incorrect."
        Assert-Sequence @("kind") @($schemas.Pet.properties.Keys) "Base properties were incorrect."
        Assert-Equal "string" $schemas.Pet.properties.kind.type "Base discriminator property type was incorrect."
        Assert-Sequence @("type") @($schemas.Pet.properties.kind.Keys) "Base discriminator property retained generated metadata."
        Assert-Sequence @("kind") @($schemas.Pet.required) "Base did not require the discriminator property."
        Assert-Equal "#/components/schemas/Cat" $schemas.Pet.discriminator.mapping.cat "Cat mapping changed."
        Assert-Equal "#/components/schemas/Dog" $schemas.Pet.discriminator.mapping.dog "Dog mapping changed."
        Assert-Equal "#/components/schemas/Pet" $schemas.Cat.allOf[0].'$ref' "Cat did not inherit Pet."
        Assert-Equal "#/components/schemas/Pet" $schemas.Dog.allOf[0].'$ref' "Dog did not inherit Pet."
        Assert-Sequence @("cat") @($schemas.Cat.properties.kind.enum) "Cat discriminator value changed."
        Assert-Sequence @("kind", "meow") @($schemas.Cat.required) "Cat requirements changed."
        $validation = Assert-OpenApiDiscriminatorMappings -Document $result.Document
        Assert-Equal 1 $validation.DiscriminatorCount "Inherited discriminator was not validated."
    }

    Invoke-Test "singleton discriminators use the same inheritance representation" {
        $document = New-InheritanceTestDocument -Singleton
        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        Assert-Equal 1 $result.RefactoredParentCount "Singleton parent was not transformed."
        Assert-Equal 1 $result.InheritedChildCount "Singleton child count was incorrect."
        Assert-True (-not $result.Document.components.schemas.Pet.Contains("oneOf")) "Singleton base retained oneOf."
        Assert-Equal "#/components/schemas/Pet" $result.Document.components.schemas.Cat.allOf[0].'$ref' "Singleton child did not inherit."
    }

    Invoke-Test "intermediate discriminator bases inherit their outer parent and retain inner routing" {
        $document = New-HierarchyInheritanceTestDocument
        $before = ConvertTo-Json -InputObject $document -Compress -Depth 100
        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        $schemas = $result.Document.components.schemas
        Assert-Equal 2 $result.RefactoredParentCount "Nested discriminator parent count was incorrect."
        Assert-Equal 4 $result.InheritedChildCount "Nested inheritance edge count was incorrect."
        Assert-Equal "#/components/schemas/InputItem" $schemas.Message.allOf[0].'$ref' "Message did not inherit InputItem."
        Assert-Equal "#/components/schemas/Message" $schemas.UserMessage.allOf[0].'$ref' "UserMessage did not inherit Message."
        Assert-Equal "#/components/schemas/Message" $schemas.AssistantMessage.allOf[0].'$ref' "AssistantMessage did not inherit Message."
        Assert-Equal "#/components/schemas/InputItem" $schemas.FunctionCall.allOf[0].'$ref' "FunctionCall did not inherit InputItem."
        Assert-Sequence @("message") @($schemas.Message.properties.type.enum) "Message lost its outer discriminator value."
        Assert-Sequence @("type", "role") @($schemas.Message.required) "Message did not require both discriminator properties."
        Assert-Equal "role" $schemas.Message.discriminator.propertyName "Message lost its role discriminator."
        Assert-Equal "#/components/schemas/Message" $schemas.InputItem.discriminator.mapping.message "Outer mapping bypassed the intermediate base."
        Assert-Equal "#/components/schemas/UserMessage" $schemas.Message.discriminator.mapping.user "Inner mapping changed."
        Assert-Equal $before (ConvertTo-Json -InputObject $document -Compress -Depth 100) "Inheritance mutated its source."
        $null = Assert-OpenApiDiscriminatorMappings -Document $result.Document
        $second = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $result.Document
        Assert-Equal 0 $second.RefactoredParentCount "Second pass refactored an inherited intermediate base."
        Assert-True (Test-JsonValuesEqual -Left $result.Document -Right $second.Document) "Second pass changed the nested hierarchy."
    }

    Invoke-Test "deeper discriminator chains preserve one immediate base at every level" {
        $document = New-HierarchyInheritanceTestDocument
        $schemas = $document.components.schemas
        $assistant = Copy-JsonValue -Value $schemas.AssistantMessage
        $schemas.AssistantMessage = [ordered]@{
            type = "object"
            required = @("role")
            properties = [ordered]@{ role = [ordered]@{ type = "string"; enum = @("assistant") } }
            discriminator = [ordered]@{ propertyName = "phase"; mapping = [ordered]@{ draft = "#/components/schemas/AssistantDraft"; final = "#/components/schemas/AssistantFinal" } }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/AssistantDraft" }, [ordered]@{ '$ref' = "#/components/schemas/AssistantFinal" })
        }
        foreach ($phase in @("draft", "final")) {
            $leaf = Copy-JsonValue -Value $assistant
            $leaf.required = [object[]]@($leaf.required + "phase")
            $leaf.properties["phase"] = [ordered]@{ type = "string"; enum = @($phase) }
            $name = if ($phase -ceq "draft") { "AssistantDraft" } else { "AssistantFinal" }
            $schemas[$name] = $leaf
        }
        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        $schemas = $result.Document.components.schemas
        Assert-Equal 3 $result.RefactoredParentCount "Deep hierarchy lost a discriminator base."
        Assert-Equal "#/components/schemas/Message" $schemas.AssistantMessage.allOf[0].'$ref' "Assistant base skipped Message."
        Assert-Equal "#/components/schemas/AssistantMessage" $schemas.AssistantDraft.allOf[0].'$ref' "Draft skipped its immediate base."
        Assert-Equal "#/components/schemas/AssistantMessage" $schemas.AssistantFinal.allOf[0].'$ref' "Final skipped its immediate base."
        Assert-Sequence @("assistant") @($schemas.AssistantMessage.properties.role.enum) "Assistant base lost its parent's discriminator value."
        Assert-Equal "phase" $schemas.AssistantMessage.discriminator.propertyName "Assistant base lost its own discriminator."
        $second = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $result.Document
        Assert-True (Test-JsonValuesEqual -Left $result.Document -Right $second.Document) "Deep hierarchy was not a fixed point."
    }

    Invoke-Test "shared nested bases and inheritance cycles fail explicitly" {
        $shared = New-HierarchyInheritanceTestDocument
        $shared.components.schemas["OtherInputItem"] = Copy-JsonValue -Value $shared.components.schemas.InputItem
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $shared
        } "*Message*multiple inheritance parents*shared nested bases*"

        $cycle = New-HierarchyInheritanceTestDocument
        $cycle.components.schemas.InputItem["properties"] = [ordered]@{ role = [ordered]@{ type = "string"; enum = @("user") } }
        $cycle.components.schemas.InputItem["required"] = @("role")
        $cycle.components.schemas.Message.oneOf[0].'$ref' = "#/components/schemas/InputItem"
        $cycle.components.schemas.Message.discriminator.mapping.user = "#/components/schemas/InputItem"
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $cycle
        } "*inheritance cycle*"
    }

    Invoke-Test "shared leaves below a nested base still receive parent-specific clones" {
        $document = New-HierarchyInheritanceTestDocument
        $document.components.schemas["UserResult"] = [ordered]@{
            discriminator = [ordered]@{ propertyName = "role"; mapping = [ordered]@{ user = "#/components/schemas/UserMessage" } }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/UserMessage" })
        }
        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        $schemas = $result.Document.components.schemas
        Assert-Equal 2 $result.ClonedChildCount "Shared nested leaf was not cloned for each parent."
        $messageChild = $schemas.Message.discriminator.mapping.user.Substring("#/components/schemas/".Length)
        $resultChild = $schemas.UserResult.discriminator.mapping.user.Substring("#/components/schemas/".Length)
        Assert-Equal "#/components/schemas/Message" $schemas[$messageChild].allOf[0].'$ref' "Message leaf clone inherited the wrong base."
        Assert-Equal "#/components/schemas/UserResult" $schemas[$resultChild].allOf[0].'$ref' "Result leaf clone inherited the wrong base."
        Assert-Equal "#/components/schemas/InputItem" $schemas.Message.allOf[0].'$ref' "Shared-leaf cloning broke the intermediate base."
        Assert-Sequence @("message") @($schemas[$messageChild].properties.type.enum) "Nested leaf clone lost its outer literal."
    }

    Invoke-Test "intermediate bases do not silently discard payload constraints" {
        $document = New-HierarchyInheritanceTestDocument
        $document.components.schemas.Message.properties["content"] = [ordered]@{ type = "string" }
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        } "*Message*unsupported property 'content'*"
    }

    Invoke-Test "shared children are cloned once per parent and external refs keep the original" {
        $document = New-InheritanceTestDocument -Singleton
        $document.components.schemas["SearchResult"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{ cat = "#/components/schemas/Cat" }
            }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Cat" })
        }
        $document.paths["/cats"] = [ordered]@{
            get = [ordered]@{
                responses = [ordered]@{
                    "200" = [ordered]@{
                        description = "ok"
                        content = [ordered]@{
                            "application/json" = [ordered]@{
                                schema = [ordered]@{ '$ref' = "#/components/schemas/Cat" }
                            }
                        }
                    }
                }
            }
        }

        $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        $schemas = $result.Document.components.schemas
        Assert-Equal 2 $result.ClonedChildCount "Shared child clone count was incorrect."
        Assert-True ($schemas.Contains("Cat")) "Externally referenced original child was removed."
        $petClone = $schemas.Pet.discriminator.mapping.cat.Substring("#/components/schemas/".Length)
        $searchClone = $schemas.SearchResult.discriminator.mapping.cat.Substring("#/components/schemas/".Length)
        Assert-True (-not [string]::Equals($petClone, $searchClone, [System.StringComparison]::Ordinal)) "Parents reused one clone."
        Assert-Equal "#/components/schemas/Pet" $schemas[$petClone].allOf[0].'$ref' "Pet clone inherited the wrong base."
        Assert-Equal "#/components/schemas/SearchResult" $schemas[$searchClone].allOf[0].'$ref' "Search clone inherited the wrong base."
        Assert-Equal "#/components/schemas/Cat" $result.Document.paths["/cats"].get.responses["200"].content["application/json"].schema.'$ref' "External reference was rewritten."

        $unreferenced = New-InheritanceTestDocument -Singleton
        $unreferenced.components.schemas["SearchResult"] = [ordered]@{
            discriminator = [ordered]@{
                propertyName = "kind"
                mapping = [ordered]@{ cat = "#/components/schemas/Cat" }
            }
            oneOf = @([ordered]@{ '$ref' = "#/components/schemas/Cat" })
        }
        $unreferencedResult = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $unreferenced
        Assert-Equal 1 $unreferencedResult.RemovedSharedChildCount "Unreferenced shared child removal count was incorrect."
        Assert-True (-not $unreferencedResult.Document.components.schemas.Contains("Cat")) "Unreferenced original shared child remained."
    }

    Invoke-Test "unsupported parent siblings and existing child allOf fail clearly" {
        $document = New-InheritanceTestDocument -Singleton
        $document.components.schemas.Pet["description"] = "unsupported"
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        } "*Pet*unsupported sibling 'description'*"

        $document = New-InheritanceTestDocument -Singleton
        $document.components.schemas.Cat["allOf"] = @([ordered]@{ type = "object" })
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        } "*Cat*already declares allOf*"
    }

    Invoke-Test "incomplete mappings and mismatched discriminator values fail clearly" {
        $document = New-InheritanceTestDocument
        $null = $document.components.schemas.Pet.discriminator.mapping.Remove("dog")
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        } "*incomplete or noncanonical discriminator mappings*"

        $document = New-InheritanceTestDocument
        $document.components.schemas.Cat.properties.kind.enum[0] = "kitten"
        Assert-Throws {
            ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
        } "*mapping key 'cat'*not accepted*"
    }

    Invoke-Test "transformation is a structural fixed point" {
        $first = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document (New-InheritanceTestDocument)
        $second = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $first.Document
        Assert-Equal 0 $second.RefactoredParentCount "Second pass transformed inherited bases."
        Assert-True (Test-JsonValuesEqual -Left $first.Document -Right $second.Document) "Second pass changed the inherited document."
    }

    Invoke-Test "CLI transformation is byte-idempotent in place" {
        $inputPath = Join-Path $temporaryDirectory "sample.componentized.openapi.json"
        $outputPath = Join-Path $temporaryDirectory "sample.componentized.discriminator-inherited.openapi.json"
        $document = New-InheritanceTestDocument
        $document.paths["/test"] = [ordered]@{
            get = [ordered]@{
                responses = [ordered]@{
                    "200" = [ordered]@{ description = "ok" }
                }
            }
        }
        $null = Write-OpenApiJsonFile -Document $document -Path $inputPath
        $null = Invoke-OpenApiDiscriminatedInheritanceRefactor -InputPath $inputPath -OutputPath $outputPath
        [byte[]]$firstBytes = [System.IO.File]::ReadAllBytes($outputPath)
        $null = Invoke-OpenApiDiscriminatedInheritanceRefactor -InputPath $outputPath -OutputPath $outputPath
        Assert-BytesEqual $firstBytes ([System.IO.File]::ReadAllBytes($outputPath)) "Second CLI pass changed inherited bytes."
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Write-Host ""
Write-Host "Passed: $script:PassedTests"
Write-Host "Failed: $script:FailedTests"
if ($script:FailedTests -gt 0) {
    Write-Host ""
    foreach ($failure in $script:Failures) {
        Write-Host $failure -ForegroundColor Red
    }
    exit 1
}