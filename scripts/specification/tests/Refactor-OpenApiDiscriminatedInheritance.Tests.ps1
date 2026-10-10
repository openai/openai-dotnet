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
        Assert-Equal "string" $schemas.Pet.properties.kind.type "Base selector type was incorrect."
        Assert-Sequence @("type") @($schemas.Pet.properties.kind.Keys) "Base selector retained generated metadata."
        Assert-Sequence @("kind") @($schemas.Pet.required) "Base did not require the selector."
        Assert-Equal "#/components/schemas/Cat" $schemas.Pet.discriminator.mapping.cat "Cat mapping changed."
        Assert-Equal "#/components/schemas/Dog" $schemas.Pet.discriminator.mapping.dog "Dog mapping changed."
        Assert-Equal "#/components/schemas/Pet" $schemas.Cat.allOf[0].'$ref' "Cat did not inherit Pet."
        Assert-Equal "#/components/schemas/Pet" $schemas.Dog.allOf[0].'$ref' "Dog did not inherit Pet."
        Assert-Sequence @("cat") @($schemas.Cat.properties.kind.enum) "Cat selector changed."
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

    Invoke-Test "incomplete mappings and mismatched selectors fail clearly" {
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